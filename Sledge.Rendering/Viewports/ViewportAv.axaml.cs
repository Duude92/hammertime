using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform;
using Avalonia.Styling;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Rendering.Viewports.ViewportResolver;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Veldrid;
using Vortice.Mathematics;
using Vulkan;
using Vulkan.Win32;
using static Sledge.Rendering.WinApi;
namespace Sledge.Rendering;
public partial class ViewportAv : UserControl
{
	public ViewportAv()
	{
		InitializeComponent();
		this.Background = Brushes.Yellow;
	}
	public void CreateViewportHost(GraphicsDevice graphics, GraphicsDeviceOptions options, TextureSampleCount sampleCount)
	{
		ViewportHost.InitViewport(graphics, options, sampleCount);
		ViewportHost.CreateOverlay();
	}
}

public class ViewportAvHost : NativeControlHost, IViewport
{
	private static int _nextId = 1;
	private static readonly IntPtr HInstance = Process.GetCurrentProcess().Handle;

	private bool _resizeRequired;

	public int ID { get; private set; }
	public Swapchain Swapchain { get; private set; }

	public ICamera Camera
	{
		get => _camera;
		set
		{
			_camera = value;
			_camera.Width = (int)800;
			_camera.Height = (int)600;
		}
	}

	public Control Control => this.Parent as Control;
	public Framebuffer ViewportFramebuffer { get; private set; }
	public ViewportOverlay Overlay { get; private set; }
	public Resources.Texture ViewportRenderTexture { get; private set; }
	public new bool IsFocused { get; set; }

	public IntPtr Hwnd { get; private set; }

	private int _unfocusedCounter = 0;
	private ICamera _camera;
	private GraphicsDevice _device;
	private GraphicsDeviceOptions _options;
	private TextureSampleCount _sampleCount;
	private IViewportResolver _viewportResolver;
	private Texture _mainSceneColorTexture;
	private Texture _viewportResolvedTexture;

	public event EventHandler<long> OnUpdate;

	public new float Width { get; set; }
	public new float Height { get; set; }

	protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
	{
		var parentHwnd = parent.Handle;

		SetWindowLongPtr(parentHwnd, GWL_EXSTYLE,
			(IntPtr)(GetWindowLongPtr(parentHwnd, GWL_EXSTYLE).ToInt64() | WS_EX_TRANSPARENT));

		Hwnd = parentHwnd;
		InitViewport(_device, _options, TextureSampleCount.Count4);

		return new PlatformHandle(parentHwnd, "HWND");

	}
	public ViewportAvHost()
	{
		this.SizeChanged += OnResize;
		Camera = new PerspectiveCamera { Width = (int)800, Height = (int)600 };
		this.AttachedToLogicalTree += (s, e) =>
		{
			var top = e.Root as TopLevel;
			if (top != null)
			{
				top.PointerMoved += Top_PointerMoved;
			}
		};
	}
	private void Top_PointerMoved(object sender, PointerEventArgs e)
	{
		if (this.Bounds.Contains(e.GetPosition(this)) && !this.IsFocused)
		{
			this.OnPointerEntered(e);
		}
		else if (!this.Bounds.Contains(e.GetPosition(this)) && this.IsFocused)
		{
			this.OnPointerExited(e);
		}
	}

	public void CreateOverlay() => Overlay = new ViewportOverlay(this);

