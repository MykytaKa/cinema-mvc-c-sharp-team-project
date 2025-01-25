using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RegisterUserDto
    {
        public string Email { get; set; } // Email
        public string Password { get; set; } // Пароль
        public string ConfirmPassword { get; set; } // Підтвердження пароля
        public string FirstName { get; set; } // Ім'я
        public string LastName { get; set; } // Прізвище
        public DateTime DateOfBirth { get; set; } // Дата народження
        public string PhoneNumber { get; set; }
       
    }
}
