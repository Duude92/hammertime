using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Newtonsoft.Json.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using static System.Windows.Forms.AxHost;
using KeyboardState = Sledge.Shell.Input.KeyboardState;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
	public class RotateTransformHandle : BoxResizeHandle, ITransformationHandle
	{
		private readonly RotationOrigin _origin;
		private Vector3? _rotateStart;
		private Vector3? _rotateEnd;

		public string Name => "Rotate";

		public RotateTransformHandle(BoxDraggableState state, ResizeHandle handle, RotationOrigin origin) : base(state, handle)
		{
			_origin = origin;
		}

		protected override void SetCursorForHandle(MapViewport viewport, ResizeHandle handle)
		{
			var ct = ToolCursors.RotateCursor;
			viewport.Control.Cursor = ct;
		}

		public override void StartDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			_rotateStart = _rotateEnd = position;
			base.StartDrag(document, viewport, camera, e, position);
		}

		public override void Drag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 lastPosition, Vector3 position)
		{
			_rotateEnd = position;
		}

		public override void EndDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
		{
			var selection = document.Selection.Where(x => x is Sledge.BspEditor.Primitives.MapObjects.Entity).OfType<Primitives.MapObjects.Entity>();
			//if (document.Selection.Any() && document.Selection.First() is Sledge.BspEditor.Primitives.MapObjects.Entity entity && entity.EntityData.Properties.TryGetValue("angles", out var angleString))
			{
				var tsn = new Transaction();

				foreach (var entity in selection)
				{
					if (entity.EntityData.Properties.TryGetValue("angles", out var angleString))
					{

						var initialAngles = angleString.Split(' ');
						var initial = NumericsExtensions.Parse(initialAngles[0], initialAngles[1], initialAngles[2], System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);

						var origin = camera.ZeroUnusedCoordinate((_rotateStart.Value + _rotateEnd.Value) / 2);
						if (_origin != null) origin = _origin.Position;

						var forigin = camera.Flatten(origin);

						var origv = Vector3.Normalize(_rotateStart.Value - forigin);
						var newv = Vector3.Normalize(_rotateEnd.Value - forigin);

						Vector3 previousLocalRotationRadians = new Vector3(
							MathHelper.DegreesToRadians(initial.X),
							MathHelper.DegreesToRadians(initial.Y),
							MathHelper.DegreesToRadians(initial.Z));
						Matrix4x4 yawMatrix = Matrix4x4.CreateRotationY(-previousLocalRotationRadians.X);
						Matrix4x4 pitchMatrix = Matrix4x4.CreateRotationX(previousLocalRotationRadians.Z);
						Matrix4x4 rollMatrix = Matrix4x4.CreateRotationZ(previousLocalRotationRadians.Y);

						Matrix4x4 rotationMatrix = pitchMatrix * yawMatrix * rollMatrix;

						// Now, newLocalRotationDegrees contains the updated local rotation in degrees

						Matrix4x4 viewRotation = Matrix4x4.Identity;
						if (camera is OrthographicCamera orthographic)
						{

							var dot = origv.Dot(newv);

							var angle = Math.Acos(Math.Max(-1, Math.Min(1, dot)));
							if ((origv.Cross(newv).Z < 0)) angle *= -1;

							var roundingDegrees = 15f;
							if (KeyboardState.Alt) roundingDegrees = 1;

							var deg = angle * (180 / Math.PI);
							float rnd = (float)(Math.Round(deg / roundingDegrees) * roundingDegrees);

							var anglerad = (float)(rnd * (Math.PI / 180));
							switch (orthographic.ViewType)
							{
								case OrthographicCamera.OrthographicType.Top:
									viewRotation = Matrix4x4.CreateRotationZ(anglerad);
									break;
								case OrthographicCamera.OrthographicType.Front:
									viewRotation = Matrix4x4.CreateRotationX(anglerad);
									break;
								case OrthographicCamera.OrthographicType.Side:
									viewRotation = Matrix4x4.CreateRotationY(-anglerad);
									break;
								default:
									break;
							}

							rotationMatrix *= viewRotation;


							var newLocalRotationDegrees = ExtractEulerAngles(rotationMatrix);

							var op = new EditEntityDataProperties(entity.ID, new Dictionary<string, string>() {
						{"angles", $"{Math.Round( MathHelper.RadiansToDegrees( newLocalRotationDegrees.Y))} {Math.Round( MathHelper.RadiansToDegrees( -newLocalRotationDegrees.Z))} {Math.Round(MathHelper.RadiansToDegrees(-newLocalRotationDegrees.X))}" }});

							tsn.Add(op);
						}
					}
				}
				((Action)(async () => await MapDocumentOperation.Perform(document, tsn)))();
			}
			_rotateStart = _rotateEnd = null;
			base.EndDrag(document, viewport, camera, e, position);
		}


		private Vector3 ExtractEulerAngles(Matrix4x4 matrix)
		{
			float x, y, z;

			// Extract rotation around Y axis
			y = (float)Math.Asin(matrix.M13);

			// Handle special cases for pitch near +-90 degrees
			if ((float)Math.Abs(matrix.M13) < 0.99999)
			{
				// Extract rotation around X and Z axes
				x = (float)Math.Atan2(-matrix.M23, matrix.M33);
				z = (float)Math.Atan2(-matrix.M12, matrix.M11);
			}
			else
			{
				// Gimbal lock case: rotation around X axis is set to 0, and extract rotation around Z axis
				x = 0;
				z = (float)Math.Atan2(matrix.M21, matrix.M22);
			}

			return new Vector3(x, y, z);
		}
		public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
		{
			var (wpos, soff) = GetWorldPositionAndScreenOffset(camera);
			var spos = camera.WorldToScreen(wpos) + soff;

			const float radius = 4;

			im.AddCircleFilled(spos.ToVector2(), radius, Color.White);
			im.AddCircle(spos.ToVector2(), radius, Color.Black);
		}

		public Matrix4x4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, MapDocument doc)
		{
			var origin = camera.ZeroUnusedCoordinate((state.OrigStart + state.OrigEnd) / 2);
			if (_origin != null) origin = _origin.Position;

			if (!_rotateStart.HasValue || !_rotateEnd.HasValue) return null;

			var forigin = camera.Flatten(origin);

			var origv = Vector3.Normalize(_rotateStart.Value - forigin);
			var newv = Vector3.Normalize(_rotateEnd.Value - forigin);

			var angle = Math.Acos(Math.Max(-1, Math.Min(1, origv.Dot(newv))));
			if ((origv.Cross(newv).Z < 0)) angle = 2 * Math.PI - angle;

			// TODO post-beta: configurable rotation snapping
			var roundingDegrees = 15f;
			if (KeyboardState.Alt) roundingDegrees = 1;

			var deg = angle * (180 / Math.PI);
			var rnd = Math.Round(deg / roundingDegrees) * roundingDegrees;
			angle = rnd * (Math.PI / 180);

			Matrix4x4 rotm;
			if (camera.ViewType == OrthographicCamera.OrthographicType.Top) rotm = Matrix4x4.CreateRotationZ((float)angle);
			else if (camera.ViewType == OrthographicCamera.OrthographicType.Front) rotm = Matrix4x4.CreateRotationX((float)angle);
			else rotm = Matrix4x4.CreateRotationY((float)-angle); // The Y axis rotation goes in the reverse direction for whatever reason

			var mov = Matrix4x4.CreateTranslation(-origin.X, -origin.Y, -origin.Z);
			var rot = Matrix4x4.Multiply(mov, rotm);
			var inv = Matrix4x4.Invert(mov, out var i) ? i : Matrix4x4.Identity;
			return Matrix4x4.Multiply(rot, inv);
		}

		public TextureTransformationType GetTextureTransformationType(MapDocument doc)
		{
			var tl = doc.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
			return !tl.TextureLock ? TextureTransformationType.None : TextureTransformationType.Uniform;
		}
	}
}