using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Casamia.Model
{
	/// <summary>
	/// Exe
	/// </summary>
	public class Executor
	{
		private string placeHolder;
		private string name;
		private string[] potentialParameters;

		public string[] PotentialParameters
		{
			get { return potentialParameters; }
			set { potentialParameters = value; }
		}

		public string Name
		{
			get { return name; }
			set
			{
				if (!string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
				{
					name = value;
					placeHolder = string.Format(Util.EXECUTOR_FORMATTOR, name);
				}
			}
		}

		public string Path
		{
			get;
			set;
		}


		public string PlaceHolder
		{
			get
			{
				return placeHolder;
			}
		}

		public string Description
		{
			get;
			set;
		}
	}
}
