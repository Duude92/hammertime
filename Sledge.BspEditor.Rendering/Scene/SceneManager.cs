using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Environment.Goldsource;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.FileSystem;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Resources;
using Sledge.Shell.Registers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Sledge.BspEditor.Rendering.Scene
{
	/// <summary>
	/// The entry point for the rendering infrastructure.
	/// Handles when map documents are opened and closed, changed, and activated.
	/// </summary>
	[Export(typeof(IStartupHook))]
#if DEBUG_EXTRA
    [Export]
#endif
	public class SceneManager : IStartupHook
	{
		private readonly Lazy<MapObjectConverter> _converter;
		private readonly Lazy<EngineInterface> _engine;
		private readonly DocumentRegister _documentRegister;
		private readonly ResourceCollection _resourceCollection;

		private readonly object _lock = new object();
		private SceneBuilder _sceneBuilder;

		private WeakReference<MapDocument> _activeDocument = new WeakReference<MapDocument>(null);

		[ImportingConstructor]
		public SceneManager(
			[Import] Lazy<MapObjectConverter> converter,
			[Import] Lazy<EngineInterface> engine,
			[Import] DocumentRegister documentRegister,
			[Import] ResourceCollection resourceCollection
		)
		{
			_converter = converter;
			_engine = engine;
			_documentRegister = documentRegister;
			_resourceCollection = resourceCollection;
		}

		/// <inheritdoc />
		public Task OnStartup()
		{
			Oy.Subscribe<object>("SettingsChanged", SettingsChanged);
			Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated);
			Oy.Subscribe<IDocument>("Document:Closed", DocumentClosed);
			Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
			Oy.Subscribe<Change>("MapDocument:Changed:Early", DocumentChangedEarly);

			return Task.FromResult(0);
		}

		public List<List<BufferBuilder.BufferAllocation>> GetCurrentAllocationInformation()
		{
			return _sceneBuilder?.BufferBuilders.Select(x => x.AllocationInformation).ToList();
		}

		// Document events
		private async Task DocumentChangedEarly(Change change)
		{
			await LoadSkybox();
			await UpdateEnvironmentLight();

		}

		private async Task UpdateEnvironmentLight()
		{
			if (!_activeDocument.TryGetTarget(out var md)) return;
			var light = md.Map.Root.Hierarchy.OfType<Entity>().FirstOrDefault(Entity => Entity.EntityData.Name == "light_environment");
			if (light != null)
			{
				var data = light.EntityData;
				var angles = data.GetVector3("angles");
				if (!angles.HasValue)
				{
					// Probably somewhere there is function to convert string to Vector3, but I don't remember for sure.
					var gameData = await md.Environment.GetGameData();
					var light_default = gameData.Classes.FirstOrDefault(x => x.ClassType != ClassType.Base && (x.Name ?? "").ToLower() == "light_environment");
					var angleString = light_default.Properties.FirstOrDefault(x => x.Name == "angles").DefaultValue;
					var pitchString = light_default.Properties.FirstOrDefault(x => x.Name == "pitch")?.DefaultValue;
					var yawString = light_default.Properties.FirstOrDefault(x => x.Name == "angle")?.DefaultValue;

					var spl = (angleString ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					var resultAngle = Vector3.Zero;

					if (float.TryParse(spl[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
						&& float.TryParse(spl[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
						&& float.TryParse(spl[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
					{
						resultAngle = new Vector3(x, y, z);
					}
					if (!String.IsNullOrEmpty(pitchString))
						resultAngle.X = int.Parse(pitchString);
					if (!String.IsNullOrEmpty(yawString))
						resultAngle.Y = int.Parse(yawString);
					angles = resultAngle;
				}
				var anglesValue = angles.Value;
				var pitch = data.Get<float>("pitch", angles.Value.X);
				if (pitch != float.NaN)
					anglesValue.X = pitch;
				var yaw = data.Get<float>("angle", angles.Value.Y);
				if (yaw != float.NaN)
					anglesValue.Y = yaw;
				Engine.Interface.SetLightAngles(anglesValue);
			}
		}

		private async Task DocumentChanged(Change change)
		{
			if (_activeDocument.TryGetTarget(out var md) && change.Document == md)
			{
				if (change.AffectedData.Any(x => x.AffectsRendering))
				{
					await UpdateScene(change.Document, null);
				}
				else if (change.HasObjectChanges)
				{
					await UpdateScene(change.Document, change.Added.Union(change.Updated).Union(change.Removed));
				}
			}
		}

		private async Task SettingsChanged(object o)
		{
			var doc = _activeDocument.TryGetTarget(out var md) ? md : null;
			await UpdateScene(doc, null);
		}

		private async Task DocumentActivated(IDocument doc)
		{
			var md = doc as MapDocument;
			_activeDocument = new WeakReference<MapDocument>(md);
			await UpdateScene(md, null);
			await LoadSkybox();
			await UpdateEnvironmentLight();
		}
		private async Task LoadSkybox()
		{
			if (_activeDocument.TryGetTarget(out var md) && md?.Environment is GoldsourceEnvironment environment1)
			{
				var data = md.Map.Root.Data.Get<EntityData>().First();
				var dd = md.Map.Data.GetOne<DisplayData>() ?? new DisplayData();

				var skyname = data?.Get<string>("skyname", null);
				if (dd.SkyboxName == skyname) return;

				dd.SkyboxName = skyname;

				md.Map.Data.Replace(dd);

				var sky = environment1.GetSkyboxes().FirstOrDefault(x => x.Name == skyname);
				if (sky == null) return;

				UpdateScene(md, null);

				await _resourceCollection.UploadCubemap(sky.File as CompositeFile);

			}
		}

		private async Task DocumentClosed(IDocument doc)
		{
			var envs = _documentRegister.OpenDocuments.OfType<MapDocument>().Select(x => x.Environment).ToHashSet();
			_resourceCollection.DisposeOtherEnvironments(envs);
			if (_activeDocument.TryGetTarget(out var md) && md == doc)
			{
				await UpdateScene(null, null);
			}
		}

		// Scene handling

		private Task UpdateScene(MapDocument md, IEnumerable<IMapObject> affected)
		{
			var waitTask = Task.CompletedTask;
			lock (_lock)
			{
				if (_sceneBuilder == null)
				{
					_sceneBuilder = new SceneBuilder(_engine.Value);
					_engine.Value.Add(_sceneBuilder.SceneBuilderRenderable);
					affected = null;
				}

				using (_engine.Value.Pause())
				{
					if (affected == null || md == null)
					{
						foreach (var r in _sceneBuilder.GetAllRenderables())
						{
							_engine.Value.Remove(r);
							if (r is IUpdateable u) _engine.Value.Remove(u);
						}
						_sceneBuilder.Clear();
					}

					if (md != null)
					{
						var resourceCollector = new ResourceCollector();
						waitTask = _converter.Value.Convert(md, _sceneBuilder, affected, resourceCollector)
							.ContinueWith(t => {
								if (t.IsFaulted) { throw t.Exception; }
								return HandleResources(md.Environment, resourceCollector);
							});
						_converter.Value.ConvertSky(md, _sceneBuilder, resourceCollector).Wait();
					}
				}
			}

			return waitTask;
		}

		private async Task HandleResources(IEnvironment environment, ResourceCollector resources)
		{
			var add = resources.GetRenderablesToAdd().ToHashSet();
			var rem = resources.GetRenderablesToRemove().ToHashSet();

			foreach (var r in add) _engine.Value.Add(r);
			foreach (var r in add.OfType<IUpdateable>()) _engine.Value.Add(r);

			foreach (var r in rem.OfType<IUpdateable>()) _engine.Value.Remove(r);
			foreach (var r in rem) _engine.Value.Remove(r);



			await _resourceCollection.Upload(environment, resources);
		}
	}
}
