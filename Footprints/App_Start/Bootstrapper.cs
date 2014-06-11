using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Neo4jClient;
using Footprints.DAL.Abstract;
using Footprints.DAL.Infrastructure;
using Footprints.DAL.Concrete;

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
            builder.RegisterType<ICommentRepository>().As<Comments>();
            builder.RegisterType<IJourneyRepository>().As<Journeys>();
            builder.RegisterType<IDestinationRepository>().As<IDestinationRepository>();
            builder.RegisterType<IUserRepository>().As<Users>();
            builder.RegisterType<IDbContext>().As<DbContext>();
            var container = builder.Build();
            return container;
        }
    }
}