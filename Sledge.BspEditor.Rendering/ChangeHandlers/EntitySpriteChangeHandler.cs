using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.ChangeHandling;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
	[Export(typeof(IMapDocumentChangeHandler))]
	public class EntitySpriteChangeHandler : IMapDocumentChangeHandler
	{
		public string OrderHint => "M";
		private readonly Lazy<ResourceCollection> _resourceCollection;

		[ImportingConstructor]
		public EntitySpriteChangeHandler(
			[Import] Lazy<ResourceCollection> resourceCollection
		)
		{
			_resourceCollection = resourceCollection;
		}
		public async Task Changed(Change change)
		{
			var gd = await change.Document.Environment.GetGameData();
			var tc = await change.Document.Environment.GetTextureCollection();
			foreach (var entity in change.Added.Union(change.Updated).OfType<Entity>())
			{
				var sn = GetSpriteData(entity, gd);
				var es = entity.Data.GetOne<EntitySprite>();
				if(sn==null && es != null)
				{
					entity.Data.Remove(x => x == es);
					continue;
				}
				if (sn == null) continue;
				if (es != null &&
					es.Name == sn.Name &&
					es.Scale == sn.Scale &&
					es.Color == sn.Color &&
					es.Framerate == sn.Framerate && es.ContentsReplaced) continue;
				var sd = await CreateSpriteData(entity, change.Document, gd, tc, sn.Name);
				if (es != null)
					_resourceCollection.Value.DestroyModelRenderable(change.Document.Environment, es.Renderable);

				if (sd == null) entity.Data.Remove(x => x == es);
				else entity.Data.Replace(sd);
				entity.DescendantsChanged();
			}
			// Ensure removed entity renderable are disposed properly
			foreach (var rem in change.Removed)
			{
				var em = rem.Data.GetOne<EntitySprite>();
				if (em?.Renderable == null) continue;

				_resourceCollection.Value.DestroyModelRenderable(change.Document.Environment, em.Renderable);
				rem.Data.Remove(em);
			}
		}

		private async Task<EntitySprite> CreateSpriteData(Entity entity, MapDocument doc, GameData gd, TextureCollection tc, string name)
		{
			if (!tc.HasTexture(name)) return null;

			var texture = await tc.GetTextureItem(name);
			if (texture == null) return null;

			var cls = gd?.GetClass(entity.EntityData.Name);
			var scale = 1f;
			var color = Color.White;
			var framerate = 1;
			SizeF? size = new SizeF(entity.BoundingBox.Width, entity.BoundingBox.Height);

			if (cls != null)
			{
				if (cls.Properties.Any(x => String.Equals(x.Name, "scale", StringComparison.CurrentCultureIgnoreCase)))
				{
					scale = entity.EntityData.Get<float>("scale", 1);
					//I don't know why is that necessary, or not, so I'll just comment this since scales less .1 works fine (seems like that)
					//if (scale <= 0.1f) scale = 1;
				}

				var colProp = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Color255 || x.VariableType == VariableType.Color1);
				if (colProp != null)
				{
					var col = entity.EntityData.GetVector3(colProp.Name);
					if (colProp.VariableType == VariableType.Color255) col /= 255f;
					if (col.HasValue) color = col.Value.ToColor();
				}

				if (cls.Behaviours.Any(x => string.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase)))
				{
					size = texture.Size;
				}
				if (cls.Properties.Any(x => string.Equals(x.Name, "framerate", StringComparison.InvariantCultureIgnoreCase)))
				{
					framerate = (int)entity.EntityData.Get<float>("framerate", 1);
				}
			}
			var renderable = await _resourceCollection.Value.CreateSpriteRenderable(doc.Environment, name);

			return new EntitySprite(name, scale, color, size, framerate, renderable);
		}

		private static EntitySpriteData GetSpriteData(Entity entity, GameData gd)
		{
			if (entity.Hierarchy.HasChildren || String.IsNullOrWhiteSpace(entity.EntityData.Name)) return null;
			var cls = gd?.GetClass(entity.EntityData.Name);
			if (cls == null) return null;

			var spr = cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase))
					  ?? cls.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "iconsprite", StringComparison.InvariantCultureIgnoreCase));
			if (spr == null) return null;

			var result = new EntitySpriteData()
			{
				Name = GetName(),
				Scale = GetScale(),
				Color = GetColor(),
				Framerate = GetFramerate(),
			};

			return result;
			int GetFramerate()
			{
				var prop = cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "framerate", StringComparison.InvariantCultureIgnoreCase));
				if (prop != null)
				{
					return (int)entity.EntityData.Get(prop.Name, 1f);
				}
				return 1;
			}
			Color GetColor()
			{
				var colProp = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Color255 || x.VariableType == VariableType.Color1);
				if (colProp != null)
				{
					var col = entity.EntityData.GetVector3(colProp.Name);
					if (colProp.VariableType == VariableType.Color255) col /= 255f;
					if (col.HasValue) return col.Value.ToColor();
				}
				return Color.Black;
			}
			string GetName()
			{

				// First see if the studio behaviour forces a model...
				if (spr.Values.Count == 1 && !String.IsNullOrWhiteSpace(spr.Values[0]))
				{
					return spr.Values[0].Trim();
				}

				// Find the first property that is a studio type, or has a name of "sprite"...
				var prop = cls.Properties.FirstOrDefault(x => x.VariableType == VariableType.Sprite) ??
						   cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
				if (prop != null)
				{
					var val = entity.EntityData.Get(prop.Name, prop.DefaultValue);
					if (!String.IsNullOrWhiteSpace(val)) return val;
				}
				return null;
			}
			float GetScale()
			{
				var prop = cls.Properties.FirstOrDefault(x => String.Equals(x.Name, "scale", StringComparison.InvariantCultureIgnoreCase));
				if (prop != null)
				{
					return entity.EntityData.Get(prop.Name, 1f);
				}
				return 1f;
			}
		}
		private class EntitySpriteData
		{
			public string Name { get; set; }
			public float Scale { get; set; }
			public Color Color { get; set; }
			public int Framerate { get; set; }
		}
	}
}
