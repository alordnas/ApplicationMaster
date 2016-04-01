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
                       worker.AddTask(GetAddTask(projectPath));

                       worker.Run();
                   },
                   () =>
                   {
                       worker.AddTask(GetImportTask(projectPath));

                       worker.Run();
                   });

			svnChecker.Run();
		}

        private Command GetCreateUnityProject(string projectPath) 
        {
            Command anTask = new Command();

            anTask.Executor = Util.UNITY;

            string packageFullPath = CreateCaseData.Current.GetImportPackagePath();

            if (!File.Exists(packageFullPath))
            {
				LogManager.Instance.LogError("丢失：{0}", packageFullPath);
                anTask.Argument = string.Format("-quit -createProject \"{0}\" -BatchMode -exit", projectPath);
            }
            else
                anTask.Argument = string.Format("-quit -createProject \"{0}\" -importPackage \"{1}\" -BatchMode -exit", projectPath, packageFullPath);

            return anTask;
        }
		
		private AnTask GetAddTask(string projectPath) 
		{
			AnTask anTask = new AnTask();

			string[] arguments = new string[]
			{
				string.Format("add \"{0}\"", projectPath),
				string.Format("propset svn:ignore -F {0} \"{1}\"", XMLManage.GetString(Util.IGNOREPATTREN), projectPath),
				string.Format("revert --depth=infinity \"{0}\\Library\"", projectPath),
				string.Format("propset svn:externals \"{0}\\Assets\" -F {1}", projectPath, XMLManage.GetString(Util.EXTERNALPATH)),
				string.Format("update \"{0}\"", projectPath)
			};

			Command subTask = GetCreateUnityProject(projectPath);
			subTask.StatusChanged +=
				(object sender, CommandStatusEventArgs e) =>
				{
					if (e.NewStatus == CommandStatus.Completed)
					{
						string[] otherPaths = CreateCaseData.Current.GetPathsNeedCreated(projectPath);
						for (int i = 0, length = otherPaths.Length; i < length; i++)
						{
							System.IO.Directory.CreateDirectory(otherPaths[i]);
						}
					}
				};
			subTask.ErrorOccur += (object sender, CommandEventArgs e) =>
			{
				LogManager.Instance.LogError(e.Message);
			}; 
			anTask.AddChild(subTask);

            for (int i = 0,length = arguments.Length; i < length; i++)
            {
                subTask = new Command();

                subTask.Executor = Util.SVN;

                subTask.Argument = arguments[i];
				subTask.ErrorOccur += (object sender, CommandEventArgs e) =>
				{
					LogManager.Instance.LogError(e.Message);
				}; 

				anTask.AddChild(subTask);
            }
			return anTask;
		}


		private AnTask GetImportTask(string projectPath)
		{
			AnTask anTask = new AnTask();

            string svnPath = string.Format("{0}/{1}", svnRoot, CreateCaseData.Current.ProjectName);


            string[] arguments = new string[]
			{
				string.Format("import {0} {1} -m \"Initial import\"", projectPath, svnPath),
				string.Format("checkout {0} {1}", svnPath, projectPath),
				string.Format("propset svn:ignore -F {0} \"{1}\"", XMLManage.GetString(Util.IGNOREPATTREN), projectPath),
				string.Format("revert --depth=infinity \"{0}\\Library\"", projectPath),
				string.Format("propset svn:externals \"{0}\\Assets\" -F {1}", projectPath, XMLManage.GetString(Util.EXTERNALPATH)),
				string.Format("update \"{0}\"", projectPath),
				string.Format("commit {0} -m \"Add extenal\"", projectPath)
			};

			Command subTask = GetCreateUnityProject(projectPath);

			subTask.StatusChanged +=
				(object sender, CommandStatusEventArgs e) =>
				{
					if (e.NewStatus == CommandStatus.Completed)
					{
						string[] otherPaths = CreateCaseData.Current.GetPathsNeedCreated(projectPath);

						for (int i = 0, length = otherPaths.Length; i < length; i++)
						{
							System.IO.Directory.CreateDirectory(otherPaths[i]);
						}

						Directory.Delete(string.Format("{0}/Library", projectPath), true);

						if (Directory.Exists(string.Format("{0}/Temp", projectPath)))
						{
							Directory.Delete(string.Format("{0}/Temp", projectPath), true);
						}
					}
				};
			subTask.ErrorOccur += (object sender, CommandEventArgs e) =>
			{
				LogManager.Instance.LogError(e.Message);
			}; 

			anTask.AddChild(subTask);

            for (int i = 0, length = arguments.Length; i < length; i++)
            {
				subTask = new Command();

				subTask.Executor = Util.SVN;

				subTask.Argument = arguments[i];

				if (i == 0)
				{
					subTask.StatusChanged +=
						(object sender, CommandStatusEventArgs e) =>
						{
							if (e.NewStatus == CommandStatus.Completed)
							{
								Directory.Delete(projectPath, true);
								Directory.CreateDirectory(projectPath);
							}
						};
					subTask.ErrorOccur += (object sender, CommandEventArgs e) =>
					{
						LogManager.Instance.LogError(e.Message);
					};
				}

				anTask.AddChild(subTask);
            }

			return anTask;

		}
	}
}
