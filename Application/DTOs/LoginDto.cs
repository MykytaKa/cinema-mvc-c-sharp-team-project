using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class LoginDto
    {
        public string Login { get; set; } // Email або номер телефону
        public string Password { get; set; }
    }
}
