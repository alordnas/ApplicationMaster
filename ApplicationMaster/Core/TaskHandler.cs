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
		private static Dictionary<int,TaskWorker> workMaps = new Dictionary<int,TaskWorker>();
		private static ObservableCollection<AnTask> activeTaskItems = new ObservableCollection<AnTask>();
		public static ObservableCollection<AnTask> TaskCollections
		{
			get
			{
				return activeTaskItems;
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
				activeTaskItems.Add(anTask);
				TaskWorker worker = new TaskWorker(anTask.Name);
				worker.AddTask(anTask);
				worker.Run();
			}
		}

		public static void AddActiveTask(AnTask anTask,TaskWorker worker)
		{
			if (anTask.ID == 0)
			{
				anTask.ID = activeTaskItems.Count + 1;
				workMaps.Add(anTask.ID, worker);
				MyLog.Append(anTask);
			}
			else
			{
				workMaps[anTask.ID] = worker;
			}
			activeTaskItems.Add(anTask);
		}
	}
}
