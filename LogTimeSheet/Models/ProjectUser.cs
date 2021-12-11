using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LogTimeSheet.Models
{
    public class ProjectUser
    {
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }
        public User User { get; set; }
    }
}