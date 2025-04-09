using LogicAndTrick.Oy;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Tools.Texture;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.DataStructures.Geometric;
using Sledge.Formats.Bsp.Lumps;
using Sledge.Rendering.Engine;
using Sledge.Shell.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Vortice.Mathematics;
using static System.Net.Mime.MediaTypeNames;

namespace Sledge.BspEditor.Tools.ShadowBake;
[Export(typeof(ISidebarComponent))]
[Export(typeof(IInitialiseHook))]
public partial class ShadowBakeTool : UserControl, ISidebarComponent, IInitialiseHook
{
	private WeakReference<MapDocument> _document = new WeakReference<MapDocument>(null);
	private const int LightMaxDistance = 5000;

	public string Title => "Bake Light";

	public object Control => this;

	public ShadowBakeTool()
	{
		CreateHandle();

		InitializeComponent();
	}

	private void SetDocument(IDocument document)
	{
		var md = document as MapDocument;
		_document = new WeakReference<MapDocument>(md);
	}

	public bool IsInContext(IContext context)
	{
		return context.TryGet("ActiveTool", out CameraTool _);
	}

	private async void BakeButton_Click(object sender, System.EventArgs e)
	{
		_document.TryGetTarget(out var doc);

		var solids = doc.Map.Root.Hierarchy.OfType<Solid>().ToList();
		var textureCollection = await doc.Environment.GetTextureCollection();
		var textures = solids.SelectMany(x => x.Faces).Select(f => f.Texture).DistinctBy(t => t.Name).ToList();


		var texturesCollection = await textureCollection.GetTextureItems(textures.Select(x => x.Name));

		var faces = solids.SelectMany(x => x.Faces).Where(x => !x.Texture.Name.Equals("sky")).ToList();
		var lightVectorRotation = Engine.Interface.GetLightAnglesRadians();
		Quaternion lightRotation = Quaternion.CreateFromYawPitchRoll(lightVectorRotation.X, lightVectorRotation.Y, lightVectorRotation.Z);
		Vector3 lightDirection = Vector3.Transform(-Vector3.UnitX, lightRotation);

		var iopt = MapObjectExtensions.IgnoreOptions.IgnoreClip | MapObjectExtensions.IgnoreOptions.IgnoreNull;
		var resources = new EngineInterface.DepthResource[faces.Count];
		var i = 0;
		var rand = new Random(DateTime.Now.Millisecond);
		var solids2 = doc.Map.Root.Collect(x => true, y => y is Solid).ToArray();

		foreach (var face in faces)
		{
			var texFile = texturesCollection.FirstOrDefault(t => t.Name.ToLower().Equals(face.Texture.Name.ToLower()));
			//var scaleX = MathF.Ceiling(face.Texture.XScale);
			//var scaleY = MathF.Ceiling(face.Texture.YScale);

			// IMPORTANT: Normalize UVs
			var uvs = face.GetTextureCoordinates(64, 64);
			var uvMax = uvs.Select(x => (MathF.Abs(x.Item2), MathF.Abs(x.Item3)));
			var uvMaxU = uvMax.Max(x => x.Item1);
			var uvMaxV = uvMax.Max(x => x.Item2);
			var normalizedUvs = uvs.Select(x => new Vector2(x.Item2 / uvMaxU, x.Item3 / uvMaxV)).ToArray();
			face.Uv1 = normalizedUvs;

			uint width = (uint)MathF.Ceiling((texFile?.Width ?? 256));// * face.Texture.XScale);
			uint height = (uint)MathF.Ceiling((texFile?.Height ?? 256));// * face.Texture.YScale);
			width = width < 8 ? 8 : width;
			height = height < 8 ? 8 : height;
			width = 64;
			height = 64;

			var resource = Engine.Interface.CreateDepthTexture(width, height);
			face.LightMap = resource.Texture;
			var rs = new float[width * height];
			var w = 0;
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var worldPosition = face.ProjectedUVtoWorld((float)x / width, (float)y / height);

					var projection = worldPosition;
					var line = new Line(projection, projection - (lightDirection.Normalise() * LightMaxDistance));
					resource.MappedResource[w] = 1f;
					foreach (var solid in solids)
					{
						if (solid.BoundingBox.IntersectsWith(line))
						{
							var intersect = solid.GetIntersectionPoint(line);
							if (intersect.HasValue)
							{
								resource.MappedResource[w] = 0.5f;

								break;
							}
						}
					}
					rs[w] = resource.MappedResource[w];
					w++;
				}
			}

			resources[i] = resource;
			var rss = rs.Distinct();
			if (rss.Count() > 1)
			{
				Log.Debug("1", "2");
			}
			i++;
		}
		Engine.Interface.CopyDepthTexture(resources);
	}

	public Task OnInitialise()
	{
		Oy.Subscribe<IDocument>("Document:Activated", SetDocument);
		return Task.FromResult(0);
	}
}