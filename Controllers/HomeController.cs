using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Activity_center.Models;

namespace Activity_center.Controllers
{
    public class HomeController : Controller
    {
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("createuser")]
        public IActionResult CreateUser(User newUser)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists in db
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View("Index");
                }
                // Hash new user's password and save new user to db
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();
                UserSession = newUser.UserId;
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser currUser)
        {
            if (ModelState.IsValid)
            {
                // Check if email exists in database
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == currUser.LoginEmail);
                if (existingUser == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(currUser, existingUser.Password, currUser.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                UserSession = existingUser.UserId;
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if (UserSession==null)
                return RedirectToAction("Index");
            
            else
            {
                var AllShindigs = dbContext.Shindigs
                    .Include(s =>s.Responses)
                    .Include(a =>a.Creator)
                    .OrderBy(s => s.Date)
                    .ToList();
                User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == UserSession);    
                ViewBag.User=thisUser;
                ViewBag.UserId=UserSession;
                return View("Dashboard",AllShindigs);    
            }    
        }

        [HttpGet("new")]
        public IActionResult New()
        {
            if (UserSession==null)
                return RedirectToAction("Index");
            else
            {
                return View("New");
            }    
        }

        [HttpPost("create")]
        public IActionResult Create(Shindig newShindig)
        {
            if (UserSession == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                newShindig.UserId=(int)UserSession;
                dbContext.Shindigs.Add(newShindig);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("New");
            }
        }

        [HttpGet("{shindigId}")]
        public IActionResult Show(int shindigId)
        {
            if (UserSession==null)
                return RedirectToAction("Index");
            else
            {
                var thisShindig=dbContext.Shindigs
                    .Include(s =>s.Creator)
                    .Include(s =>s.Responses)
                    .ThenInclude(r => r.Guest)
                    .FirstOrDefault(s =>s.ShindigId==shindigId);
                ViewBag.UserId=UserSession;
                    return View(thisShindig);
            }
        }


        [HttpGet("delete/{shindigId}")]
        public IActionResult Delete(int shindigId)
        {
            if (UserSession == null)
                return RedirectToAction("Index");

            Shindig toDelete = dbContext.Shindigs.FirstOrDefault(w => w.ShindigId == shindigId);
            
            if (toDelete == null)
                return RedirectToAction("Dashboard");
            // Redirect to dashboard if user trying to delete isn't the wedding creator
            if (toDelete.UserId != UserSession)
                return RedirectToAction("Dashboard");
            
            dbContext.Shindigs.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("Join/{shindigId}")]
        public IActionResult Join(int shindigId)
        {
            if (UserSession==null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Response newResponse= new Response()
                {
                    ShindigId=shindigId,
                    UserId=(int)UserSession
                };
                dbContext.Responses.Add(newResponse);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet("Leave/{shindigId}")]
        public IActionResult Leave(int shindigId)
        {
            if (UserSession == null)
                return RedirectToAction("Index");
            
            // Query to grab the appropriate response to remove
            Response toDelete = dbContext.Responses.FirstOrDefault(r => r.ShindigId == shindigId && r.UserId == UserSession);
            
            // Redirect to dashboard if no match for response in db
            if (toDelete == null)
                return RedirectToAction("Dashboard");

            dbContext.Responses.Remove(toDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
