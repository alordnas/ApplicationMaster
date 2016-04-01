using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Casamia.DataSource;
using Casamia.Logging;
using Casamia.MyFacility;

namespace Casamia
{
	/// <summary>
	/// ReferenceWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ReferenceWindow : Window
	{

		public ReferenceWindow()
		{
			InitializeComponent();

			userJob.Items.Add(Constants.Designer);
			userJob.Items.Add(Constants.Modeller);

			bool isDesigner;
			if (XMLManage.TryGetBool(Util.Saved_UserJob, out isDesigner))
			{
				if (isDesigner)
				{
					userJob.Text = Constants.Designer;
				}
				else
				{
					userJob.Text = Constants.Modeller;

				}
			}

			designSvnPath.Text = XMLManage.GetString(Util.DESIGNURL);
			modellingSvnPath.Text = XMLManage.GetString(Util.PRODUCTURL);

			unityExePath.Text = XMLManage.GetString(Util.UNITY);
			svnExePath.Text = XMLManage.GetString(Util.SVN);

			timeoutSetting.Text = XMLManage.GetString(Util.MAXIMUM_EXECUTE_TIME);

		}

		private void closeButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void minimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = System.Windows.WindowState.Minimized;
		}


		private void cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void change1_Click(object sender, RoutedEventArgs e)
		{
			if (designSvnPath.IsEnabled)
			{
				Parameter parameter = new Parameter();
				parameter.Key = Util.DESIGNURL;
				parameter.Value = designSvnPath.Text;
				XMLManage.SaveParameter(parameter);

				change1.Content = "修改";
				designSvnPath.IsEnabled = false;

			}
			else
			{
				designSvnPath.IsEnabled = true;
				change1.Content = "应用";
			}
		}

		private void change2_Click(object sender, RoutedEventArgs e)
		{
			if (modellingSvnPath.IsEnabled)
			{
				Parameter parameter = new Parameter();
				parameter.Key = Util.PRODUCTURL;
				parameter.Value = modellingSvnPath.Text;
				XMLManage.SaveParameter(parameter);

				change2.Content = "修改";
				modellingSvnPath.IsEnabled = false;

			}
			else
			{
				modellingSvnPath.IsEnabled = true;
				change2.Content = "应用";
			}
		}

		private void change3_Click(object sender, RoutedEventArgs e)
		{
			if (unityExePath.IsEnabled)
			{
				if (File.Exists(unityExePath.Text))
				{
					Parameter parameter = new Parameter();
					parameter.Key = Util.UNITY;
					parameter.Value = unityExePath.Text;
					XMLManage.SaveParameter(parameter);

					change3.Content = "修改";
					unityExePath.IsEnabled = false;
				}
				else
				{
					LogManager.Instance.LogError(Constants.Path_No_Exist_Error, unityExePath.Text);
					unityExePath.Focus();
				}
			}
			else
			{
				unityExePath.IsEnabled = true;
				change3.Content = "应用";
			}
		}

		private void change4_Click(object sender, RoutedEventArgs e)
		{
			if (svnExePath.IsEnabled)
			{

				if (File.Exists(svnExePath.Text))
				{
					Parameter parameter = new Parameter();
					parameter.Key = Util.SVN;
					parameter.Value = svnExePath.Text;
					XMLManage.SaveParameter(parameter);

					change4.Content = "修改";
					svnExePath.IsEnabled = false;
				}
				else
				{
					LogManager.Instance.LogError(Constants.Path_No_Exist_Error, svnExePath.Text);
					svnExePath.Focus();
				}
			}
			else
			{
				svnExePath.IsEnabled = true;
				change4.Content = "应用";
			}
		}

		private void userJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (userJob.SelectedIndex == 0)
			{
				XMLManage.SaveBool(Util.Saved_UserJob, true);
			}
			else
			{
				XMLManage.SaveBool(Util.Saved_UserJob, false);
			}
		}

		private void timeoutSetting_TextChanged(object sender, TextChangedEventArgs e)
		{
			int t;
			if (XMLManage.GetString(Util.MAXIMUM_EXECUTE_TIME).Equals(timeoutSetting.Text))
			{
				change5.Visibility = System.Windows.Visibility.Hidden;
			}
			else if (int.TryParse(timeoutSetting.Text, out t))
			{
				change5.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				change5.Visibility = System.Windows.Visibility.Hidden;
			}
		}

		private void change5_Click(object sender, RoutedEventArgs e)
		{
			Parameter p = new Parameter();

			p.Key = Util.MAXIMUM_EXECUTE_TIME;

			p.Value = timeoutSetting.Text;

			XMLManage.SaveParameter(p);

			MyWorker.timeout = int.Parse(timeoutSetting.Text);

			change5.Visibility = System.Windows.Visibility.Hidden;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}
	}
}
