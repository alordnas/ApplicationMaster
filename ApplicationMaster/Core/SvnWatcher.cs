using Casamia.DataSource;
using Casamia.MyFacility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Casamia.Core
{
	public class SvnWatcher
	{
		private static TaskWorker worker = new TaskWorker(null);

		private static List<TreeNode> treeNodes = new List<TreeNode>();

		private static List<bool> onControl = new List<bool>();


		public SvnWatcher(TreeNode node) 
		{
			treeNodes.Add(node);

			if (MyWorker.isSvnExeReady)
			{
				CommonTask.SvnCheckDiff(worker, node.filePath,
					() => 
					{
						onControl.Add(true);
					},
					() => 
					{
						onControl.Add(false);
					}
					);

				worker.OnCompleteAll = () => 
				{
					if (worker.anyTask && !worker.IsBusy)
					{
						worker.Run();
					}
					else
					{
						ReseSetIcon();
					}
				};

				if (treeNodes.Count == 1)
				{
					worker.Run();
				}
			}
		}

		private void ReseSetIcon()
		{

			for (int i = 0, length = treeNodes.Count; i < length; i++)
			{
				TreeNode treeNode = treeNodes[i];

				if (onControl[i])
				{
					treeNode.icon = new BitmapImage(new Uri("/ProRunner;component/Images/Svn/normal.png", UriKind.Relative));
				}
				else
				{
					treeNode.icon = new BitmapImage(new Uri("/ProRunner;component/Images/non-versioned.png", UriKind.Relative));
				}

			}

			
		}
	}
}
