using Casamia.DataSource;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Casamia.MyFacility;
using Casamia.Core;
using Casamia.Logging;


namespace Casamia
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public static CreateProjectWindow Current;

        public CreateProjectWindow(string parentPath)
        {
            InitializeComponent();

            InitializeCaseData(parentPath);
        }

        private void InitializeCaseData(string parentPath)
        {
            CreateCaseData.Current = (CreateCaseData)FindResource("caseData");

            CreateCaseData.Current.ParentPath = parentPath;

			CreateCaseData.Current.ProjectStyle = MyUser.GetUserName();

        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void create_Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CreateCaseData.Current.ProjectName))
            {
				LogManager.Instance.LogError("请输入项目名称!");

                fileName_TextBox.Focus();
                return;
            }

			if (CreateCaseData.Current.IsGoodFolder())
			{
				Creator projectCreator = new Creator();

				projectCreator.Create();
			}
			else
			{
				LogManager.Instance.LogError("项目路径不符合要求，请选择正确的创建文件夹");
			}

			Close();
			
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Current = null;
        }

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

    }
}
