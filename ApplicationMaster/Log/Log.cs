using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casamia.Logging
{

	public class Log
	{
		[Flags]
		public enum level
		{
			None = 0,
			Verbose = 1,
			Infomation = 2,
			Waring = 4,
			Error = 16,
			Fatal = 32,
		}

		public level Level
		{
			get;
			set;
		}
		public DateTime Date
		{
			get;
			set;
		}
		public string Message
		{
			get;
			set;
		}
	}
}
