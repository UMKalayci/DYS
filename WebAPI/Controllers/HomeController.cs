using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Business.Abstract;
using Core.Utilities.Results;
using Entities.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        private readonly IFileService _fileService;
        public HomeController(IFileService fileService)
        {
            _fileService = fileService;
        }

        public IActionResult Index(bool onlyUser)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var  result= _fileService.GetAllFiles(userId);
            if (result.Success)
            {
                return View(result.Data);
            }
            else
            {
                return View();
            }
        }
    }
}