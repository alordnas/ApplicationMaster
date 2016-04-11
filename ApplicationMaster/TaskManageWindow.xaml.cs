using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Casamia.Core;
using Casamia.Model;
using Casamia.Logging;
using Casamia.Model;

namespace Casamia
{
	/// <summary>
	/// TaskManageWindow.xaml 的交互逻辑
	/// </summary>
	public partial class TaskManageWindow : Window
	{
		ObservableCollection<Command> currentCommands = new ObservableCollection<Command>();

		public TaskManageWindow()
		{
			InitializeComponent();

			if (exe_ComboBox.Items.Count>0)
			{
				exe_ComboBox.SelectedIndex = 0;
			}
			commandListBox.ItemsSource = currentCommands;
		}

		#region EVENT_HANDLER

		#endregion EVENT_HANDLER


		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
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

			AnTask anTask = new AnTask()
			{
				Name = taskName,
				Description = string.IsNullOrEmpty(describe_TextBox.Text) ? "无" : describe_TextBox.Text,
			};
			anTask.AddCommands(currentCommands);
			TaskManager.AddProtoTask(anTask);
			TaskManager.SaveProtoTasks();
			task_ListBox.SelectedItem = anTask;
		}

		private void task_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AnTask anTask = task_ListBox.SelectedValue as AnTask;
			if (null != anTask)
			{
				currentCommands = new ObservableCollection<Command>(anTask.Commands);
				commandListBox.ItemsSource = currentCommands;
			}
		}

		private void addSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			if (CheckSubTask())
			{
				Command command = new Command();
				Executor executor = exe_ComboBox.SelectedItem as Executor;
				if (null != executor)
				{
					command.Executor = executor.PlaceHolder;
					command.Argument = arg_Combox.Text;
					command.Timeout = TimeSpan.FromSeconds(int.Parse(timeout_TextBox.Text));
					currentCommands.Add(command);
				}
			}
		}


		private bool CheckSubTask()
		{
			if (null == exe_ComboBox.SelectedItem)
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
				Command command= commandListBox.SelectedItem as Command;

				if (null != command)
				{
					Executor executor = exe_ComboBox.SelectedItem as Executor;
					if (null != executor)
					{
						command.Executor = executor.PlaceHolder;
						command.Argument = arg_Combox.Text;
						command.Timeout = TimeSpan.FromSeconds(int.Parse(timeout_TextBox.Text));
						coverSubTask_Button.IsEnabled = false;
					}
				}
				else
				{
					LogManager.Instance.LogWarning("请在任务列表中选择需要覆盖的子任务");
				}
			}
		}

		private void saveTask_Button_Click(object sender, RoutedEventArgs e)
		{
			AnTask task = task_ListBox.SelectedItem as AnTask;
			if (null != task)
			{
				task.Name = taskName_TextBox.Text;
				task.Description = describe_TextBox.Text;
			}
			TaskManager.SaveProtoTasks();
		}

		private void deleteTask_Button_Click(object sender, RoutedEventArgs e)
		{
			AnTask task = task_ListBox.SelectedItem as AnTask;
			TaskManager.RemoveProtoTask(task);
			TaskManager.SaveProtoTasks();
		}

		private void exit_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		
		private void deleteSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			Command command = commandListBox.SelectedItem as Command;
			currentCommands.Remove(command);
		}

		private void clearSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			currentCommands.Clear();
		}

		private void subTask_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Command command = commandListBox.SelectedValue as Command;
			if (null != command)
			{
				exe_ComboBox.SelectedItem = ExecutorManager.Instance.GetByPlaceHolder(command.Executor);
				arg_Combox.Text = command.Argument;
				timeout_TextBox.Text = ((int)command.Timeout.TotalSeconds).ToString();
			}
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
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
