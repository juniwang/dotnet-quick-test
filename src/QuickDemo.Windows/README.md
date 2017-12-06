## EventSource

Reference to [Microsoft EventSource](https://github.com/Microsoft/dotnet-samples/blob/master/Microsoft.Diagnostics.Tracing/EventSource/docs/EventSource.md) for details about EventSource.

#### turn on ETW
By default, ETW events are NOT on, they need to be turned on by an ETW 'controller'. The easiest way to turn on ETW EventSources is 
with the PerfView too. See: [Tracing event source](https://blogs.msdn.microsoft.com/vancem/2012/08/13/windows-high-speed-logging-etw-in-c-net-using-system-diagnostics-tracing-eventsource/)

#### view
[Download PerfView](https://www.microsoft.com/en-us/download/details.aspx?id=28567) and issue:
```
PerfView /OnlyProviders=*QuickDemoEventSource collect
```

#### references
 - [Cloud service perf counter](https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-dotnet-diagnostics-performance-counters)
 - Azure Storage:`<Setting name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" value="UseDevelopmentStorage=true" />`. [More](https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-dotnet-diagnostics-storage)
 - [Azure Cloud service integration](https://docs.microsoft.com/en-us/azure/cloud-services/cloud-services-dotnet-diagnostics)
 - [Tools for Cloud Service](https://docs.microsoft.com/en-us/azure/vs-azure-tools-diagnostics-for-cloud-services-and-virtual-machines)
 - [Azure Service Fabric](https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-diagnostics-how-to-monitor-and-diagnose-services-locally)
             
