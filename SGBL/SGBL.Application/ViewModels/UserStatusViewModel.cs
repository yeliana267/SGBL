﻿
using System.ComponentModel.DataAnnotations;

namespace SGBL.Application.ViewModels
{
    public class UserStatusViewModel : BaseViewModel
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = default!;

    }
}
