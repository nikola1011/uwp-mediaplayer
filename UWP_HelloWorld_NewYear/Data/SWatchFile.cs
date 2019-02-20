using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP_HelloWorld_NewYear.Data
{
    public class SWatchFile 
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public ulong Size { get; set; }
        public StorageItemThumbnail Thumbnail { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public FileType Type { get; set; }
        public BitmapImage ThumbnailBitmapImage { get; set; }
        public StorageFile StorageFile { get; set; }

        // TODO: Delete when LayoutPage.xaml is not longer needed
        //public ImageSource SwatchImage = new BitmapImage(new Uri("ms-appx:///Assets/music_icon.png"));

        public static FileType GetFileTypeFromFileContentType(string fileContentType)
        {
            string fileContentTypePrefix = fileContentType.Split('/')?[0];
            switch (fileContentTypePrefix)
            {
                case "video":
                    {
                        return FileType.Video;
                    }
                case "audio":
                    {
                        return FileType.Audio;
                    }
                case "image":
                    {
                        return FileType.Image;
                    }
                default:
                    return FileType.Other;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().Equals(this.GetType()))
                return false;

            SWatchFile file = (SWatchFile)obj;

            return Name == file.Name && Path == file.Path && Type == file.Type; // Note: Thumbnail.Equals(file.Thumbnail) return false (that's why it's excluded from check)
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }

    public enum FileType
    {
        Video = 1,
        Audio = 2,
        Image = 4,
        Other = 8
    }
}
