using System;
using System.Collections.ObjectModel;
using System.IO;

using Newtonsoft.Json;

using Casamia.Logging;
using Casamia.Model;
using System.Text;

namespace Casamia.Core
{
	public class ExecutorManager
	{
		
		#region VARIABLE

		private static ExecutorManager instance;

		private ObservableCollection<Executor> executors;
		private string configPath;


		#endregion VARIABLE

		#region PROPERTIES

		public static ExecutorManager Instance
		{
			get
			{
				if (null == instance)
				{
					instance = new ExecutorManager();
				}
				return instance;
			}
		}

		public ObservableCollection<Executor> Executors
		{
			get
			{
				return executors;
			}
			set
			{
				executors = value;
			}
		}

		#endregion PROPERTIES

		#region CONSTRUCTOR

		private ExecutorManager()
		{
			configPath = Casamia.Properties.Settings.Default.EXECUTOR_CONFIG_PATH;
			Init();
		}

		#endregion CONSTRUCTOR

		#region PUBLIC

		public void Save()
		{
			string jsonStr = JsonConvert.SerializeObject(executors);
			File.WriteAllText(configPath, jsonStr);
		}

		public void Add(Executor executor)
		{
			if (null != executor && !executors.Contains(executor))
			{
				executors.Add(executor);
			}
		}

		public Executor GetByPlaceHolder(string placeHolder)
		{
			foreach (var item in executors)
			{
				if(string.Equals(item.PlaceHolder , placeHolder))
				{
					return item;
				}
			}
			return null;
		}

		#endregion PUBLIC

		#region FUNCTION

		void Init()
		{
			try
			{
				if (File.Exists(configPath))
				{
					string jsonStr = File.ReadAllText(configPath);
					if (!string.IsNullOrEmpty(jsonStr))
					{
						executors = JsonConvert.DeserializeObject<ObservableCollection<Executor>>(jsonStr);
						StringBuilder sb = new StringBuilder();
						foreach (var executor in executors)
						{
							if (!File.Exists(executor.Path))
							{
								sb.Append(executor.Name);
								sb.Append(',');
							}
						}
						if(sb.Length>0)
						{
							LogManager.Instance.LogError(
								"Following executor cannot find corresponding application .",
								sb.ToString()
								);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.Instance.LogError("Fail to deserialize {0} :{1}", configPath, ex.Message);
			}

			if (null == executors)
			{
				executors = new ObservableCollection<Executor>();
			}
		}

		#endregion FUNCTION


	}
}
