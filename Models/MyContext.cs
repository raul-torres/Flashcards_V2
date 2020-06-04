using Microsoft.EntityFrameworkCore;
 
namespace Flashcard2.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }

         public DbSet<User> Users {get;set;}
         public DbSet<Deck> Decks {get;set;}
         public DbSet<Card> Cards {get;set;}
         public DbSet<UserDeckFav> UserDeckFavs {get;set;}
    }
}