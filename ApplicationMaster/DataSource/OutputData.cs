using Casamia.MyFacility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casamia.Model
{
    public class OutputData : INotifyPropertyChanged
    {
        public static OutputData Current;


        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propName)
        {

            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }

        }

        public bool IsTodaysLog = true;

        public string _logText_Old; 

        public string _logText_Today = "";

        public void NotifyLogChanged() 
        {
            Notify("LogText");
        }

        public string LogText
        {
            get
            {
                if (IsTodaysLog)
                    return _logText_Today;
                else
                    return _logText_Old;
                
            }
            set
            {
                if (IsTodaysLog)
                    _logText_Today = value;
                else
                    _logText_Old = value;

                Notify("LogText");
            }
        }

        private string _logFile;
        public string LogFile
        {
            get
            {
                return _logFile;
            }
            set
            {
                _logFile = value;

                Notify("LogFile");
            }
        }


        private ObservableCollection<string> _logFiles= new ObservableCollection<string>();
        public ObservableCollection<string> LogFiles
        {
            get
            {
                return _logFiles;
            }
            set
            {
                _logFiles = value;
                Notify("LogFiles");
            }
        }





        private string _title;

        public string Title 
        {
            get 
            {
                return _title;
            }
            set 
            {
                _title = value;
                Notify("Title");
            }
        }
    }
}
