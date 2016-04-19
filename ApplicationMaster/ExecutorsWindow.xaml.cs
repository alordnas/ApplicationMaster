using System.Windows;

using Casamia.Core;

namespace Casamia
{
	/// <summary>
	/// Interaction logic for ExecutorsWindow.xaml
	/// </summary>
	public partial class ExecutorsWindow : Window
	{
		public ExecutorsWindow()
		{
			InitializeComponent();
		}

		private void buttonAddClick(object sender, RoutedEventArgs e)
		{
			ExecutorManager.Instance.Add(new Model.Executor()
			{
				Name="New Executor",
				Description = "",
			});
		}

		private void buttonRemoveClick(object sender, RoutedEventArgs e)
		{
			
		}
	}
}
