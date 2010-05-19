using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library;
using MediaBrowser.Library.Plugins.Configuration;
using MediaBrowser.Library.Configuration;

namespace MBSettingSaver
{
    public class PluginOptions : PluginConfigurationOptions
    {
        [Label("Save Path:")]
        public string SavePath = System.IO.Path.Combine(ApplicationPaths.AppPluginPath, "MBSettingSaver");

    }
}