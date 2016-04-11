using Casamia.Model;
using Casamia.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Casamia.Core;

namespace Casamia.MyFacility
{
	public class MyUser
	{

		private static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		public static Job UserJob = Job.UnKnow;

		private static bool onSvn = false;

		public static bool OnSvn
		{
			get
			{
				return onSvn;
			}
			set
			{
				if (onSvn != value)
				{
					onSvn = value;
					WorkSpaceManager.Instance.IsLocal = !value;
				}
			}
		}

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
			WorkSpaceManager.Instance.SetCurrent("UnityDesigner");
			UserJob = Job.Designer;
			if (OnSvn)
			{
				SvnMenu.OpenSvn();
			}
			else
			{
				mainWindow.RefreshWholeTree();
			}
		}

		public static void SwitchToModeller()
		{
			WorkSpaceManager.Instance.SetCurrent("UnityModel");
			UserJob = Job.Modeler;
			if (OnSvn)
			{
				SvnMenu.OpenSvn();
			}
			else
			{
				mainWindow.RefreshWholeTree();
			}
		}

		public static string GetUserName()
		{
			return UserJob == Job.Designer ? Constants.Design : Constants.Modelling;
		}
	}
}
