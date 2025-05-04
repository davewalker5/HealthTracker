using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Mvc.Models;
using HealthTracker.Client.Interfaces;

namespace HealthTracker.Mvc.Controllers
{
    public class LoginController : Controller
    {
        public const string LoginPath = "/login";

        private readonly IAuthenticationClient _client;
        private readonly IAuthenticationTokenProvider _tokenProvider;

        public LoginController(IAuthenticationClient client, IAuthenticationTokenProvider provider)
        {
            _client = client;
            _tokenProvider = provider;
        }

        /// <summary>
        /// Serve the login page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        /// <summary>
        /// Handle a request to login
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                // Authenticate with the sevice
                string token = await _client.AuthenticateAsync(model.UserName, model.Password);
                if (!string.IsNullOrEmpty(token))
                {
                    // Successful, so store the token in session, and redirect to the home page
                    _tokenProvider.SetToken(token);
                    result = RedirectToAction("Index", "Home");
                }
                else
                {
                    model.Message = "Incorrect username or password";
                    result = View(model);
                }
            }
            else
            {
                result = View(model);
            }

            return result;
        }

        /// <summary>
        /// Handle a request to log out
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult LogOut()
        {
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }
    }
}
