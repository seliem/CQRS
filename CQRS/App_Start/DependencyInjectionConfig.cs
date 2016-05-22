﻿using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;
using CQRS.Controllers;
using CQRS.Infrastructure.DependencyInjection;
using CQRS.Infrastructure.DependencyInjection.Interfaces;
using Owin;
using Registration = CQRS.DependencyInjection.Registration;

namespace CQRS.App_Start
{
    public class DependencyInjectionConfig
    {
        public static void Register(IAppBuilder appBuilder)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var config = new HttpConfiguration();

            containerBuilder.RegisterWebApiFilterProvider(config);

            containerBuilder.Register(c => HttpContext.Current).As<HttpContext>().InstancePerRequest();
            containerBuilder.Register(c => HttpContext.Current.Request).As<HttpRequest>().InstancePerRequest();

            containerBuilder.RegisterType<TestInject>().As<ITestInject>();

            Registration.Register(containerBuilder);

            var container = containerBuilder.Build();

            var customDependencyResolverContainerBuilder = new ContainerBuilder();

            customDependencyResolverContainerBuilder.RegisterType<CustomDependencyResolver>()
                .As<ICustomDependencyResolver>();

            customDependencyResolverContainerBuilder.Update(container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
        }
    }
}