using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Plugins.Configuration;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Configuration;

namespace MBFavorites
{
    public class PluginOptions : PluginConfigurationOptions
    {
        [Label("Menu Name:")]
        public string MenuName = "Favorites";
        [Label("Location:")]
        public string FavoritesRoot = ApplicationPaths.AppPluginPath;
    }
}
