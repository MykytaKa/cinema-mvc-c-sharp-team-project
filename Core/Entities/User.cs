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

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        public DateTime DateOfBirthday { get; set; }

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public User_Type UserType { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
