using Casamia.MyFacility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casamia.Core;

namespace Casamia.Model
{
    public class CreateCaseData : INotifyPropertyChanged
    {
        public static CreateCaseData Current;

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string propName)
        {

            if (PropertyChanged != null)
            {

                PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }

        }


        private string _parentPath = "";
        public string ParentPath
        {
            get 
            {
                return _parentPath;
            }
            set 
            {
                _parentPath = TreeHelper.RectifyPath(value);
                RefreshPorjectInfo();
                Notify("ParentPath");
            }
        }

        private string _projectName = "";
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                Notify("ProjectName");
            }
        }




        private string _svnRootPath = "";
        public string SvnRootPath
        {
            get
            {
                return _svnRootPath;
            }
            set
            {
                _svnRootPath = value;
                Notify("SvnRootPath");
            }
        }

        private string _createThesePath = "";

        public string CreateThesePath
        {
            get
            {
				return _createThesePath;
            }
            set
            {
				_createThesePath = value;
				Notify("CreateThesePath");
            }
        }


		public bool IsGoodFolder()
		{
			string subPath = ParentPath.Substring(WorkSpaceManager.Instance.WorkingPath.Length - 1);
			int deep = 0;
			for (int i = 0, length = subPath.Length; i < length; i++)
			{
				if (subPath[i] == '/' && i != length - 1)
					deep++;
			}

			return WorkSpaceManager.Instance.Current.UrlDepth - deep == 1;
		}


		public string GetProjcetPath() 
		{
			string name = ProjectName;
			if (string.IsNullOrEmpty(name))
				return null;


			return string.Format("{0}/{1}", ParentPath, ProjectName);
		}


		public void RefreshPorjectInfo()
		{
			CreateCaseData.Current.CreateThesePath = "";

			string url = string.Empty;
			string subPath = CreateCaseData.Current.ParentPath.Replace(
				TreeHelper.RectifyPath(WorkSpaceManager.Instance.WorkingPath),
				""
				);
            if (subPath.Length < CreateCaseData.Current.ParentPath.Length)
            {
                url = string.Format("{0}{1}", url, subPath);
                CreateCaseData.Current.SvnRootPath = TreeHelper.RectifyPath(url);
                return;
            }

            CreateCaseData.Current.SvnRootPath = TreeHelper.RectifyPath(url);
		}
    }
}
