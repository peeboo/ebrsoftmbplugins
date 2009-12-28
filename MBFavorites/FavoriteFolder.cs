using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using MediaBrowser.Interop;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library;
using MediaBrowser;

namespace MBFavorites
{
    class FavoriteFolder : MediaBrowser.Library.Entities.Folder
    {
        private static List<BaseItem> items = new List<BaseItem>();

        public override string Name
        {
            get
            {
                return "Favorites";
            }
            set
            {
            }
        }

        //private WshShellClass wShell = new WshShellClass();

        //protected override List<BaseItem> ActualChildren
        //{
        //    get
        //    {
        //        return items;
        //    }
        //}

        //protected override List<BaseItem> GetNonCachedChildren()
        //{
        //    return items; //we don't have any actual physical children
        //}

        //public override void ValidateChildren()
        //{
        //    //do nothing
        //}

        public void AddChild(Item item)
        {
            ShellLink shortcut = new ShellLink();
            string name = new string (item.Name
                .ToCharArray()
                .Where(e => !System.IO.Path.GetInvalidFileNameChars().Contains(e))
                .ToArray());
            shortcut.ShortCutFile = System.IO.Path.Combine(this.Path, name + ".lnk");
            shortcut.Target = item.Path;
            shortcut.Save();
            shortcut.Dispose();
            this.ValidateChildren();
            
        }

        public void RemoveChild(Item item)
        {
            string name = new string(item.Name
                .ToCharArray()
                .Where(e => !System.IO.Path.GetInvalidFileNameChars().Contains(e))
                .ToArray());
            string path = System.IO.Path.Combine(this.Path, name + ".lnk");
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Logger.ReportException("Favorites - Error removing favorite", ex);
            }
            this.ValidateChildren();
        }

        public void Clear()
        {
            //remove all our favorites
            try
            {
                foreach (string file in Directory.GetFiles(this.Path))
                {
                    string fn = System.IO.Path.Combine(this.Path, file);
                    if (System.IO.Path.GetExtension(fn) == ".lnk")
                        File.Delete(fn);
                }
                this.ValidateChildren();
                Application.CurrentInstance.RootFolderModel.RefreshUI();
            }
            catch (Exception ex)
            {
                Logger.ReportException("Favorites - Error Clearing Favorites", ex);
            }
        }


    }
}
