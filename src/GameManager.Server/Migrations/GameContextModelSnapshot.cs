﻿// <auto-generated />
using System;
using GameManager.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameManager.Server.Migrations
{
    [DbContext(typeof(GameContext))]
    partial class GameContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.3");

            modelBuilder.Entity("GameManager.Server.Models.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CurrentTurnPlayerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("EntryCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastTurnStartTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EntryCode")
                        .IsUnique();

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GameManager.Server.Models.GameOptions", b =>
                {
                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ShareOtherPlayerTrackers")
                        .HasColumnType("INTEGER");

                    b.HasKey("GameId");

                    b.ToTable("GameOptions");
                });

            modelBuilder.Entity("GameManager.Server.Models.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastHeartbeat")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Order")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("GameManager.Server.Models.Tracker", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GameId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("StartingValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Trackers");
                });

            modelBuilder.Entity("GameManager.Server.Models.TrackerValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TrackerId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TrackerId");

                    b.ToTable("TrackerValues");
                });

            modelBuilder.Entity("GameManager.Server.Models.GameOptions", b =>
                {
                    b.HasOne("GameManager.Server.Models.Game", null)
                        .WithOne("Options")
                        .HasForeignKey("GameManager.Server.Models.GameOptions", "GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GameManager.Server.Models.Player", b =>
                {
                    b.HasOne("GameManager.Server.Models.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("GameManager.Server.Models.Tracker", b =>
                {
                    b.HasOne("GameManager.Server.Models.Game", "Game")
                        .WithMany("Trackers")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("GameManager.Server.Models.TrackerValue", b =>
                {
                    b.HasOne("GameManager.Server.Models.Player", "Player")
                        .WithMany("TrackerValues")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameManager.Server.Models.Tracker", "Tracker")
                        .WithMany()
                        .HasForeignKey("TrackerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Tracker");
                });

            modelBuilder.Entity("GameManager.Server.Models.Game", b =>
                {
                    b.Navigation("Options")
                        .IsRequired();

                    b.Navigation("Players");

                    b.Navigation("Trackers");
                });

            modelBuilder.Entity("GameManager.Server.Models.Player", b =>
                {
                    b.Navigation("TrackerValues");
                });
#pragma warning restore 612, 618
        }
    }
}
