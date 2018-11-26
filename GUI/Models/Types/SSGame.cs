using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Models.Types {
    public class SSGame {
        [Key]
        public int Key { get; set; }        
        public String Title { get; set; }
        public Double Price { get; set; }
        public String Link { get; set; }
        public String ImageUrl { get; set; }
    }
}
