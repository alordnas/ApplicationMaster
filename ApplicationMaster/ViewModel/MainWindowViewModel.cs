using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Casamia.Core;
using Casamia.Model;

namespace Casamia.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Variable

		private ObservableCollection<AnTaskViewModel> activeTasks;
		private AnTaskViewModel selectedActiveTask;
		private TaskCollectionViewModel taskCollectionViewModel;
		private WorkSpaceCollectionViewModel workSpaceCollectionViewModel;

		private RelayCommand _executeProtoTaskCommand;
		private RelayCommand _switchWorkSpaceCommand;

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

		public WorkSpaceCollectionViewModel WorkSpaceCollectionViewModel
		{
			get
			{
				if (null == workSpaceCollectionViewModel)
				{
					workSpaceCollectionViewModel = new WorkSpaceCollectionViewModel();
				}
				return workSpaceCollectionViewModel;
			}
		}

		public TaskCollectionViewModel TaskCollectionViewModel
		{
			get
			{
				if(null == taskCollectionViewModel)
				{
					taskCollectionViewModel = new TaskCollectionViewModel();
				}
				return taskCollectionViewModel;
			}
		}

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
			set
			{
				if (selectedActiveTask != value)
				{
					selectedActiveTask = value;
					OnPropertyChanged("SelectedActiveTask");
				}
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

		#region Presentation

		public ICommand SwitchWorkSpaceCommand
		{
			get
			{
				if(null == _switchWorkSpaceCommand)
				{
					_switchWorkSpaceCommand = new RelayCommand(
						(p) =>
						{
							WorkSpaceViewModel workSpace = p as WorkSpaceViewModel;
							if (null != workSpace && WorkSpaceCollectionViewModel.CurrentWorkSpace != workSpace)
							{
								WorkSpaceCollectionViewModel.CurrentWorkSpace = workSpace;
							}
						},
						(p) =>
						{
							return WorkSpaceCollectionViewModel.WorkSpaces.Count > 0;
						}
						);
				}
				return _switchWorkSpaceCommand;
			}
		}

		public ICommand ExecuteProtoTaskCommmand
		{
			get
			{
				if (null == _executeProtoTaskCommand)
				{
					_executeProtoTaskCommand = new RelayCommand(
						(p) =>
						{
							AnTaskViewModel taskViewModel = p as AnTaskViewModel;
							if (null != taskViewModel)
							{
								string[] projectPaths = null;
								List<TreeNode> nodes = TreeHelper.GetSelectedProjects(TreeNode.Root);
								projectPaths = TreeHelper.GetTreeNodePaths(nodes);
								CommonTask.RunTask(taskViewModel.Task, projectPaths);
							}
						}
						);
				}
				return _executeProtoTaskCommand;
			}
		}

		#endregion // Presentation

		#region Properties

		public override string DisplayName
		{
			get
			{
				return "this is the main";
			}
			protected set
			{
				base.DisplayName = value;
			}
		}

		#endregion // Properties

		#region RequestClose [event]

		/// <summary>
		/// Raised when this workspace should be removed from the UI.
		/// </summary>
		public event EventHandler RequestClose;

		void OnRequestClose()
		{
			EventHandler handler = this.RequestClose;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		#endregion // RequestClose [event]
	}
}
