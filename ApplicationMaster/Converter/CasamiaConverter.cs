using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Casamia.DataSource;

namespace Casamia.Converter
{
	class ScheduleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class TaskStatusConvert : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			AnTask anTask = value as AnTask;
			if(null == anTask)
			{	
				return CommandStatus.Error;
			}
			Command[] commands = anTask.Commands;
			if (null == commands || commands.Length == 0)
			{
				return CommandStatus.Error;
			}
			else
			{
				for (int i = commands.Length - 1; i > 0; i--)
				{
					Command command = commands[i];
					if(null != command && command.Status!= CommandStatus.Waiting)
					{
						return command.Status;
					}
				}
				return commands[0].Status;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	class EnumDescriptionConverter:IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var memInfo = targetType.GetMember(value.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
				false);
			return ((DescriptionAttribute)attributes[0]).Description;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
