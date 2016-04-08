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
		[Newtonsoft.Json.JsonProperty]
		private List<Command> commandList = new List<Command>();
		private TimeSpan timeCost;

		#endregion VARIABLE

		#region PROPERTIES

		public TimeSpan TimeCost
		{
			get { return timeCost; }
			private set
			{
				if (!TimeSpan.Equals(timeCost, value))
				{
					timeCost = value;
					OnPropertyChanged("TimeCost");
				}
			}
		}

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
			private set
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

		[Newtonsoft.Json.JsonIgnore]
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

		public void AddCommand(Command command)
		{
			commandList.Add(command);
			command.StatusChanged += command_StatusChanged;
		}

		public void Clear()
		{
			commandList.Clear();
		}

		public void AddCommands(IEnumerable<Command> commands)
		{
			foreach (var item in commands)
			{
				item.StatusChanged += command_StatusChanged;
			}
			commandList.AddRange(commands);
		}

		public object Clone()
		{
			AnTask anotherTask = new AnTask()
			{
				description = this.description,
				name = this.name,
				status = CommandStatus.Waiting,
			};

			Command[] existCommands = this.Commands;
			int commandCount = existCommands.Length;
			Command[] cloneCommands = new Command[commandCount];
			for (int i = 0; i < commandCount; i++)
			{
				Command command = existCommands[i].Clone() as Command;
				if (null != command)
				{
					cloneCommands[i] = command;
				}
			}
			anotherTask.AddCommands(cloneCommands);
			return anotherTask;
		}

		#endregion PUBLIC

		#region FUNCTION

		void command_StatusChanged(object sender, Model.EventArgs.CommandStatusEventArgs e)
		{
			if (e.NewStatus == CommandStatus.Completed || e.NewStatus == CommandStatus.Error)
			{
				OnPropertyChanged("Commands");
			}
			CommandStatus newStatus = GetStatus();

			if(status == CommandStatus.Waiting && newStatus == CommandStatus.Running)
			{
				StartTime = DateTime.Now;
			}

			if (newStatus != status)
			{
				status = newStatus;
				OnPropertyChanged("Status");
			}
			if (status != CommandStatus.Waiting)
			{
				TimeCost = (DateTime.Now - startTime);
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

		public override string ToString()
		{
			return this.name;
		}

		#endregion FUNCTION

		public event PropertyChangedEventHandler PropertyChanged;

	}
}
