using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankWebAPI.Controllers
{
    [Route("[controller]")]
    public class HomeController : BaseController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {

            return View();
        }
    }
}
