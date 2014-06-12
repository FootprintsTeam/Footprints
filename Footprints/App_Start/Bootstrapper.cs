using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Neo4jClient;
using Footprints.DAL.Abstract;
using Footprints.DAL.Infrastructure;
using Footprints.DAL.Concrete;
using System.Web.Mvc;

namespace Footprints.App_Start
{
    public class Bootstrapper
    {
        public static IContainer SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.Register<IGraphClient>(context =>
            {
                var graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"));
                graphClient.Connect();
                return graphClient;
            }).SingleInstance();
            builder.RegisterType<Comments>().As<ICommentRepository>();
            builder.RegisterType<Journeys>().As<IJourneyRepository>();
            builder.RegisterType<Destinations>().As<IDestinationRepository>();
            builder.RegisterType<Users>().As<IUserRepository>();
            builder.RegisterType<DbContext>().As<IDbContext>();
            var container = builder.Build();
            return container;
        }
    }
}