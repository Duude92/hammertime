using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;
using Sledge.Rendering.Viewports.ViewportResolver;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Veldrid;
using Vulkan.Win32;

namespace Sledge.Rendering;
public partial class ViewportAv : UserControl
{
	public ViewportAv()
	{
		InitializeComponent();
	}
	public void CreateViewportHost(GraphicsDevice graphics, GraphicsDeviceOptions options, TextureSampleCount sampleCount)
	{
		ViewportHost.InitViewport(graphics, options, sampleCount);
	}
}

public class ViewportAvHost : NativeControlHost, IViewport
{
	private static int _nextId = 1;
	private static readonly IntPtr HInstance = Process.GetCurrentProcess().Handle;

	private bool _resizeRequired;

	public new int Width => (int)base.Width;
	public new int Height => (int)base.Height;

	public int ID { get; private set; }
	public Swapchain Swapchain { get; private set; }

	public ICamera Camera
	{
		get => _camera;
		set
		{
			_camera = value;
			_camera.Width = (int)Width;
			_camera.Height = (int)Height;
		}
	}

	public Control Control => this;
	public Framebuffer ViewportFramebuffer { get; private set; }
	public ViewportOverlay Overlay { get; }
	public Resources.Texture ViewportRenderTexture { get; private set; }
	public bool IsFocused => _isFocused;

	public IntPtr Hwnd { get; private set; }

	System.Windows.Forms.Control IViewport.Control => null;

	private bool _isFocused;
	private int _unfocusedCounter = 0;
	private ICamera _camera;
	private GraphicsDevice _device;
	private TextureSampleCount _sampleCount;
	private IViewportResolver _viewportResolver;
	private Texture _mainSceneColorTexture;
	private Texture _viewportResolvedTexture;

	public event EventHandler<long> OnUpdate;

	protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
	{
		// parent.Handle is the Avalonia window HWND (on Win32 backend)
		var parentHwnd = parent.Handle;

		// Use the builtin "STATIC" window class for a child host
		const int WS_CHILD = 0x40000000;
		const int WS_VISIBLE = 0x10000000;

		var hwnd = CreateWindowExW(
			0, "STATIC", "",
			WS_CHILD | WS_VISIBLE,
			0, 0,
			Math.Max(1, (int)Bounds.Width),
			Math.Max(1, (int)Bounds.Height),
			parentHwnd,
			IntPtr.Zero,
			GetModuleHandle(null),
			IntPtr.Zero);

		if (hwnd == IntPtr.Zero)
			throw new InvalidOperationException("CreateWindowExW failed: " + Marshal.GetLastWin32Error());

		Hwnd = hwnd;
		// Return a handle Avalonia will track
		return new PlatformHandle(hwnd, "HWND");
	}
	public ViewportAvHost()
	{

	}
	public void InitViewport(GraphicsDevice graphics, GraphicsDeviceOptions options, TextureSampleCount sampleCount)
	{
		_device = graphics;
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
		Camera = new PerspectiveCamera { Width = (int)w, Height = (int)h };
		var source = SwapchainSource.CreateWin32(hWnd, hInstance);
		var desc = new SwapchainDescription(source, (uint)w, (uint)h, options.SwapchainDepthFormat, options.SyncToVerticalBlank);
		Swapchain = graphics.ResourceFactory.CreateSwapchain(desc);

		sampleCount = TextureSampleCount.Count4;
		InitFramebuffer((uint)w, (uint)h, sampleCount);

		//Overlay = new ViewportOverlay(this);
	}
	public void InitFramebuffer(TextureSampleCount sampleCount)
	{
		return;
		Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>

	InitFramebuffer((uint)Width, (uint)Height, sampleCount));
	}

	private void InitFramebuffer(uint width, uint height, TextureSampleCount sampleCount)
	{
		_sampleCount = sampleCount;

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
			var w = Math.Max(Width, 1);
			var h = Math.Max(Height, 1);
			Swapchain.Resize((uint)w, (uint)h);
			InitFramebuffer((uint)w, (uint)h, _sampleCount);
			_resizeRequired = false;
		}

		OnUpdate?.Invoke(this, frame);
	}
	protected override void OnPointerEntered(PointerEventArgs e)
	{
		_isFocused = true;
		base.OnPointerEntered(e);
	}


	protected override void OnPointerExited(PointerEventArgs e)
	{
		_isFocused = false;
		base.OnPointerExited(e);
	}

	//protected override void OnResize(EventArgs e)
	//{
	//	_resizeRequired = true;
	//	Camera.Width = Width;
	//	Camera.Height = Height;
	//	base.OnResize(e);
	//}

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

	#region Win32 P/Invoke
	[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr CreateWindowExW(
		int exStyle, string className, string windowName,
		int style, int x, int y, int width, int height,
		IntPtr parent, IntPtr menu, IntPtr hInstance, IntPtr lpParam);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool DestroyWindow(IntPtr hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
		int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	private static extern IntPtr GetModuleHandle(string? lpModuleName);
	#endregion

}