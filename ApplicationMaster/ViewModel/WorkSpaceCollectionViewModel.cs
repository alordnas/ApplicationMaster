using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Casamia.Core;
using Casamia.Model;

namespace Casamia.ViewModel
{
	public class WorkSpaceCollectionViewModel : ViewModelBase
	{
		#region Variable

		private ObservableCollection<WorkSpaceViewModel> workSpaces;
		private WorkSpaceViewModel currentWorkSpaceViewModel;

		private RelayCommand addWorkSpaceCommand;
		private RelayCommand removeWorkSpaceCommand;

		#endregion // Variable

		#region Custom Properties

		public ObservableCollection<WorkSpaceViewModel> WorkSpaces
		{
			get
			{
				if(null == workSpaces)
				{
					workSpaces = new ObservableCollection<WorkSpaceViewModel>();
					WorkSpace[] workSpaceArray = WorkSpaceManager.Instance.WorkSpaces;
					foreach (var item in workSpaceArray)
					{
						workSpaces.Add(new WorkSpaceViewModel(item));
					}
				}
				return workSpaces;
			}
		}

		public WorkSpaceViewModel CurrentWorkSpace
		{
			get
			{
				if (null == currentWorkSpaceViewModel && WorkSpaces.Count > 0) 
				{
					currentWorkSpaceViewModel = WorkSpaces[0];
				}
				return currentWorkSpaceViewModel;
			}
			set
			{
				if(value!= currentWorkSpaceViewModel)
				{
					currentWorkSpaceViewModel = value;
					OnPropertyChanged("CurrentWorkSpace");
				}
			}
		}

		#endregion // Custom Properties

		#region Presentation Properties

		public ICommand AddWorkSpaceCommand
		{
			get
			{
				if(null == addWorkSpaceCommand)
				{
					addWorkSpaceCommand = new RelayCommand((p) =>
					{
						WorkSpace workSpace = new WorkSpace()
						{
							Name=string.Format("WorkSpace #{0}" , WorkSpaceManager.Instance.WorkSpaces.Length),
							Description="Say something",
							CreateProjectTask = new AnTask(),
							ImportProjectTask =new AnTask(),
						};
						WorkSpaceManager.Instance.Add(workSpace);
						WorkSpaceViewModel workSpaceViewModel = new WorkSpaceViewModel(workSpace);
						CurrentWorkSpace = workSpaceViewModel;
						WorkSpaces.Add(workSpaceViewModel);
					});
				}
				return addWorkSpaceCommand;
			}
		}

		public ICommand RemoveWorkSpaceCommand
		{
			get
			{
				if(null == removeWorkSpaceCommand)
				{
					removeWorkSpaceCommand = new RelayCommand((p) =>
					{
						WorkSpaceManager.Instance.Remove(currentWorkSpaceViewModel.WorkSpace);
						WorkSpaces.Remove(currentWorkSpaceViewModel);
						if (WorkSpaces.Count > 0)
						{
							CurrentWorkSpace = WorkSpaces[0];
						}
					},
					(p) => null != currentWorkSpaceViewModel
					);
				}
				return removeWorkSpaceCommand;
			}
		}

		#endregion // Presentation Properties

	}
}
