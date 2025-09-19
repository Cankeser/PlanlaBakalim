using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlanlaBakalim.WebUI.Controllers
{
    [Authorize(Policy = "CustomerPolicy")]
    public class BaseController : Controller
    {
    }
}