	public void InitViewport(GraphicsDevice graphics, GraphicsDeviceOptions options, TextureSampleCount sampleCount)
	{
		if (Hwnd == IntPtr.Zero)
		{
			//TODO: Refactor this, as it doesn't creates hwnd before
			Hwnd = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow.TryGetPlatformHandle().Handle;
		}

		_device = graphics;
		_options = options;
		//_sampleCount = sampleCount;
		//SetStyle(ControlStyles.Opaque, true);
		//SetStyle(ControlStyles.UserPaint, true);
		//SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		//DoubleBuffered = false;

		var hWnd = Hwnd; // Will call CreateHandle internally
		var hInstance = HInstance;

		int w = 800;
		int h = 600;

		//int w = (int)Width, h = (int)Height;
		//if (w <= 0) w = 1;
		//if (h <= 0) h = 1;

		ID = _nextId++;
		var source = SwapchainSource.CreateWin32(hWnd, hInstance);
		var desc = new SwapchainDescription(source, (uint)w, (uint)h, options.SwapchainDepthFormat, options.SyncToVerticalBlank);
		Swapchain = graphics.ResourceFactory.CreateSwapchain(desc);

		InitFramebuffer((uint)w, (uint)h, sampleCount);
	}
	public void InitFramebuffer(TextureSampleCount sampleCount)
	{

		InitFramebuffer((uint)Width, (uint)Height, sampleCount);
	}

	private void InitFramebuffer(uint width, uint height, TextureSampleCount sampleCount)
	{
		_sampleCount = sampleCount;

		//FIXME: viewport/camera sizes
		var a = int.MaxValue;
		if (width > 10000 && height > 10000) { width = 800; height = 600; }
		if (width < 1 && height < 1) { width = 1; height = 1; }
		var mainSceneDepthTexture = _device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(
			width,
			height,
			1,
			1,
			Veldrid.PixelFormat.R32_Float,
			TextureUsage.DepthStencil,
			 sampleCount));
		_viewportResolvedTexture = _device.ResourceFactory.CreateTexture(TextureDescription.Texture2D(width, height, 1, 1, Veldrid.PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled,
			TextureSampleCount.Count1));
		TextureDescription mainColorDesc = TextureDescription.Texture2D(
			width,
			height,
			1,
			1,
			Veldrid.PixelFormat.B8_G8_R8_A8_UNorm,
			TextureUsage.RenderTarget | TextureUsage.Sampled,
			sampleCount);
		_mainSceneColorTexture = _device.ResourceFactory.CreateTexture(ref mainColorDesc);
		ViewportFramebuffer = _device.ResourceFactory.CreateFramebuffer(new FramebufferDescription(mainSceneDepthTexture, _mainSceneColorTexture));
		ViewportRenderTexture = new Sledge.Rendering.Resources.Texture(Engine.Engine.Instance.Context, _viewportResolvedTexture, Rendering.Resources.TextureSampleType.Standard);
		if (sampleCount == TextureSampleCount.Count1)
		{
			_viewportResolver = new SingleSampleResolver();
		}
		else
		{
			_viewportResolver = new MultisampledResolver();
		}
	}
	//protected override bool IsInputKey(Keys keyData)
	//{
	//	// Force all keys to be passed to the regular key events
	//	return true;
	//}

	public void Update(long frame)
	{
		if (_resizeRequired)
		{
			var w = Bounds.Width;
			var h = Bounds.Height;
			Swapchain.Resize((uint)w, (uint)h);
			InitFramebuffer((uint)w, (uint)h, _sampleCount);
			_resizeRequired = false;
		}

		OnUpdate?.Invoke(this, frame);
	}
	protected override void OnPointerEntered(PointerEventArgs e)
	{
		Focus();
		IsFocused = true;
		base.OnPointerEntered(e);
	}


	protected override void OnPointerExited(PointerEventArgs e)
	{
		IsFocused = false;
		base.OnPointerExited(e);
	}

	private void OnResize(object sender, SizeChangedEventArgs e)
	{
		Width = (float)base.Bounds.Width;
		Height = (float)base.Bounds.Height;
		_resizeRequired = true;
		Camera.Width = (int)Bounds.Width;
		Camera.Height = (int)Bounds.Height;
	}

	public bool ShouldRender(long frame)
	{
		if (!IsFocused)
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

	protected void Dispose(bool disposing)
	{
		if (disposing)
		{
			//Overlay.Dispose();
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
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}