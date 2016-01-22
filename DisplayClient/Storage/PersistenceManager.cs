using DisplayClient.Log;
using Remote_Content_Show_Container;
using Remote_Content_Show_Protocol;
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

        public const string SavedJobConfigurationFilename = "job.config";

        public static string GetWriteablePath()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            return storageFolder.Path;
        }

        public static void SaveJobConfiguration(Job_Configuration configuration)
        {
            RCS_Job jobMessage = new RCS_Job(configuration);
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

        public static BitmapImage GetConfigurationImage()
        {
            string path = Path.Combine(GetWriteablePath(), SavedConfigurationImageFilename);

            if (File.Exists(path))
            {
                return new BitmapImage(new Uri(Path.Combine(GetWriteablePath(), SavedConfigurationImageFilename)));
            }
            
            // no image

            return null;
        }
    }
}
