namespace BankWebAPI.Controllers
{
    //using System.Linq;
    //using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    //[Authorize]
    public abstract class BaseController : Controller
    {
        protected IActionResult RedirectToHome() => this.RedirectToAction("Index", "Home");

        protected string GetCurrentUserId()
        {
            //if (!this.User.Identity.IsAuthenticated)
            //{
            //    return null;
            //}

            //var claim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return "System";
        }

        protected void ShowErrorMessage(string message)
        {
            this.TempData["ErrorMessage"] = message;
        }

        protected void ShowSuccessMessage(string message)
        {
            this.TempData["SuccessMessage"] = message;
        }
    }
}