using Casamia.DataSource;

namespace Casamia.Model.EventArgs
{
	public class CommandEventArgs : System.EventArgs
	{
		public string Message
		{
			get;
			private set;
		}
		public CommandStatus Status
		{
			get;
			private set;
		}

		public CommandEventArgs(string message, CommandStatus status)
			: base()
		{
			Message = message;
			Status = status;
		}

	}
}
