using Casamia.MyFacility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casamia.DataSource
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


		private string _projectStyle = "";
		public string ProjectStyle 
		{
			get 
			{
				return _projectStyle;
			}
			set 
			{
				_projectStyle = value;
				Notify("ProjectStyle");
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

        private string _packagePath;
        public string PackagePath
        {
            get
            {
                return _packagePath;
            }
            set
            {
                
                _packagePath = value;

                Parameter parameter = new Parameter();
                parameter.Key = Util.SAVED_PACKAGE_DIR;
                parameter.Value = value;
                XMLManage.SaveParameter(parameter);
                Notify("PackagePath");
            }
        }


		public bool IsGoodFolder()
		{
			string subPath = ParentPath.Substring(InputData.Current.Path.Length - 1);
			int deep = 0;
			for (int i = 0, length = subPath.Length; i < length; i++)
			{
				if (subPath[i] == '/' && i != length - 1)
					deep++;
			}
			switch (MyUser.UserJob)
			{
				case Job.Designer:
					if (deep == 2)
						return true;
					break;
				case Job.Modeler:
					if (deep == 0)
						return true;
					break;
				case Job.UnKnow:
					break;
				default:
					break;
			}

			return false;
		}


		public string GetProjcetPath() 
		{
			string name = ProjectName;
			if (string.IsNullOrEmpty(name))
				return null;


			return string.Format("{0}/{1}", ParentPath, ProjectName);
		}


		public string[] GetPathsNeedCreated(string projectPath) 
		{
			if (CreateThesePath != null)
			{
				string _o_path = CreateThesePath.Replace(Util.PROJECT_PATH_PLACEHOLDER, projectPath);
				return _o_path.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			}

			return new string[0];
		}

		public string GetImportPackagePath() 
		{
			return string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, PackagePath.TrimStart(new char[]{'/','\\'}));
		}

		public void RefreshPorjectInfo()
		{
			CreateCaseData.Current.CreateThesePath = "";

			string url = string.Empty;
			switch (MyUser.UserJob)
			{
				case Job.Designer:
					url = XMLManage.GetString(Util.DESIGNURL);
					CreateCaseData.Current.ProjectStyle = Constants.Design;
					CreateCaseData.Current.CreateThesePath += string.Format(Util.PROJECT_PATH_PLACEHOLDER + "/Maps;");
					CreateCaseData.Current.CreateThesePath += string.Format(Util.PROJECT_PATH_PLACEHOLDER + "/Reference;");
					CreateCaseData.Current.PackagePath = Util.Design_Package_Path;
					break;
				case Job.Modeler:
					url = XMLManage.GetString(Util.PRODUCTURL);
					CreateCaseData.Current.ProjectStyle = Constants.Modelling;
					CreateCaseData.Current.CreateThesePath += string.Format(Util.PROJECT_PATH_PLACEHOLDER + "/reference;");
					CreateCaseData.Current.PackagePath = Util.Model_Package_Path;
					break;
				case Job.UnKnow:
					break;
				default:
					break;
			}


            string subPath = CreateCaseData.Current.ParentPath.Replace(TreeHelper.RectifyPath(InputData.Current.Path), "");
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
