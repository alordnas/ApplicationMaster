using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casamia.Model;

namespace Casamia.ViewModel
{
	public class WorkSpaceViewModel : ViewModelBase
	{
		#region Variable

		private WorkSpace _workSpace;
		private AnTaskViewModel createTaskViewModel;
		private AnTaskViewModel importTaskViewModel;

		#endregion // Variable

		#region Constructor

		public WorkSpaceViewModel(WorkSpace workSpace)
		{
			_workSpace = workSpace;
			createTaskViewModel = new AnTaskViewModel(_workSpace.CreateProjectTask);
			importTaskViewModel = new AnTaskViewModel(_workSpace.ImportProjectTask);
		}

		#endregion // Constructor

		#region Custom Properties

		public WorkSpace WorkSpace
		{
			get
			{
				return _workSpace;
			}
		}

		public string Name
		{
			get
			{
				return _workSpace.Name;
			}
			set
			{
				if (!string.Equals(_workSpace.Name, value))
				{
					_workSpace.Name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		public string Description
		{
			get
			{
				return _workSpace.Description;
			}
			set
			{
				if (!string.Equals(_workSpace.Description, value))
				{
					_workSpace.Description = value;
					base.OnPropertyChanged("Description");
				}
			}
		}

		public string Url
		{
			get
			{
				return _workSpace.Url;
			}
			set
			{
				if (!string.Equals(value, _workSpace.Url, System.StringComparison.OrdinalIgnoreCase))
				{
					_workSpace.Url = value;
					OnPropertyChanged("Url");
				}
			}
		}

		public string LocalUrl
		{
			get { return _workSpace.LocalUrl; }
			set
			{
				if (!string.Equals(value, _workSpace.LocalUrl, System.StringComparison.OrdinalIgnoreCase))
				{
					_workSpace.LocalUrl = value;
					OnPropertyChanged("LocalUrl");
				}
			}
		}

		public int UrlDepth
		{
			get { return _workSpace.UrlDepth; }
			set
			{
				if (_workSpace.UrlDepth != value)
				{
					_workSpace.UrlDepth = value;
					OnPropertyChanged("UrlDepth");
				}
			}
		}

		public AnTaskViewModel CreateProjectTask
		{
			get
			{
				return createTaskViewModel;
			}
			set
			{
				if (null != value && null != value.Task)
				{
					_workSpace.CreateProjectTask = value.Task;
					OnPropertyChanged("CreateProjectTask");
				}
			}
		}

		public AnTaskViewModel ImportProjectTask
		
		{
			get
			{
				return importTaskViewModel;
			}
			set
			{
				if (null != value && null != value.Task)
				{
					_workSpace.ImportProjectTask = value.Task;
					OnPropertyChanged("ImportProjectTask");
				}
			}
		}

		#endregion // Custom Properties

	}
}
