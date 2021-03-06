﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.Mvc;
using Neo4jClient;
using Footprints.DAL.Abstract;
using Footprints.DAL.Infrastructure;
using Footprints.DAL.Concrete;
using System.Web.Mvc;
using Footprints.Services;
using System.Reflection;
using Footprints.Common;

namespace Footprints.App_Start
{
    public class Bootstrapper
    {        
        public static IContainer SetAutofacContainer()
        {    
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //register database connection
            builder.Register<IGraphClient>(context =>
            {
                var graphClient = new GraphClient(new Uri("http://localhost:7474/db/data"));
                try
                {
                    graphClient.Connect();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                return graphClient;
            }).SingleInstance();

            //register repository layer
            builder.RegisterType<CommentRepository>().As<ICommentRepository>();
            builder.RegisterType<JourneyRepository>().As<IJourneyRepository>();
            builder.RegisterType<DestinationRepository>().As<IDestinationRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<NewsFeedRepository>().As<INewsFeedRepository>();

            //register service layer
            builder.RegisterType<CommentService>().As<ICommentService>();
            builder.RegisterType<JourneyService>().As<IJourneyService>();
            builder.RegisterType<DestinationService>().As<IDestinationService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<NewsfeedService>().As<INewsfeedService>();
			builder.RegisterType<Search>().As<ISearch>();

            var container = builder.Build();
            return container;
        }
    }
}