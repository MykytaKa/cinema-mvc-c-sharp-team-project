using Core.Entities;
using System.ComponentModel.DataAnnotations;

public class UserViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "User role is required.")]
    public int TypeId { get; set; }

    public User_Type UserType { get; set; }

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public List<User> AllUsers { get; set; } = new List<User>();
    public List<User_Type> AvailableRoles { get; set; } = new List<User_Type>();
}
