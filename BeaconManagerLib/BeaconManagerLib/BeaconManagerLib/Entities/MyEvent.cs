using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeaconManagerLib.Entities
{
    public class MyEvent
    {
        public int ID { get; set; }
        [Required]
        public Beacon Beacon { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ICollection<User> Users { get; set; }
    }
}