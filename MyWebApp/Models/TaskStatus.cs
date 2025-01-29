using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public enum SpoonTaskStatus
    {
        [Display(Name = "待处理")]
        Pending,
        [Display(Name = "进行中")]
        InProgress,
        [Display(Name = "已完成")]
        Completed,
        [Display(Name = "已取消")]
        Cancelled
    }
}