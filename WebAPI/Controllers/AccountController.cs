using Business.Abstract;
using Business.Concrete;
using Core.Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AccountController : Controller
    {
        private IAuthService _authService;
        private IUserService _userService;

        public AccountController(IAuthService authService, IUserService userService)
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
            if (ModelState.IsValid)
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
            }
            ModelState.AddModelError("", "Kullanıcı oluşturulamadı.");
            return View();
        }

        public IActionResult PasswordRecovery()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PasswordRecovery(ResetPasswordModel resetPasswordModel)
        {
            var user = _userService.GetByMail(resetPasswordModel.Email);
            if (user != null)
            {
                var resetToken = _authService.CreateAccessToken(user);

                var fromAddress = new MailAddress("bmugurmutlukalayci@gmail.com", "Uğur Mutlu KALAYCI");
                var toAddress = new MailAddress(user.Email, "Dursun KALAYCI");
                const string fromPassword ="++";
                const string subject = "Şifre Güncelleme Talebi";
                string body = $"<a target=\"_blank\" href=\"https://localhost:44335{Url.Action("UpdatePassword", "Account", new { UserEmail = user.Email, Token = HttpUtility.UrlEncode(resetToken.Data.Token) })}\">Yeni şifre talebi için tıklayınız</a>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                ViewBag.State = true;
            }
            else
                ViewBag.State = false;

            return View();
        }

        public IActionResult UpdatePassword(string UserEmail, string Token)
        {
            var updatePasswordModel = new UpdatePasswordModel();
            updatePasswordModel.UserEmail = UserEmail;
            updatePasswordModel.Token = Token;
            return View(updatePasswordModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(UpdatePasswordModel updatePasswordModel)
        {
            if (!ModelState.IsValid)
                return View(updatePasswordModel);

            var user = _userService.GetByMail(updatePasswordModel.UserEmail);
            if (user == null)
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            var result =  _authService.ResetPasswordAsync(user, HttpUtility.UrlDecode(updatePasswordModel.Token), updatePasswordModel.Password);
            if (!result.Success)
            {
                return View();
            }
           return RedirectToAction(nameof(ResetPasswordConfirmation));

        }

        public IActionResult ResetPasswordConfirmation(bool IsSuccess)
        {
            return View(IsSuccess);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}