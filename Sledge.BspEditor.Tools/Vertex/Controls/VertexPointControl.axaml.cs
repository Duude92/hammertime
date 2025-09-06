using Avalonia.Controls;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Tools.Vertex.Tools;
using Sledge.Common.Easings;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Translations;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using Sledge.Shell;
using static System.Net.Mime.MediaTypeNames;
using Avalonia.Interactivity;

namespace Sledge.BspEditor.Tools;

[Export]
[AutoTranslate]
public partial class VertexPointControl : UserControl
{
	public bool AutomaticallyMerge
	{
		get => AutoMerge.IsChecked.Value;
		set => AutoMerge.IsChecked = value;
	}

	public bool SplitEnabled
	{
		get => SplitButton.IsEnabled;
		set => SplitButton.IsEnabled = value;
	}

	#region Translations

	public string MergeOverlappingVertices { set => this.InvokeLater(() => MergeButton.Content = value); }
	public string MergeAutomatically { set => this.InvokeLater(() => AutoMerge.Content = value); }
	public string SplitFace { set => this.InvokeLater(() => SplitButton.Content = value); }
	public string ShowPoints { set => this.InvokeLater(() => ShowPointsCheckbox.Content = value); }
	public string ShowMidpoints { set => this.InvokeLater(() => ShowMidpointsCheckbox.Content = value); }
	public string MergeResults { get; set; }

	#endregion

	public VertexPointControl()
	{
		InitializeComponent();
		//CreateHandle();
	}

	private void SplitButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("Command:Run", new CommandMessage("BspEditor:VertexSplitEdge"));
	}

	private void MergeButtonClicked(object sender, RoutedEventArgs e)
	{
		Oy.Publish("VertexPointTool:Merge", string.Empty);
	}

	public void ShowMergeResult(int mergedVertices, int removedFaces)
	{
		if (mergedVertices + removedFaces <= 0) return;
		MergeResultsLabel.Content = String.Format(MergeResults, mergedVertices, removedFaces);
		//MergeResultsLabel.Trigger();
	}

	private void ShowPointsChanged(object sender, EventArgs e)
	{
		VertexPointTool.VisiblePoints v;
		var sp = ShowPointsCheckbox.IsChecked.Value;
		var smp = ShowMidpointsCheckbox.IsChecked.Value;

		if (sp && smp)
		{
			v = VertexPointTool.VisiblePoints.All;
		}
		else if (sp)
		{
			v = VertexPointTool.VisiblePoints.Vertices;
		}
		else if (smp)
		{
			v = VertexPointTool.VisiblePoints.Midpoints;
		}
		else if (sender == ShowMidpointsCheckbox)
		{
			v = VertexPointTool.VisiblePoints.Vertices;
			ShowPointsCheckbox.IsChecked = true;
		}
		else
		{
			v = VertexPointTool.VisiblePoints.Midpoints;
			ShowMidpointsCheckbox.IsChecked = true;
		}

		Oy.Publish("VertexPointTool:SetVisiblePoints", v.ToString());
	}

	public void SetVisiblePoints(VertexPointTool.VisiblePoints visiblePoints)
	{
		ShowPointsCheckbox.IsChecked = visiblePoints != VertexPointTool.VisiblePoints.Midpoints;
		ShowMidpointsCheckbox.IsChecked = visiblePoints != VertexPointTool.VisiblePoints.Vertices;
	}
}

public class FadeLabel : Label, IDisposable
{
	private long _lastTick;
	private long _remaining;

	private readonly System.Windows.Forms.Timer _timer;
	private readonly Easing _easing;

	public int FadeTime { get; set; } = 1000;

	public FadeLabel()
	{
		_easing = Easing.FromType(EasingType.Sinusoidal, EasingDirection.Out);
		_timer = new()
		{
			Enabled = false,
			Interval = 50
		};
		_timer.Tick += (s, e) => Tick();
		_remaining = 0;
	}

	private void Tick()
	{
		var tick = DateTime.Now.Ticks / 10000;
		_remaining -= (tick - _lastTick);
		_lastTick = tick;
		if (_remaining <= 0) _timer.Stop();
		//Invalidate();
		//Refresh();
	}

	public void Trigger()
	{
		_remaining = FadeTime;
		_lastTick = DateTime.Now.Ticks / 10000;
		_timer.Start();
	}

	public void Dispose() => Dispose(true);
	protected void Dispose(bool disposing)
	{
		if (disposing) _timer.Dispose();
		//base.Dispose(disposing);
	}

	//protected override void OnPaint(PaintEventArgs e)
	//{
	//	var val = _easing.Evaluate((_remaining * 1d) / FadeTime);
	//	val = Math.Min(1, Math.Max(0, val));
	//	var a = (int)(val * 255);
	//	var c = Color.FromArgb(a, ForeColor);

	//	using (var brush = new SolidBrush(c))
	//	{
	//		e.Graphics.DrawString(Text, Font, brush, ClientRectangle);
	//	}
	//}
}
