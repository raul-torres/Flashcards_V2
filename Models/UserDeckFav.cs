using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Flashcard2.Models
{
    public class UserDeckFav
    {
        [Key]
        public int UserDeckFavId {get;set;}

        public int UserId {get;set;}
        public User User {get;set;}


        public int DeckId {get;set;}
        public Deck Deck{get;set;}

    /* -------------------------------------------------------------------------------- */
    // DATETIMEs
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}