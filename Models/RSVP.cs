#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8618
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class RSVP
{
    [Key]
    public int RSVPId { get; set; }

    //------------------- MANY TO MANY -------------------
    // Foreign Keys
    public int UserId    { get; set; }
    public int WeddingId { get; set; }
    // Navigation Properties
    public User?    User    { get; set; }
    public Wedding? Wedding { get; set; }
}
