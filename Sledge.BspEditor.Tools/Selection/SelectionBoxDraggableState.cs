using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Commands.Clipboard;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Mutation;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Selection.TransformationHandles;
using Sledge.BspEditor.Tools.Widgets;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Selection
{
	public class SelectionBoxDraggableState : BoxDraggableState
	{
		private readonly SelectTool _tool;

		public enum TransformationMode
		{
			Resize,
			Rotate,
			Skew
		}
		private List<IDraggable>[] _handles;
		public TransformationMode CurrentTransformationMode { get; private set; }
		private RotationOrigin _rotationOrigin;

		public List<Widget> Widgets { get; private set; }

		private bool _showWidgets;
		public bool ShowWidgets
		{
			get => _showWidgets;
			set
			{
				_showWidgets = value;
				Update();
			}
		}
		private readonly MoveWidget _scaleWidget;
		private readonly RotationWidget _rotationWidget;

		public SelectionBoxDraggableState(SelectTool tool) : base(tool)
		{
			_tool = tool;
			Widgets = new List<Widget>
			{
				(_rotationWidget = new RotationWidget(tool.GetDocument()) { Active = false }),
				(_scaleWidget = new MoveWidget(tool.GetDocument()) {Active = false})
			};
			BindWidgets();
		}

		private void BindWidgets()
		{
			foreach (var w in Widgets)
			{
				w.Transforming += WidgetTransforming;
				w.Transformed += WidgetTransformed;
			}
		}

		private void WidgetTransforming(Widget sender, Matrix4x4? transformation)
		{
			if (transformation.HasValue)
			{
				Engine.Interface.SetSelectiveTransform(transformation.Value);
			}
		}

		private void WidgetTransformed(Widget sender, Matrix4x4? transformation)
		{
			var document = Tool.GetDocument();
			var task = Task.CompletedTask;

			if (document != null && transformation.HasValue)
			{
				var objects = document.Selection.GetSelectedParents().ToList();
				var shf = KeyboardState.Shift;

				var transaction = new Transaction();
				if (!shf)
				{

					// Perform the operation
					var matrix = transformation.Value;
					var transformOperation = new Transform(matrix, objects);
					transaction.Add(transformOperation);

					// Texture for texture operations
					var tl = document.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
					if (tl.TextureLock && sender.IsUniformTransformation)
					{
						transaction.Add(new TransformTexturesUniform(matrix, objects.SelectMany(x => x.FindAll())));
					}
					else if (tl.TextureScaleLock && sender.IsScaleTransformation)
					{
						transaction.Add(new TransformTexturesScale(matrix, objects.SelectMany(x => x.FindAll())));
					}
				}
				else if (shf && sender is MoveWidget)
				{
					transaction.Add(new Deselect(document.Selection));
					var matrix = transformation.Value;
					var clone = objects.Select(x => (IMapObject)x.Copy(document.Map.NumberGenerator)).ToList();
					transaction.Add(new Attach(document.Map.Root.ID, clone));
					var transformOperation = new Transform(matrix, clone);
					transaction.Add(transformOperation);
					transaction.Add(new Select(clone));
				}
				if (sender is RotationWidget)
				{
					foreach (var entity in objects.OfType<Primitives.MapObjects.Entity>())
					{
						var previousAngle = entity.EntityData.GetVector3("angles");
						if (previousAngle.HasValue)
						{
							if (!entity.Hierarchy.HasChildren)
							{

								var oldAngle = previousAngle.Value *(float) (Math.PI / 180);

								var newLocalRotationDegrees = MathHelper.ExtractEulerAngles(transformation.Value);

								var op = new EditEntityDataProperties(entity.ID, new Dictionary<string, string>() {
						{"angles", $"{Math.Round( MathHelper.RadiansToDegrees(oldAngle.X +  newLocalRotationDegrees.Y))} {Math.Round( MathHelper.RadiansToDegrees(oldAngle.Y -newLocalRotationDegrees.Z))} {Math.Round(MathHelper.RadiansToDegrees(oldAngle.Z-newLocalRotationDegrees.X))}" }});

								transaction.Add(op);
							}
						}
					}
				}

				task = MapDocumentOperation.Perform(document, transaction);
			}

			task.ContinueWith(_ =>
			{
				Engine.Interface.SetSelectiveTransform(Matrix4x4.Identity);
				SetRotationOrigin(document.Selection.GetSelectionBoundingBox().Center);
			});
		}

		public void Update()
		{
			foreach (var widget in Widgets)
			{
				widget.Active = State.Action != BoxAction.Idle && CurrentTransformationMode == widget.WidgetTransformationMode && ShowWidgets;
				widget.SetPivotPoint(_rotationOrigin.Position);
				widget.State = State;
			}
		}

		protected override void CreateBoxHandles()
		{
			var resize = new List<IDraggable>
			{
				new ResizeTransformHandle(this, ResizeHandle.TopLeft),
				new ResizeTransformHandle(this, ResizeHandle.TopRight),
				new ResizeTransformHandle(this, ResizeHandle.BottomLeft),
				new ResizeTransformHandle(this, ResizeHandle.BottomRight),

				new ResizeTransformHandle(this, ResizeHandle.Top),
				new ResizeTransformHandle(this, ResizeHandle.Right),
				new ResizeTransformHandle(this, ResizeHandle.Bottom),
				new ResizeTransformHandle(this, ResizeHandle.Left),

				new ResizeTransformHandle(this, ResizeHandle.Center),
			};

			if (_rotationOrigin == null)
			{
				_rotationOrigin = new RotationOrigin(Tool);
				_rotationOrigin.DragMoved += (sender, args) => Update();
			}

			var rotate = new List<IDraggable>
			{
				_rotationOrigin,

				new RotateTransformHandle(this, ResizeHandle.TopLeft, _rotationOrigin),
				new RotateTransformHandle(this, ResizeHandle.TopRight, _rotationOrigin),
				new RotateTransformHandle(this, ResizeHandle.BottomLeft, _rotationOrigin),
				new RotateTransformHandle(this, ResizeHandle.BottomRight, _rotationOrigin),

				new ResizeTransformHandle(this, ResizeHandle.Center),
			};

			var skew = new List<IDraggable>
			{
				new SkewTransformHandle(this, ResizeHandle.Top),
				new SkewTransformHandle(this, ResizeHandle.Right),
				new SkewTransformHandle(this, ResizeHandle.Bottom),
				new SkewTransformHandle(this, ResizeHandle.Left),

				new ResizeTransformHandle(this, ResizeHandle.Center),
			};

			_handles = new[] { resize, rotate, skew };
		}

		public override IEnumerable<IDraggable> GetDraggables()
		{
			if (State.Action == BoxAction.Idle || State.Action == BoxAction.Drawing) return new IDraggable[0];
			return _handles[(int)CurrentTransformationMode];
		}

		public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera,
			ViewportEvent e, Vector3 position)
		{
			return false;
		}

		public Matrix4x4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, MapDocument doc)
		{
			if (State.Action != BoxAction.Resizing) return null;
			var tt = Tool.CurrentDraggable as ITransformationHandle;
			return tt?.GetTransformationMatrix(viewport, camera, State, doc);
		}

		public TextureTransformationType GetTextureTransformationType(MapDocument doc)
		{
			if (State.Action != BoxAction.Resizing) return TextureTransformationType.None;
			var tt = Tool.CurrentDraggable as ITransformationHandle;
			return tt?.GetTextureTransformationType(doc) ?? TextureTransformationType.None;
		}

		public void Cycle()
		{
			var intMode = (int)CurrentTransformationMode;
			var numModes = Enum.GetValues(typeof(TransformationMode)).Length;
			var nextMode = (intMode + 1) % numModes;
			SetTransformationMode((TransformationMode)nextMode);
		}

		public void SetRotationOrigin(Vector3 origin)
		{
			_rotationOrigin.Position = origin;
			Update();
		}

		public void SetTransformationMode(TransformationMode mode)
		{
			CurrentTransformationMode = mode;

			_rotationOrigin.Position = new Box(State.Start, State.End).Center;

			_tool.TransformationModeChanged(CurrentTransformationMode);
			Update();
		}

		protected override void DrawBox(IViewport viewport, OrthographicCamera camera, I2DRenderer im, Vector3 start, Vector3 end)
		{
			im.AddRectFilled(start.ToVector2(), end.ToVector2(), GetRenderFillColour());
			im.AddRect(start.ToVector2(), end.ToVector2(), GetRenderBoxColour());
		}

		protected override Color GetRenderFillColour()
		{
			var c = base.GetRenderFillColour();
			var a = Math.Min(100, Math.Max(0, _tool.SelectionBoxBackgroundOpacity));
			return Color.FromArgb(a, c);
		}
	}
}