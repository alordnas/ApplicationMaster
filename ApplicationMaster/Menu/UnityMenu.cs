using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casamia.Menu
{
    public class UnityMenu
    {
        public static void CreateUnityProject(string parentPath) 
        {
            if (CreateProjectWindow.Current == null)
            {
                CreateProjectWindow.Current = new CreateProjectWindow(parentPath);

				CreateProjectWindow.Current.ShowDialog();
            }
           
        }

    }
}
