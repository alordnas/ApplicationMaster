using System;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Threading;

using Casamia.ViewModel;

namespace Casamia
{
	/// <summary>
	/// App.xaml 的交互逻辑
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			AppDomain.CurrentDomain.FirstChanceException += FirstChance;

			AppDomain.CurrentDomain.UnhandledException += Unhandled;

			this.DispatcherUnhandledException +=
				new DispatcherUnhandledExceptionEventHandler(AppUnhandled);

			this.Dispatcher.UnhandledException +=
				new DispatcherUnhandledExceptionEventHandler(DispUnhandled);

			this.Dispatcher.UnhandledExceptionFilter +=
				new DispatcherUnhandledExceptionFilterEventHandler(DispFilter);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			MainWindow window = new MainWindow();
			var viewModel = new MainWindowViewModel();
			// When the ViewModel asks to be closed, 
			// close the window.
			EventHandler handler = null;
			handler = delegate
			{
				viewModel.RequestClose -= handler;
				window.Close();
			};
			viewModel.RequestClose += handler;

			window.DataContext = viewModel;

			window.Show();
		}

		static void FirstChance(object sender, FirstChanceExceptionEventArgs e)
		{
            //MessageBox.Show(e.Exception.Message);
            //MyFacility.MyConsole.WriteError(e.Exception);
		}

		static void Unhandled(object sender, UnhandledExceptionEventArgs e)
		{
            //MessageBox.Show(string.Format("DispatcherUnhandledException {0} - {1}", e.ExceptionObject, e.IsTerminating));

            //MyFacility.MyConsole.WriteError(string.Format("DispatcherUnhandledException {0} - {1}", e.ExceptionObject, e.IsTerminating));
		}

		static void AppUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
            //MessageBox.Show(string.Format("UnhandledException {0}", e.Exception));

            //MyFacility.MyConsole.WriteError(string.Format("UnhandledException {0}", e.Exception));

		}

		static void DispUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
            //MessageBox.Show(string.Format("UnhandledException : {0}", e.Exception));

            //MyFacility.MyConsole.WriteError(string.Format("UnhandledException : {0}", e.Exception));
		}

		static void DispFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
		{
            //MessageBox.Show(string.Format("UnhandledExceptionFilter {0}", e.Exception));
            //MyFacility.MyConsole.WriteError(string.Format("UnhandledExceptionFilter {0}", e.Exception));
		}
	}

}
