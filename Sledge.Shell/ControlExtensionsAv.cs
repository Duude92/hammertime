using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace Sledge.Shell
{
	public static class ControlExtensionsAv
	{
		/*
		public void InvokeLater(Action action) => Dispatcher.UIThread.Post(action);
		public async Task InvokeLaterAsync(Action action) => await Dispatcher.UIThread.InvokeAsync(action);
		public async Task InvokeAsync(Action action) => await Dispatcher.UIThread.InvokeAsync(action);
		public void InvokeSync(Action action) => Dispatcher.UIThread.Invoke(action);
		public void Invoke(Action action) => action.Invoke(); //Dispatcher.UIThread.Invoke(action);

        */


		/// <summary>
		/// Invoke a delegate synchronously on the UI thread.
		/// Blocks the UI until complete. Use with caution.
		/// </summary>
		/// <param name="control">The control to invoke upon</param>
		/// <param name="action">The delegate to run</param>
		public static void InvokeSync(this Control control, Action action)
		{
			if (Dispatcher.UIThread.CheckAccess())
				action();
			else
				Dispatcher.UIThread.Post(() => action());
		}

		/// <summary>
		/// Invoke a delegate asynchronously on the UI thread and return an awaitable task.
		/// Awaiting the task will block the UI until complete. Use with caution.
		/// </summary>
		/// <param name="control">The control to invoke upon</param>
		/// <param name="action">The delegate to run</param>
		/// <returns>The task that will resolve once the delegate is complete</returns>
		public static async Task InvokeAsync(this Control control, Action action)
		{
			await Dispatcher.UIThread.InvokeAsync(action);
		}

		/// <summary>
		/// Invoke a delegate asynchronously on the UI thread and return immediately.
		/// The delegate will run at some unknown time. Use carefully.
		/// </summary>
		/// <param name="control">The control to invoke upon</param>
		/// <param name="action">The delegate to run</param>
		public static void InvokeLater(this Control control, Action action)
		{
			control.InvokeSync(action);
		}

		public static Task InvokeLaterAsync(this Control control, Action action)
		{
			var tcs = new TaskCompletionSource<int>();
			control.InvokeAsync(() =>
			{
				try
				{
					action();
					tcs.SetResult(0);
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			});
			return tcs.Task;
		}

		/// <summary>
		/// Opens a dialog and returns an awaitable task that will resolve once the dialog has closed.
		/// </summary>
		/// <param name="form">The dialog to open</param>
		/// <returns>The task that will resolve once the dialog is closed</returns>
		public static async Task<System.Windows.Forms.DialogResult> ShowDialogAsync(this Window form, Window parent)
		{
			await Task.Yield();
			return form.ShowDialog<System.Windows.Forms.DialogResult>(parent).Result;
		}
	}
}
