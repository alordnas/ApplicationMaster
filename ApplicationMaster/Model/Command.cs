using System;
using System.ComponentModel;
using System.Text;

using Casamia.Core;
using Casamia.Model;
using Casamia.Model.EventArgs;

namespace Casamia.Model
{
	public class Command : ICloneable, IDataErrorInfo
	{
		#region EVENT

		public event EventHandler<CommandEventArgs> CommandFeedbackReceived;
		public event EventHandler<CommandEventArgs> ErrorOccur;
		public event EventHandler<CommandStatusEventArgs> StatusChanged;

		#endregion EVENT

		#region VARIABLE

		private string argument = string.Empty;
		private CommandStatus status = CommandStatus.Waiting;
		StringBuilder logBuilder = new StringBuilder();
		StringBuilder errorBuilder = new StringBuilder();

		#endregion

		#region PROPERTIES

		public string Description
		{
			get;
			set;
		}

		public TimeSpan Timeout
		{
			get;
			set;
		}

		public string Executor
		{
			get;
			set;
		}

		public string Result
		{
			get
			{
				lock (logBuilder)
				{
					return logBuilder.ToString();
				}
			}
		}

		public void Output(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				if (null != CommandFeedbackReceived)
				{
					CommandFeedbackReceived(
						this,
						new CommandEventArgs(message, CommandStatus.Running)
						);
				}
			}
			logBuilder.Append(message);
		}

		public string ErrorLog
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
				}
			}
		}


		public CommandStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if (value != status)
				{
					CommandStatus oldStatus = status;
					status = value;
					if (null != StatusChanged)
					{
						StatusChanged(this, new CommandStatusEventArgs(oldStatus, status));
					}
				}
			}
		}

		public string Argument
		{
			get;
			set;
		}

		#endregion PROPERTIES

		#region IDataErrorInfo

		public string Error
		{
			get
			{
				return null;
			}
		}

		public string this[string columnName]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion // IDataErrorInfo

		#region PUBLIC

		public void Reset()
		{
			Status = CommandStatus.Waiting;
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
				status = CommandStatus.Waiting,
				Executor = Executor,
				Argument = Argument,
				Description = Description,
				Timeout = Timeout,
			};
			return command;
		}

		#endregion PUBLIC

	}
}
