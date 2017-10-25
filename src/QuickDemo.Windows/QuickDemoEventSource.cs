using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Windows
{
    /// <summary>
    /// Quick Demo how to use EventSource.
    /// 
    /// Reference to https://github.com/Microsoft/dotnet-samples/blob/master/Microsoft.Diagnostics.Tracing/EventSource/docs/EventSource.md for details about EventSource
    /// </summary>
    [EventSource(Name = "QuickDemoEventSource")]
    internal sealed class QuickDemoEventSource : EventSource
    {
        public static readonly QuickDemoEventSource Current = new QuickDemoEventSource();
        private QuickDemoEventSource() : base()
        {
            /*
             * Important !!!
             * By default, ETW events are NOT on, they need to be turned on by an ETW 'controller'.   The easiest way to turn on ETW EventSources is with the PerfView too.
             * see: https://blogs.msdn.microsoft.com/vancem/2012/08/13/windows-high-speed-logging-etw-in-c-net-using-system-diagnostics-tracing-eventsource/
             * 
             * Local:
             * user>: PerfView /OnlyProviders=*QuickDemoEventSource collect
             * Download page: https://www.microsoft.com/en-us/download/details.aspx?id=28567
             * 
             * Performance counter configuration:
             * https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-dotnet-diagnostics-performance-counters
             * 
             * To store trace in azure storage:
             * <Setting name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" value="UseDevelopmentStorage=true" />
             * see more: https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-dotnet-diagnostics-storage
             * 
             * Azure cloud service integration:
             * https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-dotnet-diagnostics
             * https://docs.microsoft.com/en-us/azure/vs-azure-tools-diagnostics-for-cloud-services-and-virtual-machines
             * 
             * Azure service fabric:
             * https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-diagnostics-how-to-monitor-and-diagnose-services-locally
             */
        }

        #region Keywords

        // Event keywords can be used to categorize events. 
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords Page = (EventKeywords)1;
            public const EventKeywords DataBase = (EventKeywords)2;
            public const EventKeywords Diagnostic = (EventKeywords)4;
            public const EventKeywords Perf = (EventKeywords)8;
        }

        public static class Tasks
        {
            public const EventTask Page = (EventTask)1;
            public const EventTask DBQuery = (EventTask)2;
        }

        #endregion

        #region Events

        // Define an instance method for each event you want to record and apply an [Event] attribute to it.
        // The method name is the name of the event.
        // Pass any parameters you want to record with the event (only primitive integer types, DateTime, Guid & string are allowed).
        // Each event method implementation should check whether the event source is enabled, and if it is, call WriteEvent() method to raise the event.
        // The number and types of arguments passed to every event method must exactly match what is passed to WriteEvent().
        // Put [NonEvent] attribute on all methods that do not define an event.
        // For more information see https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventsource.aspx
        [Event(1, Message = "Application Failure: {0}", Level = EventLevel.Error, Keywords = Keywords.Diagnostic)]
        public void Failure(string message) { WriteEvent(1, message); }

        [Event(2, Message = "Starting up.", Keywords = Keywords.Perf, Level = EventLevel.Informational)]
        public void Startup() { WriteEvent(2); }

        [Event(3, Message = "loading page {1} activityID={0}", Opcode = EventOpcode.Start,
            Task = Tasks.Page, Keywords = Keywords.Page, Level = EventLevel.Informational)]
        public void PageStart(int ID, string url)
        {
            if (IsEnabled())
                WriteEvent(3, ID, url);
        }

        [Event(4, Opcode = EventOpcode.Stop, Task = Tasks.Page, Keywords = Keywords.Page, Level = EventLevel.Informational)]
        public void PageStop(int ID) { if (IsEnabled()) WriteEvent(4, ID); }

        [Event(5, Opcode = EventOpcode.Start, Task = Tasks.DBQuery, Keywords = Keywords.DataBase, Level = EventLevel.Informational)]
        public void DBQueryStart(string sqlQuery) { WriteEvent(5, sqlQuery); }

        [Event(6, Opcode = EventOpcode.Stop, Task = Tasks.DBQuery, Keywords = Keywords.DataBase, Level = EventLevel.Informational)]
        public void DBQueryStop() { WriteEvent(6); }

        [Event(7, Level = EventLevel.Verbose, Keywords = Keywords.DataBase)]
        public void Mark(int ID) { if (IsEnabled()) WriteEvent(7, ID); }

        # endregion
    }
}
