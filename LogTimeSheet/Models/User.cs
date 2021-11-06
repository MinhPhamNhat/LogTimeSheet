using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [Required(ErrorMessage = "User id is required")]
        public string UserId { get; set; }

        [DisplayName("Role")]
        [Required(ErrorMessage = "Role is required")]
        [Range(0, 2, ErrorMessage = "Role must be 0 (ADMIN), 1 (PM), 2 (Staff)")]
        public int Role { get; set; }

        [DisplayName("Position")]
        [Required(ErrorMessage = "Role is required")]
        public string Position { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Role is required")]
        public string Name { get; set; }

        [DisplayName("Username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }
    }
}