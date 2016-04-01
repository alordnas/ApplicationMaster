using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casamia.Logging
{
	public interface ILogger
	{
		void Clear();

		void Log(Log log);
	}
}
