using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Casamia.Model;

namespace Casamia.Converter
{
	class ProgressConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			Command[] commands = value as Command[];
			if (null == commands || commands.Length == 0)
			{
				return "Error";
			}
			else
			{
				int length = commands.Length - 1;
				for (; 0 <= length; length--)
				{
					if (commands[length].Status != CommandStatus.Waiting)
					{
						break;
					}
				}
				return string.Format("{0}/{1}", length + 1, commands.Length);
			}
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
			if (null == anTask)
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
					if (null != command && command.Status != CommandStatus.Waiting)
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

	class EnumDescriptionConverter : IValueConverter
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

	class StringArrayToStringConvert : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (null != value)
			{

				string[] values = value as string[];
				return string.Join(System.Environment.NewLine, values);
			}
			else
			{
				return value;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (null != value)
			{
				string str = value as string;
				return str.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				return value;
			}
		}
	}
}
