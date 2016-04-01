using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Casamia
{
	struct CommandPacket
	{
		public string exeName { get; set; }
		public List<string> arguments { get; set; }
	}
	class CommandParse
	{
		/// <summary>
		/// 解析命令
		/// </summary>
		/// <param name="command"></param>
		/// <param name="filePaths"></param>
		/// <returns></returns>
		public static CommandPacket Parse(string command, List<string> filePaths)
		{
			CommandPacket commandPacket = new CommandPacket();
			commandPacket.exeName = null;
			commandPacket.arguments = new List<string>();

			command = command.Trim();
			int sIndex = command.IndexOf(" ");
			string expectedExe = null;
			string expectedArgument = null;
			if (sIndex < 0)
			{
				expectedExe = command;
			}
			else
			{
				expectedExe = command.Substring(0, sIndex);
				expectedArgument = command.Substring(sIndex).Trim();
			}
			if (expectedExe.StartsWith("%") && expectedExe.EndsWith("%"))
			{
				expectedExe = expectedExe.Substring(1, expectedExe.Length - 2);
				commandPacket.exeName = XMLManage.GetString(expectedExe.ToLower());
			}
			else
			{
				return commandPacket;
			}
			if (filePaths != null && filePaths.Count > 0)
			{
				if (!string.IsNullOrEmpty(expectedArgument))
				{
					int fIndex = expectedArgument.IndexOf("%filepath%", System.StringComparison.OrdinalIgnoreCase);
					if (fIndex >= 0)
					{
						string filepath = expectedArgument.Substring(fIndex, "%filepath%".Length);
						foreach (string path in filePaths)
						{
							string argument = expectedArgument.Replace(filepath, path);
							commandPacket.arguments.Add(argument);
						}
					}
					else
					{
						foreach (string path in filePaths)
						{
							string argument = string.Format("{0} {1}", expectedArgument, path);
							commandPacket.arguments.Add(argument);
						}
					}
				}

			}
			else
			{
				if (!string.IsNullOrEmpty(expectedArgument))
				{
					commandPacket.arguments.Add(expectedArgument);
				}
			}
			return commandPacket;
		}
	}
}
