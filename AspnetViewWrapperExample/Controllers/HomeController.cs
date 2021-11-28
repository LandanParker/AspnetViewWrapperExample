using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspnetViewWrapperExample.Elements;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspnetViewWrapperExample.Models;
using RazorEngine.Templating;

namespace AspnetViewWrapperExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public dynamic ExampleObject { get; set; } = new
        {
            someprop = new
            {
                what = (Action<string>)((s) =>
                {
                    Console.WriteLine("example");
                })
            }
        };
        
        public HomeController(ILogger<HomeController> logger, RazorServiceImpl impl)
        {
            _logger = logger;
            impl.RazorEngineService.RunCompile(nameof(HomeController), "keyname", null, new {Name="name"});
        }

        public IActionResult Index([FromServices] RazorServiceImpl impl)
        {
            string template = "nothing";

            TagElement test = new TagElement{Tag = "div"};
                
            TestClass obj = new TestClass {Value = $"some value: {DateTime.UtcNow.Second}"};
            TestClass obj2 = new TestClass {Value = $"another value: {DateTime.UtcNow.Second}"};
            TestClass obj3 = new TestClass {Value = $"another 2value: {DateTime.UtcNow.Second}"};
            
            test.AssignRoot()
                .AddChild(out TagElement subElement, e =>
                {
                    e.Tag = "div";
                    e.SetDataContainer(obj);
                    e.WithClasses(out CssClasses classes);
                    e.WithStyles(out Styles styles);
                    classes.addClass = "someclass";
                    styles.AddStyle("background-color",$"rgb({new Random().Next(255)},{new Random().Next(255)},{new Random().Next(255)})");
                });

            int total = new Random().Next(5);

            for (int i = 0; i < total; i++)
            {
                TestClass obji = new TestClass {Value = $"item: {i} {DateTime.UtcNow.Second}"};
                test.AddChild(out TagElement iterated, e =>
                {
                    e.Tag = "div";
                    e.SetDataContainer(obji);
                    e.WithClasses(out CssClasses classes);
                    e.WithStyles(out Styles styles);
                    classes.addClass = "someclass";
                    styles.AddStyle("background-color",$"rgb({new Random().Next(255)},{new Random().Next(255)},{new Random().Next(255)})");
                });
                iterated.SetContent(nameof(obji.Value));
            }

            subElement.SetContent(nameof(obj.Value));
            
            Console.WriteLine(test.ToString().GetHashCode());
            
            string resp = impl.RazorEngineService.RunCompile(test.ToString(), $"{test.ToString().GetHashCode()}", null, test); 

            ViewData.Model = resp;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}