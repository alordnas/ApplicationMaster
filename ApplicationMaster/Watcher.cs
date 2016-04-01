using System;
using System.IO;
using System.Security.Permissions;

using Casamia.DataSource;
using Casamia.Logging;

namespace Casamia
{
	public class Watcher
	{

		private static MainWindow mainWindow = App.Current.MainWindow as MainWindow;

		TreeNode _parent;
		public Watcher(TreeNode parent)
		{
			_parent = parent;

			Run(_parent.filePath);
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public void Run(string dir)
		{

			try
			{
				// Create a new FileSystemWatcher and set its properties.
				FileSystemWatcher watcher = new FileSystemWatcher();
				watcher.Path = dir;
				watcher.NotifyFilter = NotifyFilters.DirectoryName;

				watcher.Created += new FileSystemEventHandler(OnCreated);
				watcher.Deleted += new FileSystemEventHandler(OnDeleted);
				watcher.Renamed += new RenamedEventHandler(OnRenamed);

				watcher.EnableRaisingEvents = true;
			}
			catch (Exception e)
			{
				if (mainWindow != null)
				{
					LogManager.Instance.LogError(e.Message);
				}
			}



		}


		private void OnCreated(object source, FileSystemEventArgs e)
		{
			if (!_parent.isDeepLimited && !_parent.isProject)
			{
				if (mainWindow != null)
				{
					mainWindow.AddTreeNodeChild(_parent, e.FullPath);
				}
			}
		}

		private void OnDeleted(object source, FileSystemEventArgs e)
		{
			if (mainWindow != null)
			{
				mainWindow.DeleteTreeNodeChild(_parent, e.FullPath);
			}
		}

		private void OnRenamed(object source, RenamedEventArgs e)
		{
			if (mainWindow != null)
			{
				mainWindow.RenameTreeNodeChild(_parent, e.OldFullPath, e.FullPath);
			}
		}
	}
}

