using System;
using System.Collections.Generic;
using System.IO;

using Casamia.Model;
using Casamia.Logging;
using Casamia.Model.EventArgs;

namespace Casamia.Core
{
	public class CommonTask
	{
		public static Dictionary<string, AnTask> CommonTasks = new Dictionary<string, AnTask>();

		public static List<AnTask> Allocation(AnTask anTask, string[] projectPaths)
		{
			List<AnTask> list = new List<AnTask>();

			for (int i = 0, projectCount = projectPaths.Length; i < projectCount; i++)
			{
				string projectPath = projectPaths[i];
				AnTask newTask = anTask.Clone() as AnTask;
				TaskManager.NormalizeTask(newTask, projectPath);
				list.Add(newTask);
			}

			return list;
		}


		public static void RunCommand(string command, string[] projectPaths)
		{
			Command subTask = CommonTask.ParseCommand(command);

			if (subTask != null)
			{
				AnTask anTask = new AnTask();

				anTask.AddCommand(subTask);

				RunTask(anTask, projectPaths);
			}
		}

		public static void RunTask(AnTask anTask, string[] projectPaths)
		{
			TaskWorker worker = new TaskWorker(anTask.Name);

			worker.OnCompleteAll = () =>
			{
				LogManager.Instance.LogInfomation("<{0}>执行完毕", anTask.Name);
			};

			if (ContainPlaceHolder(anTask))
			{
				if (projectPaths.Length == 0)
				{
					LogManager.Instance.LogError("该任务基于选中的Unity项目，请在列表框选择至少一个项目。");
					return;
				}
				else
				{
					List<AnTask> taskList = Allocation(anTask, projectPaths);
					worker.AddTasks(taskList.ToArray());
				}
			}
			else
			{
				worker.AddTask(anTask);
			}
			worker.Run();
		}

		private static bool ContainPlaceHolder(AnTask anTask)
		{
			foreach (var subTask in anTask.Commands)
			{
				if (subTask.Argument != null && subTask.Argument.Contains(Util.PROJECT_PATH_PLACEHOLDER))
				{
					return true;
				}
			}

			return false;
		}

		public static Command ParseCommand(string commandString)
		{
			Command command = new Command();
			commandString = commandString.Trim();
			int spaceIndex = commandString.IndexOf(" ");
			if (spaceIndex < 0)
			{
				commandString = commandString.ToLower();
				string exe = XMLManage.GetString(commandString);
				if (exe != null && exe.EndsWith(".exe"))
				{
					command.Argument = null;
					LogManager.Instance.LogWarning("没有参数的命令：{0}", commandString);
					return command;
				}
				else
				{
					LogManager.Instance.LogError("不能识别的命令：{0}", commandString);
					return null;
				}
			}
			else
			{
				string executor = commandString.Substring(0, spaceIndex);
				string exe = XMLManage.GetString(executor.ToLower());
				if (exe != null && exe.EndsWith(".exe"))
				{
					string argment = commandString.Substring(spaceIndex, commandString.Length - spaceIndex);
					argment = argment.Trim();
					spaceIndex = argment.LastIndexOf(" ");

					if (0 < spaceIndex)
					{
						string timeout = argment.Substring(spaceIndex, argment.Length - spaceIndex);
						int _timeout;
						if (int.TryParse(timeout.Trim(), out _timeout))
						{
							string _argment = argment.Substring(0, spaceIndex);
							command.Executor = executor;
							command.Argument = _argment;
							command.Timeout = TimeSpan.FromSeconds(_timeout);
						}
						else
						{
							command.Executor = executor;
							command.Argument = argment;
						}
					}
					else
					{
						int _timeout;
						if (int.TryParse(argment.Trim(), out _timeout))
						{
							command.Executor = executor;
							command.Argument = null;
							LogManager.Instance.LogWarning("没有参数的命令：{0}", commandString);
							command.Timeout = TimeSpan.FromSeconds(_timeout);
						}
						else
						{
							command.Executor = executor;
							command.Argument = argment;
						}
					}
					return command;
				}
				else
				{
					LogManager.Instance.LogError("不能识别的命令：{0}", commandString);
					return null;
				}
			}
		}

		public static void SvnCheckDiff(TaskWorker svnWorker, string projectPath, Action DoThisIfIsWorkingCopy, Action DoThisIfIsNoAWorkingCopy)
		{
			AnTask anTask = TaskManager.GetEmbeddedTask("SVN_DIFF_TASK");

			TaskManager.NormalizeTask(anTask, projectPath);

			if (anTask.Commands != null && anTask.Commands.Length > 0)
			{
				Command command = anTask.Commands[0];
				command.StatusChanged +=
					(object sender, CommandStatusEventArgs e) =>
					{
						if (e.NewStatus == CommandStatus.Completed && DoThisIfIsWorkingCopy != null)
						{
							DoThisIfIsWorkingCopy();
						}
					};
				command.ErrorOccur += (object sender, CommandEventArgs e) =>
				{
					if (DoThisIfIsNoAWorkingCopy != null)
					{
						DoThisIfIsNoAWorkingCopy();
					}
				};
			}

			svnWorker.AddTask(anTask);
		}
	}
}
