﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Requests
{
    public class RolePostRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string DisplayName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
    }
}
