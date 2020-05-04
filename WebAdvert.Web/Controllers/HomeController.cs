using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models;
using WebAdvert.Web.ServiceClients;

namespace WebAdvert.Web.Controllers
{
    public class HomeController : Controller
    {
        public IMapper Mapper { get; }
        public IAdvertApiClient ApiClient { get; }

        public HomeController(IMapper mapper, IAdvertApiClient apiClient)
        {
            Mapper = mapper;
            ApiClient = apiClient;
        }

        [Authorize]
        [ResponseCache(Duration = 60)]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
