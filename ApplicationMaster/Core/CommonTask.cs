using System;
using System.Collections.Generic;
using System.IO;

using Casamia.DataSource;
using Casamia.Logging;
using Casamia.Model.EventArgs;

namespace Casamia.Core
{
	public class CommonTask
	{
		public static Dictionary<string, AnTask> CommonTasks = new Dictionary<string, AnTask>();

		public static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		public static void Initialize()
		{
			if (File.Exists(Util.TASK_RECORD_FILE))
			{
				ReadTaskFile(Util.TASK_RECORD_FILE);
			}
			else
			{
				Directory.CreateDirectory(Path.GetDirectoryName(Util.TASK_RECORD_FILE));
			}
		}

		private static void ReadTaskFile(string filePath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(Util.TASK_RECORD_FILE)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(Util.TASK_RECORD_FILE));
			}

			using (FileStream stream = File.OpenRead(filePath))
			{
				try
				{
					using (BinaryReader reader = new BinaryReader(stream))
					{
						int count = reader.ReadInt32();
						for (int i = 0; i < count; i++)
						{
							string taskName = reader.ReadString();

							AnTask anTask = new AnTask();

							int subTaskCount = reader.ReadInt32();

							Command[] subTasks = new Command[subTaskCount];

							for (int j = 0; j < subTaskCount; j++)
							{
								Command subTask = new Command();

								subTask.Executor = reader.ReadString();

								subTask.Argument = reader.ReadString();

								subTask.Timeout = TimeSpan.FromSeconds(reader.ReadInt32());

								subTasks[j] = subTask;
							}

							anTask.Description = reader.ReadString();

							anTask.AddChildren(subTasks);



							CommonTasks.Add(taskName, anTask);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.Instance.LogError("错误的文件内容：{0} ：{1} ", filePath, ex);
				}
			}
		}

		private static void WriteTaskFile(string filePath)
		{
			using (FileStream stream = File.OpenWrite(filePath))
			{
				try
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						writer.Write(CommonTasks.Count);

						foreach (var tasks in CommonTasks)
						{
							writer.Write(tasks.Key);

							var childTasks = tasks.Value.Commands;

							writer.Write(childTasks.Length);

							foreach (var task in childTasks)
							{
								writer.Write(task.Executor);

								writer.Write(task.Argument);

								writer.Write((int)task.Timeout.TotalSeconds);
							}
							writer.Write(tasks.Value.Description);
						}

						writer.Flush();
					}
				}
				catch (Exception ex)
				{
					LogManager.Instance.LogError("无法保存文件 [{0}]:{1}", filePath , ex);
				}
			}
		}

		public static void Flush()
		{
			WriteTaskFile(Util.TASK_RECORD_FILE);

			mainWindow.ResetTaskMenu();
		}


		public static List<AnTask> Allocation(AnTask anTask, string[] projectPaths)
		{
			List<AnTask> list = new List<AnTask>();

			for (int i = 0, projectCount = projectPaths.Length; i < projectCount; i++)
			{
				string projectPath = projectPaths[i];

				AnTask newTask = new AnTask();

				newTask.Description = anTask.Description;

				foreach (var subTask in anTask.Commands)
				{
					Command childTask = new Command();
					childTask.Executor = subTask.Executor;
					childTask.Argument = subTask.Argument.Replace(Util.PROJECT_PATH_PLACEHOLDER, projectPath);
					childTask.Timeout = subTask.Timeout;
					newTask.AddChild(childTask);
				}

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

				anTask.AddChild(subTask);

				RunTask(Constants.TASK_NAME_COMMAND, anTask, projectPaths);
			}
		}

		public static void RunTask(string taskName, string[] projectPaths)
		{
			AnTask anTask;

			if (CommonTask.CommonTasks.TryGetValue(taskName, out anTask))
			{
				LogManager.Instance.LogInfomation("执行任务<{0}>...", taskName);
				RunTask(taskName, anTask.Clone() as AnTask, projectPaths);
			}
		}

		public static void RunTask(string taskName, AnTask anTask, string[] projectPaths)
		{
			TaskWorker worker = new TaskWorker(taskName);

			worker.OnCompleteAll = () =>
			{
				LogManager.Instance.LogInfomation("<{0}>执行完毕", taskName);
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

		public static Command ParseCommand(string command)
		{
			Command task = new Command();
			command = command.Trim();
			int spaceIndex = command.IndexOf(" ");
			if (spaceIndex < 0)
			{
				command = command.ToLower();
				string exe = XMLManage.GetString(command);
				if (exe != null && exe.EndsWith(".exe"))
				{
					task.Exe = exe;
					task.Argument = null;
					LogManager.Instance.LogWarning("没有参数的命令：{0}", command);
					return task;
				}
				else
				{
					LogManager.Instance.LogError("不能识别的命令：{0}", command);
					return null;
				}
			}
			else
			{
				string executor = command.Substring(0, spaceIndex);
				string exe = XMLManage.GetString(executor.ToLower());
				if (exe != null && exe.EndsWith(".exe"))
				{
					string argment = command.Substring(spaceIndex, command.Length - spaceIndex);
					argment = argment.Trim();
					spaceIndex = argment.LastIndexOf(" ");

					if (0 < spaceIndex)
					{
						string timeout = argment.Substring(spaceIndex, argment.Length - spaceIndex);
						int _timeout;
						if (int.TryParse(timeout.Trim(), out _timeout))
						{
							string _argment = argment.Substring(0, spaceIndex);
							task.Executor = executor;
							task.Argument = _argment;
							task.Timeout = TimeSpan.FromSeconds(_timeout);
						}
						else
						{
							task.Executor = executor;
							task.Argument = argment;
						}
					}
					else
					{
						int _timeout;
						if (int.TryParse(argment.Trim(), out _timeout))
						{
							task.Executor = executor;
							task.Argument = null;
							LogManager.Instance.LogWarning("没有参数的命令：{0}", command);
							task.Timeout = TimeSpan.FromSeconds(_timeout);
						}
						else
						{
							task.Executor = executor;
							task.Argument = argment;
						}
					}
					return task;
				}
				else
				{
					LogManager.Instance.LogError("不能识别的命令：{0}", command);
					return null;
				}
			}
		}

		public static void SvnCheckDiff(TaskWorker svnWorker, string projectPath, Action DoThisIfIsWorkingCopy, Action DoThisIfIsNoAWorkingCopy)
		{
			AnTask anTask = new AnTask();
			Command subTask = new Command();
			subTask.Executor = Util.SVN;
			subTask.Argument = string.Format("diff {0}", projectPath);
			subTask.StatusChanged +=
				(object sender, CommandStatusEventArgs e) =>
				{
					if (e.NewStatus == CommandStatus.Completed && DoThisIfIsWorkingCopy != null)
					{
						DoThisIfIsWorkingCopy();
					}
				};
			subTask.ErrorOccur += (object sender, CommandEventArgs e) =>
			{
				if (DoThisIfIsNoAWorkingCopy != null)
				{
					DoThisIfIsNoAWorkingCopy();
				}
			};

			anTask.AddChild(subTask);

			svnWorker.AddTask(anTask);
		}
	}
}
