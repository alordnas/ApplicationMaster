using Casamia.MyFacility;
using Casamia.Core;
using Casamia.DataSource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Casamia.Logging;

namespace Casamia.Menu
{
    public class CommonMethod
    {
        static MainWindow mainWindow = App.Current.MainWindow as MainWindow;


        public static void Exit()
        {
            mainWindow.Close();
        }


        public static void SetTitle()
        {
			string title = string.Empty;
			if ((MyWorker.bgWorker != null) && MyWorker.bgWorker.IsBusy)
			{
				title = "Projects(正在运行)";
			}
			else
			{
				title = "Projects";
			}

			//OutputData.Current.Title = title + (MyUser.OnSvn ? Constants.title_s : Constants.title_l);
        }

        public static string OpenFileDlg() 
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            {
                bool? ret = openFileDlg.ShowDialog();
                if (null != ret && ret.Value)
                {
                    return openFileDlg.FileName;
                }
            }
            return null;
        }

        public static string SaveFileDlg()
        {
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            {
                bool? result = saveFileDlg.ShowDialog();
                if (null != result && result.Value)
                {
                    return saveFileDlg.FileName;
                }
            }
            return null;
        }


        public static void ExportSelectedProject(string path)
        {
            List<TreeNode> selectedProjects = TreeHelper.GetSelectedProjects(TreeNode.Root);

            int length = selectedProjects.Count;
            string[] filePaths = new string[length];

            for (int i = 0; i < length; i++)
            {
                filePaths[i] = selectedProjects[i].filePath;
            }

            File.AppendAllLines(path, filePaths);
        }


        public static void OpenExplorer(string path) 
        {
            if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", "/select," + path.Replace("/", "\\"));
            }
            else
            {
				LogManager.Instance.LogError(Constants.Path_No_Exist_Error, path);
            }

        }

        public static void ImportSelectedProjects(string path)
        {
         
            string[] selectedList = File.ReadAllLines(path);
            int length = selectedList.Length;


            if (length > 0)
            {

                List<string> names = null;

                EditMenu.SelectAll(false);

                for (int i = 0; i < length; i++)
                {
                    string selectedPath = selectedList[i];

                    if (selectedPath != null)
                    {
                        selectedPath = TreeHelper.RectifyPath(selectedPath);

                        if (TreeHelper.IsChildPath(selectedPath))
                        {
                            TreeNode child = TreeHelper.FindChildIgnoreDeep(TreeNode.Root, selectedPath);
                            if (child != null)
                            {
                                child.isChecked = true;
                            }

							LogManager.Instance.LogInfomation(selectedPath);
                        }
                        else
                        {
                            names = names == null ? new List<string>() : names;

                            names.Add(selectedPath);
                        }

                    }
                }

                if (names != null)
                {
                    List<TreeNode> projects = TreeHelper.GetALLProjects(TreeNode.Root);

                    foreach (var name in names)
                    {
                        foreach (var project in projects)
                        {
                            if (name.StartsWith(project.fileName, StringComparison.OrdinalIgnoreCase))
                            {
                                project.isChecked = true;
                            }
                        }
                    }
                }
            }
        }

		public static string SvnToLocalPath(string svnPath)
		{
			TreeNode root = TreeNode.SvnRoot;
			string subPath = svnPath.Replace(root.filePath, "");

			switch (MyUser.UserJob)
			{
				case Job.Designer:
					string dRoot = XMLManage.GetString(Util.DESIGNPATH);
					return string.Format("{0}{1}", dRoot, subPath);
				case Job.Modeler:
					string mRoot = XMLManage.GetString(Util.PRODUCTPATH);
					return string.Format("{0}{1}", mRoot, subPath);
				case Job.UnKnow:
					break;
				default:
					break;
			}
			return null;
		}
    }
}
