﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MurderGame.UI.Controllers
{
    [Authorize(Roles = "Member")]

    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
