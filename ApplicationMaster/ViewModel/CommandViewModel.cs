using System;
using System.Text;
using System.Windows.Input;

using Casamia.Core;
using Casamia.Model;

namespace Casamia.ViewModel
{
    public class CommandViewModel : ViewModelBase
    {
        #region Fields

        readonly Command _command;
        RelayCommand addUrlCommand;
        RelayCommand addProjectCommand;
        bool isSelected;
        private Executor executor;
        private DateTime startTime;
        private TimeSpan duration;
        private StringBuilder transientLogBuilder = new StringBuilder();
        private int transientCounter = 0;
        private StringBuilder logBuilder = new StringBuilder();
        private StringBuilder errorBuilder = new StringBuilder();

        #endregion // Fields

        #region Constructor

        public CommandViewModel(Command command)
        {
            _command = command;
            _command.CommandFeedbackReceived += _command_CommandFeedbackReceived;
            _command.StatusChanged += _command_StatusChanged;
            _command.ErrorOccur += _command_ErrorOccur;
        }

        private void _command_ErrorOccur(object sender, Model.EventArgs.CommandEventArgs e)
        {
            OnPropertyChanged("Error");
        }

        private void _command_StatusChanged(object sender, Model.EventArgs.CommandStatusEventArgs e)
        {
            if (e.OldStatus != CommandStatus.Running && e.NewStatus == CommandStatus.Running)
            {
                startTime = DateTime.Now;
                OnPropertyChanged("StartTime");
            }
            OnPropertyChanged("Status");
        }

        private void _command_CommandFeedbackReceived(object sender, Model.EventArgs.CommandEventArgs e)
        {
            Output = e.Message;
        }

        #endregion // Constructor

        #region Custom Properties

        public string Argument
        {
            get
            {
                return _command.Argument;
            }
            set
            {
                if (!string.Equals(_command.Argument, value))
                {
                    _command.Argument = value;
                    base.OnPropertyChanged("Argument");
                }
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
                return _command.Description;
            }
            set
            {
                if (!string.Equals(_command.Description, value))
                {
                    _command.Description = value;
                    base.OnPropertyChanged("Description");
                }
            }
        }

        public Executor Executor
        {
            get
            {
                if (null == executor)
                {
                    executor = ExecutorManager.Instance.GetByPlaceHolder(_command.Executor);
                }
                return executor;
            }
            private set
            {
                if (executor != value && !string.Equals(executor.PlaceHolder, _command.Executor))
                {
                    executor = value;
                    _command.Executor = executor.PlaceHolder;
                    base.OnPropertyChanged("Executor");
                }
            }
        }

        public string Output
        {
            get
            {
                lock (transientLogBuilder)
                {
                    return transientLogBuilder.ToString();
                }
            }
            private set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (transientCounter < 10)
                    {
                        if (_command.Status == CommandStatus.Running)
                        {
                            duration = DateTime.Now - startTime;
                        }
                        OnPropertyChanged("Duration");
                        OnPropertyChanged("Output");
                    }
                    else
                    {
                        transientCounter = 0;
                        transientLogBuilder.Clear();
                    }

                    transientCounter++;
                    transientLogBuilder.AppendLine(value);
                }
            }
        }

        public string Error
        {
            get
            {
                return _command.ErrorLog;
            }
        }

        public CommandStatus Status
        {
            get
            {
                return _command.Status;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
        }

        public TimeSpan TimeCost
        {
            get
            {
                return duration;
            }
        }

        #endregion // Custom Properties

        #region Presentation Properties

        public ICommand AddUrlCommand
        {
            get
            {
                if (null == addUrlCommand)
                {
                    addUrlCommand = new RelayCommand(
                        (par) =>
                        {
                            this.Argument = string.Format("{0} {1}", this.Argument, Util.PROJECT_URL_PLACEHOLDER);
                        }
                        );
                }
                return addUrlCommand;
            }
        }
        
        public ICommand AddProjectCommand
        {
            get
            {
                if (null == addProjectCommand)
                {
                    addProjectCommand = new RelayCommand(
                        (par) =>
                        {
                            this.Argument = string.Format("{0} {1}", this.Argument, Util.PROJECT_PATH_PLACEHOLDER);
                        }
                        );
                }
                return addProjectCommand;
            }
        }
        
        #endregion // Presentation Properties

        #region Properties

        public override string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(_command.Description))
                {
                    return _command.Description;
                }
                else
                {
                    return string.Format("{0} {1}", _command.Executor, _command.Argument);
                }
            }
        }

        #endregion // Properties

    }
}
