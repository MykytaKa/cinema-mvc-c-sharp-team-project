using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RegisterUserDto
    {
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string ConfirmPassword { get; set; } 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public DateTime DateOfBirth { get; set; } 
        public string PhoneNumber { get; set; }
       
    }
}
