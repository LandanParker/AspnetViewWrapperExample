using System;
using System.Text;
using RazorEngine;
using RazorEngine.Templating;

namespace AspnetViewWrapperExample
{
    public class RazorServiceImpl
    {
        public IRazorEngineService RazorEngineService { get; set; }
        public RazorServiceImpl()
        {
            RazorEngineService = Engine.Razor;
            Console.WriteLine("did something");
        }
        
        public void RegisterTemplate(string template, string key, object model)
        {
            RazorEngineService.RunCompile(template, key, null, model);
        }
    }
}