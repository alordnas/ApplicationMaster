using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using Casamia.Model;
using System;

namespace Casamia.Core
{
	public class WorkSpaceManager
	{
		#region VARIABLE

		private static WorkSpaceManager instance;

		private List<WorkSpace> workSpaces;
		private WorkSpace current;
		private string configPath;
		private bool isLocal;

		#endregion VARIABLE

		#region PROPERTIES

		public static WorkSpaceManager Instance
		{
			get
			{
				if(null == instance)
				{
					instance = new WorkSpaceManager();
				}
				return instance;
			}
		}

		public string WorkingPath
		{
			get
			{
				return isLocal ? Current.LocalUrl : Current.Url;
			}
		}

		public WorkSpace Current
		{
			get
			{
				if (current == null && workSpaces != null && workSpaces.Count > 0)
				{
					current = workSpaces[0];
				}
				return current;
			}
			set { current = value; }
		}

		public WorkSpace[] WorkSpaces
		{
			get
			{
				return workSpaces.ToArray();
			}
		}

		#endregion PROPERTIES

		#region CONSTRUCTOR

		private WorkSpaceManager()
		{
			configPath = Casamia.Properties.Settings.Default.WORKSPACE_CONFIG_PATH;
			Init();
		}

		#endregion CONSTRUCTOR

		#region PUBLIC

		public void Save()
		{
			string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(workSpaces);
			File.WriteAllText(configPath, jsonStr);
		}

		public void Add(WorkSpace workSpace)
		{
			if(null != workSpace && !workSpaces.Contains(workSpace))
			{
				workSpaces.Add(workSpace);
			}
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
						workSpaces = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkSpace>>(jsonStr);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.LogManager.Instance.LogError("Fail to deserialize {0} :{1}", configPath, ex.Message);
			}

			if(null ==workSpaces)
			{
				workSpaces = new List<WorkSpace>();
			}
		}

		#endregion FUNCTION

	}
}
