using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace LogTimeSheet.Models
{
    [Table("Subtask")]
    public class Subtask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubtaskId { get; set; }

        [DisplayName("Subtask Name")]
        [Required(ErrorMessage = "Subtask name is required")]
        public string Name { get; set; }

        [DisplayName("Project")]
        [Required(ErrorMessage = "Project is required")]
        public virtual Project Project { get; set; }

        [DisplayName("Users Logs")]
        public virtual ICollection<Log> Logs { get; set; }
    }
}