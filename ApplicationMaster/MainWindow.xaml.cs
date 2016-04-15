using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;

using Casamia.Core;
using Casamia.Logging;
using Casamia.Menu;
using Casamia.Model;
using Casamia.MyFacility;

namespace Casamia
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{

		readonly string OUTPUT_DATA = "outputData";
		readonly string INPUT_DATA = "inputData";

		public readonly int cornerWidth = 8;
		public Point mousePoint = new Point();
		public readonly int customBorderThickness = 7;

		private delegate void BuildTreeDelegate(TreeNode childTree);
		private ObservableCollection<AnTask> activeTask = new ObservableCollection<AnTask>();

		List<string> commands = new List<string>();
		private ObservableCollection<AnTask> activeTasks = new ObservableCollection<AnTask>();

		public TreeNode selectedNode = null;

		static System.Threading.Mutex _mutex;
		public MainWindow()
		{
			try
			{
				InitializeComponent();

				LogManager.Instance.SetLogger(new MyConsole(LogGrid));
				LogManager.Instance.AllowLevel = Log.level.Error | Log.level.Infomation | Log.level.Waring;

				this.SourceInitialized += MainWindow_SourceInitialized;
				filterListBox.ItemsSource = Enum.GetValues(typeof(Casamia.Logging.Log.level));
				CommonMethod.SetTitle();
				MyLog.Initialize();
				MyWorker.Initialize();
				task_DataGrid.ItemsSource = activeTasks;
				TaskManager.ActivateTask += (AnTask anTask) =>
				{
					this.Dispatcher.BeginInvoke(new Action(() =>
					{
						activeTasks.Add(anTask);
					}));
				};
				RefreshWholeTree();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error");
				System.Environment.Exit(0);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			
		}
		private void lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Log.level newlevel = Log.level.None;
			for (int i = 0; i < filterListBox.SelectedItems.Count; i++)
			{
				object selectItem = filterListBox.SelectedItems[i];
				if (selectItem is Log.level)
				{
					newlevel |= (Log.level)filterListBox.SelectedItems[i];
				}
			}

			LogManager.Instance.AllowLevel = newlevel;
		}

		public void RefreshWholeTree()
		{
			string workPath = WorkSpaceManager.Instance.WorkingPath;
			if (!Directory.Exists(workPath))
			{
				LogManager.Instance.LogError(Constants.Path_No_Exist_Error, workPath);
				dir_TreeView.ItemsSource = null;
				return;
			}

			TreeNode.Root = new TreeNode(null);
			TreeNode.Root.isRoot = true;
			TreeNode.Root.filePath = workPath;

			this.Dispatcher.BeginInvoke(new BuildTreeDelegate(BuildTree), TreeNode.Root);

			dir_TreeView.ItemsSource = TreeNode.Root.children;

			selectedNode = TreeNode.Root;
		}

		#region<<customs>>


		/// <summary>
		/// 构造树
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="parent"></param>
		private void BuildTree(TreeNode parent)
		{
			if (parent.isDeepLimited || parent.isProject)
			{
				return;
			}

			string anyError = TreeHelper.CauseError(parent.filePath);

			if (string.IsNullOrEmpty(anyError))
			{
				string[] directories = Directory.GetDirectories(parent.filePath, "*", SearchOption.TopDirectoryOnly);

				for (int i = 0, length = directories.Length; i < length; i++)
				{
					string dir = directories[i];

					TreeNode node = new TreeNode(parent);

					node.filePath = dir;

					parent.children.Add(node);

					BuildTree(node);
				}
			}
			else
			{
				LogManager.Instance.LogError(anyError);
			}
		}

		public void AddTreeNodeChild(TreeNode parent, string childDir)
		{
			if (childDir.EndsWith(Util.Unity_Assets) ||
				childDir.EndsWith(Util.Unity_ProjectSettings) ||
				childDir.EndsWith(Util.Svn_Dot_Svn))
			{
				return;
			}

			Dispatcher.BeginInvoke(new Action(() =>
			{
				TreeNode child = new TreeNode(parent);

				child.filePath = childDir;

				parent.children.Add(child);

			}));
		}

		public void RenameTreeNodeChild(TreeNode parent, string oldDir, string newDir)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				oldDir = TreeHelper.RectifyPath(oldDir);
				TreeNode node = TreeHelper.FindChild(parent, oldDir);

				if (node != null)
				{
					node.filePath = newDir;

					LogManager.Instance.LogInfomation("File: {0} renamed to {1}", oldDir, node.filePath);
				}
			}));
		}

		public void DeleteTreeNodeChild(TreeNode parent, string dir)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				TreeNode node = TreeHelper.FindChild(parent, TreeHelper.RectifyPath(dir));

				if (node != null)
				{
					parent.children.Remove(node);
					if (node.isProject)
					{
						LogManager.Instance.LogInfomation("File: {0} Delete", node.filePath);
					}
				}
			}));
		}

		#endregion

		#region <<Window Events>>
		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			if (MyWorker.IsBusy)
			{
				MessageBoxResult result = MessageBox.Show("任务未完成，确定退出吗？", "提示", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					Application.Current.Shutdown();
				}
			}
			else
			{
				Application.Current.Shutdown();
			}
		}

		void OnProtoTaskClick(object sender, RoutedEventArgs e)
		{
			AnTask anTask = (sender as MenuItem).Header as AnTask;
			if (null == anTask) return;

			string[] projectPaths = null;
			List<TreeNode> nodes = TreeHelper.GetSelectedProjects(TreeNode.Root);
			projectPaths = TreeHelper.GetTreeNodePaths(nodes);
			CommonTask.RunTask(anTask, projectPaths);
		}

		/// <summary>
		/// 树节点获取焦点时显示背景色
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var treeViewItem = TreeHelper.VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
			if (treeViewItem != null)
			{
				treeViewItem.Focus();
			}
		}


		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			TreeNode node = (sender as CheckBox).DataContext as TreeNode;

			TreeHelper.SetChildrenChecked(node, true);

			if (!WorkSpaceManager.Instance.IsLocal)
			{
				status_TextBlock.Text = string.Format("已选：{0}", TreeHelper.GetSelectedLeaves(TreeNode.SvnRoot).Count);
			}
			else
			{
				status_TextBlock.Text = string.Format("已选：{0}", TreeHelper.GetSelectedLeaves(TreeNode.Root).Count);
			}

			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				if (TreeNode.LastCheckedNode != null)
				{
					List<TreeNode> projects = null;

					if (!WorkSpaceManager.Instance.IsLocal)
					{
						projects = TreeHelper.GetALLLeaves(TreeNode.SvnRoot);
					}
					else
					{
						projects = TreeHelper.GetALLLeaves(TreeNode.Root);
					}

					TreeNode lastCheckedNode = TreeNode.LastCheckedNode;

					int indexFirst = projects.IndexOf(lastCheckedNode);

					int indexSecond = projects.IndexOf(node);

					if (indexSecond < indexFirst)
					{
						int temp = indexFirst;
						indexFirst = indexSecond;
						indexSecond = temp;
					}

					for (int i = indexFirst; i <= indexSecond; i++)
					{
						if (projects[i].isProject)
						{
							projects[i].isChecked = true;
						}
					}
				}
			}

			TreeNode.LastCheckedNode = node;
		}
		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			TreeNode node = (sender as CheckBox).DataContext as TreeNode;

			TreeHelper.SetChildrenChecked(node, false);

			if (!WorkSpaceManager.Instance.IsLocal)
			{
				status_TextBlock.Text = string.Format("已选：{0}", TreeHelper.GetSelectedLeaves(TreeNode.SvnRoot).Count);
			}
			else
			{
				status_TextBlock.Text = string.Format("已选：{0}", TreeHelper.GetSelectedLeaves(TreeNode.Root).Count);
			}

		}


		void MainWindow_SourceInitialized(object sender, EventArgs e)
		{
			HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
			if (source == null)
				// Should never be null  
				throw new Exception("Cannot get HwndSource instance.");

			source.AddHook(new HwndSourceHook(this.WndProc));
		}
		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case Win32.WM_GETMINMAXINFO: // WM_GETMINMAXINFO message  
					Win32.WmGetMinMaxInfo(hwnd, lParam, this);
					handled = true;
					break;
				case Win32.WM_NCHITTEST: // WM_NCHITTEST message  
					IntPtr intPtr = Win32.WmNCHitTest(lParam, ref handled, this);
					return intPtr;
			}
			return IntPtr.Zero;
		}

		#endregion

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (!string.IsNullOrEmpty(quickExec_Textbox.Text))
				{
					commands.Add(quickExec_Textbox.Text);

					string[] projectPaths = TreeHelper.GetTreeNodePaths(TreeHelper.GetSelectedProjects(TreeNode.Root));

					CommonTask.RunCommand(quickExec_Textbox.Text, projectPaths);
				}
			}

			if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				MyHotkeys.OnCtrlKeyDown(e.Key);
			}

		}
		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Up)
			{
				if (!string.IsNullOrEmpty(quickExec_Textbox.Text))
				{
					int index = commands.IndexOf(quickExec_Textbox.Text);
					if (0 < index)
					{
						quickExec_Textbox.Text = commands[index - 1];
					}
				}
			}
			if (e.Key == Key.Down)
			{
				if (!string.IsNullOrEmpty(quickExec_Textbox.Text))
				{
					int index = commands.IndexOf(quickExec_Textbox.Text);

					if (0 <= index && index < commands.Count - 1)
					{
						quickExec_Textbox.Text = commands[index + 1];
					}
				}
			}
		}

		private void openDir_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			FileMenu.OpenDir();
		}


		private void selectAll_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditMenu.SelectAll(true);
		}


		private void selectNone_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditMenu.SelectAll(false);
		}

		private void openSvn_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			WorkSpaceManager.Instance.IsLocal = false;
			SvnMenu.OpenSvn();
		}

		private void closeSvn_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			WorkSpaceManager.Instance.IsLocal = true;
			RefreshWholeTree();
		}

		private void exportDir_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditMenu.ExportDirList();
		}

		private void importDir_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditMenu.ImportDirList();
		}

		private void expand_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditMenu.ExpandTree(dir_TreeView, true);
		}

		private void shrink_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			EditMenu.ExpandTree(dir_TreeView, false);

		}

		private void createUnity_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (!WorkSpaceManager.Instance.IsLocal)
			{
				LogManager.Instance.LogError("创建项目时需要你退出SVN模式");
				return;
			}

			if (selectedNode != null)
			{
				UnityMenu.CreateUnityProject(selectedNode.filePath);
			}
			else
			{
				LogManager.Instance.LogError(
					Constants.Path_No_Exist_Error, 
					WorkSpaceManager.Instance.WorkingPath
					);
			}
		}

		private void dir_TreeView_DragEnter(object sender, DragEventArgs e)
		{
			string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
			if (path != null)
			{
				LogManager.Instance.LogInfomation("临时目录：{0}", path);
				RefreshWholeTree();
			}
		}

		private void log_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MyLog.ChangeLogText(log_ComboBox.SelectedValue.ToString());
		}
		
		private ContextMenu GetContextMenu_Level_Svn()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("checkout_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			item.Header = "检出" + selectedNode.fileName;

			parent.Items.Add(item);

			item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);


			return parent;
		}

		private ContextMenu GetContextMenu_Level_Svn_Normal()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);


			return parent;
		}


		private ContextMenu GetContextMenu_Level_Modelling()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("createProject_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			return parent;
		}


		private ContextMenu GetContextMenu_Level_Design()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			parent.Items.Add(new Separator());

			item = FindResource("expand_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			item = FindResource("shrink_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			return parent;
		}

		private ContextMenu GetContextMenu_Level1()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			return parent;
		}


		private ContextMenu GetContextMenu_Level2()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("createProject_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			return parent;
		}

		private ContextMenu GetContextMenu_Level3()
		{
			ContextMenu parent = new ContextMenu();

			MenuItem item = FindResource("openExplorer_MenuItem") as MenuItem;
			if (item.Parent != null)
			{
				ContextMenu oldParent = item.Parent as ContextMenu;

				oldParent.Items.Clear();
			}
			parent.Items.Add(item);

			return parent;
		}

		private void openExplorer_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			string path = WorkSpaceManager.Instance.Current.ToLocalPath(selectedNode.filePath);
			if (selectedNode != null)
			{
				CommonMethod.OpenExplorer(path);
			}
			else
			{
				LogManager.Instance.LogError(
					Constants.Path_No_Exist_Error,
					path
					);
			}
		}

		private void OnWorkspaceClick(object sender, RoutedEventArgs e)
		{
			MenuItem button = sender as MenuItem;
			if(null != button)
			{
				WorkSpaceManager.Instance.SetCurrent(button.Header as string);

				if (WorkSpaceManager.Instance.IsLocal)
				{
					RefreshWholeTree();
				}
				else
				{
					SvnMenu.OpenSvn();
				}
			}
		}

		private void dir_TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			TreeView treeView = sender as TreeView;

			var element = treeView.InputHitTest(e.GetPosition(sender as IInputElement));

			if (element is Grid)
			{
				if (treeView.ContextMenu == null || !treeView.ContextMenu.HasItems)
				{
					treeView.ContextMenu = GetContextMenu_Level_Design();
				}
			}
		}

		private void treeItem_StackPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			var treeViewItem = TreeHelper.VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;

			if (treeViewItem != null)
			{
				if (treeViewItem.DataContext is TreeNode)
				{
					TreeNode node = treeViewItem.DataContext as TreeNode;

					selectedNode = node;


					if (selectedNode.IsSvnNode)
					{
						if (selectedNode.isLeaf)
						{
							treeViewItem.ContextMenu = GetContextMenu_Level_Svn();
						}
						else
						{
							treeViewItem.ContextMenu = GetContextMenu_Level_Svn_Normal();
						}
					}
					else
					{
						if (treeViewItem.ContextMenu == null || !treeViewItem.ContextMenu.HasItems)
						{
							if (node.isProject)
							{
								treeViewItem.ContextMenu = GetContextMenu_Level3();
							}
							else if (!node.parent.isRoot)
							{
								treeViewItem.ContextMenu = GetContextMenu_Level2();
							}
							else
							{
								treeViewItem.ContextMenu = GetContextMenu_Level1();
							}
						}
					}
				}
			}
		}

		private void item_StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			TreeNode node = (sender as StackPanel).DataContext as TreeNode;

			if (!!WorkSpaceManager.Instance.IsLocal)
			{
				if (node.isProject)
				{
					node = node.parent;
				}

				if (CreateCaseData.Current != null)
				{
					CreateCaseData.Current.ParentPath = node.ToString();
				}

				selectedNode = node;

				LogManager.Instance.LogDebug(selectedNode.filePath);
			}
		}

		private void preference_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			new ReferenceWindow().ShowDialog();
		}

		private void taskManager_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			new TaskManageWindow().ShowDialog();
		}

		private void checkout_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SvnMenu.CheckoutSelectedProjects();
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{

		}

		private void checkoutSelected_MenuItem_Click(object sender, RoutedEventArgs e)
		{
			SvnMenu.CheckoutProjects(new string[] { selectedNode.filePath });
		}


		private void taskDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			DataGrid taskDataGrid = sender as DataGrid;

			var selectedCells = taskDataGrid.SelectedCells;

			if (selectedCells != null && 0 < selectedCells.Count)
			{
				if (selectedCells[0].Item != null && selectedCells[0].Item is AnTask)
				{
					AnTask item = selectedCells[0].Item as AnTask;
					commandDataGrid.ItemsSource = item.Commands;
				}
				else
				{
					commandDataGrid.ItemsSource = null;
				}

				stopTask_Button.IsEnabled = true;
				redoTask_Button.IsEnabled = true;
				parallel_Button.IsEnabled = true;
			}
			else
			{
				commandDataGrid.ItemsSource = null;
				stopTask_Button.IsEnabled = false;
				redoTask_Button.IsEnabled = false;
				parallel_Button.IsEnabled = false;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			MyLog.Save();
		}

		private void taskLog_DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			DataGrid taskDataGrid = sender as DataGrid;

			var selectedCells = taskDataGrid.SelectedCells;

			if (selectedCells != null && 0 < selectedCells.Count)
			{
				if (selectedCells[0].Item != null && selectedCells[0].Item is AnTask)
				{
					AnTask item = selectedCells[0].Item as AnTask;

					subTaskLog_DataGrid.ItemsSource = item.Commands;
				}
				else
				{
					subTaskLog_DataGrid.DataContext = null;
				}
			}
			else
			{
				subTaskLog_DataGrid.DataContext = null;
			}
		}

		private void checkOutput_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void checkError_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		private void stopTask_Button_Click(object sender, RoutedEventArgs e)
		{
			IList<DataGridCellInfo> cells = task_DataGrid.SelectedCells;

			if (cells != null && 0 < cells.Count)
			{

				for (int i = 0, length = cells.Count; i < length; i = i + 7)
				{
					DataGridCellInfo cell = cells[i];

					AnTask anTask = cell.Item as AnTask;
					if (anTask != null)
					{
						List<Command> subTasks = new List<Command>(anTask.Commands);

						if (subTasks != null)
						{
							foreach (var subTask in subTasks)
							{
								if (CommandStatus.Waiting == CommandStatus.Waiting)
								{
									subTask.Status = CommandStatus.Cancel;
								}
							}
						}
					}
				}
			}
		}

		private void redoTask_Button_Click(object sender, RoutedEventArgs e)
		{
			IList<DataGridCellInfo> cells = task_DataGrid.SelectedCells;

			if (cells != null && 0 < cells.Count)
			{
				List<AnTask> tasks = new List<AnTask>();
				for (int i = 0, length = cells.Count; i < length; i = i + 7)
				{
					DataGridCellInfo cell = cells[i];
					AnTask anTask = cell.Item as AnTask;
					if(null != anTask )
					{
						tasks.Add(anTask);
					}
				}

				TaskWorker worker = new TaskWorker("重做");
				worker.AddTasks(tasks.ToArray());
				worker.Run();
			}
		}

		private void parallel_Button_Click(object sender, RoutedEventArgs e)
		{
			IList<DataGridCellInfo> cells = task_DataGrid.SelectedCells;

			if (cells != null && 0 < cells.Count)
			{
				List<AnTask> anTasks = new List<AnTask>();

				for (int i = 0, length = cells.Count; i < length; i = i + 7)
				{
					DataGridCellInfo cell = cells[i];
					AnTask item = cell.Item as AnTask;

					if (item.Status == CommandStatus.Waiting)
					{
						TaskManager.RemoveTask(item);
						anTasks.Add(item.Clone() as AnTask);
					}
				}

				TaskManager.ParallelTask(anTasks.ToArray());
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			LogManager.Instance.Clear();
		}

	}
}
