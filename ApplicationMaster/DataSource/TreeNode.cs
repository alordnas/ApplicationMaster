using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Casamia.Core;


namespace Casamia.Model
{
    public class TreeNode : INotifyPropertyChanged
    {
		private static TreeNode _root;
		public static TreeNode Root 
		{
			get 
			{
				return _root;
			}
			set 
			{
				_root = value;

			}
		}


		public static TreeNode _svnRoot;

		public static TreeNode SvnRoot 
		{
			get 
			{
				return _svnRoot;
			}
			set 
			{

				_svnRoot = value;

			}
		}

		public static TreeNode LastCheckedNode = null;

		private TreeNode _parent;
		public TreeNode parent 
		{
			get 
			{
				return _parent;
			}
			set 
			{
				_parent = value;

				if (_parent != null)
				{
					deep = _parent.deep + 1;

				}
			}
		}

		public bool IsSvnNode = false;

		//private SvnWatcher svnWatcher = null;

		private ImageSource _icon = new BitmapImage(new Uri("Images/folder.jpg", UriKind.Relative));

		public ImageSource icon 
		{
			get 
			{
				return _icon;
			}
			set 
			{
				_icon = value;

				NotifyPropertyChanged("icon");	
			}
		}

		private int deep;

        private bool _isChecked = false;
		public bool isChecked 
        {
            get 
            {
				

                return _isChecked;
            }
            set 
            {
                _isChecked = value;

                NotifyPropertyChanged("isChecked");
            }
        }

		public bool isRoot;

		public string fileName { get; set; }


		private string _filePath;
		public string filePath 
		{
			get 
			{
				return _filePath;
			}
			set 
			{
				_filePath = TreeHelper.RectifyPath(value);

				NotifyPropertyChanged("filePath");

				fileName = Path.GetFileName(_filePath);

				NotifyPropertyChanged("fileName");

				if (!IsSvnNode)
				{
					childrenWatcher = new Watcher(this);

					if (isLeaf)
					{
						//svnWatcher = new SvnWatcher(this);
					}
				}

                
			} 
		}

		private Watcher childrenWatcher;


		public bool isDeepLimited
		{
			get 
			{
				return Util.TREE_DEEP_LITIM <= deep;
			}
		}

		public bool isLeaf
		{
			get
			{
				return children.Count == 0;
			}
		}

		public bool isProject
		{
			get
			{
				return IsProject(filePath);
			}

		}

		public static bool IsProject(string __filePath) 
		{
			return Directory.Exists(string.Format(string.Format("{0}/Assets", __filePath == null ? string.Empty : __filePath))) &&

				Directory.Exists(string.Format(string.Format("{0}/ProjectSettings", __filePath == null ? string.Empty : __filePath)));
		}

		public ObservableCollection<TreeNode> children { get; set; }


        public TreeNode(TreeNode _parent) 
        {

			parent = _parent;

			children = new ObservableCollection<TreeNode>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

       
        public override string ToString()
        {
            return filePath;
        }

        public override bool Equals(object obj)
        {
            if (obj is TreeNode)
            {
                TreeNode node = obj as TreeNode;

                return filePath.Equals(node.filePath);
            }

            return false;
        }

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

    }
}
