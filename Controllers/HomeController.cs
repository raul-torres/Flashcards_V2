using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Flashcard2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace Flashcard2.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
    // HANDLING REGISTRATION
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        // HANDLING USER CREATION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpPost("usercreator")]
            public IActionResult usercreator(User NewUser)
            {
                if(ModelState.IsValid)
                {
                    if(dbContext.Users.Any(U => U.Email == NewUser.Email))
                    {
                        ModelState.AddModelError("Email", "Email is already linked to an account");
                        return View("Index");
                    }
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);

                    dbContext.Add(NewUser);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserInSession", NewUser.UserId);
                    return RedirectToAction("Home");
                }
                return View("Index");
            }

    // HANLING LOGIN PAGE 
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        // HANDLING USER LOGIN ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpPost("access")]
            public IActionResult access(LoginUser AccessUser)
            {
                if(ModelState.IsValid)
                {
                    var UserInDb = dbContext.Users.FirstOrDefault(u => u.Email == AccessUser.LEmail);
                    if(UserInDb == null)
                    {
                        ModelState.AddModelError("LEmail", "Invalid email or password");
                        return View("Login");
                    }
                    var Hasher = new PasswordHasher<LoginUser>();
                    var Result = Hasher.VerifyHashedPassword(AccessUser, UserInDb.Password, AccessUser.LPassword);
                    if(Result == 0)
                    {
                        ModelState.AddModelError("LEmail", "Invalid email or password");
                        return View("Login");
                    }
                    HttpContext.Session.SetInt32("UserInSession", UserInDb.UserId);
                    return RedirectToAction("Home");
                }
                return View("Login");
            }
        // HANDLING USER LOG OUT ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpGet("logout")]
            public IActionResult logout()
            {
                HttpContext.Session.Clear();

                return RedirectToAction("Login");
            }
    // HANDLING HOME PAGE
        [HttpGet("Home")]
        public IActionResult Home()
        {
           int? Session = HttpContext.Session.GetInt32("UserInSession");
           if(Session == null)
           {
               return RedirectToAction("Login");
           }
           ViewBag.CurrentUser = dbContext.Users
                .FirstOrDefault(u => u.UserId == (int)Session);
            return View();
        }
    // HANDLING NEW DECK PAGE 
        [HttpGet("newdeck")]
        public IActionResult NewDeck()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        //HANDLING NEW DECK POST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpPost("newdeck")]
            public IActionResult newdeck(Deck NewDeck)
            {
                if(ModelState.IsValid)
                {
                    NewDeck.UserId = (int)HttpContext.Session.GetInt32("UserInSession");
                    dbContext.Add(NewDeck);
                    dbContext.SaveChanges();
                    return RedirectToAction("alldecks");
                }
                return View("alldecks");
            }
        // HANDLING DECK DELETION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpGet("deletedeck/{IdDeck}")]
            public IActionResult deletedeck(int IdDeck)
            {
                Deck DeckToDelete = dbContext.Decks 
                    .FirstOrDefault(D => D.DeckId == IdDeck);
                dbContext.Remove(DeckToDelete);
                dbContext.SaveChanges();
                return RedirectToAction("alldecks");
            }
    // HANDLING ALL DECK VIEW
        [HttpGet("alldecks")]
        public IActionResult AllDecks()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.AllDecks = dbContext.Decks
                .Include(x => x.Flashcards)
                .Where(U => U.UserId == Session)
                .ToList();
            return View();
        }
    // HANDLING THIS DECK VIEW
        [HttpGet("thisdeck/{IdDeck}")]
        public IActionResult ThisDeck(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.ThisDeck = dbContext.Decks
                .Include(U => U.Creator)
                .Include(z => z.Flashcards)
                .FirstOrDefault(D => D.DeckId == IdDeck);
            return View();
        }
    // HANDLING EDIT DECK VIEW
        [HttpGet("edit/{IdDeck}")]
        public IActionResult Edit(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            Deck ThisDeck = dbContext.Decks
                .FirstOrDefault(D => D.DeckId == IdDeck);
            return View(ThisDeck);
        }
        // HANDLING EDIT DECK CHANGES POST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpPost("/makechanges/{IdDeck}")]
            public IActionResult makechanges(int IdDeck, Deck UpdatedDeck)
            {
                Deck CurrentDeck = dbContext.Decks 
                        .FirstOrDefault(D => D.DeckId == IdDeck);
                if(ModelState.IsValid)
                {
                    CurrentDeck.DeckName = UpdatedDeck.DeckName;
                    CurrentDeck.Description = UpdatedDeck.Description;
                    dbContext.SaveChanges();
                    return Redirect("/thisdeck/" + IdDeck);
                }
                return View("edit", CurrentDeck);
            }
    // HANDLING ALL CARDS VIEW
        [HttpGet("allcards/{IdDeck}")]
        public IActionResult AllCards(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            List<Card> CardsInThisDeck = dbContext.Cards
                .Where(D => D.DeckId == IdDeck)
                .ToList();
            ViewBag.Deck = IdDeck;
            return View(CardsInThisDeck);
        }
    // HANDLING NEW CARD FORM 
        [HttpGet("newcard/{IdDeck}")]
        public IActionResult NewCard(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.ThisDeck = IdDeck;
            return View();
        }
    // HANDLING CARD EDIT (EDIT) ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [HttpGet("editcard/{IdCard}")]
        public IActionResult EditCard(int IdCard)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            Card ThisCard = dbContext.Cards
                  .Include(Y => Y.DeckSlot)
                  .FirstOrDefault(Z => Z.CardId == IdCard);
            return View(ThisCard);
            }
        // HANDLING NEW CARD (POST) ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpPost("card/{IdDeck}")]
            public IActionResult card(int IdDeck, Card NewCard)
            {
                if(ModelState.IsValid)
                {
                    NewCard.DeckId = IdDeck;
                    dbContext.Add(NewCard);
                    dbContext.SaveChanges();
                    return Redirect("/newcard/" + IdDeck);
                }
                return View("newcard");
            }
        // HANDLING CARD EDIT SAVE CHANGES (POST)
            [HttpPost("savecardchanges/{IdCard}")]
            public IActionResult savecardchanges(int IdCard, Card UpdatedCard)
            {
                if(ModelState.IsValid)
                {
                    Card ThisCard = dbContext.Cards
                        .FirstOrDefault(Z => Z.CardId == IdCard);
                    ThisCard.Question = UpdatedCard.Question;
                    ThisCard.Answer = UpdatedCard.Answer;
                    dbContext.SaveChanges();
                    return Redirect("/allcards/" + ThisCard.DeckId);
                }
                Card CardToPass = dbContext.Cards
                        .FirstOrDefault(Z => Z.CardId == IdCard);
                return View("editcard", CardToPass);
            }
        // HANDLING CARD DELETE (DELETE) ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            [HttpGet("deletecard/{IdCard}/{IdDeck}")]
            public IActionResult deletecard(int IdCard, int IdDeck)
            {
                Card DeleteCard = dbContext.Cards
                    .FirstOrDefault(Z => Z.CardId == IdCard);
                dbContext.Remove(DeleteCard);
                dbContext.SaveChanges();
                return Redirect("/allcards/"+ IdDeck);
            }
    // HANDLING ALL DECKS VIEW (GET)
        [HttpGet("everysingledeck")]
        public IActionResult EverySingleDeck()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            List<Deck> EveryDeck = dbContext.Decks
                .Include(C => C.Creator)
                .Include(x => x.Flashcards)
                .Include(f => f.FavoriteBy)
                .ThenInclude(u => u.User)
                .ToList();
            ViewBag.CurrentUser = Session;
            return View(EveryDeck);
        }
    // HANDLING OTHER PERSONS DECK INFO (GET)
        [HttpGet("thispersonsdeck/{IdDeck}")]
        public IActionResult ThisPersonsDeck(int IdDeck)
        {
            int? Sessions = HttpContext.Session.GetInt32("UserInSession");
            if(Sessions == null)
            {
                return RedirectToAction("Login");
            }
            Deck ThisDeck = dbContext.Decks
                .Include(y => y.Flashcards)
                .Include(x => x.Creator)
                .Include(h => h.FavoriteBy)
                .ThenInclude(i => i.User)
                .FirstOrDefault(z => z.DeckId == IdDeck);
            ViewBag.User = (int)HttpContext.Session.GetInt32("UserInSession");
            return View(ThisDeck);
        }
    // HANDLING OTHER USER CARDS VIEW (GET)
        [HttpGet("usercards/{IdDeck}")]
        public IActionResult UserCards(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            List<Card> AllCards = dbContext.Cards    
                .Where(x => x.DeckId == IdDeck)
                .ToList();
            ViewBag.DeckId = IdDeck;
            return View(AllCards);
        }
    // HANDLING SHARED STATUS CHANGE TO PRIVATE
        [HttpGet("pri/{IdDeck}")]
        public IActionResult pri(int IdDeck)
        {
            Deck ThisDeck = dbContext.Decks
                .FirstOrDefault(z => z.DeckId == IdDeck);
            ThisDeck.Shared = false;
            dbContext.SaveChanges();
            return Redirect("/thisdeck/" + IdDeck);
        }
    // HANDLING SHARED STATUS CHANGE TO PUBLIC
        [HttpGet("pub/{IdDeck}")]
        public IActionResult pub(int IdDeck)
        {
            Deck CDeck = dbContext.Decks
                .FirstOrDefault(y => y.DeckId == IdDeck);
            CDeck.Shared = true;
            dbContext.SaveChanges();
            return Redirect("/thisdeck/" + IdDeck);
        }
    // HANDLING ADDING TO FAVORITE
        [HttpGet("fav/{IdDeck}")]
        public IActionResult fav(UserDeckFav NewFav, int IdDeck)
        {
            NewFav.UserId = (int)HttpContext.Session.GetInt32("UserInSession");
            NewFav.DeckId = IdDeck;
            dbContext.Add(NewFav);
            dbContext.SaveChanges();
            return Redirect("/thispersonsdeck/" + IdDeck);
        }
    // HANDLING REMOVING FROM FAVORITE
        [HttpGet("unfav/{IdDeck}")]
        public IActionResult unfav(int IdDeck)
        {
            int Session = (int)HttpContext.Session.GetInt32("UserInSession");
            UserDeckFav ThisRelation = dbContext.UserDeckFavs
                .Where(D => D.DeckId == IdDeck)
                .FirstOrDefault(U => U.UserId == Session);
            dbContext.Remove(ThisRelation);
            dbContext.SaveChanges();
            return Redirect("/thispersonsdeck/" + IdDeck);
        }
    // HANDLING FAVORITE DECKS VIEW (GET)
        [HttpGet("myfavdecks")]
        public IActionResult MyFavDecks()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            List<Deck> AllDecks = dbContext.Decks
                .Include(Fl => Fl.Flashcards)
                .Include(C => C.Creator)
                .Include(Fa => Fa.FavoriteBy)
                .ThenInclude(U => U.User)
                .ToList();
            ViewBag.User = (int)HttpContext.Session.GetInt32("UserInSession");
            return View(AllDecks);
        }
    // HANDLING QUIZ VIEW (GET)
        [HttpGet("quiz")]
        public IActionResult Quiz()
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("login");
            }
            List<Deck> AllDecks = dbContext.Decks   
                .Include(Fl => Fl.Flashcards)
                .Include(C => C.Creator)
                .Include(Fa => Fa.FavoriteBy)
                .ThenInclude(U => U.User)
                .ToList();
            ViewBag.User = HttpContext.Session.GetInt32("UserInSession");
            return View(AllDecks);
        }
    // HANDLING RULES OF QUIZ (GET)
        [HttpGet("usedeck/{IdDeck}")]
        public IActionResult UseDeck(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            Deck ThisDeck = dbContext.Decks 
                .Include(Fl => Fl.Flashcards)
                .FirstOrDefault(D => D.DeckId == IdDeck);
            return View(ThisDeck);
        }
    
    // HANDLING QUIZ BEGIN (GET)
        [HttpGet("beginquiz/{IdDeck}")]
        public IActionResult BeginQuiz(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            HttpContext.Session.Remove("ViewedCards");
            List<Card> AllCards = dbContext.Cards
                .Where(D => D.DeckId == IdDeck)
                .ToList();
            int NumCards = AllCards.Count();
            Random random = new Random();
            int RanNumber = random.Next(0, NumCards);
            Card ThisCard = AllCards[RanNumber];
            HttpContext.Session.SetString("ViewedCards", RanNumber.ToString());

            string CorrectCards = HttpContext.Session.GetString("ViewedCards");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~CORRECT CARDS~~~~~~~~~~~~~~~~~~~~~~~"+ CorrectCards);

            return View(ThisCard);
        }
    // HANDLING CORRECT GUESS (GET)
        [HttpGet("gssdr/{IdDeck}")]
        public IActionResult gssdr(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            List<Card> AllCards = dbContext.Cards
                .Where(D => D.DeckId == IdDeck)
                .ToList();
            int NumCards = AllCards.Count();
            Random random = new Random();
            int RanNumber = random.Next(0, NumCards);
            string CorrectCards = HttpContext.Session.GetString("ViewedCards");
            // If statement checking to see if all cards have been used
            if(AllCards.Count() == CorrectCards.Length)
            {
                return RedirectToAction("Home");
            }
            // While loop checking to see if random number ID has already been used
            int i = 0;
            while(i < CorrectCards.Length)
            {  
                Console.WriteLine("~~~~~~~~~~~~~~~~~I is currently " + i);
                // I want to check to see if the index on the card list has already been used
                Console.WriteLine("RIGHT GUESS");
                Console.WriteLine("~~~~~~~~~~~~~~~~~RANDOM NUMBER SELECTED~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + RanNumber.GetType());
                Console.WriteLine("~~~~~~~~~~~~~~~~~CORRECTCARDS[I]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + CorrectCards[i].GetType());
                if(RanNumber.ToString() == CorrectCards[i].ToString())
                {
                    RanNumber = random.Next(0, NumCards);
                    i = 0;
                } else {
                    i++;
                }
            }
            CorrectCards += RanNumber.ToString();
            HttpContext.Session.SetString("ViewedCards", CorrectCards);
            Card ThisCard = AllCards[RanNumber];

            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~CORRECT CARDS~~~~~~~~~~~~~~~~~~~~~~~~~~~"+ CorrectCards);

            return View("BeginQuiz", ThisCard);
        }
    // HANDLING INCORRECT GUESS (GET)
        [HttpGet("gssdw/{IdDeck}")]
        public IActionResult gssdw(int IdDeck)
        {
            int? Session = HttpContext.Session.GetInt32("UserInSession");
            if(Session == null)
            {
                return RedirectToAction("Login");
            }
            List<Card> AllCards = dbContext.Cards
                .Where(D => D.DeckId == IdDeck)
                .ToList();
            int NumCards = AllCards.Count();
            Random random = new Random();
            int RanNumber = random.Next(0, NumCards);
            string CorrectCards = HttpContext.Session.GetString("ViewedCards");
            CorrectCards = CorrectCards.Remove(CorrectCards.Length - 1);
            // If statement checking to see if all cards have been used
            if(AllCards.Count() == CorrectCards.Length)
            {
                return RedirectToAction("Home");
            }
            // While loop checking to see if random number ID has already been used
            int i = 0;
            while(i < CorrectCards.Length)
            {   
                Console.WriteLine("~~~~~~~~~~~~~~~~~I is currently " + i);
                // I want to check to see if the index on the card list has already been used
                Console.WriteLine("WRONG GUESS");
                Console.WriteLine("~~~~~~~~~~~~~~~~~RANDOM NUMBER SELECTED~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + RanNumber.GetType());
                Console.WriteLine("~~~~~~~~~~~~~~~~~CORRECTCARDS[I]~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + CorrectCards[i].GetType());

                if(RanNumber.ToString() == CorrectCards[i].ToString())
                {
                    RanNumber = random.Next(0, NumCards);
                    i = 0;
                } else {
                    i++;
                }
            }
            CorrectCards += RanNumber.ToString();
            HttpContext.Session.SetString("ViewedCards", CorrectCards);
            Card ThisCard = AllCards[RanNumber];

            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~CORRECT CARDS~~~~~~~~~~~~~~~~~~~~~~~~~~~"+ CorrectCards);

            return View("BeginQuiz", ThisCard);  
        }
    }
}