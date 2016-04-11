using Casamia.MyFacility;
using Casamia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Casamia.Core;

namespace Casamia.Menu
{
    public class EditMenu
    {
        public static void SelectAll(bool check)
        {
			if (!WorkSpaceManager.Instance.IsLocal)
			{
				if (TreeNode.SvnRoot != null)
				{
					TreeHelper.SetChildrenChecked(TreeNode.SvnRoot, check);
				}
			}
			else
			{
				if (TreeNode.Root != null)
				{
					TreeHelper.SetChildrenChecked(TreeNode.Root, check);
				}
			}

            
        }

        public static void ExpandTree(TreeView treeView,bool expand)
        {
            if (treeView != null) 
            {
                ExpandInternal(treeView,expand);
            }
        }

        static void ExpandInternal(DependencyObject targetItemContainer, bool expand) 
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(targetItemContainer);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject dpObject = VisualTreeHelper.GetChild(targetItemContainer, i);

                if (dpObject is TreeViewItem)
                {
                    TreeViewItem item = dpObject as TreeViewItem;
                    item.IsExpanded = expand;
                    item.UpdateLayout();
                }
                ExpandInternal(dpObject, expand);
            }
        }

		public static void ExportDirList()
		{
			string filePath = CommonMethod.SaveFileDlg();
			if (!string.IsNullOrEmpty(filePath))
			{
				CommonMethod.ExportSelectedProject(filePath);
			}
		}

		public static void ImportDirList()
		{
			string filePath = CommonMethod.OpenFileDlg();
			if (!string.IsNullOrEmpty(filePath))
			{
				CommonMethod.ImportSelectedProjects(filePath);
				MainWindow mainWindow = App.Current.MainWindow as MainWindow;
				ExpandTree(mainWindow.dir_TreeView,true);
				mainWindow.status_TextBlock.Text = string.Format("已选：{0}", TreeHelper.GetSelectedLeaves(
					WorkSpaceManager.Instance.IsLocal ? TreeNode.Root : TreeNode.SvnRoot).Count);
			}
		}
    }
}
