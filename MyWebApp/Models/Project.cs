using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class SpoonProjectModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "项目名称不能为空")]
        [Display(Name = "项目名称")]
        public required string Name { get; set; }

        [Display(Name = "项目描述")]
        public required string Description { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<SpoonTaskModel> Tasks { get; set; } = new List<SpoonTaskModel>();
    }
} 