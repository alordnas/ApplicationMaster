using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Casamia
{
	class Util
	{
		public static string TASK_RECORD_FILE = AppDomain.CurrentDomain.BaseDirectory + "Task/CommonTask.bytes";

		public static string Log_Folder = AppDomain.CurrentDomain.BaseDirectory + "Log";

		public static readonly string PROJECT_PATH_PLACEHOLDER = Properties.Settings.Default.SELECTED_PROJECT_PLACEHOLDER;
		public static readonly string PROJECT_URL_PLACEHOLDER = Properties.Settings.Default.SELECTED_PROJECT_URL_PLACEHOLDER;
		
		public static string EXECUTOR_FORMATTOR = "%EXE_APP_{0}%";


		public static string Unity_Assets = "Assets";
		public static string Unity_ProjectSettings = "ProjectSettings";
		public static string Svn_Dot_Svn = ".svn";


	}
}
