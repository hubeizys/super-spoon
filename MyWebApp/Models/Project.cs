using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "项目名称不能为空")]
        [Display(Name = "项目名称")]
        public string Name { get; set; }

        [Display(Name = "项目描述")]
        public string Description { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Task> Tasks { get; set; }
    }
} 