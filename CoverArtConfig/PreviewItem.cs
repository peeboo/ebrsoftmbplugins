using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CoverArtConfig
{
    public class PreviewItem
    {

        private string preview;
        private string name;

        public string Preview {
            get { return preview;}
        }

        public string Name {
            get { return name; }
        }
        
        public PreviewItem(string p, string n)
        {
            preview = p;
            name = n;
        }


    }
}
