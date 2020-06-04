using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Flashcard2.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required(ErrorMessage="Please include your first name")]
        public string FirstName {get;set;}

        [Required(ErrorMessage="Please include your last name")]
        public string LastName {get;set;}

        [Required(ErrorMessage="Please include your email")]
        [EmailAddress(ErrorMessage="Email is not valid")]
        public string Email {get;set;}

        [Required(ErrorMessage="Password is required")]
        [MinLength(7, ErrorMessage="Password must be atleast 7 characters long")]
        [RegularExpression("^.*(?=.{6,18})(?=.*)(?=.*[A-Za-z])(?=.*[@%&#%^&*!]{1,}).*$", ErrorMessage = "Password must contain atleast 1 letter, 1 number and 1 special character")]
        public string Password {get;set;}



    // DATESTAMP INFORMATION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    // PASSWORD COMPARING ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [NotMapped]
        [Compare("Password", ErrorMessage="Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword {get;set;}

    // RELATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public List<Deck> MyDecks {get;set;}

        public List<UserDeckFav> FavoriteDecks {get;set;}
    }
}