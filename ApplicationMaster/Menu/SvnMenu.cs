using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

using Newtonsoft.Json;

using Casamia.Core;
using Casamia.Model;
using Casamia.Logging;
using Casamia.Model.EventArgs;
using Casamia.MyFacility;

namespace Casamia.Menu
{
	public class SvnMenu
	{
		static MainWindow mianWindow = App.Current.MainWindow as MainWindow;
		public static void OpenSvn()
		{
			CommonMethod.SetTitle();

			TreeNode.SvnRoot = new TreeNode(null);
			TreeNode.SvnRoot.isRoot = true;
			TreeNode.SvnRoot.IsSvnNode = true;
			TreeNode.SvnRoot.filePath = WorkSpaceManager.Instance.WorkingPath;
			mianWindow.selectedNode = TreeNode.SvnRoot;
			mianWindow.dir_TreeView.ItemsSource = null;
			mianWindow.dir_TreeView.ItemsSource = TreeNode.SvnRoot.children;
			LogManager.Instance.LogInfomation("正在加载：{0}...", WorkSpaceManager.Instance.WorkingPath);

			Build(
				TreeNode.SvnRoot,
				() =>
				{
					StartChecking();
					LogManager.Instance.LogInfomation("完成加载：{0}", WorkSpaceManager.Instance.WorkingPath);
				},
				WorkSpaceManager.Instance.WorkingDepth
			);
		}

		private static void Build(TreeNode root, Action onCompleted, int deep)
		{
			TaskWorker worker = new TaskWorker(null);

			HandleTask(worker, root, deep);

			worker.OnCompleteAll = () =>
			{
				if (worker.anyTask)
				{
					worker.Run();
				}
				else
				{
					if (onCompleted != null)
					{
						onCompleted();
					}
				}
			};

			worker.Run();
		}

		private static void HandleTask(TaskWorker worker, TreeNode root, int deep, int curDeep = 0)
		{
			curDeep++;

			AnTask anTask = TaskManager.GetEmbeddedTask("SVN_LIST_TASK");
			TaskManager.NormalizeTask(anTask, root.filePath);
			if (null != anTask && null != anTask.Commands && anTask.Commands.Length > 0)
			{
				Command command = anTask.Commands[0];
				command.StatusChanged +=
					(object sender, CommandStatusEventArgs e) =>
					{
						switch (e.NewStatus)
						{
							case CommandStatus.Completed:
								{
									ObservableCollection<TreeNode> children = ParseListOutPut(root, command.Result);

									if (curDeep < deep)
									{
										for (int i = 0, length = children.Count; i < length; i++)
										{
											TreeNode child = children[i];
											HandleTask(worker, child, deep, curDeep);
										}
									}
									if (curDeep != deep)
									{
										worker.OnCompleteAll = null;
									}
								}
								break;
							default:
								break;
						}
					};
			}

			worker.AddTask(anTask);
		}

		private static ObservableCollection<TreeNode> ParseListOutPut(TreeNode parent, string output)
		{
			string[] names = output.Split(new char[] { '\n', '/' },
				StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0, length = names.Length; i < length; i++)
			{
				string name = names[i].Trim(new char[] { '\r', '\n' });

				if (!string.IsNullOrWhiteSpace(name) && !name.Contains("."))
				{
					string svnPath = string.Format("{0}/{1}", parent.filePath, name);

					TreeNode treeNode = new TreeNode(parent);

					treeNode.IsSvnNode = true;

					treeNode.filePath = svnPath;

					parent.children.Add(treeNode);
				}
			}
			return parent.children;
		}

		private static void StartChecking()
		{

			SilentWorker worker = new SilentWorker();

			List<TreeNode> children = TreeHelper.GetALLLeaves(TreeNode.SvnRoot);

			string[] filePaths = TreeHelper.GetTreeNodePaths(children);

			worker.Do(
				() =>
				{
					worker.resultBools = new bool[filePaths.Length];

					for (int i = 0, length = filePaths.Length; i < length; i++)
					{
						string filePath = filePaths[i];

						string local = WorkSpaceManager.Instance.Current.ToLocalPath(filePath);

						if (Directory.Exists(TreeHelper.RectifyPath(local)))
						{
							worker.resultBools[i] = true;
						}
					}
				},
				() =>
				{
					for (int i = 0, length = worker.resultBools.Length; i < length; i++)
					{
						if (worker.resultBools[i])
						{
							//children[i].icon = new BitmapImage(new Uri("/ProRunner;component/Images/Svn/normal.png", UriKind.Relative));
						}
						else
						{
							children[i].icon = new BitmapImage(new Uri("Images/NewProject.png", UriKind.Relative));
						}
					}
				});
		}


		public static void CheckoutSelectedProjects()
		{
			if (!WorkSpaceManager.Instance.IsLocal)
			{
				List<TreeNode> projects = TreeHelper.GetSelectedLeaves(TreeNode.SvnRoot);

				string[] svnPaths = TreeHelper.GetTreeNodePaths(projects);

				CheckoutProjects(svnPaths);
			}
			else
			{
				LogManager.Instance.LogError("检出时，请在SVN模式下的列表中选择需要检出的项目");
			}
		}

		public static void CheckoutProjects(string[] svnPaths)
		{
			for (int i = 0; i < svnPaths.Length; i++)
			{

				AnTask anTask = TaskManager.GetEmbeddedTask("CheckoutProject");
				TaskManager.NormalizeTask(anTask, svnPaths[i]);
				if (null != anTask)
				{
					TaskWorker worker = new TaskWorker("检出项目");
					worker.AddTask(anTask);

					worker.OnCompleteAll = () =>
					{
						LogManager.Instance.LogInfomation("检出完毕");
					};

					worker.Run();
				}

			}
		}

	}
}
