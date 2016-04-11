using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Casamia.Model;

namespace Casamia.Model.EventArgs
{
	public class CommandStatusEventArgs : System.EventArgs
	{
		public CommandStatus OldStatus
		{
			get;
			set;
		}

		public CommandStatus NewStatus
		{
			get;
			set;
		}

		public CommandStatusEventArgs(CommandStatus oldStatus , CommandStatus newStatus):
			base()
		{
			OldStatus = oldStatus;
			NewStatus = newStatus;
		}
	}
}
