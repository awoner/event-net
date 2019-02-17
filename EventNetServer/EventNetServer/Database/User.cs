using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Web.Helpers;

namespace EventNetServer.Database
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Bio { get; set; }
        [Required]
        public string PasswordStored { get; private set; }
        [NotMapped]
        public string Password
        {
            get => PasswordStored;
            set
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                PasswordStored = System.Text.Encoding.ASCII.GetString(data);
            }
        }

        public MyEvent Event { get; set; }
    }
}
