﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using XAM.Models;

#nullable disable

namespace XAM.Migrations
{
    [DbContext(typeof(XamDbContext))]
    [Migration("20231105132021_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("XAM.Models.DataHolder", b =>
                {
                    b.Property<int>("DataHolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DataHolderId"));

                    b.Property<string>("CurrentCocktail")
                        .HasColumnType("text");

                    b.Property<DateTime?>("TimeUntilNextCocktail")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("DataHolderId");

                    b.ToTable("DataHoldersTable");
                });

            modelBuilder.Entity("XAM.Models.Exam", b =>
                {
                    b.Property<int>("ExamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ExamId"));

                    b.Property<int>("ChallengeHighscore")
                        .HasColumnType("integer");

                    b.Property<int?>("DataHolderId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ExamId");

                    b.HasIndex("DataHolderId");

                    b.ToTable("ExamsTable");
                });

            modelBuilder.Entity("XAM.Models.Flashcard", b =>
                {
                    b.Property<int>("FlashcardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FlashcardId"));

                    b.Property<string>("BackText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ExamId")
                        .HasColumnType("integer");

                    b.Property<string>("FrontText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("FlashcardId");

                    b.HasIndex("ExamId");

                    b.ToTable("FlashcardsTable");
                });

            modelBuilder.Entity("XAM.Models.StatisticsHolder", b =>
                {
                    b.Property<int>("StatisticsId")
                        .HasColumnType("integer");

                    b.Property<int>("LifetimeCreatedExamsCounter")
                        .HasColumnType("integer");

                    b.Property<int>("LifetimeCreatedFlashcardsCounter")
                        .HasColumnType("integer");

                    b.Property<int>("TodayChallengesTakenCounter")
                        .HasColumnType("integer");

                    b.Property<int>("TodayCreatedExamsCounter")
                        .HasColumnType("integer");

                    b.Property<int>("TodayCreatedFlashcardsCounter")
                        .HasColumnType("integer");

                    b.Property<int>("TodayHighscoresBeatenCounter")
                        .HasColumnType("integer");

                    b.HasKey("StatisticsId");

                    b.ToTable("StatisticsTable");
                });

            modelBuilder.Entity("XAM.Models.Exam", b =>
                {
                    b.HasOne("XAM.Models.DataHolder", null)
                        .WithMany("Exams")
                        .HasForeignKey("DataHolderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("XAM.Models.Flashcard", b =>
                {
                    b.HasOne("XAM.Models.Exam", null)
                        .WithMany("Flashcards")
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("XAM.Models.StatisticsHolder", b =>
                {
                    b.HasOne("XAM.Models.DataHolder", null)
                        .WithOne("Statistics")
                        .HasForeignKey("XAM.Models.StatisticsHolder", "StatisticsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XAM.Models.DataHolder", b =>
                {
                    b.Navigation("Exams");

                    b.Navigation("Statistics")
                        .IsRequired();
                });

            modelBuilder.Entity("XAM.Models.Exam", b =>
                {
                    b.Navigation("Flashcards");
                });
#pragma warning restore 612, 618
        }
    }
}
