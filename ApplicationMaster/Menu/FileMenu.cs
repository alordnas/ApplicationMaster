using Casamia.MyFacility;

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
			Parameter parameter = new Parameter();

			string user = MyUser.UserJob == Job.Designer ? Util.DESIGNPATH : Util.PRODUCTPATH;

			parameter.Key = user;
			parameter.Value = dir;

			XMLManage.SaveParameter(parameter);

		}



	}
}
