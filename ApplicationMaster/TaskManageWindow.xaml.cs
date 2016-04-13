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

		#region EVENT_HANDLER

		private void task_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			AnTask anTask = task_ListBox.SelectedValue as AnTask;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void exit_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion EVENT_HANDLER


	}
}
