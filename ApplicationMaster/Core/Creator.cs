using Casamia.MyFacility;
using Casamia.Model;
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
					   worker.AddTask(GetCreateUnityTask(projectPath));

					   worker.Run();
				   },
				   () =>
				   {
					   worker.AddTask(GetImportUnityTask(projectPath));
					   worker.Run();
				   });

			svnChecker.Run();
		}
		
		private AnTask GetImportUnityTask(string projectPath)
		{
			AnTask anTask = WorkSpaceManager.Instance.Current.ImportProjectTask.Clone() as AnTask;
			TaskManager.NormalizeTask(anTask, projectPath);
			return anTask;
		}

		private AnTask GetCreateUnityTask(string projectPath)
		{
			AnTask anTask = WorkSpaceManager.Instance.Current.CreateProjectTask.Clone() as AnTask;
			TaskManager.NormalizeTask(anTask, projectPath);
			return anTask;
		}
	}
}
