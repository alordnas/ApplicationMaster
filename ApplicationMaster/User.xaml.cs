using Casamia.Model;
using Casamia.MyFacility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Casamia
{
	/// <summary>
	/// Interaction logic for User.xaml
	/// </summary>
	public partial class User : Window
	{

		public static User Current;

		public User()
		{
			InitializeComponent();
		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void minimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void Window_Closed(object sender, EventArgs e)
		{

		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void desing_Button_Click(object sender, RoutedEventArgs e)
		{
			MyUser.SaveUserJob(Job.Designer);

			Close();
		}

		private void product_Button_Click(object sender, RoutedEventArgs e)
		{
			MyUser.SaveUserJob(Job.Modeler);

			Close();
		}
	}
}
