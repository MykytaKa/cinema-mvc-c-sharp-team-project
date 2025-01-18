using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class User
    {
        [Key]
        public int User_ID { get; set; }

        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Email { get; set; }
        public string Hash_Password { get; set; }
        public DateTime Date_Of_Birthday { get; set; }

        public int ID_Type { get; set; }
        public User_Type User_Type { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
