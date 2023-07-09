using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperBlogData.Models.Requests
{
    public class UserPutRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public string? About { get; set; }

        public string MakeFullName()
        {
            if (string.IsNullOrEmpty(MiddleName))
                return $"{LastName} {FirstName}";
            return $"{LastName} {FirstName} {MiddleName}";
        }
    }
}
