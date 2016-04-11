﻿using System;
using System.ComponentModel;
using System.Text;

using Casamia.Core;
using Casamia.Model;
using Casamia.Model.EventArgs;

namespace Casamia.Model
{
	public class Command : ICloneable, INotifyPropertyChanged
	{
		#region EVENT

		public event EventHandler<CommandEventArgs> CommandFeedbackReceived;
		public event EventHandler<CommandEventArgs> ErrorOccur;
		public event EventHandler<CommandStatusEventArgs> StatusChanged;

		#endregion EVENT

		#region VARIABLE

		private StringBuilder logBuilder = new StringBuilder();
		private StringBuilder errorBuilder = new StringBuilder();
		private string _exe;
		private string _executor;
		private string argument = string.Empty;
		private DateTime startTime;
		private TimeSpan timeCost;
		private TimeSpan timeout;
		private CommandStatus status = CommandStatus.Waiting;
		private string description;


		#endregion

		#region PROPERTIES

		public event PropertyChangedEventHandler PropertyChanged;

		public string Description
		{
			get { return description; }
			set
			{
				description = value;
			}
		}
		public string Exe
		{
			get
			{
				return _exe;
			}
			private set
			{
				_exe = value;
			}
		}

		public string Executor
		{
			get
			{
				return _executor;
			}
			set
			{
				if (!string.Equals(_executor, value))
				{
					_executor = value;
					Executor executor = ExecutorManager.Instance.GetByPlaceHolder(_executor);
					if(null != executor)
					{
						_exe = executor.Path;
					}
					else
					{
						Logging.LogManager.Instance.LogError(
							"{0} cannot find corresponding application",
							_executor
							);
					}
				}
			}
		}

		public string Output
		{
			get
			{
				lock (logBuilder)
				{
					return logBuilder.ToString();
				}
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					lock (logBuilder)
					{
						logBuilder.AppendLine(value);
					}
					OnPropertyChanged("TimeCost");
					if (null != CommandFeedbackReceived)
					{
						CommandFeedbackReceived(
							this, 
							new CommandEventArgs(value, CommandStatus.Running)
							);
					}
					OnPropertyChanged("Output");
				}
			}
		}

		public string Error
		{
			get
			{
				lock (errorBuilder)
				{
					return errorBuilder.ToString();
				}
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					lock (errorBuilder)
					{
						errorBuilder.Append(value);
					}
					if (null != ErrorOccur)
					{
						ErrorOccur(this, new CommandEventArgs(value, Status));
					}
					Status = CommandStatus.Error;
					OnPropertyChanged("Error");
				}
			}
		}


		public CommandStatus Status
		{
			get { return status; }
			set
			{
				if (status != value)
				{
					CommandStatus oldStatus = status;
					status = value;
					if(oldStatus == CommandStatus.Waiting && status!= CommandStatus.Running)
					{
						StartTime = DateTime.Now;
					}
					if (null != StatusChanged)
					{
						StatusChanged(this, new CommandStatusEventArgs(oldStatus, status));
					}
					OnPropertyChanged("Status");
				}
			}
		}

		public string Argument
		{
			get { return argument; }
			set
			{
				if (!string.Equals(argument, value))
				{
					argument = value;
					OnPropertyChanged("Argument");
				}
			}
		}

		public TimeSpan Timeout
		{
			get { return timeout; }
			set
			{
				if (timeout != value)
				{
					timeout = value;
					OnPropertyChanged("Timeout");
				}
			}
		}


		public TimeSpan TimeCost
		{
			get { return timeCost; }
			set
			{
				if (timeCost != value)
				{
					timeCost = value;
					OnPropertyChanged("TimeCost");
				}
			}
		}

		public DateTime StartTime
		{
			get { return startTime; }
			set
			{
				if (!DateTime.Equals(startTime, value))
				{
					startTime = value;
					OnPropertyChanged("StartTime");
				}
			}
		}

		#endregion PROPERTIES

		#region PUBLIC

		public void Reset()
		{
			Status = CommandStatus.Waiting;
			timeCost = TimeSpan.MinValue;
			startTime = DateTime.MinValue;
			lock (logBuilder)
			{
				logBuilder.Clear();
			}
			lock (errorBuilder)
			{
				errorBuilder.Clear();
			}
		}

		public object Clone()
		{
			Command command = new Command()
			{
				timeout = this.timeout,
				status = CommandStatus.Waiting,
				_exe = this._exe,
				_executor = this._executor,
				argument = this.argument,
				startTime = DateTime.MinValue,
			};
			return command;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Executor, Argument, Timeout);
		}

		#endregion PUBLIC

		#region FUNCTION

		void OnPropertyChanged(string name)
		{
			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion FUNCTION

	}
}