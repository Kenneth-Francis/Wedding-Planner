#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models;

public class LoginUser
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [DisplayName("Email:")]
    public string LEmail { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [DisplayName("Password:")]
    public string LPassword { get; set; }
}
