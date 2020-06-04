using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flashcard2.Models
{
    public class Card
    {
        [Key]
        public int CardId {get;set;}

        [Required(ErrorMessage="Please include a question")]
        public string Question {get;set;}

        [Required(ErrorMessage="Please include an answer")]
        public string Answer {get;set;}

        public int DeckId {get;set;}

        public Deck DeckSlot{get;set;}


    /* -------------------------------------------------------------------------------- */
    // DATETIMEs
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}