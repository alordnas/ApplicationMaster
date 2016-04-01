using Casamia.DataSource;
using Casamia.MyFacility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Casamia.Core
{
	public class TaskHandler
	{
		private static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		private static List<AnTask> ActiveTasks = new List<AnTask>();

		private static Dictionary<int,TaskWorker> workMaps = new Dictionary<int,TaskWorker>();

		private static ObservableCollection<AnTask> activeTaskItems = new ObservableCollection<AnTask>();

		public static ObservableCollection<AnTask> TaskCollections
		{
			get
			{
				return activeTaskItems;
			}
		}


		public static ObservableCollection<SubTaskItem> GetSubTaskItem(AnTask currentTaskLogItem) 
		{
			ObservableCollection<SubTaskItem> subTaskItems = new ObservableCollection<SubTaskItem>();
			if (currentTaskLogItem != null)
			{
				AnTask anTask = currentTaskLogItem;

				int index = 0;

				foreach (var subTask in anTask.Commands)
				{
					index++;

					SubTaskItem item = new SubTaskItem();

					item.ID = index;

					item.Exe = subTask.Exe;

					item.Argument = subTask.Argument;

					item.TimeCost = subTask.Timecost;

					if (subTask.Timeout < 0)
					{
						item.Timeout = "等待返回";					
					}
					else
					{
						int _systemTimeout = MyFacility.MyWorker.timeout < 0 ? int.MaxValue : MyFacility.MyWorker.timeout;
						item.Timeout = Math.Min(subTask.Timeout, _systemTimeout).ToString();
					}

					ParseStatus(item, subTask.Status);

					//item.StartTime = subTask.StartTime;

					item.Output = (!string.IsNullOrEmpty(subTask.Output)) ? "查看" : string.Empty;

					item.outputContent = subTask.Output;

					item.Error = (!string.IsNullOrEmpty(subTask.Error)) ? "查看" : string.Empty;

					item.errorContent = subTask.Error;

					subTaskItems.Add(item);
				}
			}
			return subTaskItems;
		}

		public static void StopSelectedTasks(IList<DataGridCellInfo> cells)
		{
			for (int i = 0,length = cells.Count; i < length; i = i + 7)
			{
				DataGridCellInfo cell = cells[i];

				TaskItem item = cell.Item as TaskItem;

				AnTask anTask = ActiveTasks[item.ID - 1];

				if (anTask != null)
				{
					List<Command> subTasks = new List<Command>(anTask.Commands);

					if (subTasks != null)
					{
						foreach (var subTask in subTasks)
						{
							if (CommandStatus.Waiting == CommandStatus.Waiting)
							{
								subTask.Status = CommandStatus.Cancel;
							}
						}
					}
				}
			}
		}

		public static void RemoveTask(AnTask anTask)
		{
			TaskWorker o_worker = workMaps[anTask.ID];
			o_worker.RemoveTask(anTask);
		}
		
		public static void ParallelTask(AnTask[] anTasks)
		{
			foreach (var anTask in anTasks)
			{
				ActiveTasks.Remove(anTask);
				TaskWorker worker = new TaskWorker(anTask.Name);
				worker.AddTask(anTask);
				worker.Run();
			}
		}


		private static void ResetTask(AnTask anTask)
		{
			foreach (var command in anTask.Commands)
			{
				command.Reset();
			}
		}

		private static TaskItem GetTaskItem(AnTask activeTask)
		{
			TaskItem item = new TaskItem();
			item.ID = activeTask.ID;
			item.TaskName = activeTask.Name;
			LookupStatus(item, activeTask);
			item.Description = activeTask.Description;
			item.SubTaskItems = GetSubTaskItem(activeTask);
			return item;
		}


		private static void ParseStatus(SubTaskItem item, CommandStatus status) 
		{
			string display = string.Empty;

			switch (status)
			{
				case CommandStatus.Waiting:
					item.Status = "等待";
					item.Color = Brushes.Black;
					break;
				case CommandStatus.Running:
					item.Status = "正在运行";
					item.Color = Brushes.Blue;
					break;
				case CommandStatus.Error:
					item.Status = "错误";
					item.Color = Brushes.Red;
					break;
				case CommandStatus.Timeout:
					item.Status = "超时";
					item.Color = Brushes.Orange;
					break;
				case CommandStatus.Completed:
					item.Status = "完成";
					item.Color = Brushes.Green;
					break;
				case CommandStatus.Cancel:
					item.Status = "已取消";
					item.Color = Brushes.Gray;
					break;
				default:
					break;
			}
		}

		private static void LookupStatus(TaskItem item, AnTask anTask)
		{
			string display = string.Empty;

			List<Command> subTasks = new List<Command>(anTask.Commands);

			for (int i = 0,length = subTasks.Count; i < length; i++)
			{
				Command subTask = subTasks[i];

				if (i == 0)
				{
					if (subTask.Status == CommandStatus.Waiting)
					{
						display = "等待";

						item.Color = Brushes.Black;

						break;
					}

					//item.StartTime = subTask.StartTime;
				}

				if (subTask.Status == CommandStatus.Error)
				{
					display =  "错误";

					item.Color = Brushes.Red;

					break;
				}

				if (subTask.Status == CommandStatus.Cancel)
				{
					display =  "已取消";

					item.Color = Brushes.Gray;

					break;
				}
			
				if (subTask.Status == CommandStatus.Running)
				{
					display = "正在运行...";

					item.Color = Brushes.Blue;
				}

				if (subTask.Status == CommandStatus.Timeout && display == string.Empty)
				{
					display = "超时";

					item.Color = Brushes.Orange;
				}

				if (i == length - 1 && subTask.Status == CommandStatus.Completed)
				{
					display = "完成";

					item.Color = Brushes.Green;
				}
			}

			int completedCount = 0;

			long totalCost = 0;

			for (int i = 0, length = subTasks.Count; i < length; i++)
			{
				Command subTask = subTasks[i];

				if (subTask.Status == CommandStatus.Completed) 
				{
					completedCount++;
				}

				totalCost += subTask.Timecost;
			}

			item.Status = display;
			item.Schedule = string.Format("{0}/{1}", completedCount, subTasks.Count);
			item.TimeCost = totalCost;
		}

		public static void AddActiveTask(AnTask anTask,TaskWorker worker)
		{
			if (anTask.ID == 0)
			{
				anTask.ID = ActiveTasks.Count + 1;
				ActiveTasks.Add(anTask);
				workMaps.Add(anTask.ID, worker);
				
				MyLog.Append(anTask);

			}
			else
			{
				ActiveTasks.Insert(anTask.ID - 1, anTask);
				
				workMaps[anTask.ID] = worker;
			}
	
			// add to datasource.
			activeTaskItems.Add(anTask);
		}

	}

	public class TaskItem 
	{
		public int ID { get; set; }

		public string TaskName { get; set; }

		public string Status { get; set; }

		public string Description { get; set; }

		public string StartTime { get; set; }

		public long TimeCost { get; set; }
		public string Schedule { get; set; }
		public SolidColorBrush Color { get; set; }
		public ObservableCollection<SubTaskItem> SubTaskItems = new ObservableCollection<SubTaskItem>();

	}

	public class SubTaskItem
	{
		public int ID { get; set; }

		public string Exe { get; set; }

		public string Argument { get; set; }

		public string StartTime { get; set; }

		public string Timeout { get; set; }

		public string Status { get; set; }

		public long TimeCost { get; set; }

		public string Output { get; set; }

		public string Error { get; set; }

		public SolidColorBrush Color { get; set; }


		public Visibility OutputButtonVisible {
			get 
			{
				return !string.IsNullOrEmpty(Output) ? Visibility.Visible : Visibility.Hidden;
			}
			set 
			{

			}
		}
		public Visibility ErrorButtonVisible
		{
			get 
			{
				return !string.IsNullOrEmpty(Error) ? Visibility.Visible : Visibility.Hidden;
			}
			set 
			{

			}
		}


		public string outputContent = string.Empty;

		public string errorContent = string.Empty;
		
	}

}
