using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FilmSimilarity
    {
        [Key]
        public int Id { get; set; }

        public int Film1Id { get; set; }
        public Film Film1 { get; set; }

        public int Film2Id { get; set; }
        public Film Film2 { get; set; }

        public double SimilarityScore { get; set; }
    }
}
