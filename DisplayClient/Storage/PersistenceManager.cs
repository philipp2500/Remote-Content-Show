using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace DisplayClient.Storage
{
    public static class PersistenceManager
    {
        public const string SavedConfigurationImageFilename = "configImage.jpg";

        public static string GetWriteablePath()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            return storageFolder.Path;
        }

        public static void SaveConfigurationImage(byte[] bytes)
        {
            File.WriteAllBytes(Path.Combine(GetWriteablePath(), PersistenceManager.GetWriteablePath()), bytes);
        }

        public static BitmapImage GetConfigurationImage()
        {
            return new BitmapImage(new Uri(Path.Combine(GetWriteablePath(), SavedConfigurationImageFilename)));
        }
    }
}
