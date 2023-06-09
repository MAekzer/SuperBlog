﻿using SuperBlogData.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace SuperBlogData.Models.ViewModels
{
    public class TagViewModel
    {
        [Required(ErrorMessage = "Введите название тега")]
        public string Name { get; set; }
        public Dictionary<Tag, int> Tags { get; set; } = new Dictionary<Tag, int>();
    }
}
