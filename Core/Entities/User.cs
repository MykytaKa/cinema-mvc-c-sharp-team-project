using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Email { get; set; }
        public string Hash_Password { get; set; }
        public DateTime Date_Of_Birthday { get; set; }

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public User_Type User_Type { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
