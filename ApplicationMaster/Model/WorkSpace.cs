using System.ComponentModel;

namespace Casamia.Model
{
	public class WorkSpace
	{
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
			return path.Replace(LocalUrl.Replace('\\', '/'), Url);
		}

		public string ToLocalPath(string path)
		{
			return path.Replace(Url, LocalUrl);
		}

		#endregion PUBLIC
	}
}
