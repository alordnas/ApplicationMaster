using System.ComponentModel;

namespace Casamia.Model
{
	public class WorkSpace 
	{
		#region VARIABLE

		private string url;
		private string localUrl;
		private string name;
		private int urlDepth;
		private string ext;
		private string description;
		
		#endregion VARIABLE

		#region PROPERTIES

		public AnTask CreateProjectTask
		{
			get;
			set;
		}

		public AnTask ImportProjectTask
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public string LocalUrl
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int UrlDepth
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		#endregion PROPERTIES

		#region PUBLIC

		public string ToUrlPath(string path)
		{
			path = path.Replace('\\', '/');
			return path.Replace(localUrl.Replace('\\', '/'), url);
		}

		public string ToLocalPath(string path)
		{
			return path.Replace(url, localUrl);
		}

		#endregion PUBLIC
	}
}
