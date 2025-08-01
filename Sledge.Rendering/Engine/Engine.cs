﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using SharpDX.Direct3D;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Rendering.Engine
{
	public class Engine : IDisposable
	{
		internal static Engine Instance { get; } = new Engine();
		public static EngineInterface Interface { get; } = new EngineInterface();

		public GraphicsDevice Device { get; }
		public Thread RenderThread { get; private set; }
		public Scene Scene { get; }
		internal bool IsShadowsEnabled { get; set; } = false;
		internal Vector3 LightAngle { get; set; } = Vector3.Zero;
		internal RenderContext Context { get; }

		private CancellationTokenSource _token;

		private readonly GraphicsDeviceOptions _options;
		private readonly Stopwatch _timer;
		private readonly object _lock = new object();
		private readonly List<IViewport> _renderTargets;
		private readonly Dictionary<PipelineGroup, List<IPipeline>> _pipelines;
		private readonly CommandList _commandList;

		private RgbaFloat _clearColourPerspective;
		private RgbaFloat _clearColourOrthographic;
		internal int InactiveTargetFps { get; set; } = 10;
		private long _previousFrameTime = DateTime.Now.Ticks;
		private ViewProjectionBuffer _lightData;
		private ViewProjectionBuffer _cameraBuffer;

		private Engine()
		{
			_options = new GraphicsDeviceOptions
			{
				HasMainSwapchain = false,
				ResourceBindingModel = ResourceBindingModel.Improved,
				SwapchainDepthFormat = PixelFormat.R32_Float,
			};

			Device = GraphicsDevice.CreateD3D11(_options);
			DetectFeatures(Device);
			Scene = new Scene();

			_commandList = Device.ResourceFactory.CreateCommandList();

			SetClearColour(CameraType.Both, RgbaFloat.Black);

			float orthoSize = 5000f; // Adjust based on scene size
			float nearPlane = 0.01f;
			float farPlane = 10000f;

			var _lightProjection = Matrix4x4.CreateOrthographic(
				orthoSize, // Width
				orthoSize, // Height
				nearPlane,
				farPlane
			);

			Vector3 lightPosition = new Vector3(0, 0, 1000); // Light high above the scene
			Vector3 lightTarget = new Vector3(0, 0, 0); // Looking at the scene center
			Vector3 upVector = Vector3.UnitZ; // Adjust if needed


			var _lightView = Matrix4x4.CreateLookAt(
				lightPosition,
				lightTarget,
				upVector
			);
			_lightData = new ViewProjectionBuffer { Projection = _lightProjection, View = _lightView };

			_timer = new Stopwatch();
			_token = new CancellationTokenSource();

			_renderTargets = new List<IViewport>();
			_pipelines = new Dictionary<PipelineGroup, List<IPipeline>>();
			Context = new RenderContext(Device);
			Scene.Add(Context);
#if DEBUG
			Scene.Add(new FpsMonitor());
#endif

			RenderThread = new Thread(Loop);

			_pipelines.Add(PipelineGroup.Opaque, new List<IPipeline>());
			_pipelines.Add(PipelineGroup.Transparent, new List<IPipeline>());
			_pipelines.Add(PipelineGroup.Overlay, new List<IPipeline>());

			AddPipeline(new SkyboxPipeline());
			AddPipeline(new WireframePipeline());
			AddPipeline(new TexturedOpaquePipeline());
			AddPipeline(new BillboardOpaquePipeline());
			AddPipeline(new WireframeModelPipeline());
			AddPipeline(new TexturedModelPipeline());

			AddPipeline(new TexturedAlphaPipeline());
			AddPipeline(new TexturedAdditivePipeline());
			AddPipeline(new BillboardAlphaPipeline());
//#if DEBUG
//			AddPipeline(new SwapchainShadowOverlay(shadowdepth.NearShadowResourceTexture));
//#endif
			AddPipeline(new SwapchainOverlayPipeline());
			AddPipeline(new OverlayPipeline());
			AddPipeline(new ShadowOverlayPipeline(_lightData));

			Application.ApplicationExit += Shutdown;
		}

		private void DetectFeatures(GraphicsDevice device)
		{
			var dev = device.GetType().GetProperty("Device");
			var dxd = dev?.GetValue(device) as SharpDX.Direct3D11.Device;
			var fl = dxd?.FeatureLevel ?? FeatureLevel.Level_10_0; // Just assume it's DX10, whatever
			if (fl < FeatureLevel.Level_10_0)
			{
				MessageBox.Show($"Sledge requires DirectX 10, but your computer only has version {fl}.", "Unsupported graphics card!");
				Environment.Exit(1);
			}
			Features.FeatureLevel = fl;
		}

		internal void SetClearColour(CameraType type, RgbaFloat colour)
		{
			lock (_lock)
			{
				if (type == CameraType.Both) _clearColourOrthographic = _clearColourPerspective = colour;
				else if (type == CameraType.Orthographic) _clearColourOrthographic = colour;
				else _clearColourPerspective = colour;
			}
		}

		public void AddPipeline(IPipeline pipeline)
		{
			pipeline.Create(Context, TextureSampleCount.Count1);
			lock (_lock)
			{
				_pipelines[pipeline.Group].Add(pipeline);
				_pipelines[pipeline.Group].Sort((a, b) => a.Order.CompareTo(b.Order));
			}
		}

		public void Dispose()
		{
			_pipelines.SelectMany(x => x.Value).ToList().ForEach(x => x.Dispose());
			_pipelines.Clear();

			_renderTargets.ForEach(x => x.Dispose());
			_renderTargets.Clear();

			Device.Dispose();
			_token.Dispose();
		}

		private void Shutdown(object sender, EventArgs e)
		{
			Dispose();
			Application.ApplicationExit -= Shutdown;
		}

		// Render loop

		private void Start()
		{
			_timer.Start();
			RenderThread.Start(_token.Token);
			RenderThread.Name = "Render Thread";
		}

		private void Stop()
		{
			_token.Cancel();
			_timer.Stop();

			RenderThread = new Thread(Loop);
			_token = new CancellationTokenSource();
		}

		private int _paused = 0;
		private TextureSampleCount _sampleCount = TextureSampleCount.Count1;
		private readonly ManualResetEvent _pauseThreadEvent = new ManualResetEvent(false);

		public IDisposable Pause()
		{
			_paused++;
			if (_timer.IsRunning) _pauseThreadEvent.WaitOne();
			return new PauseImpl(() =>
			{
				_paused--;
				_pauseThreadEvent.Reset();
			});
		}
		private class PauseImpl : IDisposable
		{
			private readonly Action _disposeAction;
			public PauseImpl(Action disposeFunc) => _disposeAction = disposeFunc;
			public void Dispose() => _disposeAction();
		}

		private void Loop(object o)
		{
			var token = (CancellationToken)o;
			try
			{
				var lastFrame = _timer.ElapsedMilliseconds;
				while (!token.IsCancellationRequested)
				{
					var frame = _timer.ElapsedMilliseconds;
					var diff = (frame - lastFrame);
					if (diff < 8 || _paused > 0)
					{
						if (_paused > 0) _pauseThreadEvent.Set();
						Thread.Sleep(1);
						continue;
					}

					lastFrame = frame;
					Render(frame);
					Device.WaitForIdle();
				}
			}
			catch (ThreadInterruptedException)
			{
				// exit
			}
			catch (ThreadAbortException)
			{
				// exit
			}
		}

		private void Render(long frame)
		{
			lock (_lock)
			{
				Scene.Update(frame);
				var overlays = Scene.GetOverlayRenderables().ToList();

				long currentTime = DateTime.Now.Ticks;
				long elapsedTime = currentTime - _previousFrameTime;
				double millisecondsPerFrame = 1000.0 / InactiveTargetFps;
				bool shouldRender = (elapsedTime >= millisecondsPerFrame * TimeSpan.TicksPerMillisecond);

				foreach (var rt in _renderTargets)
				{
					rt.Update(frame);
					rt.Overlay.Build(overlays);
					if (rt.IsFocused || (!rt.IsFocused && shouldRender))
					{
						_cameraBuffer = new ViewProjectionBuffer { Projection = rt.Camera.Projection, View = rt.Camera.View };
						Render(rt);
					}
				}
				if (shouldRender)
					_previousFrameTime = currentTime;
			}
		}

		private void Render(IViewport renderTarget)
		{
			_commandList.Begin();
			_commandList.SetFramebuffer(renderTarget.ViewportFramebuffer);
			_commandList.ClearDepthStencil(1);

			var cc = renderTarget.Camera.Type == CameraType.Perspective
				? _clearColourPerspective
				: _clearColourOrthographic;
			_commandList.ClearColorTarget(0, cc);

			//foreach (var group in _pipelines.OrderBy(x => (int) x.Key))
			//{
			//    foreach (var pipeline in group.Value.OrderBy(x => x.Order))
			//    {
			//        pipeline.SetupFrame(Context, renderTarget);
			//    }
			//}

			//var renderables = _pipelines.ToDictionary(x => x, x => Scene.GetRenderables(x, renderTarget).OrderBy(r => r.Order).ToList());

			// foreach (var pg in _pipelines.GroupBy(x => x.Group).OrderBy(x => x.Key))
			// {
			//     foreach (var pipeline in pg.OrderBy(x => x.Order))
			//     {
			//         if (!renderables.ContainsKey(pipeline)) continue;
			//         pipeline.Render(Context, renderTarget, _commandList, renderables[pipeline]);
			//     }
			// 
			//     foreach (var pipeline in pg.OrderBy(x => x.Order))
			//     {
			//         if (!renderables.ContainsKey(pipeline)) continue;
			//         pipeline.RenderTransparent(Context, renderTarget, _commandList, renderables[pipeline]);
			//     }
			// }

			var cameraLocation = renderTarget.Camera.Location;
			var transparentPipelines = _pipelines[PipelineGroup.Transparent];

			// Get the location objects and sort them by distance from the camera
			var locationObjects =
				from t in transparentPipelines
				from renderable in Scene.GetRenderables(t, renderTarget)
				from location in renderable.GetLocationObjects(t, renderTarget)
				orderby (cameraLocation - location.Location).LengthSquared() descending
				select new { Pipeline = t, Renderable = renderable, Location = location };


			foreach (var opaque in _pipelines[PipelineGroup.Opaque])
			{
				opaque.SetupFrame(Context, _cameraBuffer);
				opaque.Render(Context, renderTarget, _commandList, Scene.GetRenderables(opaque, renderTarget));
			}


			foreach (var transparent in transparentPipelines)
			{
				transparent.SetupFrame(Context, _cameraBuffer);
			}
			foreach (var lo in locationObjects)
			{
				lo.Pipeline.Render(Context, renderTarget, _commandList, lo.Renderable, lo.Location);
			}

			foreach (var pipeline in transparentPipelines)
			{
				if (pipeline.Type == PipelineType.BillboardAlpha || (IsShadowsEnabled && pipeline.Type == PipelineType.ShadowOverlay))
					pipeline.Render(Context, renderTarget, _commandList, Scene.GetRenderables(pipeline, renderTarget));
			}

			_commandList.End();
			Device.SubmitCommands(_commandList);

			_commandList.Begin();
			renderTarget.ResolveRenderTexture(_commandList);
			_commandList.End();
			Device.SubmitCommands(_commandList);

			_commandList.Begin();
			_commandList.SetFramebuffer(renderTarget.Swapchain.Framebuffer);
			_commandList.ClearDepthStencil(1);
			_commandList.ClearColorTarget(0, cc);

			foreach (var overlay in _pipelines[PipelineGroup.Overlay])
			{
				overlay.SetupFrame(Context, _cameraBuffer);
					overlay.Render(Context, renderTarget, _commandList, Scene.GetRenderables(overlay, renderTarget));
			}
			_commandList.End();
			Device.SubmitCommands(_commandList);
			Device.SwapBuffers(renderTarget.Swapchain);
		}

		// Viewports

		internal event EventHandler<IViewport> ViewportCreated;
		internal event EventHandler<IViewport> ViewportDestroyed;

		internal IViewport CreateViewport()
		{
			lock (_lock)
			{
				var control = new Viewports.Viewport(Device, _options, _sampleCount);
				control.Disposed += DestroyViewport;

				if (!_renderTargets.Any()) Start();
				_renderTargets.Add(control);

				Scene.Add((IRenderable)control.Overlay);
				Scene.Add((IUpdateable)control.Overlay);
				ViewportCreated?.Invoke(this, control);

				return control;
			}
		}

		private void DestroyViewport(object viewport, EventArgs e)
		{
			if (!(viewport is IViewport t)) return;

			lock (_lock)
			{
				_renderTargets.Remove(t);
				Device.WaitForIdle();

				if (!_renderTargets.Any()) Stop();

				ViewportDestroyed?.Invoke(this, t);
				Scene.Remove((IRenderable)t.Overlay);
				Scene.Remove((IUpdateable)t.Overlay);

				t.Control.Disposed -= DestroyViewport;
				t.Dispose();
			}
		}

		internal void SetMSAA(int mSAAoption)
		{
			lock (_lock)
			{
				_sampleCount = (TextureSampleCount)mSAAoption;
				foreach (var pl in _pipelines.SelectMany(x => x.Value))
				{
					if (pl.Group == PipelineGroup.Overlay) continue;
					pl.Create(Context, _sampleCount);
				}
				foreach (var rt in _renderTargets)
				{
					rt.InitFramebuffer(_sampleCount);
				}
			}
		}
		public class ViewProjectionBuffer
		{
			public Matrix4x4 Projection;
			public Matrix4x4 View;
		}
	}
}
