using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace ConsoleWithKatana
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void Main(string[] args)
        {
            const string url = "http://localhost:8090";

            // start the server
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server has started!");

                // keep the server running untill a key is pressed in the console
                Console.ReadKey();
                Console.WriteLine("Server is stopping!");
            }
        }
    }

    public class Startup
    {
        /// <summary>
        ///     Configure Katana by adding middleware into the OWIN pipeline.
        /// </summary>
        /// <param name="appBuilder">
        ///     Describes how the application is going to behave and response to Http requests.
        /// </param>
        public void Configuration(IAppBuilder appBuilder)
        {
            // Inserts a middleware into the OWIN pipeline
            //appBuilder.Use<HelloWorldComponent>();

            //by using syntactic sugar
            appBuilder.UseHelloWorld();

            return;

            appBuilder.UseWelcomePage();

            // This Run() method is a method Katana will call into to process all Http request.
            appBuilder.Run(context =>
            {
                // on this 'context' object I have access to info about the request,
                // so I could look at authentication properties,
                // the env properties - which contains all the info about the request:
                //                      Http headers, the cookies, the path, the verb,
                // and also the request object & response object.
                return context.Response.WriteAsync("Hellow with Katana!");
            });
        }
    }

    /// <summary>
    /// Provides syntactic sugar to install the 'HelloWorldComponent'
    /// </summary>
    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder appBuilder)
        {
            appBuilder.Use<HelloWorldComponent>();
        }
    }

    /// <summary>
    /// Define a Katana component.
    /// Not every componentn has to be a class. Instead, one could just call app.Run()
    /// </summary>
    public class HelloWorldComponent
    {
        // AppFunc is an alias for type  Func<IDictionary<string, object>, Task>
        private readonly AppFunc _nextComponentInPipeline;

        public HelloWorldComponent(AppFunc nextComponentInPipeline)
        {
            _nextComponentInPipeline = nextComponentInPipeline;
        }

        /// <summary>
        /// Entry point for Katana which is going to poke at it with reflection.
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public Task Invoke(IDictionary<string, object> environment)
        {
            // call next component in the pipeline
            //await _nextComponentInPipeline(environment);

            // or just return response and don't call any more components in the pipeline
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Hello invoked!");
            }
        }
    }
}
