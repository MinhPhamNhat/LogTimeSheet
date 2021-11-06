using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Models
{
    [Table("Log")]
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        [DisplayName("Log note")]
        [Required(ErrorMessage = "Log note is required")]
        public string Note { get; set; }

        [DisplayName("Log Init Time")]
        [Required(ErrorMessage = "Log init time is required")]
        public DateTime InitTime { get; set; }

        [DisplayName("Log Standard time")]
        [Required(ErrorMessage = "Log Standard time is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public Double Stdtime { get; set; }

        [DisplayName("Date to log")]
        [Required(ErrorMessage = "Log Date is required")]
        public DateTime DateLog { get; set; }

        [DisplayName("Log Overtime")]
        [Required(ErrorMessage = "Log Overtime is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public Double Overtime { get; set; }

        [DisplayName("Is Approved")]
        public bool IsApproved { get; set; }

        [DisplayName("Date Approved")]
        public DateTime? DateApproved { get; set; }

        [DisplayName("User Approved this Log")]
        public virtual User UserApproved { get; set; }

        [DisplayName("Log Subtask")]
        [Required(ErrorMessage = "Log Subtask is required")]
        public virtual Subtask Subtask { get; set; }

        [DisplayName("User")]
        [Required(ErrorMessage = "User who log this timesheet is required")]
        public virtual User User { get; set; }

    }
}