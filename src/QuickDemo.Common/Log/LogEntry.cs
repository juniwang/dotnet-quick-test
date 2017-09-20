﻿using log4net.Appender;
using log4net.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common.Log
{
    class LogEntry : TableEntity
    {
        public LogEntry(LoggingEvent e, PartitionKeyTypeEnum partitionKeyType)
        {
            Domain = e.Domain;
            Identity = e.Identity;
            Level = e.Level.ToString();
            var sb = new StringBuilder(e.Properties.Count);
            foreach (DictionaryEntry entry in e.Properties)
            {
                sb.AppendFormat("{0}:{1}", entry.Key, entry.Value);
                sb.AppendLine();
            }
            Properties = sb.ToString();
            Message = e.RenderedMessage + Environment.NewLine + e.GetExceptionString();
            ThreadName = e.ThreadName;
            EventTimeStamp = e.TimeStamp;
            UserName = e.UserName;
            Location = e.LocationInformation.FullInfo;
            ClassName = e.LocationInformation.ClassName;
            FileName = e.LocationInformation.FileName;
            LineNumber = e.LocationInformation.LineNumber;
            MethodName = e.LocationInformation.MethodName;
            StackFrames = e.LocationInformation.StackFrames;

            if (e.ExceptionObject != null)
            {
                Exception = e.ExceptionObject.ToString();
            }

            PartitionKey = e.MakePartitionKey(partitionKeyType);
            RowKey = e.MakeRowKey();

        }
        public string UserName { get; set; }
        public DateTime EventTimeStamp { get; set; }
        public string ThreadName { get; set; }
        public string Message { get; set; }
        public string Properties { get; set; }
        public string Level { get; set; }
        public string Identity { get; set; }
        public string Domain { get; set; }
        public string Location { get; set; }
        public string Exception { get; set; }
        public string ClassName { get; set; }
        public string FileName { get; set; }
        public string LineNumber { get; set; }
        public string MethodName { get; set; }
        public StackFrameItem[] StackFrames { get; set; }
    }
}
