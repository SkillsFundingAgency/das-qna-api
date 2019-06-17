using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.QnA.Api.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return Ok();
        }
    }
}