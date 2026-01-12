using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;

namespace HammerTime.Source.Providers.Model.Mdl44
{
	internal class SourceModelRenderable : IModelRenderable
	{
		private MdlModel _model;
		private DeviceBuffer _transformsBuffer;
		private ResourceSet _transformsResourceSet;
		private DeviceBuffer _frozenTransformsBuffer;
		private ResourceSet _frozenTransformsResourceSet;
		private Matrix4x4[] _transforms;


		public SourceModelRenderable(MdlModel model)
		{
			_model = model;
		}

		public IModel Model => _model;

		public Vector3 Origin { get; set; }
		public Vector3 Angles { get; set; }
		public int Sequence { get; set; }
		public VertexFlags Flags { get; set; }

		public void CreateResources(EngineInterface engine, RenderContext context)
		{
			_transformsBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 128, BufferUsage.UniformBuffer)
			);

			_transformsResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _transformsBuffer)
			);

			_frozenTransformsBuffer = context.Device.ResourceFactory.CreateBuffer(
				new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>() * 128, BufferUsage.UniformBuffer)
			);

			_frozenTransformsResourceSet = context.Device.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _frozenTransformsBuffer)
			);
			var rot = _model.Model.Bones[0].Data.rot;
			var transform = Matrix4x4.CreateFromYawPitchRoll(rot.Z, rot.Y, rot.X);
			_transforms = new Matrix4x4[1] { transform };
			//_transforms = _model.Model.Bones.Select(b => b.Data.quat).Select(x =>
			//{
			//	var a = x.X;
			//	x.X = x.Z;
			//	x.Z = a;
			//	return x;
			//}).Select(x => Matrix4x4.CreateFromQuaternion(x)).ToArray();
		}

		public void DestroyResources()
		{
			_transformsBuffer?.Dispose();
			_transformsResourceSet?.Dispose();
			_frozenTransformsBuffer?.Dispose();
			_frozenTransformsResourceSet?.Dispose();

			_transformsResourceSet = null;
			_transformsBuffer = null;
			_frozenTransformsResourceSet = null;
			_frozenTransformsBuffer = null;
		}

		public void Dispose()
		{
			return;
		}

		public (Vector3, Vector3) GetBoundingBox()
		{
			var (min, max) = _model.GetBoundingBox(Sequence, 0, 0);

			var tf = GetModelTransformation();
			var box = new Box(min, max);
			box = box.Transform(tf);

			return (box.Start, box.End);
		}

		public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
		{
			yield break;
		}

		public Matrix4x4 GetModelTransformation()
		{
			Matrix4x4 yawMatrix = Matrix4x4.CreateRotationY(Angles.X);
			Matrix4x4 pitchMatrix = Matrix4x4.CreateRotationX(Angles.Z);
			Matrix4x4 rollMatrix = Matrix4x4.CreateRotationZ(Angles.Y);

			Matrix4x4 rotationMatrix = pitchMatrix * yawMatrix * rollMatrix;
			return rotationMatrix * Matrix4x4.CreateTranslation(Origin);
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
		{
			if (_transformsResourceSet == null || _transformsBuffer == null) return;

			if (pipeline.Type == PipelineType.WireframeModel)
			{
				//if (_lastSequence != Sequence)
				//{
				//	var transforms = new Matrix4x4[128];
				//	_model.Model.GetTransforms(Sequence, 0, 0, ref transforms);
				cl.UpdateBuffer(_frozenTransformsBuffer, 0, _transforms);
				//	_lastSequence = Sequence;
				//}
				cl.SetGraphicsResourceSet(1, _frozenTransformsResourceSet);
			}
			else
			{
				cl.UpdateBuffer(_transformsBuffer, 0, _transforms);
				cl.SetGraphicsResourceSet(2, _transformsResourceSet);
			}

			_model.Render(context, pipeline, viewport, cl);
		}

		public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
		{
			//
		}

		public bool ShouldRender(IPipeline pipeline, IViewport viewport)
		{
			if (pipeline.Type == PipelineType.WireframeModel)
			{
				if (viewport.Camera.Type != CameraType.Orthographic) return false;
				if (viewport.Camera is OrthographicCamera oc && oc.Zoom < 0.25f) return false;
				return true;
			}
			else if (pipeline.Type == PipelineType.TexturedModel)
			{
				if (viewport.Camera.Type != CameraType.Perspective) return false;
				return true;
			}
			return false;
		}

		public void Update(long frame)
		{
			return;
		}
	}
}
