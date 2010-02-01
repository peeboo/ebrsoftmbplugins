using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;

namespace CoverArt
{
    //******************************************************************************************************************
    //  This class is used to house your configuration parameters.  It is a decendant of ModelItem and will be passed 
    //  into your config panel at runtime (see configpanel setup in plugin.cs) so that you can bind to its properties.
    //******************************************************************************************************************

    public class MyConfig : ModelItem
    {
        public MyConfig() { }

        private bool testOption;

        public bool CoverArtTestOption { get { return testOption; } set { if (testOption != value) { testOption = value; FirePropertyChanged("CoverArtTestOption"); } } }

    }
}
