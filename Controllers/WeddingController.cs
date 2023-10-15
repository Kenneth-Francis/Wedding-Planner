using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;
    private MyContext _context;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger  = logger;
        _context = context;
    }

    //------------------------------------------------
    //-------------------- ROUTES --------------------

    // Displays All Weddings
    [HttpGet("weddings")]
    public ViewResult AllWeddings()
    {
        // Get List of Weddings with associated Planner
        List<Wedding> WeddingsFromDb =
            _context.Weddings   .Include(w => w.RSVPs)
                                .ThenInclude(u => u.User)
                                .Include(w => w.Planner)
                                .ToList();

        HttpContext.Session.SetString("Action", "AllWeddings");
        // Return AllWeddings View
        return View(WeddingsFromDb);
    }

    // Displays the Add a Wedding Form
    [HttpGet("weddings/new")]
    public ViewResult NewWedding()
    {
        HttpContext.Session.SetString("Action", "NewWedding");
        return View();
    }

    // Adds the New Wedding to the DB
    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        // If Invalid Data: Throw Errors and Return NewWedding View
        if (!ModelState.IsValid) return View("NewWedding");

        // If Valid Data:
        else
        {
            //Add Wedding to DB
            _context.Add(newWedding);
            _context.SaveChanges();
            
            // Redirect to AllWeddings Action, 
            return RedirectToAction("ViewWedding", new {id = newWedding.WeddingId});
        }
    }

    // Displays One Wedding
    [HttpGet("weddings/{id}/view")]
    public IActionResult ViewWedding(int id)
    {
        // Get One Wedding by ID with RSVP associated Users
        Wedding? oneWedding = _context.Weddings
                                .Include(w => w.RSVPs)
                                .ThenInclude(r => r.User)
                                .FirstOrDefault(w => w.WeddingId == id);

        HttpContext.Session.SetString("Action", "ViewWedding");
        // Return ViewWedding View
        return View(oneWedding);
    }

    // Deletes One Wedding
    [HttpPost("weddings/{id}/delete")]
    public IActionResult DeleteWedding(int id)
    {
        // Get One Wedding by ID
        Wedding? oneWedding = _context.Weddings.SingleOrDefault(w => w.WeddingId == id);

        // If Wedding Exists:
        if (oneWedding != null)
        {
            // Delete Wedding from DB
            _context.Weddings.Remove(oneWedding);
            _context.SaveChanges();
        }

        // Redirect to AllWeddings Action
        return RedirectToAction("AllWeddings");
    }

    // Toggles RSVP Status of a Wedding
    [HttpPost("weddings/{id}/rsvp")]
    public RedirectToActionResult ToggleRSVP(int id)
    {
        int UserId = (int)HttpContext.Session.GetInt32("UserId");
        RSVP? existingRSVP = _context.RSVPs.FirstOrDefault(r => r.WeddingId == id && UserId == r.UserId);

        if (existingRSVP == null)
        {
            RSVP newRSVP = new(){UserId = UserId, WeddingId = id};
            _context.Add(newRSVP);
        }
        else
        {
            _context.Remove(existingRSVP);
        }

        _context.SaveChanges();
        return RedirectToAction("AllWeddings");
    }




}
