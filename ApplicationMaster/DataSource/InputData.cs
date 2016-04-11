using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Casamia.Model
{
    public class InputData : INotifyPropertyChanged
    {
        public static InputData Current;


        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propName)
        {

            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }
        }

        private string _path;

        public string Path 
        {
            get 
            {
                return _path;
            }
            set 
            {
                _path = value;
				
                Notify("Path");
            }
        }

		private string _userJob;
		public string UserJob 
		{
			get 
			{
				return _userJob;
			}
			set 
			{
				_userJob = value;
				Notify("UserJob");
			}
		}

		private int _percent = 0;

		public int Percent 
		{
			get 
			{
				return _percent;
			}
			set 
			{
				_percent = value;
				Notify("Percent");
			}
		}

    }


}
