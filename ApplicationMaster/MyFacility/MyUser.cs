using Casamia.DataSource;
using Casamia.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Casamia.MyFacility
{
	public class MyUser
	{

		private static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		public static Job UserJob = Job.UnKnow;

		public static bool OnSvn = false;

		public static void Initialize() 
		{
			bool isUserADesigner;
			if (XMLManage.TryGetBool(Util.Saved_UserJob, out isUserADesigner))
			{
				if (isUserADesigner)
				{
                    SaveUserJob(Job.Designer);
				}
				else
				{
                    SaveUserJob(Job.Modeler);
				}
			}
			else
			{
				new User().ShowDialog();
			}
		}

		public static void SaveUserJob(Job job)
		{
			switch (job)
			{
				case Job.Designer:
                    SwitchToDesigner();
					break;
				case Job.Modeler:
                    SwitchToModeller();
					break;
				case Job.UnKnow:
					break;
				default:
					break;
			}
		}

		public static void SwitchUserJob()
		{
			switch (UserJob)
			{
				case Job.Designer:
					SwitchToModeller();
					break;
				case Job.Modeler:
					SwitchToDesigner();
					break;
				case Job.UnKnow:
					new User().Show();
					break;
				default:
					break;
			}
		}

		public static void ResetUserJob()
		{
			SaveUserJob(UserJob);
		}


		public static void SwitchToDesigner(string dafault = null) 
		{
			XMLManage.SaveBool(Util.Saved_UserJob, true);

			InputData.Current.UserJob = Constants.Design + "：";
            UserJob = Job.Designer;
			if (OnSvn)
			{
				InputData.Current.Path = XMLManage.GetString(Util.DESIGNURL);
				SvnMenu.OpenSvn();
			}
			else
			{
				InputData.Current.Path = XMLManage.GetString(Util.DESIGNPATH);
				mainWindow.RefreshWholeTree();
			}
		}

        public static void SwitchToModeller() 
		{
			XMLManage.SaveBool(Util.Saved_UserJob, false);

			InputData.Current.UserJob = Constants.Modelling + "：";
			UserJob = Job.Modeler;
			if (OnSvn)
			{
				InputData.Current.Path = XMLManage.GetString(Util.PRODUCTURL);
				SvnMenu.OpenSvn();
			}
			else
			{
				InputData.Current.Path = XMLManage.GetString(Util.PRODUCTPATH);
				mainWindow.RefreshWholeTree();
			}
		}

		public static string GetUserName() 
		{
			return UserJob == Job.Designer ? Constants.Design : Constants.Modelling;
		}
	}
}
