using System;
using System.Collections.ObjectModel;

using Casamia.Logging;

namespace Casamia.MyFacility
{
	public class MyConsole : ILogger
	{
		public MyConsole(System.Windows.Controls.DataGrid logGrid)
		{
			_LogGrid = logGrid;
			_LogGrid.ItemsSource = logs;
		}

		ObservableCollection<Casamia.Logging.Log> logs = new ObservableCollection<Casamia.Logging.Log>();

		private System.Windows.Controls.DataGrid _LogGrid;
		public void Log(Casamia.Logging.Log log)
		{
			_LogGrid.Dispatcher.Invoke(new Action(() => {
				logs.Insert(0, log);

				if(logs.Count>10000)
				{
					logs.RemoveAt(logs.Count - 1);
				}
				//_LogGrid.ScrollIntoView(_LogGrid.Items[_LogGrid.Items.Count - 1]);
			}));
		}

		public void Clear()
		{
			_LogGrid.Dispatcher.Invoke(new Action(() =>
			{
				logs.Clear();
			}));
		}
	}
}
