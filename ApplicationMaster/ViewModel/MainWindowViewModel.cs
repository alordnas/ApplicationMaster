using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Casamia.Core;
using Casamia.Model;

namespace Casamia.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Variable

		private ObservableCollection<AnTaskViewModel> activeTasks;
		private ObservableCollection<AnTaskViewModel> protoTasks;
		private ObservableCollection<WorkSpaceViewModel> workSpaces;
		private AnTaskViewModel selectedActiveTask;

		#endregion // Variable

		#region Constructor

		public MainWindowViewModel()
		{
			TaskManager.ActivateTask += TaskManager_ActivateTask;
		}

		#endregion // Constructor

		#region Event Handler

		void TaskManager_ActivateTask(AnTask obj)
		{
			ActiveTasks.Add(new AnTaskViewModel(obj));
		}

		#endregion // Event Handler

		#region Custom Properties

		public AnTaskViewModel SelectedActiveTask
		{
			get
			{
				if(null == selectedActiveTask && ActiveTasks.Count>0)
				{
					selectedActiveTask = ActiveTasks[0];
				}
				return selectedActiveTask;
			}
		}

		public ObservableCollection<WorkSpaceViewModel> WorkSpaces
		{
			get
			{
				if(null == workSpaces)
				{
					workSpaces = new ObservableCollection<WorkSpaceViewModel>();
				}
				return workSpaces;
			}
		}

		public ObservableCollection<AnTaskViewModel> ProtoTasks
		{
			get
			{
				if (null == protoTasks)
				{
					protoTasks = new ObservableCollection<AnTaskViewModel>();
					AnTask[] tasks = TaskManager.ProtoTasks;
					foreach (var anTask in tasks)
					{
						protoTasks.Add(new AnTaskViewModel(anTask));
					}
				}
				return protoTasks;
			}
		}

		public ObservableCollection<AnTaskViewModel> ActiveTasks
		{
			get
			{
				if(null == activeTasks)
				{
					activeTasks = new ObservableCollection<AnTaskViewModel>();
				}
				return activeTasks;
			}
		}

		#endregion // Custom Properties

	}
}
