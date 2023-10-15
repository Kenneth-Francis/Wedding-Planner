#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [MinLength(2, ErrorMessage = "First Name must be at least 2 characters.")]
    [DisplayName("First Name:")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required.")]
    [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters.")]
    [DisplayName("Last Name:")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [UniqueEmail]
    [DisplayName("Email:")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    [DisplayName("Password:")]
    public string Password { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Please confirm password.")]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [DisplayName("Confirm Password:")]
    public string Confirm { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    //-------------------- ONE TO MANY --------------------
    // User's List of Planned Weddings
    public List<Wedding> PlannedWeddings { get; set; } = new();

    //------------------- MANY TO MANY -------------------
    // List of RSVP Associations
    public List<RSVP> RSVPs { get; set; } = new();
}


public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Email is required!");
        }

        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));

        if (_context.Users.Any(e => e.Email == value.ToString()))
        {
            return new ValidationResult("Email must be unique!");
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}
