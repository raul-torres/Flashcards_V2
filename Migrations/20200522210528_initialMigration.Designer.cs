﻿// <auto-generated />
using System;
using Flashcard2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flashcard2.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20200522210528_initialMigration")]
    partial class initialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Flashcard2.Models.Card", b =>
                {
                    b.Property<int>("CardId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Answer")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("DeckId");

                    b.Property<string>("Question")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("CardId");

                    b.HasIndex("DeckId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("Flashcard2.Models.Deck", b =>
                {
                    b.Property<int>("DeckId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("DeckName")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<bool>("Shared");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("DeckId");

                    b.HasIndex("UserId");

                    b.ToTable("Decks");
                });

            modelBuilder.Entity("Flashcard2.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Flashcard2.Models.UserDeckFav", b =>
                {
                    b.Property<int>("UserDeckFavId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("DeckId");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("UserDeckFavId");

                    b.HasIndex("DeckId");

                    b.HasIndex("UserId");

                    b.ToTable("UserDeckFavs");
                });

            modelBuilder.Entity("Flashcard2.Models.Card", b =>
                {
                    b.HasOne("Flashcard2.Models.Deck", "DeckSlot")
                        .WithMany("Flashcards")
                        .HasForeignKey("DeckId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Flashcard2.Models.Deck", b =>
                {
                    b.HasOne("Flashcard2.Models.User", "Creator")
                        .WithMany("MyDecks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Flashcard2.Models.UserDeckFav", b =>
                {
                    b.HasOne("Flashcard2.Models.Deck", "Deck")
                        .WithMany("FavoriteBy")
                        .HasForeignKey("DeckId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Flashcard2.Models.User", "User")
                        .WithMany("FavoriteDecks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
