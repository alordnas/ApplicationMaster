using Casamia.Core;
using Casamia.DataSource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casamia.Logging;

namespace Casamia.MyFacility
{
    public class MyLog
    {
		static string[] logFilePaths;


		static string todayLogFilePath;

		static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		public static List<AnTask> LogTasks
		{
			get 
			{
				if (mainWindow.log_ComboBox.SelectedIndex == 0)
					return todayLogTasks;
				else
					return noTodayLogTasks;
			}
		}

		private static List<AnTask> todayLogTasks = new List<AnTask>();

		private static List<AnTask> noTodayLogTasks = new List<AnTask>();


		public static void Initialize() 
        {

            todayLogFilePath = GetTodayLogPath();

            if (!File.Exists(todayLogFilePath))
            {
                if (!Directory.Exists(Util.Log_Folder))
                {
                    Directory.CreateDirectory(Util.Log_Folder);
                }

				WriteLog(todayLogFilePath);
            }

			todayLogTasks = ReadLog(todayLogFilePath);

            logFilePaths = Directory.GetFiles(Util.Log_Folder, "*.txt", SearchOption.AllDirectories);

			List<string> logfileNames = new List<string>();

            for (int i = logFilePaths.Length - 1; i >= 0; i--)
            {
				 logfileNames.Add(Path.GetFileNameWithoutExtension(logFilePaths[i]));
            }

			mainWindow.log_ComboBox.ItemsSource = logfileNames;

			mainWindow.log_ComboBox.SelectedIndex = 0;

        }


		private static void WriteLog(string filePath) 
		{
			using (FileStream stream = File.OpenWrite(filePath))
			{
				try
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{

						writer.Write(todayLogTasks.Count);

						for (int i = 0, length = todayLogTasks.Count; i < length; i++)
						{
							AnTask anTask = todayLogTasks[i];

							writer.Write(anTask.Name);

							writer.Write(anTask.Description);

							writer.Write(anTask.Commands.Length);


							for (int j = 0, count = anTask.Commands.Length; j < count; j++)
							{
								Command subTask = anTask.Commands[j];

								writer.Write(subTask.Executor == null ? string.Empty : subTask.Executor);

								writer.Write(subTask.Argument == null ? string.Empty : subTask.Argument);

								writer.Write(subTask.Error == null ? string.Empty : subTask.Error);

								writer.Write(subTask.Output == null ? string.Empty : subTask.Output);

								writer.Write(subTask.StartTime.ToString());

								writer.Write((int)(subTask.Status));
								// used to be system timeout.
								writer.Write(0);

								writer.Write((long)subTask.TimeCost.TotalMilliseconds);

								writer.Write((int)subTask.Timeout.TotalSeconds);
							}
						}
						writer.Flush();
					}
				}
				catch (Exception)
				{
					LogManager.Instance.LogError("无法保存文件：{0}", filePath);
				}
			}
		}

		private static List<AnTask> ReadLog(string filePath) 
		{
			List<AnTask> _logTasks = new List<AnTask>();

			using (FileStream stream = File.OpenRead(filePath))
			{
				try
				{
					using (BinaryReader reader = new BinaryReader(stream))
					{
						int anTaskCount = reader.ReadInt32();

						for (int i = 0; i < anTaskCount; i++)
						{
							AnTask anTask = new AnTask();
							anTask.Name = reader.ReadString();

							anTask.Description = reader.ReadString();

							int subTaskCount = reader.ReadInt32();

							for (int j = 0; j < subTaskCount; j++)
							{
								Command subTask = new Command();

								subTask.Executor = reader.ReadString();

								subTask.Argument = reader.ReadString();

								subTask.Error = reader.ReadString();

								subTask.Output = reader.ReadString();

								// starttime
								reader.ReadString();

								subTask.Status = (CommandStatus)reader.ReadInt32();

								// ignore system timeout.
								reader.ReadInt32();
								reader.ReadInt64();

								subTask.Timeout = TimeSpan.FromSeconds(reader.ReadInt32());

								anTask.AddCommand(subTask);
							}

							_logTasks.Add(anTask);
						}

					}
				}
				catch (Exception)
				{
					LogManager.Instance.LogError("错误的文件内容：{0}", filePath);
				}
			}

			return _logTasks;
		}


		private static string GetTodayLogPath() 
        {
            return string.Format("{0}/{1}-{2}-{3}.txt", 
                Util.Log_Folder, 
                DateTime.Today.Year,
                DateTime.Today.Month < 10 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString(),
                DateTime.Today.Day < 10 ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString());
        }


		public static void ChangeLogText(string fileName) 
        {
            string logFilePath = string.Format("{0}/{1}.txt", Util.Log_Folder, fileName);

            if (logFilePath != todayLogFilePath)
            {
				WriteLog(todayLogFilePath);

                if (File.Exists(logFilePath))
                {
					noTodayLogTasks = ReadLog(logFilePath);
					
                }
            }
			else
			{
				if (!File.Exists(todayLogFilePath))

					WriteLog(todayLogFilePath);

				todayLogTasks = ReadLog(todayLogFilePath);
			}
        }

		public static void Save() 
        {
			WriteLog(todayLogFilePath);
        }

		public static void Append(AnTask anTask)
        {
			todayLogTasks.Add(anTask);
        }
    }
}
