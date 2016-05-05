using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using Casamia.Model;
using Casamia.Core;

namespace Casamia.ViewModel
{
    public class CommandViewModel : ViewModelBase
    {
        #region Fields

        readonly Command _command;
        readonly ObservableCollection<Command> commandCollection;
        RelayCommand addCommand;
        RelayCommand removeCommand;
        bool isSelected;
        private Executor executor;

        #endregion // Fields

        #region Constructor

        public CommandViewModel(Command command, ObservableCollection<Command> commands)
        {
            _command = command;
            commandCollection = commands;
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
                if (executor != value)
                {
                    executor = value;
                    base.OnPropertyChanged("Executor");
                }
            }
        }

        public string ExecutorPlaceHolder
        {
            get
            {
                return _command.Executor;
            }
            set
            {
                if (!string.Equals(_command.Executor, value))
                {
                    Executor executor = ExecutorManager.Instance.GetByPlaceHolder(value);
                    if (null == executor)
                    {
                        Logging.LogManager.Instance.LogError(
                            "{0} cannot find corresponding application",
                            value
                            );
                    }
                    else
                    {
                        _command.Executor = value;
                        base.OnPropertyChanged("ExecutorPlaceHolder");
                        Executor = executor;
                    }
                }
            }
        }

        #endregion // Custom Properties

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
