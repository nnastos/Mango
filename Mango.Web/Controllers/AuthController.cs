﻿using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();

            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto? responseDto = await _authService.LoginAsync(obj);

            if (responseDto != null && responseDto.IsSuccess)
            {

                LoginResponseDto? loginResponse =
                JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                return RedirectToAction("Index", "Home");


            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto.Message);
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = SD.RoleAdmin,
                    Value = SD.RoleAdmin
                },
                new SelectListItem
                {
                    Text = SD.RoleCustomer,
                    Value = SD.RoleCustomer
                }
            };

            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto? result = await _authService.RegisterAsync(obj);
            ResponseDto? assignRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }

                assignRole = await _authService.AssignRoleAsync(obj);

                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration successful";
                    return RedirectToAction(nameof(Login));
                }
            }

            var roleList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = SD.RoleAdmin,
                    Value = SD.RoleAdmin
                },
                new SelectListItem
                {
                    Text = SD.RoleCustomer,
                    Value = SD.RoleCustomer
                }
            };

            ViewBag.RoleList = roleList;

            return View(obj);
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
