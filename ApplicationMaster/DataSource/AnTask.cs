using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Casamia.DataSource
{
	public class AnTask : INotifyPropertyChanged, ICloneable
	{
		#region VARIABLE

		private string description;
		private string name;
		private DateTime startTime;
		private CommandStatus status;
		private List<Command> commandList = new List<Command>();

		#endregion VARIABLE

		#region PROPERTIES
		public string Description
		{
			get { return description; }
			set
			{
				description = value;
				OnPropertyChanged("Description");
			}
		}
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged("Name");
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
					if (null != PropertyChanged)
					{
						OnPropertyChanged("StartTime");
					}
				}
			}
		}
		public int ID
		{
			get;
			set;
		}

		public Command[] Commands
		{
			get
			{
				return commandList.ToArray();
			}
		}

		public CommandStatus Status
		{
			get
			{
				return status;
			}
		}


		#endregion PROPERTIES

		#region PUBLIC

		public void AddChild(Command command)
		{
			commandList.Add(command);
			command.StatusChanged += command_StatusChanged;
		}

		public void Clear()
		{
			commandList.Clear();
		}

		public void AddChildren(IEnumerable<Command> commands)
		{
			foreach (var item in commands)
			{
				item.StatusChanged += command_StatusChanged;
			}
			commandList.AddRange(commands);
		}

		public object Clone()
		{
			Command[] cloneCommands = new Command[this.commandList.Count];
			for (int length = commandList.Count, i = 0; i < length; i++)
			{
				cloneCommands[i] = commandList[i].Clone() as Command;
			}
			AnTask anotherTask = new AnTask()
			{
				description = this.description,
				name = this.name,
				status = CommandStatus.Waiting,
			};
			anotherTask.commandList.Clear();
			anotherTask.commandList.AddRange(cloneCommands);
			return anotherTask;
		}

		#endregion PUBLIC

		#region FUNCTION

		void command_StatusChanged(object sender, Model.EventArgs.CommandStatusEventArgs e)
		{
			CommandStatus newStatus = GetStatus();
			if(newStatus != status)
			{
				status = newStatus;
				OnPropertyChanged("Status");
			}
		}

		CommandStatus GetStatus()
		{	
			if (null == Commands || Commands.Length == 0)
			{
				return CommandStatus.Error;
			}
			else
			{
				for (int i = Commands.Length - 1; i > 0; i--)
				{
					Command command = Commands[i];
					if (null != command && command.Status != CommandStatus.Waiting)
					{
						return command.Status;
					}
				}
				return Commands[0].Status;
			}
		}

		void OnPropertyChanged(string name)
		{
			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion FUNCTION

		public event PropertyChangedEventHandler PropertyChanged;

	}
}
