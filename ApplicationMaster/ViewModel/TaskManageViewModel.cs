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
	public class TaskManageViewModel : ViewModelBase
	{
		#region Variable

		private ObservableCollection<AnTaskViewModel> protoTasks;
		private RelayCommand addTaskCommand;
		private RelayCommand removeTaskCommand;
		private RelayCommand saveCommand;
		private AnTaskViewModel _selectedTaskViewModel;

		#endregion // Variable

		#region Custom Properties

		public AnTaskViewModel SelectedTask
		{
			get
			{
				return _selectedTaskViewModel;
			}
			set
			{
				if (value != _selectedTaskViewModel)
				{
					_selectedTaskViewModel = value;
					OnPropertyChanged("SelectedTask");
				}
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
					for (int length = tasks.Length, i = 0; i < length; i++)
					{
						protoTasks.Add(new AnTaskViewModel(tasks[i]));
					}
				}
				return protoTasks;
			}
		}

		#endregion // Custom Properties

		#region Presentation Properties

		public ICommand RemoveTaskCommand
		{
			get
			{
				if (null == removeTaskCommand)
				{
					removeTaskCommand = new RelayCommand(
						(par) =>
						{
							TaskManager.RemoveProtoTask(_selectedTaskViewModel.Task);
							ProtoTasks.Remove(_selectedTaskViewModel);
						},
						(par) =>
						{
							return null != _selectedTaskViewModel;
						}
						);
				}
				return removeTaskCommand;
			}
		}

		public ICommand SaveCommand
		{
			get
			{
				if (null == saveCommand)
				{
					saveCommand = new RelayCommand(
						(par) =>
						{
							TaskManager.SaveProtoTasks();
						}
						);
				}
				return saveCommand;
			}
		}

		public ICommand AddTaskCommand
		{
			get
			{
				if (null == addTaskCommand)
				{
					addTaskCommand = new RelayCommand(
						(par) =>
						{
							AnTask anTask = new AnTask()
							{
								Name = string.Format("Task #{0}", TaskManager.ProtoTasks.Length),
							};
							AnTaskViewModel anTaskViewModel = new AnTaskViewModel(anTask);
							ProtoTasks.Add(anTaskViewModel);
						}
						);
				}
				return addTaskCommand;
			}
		}

		#endregion // Presentation Properties
	}
}
