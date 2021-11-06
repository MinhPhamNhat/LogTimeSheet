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
    [Table("Project")]
    public class Project
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        [DisplayName("Project Name")]
        [Required(ErrorMessage = "Project name is required")]
        public string Name { get; set; }

        [DisplayName("Project Code")]
        [Required(ErrorMessage = "Project Code is required")]
        public string ProjectCode { get; set; }

        [DisplayName("Project type")]
        [Required(ErrorMessage = "Project type is required")]
        public bool Type { get; set; }

        [DisplayName("Project Init time")]
        [Required(ErrorMessage = "Project Init time is required")]
        public DateTime InitTime { get; set; }

        [DisplayName("Project start date")]
        [Required(ErrorMessage = "Project start time is required")]
        public DateTime StartDate { get; set; }

        [DisplayName("Project end date")]
        [Required(ErrorMessage = "Project end time is required")]
        public DateTime EndDate { get; set; }

        [DisplayName("Project manager")]
        public virtual User Manager { get; set; }

        [DisplayName("Project Member")]
        public virtual ICollection<ProjectUser> ProjectUsers { get; set; }

        public virtual ICollection<Subtask> Subtasks { get; set; }
    }
}