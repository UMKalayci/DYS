using Business.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    public class AccountController : Controller
    {
        private IAuthService _authService;
        private IUserService _userService;

        public AccountController(IAuthService authService,IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(UserForLoginDto userLogin)
        {
            if (ModelState.IsValid)
            {
                var result = _authService.Login(userLogin);
                if (result.Success)
                {
                    var userRoles = _userService.GetClaims(result.Data);
                    var claims = new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, result.Data.Id.ToString()),
                    new Claim(ClaimTypes.Name,result.Data.FirstName+" "+result.Data.LastName),
                    };
                    foreach (var item in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.Name));
                    }

                    var userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(new[] { userIdentity });
                    HttpContext.SignInAsync(principal);
                    return RedirectToAction("Index", "Home");

                }

            }

            ModelState.AddModelError("", "Giriş işlemi başarısız.");

            return View();
        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(UserForRegisterDto userRegister)
        {

            var userExists = _authService.UserExists(userRegister.Email);
            if (!userExists.Success)
            {
                ModelState.AddModelError("", "Kullanıcı zaten mevcut.");
                return View();
            }
            if (userRegister.Password != userRegister.ConfirmPassword)
            {
                ModelState.AddModelError("", "Girilen şifreler uyuşmamaktadır.");
                return View();
            }

            var registerResult = _authService.Register(userRegister);
            if (registerResult.Success)
            {
                var userRoles = _userService.GetClaims(registerResult.Data);
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, registerResult.Data.Id.ToString()),
                    new Claim(ClaimTypes.Name,registerResult.Data.FirstName+" "+registerResult.Data.LastName),
                    };

                var userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(new[] { userIdentity });
                HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Home");

            }

            ModelState.AddModelError("", "Kullanıcı oluşturulamadı.");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}