using System;
using System.ComponentModel;

namespace Casamia.DataSource
{
	public enum CommandStatus
	{
		[Description("等待中")]
		Waiting,
		[Description("运行中")]
		Running,
		[Description("错误")]
		Error,
		[Description("超时")]
		Timeout,
		[Description("完成")]
		Completed,
		[Description("取消")]
		Cancel
	}
}
