using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Flashcard2.Models
{
    public class Deck
    {
        [Key]
        public int DeckId {get;set;}

        [Required(ErrorMessage="Please include a name for your group")]
        public string DeckName {get;set;}

        public string Description {get;set;}

        public int UserId {get;set;}

        public User Creator {get;set;}

        public List<Card> Flashcards {get;set;}

        public bool Shared {get;set;} = true;

        public List<UserDeckFav> FavoriteBy {get;set;}

    /* -------------------------------------------------------------------------------- */
    // DATETIMEs
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}