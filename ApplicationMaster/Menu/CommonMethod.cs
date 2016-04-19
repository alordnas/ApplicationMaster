using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Casamia.Core;
using Casamia.Model;
using Casamia.Logging;
using Casamia.MyFacility;

namespace Casamia.Menu
{
	public class CommonMethod
	{

		public static void SetTitle()
		{
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
	}
}
