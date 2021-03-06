﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Management;
using MediaBrowser.Library.Filesystem;

namespace CoverArt
{
    public struct SkewRatios
    {
        public double TLx, TLy, TRx, TRy, BLx, BLy, BRx, BRy;

        public SkewRatios(double tlx, double tly, double trx, double tryy, double blx, double bly, double brx, double bry)
        {
            TLx = tlx;
            TLy = tly;
            TRx = trx;
            TRy = tryy;
            BLx = blx;
            BLy = bly;
            BRx = brx;
            BRy = bry;
        }
    }

    public static class CAHelper
    {
        public static bool IsArtistFolder(this IMediaLocation location)
        {
            IFolderMediaLocation folder = location as IFolderMediaLocation;
            if (folder != null)
            {
                if (Path.HasExtension(folder.Path))
                    return false;

                if (IsAlbumFolder(folder.Path))
                    return false;

                DirectoryInfo directoryInfo = new DirectoryInfo(folder.Path);
                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                    if (IsAlbumFolder(directory.FullName))
                        return true;

                return false;


            }
            return false;
        }

        public static bool IsPlaylistFolder(string folder)
        {
            return true;
        }

        public static bool IsMusic(string filename)
        {
            string extension = System.IO.Path.GetExtension(filename).ToLower();

            switch (extension)
            {
                case ".mp3":
                case ".m3u":
                case ".wma":
                case ".acc":
                case ".flac":
                case ".m4a":
                case ".wpl":
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsAlbumFolder(string path)
        {
            if (!Directory.Exists(path))
                return false;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles())
                if (IsMusic(fileInfo.FullName))
                    return true;

            return false;
        }

        public static bool IsArtistAlbumFolder(string path)
        {
            if (!Directory.Exists(path))
                return false;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles())
                if (IsMusic(fileInfo.FullName))
                    return true;

            return false;
        }

        public static Bitmap RoundCorners(Bitmap image, Double cofactor, Int32 corners)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            Graphics g = Graphics.FromImage(bitmap);

            g.Clear(Color.Transparent);
            if (corners == 1)
                FillRoundedRectangle(g, new Rectangle(-1, 0, bitmap.Width, bitmap.Height), (Convert.ToInt32(Convert.ToInt32(bitmap.Height / 10 * cofactor)) / 2) * 2, new TextureBrush(image));
            if (corners == 2)
                FillRightRoundedRectangle(g, new Rectangle(-1, 0, bitmap.Width, bitmap.Height), (Convert.ToInt32(Convert.ToInt32(bitmap.Height / 10 * cofactor)) / 2) * 2, new TextureBrush(image));
            g.Dispose();
            return bitmap;
        }

        public static void FillRoundedRectangle(Graphics g, Rectangle r, int d, Brush b)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillPie(b, r.X, r.Y, d + 1, d + 1, 180, 90);
            g.FillPie(b, r.X + r.Width - d - 1, r.Y, d + 1, d + 1, 270, 90);
            g.FillPie(b, r.X, r.Y + r.Height - d, d + 1, d + 1, 90, 90);
            g.FillPie(b, r.X + r.Width - d - 1, r.Y + r.Height - d, d + 1, d + 1, 0, 90);

            g.FillRectangle(b, r.X + d / 2, r.Y, r.Width - d + 1, d / 2);
            g.FillRectangle(b, r.X, (r.Y + d / 2) - 1, r.Width, r.Height - d + 2);
            g.FillRectangle(b, r.X + d / 2, r.Y + r.Height - d / 2, r.Width - d + 1, d / 2);
        }

        public static void FillRightRoundedRectangle(Graphics g, Rectangle r, int d, Brush b)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillPie(b, r.X + r.Width - d - 1, r.Y, d + 1, d + 1, 270, 90);
            g.FillPie(b, r.X + r.Width - d - 1, r.Y + r.Height - d, d + 1, d + 1, 0, 90);

            g.FillRectangle(b, r.X, r.Y, r.Width - d / 2 + 1, d / 2);
            g.FillRectangle(b, r.X, (r.Y + d / 2) - 1, r.Width, r.Height - d + 2);
            g.FillRectangle(b, r.X, r.Y + r.Height - d / 2, r.Width - d / 2 + 1, d / 2);
        }


        /// <summary>
        /// Returns MAC Address from first Network Card in Computer
        /// </summary>
        /// <returns>[string] MAC Address</returns>
        public static string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)  // only return MAC Address from first card
                {
                    try
                    {
                        if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                    }
                    catch
                    {
                        mo.Dispose();
                        return "";
                    }
                }
                mo.Dispose();
            }
            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

    }

}
