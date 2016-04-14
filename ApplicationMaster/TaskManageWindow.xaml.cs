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

		public TaskManageWindow()
		{
			InitializeComponent();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			TaskManager.SaveProtoTasks();
		}

		private void addTask_Button_Click(object sender, RoutedEventArgs e)
		{
			string taskName = string.Format("Task #{0}", TaskManager.ProtoTasks.Count);
			AnTask anTask = new AnTask()
			{
				Name = taskName,
			};
			TaskManager.AddProtoTask(anTask);
			TaskManager.SaveProtoTasks();
			task_ListBox.ItemsSource = TaskManager.ProtoTasks;
		}

		private void deleteTask_Button_Click(object sender, RoutedEventArgs e)
		{
			AnTask anTask = task_ListBox.SelectedItem as AnTask;
			if (null != anTask)
			{
				TaskManager.RemoveProtoTask(anTask);
				TaskManager.SaveProtoTasks();
				task_ListBox.ItemsSource = TaskManager.ProtoTasks;
			}
		}
	}
}
