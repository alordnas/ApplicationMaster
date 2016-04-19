using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Casamia.Model;
using Casamia.Logging;
using Casamia.MyFacility;
using Casamia.Core;

namespace Casamia
{
	/// <summary>
	/// ReferenceWindow.xaml 的交互逻辑
	/// </summary>
	public partial class WorkspaceWindow : Window
	{
		public WorkspaceWindow()
		{
			InitializeComponent();
		}

		protected override void OnClosed(System.EventArgs e)
		{
			base.OnClosed(e);
			WorkSpaceManager.Instance.Save();
		}

	}
}
