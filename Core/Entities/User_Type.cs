using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class User_Type
    {
        [Key]
        public int ID_Type { get; set; }

        public string Type_Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
