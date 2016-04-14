using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using Casamia.Core;
using Casamia.Logging;
using Casamia.Model;

namespace Casamia
{
	/// <summary>
	/// Interaction logic for TaskEditor.xaml
	/// </summary>
	public partial class TaskEditor : UserControl
	{
		
		ObservableCollection<Command> currentCommands = new ObservableCollection<Command>();
		public TaskEditor()
		{
			InitializeComponent();
			if (exe_ComboBox.Items.Count > 0)
			{
				exe_ComboBox.SelectedIndex = 0;
			}
			//commandListBox.ItemsSource = currentCommands;
		}

		#region PUBLIC
		
		#endregion PUBLIC


		#region EVENTHANDLER

		private void addSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			AnTask anTask = DataContext as AnTask;
			Executor executor = exe_ComboBox.SelectedItem as Executor;
			if (null != anTask && null != executor)
			{
				Command command = new Command()
				{
					Executor = executor.PlaceHolder,
					Timeout = new TimeSpan(0, 0, -1),
				};
				commandListBox.SelectedItem = command;
				anTask.AddCommand(command);
			}
		}
		
		private void deleteSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			AnTask anTask = DataContext as AnTask;
			Command command = commandListBox.SelectedItem as Command;
			if (null != anTask)
			{
				anTask.RemoveCommand(command);
			}
		}

		private void clearSubTask_Button_Click(object sender, RoutedEventArgs e)
		{
			AnTask anTask = DataContext as AnTask;
			if (null != anTask)
			{
				anTask.Clear();
			}
		}

		private void addPath_Button_Click(object sender, RoutedEventArgs e)
		{
			arg_Combox.Text += " " + Util.PROJECT_PATH_PLACEHOLDER;
			arg_Combox.Text = arg_Combox.Text.Trim();
		}

		#endregion EVENTHANDLER
	}
}
