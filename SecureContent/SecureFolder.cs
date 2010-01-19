using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Entities;

namespace SecureContent
{
    public class SecureFolder : Folder
    {
        public override string Name
        {
            get
            {
                return "Secure";
            }
            set
            {
            }
        }

        private static SecureFolder _instance = new SecureFolder();
        public static SecureFolder Instance
        {
            get
            {
                return _instance;
            }
        }


    }
}
