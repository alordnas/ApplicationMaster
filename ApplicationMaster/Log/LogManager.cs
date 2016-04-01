using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Casamia.Logging
{
	public class LogManager
	{
		ILogger defaultLogger;

		private static LogManager instance;
		public Log.level AllowLevel
		{
			get;
			set;
		}
		public static LogManager Instance
		{
			get
			{
				if (null == instance)
				{
					instance = new LogManager();
				}
				return instance;
			}
		}

		public void SetLogger(ILogger logger)
		{
			defaultLogger = logger;
		}


		public void LogError(string msg, params object[] param)
		{
			Log log = new Casamia.Logging.Log()
			{
				Date = DateTime.Now,
				Level = Casamia.Logging.Log.level.Error,
				Message = string.Format(msg, param),
			};
			if (AllowLevel.HasFlag(log.Level))
			{
				Log(log);
			}
		}

		public void LogDebug(string msg, params object[] param)
		{
			Log log = new Casamia.Logging.Log()
			{
				Date = DateTime.Now,
				Level = Casamia.Logging.Log.level.Verbose,
				Message = string.Format(msg, param),
			};
			if (AllowLevel.HasFlag(log.Level))
			{
				Log(log);
			}
		}


		public void LogInfomation(string msg, params object[] param)
		{
			Log log = new Casamia.Logging.Log()
			{
				Date = DateTime.Now,
				Level = Casamia.Logging.Log.level.Infomation,
				Message = string.Format(msg, param),
			};
			if (AllowLevel.HasFlag(log.Level))
			{
				Log(log);
			}
		}

		public void LogWarning(string msg, params object[] param)
		{
			Log log = new Casamia.Logging.Log()
			{
				Date = DateTime.Now,
				Level = Casamia.Logging.Log.level.Waring,
				Message = string.Format(msg, param),
			};
			if (AllowLevel.HasFlag(log.Level))
			{
				Log(log);
			}
		}

		private void Log(Log log)
		{
			if (AllowLevel.HasFlag(log.Level))
			{
				defaultLogger.Log(log);
			}
		}


		public void Log(string msg, Log.level level, params object[] param)
		{
			Log log = new Casamia.Logging.Log()
			{
				Date = DateTime.Now,
				Level = level,
				Message = string.Format(msg, param),
			};

			if (AllowLevel.HasFlag(log.Level))
			{
				defaultLogger.Log(log);
			}
		}

		public void Clear()
		{
			defaultLogger.Clear();
		}

	}
}
