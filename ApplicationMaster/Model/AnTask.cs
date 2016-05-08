using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Casamia.Model
{
    public class AnTask : ICloneable
    {
        #region VARIABLE

        private List<Command> commandList = new List<Command>();

        #endregion VARIABLE

        #region PROPERTIES

        public List<Command> CommandList
        {
            get
            {
                return commandList;
            }
            set
            {
                commandList = value;
            }
        }

        public string Description
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonIgnore]
        public Command[] Commands
        {
            get
            {
                Command[] commands = new Command[commandList.Count];
                commandList.CopyTo(commands, 0);
                return commands;
            }
        }

        #endregion PROPERTIES

        #region PUBLIC

        public void AddCommand(Command command)
        {
            if (null != command)
            {
                commandList.Add(command);
            }
        }

        public void RemoveCommand(Command command)
        {
            if (null != command)
            {
                commandList.Remove(command);
            }
        }

        public void Clear()
        {
            commandList.Clear();
        }

        public void AddCommands(IEnumerable<Command> commands)
        {
            foreach (var item in commands)
            {
                commandList.Add(item);
            }

        }

        public object Clone()
        {
            AnTask anotherTask = new AnTask()
            {
                Description = this.Description,
                Name = this.Name
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

        //void command_StatusChanged(object sender, Model.EventArgs.CommandStatusEventArgs e)
        //{
        //    if (e.NewStatus == CommandStatus.Completed || e.NewStatus == CommandStatus.Error)
        //    {
        //        OnPropertyChanged("Commands");
        //    }

        //    if (status == CommandStatus.Waiting && newStatus == CommandStatus.Running)
        //    {
        //        StartTime = DateTime.Now;
        //    }

        //    if (newStatus != status)
        //    {
        //        status = newStatus;
        //    }
        //    if (status != CommandStatus.Waiting)
        //    {
        //        TimeCost = (DateTime.Now - startTime);
        //    }
        //}

        #endregion FUNCTION

    }
}
