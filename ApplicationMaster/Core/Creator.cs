using Casamia.MyFacility;
using Casamia.DataSource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casamia.Logging;
using Casamia.Model.EventArgs;

namespace Casamia.Core
{
	class Creator
	{
		string projectPath;

		string svnRoot;

		bool IsDesignMode(string type)
		{
			return type.Contains("designer");
		}

		bool IsFurnitureMode(string type)
		{
			return type.Contains("modeler");
		}

		public void Create()
		{
			projectPath = CreateCaseData.Current.GetProjcetPath();
			if (projectPath == null)
			{
				Casamia.Logging.LogManager.Instance.LogError("逻辑错误，请联系程序维护人员。");
				return;
			}

			svnRoot = CreateCaseData.Current.SvnRootPath;

			Casamia.Logging.LogManager.Instance.LogInfomation("正在创建{0}... ", projectPath);

			if (Directory.Exists(projectPath))
			{
				Casamia.Logging.LogManager.Instance.LogError(
					Constants.Path_Already_Exist_Error,
					XMLManage.GetString(Util.UNITY)
					);
				return;
			}

			TaskWorker worker = new TaskWorker(Constants.TASK_NAME_CREATEPROJECT);

			worker.OnCompleteAll = () =>
			{
				LogManager.Instance.LogInfomation("创建完毕{0}", projectPath);
			};

			TaskWorker svnChecker = new TaskWorker(null);

			CommonTask.SvnCheckDiff(svnChecker, Path.GetDirectoryName(projectPath),
				   () =>
				   {
					   if (IsDesignMode(WorkSpaceManager.Instance.Current.Ext))
					   {
						   worker.AddTask(GetCreateDesignUnityTask(projectPath));
					   }
					   else if (IsFurnitureMode(WorkSpaceManager.Instance.Current.Ext))
					   {
						   worker.AddTask(GetCreateFurnitureUnityTask(projectPath));
					   }

					   worker.Run();
				   },
				   () =>
				   {
					   if (IsDesignMode(WorkSpaceManager.Instance.Current.Ext))
					   {
						   worker.AddTask(GetImportDesignUnityTask(projectPath));
					   }
					   else if (IsFurnitureMode(WorkSpaceManager.Instance.Current.Ext))
					   {
						   worker.AddTask(GetImportFurnitureUnityTask(projectPath));
					   }
					   worker.Run();
				   });

			svnChecker.Run();
		}

		private AnTask GetCreateDesignUnityTask(string projectPath)
		{
			AnTask anTask = Newtonsoft.Json.JsonConvert.DeserializeObject<AnTask>(
				Properties.Settings.Default.TASK_CREATE_UNITY_DESIGN_PROJECT
				);
			TaskManager.NormalizeTask(anTask, projectPath);
			return anTask;
		}

		private AnTask GetImportDesignUnityTask(string projectPath)
		{
			string text = Properties.Settings.Default.TASK_IMPORT_UNITY_DESIGN_PROJECT;
			AnTask anTask = Newtonsoft.Json.JsonConvert.DeserializeObject<AnTask>(
				text
				);
			TaskManager.NormalizeTask(anTask, projectPath);
			return anTask;
		}

		private AnTask GetCreateFurnitureUnityTask(string projectPath)
		{
			AnTask anTask = Newtonsoft.Json.JsonConvert.DeserializeObject<AnTask>(
				Properties.Settings.Default.TASK_CREATE_UNITY_FURNITURE_PROJECT
				);
			TaskManager.NormalizeTask(anTask, projectPath);
			return anTask;
		}

		private AnTask GetImportFurnitureUnityTask(string projectPath)
		{
			AnTask anTask = Newtonsoft.Json.JsonConvert.DeserializeObject<AnTask>(
				Properties.Settings.Default.TASK_IMPORT_UNITY_FURNITURE_PROJECT
				);
			TaskManager.NormalizeTask(anTask, projectPath);
			return anTask;
		}
	}
}
