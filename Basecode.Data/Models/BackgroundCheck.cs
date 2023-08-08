using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.Models;

public class BackgroundCheck
{
    [Key] public int Id { get; set; }

    public int CharacterReferenceId { get; set; }
    public int UserId { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Relationship { get; set; }
    public DateTime AnsweredDate { get; set; }
    public string? Q1 { get; set; }
    public string? Q2 { get; set; }
    public string? Q3 { get; set; }
    public string? Q4 { get; set; }
    public CharacterReference CharacterReference { get; set; } = null!;
    public User User { get; set; }
    
}