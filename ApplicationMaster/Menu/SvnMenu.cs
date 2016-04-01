﻿using Casamia.MyFacility;
using Casamia.DataSource;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casamia.Core;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Casamia.Model.EventArgs;
using Casamia.Logging;

namespace Casamia.Menu
{
    public class SvnMenu
    {
		static MainWindow mianWindow = App.Current.MainWindow as MainWindow;
        public static void OpenSvn() 
        {
            CommonMethod.SetTitle();

			string url = InputData.Current.Path;
			if (url != null)
			{
				switch (MyUser.UserJob)
				{
					case Job.Designer:

						TreeNode.SvnRoot = new TreeNode(null);
						TreeNode.SvnRoot.isRoot = true;
						TreeNode.SvnRoot.IsSvnNode = true;
						TreeNode.SvnRoot.filePath = url;
						mianWindow.selectedNode = TreeNode.SvnRoot;
						mianWindow.dir_TreeView.ItemsSource = null;
						LogManager.Instance.LogInfomation("正在加载：{0}...", url);

						InputData.Current.Percent = 0;
						
						Build(TreeNode.SvnRoot,
							() =>
							{
								if (MyUser.UserJob == Job.Designer)
								{
									StartChecking();

									mianWindow.dir_TreeView.ItemsSource = TreeNode.SvnRoot.children;

									InputData.Current.Percent = 100;

									InputData.Current.Percent = 0;

									LogManager.Instance.LogInfomation("完成加载：{0}", url);
								}
							},
							3);
						break;
					case Job.Modeler:

						TreeNode.SvnRoot = new TreeNode(null);
						TreeNode.SvnRoot.isRoot = true;
						TreeNode.SvnRoot.IsSvnNode = true;
						TreeNode.SvnRoot.filePath = url;
						mianWindow.dir_TreeView.ItemsSource = TreeNode.SvnRoot.children;
						mianWindow.selectedNode = TreeNode.SvnRoot;
						LogManager.Instance.LogInfomation("正在加载：{0}...", url);

						InputData.Current.Percent = 0;

						Build(TreeNode.SvnRoot,
							() =>
							{
								StartChecking();

								if (MyUser.UserJob == Job.Modeler)
								{
									InputData.Current.Percent = 100;

									InputData.Current.Percent = 0;
								}

								LogManager.Instance.LogInfomation("完成加载：{0}", url);
							},
							1);
						break;
					case Job.UnKnow:

						break;
					default:
						break;
				}	
			}

        }

        public static void CloseSvn() 
        {
			MyUser.OnSvn = false;

            CommonMethod.SetTitle();

			MyUser.ResetUserJob();
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

			Command command = new Command();
			command.Executor = Util.SVN;
			command.Argument = string.Format("list {0}", root.filePath);
			command.CommandFeedbackReceived +=
				(object sender, CommandEventArgs e) =>
				{
					LogManager.Instance.LogDebug(e.Message);
				};
			command.ErrorOccur +=
				(object sender, CommandEventArgs e) =>
				{
					LogManager.Instance.LogError(e.Message);
				};
			command.StatusChanged +=
				(object sender, CommandStatusEventArgs e) =>
				{
					switch(e.NewStatus)
					{	
						case CommandStatus.Completed:
							{

								ObservableCollection<TreeNode> children = ParseListOutPut(root, command.Output);

								if (curDeep < deep)
								{
									for (int i = 0, length = children.Count; i < length; i++)
									{
										TreeNode child = children[i];
										HandleTask(worker, child, deep, curDeep);
									}
								}

								if ((MyUser.UserJob == Job.Designer && deep == 3) || MyUser.UserJob == Job.Modeler && deep == 1)
								{
									InputData.Current.Percent += InputData.Current.Percent < 90 ? 2 : 0;
								}
								else
								{
									worker.OnCompleteAll = null;
								}
							}
							break;
						default:
							break;
					}
				};

			AnTask anTask = new AnTask();

			anTask.AddChild(command);

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

						string local = CommonMethod.SvnToLocalPath(filePath);

						if (Directory.Exists(TreeHelper.RectifyPath(local)))
						{
							worker.resultBools[i] = true;
						}
					}
				},
				() => 
				{
					for (int i = 0,length = worker.resultBools.Length; i < length; i++)
					{
						if (worker.resultBools[i])
						{
							//children[i].icon = new BitmapImage(new Uri("/ProRunner;component/Images/Svn/normal.png", UriKind.Relative));
						}
						else
						{

							children[i].icon = new BitmapImage(new Uri("/ProRunner;component/Images/NewProject.png", UriKind.Relative));
						}
					}
				});
		}


		public static void CheckoutSelectedProjects() 
		{
			if (MyUser.OnSvn)
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
			Command[] tasks = GenerateCheckOutCommand(svnPaths);

			AnTask anTask = new AnTask();

			anTask.AddChildren(tasks);

			TaskWorker worker = new TaskWorker("检出项目");

			worker.AddTask(anTask);

			worker.OnCompleteAll = () =>
			{
				LogManager.Instance.LogInfomation("检出完毕");
			};

			worker.Run();
		}


		private static Command[] GenerateCheckOutCommand(string[] svnProjectPaths) 
		{
			Command[] tasks = new Command[svnProjectPaths.Length];

			int timeout = int.Parse(XMLManage.GetString(Util.MAXIMUM_EXECUTE_TIME));


			for (int i = 0, length = svnProjectPaths.Length; i < length; )
			{
				string svnProjectPath = svnProjectPaths[i];

				string localProjectPaths = CommonMethod.SvnToLocalPath(svnProjectPath);

				//Command cleanupCmd = new Command();
				//cleanupCmd.Executor = Util.SVN;
				//cleanupCmd.Argument = string.Format("cleanup \"{0}\"", localProjectPaths);
				//cleanupCmd.Timeout = TimeSpan.FromSeconds(timeout);
				//tasks[i++] = cleanupCmd;
				//cleanupCmd.CommandFeedbackReceived +=
				//	(object sender, CommandEventArgs e) =>
				//	{
				//		LogManager.Instance.LogDebug(e.Message);
				//	};
				//cleanupCmd.ErrorOccur +=
				//	(object sender, CommandEventArgs e) =>
				//	{
				//		LogManager.Instance.LogError(e.Message);
				//	};
				//cleanupCmd.StatusChanged +=
				//	(object sender, CommandStatusEventArgs e) =>
				//	{
				//		if (e.NewStatus == CommandStatus.Completed)
				//		{
				//			LogManager.Instance.LogInfomation("cleanup {0} completed.", svnProjectPath);
				//		}
				//	};

				Command coCmd = new Command();
				coCmd.Executor = Util.SVN;
				coCmd.Argument = string.Format("checkout {0} \"{1}\"", svnProjectPath, localProjectPaths);
				coCmd.Timeout = TimeSpan.FromSeconds(timeout);
				tasks[i++] = coCmd;
				coCmd.CommandFeedbackReceived +=
					(object sender, CommandEventArgs e) =>
					{
						LogManager.Instance.LogDebug(e.Message);
					};
				coCmd.ErrorOccur +=
					(object sender, CommandEventArgs e) =>
					{
						LogManager.Instance.LogError(e.Message);
					};
				coCmd.StatusChanged +=
					(object sender, CommandStatusEventArgs e) =>
					{
						if(e.NewStatus == CommandStatus.Completed)
						{
							LogManager.Instance.LogInfomation("check out {0} completed.", svnProjectPath);
						}
					};

			}
			return tasks;
		}


    }
}
