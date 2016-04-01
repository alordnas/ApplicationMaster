using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casamia.Core
{
	public class SilentWorker : BackgroundWorker
	{

		Action completed;

		public string[] resultStrings;

		public bool[] resultBools;

		public int[] resultInts;

		public SilentWorker() 
		{
			DoWork += OnDoWork;

			RunWorkerCompleted += OnRunWorkerCompleted;
		}

		public void Do(Action work, Action onCompleted) 
		{
			completed = onCompleted;

			this.RunWorkerAsync(work);
		}

		private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (completed != null)
			{
				completed();
			}
		}

		private void OnDoWork(object sender, DoWorkEventArgs e)
		{
			if ( e.Argument != null)
			{
				Action work = e.Argument as Action;

				work();
			}
		}
	}
}
