﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private MyContext _context;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger  = logger;
        _context = context;
    }

    //--------------------------------------
    //--------------- ROUTES ---------------

    // Index - Displays the Registration and Login Forms
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    // CreateUser - Adds User to DB, Puts UserId in Session & Redirects to Success View
    [HttpPost("users/create")]
    public IActionResult CreateUser(User newUser)
    {
        // If Invalid Registration: Throw Errors and Return Index View
        if(!ModelState.IsValid) return View("Index");

        // If Valid Registration:
        else
        {
            // Hash Password and Add User to DB
            PasswordHasher<User> Hasher = new();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();

            // Put Action, UserId & UserFirstName in Session
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("UserFirstName", newUser.FirstName);

            // Redirect to AllWeddings Action
            return RedirectToAction("AllWeddings", "Wedding");
        }
    }

    // Login - Puts UserId & FirstName in Session then Redirects to Success View
    [HttpPost("users/login")]
    public IActionResult Login(LoginUser loginData)
    {
        // If Invalid Login: Throw Errors and Return Index View
        if(!ModelState.IsValid) return View("Index");

        // If Valid Login:
        else
        {
            // If Provided Email Not In DB:
            User? userInDb = _context.Users.FirstOrDefault(u => u.Email == loginData.LEmail);
            if(userInDb == null)
            {
                // Throw Error & Return Index View
                ModelState.AddModelError("LEmail", "Invalid Email/Password");
                return View("Index");
            }

            // If Provided Email Is In DB:
            // Hash Provided Password
            PasswordHasher<LoginUser> hasher = new();
            var result = hasher.VerifyHashedPassword(loginData, userInDb.Password, loginData.LPassword);

            // If Passwords Don't Match:
            if (result == 0)
            {
                // Throw Error and Return Index View
                ModelState.AddModelError("LEmail", "Invalid Email/Password");
                return View("Index");
            }
            // If Passwords Match:
            else
            {
                // Put UserId & UserFirstName in Session
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                HttpContext.Session.SetString("UserFirstName", userInDb.FirstName);

                // Redirect to AllWeddings Action
                return RedirectToAction("AllWeddings", "Wedding");
            }
        }
    }

    // Logout - Clears Session & Redirects to Index View
    [HttpPost("users/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

//----------------------------------------------
//--------------- CUSTOM METHODS ---------------
public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if(userId == null)
        {
            context.Result = new RedirectToActionResult("Index", "User", null);
        }
    }
}
