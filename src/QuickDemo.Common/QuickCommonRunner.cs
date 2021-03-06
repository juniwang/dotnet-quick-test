﻿using Autofac;
using QuickDemo.Common.Certificate;
using QuickDemo.Common.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuickDemo.Common
{
    public class QuickCommonRunner
    {
        private static QuickCommonRunner runner = new QuickCommonRunner();
        private QuickCommonRunner() { }
        public static QuickCommonRunner Instance
        {
            get
            {
                return runner;
            }
        }

        public void ObjectSort()
        {
            Sort.TrySort();
        }

        public void AutofacAdapter()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(new ForAutofac());
            builder.RegisterAdapter<ForAutofac, IForAutofac>(f => f);

            IContainer container = builder.Build();
            var bc = container.Resolve<IForAutofac>();
            Console.WriteLine(bc?.Name);
        }

        public void AutofacResolveTest()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<ForAutofac>().As<IForAutofac>();

            IContainer container = builder.Build();
            var ins1 = container.Resolve<IForAutofac>();
            var ins2 = container.Resolve<IForAutofac>();
            Console.WriteLine(ins1 == ins2);
        }

        public void GetWithCertificate(string uri, string thumbprint)
        {
            var certificate = CertificateHelper.FindCertificateByThumbprint(thumbprint);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.PreAuthenticate = true;
            req.AllowAutoRedirect = true;
            req.ClientCertificates.Add(certificate);
            req.Method = "GET";

            WebResponse resp = req.GetResponse();
            using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    line = reader.ReadLine();
                }
                Console.WriteLine(line);
            }
        }
    }
}
