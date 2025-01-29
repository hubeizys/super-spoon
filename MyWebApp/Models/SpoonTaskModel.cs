using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class SpoonTaskModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "任务标题不能为空")]
        [Display(Name = "任务标题")]
        public required string Title { get; set; }

        [Display(Name = "任务描述")]
        public required string Description { get; set; }

        [Display(Name = "截止时间")]
        [DataType(DataType.DateTime)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "重要程度")]
        public Priority Priority { get; set; }

        [Display(Name = "任务状态")]
        public SpoonTaskStatus Status { get; set; }

        [Display(Name = "提醒时间")]
        public DateTime? RemindTime { get; set; }

        [Display(Name = "所属项目")]
        public int? ProjectId { get; set; }
        public SpoonProjectModel? Project { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "更新时间")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public enum Priority
    {
        [Display(Name = "低")]
        Low,
        [Display(Name = "中")]
        Medium,
        [Display(Name = "高")]
        High,
        [Display(Name = "紧急")]
        Urgent
    }


} 