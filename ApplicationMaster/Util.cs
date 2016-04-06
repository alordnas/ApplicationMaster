using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Casamia
{
	class Util
	{

		public static string RUNNER_CONFIG_FILE = AppDomain.CurrentDomain.BaseDirectory + "RunnerConfig.xml";

		public static string COMMAND_RECORD_FOLDER = AppDomain.CurrentDomain.BaseDirectory + "Command";

		public static string TASK_RECORD_FILE = AppDomain.CurrentDomain.BaseDirectory + "Task/CommonTask.bytes";

		public static string Package_Folder = AppDomain.CurrentDomain.BaseDirectory + "Package";

		public static string Log_Folder = AppDomain.CurrentDomain.BaseDirectory + "Log";

		public static readonly string PROJECT_PATH_PLACEHOLDER = Properties.Settings.Default.SELECTED_PROJECT_PLACEHOLDER;
		public static readonly string PROJECT_URL_PLACEHOLDER = Properties.Settings.Default.SELECTED_PROJECT_URL_PLACEHOLDER;

		public static string Design_Package_Path
		{
			get
			{
				string d_path = XMLManage.GetString("Design_Package_Path");
				if (d_path != null)
				{
					return d_path;
				}
				else
				{
					return string.Format("\\Package\\Design.unitypackage"); ;
				}
			}
		}
		public static string Model_Package_Path
		{
			get
			{
				string m_path = XMLManage.GetString("Model_Package_Path");
				if (m_path != null)
				{
					return m_path;
				}
				else
				{
					return string.Format("/Package/Model.unitypackage");
				}
			}
		}
		public static string Package_Extansion = ".unitypackage";

		public static string PRODUCTPATH = "productPath";
		public static string DESIGNPATH = "designPath";
		public static string UNITY = "%EXE_APP_UNITY%";
		public static string SVN = "%EXE_APP_SVN%";
		public static string EXTERNALPATH = "externalPath";
		public static string IGNOREPATTREN = "ignorePattern";
		public static readonly int TREE_DEEP_LITIM = 3;
		public static readonly string MAXIMUM_EXECUTE_TIME = "MaximumExecuteTime";

		public static string PRODUCTURL = "productUrl";
		public static string DESIGNURL = "designUrl";

		public static string EXECUTOR_FORMATTOR = "%EXE_APP_{0}%";


		public static string Unity_Assets = "Assets";
		public static string Unity_ProjectSettings = "ProjectSettings";
		public static string Svn_Dot_Svn = ".svn";

		public static string SAVED_SYNC_SVN = "syncToSvn";
		public static string SAVED_PACKAGE_DIR = "savedPackageDir";
		public static string SAVED_MAPS_CREATE_REQUEST = "createMaps";
		public static string SAVED_PREFERENCE_CREATE_REQUEST = "createPreference";

		public static string Saved_UserJob = "UserJob";
	}
}
