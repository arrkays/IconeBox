using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IconeBox
{
    class Ico
    {
        String name;
        ListViewItem item;//objet afficher 
        String target;
        private SystemIconsImageList sysIcons = new SystemIconsImageList();


        public Ico(FileInfo file)
        {
            this.name = file.Name;
            this.target = R.GetShortcutTargetFile(file.FullName);

            //création itème
            this.item = new ListViewItem(name);
            this.item.ImageIndex = sysIcons.GetIconIndex(target);
        }

        public Ico()
        {

        }
    }


}
