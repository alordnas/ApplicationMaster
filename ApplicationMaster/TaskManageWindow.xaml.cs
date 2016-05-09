using System;
using System.Windows;

using Casamia.Core;
using Casamia.Model;

namespace Casamia.View
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
	}
}
