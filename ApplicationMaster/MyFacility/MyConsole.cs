using Casamia.DataSource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

using Casamia.Logging;
using System.Collections.ObjectModel;

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
