using DisplayClient.Log;
using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace DisplayClient.Storage
{
    public static class PersistenceManager
    {
        public const string SavedConfigurationImageFilename = "configImage.jpg";

        public const string SavedJobConfigurationFilename = "job.config";

        public const string SavedCustomFilesDirectoryName = "Files";

        public static string GetWriteablePath()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            return storageFolder.Path;
        }

        public async static Task<string> GetAssetsPath()
        {
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
            var files = await assets.GetFilesAsync();

            return assets.Path;
        }

        public static void SaveJobConfiguration(Job_Configuration configuration)
        {
            RCS_Job jobMessage = new RCS_Job(configuration, RemoteType.Client);
            byte[] bytes = Remote_Content_Show_MessageGenerator.GetMessageAsByte(jobMessage);

            try
            {
                File.WriteAllBytes(Path.Combine(GetWriteablePath(), SavedJobConfigurationFilename), bytes);
            }
            catch (Exception)
            {
                EventsManager.Log(Job_EventType.Error, configuration, "The job configuration could not be saved.");
            }
        }

        public static Job_Configuration GetJobConfiguration()
        {
            string path = Path.Combine(GetWriteablePath(), SavedJobConfigurationFilename);

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                RCS_Job jobMessage = Remote_Content_Show_MessageGenerator.GetMessageFromByte<RCS_Job>(bytes);

                return jobMessage.Configuration;
            }

            // no job

            return null;
        }

        public static void SaveConfigurationImage(byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(Path.Combine(GetWriteablePath(), SavedConfigurationImageFilename), bytes);
            }
            catch (Exception)
            {
                EventsManager.Log(Job_EventType.Error, null, "The configuration image could not be saved.");
            }
        }

        /// <summary>
        /// This method must run on the UI thread!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <returns></returns>
        public async static Task<BitmapImage> GetConfigurationImage()
        {
            string a = GetWriteablePath();
            string b = SavedConfigurationImageFilename;

            string path = Path.Combine(a, b);

            if (File.Exists(path))
            {
                BitmapImage image = new BitmapImage();

                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
                {
                    await image.SetSourceAsync(ms.AsRandomAccessStream());
                }

                return image;

                //return new BitmapImage(new Uri(path));
            }
            
            // no image

            return null;
        }

        public static void SaveFile(byte[] bytes, string filename)
        {
            try
            {
                File.WriteAllBytes(Path.Combine(GetWriteablePath(), SavedCustomFilesDirectoryName, filename), bytes);
            }
            catch (Exception)
            {
                EventsManager.Log(Job_EventType.Error, null, "The file " + filename + " could not be written on the disk.");
            }
        }

        public static byte[] GetFile(string filename)
        {
            string path = Path.Combine(GetWriteablePath(), SavedCustomFilesDirectoryName, filename);

            if (File.Exists(path))
            {
                return File.ReadAllBytes(Path.Combine(GetWriteablePath(), filename));
            }

            // no image

            return null;
        }

        public static void DeleteFile(string filename)
        {
            string path = Path.Combine(GetWriteablePath(), SavedCustomFilesDirectoryName, filename);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static FileInfo[] GetLocalFilesList()
        {
            List<FileItem> items = new List<FileItem>();
            string path = Path.Combine(GetWriteablePath(), SavedCustomFilesDirectoryName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            DirectoryInfo info = new DirectoryInfo(path);

            return info.GetFiles();
        }
    }
}
