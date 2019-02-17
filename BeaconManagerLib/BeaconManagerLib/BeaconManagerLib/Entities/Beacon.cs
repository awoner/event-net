using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BeaconManagerLib.Entities
{
    public class Beacon
    {
        [Required, Key]
        public string UUID { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int Minor { get; set; }
        [Required]
        public int Major { get; set; }
    }
}
