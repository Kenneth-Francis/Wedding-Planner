#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId { get; set; }

    [Required(ErrorMessage = "Wedder One is required.")]
    [MinLength(3, ErrorMessage = "Wedder One must be at least 3 characters.")]
    [MaxLength(45)]
    [DisplayName("Wedder One:")]
    public string WedderOne { get; set; }

    [Required(ErrorMessage = "Wedder Two is required.")]
    [MinLength(3, ErrorMessage = "Wedder Two must be at least 3 characters.")]
    [MaxLength(45)]
    [DisplayName("Wedder Two:")]
    public string WedderTwo { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    [FutureDate]
    [DataType(DataType.Date)]
    [DisplayName("Date:")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [MaxLength(45)]
    [DisplayName("Address:")]
    public string Address { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    //-------------------- ONE TO MANY --------------------
    // Foreign Key
    public int UserId { get; set; }
    // Creator of the Wedding
    public User? Planner { get; set; }

    //------------------- MANY TO MANY -------------------
    // List of RSVP Associations
    public List<RSVP> RSVPs { get; set; } = new();
}


public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime Input = (DateTime)value;
        DateTime Now = DateTime.Now;

        if (Input <= Now)
        {
            return new ValidationResult("Date must be in the future.");
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}
