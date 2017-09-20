using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using QuickDemo.Common.Log;

namespace QuickDemoWeb.Filters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CertificatesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var cert = context.Request.GetClientCertificate();
            if (cert != null)
            {
                AzureLog.Info("Certificate found:" + cert.FriendlyName);
                AzureLog.Info(cert.ToString());
            }
            else
            {
                AzureLog.Info("Certificate cannot be found.");
            }
        }
    }
}
