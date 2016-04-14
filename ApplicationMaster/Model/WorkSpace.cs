using System.ComponentModel;

namespace Casamia.Model
{
	public class WorkSpace : INotifyPropertyChanged
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

		public string Ext
		{
			get { return ext; }
			set { ext = value; }
		}

		public string Url
		{
			get { return url; }
			set
			{
				if (!string.Equals(value, url, System.StringComparison.OrdinalIgnoreCase))
				{
					url = value;
					OnPropertyChanged("Url");
				}
			}
		}

		public string LocalUrl
		{
			get { return localUrl; }
			set
			{
				if (!string.Equals(value, localUrl, System.StringComparison.OrdinalIgnoreCase))
				{
					localUrl = value;
					OnPropertyChanged("LocalUrl");
				}
			}
		}

		public string Name
		{
			get { return name; }
			set
			{
				if (!string.Equals(value, name, System.StringComparison.OrdinalIgnoreCase))
				{
					name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		public int UrlDepth
		{
			get { return urlDepth; }
			set
			{
				if (urlDepth != value)
				{
					urlDepth = value;
					OnPropertyChanged("UrlDepth");
				}
			}
		}

		public string Description
		{
			get { return description; }
			set
			{

				if (!string.Equals(value, description, System.StringComparison.OrdinalIgnoreCase))
				{
					description = value;

					OnPropertyChanged("Description");
				}
			}
		}

		#endregion PROPERTIES

		#region FUNCTION

		void OnPropertyChanged(string name)
		{
			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion FUNCTION

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

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
