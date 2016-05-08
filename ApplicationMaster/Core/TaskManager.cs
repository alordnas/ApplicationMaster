using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using Newtonsoft.Json;

using Casamia.Model;
using Casamia.MyFacility;
using Casamia.Properties;

namespace Casamia.Core
{
    public class TaskManager
    {
        private static ObservableCollection<AnTask> protoTasks = null;
        private static ObservableCollection<AnTask> embededTask = null;
        private static Dictionary<AnTask, TaskWorker> workMaps = new Dictionary<AnTask, TaskWorker>();
        private static IList<AnTask> activeTaskItems = new List<AnTask>();

        #region PROPERTIES

        public static event System.Action<AnTask> ActivateTask;

        public static IList<AnTask> TaskCollections
        {
            get
            {
                return activeTaskItems;
            }
        }

        public static ObservableCollection<AnTask> ProtoTasks
        {
            get
            {
                if (null == protoTasks)
                {
                    try
                    {
                        AnTask[] tasks = JsonConvert.DeserializeObject<AnTask[]>(File.ReadAllText(Util.TASK_RECORD_FILE));
                        protoTasks = new ObservableCollection<AnTask>(tasks);
                    }
                    catch (System.Exception ex)
                    {
                        Logging.LogManager.Instance.LogError("Fail to initialize prototasks:{0}", ex.Message);
                    }
                    if (null == protoTasks)
                    {
                        protoTasks = new ObservableCollection<AnTask>();
                    }
                }
                return protoTasks;
            }
        }

        public static ObservableCollection<AnTask> EmbeddedTask
        {
            get
            {
                if (null == embededTask)
                {
                    AnTask[] tasks = JsonConvert.DeserializeObject<AnTask[]>(Settings.Default.EMBEDDED_TASKS);
                    embededTask = new ObservableCollection<AnTask>(tasks);
                }
                return embededTask;
            }
        }

        #endregion PROPERTIES

        #region PUBLIC

        public static void SaveProtoTasks()
        {
            AnTask[] tasks = new AnTask[ProtoTasks.Count];
            ProtoTasks.CopyTo(tasks, 0);
            string jsonString = JsonConvert.SerializeObject(tasks);
            File.WriteAllText(Util.TASK_RECORD_FILE, jsonString);
        }

        public static void RemoveTask(AnTask anTask)
        {
            TaskWorker o_worker = workMaps[anTask];
            o_worker.RemoveTask(anTask);
        }

        public static void ParallelTask(AnTask[] anTasks)
        {
            foreach (var anTask in anTasks)
            {
                TaskWorker worker = new TaskWorker(anTask.Name);
                worker.AddTask(anTask);
                worker.Run();
                AddActiveTask(anTask, worker);
            }
        }

        public static void AddActiveTask(AnTask anTask, TaskWorker worker)
        {
            if (!workMaps.ContainsKey(anTask))
            {
                workMaps.Add(anTask, worker);
                MyLog.Append(anTask);
            }
            else
            {
                workMaps[anTask] = worker;
            }
            if (!activeTaskItems.Contains(anTask))
            {
                activeTaskItems.Add(anTask);
                if (null != ActivateTask)
                {
                    ActivateTask(anTask);
                }
            }
        }

        public static void AddTask(AnTask anTask)
        {

        }

        public static AnTask GetEmbeddedTask(string name)
        {
            foreach (var anTask in EmbeddedTask)
            {
                if (string.Equals(anTask.Name, name))
                {
                    return anTask.Clone() as AnTask;
                }
            }
            return null;
        }

        public static void NormalizeTask(AnTask anTask, string path)
        {
            if (null != anTask && null != anTask.Commands)
            {
                string svnPath = WorkSpaceManager.Instance.Current.ToUrlPath(path);
                string localPath = WorkSpaceManager.Instance.Current.ToLocalPath(path);
                foreach (var command in anTask.Commands)
                {
                    if (!string.IsNullOrEmpty(command.Argument))
                    {
                        command.Argument = command.Argument.Replace(
                            Util.PROJECT_PATH_PLACEHOLDER,
                            localPath
                            ).Replace(
                            Util.PROJECT_URL_PLACEHOLDER,
                            svnPath
                            );
                    }
                }
            }
        }

        public static void AddProtoTask(AnTask task)
        {
            if (!ProtoTasks.Contains(task))
            {
                ProtoTasks.Add(task);
            }
        }

        public static bool RemoveProtoTask(AnTask task)
        {
            return ProtoTasks.Remove(task);
        }

        #endregion PUBLIC
    }
}
