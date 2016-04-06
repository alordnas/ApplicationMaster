using System.Collections.Generic;
using System.Collections.ObjectModel;

using Casamia.DataSource;
using Casamia.MyFacility;

namespace Casamia.Core
{
	public class TaskManager
	{
		private static Dictionary<int,TaskWorker> workMaps = new Dictionary<int,TaskWorker>();
		private static IList<AnTask> activeTaskItems = new List<AnTask>();
		public static IList<AnTask> TaskCollections
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

		public static void AddTask(AnTask anTask)
		{

		}

		public static void NormalizeTask(AnTask anTask, string path)
		{
			if (null != anTask && null != anTask.Commands)
			{
				string svnPath = WorkSpaceManager.Instance.Current.ToUrlPath(path);
				foreach (var command in anTask.Commands)
				{
					command.Argument = command.Argument.Replace(Util.PROJECT_PATH_PLACEHOLDER, path).Replace(Util.PROJECT_URL_PLACEHOLDER, svnPath);
				}
			}
		}

	}
}
