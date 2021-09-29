using Microsoft.AspNetCore.Mvc;

namespace SampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new ContentResult()
            {
                Content = "Sample RequestFilter App" +
                "\n\nRefresh this page multiple times to hit request rate limit and get 429 http status code response" +
                "\n\nWait for 5 sec to get normal response back"
            };
        }
    }
}