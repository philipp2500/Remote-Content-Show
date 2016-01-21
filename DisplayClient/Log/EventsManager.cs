using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DisplayClient.Log
{
    public static class EventsManager
    {
        public const string LoggedEventsFileName = "events.xml";

        public static void Log(Job_EventType logType, Job_Configuration concernedJob, string description)
        {
            List<LoggedEvent> loggedEvents = new List<LoggedEvent>();

            LoggedEvent newloggedEvent = new LoggedEvent() { Type = logType, Description = description };
            newloggedEvent.Time = DateTime.Now;
            
            if (concernedJob != null)
            {
                LoggedJob newLoggedJob = new LoggedJob() { JobID = concernedJob.JobID };
                newLoggedJob.Name = concernedJob.Name;
            }

            loggedEvents.Add(newloggedEvent);

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            FileStream fs = new FileStream(Path.Combine(storageFolder.Path, LoggedEventsFileName), FileMode.Create, FileAccess.Write);

            DataContractSerializer dcs = new DataContractSerializer(typeof(List<LoggedEvent>));

            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8))
            {
                writer.WriteStartDocument();
                dcs.WriteObject(writer, loggedEvents);
            }            
        }

        public static List<LoggedEvent> GetLoggedEvents()
        {
            return GetLoggedEvents(DateTime.MinValue, DateTime.MaxValue);
        }

        public static List<LoggedEvent> GetLoggedEvents(DateTime from, DateTime to)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            FileStream fs = new FileStream(Path.Combine(storageFolder.Path, LoggedEventsFileName), FileMode.Open, FileAccess.Read);

            DataContractSerializer dcs = new DataContractSerializer(typeof(List<LoggedEvent>));
            List<LoggedEvent> events;

            using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
            {
                events = (List<LoggedEvent>)dcs.ReadObject(reader);
            }

            return events.Where(x => from >= x.Time && to <= x.Time).ToList();
        }
    }
}
