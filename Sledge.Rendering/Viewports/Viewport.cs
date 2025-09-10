using System;
using System.Diagnostics;
using System.Windows.Forms;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports.ViewportResolver;
using Veldrid;

namespace Sledge.Rendering.Viewports
{
	public class Viewport : Control, IViewportWF
	{
		private static int _nextId = 1;
		private static readonly IntPtr HInstance = Process.GetCurrentProcess().Handle;

		private bool _resizeRequired;

		public int ID { get; }
		public Swapchain Swapchain { get; }

		public ICamera Camera
		{
			get => _camera;
			set
			{
				_camera = value;
				_camera.Width = Width;
				_camera.Height = Height;
			}
		}

		public Control Control => this;
		public Framebuffer ViewportFramebuffer { get; private set; }
		public ViewportOverlay Overlay { get; }
		public Resources.Texture ViewportRenderTexture { get; private set; }
		public bool IsFocused => _isFocused;

		private bool _isFocused;
		private int _unfocusedCounter = 0;
		private ICamera _camera;
		private GraphicsDevice _device;
		private TextureSampleCount _sampleCount;
		private IViewportResolver _viewportResolver;
		private Texture _mainSceneColorTexture;
		private Texture _viewportResolvedTexture;

		public event EventHandler<long> OnUpdate;

		public Viewport(GraphicsDevice graphics, GraphicsDeviceOptions options, TextureSampleCount sampleCount)
		{
			_device = graphics;
			_sampleCount = sampleCount;
			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			DoubleBuffered = false;

			var hWnd = Handle; // Will call CreateHandle internally
			var hInstance = HInstance;

			uint w = (uint)Width, h = (uint)Height;
			if (w <= 0) w = 1;
			if (h <= 0) h = 1;

			ID = _nextId++;
			Camera = new PerspectiveCamera { Width = Width, Height = Height };

			var source = SwapchainSource.CreateWin32(hWnd, hInstance);
			var desc = new SwapchainDescription(source, w, h, options.SwapchainDepthFormat, options.SyncToVerticalBlank);
			Swapchain = graphics.ResourceFactory.CreateSwapchain(desc);

			InitFramebuffer(w, h, sampleCount);

			//Overlay = new ViewportOverlay(this);
		}
		public void InitFramebuffer(TextureSampleCount sampleCount)
		{
			InitFramebuffer((uint)Width, (uint)Height, sampleCount);
		}

		private void InitFramebuffer(uint width, uint height, TextureSampleCount sampleCount)
		{
			_sampleCount = sampleCount;

			var mainSceneDepthTexture = _device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
				width,
				height,
				1,
				1,
				PixelFormat.R32_Float,
				TextureUsage.DepthStencil,
				 sampleCount));
			_viewportResolvedTexture = _device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled,
				TextureSampleCount.Count1));
			TextureDescription mainColorDesc = TextureDescription.Texture2D(
				width,
				height,
				1,
				1,
				PixelFormat.B8_G8_R8_A8_UNorm,
				TextureUsage.RenderTarget | TextureUsage.Sampled,
				sampleCount);
			_mainSceneColorTexture = _device.ResourceFactory.CreateTexture(ref mainColorDesc);
			ViewportFramebuffer = _device.ResourceFactory.CreateFramebuffer(new FramebufferDescription(mainSceneDepthTexture, _mainSceneColorTexture));
			ViewportRenderTexture = new Sledge.Rendering.Resources.Texture(Engine.Engine.Instance.Context, _viewportResolvedTexture, Resources.TextureSampleType.Standard);
			if (sampleCount == TextureSampleCount.Count1)
			{
				_viewportResolver = new SingleSampleResolver();
			}
			else
			{
				_viewportResolver = new MultisampledResolver();
			}
		}
		protected override bool IsInputKey(Keys keyData)
		{
			// Force all keys to be passed to the regular key events
			return true;
		}

		public void Update(long frame)
		{
			if (_resizeRequired)
			{
				var w = Math.Max(Width, 1);
				var h = Math.Max(Height, 1);
				Swapchain.Resize((uint)w, (uint)h);
				InitFramebuffer((uint)w, (uint)h, _sampleCount);
				_resizeRequired = false;
			}

			OnUpdate?.Invoke(this, frame);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			_isFocused = true;
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			_isFocused = false;
			base.OnMouseLeave(e);
		}

		protected override void OnResize(EventArgs e)
		{
			_resizeRequired = true;
			Camera.Width = Width;
			Camera.Height = Height;
			base.OnResize(e);
		}

		public bool ShouldRender(long frame)
		{
			if (!_isFocused)
			{
				_unfocusedCounter++;

				// Update every 10th frame
				if (_unfocusedCounter % 10 != 0)
				{
					return false;
				}
			}

			_unfocusedCounter = 0;
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				Overlay.Dispose();
				Swapchain.Dispose();
				_viewportResolvedTexture.Dispose();
				ViewportFramebuffer.Dispose();
				ViewportRenderTexture.Dispose();
			}
		}

		public void ResolveRenderTexture(CommandList commandList)
		{
			_viewportResolver.ResolveTexture(commandList, _mainSceneColorTexture, _viewportResolvedTexture);
		}
	}
}
