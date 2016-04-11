using System.ComponentModel;
using System.IO;

using Casamia.Model;
using Casamia.Logging;

namespace Casamia.MyFacility
{
	public class MyWorker
	{

		public static BackgroundWorker bgWorker;

		public static string unityExe;

		public static string svnExe;

		public static bool isSvnExeReady;

		public static bool isUnityExeReady;

		public static int timeout = -1;

		public static void Initialize()
		{
			svnExe = XMLManage.GetString(Util.SVN);


			if (svnExe != null)
			{
				if (File.Exists(svnExe))
				{
					isSvnExeReady = true;
					LogManager.Instance.LogDebug("Svn 就绪");
				}
				else
				{
					isSvnExeReady = false;
					LogManager.Instance.LogError(Constants.Path_No_Found_Error, svnExe);
				}
			}
			else
			{
				isSvnExeReady = false;
				LogManager.Instance.LogError("Svn 未配置。");
			}

			unityExe = XMLManage.GetString(Util.UNITY);

			if (unityExe != null)
			{
				if (File.Exists(unityExe))
				{
					isUnityExeReady = true;
					LogManager.Instance.LogDebug("Unity 就绪");
				}
				else
				{
					isUnityExeReady = false;
					LogManager.Instance.LogError(Constants.Path_No_Found_Error, unityExe);
				}
			}
			else
			{
				isUnityExeReady = false;
				LogManager.Instance.LogError("Unity 未配置。");
			}

			if (!int.TryParse(XMLManage.GetString(Util.MAXIMUM_EXECUTE_TIME), out timeout)) 
			{
				Parameter p = new Parameter();
				p.Key = Util.MAXIMUM_EXECUTE_TIME;
				p.Value = "-1";
				XMLManage.SaveParameter(p);
				timeout = -1;
			}
		}

		public static bool IsBusy
		{
			get
			{
				if (bgWorker != null && bgWorker.IsBusy)
					return true;

				return false;
			}
			
		}
	}
}
