/*
 * Blog Project
 * An ASP.NET MVC Blog
 * Based on Coder Foundry Blog series
 * 
 * Mark Ambrocio 2022
 * https://github.com/markanator/csharp_AmbroBlogProject
 */
using AmbroBlogProject.Data;
using AmbroBlogProject.Models;
using AmbroBlogProject.Services;
using AmbroBlogProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace AmbroBlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _ctx;


        public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender, ApplicationDbContext ctx)
        {
            _logger = logger;
            _emailSender = emailSender;
            _ctx = ctx;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageNum = page ?? 1;
            var pageSize = 5;

                            /*.Where(b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))*/
            var blogs = _ctx.Blogs
                            .Include(b => b.BlogUser)
                            .OrderByDescending(b => b.Created)
                            .ToPagedListAsync(pageNum, pageSize);

            return View(await blogs);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            // this is where we will be emailing...
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
