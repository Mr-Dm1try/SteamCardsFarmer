using System;
using System.ComponentModel.DataAnnotations;

namespace GUI.Models.Types
{
    public class SSGame {
        [Key]
        public int Key { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string Link { get; set; }
    }
}
