using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BlueYonder.Companion.Controllers;
using System.Web.Http.Routing;
using System.Net.Http;
using BlueYonder.Companion.Controllers.Formatters;

namespace BlueYonder.Companion.Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // TODO: Module 4: Exercise 1: Task 3.1: Register the BlueYonder Resolver
            config.DependencyResolver = new BlueYonderResolver();

            config.Formatters.Add(new AtomFormatter()); 
            config.MessageHandlers.Add(new AtomHandler());
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "atom",
                routeTemplate: "atom/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}