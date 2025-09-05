using Avalonia.Controls;
using Avalonia.Data;
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
		public static void Invoke(this Control control, Action action)
		{
			control.InvokeSync(action);
		}

		/// <summary>
		/// Invoke a delegate asynchronously on the UI thread and return an awaitable task.
		/// Awaiting the task will block the UI until complete. Use with caution.
		/// </summary>
		/// <param name="control">The control to invoke upon</param>
		/// <param name="action">The delegate to run</param>
		/// <returns>The task that will resolve once the delegate is complete</returns>
		public static async Task InvokeAsync(this Control control, Func<Task> action)
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

		public static Task InvokeLaterAsync(this Control control, Func<Task> action)
		{
			var tcs = new TaskCompletionSource<int>();
			control.InvokeAsync(async () =>
			{
				try
				{
					await action();
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
			var dialog = form.ShowDialog<System.Windows.Forms.DialogResult>(parent);
			return await dialog;
		}

		public static void Add(this Avalonia.Controls.Controls controls, Control newControl, int row, int column)
		{
			var rowBinding = new Binding($"{row}");
			var colBinding = new Binding($"{column}");

			for (var i = 0; i < controls.Count; i++)
			{
				var control = controls[i];
				if (control[!Grid.RowProperty] == rowBinding && control[!Grid.ColumnProperty] == colBinding)
				{
					controls.Remove(control);
				}
			}
			Grid.SetRow(newControl, row);
			Grid.SetColumn(newControl, column);
			controls.Add(newControl);
		}

	}
}
