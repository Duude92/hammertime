using System.ComponentModel.Composition;
using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture.Spr;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.ChangeHandlers
{
	public class EntitySprite : IMapObjectData, IContentsReplaced, IBoundingBoxProvider
	{
		public string Name { get; set; }
		public float Scale { get; }
		public Color Color { get; set; }
		public SizeF Size { get; }
		public int Framerate => Renderable.Framerate;
		public SpriteRenderable Renderable { get; }

		public bool ContentsReplaced => Renderable.Model != null;

		public EntitySprite(string name, float scale, Color color, SizeF? size, int framerate, IModelRenderable renderable)
		{
			Name = name;
			Scale = scale;
			Color = color;
			Size = size ?? SizeF.Empty;
			if (renderable == null)
			{
				Renderable = new SpriteRenderable(null, null);
			}
			else
			{
				Renderable = renderable as SpriteRenderable;
			}
			Renderable.Framerate = framerate;
			Renderable.Scale = scale;
			Renderable.Tint = color.ToVector4();
		}

		public EntitySprite(SerialisedObject obj)
		{
			Name = obj.Get<string>("Name");
			Scale = obj.Get<float>("Scale");
			Color = obj.GetColor("Color");
			Size = new SizeF(obj.Get<float>("Width"), obj.Get<float>("Height"));
			Renderable = new SpriteRenderable(null, null);
		}

		[Export(typeof(IMapElementFormatter))]
		public class ActiveTextureFormatter : StandardMapElementFormatter<EntitySprite> { }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", Name);
			info.AddValue("Scale", Scale);
			info.AddValue("Color", Color);
			info.AddValue("Width", Size.Width);
			info.AddValue("Height", Size.Height);
		}

		public Box GetBoundingBox(IMapObject obj)
		{
			if (string.IsNullOrWhiteSpace(Name) || Size.IsEmpty) return null;
			var origin = obj.Data.GetOne<Origin>()?.Location ?? Vector3.Zero;
			var half = new Vector3(Size.Width, Size.Width, Size.Height) * Scale / 2;
			Renderable.Origin = origin;
			return new Box(origin - half, origin + half);
		}

		public IMapElement Copy(UniqueNumberGenerator numberGenerator)
		{
			return Clone();
		}

		public IMapElement Clone()
		{
			return new EntitySprite(Name, Scale, Color, Size, Renderable?.Framerate ?? 10, null);
		}

		public SerialisedObject ToSerialisedObject()
		{
			var so = new SerialisedObject(nameof(EntitySprite));
			so.Set(nameof(Name), Name);
			so.Set(nameof(Scale), Scale);
			so.SetColor(nameof(Color), Color);
			so.Set("Width", Size.Width);
			so.Set("Height", Size.Height);
			return so;
		}
	}
}