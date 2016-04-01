﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using Casamia.DataSource;
using Casamia.Logging;

namespace Casamia.Core
{
	public class TaskWorker : BackgroundWorker
	{
		Queue<AnTask> groupTasks = new Queue<AnTask>();

		List<AnTask> disabled = new List<AnTask>();

		public Action OnCompleteAll;

		public bool anyTask
		{
			get
			{
				return 0 < groupTasks.Count;
			}
		}

		private bool isActive
		{
			get
			{
				return !string.IsNullOrEmpty(_taskName);
			}
		}

		private string _taskName;

		public TaskWorker(string taskName)
		{
			_taskName = taskName;

			WorkerReportsProgress = true;

			WorkerSupportsCancellation = true;

			DoWork += TaskWorker_DoWork;

			RunWorkerCompleted += TaskWorker_RunWorkerCompleted;

			ProgressChanged += TaskWorker_ProgressChanged;
		}

		public void AddTasks(IEnumerable<AnTask> anTasks)
		{
			foreach (var anTask in anTasks)
			{
				AddTask(anTask);
			}
		}

		public void AddTask(AnTask anTask)
		{
			if (anTask != null)
			{
				groupTasks.Enqueue(anTask);

				if (isActive)
				{
					anTask.Name = _taskName;
					TaskHandler.AddActiveTask(anTask, this);
				}
			}
		}

		public void RemoveTask(AnTask anTask)
		{
			disabled.Add(anTask);
		}


		public void Run()
		{

			RunWorkerAsync();
		}


		private void TaskWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			LoopTasks();
		}

		private void TaskWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (OnCompleteAll != null)
			{
				OnCompleteAll();
			}

		}


		void TaskWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Debug.Print("{0} {1} {2}", sender, e.ProgressPercentage, e.UserState);

			if (e.UserState is Command)
			{
				Command subTask = e.UserState as Command;

				if (e.ProgressPercentage == -1)
				{
					subTask.Status = CommandStatus.Running;
				}
				else if (string.IsNullOrEmpty(subTask.Error))
				{
					if (e.ProgressPercentage == 0)
					{
						subTask.Status = CommandStatus.Timeout;
					}
					else if (e.ProgressPercentage == 1)
					{
						subTask.Status = CommandStatus.Completed;
					}
					else if (e.ProgressPercentage == 2)
					{
						subTask.Status = CommandStatus.Cancel;
					}
				}
				else
				{
					subTask.Status = CommandStatus.Error;

				}
			}
		}


		private void LoopTasks()
		{
			while (0 < groupTasks.Count)
			{
				AnTask anTask = groupTasks.Dequeue();

				if (disabled.Contains(anTask))
					continue;

				for (int i = 0, length = anTask.Commands.Length; i < length; i++)
				{
					Command command = anTask.Commands[i];
					if (command.Status == CommandStatus.Waiting)
					{
						command.StartTime = DateTime.Now;

						if (anTask.StartTime == DateTime.MinValue)
						{
							anTask.StartTime = DateTime.Now;
						}
						command.StatusChanged +=
							(object sender, Model.EventArgs.CommandStatusEventArgs e) =>
							{
								LogManager.Instance.LogInfomation(
									"{0} #{1} changed .{2}->{3}", 
									anTask.Name, 
									i,
									e.OldStatus,
									e.NewStatus
									);
							};
						command.CommandFeedbackReceived+=
							(object sender, Model.EventArgs.CommandEventArgs e) =>
							{
								LogManager.Instance.LogDebug(
									"{0} #{1}[{3}] : {2}",
									anTask.Name,
									i,
									e.Message,
									e.Status
									);
							};
						command.ErrorOccur +=
							(object sender, Model.EventArgs.CommandEventArgs e) =>
							{
								LogManager.Instance.LogError(
									"{0} #{1}[{3}] : {2}",
									anTask.Name,
									i,
									e.Message,
									e.Status
									);
							};

						ReportProgress(-1, command);

						bool isTimeout = RunProcess(command);

						ReportProgress(isTimeout ? 0 : 1, command);

						if (!string.IsNullOrEmpty(command.Error))
							break;
					}
					else
					{
						ReportProgress(2, command);
					}
				}
				System.Threading.Thread.Sleep(500);
			}
		}

		private bool RunProcess(Command subTask)
		{
			StringBuilder outputDataBuilder = new StringBuilder();
			Process process = new Process();
			process.StartInfo.FileName = subTask.Exe;
			process.StartInfo.Arguments = subTask.Argument;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.OutputDataReceived +=
				(object sender, DataReceivedEventArgs e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						subTask.Output = e.Data;
					}
				};
			process.ErrorDataReceived +=
				(object sender, DataReceivedEventArgs e) =>
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						subTask.Error = e.Data;
					}
				};
			Stopwatch stopWatch = new Stopwatch();

			stopWatch.Start();

			process.Start();
			Logging.LogManager.Instance.LogDebug("{0} {1}", subTask.Executor, subTask.Argument);
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();

			bool isTimeout = false;
			int actualTimeout = int.MaxValue;

			//强制等待结束
			if (subTask.Timeout < 0)
			{
				process.WaitForExit();
			}
			else
			{
				int _systemTimeout = MyFacility.MyWorker.timeout < 0 ? int.MaxValue : MyFacility.MyWorker.timeout;
				actualTimeout = Math.Min(subTask.Timeout, _systemTimeout);
				isTimeout = !process.WaitForExit(actualTimeout);
				if (!isTimeout)
				{
					subTask.Timecost = stopWatch.ElapsedMilliseconds;
				}
			}

			stopWatch.Stop();

			subTask.Timecost = Math.Min(stopWatch.ElapsedMilliseconds, actualTimeout);

			return isTimeout;
		}
	}
}