using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Casamia.Core;
using Casamia.DataSource;
using Casamia.Logging;
using Casamia.Model.EventArgs;
using Casamia.MyFacility;

namespace Casamia
{
	/// <summary>
	/// TaskManageWindow.xaml 的交互逻辑
	/// </summary>
	public partial class TaskManageWindow : Window
	{

		MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		List<Command> currentSubTasks = new List<Command>();

		string[] svnSubCommand = null;

		public TaskManageWindow()
		{
			InitializeComponent();

			string[] exer = new string[]
            {
                Util.UNITY,
                Util.SVN
            };


			exe_ComboBox.ItemsSource = exer;

			RefreshTaskNameListBox();

		}



		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void minimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = System.Windows.WindowState.Minimized;
		}

		private void addTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(taskName_TextBox.Text))
			{
				LogManager.Instance.LogError("任务名不能为空");
				taskName_TextBox.Focus();
				return;
			}

			string taskName = taskName_TextBox.Text;

			if (CommonTask.CommonTasks.ContainsKey(taskName))
			{
				LogManager.Instance.LogError("任务：{0}已存在", taskName);
				return;
			}

			AnTask anTask = new AnTask();

			anTask.AddChildren(currentSubTasks);

			anTask.Description = string.IsNullOrEmpty(describe_TextBox.Text) ? "无" : describe_TextBox.Text;

			CommonTask.CommonTasks.Add(taskName, anTask);

			CommonTask.Flush();

			RefreshTaskNameListBox();

			task_ListBox.SelectedIndex = task_ListBox.Items.Count - 1;

			LogManager.Instance.LogInfomation("新建任务：{0}", taskName);

			addTask_Button.IsEnabled = false;

		}


		private void RefreshTaskNameListBox()
		{
			string[] taskNames = new string[CommonTask.CommonTasks.Count];

			int index = 0;
			foreach (var item in CommonTask.CommonTasks.Keys)
			{
				taskNames[index] = item.ToString();
				index++;
			}

			task_ListBox.ItemsSource = taskNames;
			task_ListBox.UpdateLayout();
		}

		private void RefreshSubTaskListBox()
		{
			string[] subTasks = new string[currentSubTasks.Count];

			for (int i = 0, length = subTasks.Length; i < length; i++)
			{
				subTasks[i] = currentSubTasks[i].ToString();
			}
			subTask_ListBox.ItemsSource = subTasks;
			subTask_ListBox.UpdateLayout();

			if (currentSubTasks.Count > 0)
			{
				clearSubTask_Button.IsEnabled = true;
			}
			else
			{
				clearSubTask_Button.IsEnabled = false;
			}

			if (task_ListBox.SelectedValue != null)
			{
				saveTask_Button.IsEnabled = true;
			}
			else
			{
				saveTask_Button.IsEnabled = false;
			}

		}

		private void task_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox listBox = sender as ListBox;
			if (listBox.SelectedValue != null)
			{
				taskName_TextBox.Text = listBox.SelectedValue.ToString();

				AnTask anTask = CommonTask.CommonTasks[taskName_TextBox.Text];

				currentSubTasks.Clear();

				currentSubTasks.AddRange(anTask.Commands);

				RefreshSubTaskListBox();

				deleteTask_Button.IsEnabled = true;
			}
			else
			{
				deleteTask_Button.IsEnabled = false;
			}

			saveTask_Button.IsEnabled = false;
		}

		private void addSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (CheckSubTask())
			{
				Command subTask = new Command();
				subTask.Executor = string.Format("{0}", exe_ComboBox.Text.ToLower());
				subTask.Argument = arg_Combox.Text;
				subTask.Timeout = int.Parse(timeout_TextBox.Text);
				currentSubTasks.Add(subTask);
				RefreshSubTaskListBox();
			}
		}


		private bool CheckSubTask()
		{
			if (string.IsNullOrWhiteSpace(exe_ComboBox.Text))
			{
				LogManager.Instance.LogError("[运行程序]选项不能为空");
				exe_ComboBox.Focus();
				return false;
			}

			if (string.IsNullOrWhiteSpace(arg_Combox.Text))
			{
				LogManager.Instance.LogError("[参数]选项不能为空");
				arg_Combox.Focus();
				return false;
			}

			int timeout = 0;
			if (!string.IsNullOrWhiteSpace(timeout_TextBox.Text) && int.TryParse(timeout_TextBox.Text, out timeout))
			{

				return true;
			}
			else
			{
				LogManager.Instance.LogError("请在超时栏填入数字（负值有效，表示无最大运行时间限制）");
				return false;
			}
		}


		private void coverSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (CheckSubTask())
			{
				int index = subTask_ListBox.SelectedIndex;

				if (0 <= index && index < currentSubTasks.Count)
				{
					Command subTask = new Command();
					subTask.Executor = string.Format("{0}", exe_ComboBox.Text.ToLower());
					subTask.Argument = arg_Combox.Text;
					subTask.Timeout = int.Parse(timeout_TextBox.Text);
					currentSubTasks[index] = subTask;
					RefreshSubTaskListBox();

					coverSubTask_Button.IsEnabled = false;
				}
				else
				{
					LogManager.Instance.LogWarning("请在任务列表中选择需要覆盖的子任务");
				}
			}
		}

		private void saveTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (task_ListBox.SelectedValue != null)
			{
				string selected = task_ListBox.SelectedValue.ToString();


				if (taskName_TextBox.Text != selected)
				{
					CommonTask.CommonTasks.Remove(selected);

					AnTask anTask = new AnTask();

					anTask.AddChildren(currentSubTasks);

					anTask.Description = string.IsNullOrEmpty(describe_TextBox.Text) ? "无" : describe_TextBox.Text;

					CommonTask.CommonTasks.Add(taskName_TextBox.Text, anTask);

					RefreshTaskNameListBox();

					LogManager.Instance.LogInfomation(
						"任务：{0}  重命名为：{1}",
						selected,
						taskName_TextBox.Text
						);
				}
				else
				{
					CommonTask.CommonTasks[selected].Clear();
					CommonTask.CommonTasks[selected].AddChildren(currentSubTasks);
				}
				CommonTask.Flush();
				LogManager.Instance.LogInfomation("任务：{0} 已修改", selected);

				saveTask_Button.IsEnabled = false;
				addTask_Button.IsEnabled = false;
			}
		}

		private void deleteTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (task_ListBox.SelectedValue != null)
			{
				string taskName = task_ListBox.SelectedValue.ToString();

				if (CommonTask.CommonTasks.Remove(taskName))
				{
					LogManager.Instance.LogInfomation("任务：{0} 已删除！", taskName);

					RefreshTaskNameListBox();

					currentSubTasks.Clear();

					taskName_TextBox.Text = string.Empty;

					RefreshSubTaskListBox();

					addTask_Button.IsEnabled = false;

					CommonTask.Flush();
				}
			}
		}

		private void exit_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void taskName_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (task_ListBox.SelectedValue != null && task_ListBox.SelectedValue.ToString().Equals(taskName_TextBox.Text))
			{
				return;
			}

			if (task_ListBox.SelectedValue == null)
			{
				saveTask_Button.IsEnabled = false;
			}
			else
			{
				saveTask_Button.IsEnabled = true;
			}

			if (!string.IsNullOrWhiteSpace(taskName_TextBox.Text))
			{
				addTask_Button.IsEnabled = true;
			}
			else
			{
				addTask_Button.IsEnabled = true;
			}
		}

		private void deleteSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (0 <= subTask_ListBox.SelectedIndex && subTask_ListBox.SelectedIndex < currentSubTasks.Count)
			{
				currentSubTasks.RemoveAt(subTask_ListBox.SelectedIndex);
				RefreshSubTaskListBox();

			}
		}

		private void clearSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			currentSubTasks.Clear();
			RefreshSubTaskListBox();
		}

		private void subTask_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (subTask_ListBox.SelectedValue != null)
			{

				Command anTask = currentSubTasks[subTask_ListBox.SelectedIndex];

				exe_ComboBox.Text = anTask.Executor;
				arg_Combox.Text = anTask.Argument;
				timeout_TextBox.Text = anTask.Timeout.ToString();

				deleteSubTask_Button.IsEnabled = true;
				coverSubTask_Button.IsEnabled = true;
				addSubTask_Button.IsEnabled = true;
			}
			else
			{
				deleteSubTask_Button.IsEnabled = false;
				coverSubTask_Button.IsEnabled = false;
			}

		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void exe_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			addSubTask_Button.IsEnabled = true;

			if (exe_ComboBox.SelectedValue != null && exe_ComboBox.SelectedValue.ToString() == Util.SVN)
			{
				arg_Combox.ItemsSource = svnSubCommand;
			}
			else
			{
				string[] unityArgments = new string[]
				{
					"-projectPath %projectpath%",
					"-logFile %projectpath%",
					"-exportPackage <exportAssetPath1 ExportAssetPath2 exportFileName>",
					"-importPackage <pathname>",
					"-executeMethod <ClassName.MethodName>",
					"-createProject %projectpath%",
					"-batchmode",
					"-quit",
					"-nographics (Windows only)",
					"-silent-crashes"
				};

				arg_Combox.ItemsSource = unityArgments;
			}


		}

		private void Window_Loaded(object windowSender, RoutedEventArgs ev)
		{
			if (MyWorker.isSvnExeReady)
			{
				TaskWorker worker = new TaskWorker(null);

				Command task = new Command();

				task.Executor = Util.SVN;

				task.Argument = "help";

				task.StatusChanged +=
					(object sender, CommandStatusEventArgs e) =>
					{
						if (e.NewStatus == CommandStatus.Completed)
						{
							RecordSvnCommands(task.Output);
						}
					};
				task.ErrorOccur += (object sender, CommandEventArgs e) =>
				{
					LogManager.Instance.LogError(e.Message);
				};
				AnTask anTask = new AnTask();

				anTask.AddChild(task);

				worker.AddTask(anTask);

				worker.Run();
			}

		}



		private void RecordSvnCommands(string input)
		{
			int add_index = input.IndexOf("add");
			int upgrade_index = input.IndexOf("upgrade");

			string command = input.Substring(add_index, upgrade_index - add_index + 7);

			string[] commands = command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

			svnSubCommand = new string[commands.Length];

			for (int i = 0, length = commands.Length; i < length; i++)
			{
				svnSubCommand[i] = commands[i].Trim();

				int _index = svnSubCommand[i].IndexOf('(');
				if (_index > 0)
				{
					svnSubCommand[i] = svnSubCommand[i].Substring(0, _index);
				}
			}
		}

		private void addPath_Button_Click(object sender, RoutedEventArgs e)
		{
			arg_Combox.Text += " " + Util.PROJECT_PATH_PLACEHOLDER;

			arg_Combox.Text = arg_Combox.Text.Trim();
		}


		private void timeout_TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (coverSubTask_Button != null)
			{
				coverSubTask_Button.IsEnabled = true;
			}

			if (addSubTask_Button != null)
			{
				addSubTask_Button.IsEnabled = true;
			}
		}

	}
}
