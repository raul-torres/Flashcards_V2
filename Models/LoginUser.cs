using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Flashcard2.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage="Please include your email")]
        public string LEmail {get;set;}


        [Required(ErrorMessage="Please include your password")]
        [DataType(DataType.Password)]
        public string LPassword {get;set;}
    }
}