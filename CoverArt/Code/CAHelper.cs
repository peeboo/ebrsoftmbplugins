using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using QuadrilateralDistortion;
using MediaBrowser.Library.Filesystem;

namespace CoverArt
{
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
            if (Path.HasExtension(path))
                return false;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            foreach (System.IO.FileInfo fileInfo in directoryInfo.GetFiles())
                if (IsMusic(fileInfo.FullName))
                    return true;

            return false;
        }

        public static bool IsArtistAlbumFolder(string path)
        {
            if (Path.HasExtension(path))
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
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
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPie(b, r.X + r.Width - d - 1, r.Y, d + 1, d + 1, 270, 90);
            g.FillPie(b, r.X + r.Width - d - 1, r.Y + r.Height - d, d + 1, d + 1, 0, 90);

            g.FillRectangle(b, r.X, r.Y, r.Width - d / 2 + 1, d / 2);
            g.FillRectangle(b, r.X, (r.Y + d / 2) - 1, r.Width, r.Height - d + 2);
            g.FillRectangle(b, r.X, r.Y + r.Height - d / 2, r.Width - d / 2 + 1, d / 2);
        }

        public static Bitmap Squeeze3D(Bitmap image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            Point tl = new Point(0, 0);
            Point tr = new Point(image.Width, Convert.ToInt32(image.Height * (.055)));
            Point bl = new Point(0, image.Height);
            Point br = new Point(image.Width, Convert.ToInt32(image.Height * (.945)));
            Bitmap Perspective = QuadDistort.Distort(image, tl, tr, bl, br);

            return Perspective;
        }
    }
}
