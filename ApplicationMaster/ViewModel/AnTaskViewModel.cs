using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Casamia.Model;
using System.Collections.ObjectModel;
namespace Casamia.ViewModel
{
    public class AnTaskViewModel : ViewModelBase
    {
        #region Variable

        readonly AnTask _anTask;
        private TimeSpan duration;
        private DateTime startTime;

        private RelayCommand _addCommandCmd;
        private RelayCommand _removeCommandCmd;
        private RelayCommand _clearCommandCmd;

        private CommandViewModel _selectedCommand;

        private ObservableCollection<CommandViewModel> commandViewModelCollection;
        bool isStart = false;
        private CommandStatus status = CommandStatus.Waiting;

        #endregion // Variable

        #region Constructon

        public AnTaskViewModel(AnTask anTask)
        {
            _anTask = anTask;

            if (null == commandViewModelCollection)
            {
                commandViewModelCollection = new ObservableCollection<CommandViewModel>();
                Command[] commands = _anTask.Commands;
                for (int i = 0; i < commands.Length; i++)
                {
                    Command command = commands[i];
                    commandViewModelCollection.Add(new CommandViewModel(command));
                    command.CommandFeedbackReceived += Command_CommandFeedbackReceived;
                    command.StatusChanged += Command_StatusChanged;
                }
            }
        }


        #endregion // Constructon

        #region Custom Properties

        public AnTask Task
        {
            get
            {
                return _anTask;
            }
        }

        public string Name
        {
            get
            {
                return _anTask.Name;
            }
            set
            {
                if (!string.Equals(value, _anTask.Name))
                {
                    _anTask.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public CommandStatus Status
        {
            get
            {
                return status;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
        }

        public string Description
        {
            get
            {
                return _anTask.Description;
            }
            set
            {
                if (!string.Equals(_anTask.Description, value))
                {
                    _anTask.Description = value;
                    base.OnPropertyChanged("Description");
                }
            }
        }

        public CommandViewModel SelectedCommand
        {
            get
            {
                if (null == _selectedCommand
                    && CommandViewModelCollection != null
                    && CommandViewModelCollection.Count > 0)
                {
                    _selectedCommand = CommandViewModelCollection[0];
                }
                return _selectedCommand;
            }
            set
            {
                if (_selectedCommand != value)
                {
                    _selectedCommand = value;
                    OnPropertyChanged("SelectedCommand");
                }
            }
        }

        public ObservableCollection<CommandViewModel> CommandViewModelCollection
        {
            get
            {

                return commandViewModelCollection;
            }
        }

        #endregion // Custom Properties

        #region Presentation Properties


        public ICommand AddCommandCommand
        {
            get
            {
                if (null == _addCommandCmd)
                {
                    _addCommandCmd = new RelayCommand(
                        (par) =>
                        {
                            // TODO : add command
                            Command newCommand = new Command()
                            {
                                Description = "describe what i do.",
                                // default 
                                Timeout = new TimeSpan(0, 0, -1),
                            };
                            _anTask.AddCommand(newCommand);
                            CommandViewModel commandViewModel = new CommandViewModel(newCommand);
                            commandViewModelCollection.Add(commandViewModel);
                            SelectedCommand = commandViewModel;
                        }
                        );
                }
                return _addCommandCmd;
            }
        }

        public ICommand RemoveCommandCommand
        {
            get
            {
                if (null == _removeCommandCmd)
                {
                    _removeCommandCmd = new RelayCommand(
                        (par) =>
                        {
                            _anTask.RemoveCommand(_selectedCommand.Command);
                            CommandViewModelCollection.Remove(_selectedCommand);
                            _selectedCommand = null;
                            OnPropertyChanged("SelectedCommand");
                        },
                        (par) =>
                        {
                            return null != _selectedCommand;
                        }
                        );
                }
                return _removeCommandCmd;
            }
        }

        public ICommand ClearCommandCommand
        {
            get
            {
                if (null == _clearCommandCmd)
                {
                    _clearCommandCmd = new RelayCommand(
                        (par) =>
                        {
                            _anTask.Clear();
                            SelectedCommand = null;
                        }
                        );
                }
                return _clearCommandCmd;
            }
        }


        #endregion // Presentation Properties

        #region Properties

        public override string DisplayName
        {
            get
            {
                return _anTask.Name;
            }
        }
        #endregion // Properties

        #region Function

        private CommandStatus GetStatus()
        {
            Command[] commands = _anTask.Commands;

            if (null == commands || commands.Length == 0)
            {
                return CommandStatus.Error;
            }

            int count = commands.Length;
            for (int i = count - 1; i > 0; i--)
            {
                if (commands[i].Status != CommandStatus.Waiting)
                {
                    return commands[i].Status;
                }
            }
            return CommandStatus.Waiting;
        }

        #endregion // Function

        #region Event Handler

        private void Command_StatusChanged(object sender, Model.EventArgs.CommandStatusEventArgs e)
        {
            if (!isStart)
            {
                isStart = true;
                startTime = DateTime.Now;
                OnPropertyChanged("StartTime");
            }
            CommandStatus newStatus = GetStatus();
            if (newStatus != status)
            {
                status = newStatus;
                OnPropertyChanged("Status");
            }
        }

        private void Command_CommandFeedbackReceived(object sender, Model.EventArgs.CommandEventArgs e)
        {
            if (Status == CommandStatus.Running)
            {
                duration = DateTime.Now - startTime;
                OnPropertyChanged("Duration");
            }
        }

        #endregion Event Handler
    }
}
