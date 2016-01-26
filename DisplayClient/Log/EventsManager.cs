using DisplayClient.Storage;
using Remote_Content_Show_Container;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DisplayClient.Log
{
    public static class EventsManager
    {
        public const string LoggedEventsFilename = "events.xml";

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

        public async static void Log(Job_EventType logType, Job_Configuration concernedJob, string description)
        {
            List<LoggedEvent> loggedEvents = await GetLoggedEvents();

            LoggedEvent newloggedEvent = new LoggedEvent() { Type = logType, Description = description };
            newloggedEvent.Time = DateTime.Now;
            
            if (concernedJob != null)
            {
                LoggedJob newLoggedJob = new LoggedJob() { JobID = concernedJob.JobID };
                newLoggedJob.Name = concernedJob.Name;

                newloggedEvent.ConcernedJob = newLoggedJob;
            }
            else
            {
                LoggedJob emptyJob = new LoggedJob() { JobID = new Guid(), Name = "-" };

                newloggedEvent.ConcernedJob = emptyJob;
            }

            loggedEvents.Add(newloggedEvent);

            await semaphore.WaitAsync();

            try
            {
                // begin

                using (FileStream fs = new FileStream(Path.Combine(PersistenceManager.GetWriteablePath(), LoggedEventsFilename), FileMode.Create, FileAccess.Write))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<LoggedEvent>));

                    using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8))
                    {
                        writer.WriteStartDocument();
                        dcs.WriteObject(writer, loggedEvents);
                    }
                }

                // end
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async static void ClearLog()
        {
            List<LoggedEvent> loggedEvents = new List<LoggedEvent>();

            await semaphore.WaitAsync();

            try
            {
                // begin

                using (FileStream fs = new FileStream(Path.Combine(PersistenceManager.GetWriteablePath(), LoggedEventsFilename), FileMode.Create, FileAccess.Write))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<LoggedEvent>));

                    using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8))
                    {
                        writer.WriteStartDocument();
                        dcs.WriteObject(writer, loggedEvents);
                    }
                }

                // end
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async static Task<List<LoggedEvent>> GetLoggedEvents()
        {
            return await GetLoggedEvents(DateTime.MinValue, DateTime.MaxValue);
        }

        public async static Task<List<LoggedEvent>> GetLoggedEvents(DateTime from, DateTime to)
        {
            await semaphore.WaitAsync();

            try
            {
                // begin

                using (FileStream fs = new FileStream(Path.Combine(PersistenceManager.GetWriteablePath(), LoggedEventsFilename), FileMode.Open, FileAccess.Read))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<LoggedEvent>));
                    List<LoggedEvent> events;

                    using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                    {
                        events = (List<LoggedEvent>)dcs.ReadObject(reader);
                    }

                    return events.Where(x => from <= x.Time && to >= x.Time).ToList();
                }

                // end
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
