using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Neo4jClient;

namespace Footprints.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();
        }

        public static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register<IGraphClient>(context =>
            {
                var graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"));
                graphClient.Connect();
                return graphClient;
            }).SingleInstance();
        }
    }
}