using Casamia.Core;

namespace Casamia.Menu
{
	public class FileMenu
	{
		static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		public static void OpenDir()
		{
			System.Windows.Forms.FolderBrowserDialog d = new System.Windows.Forms.FolderBrowserDialog();

			d.SelectedPath = mainWindow.dir_TextBox.Text;

			if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var folderName = d.SelectedPath;
				mainWindow.RefreshWholeTree();
				SaveDir(folderName);
			}
		}

		private static void SaveDir(string dir)
		{
			WorkSpaceManager.Instance.Current.LocalUrl = dir;
			WorkSpaceManager.Instance.Save();
		}



	}
}
