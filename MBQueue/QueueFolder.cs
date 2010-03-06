using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library;
using MediaBrowser;
using MediaBrowser.Library.Logging;

namespace MBQueue
{
    class QueueFolder : Folder
    {
        private FolderModel folderModel;

        public override string Name
        {
            get
            {
                return "Queue";
            }
            set
            {
                base.Name = value;
            }
        }

        public override bool SelectAction(MediaBrowser.Library.Item item)
        {
            //Logger.ReportInfo("In SelectAction");
            if (folderModel == null) folderModel = ItemFactory.Instance.Create(this) as FolderModel;
            Dictionary<string,object> parms = new Dictionary<string,object>();
            parms.Add("Application", Application.CurrentInstance);
            parms.Add("Folder", folderModel);
            Application.CurrentInstance.OpenMCMLPage("resx://MBQueue/MBQueue.Resources/QueuePage",parms);
            return true;
        }
    }
}
