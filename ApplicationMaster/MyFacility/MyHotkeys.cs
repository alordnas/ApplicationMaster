using Casamia.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Casamia.MyFacility
{
	public class MyHotkeys
	{
		static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		public static void OnCtrlKeyDown(Key key) 
		{
			switch (key)
			{
				case Key.A:
					EditMenu.SelectAll(true);
					break;
				case Key.E:
					EditMenu.ExpandTree(mainWindow.dir_TreeView, true);
					break;
				case Key.S:
					EditMenu.ExpandTree(mainWindow.dir_TreeView, false);
					break;
				case Key.N:
					EditMenu.SelectAll(false);
					break;
				default:
					break;
			}
		}
	}
}
