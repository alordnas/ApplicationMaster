using Casamia.MyFacility;
using Casamia.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Casamia.Core;

namespace Casamia
{
	class TreeHelper
	{

		public static bool IsOnControl(string dir)
		{
			//if (File.Exists(XMLManage.GetString(Util.SVN)))
			//{
			//	string diff = BackgroundWorkManager.SvnChecking_Diff(dir);

			//	return string.IsNullOrEmpty(diff);
			//}

			return false;
		}



		public static string RectifyPath(string _path)
		{
			if(string.IsNullOrEmpty(_path))
			{
				return string.Empty;
			}
			string path = _path.Replace("\\", "/");

			while (path.EndsWith("/") && path.Length > 0)
			{
				path = path.Substring(0, path.Length - 1);
			}

			if (path.EndsWith(":"))
			{
				path += "/";
			}

			return path;
		}

		public static string CauseError(string dir)
		{
			FileInfo fileInfo = new FileInfo(dir);

			FileAttributes attributes = fileInfo.Attributes;

			if ((attributes & FileAttributes.System) == FileAttributes.System)
			{
				return dir + " 是系统文件，路径不合法";
			}

			if ((attributes & FileAttributes.Hidden) == FileAttributes.System)
			{
				return dir + " 是隐藏文件，路径不合法";
			}

			return null;
		}

		public static bool IsChildPath(string path)
		{
			string dir = WorkSpaceManager.Instance.WorkingPath;

			if (!string.IsNullOrEmpty(dir) && path.StartsWith(TreeHelper.RectifyPath(dir)))
				return true;
			return false;
		}


		public static List<TreeNode> GetSelectedProjects(TreeNode root)
		{
			List<TreeNode> selectedProjects = new List<TreeNode>();

			foreach (var project in GetALLProjects(root))
			{
				if (project.isChecked)
				{
					selectedProjects.Add(project);
				}
			}

			return selectedProjects;
		}

		public static List<TreeNode> GetSelectedLeaves(TreeNode root)
		{
			List<TreeNode> selectedProjects = new List<TreeNode>();

			foreach (var project in GetALLLeaves(root))
			{
				if (project.isChecked)
				{
					selectedProjects.Add(project);
				}
			}

			return selectedProjects;
		}

		public static string[] GetTreeNodePaths(List<TreeNode> nodes)
		{

			string[] projectPaths = new string[nodes.Count];

			for (int i = 0, length = nodes.Count; i < length; i++)
			{
				projectPaths[i] = nodes[i].filePath;
			}

			return projectPaths;
		}



		public static List<TreeNode> GetALLProjects(TreeNode root)
		{
			List<TreeNode> projects = new List<TreeNode>();

			SearchLeaves(root, projects, true);

			return projects;
		}

		public static void GetAllNodes(List<TreeNode> nodes, TreeNode root)
		{
			var children = root.children;

			foreach (var child in children)
			{
				nodes.Add(child);

				GetAllNodes(nodes, child);
			}
		}


		public static List<TreeNode> GetALLLeaves(TreeNode root)
		{
			List<TreeNode> projects = new List<TreeNode>();

			SearchLeaves(root, projects, false);

			return projects;
		}

		private static void SearchLeaves(TreeNode root, List<TreeNode> projects, bool onlyProject)
		{
			if (root != null)
			{

				foreach (var child in root.children)
				{
					if (child.isLeaf)
					{
						if (onlyProject)
						{
							if (child.isProject)
							{
								projects.Add(child);
							}
						}
						else
						{
							projects.Add(child);
						}
					}

					SearchLeaves(child, projects, onlyProject);
				}
			}
		}





		/// <summary>
		/// 设置字节点选中
		/// </summary>
		/// <param name="isChecked"></param>
		public static void SetChildrenChecked(TreeNode parent, bool isChecked)
		{
			foreach (TreeNode child in parent.children)
			{
				child.isChecked = isChecked;

				SetChildrenChecked(child, isChecked);
			}
		}


		public static void InvertSelection(TreeNode parent)
		{
			foreach (TreeNode child in parent.children)
			{
				child.isChecked = !child.isChecked;

				InvertSelection(child);
			}
		}

		public static void SetChildrenExpanded(TreeNode parent, bool expanded)
		{
			//foreach (TreeNode child in parent.children)
			//{
			//    child.is = isChecked;
			//    SetChildrenChecked(child, isChecked);
			//}
		}

		public static TreeNode FindChild(TreeNode parent, string dir)
		{
			foreach (TreeNode child in parent.children)
			{
				if (child.filePath.Equals(dir, StringComparison.OrdinalIgnoreCase))
				{
					return child;
				}
			}
			return null;
		}

		/// <summary>
		/// 执行这个函数之前，需要执行：RectifyPath(dir) ,IsChildPath(dir)
		/// </summary>
		/// <param name="root"></param>
		/// <param name="dir"></param>
		/// <returns></returns>
		public static TreeNode FindChildIgnoreDeep(TreeNode root, string dir)
		{
			string filePath = root.filePath;
			if (filePath.Equals(dir, System.StringComparison.OrdinalIgnoreCase))
			{
				return root;
			}

			if (filePath.Length < dir.Length)
			{
				int index = dir.IndexOf("/", filePath.Length + 1);

				string path = dir;
				if (index > 0)
				{
					path = dir.Substring(0, index);
				}
				TreeNode node = FindChild(root, path);
				if (node != null)
				{
					return FindChildIgnoreDeep(node, dir);
				}
			}
			return null;

		}

		/// <summary>
		/// 向上搜寻类型为<T>的ViualTree节点
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
		{
			while (source != null && source.GetType() != typeof(T))
				source = VisualTreeHelper.GetParent(source);

			return source;
		}



	}
}
