using System;
using System.ComponentModel.DataAnnotations;

namespace GUI.Models.Types
{
    public class SSGame {
        [Key]
        public int Key { get; set; }
        public String Title { get; set; }
        public Double Price { get; set; }
        public String Link { get; set; }
    }
}
