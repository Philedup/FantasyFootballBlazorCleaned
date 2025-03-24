﻿// <auto-generated />
using System;
using FantasyFootballBlazor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FantasyFootballBlazor.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250127013243_adminservicestables")]
    partial class adminservicestables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FantasyFootballBlazor.Data.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool?>("Admin")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool?>("Paid")
                        .HasColumnType("bit");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaypalEmail")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Survival")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("UserTeamName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.Alert", b =>
                {
                    b.Property<int>("AlertId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AlertId"));

                    b.Property<string>("IndexPageAlert")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MyTeamPageAlert")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AlertId");

                    b.ToTable("Alerts");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.GameType", b =>
                {
                    b.Property<int>("GameTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GameTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GameTypeId");

                    b.ToTable("GameTypes");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.NflWeeks", b =>
                {
                    b.Property<int>("WeekId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WeekId"));

                    b.Property<int>("Name")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("WeekId");

                    b.ToTable("NflWeeks");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.NflYear", b =>
                {
                    b.Property<int>("YearId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("YearId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("YearId");

                    b.ToTable("NFLYear");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("First")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Last")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("LastUpdatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlayerPictureImageUrl")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("PlayerId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.Team", b =>
                {
                    b.Property<int>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeamId"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PlayerId")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.HasKey("TeamId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.TeamSchedule", b =>
                {
                    b.Property<int>("TeamScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TeamScheduleId"));

                    b.Property<int>("AwayTeamId")
                        .HasColumnType("int");

                    b.Property<int?>("AwayTeamScore")
                        .HasColumnType("int");

                    b.Property<DateTime>("GameStartDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("HomeTeamId")
                        .HasColumnType("int");

                    b.Property<int?>("HomeTeamScore")
                        .HasColumnType("int");

                    b.Property<bool?>("TieBreakGame")
                        .HasColumnType("bit");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("TeamScheduleId");

                    b.ToTable("TeamSchedules");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.WeeklyStat", b =>
                {
                    b.Property<int>("WeeklyStatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WeeklyStatId"));

                    b.Property<int?>("DefenseBlockedKicks")
                        .HasColumnType("int");

                    b.Property<int?>("DefenseFumbleRecoveries")
                        .HasColumnType("int");

                    b.Property<int?>("DefenseInterceptions")
                        .HasColumnType("int");

                    b.Property<int?>("DefensePointsAllowed")
                        .HasColumnType("int");

                    b.Property<int?>("DefenseSacks")
                        .HasColumnType("int");

                    b.Property<int?>("DefenseSafeties")
                        .HasColumnType("int");

                    b.Property<int?>("DefenseTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal1")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal2")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal3")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal4")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoal5")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoalMiss1")
                        .HasColumnType("int");

                    b.Property<int?>("FieldGoalMiss2")
                        .HasColumnType("int");

                    b.Property<int?>("FumblesLost")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PassingInterceptions")
                        .HasColumnType("int");

                    b.Property<int?>("PassingSacks")
                        .HasColumnType("int");

                    b.Property<int?>("PassingTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("PassingYards")
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAfterMade")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAfterMiss")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed0")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed1")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed2")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed3")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed4")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed5")
                        .HasColumnType("int");

                    b.Property<int?>("PointsAllowed6")
                        .HasColumnType("int");

                    b.Property<int?>("ReceptionTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("ReceptionYards")
                        .HasColumnType("int");

                    b.Property<int?>("ReturnTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("ReturnYards")
                        .HasColumnType("int");

                    b.Property<int?>("RushingTouchdowns")
                        .HasColumnType("int");

                    b.Property<int?>("RushingYards")
                        .HasColumnType("int");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<int?>("TotalPoints")
                        .HasColumnType("int");

                    b.Property<int?>("TwoPointConversions")
                        .HasColumnType("int");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("WeeklyStatId");

                    b.HasIndex("PlayerId");

                    b.ToTable("WeeklyStats");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.WeeklyUserTeam", b =>
                {
                    b.Property<int>("WeeklyUserTeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WeeklyUserTeamId"));

                    b.Property<int?>("GameTypeId")
                        .HasColumnType("int");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("Week")
                        .HasColumnType("int");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("WeeklyUserTeamId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("UserId");

                    b.ToTable("WeeklyUserTeams");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.WeeklyWinner", b =>
                {
                    b.Property<int>("WeeklyWinnerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WeeklyWinnerId"));

                    b.Property<int?>("Place")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("WeeklyWinnerId");

                    b.HasIndex("UserId");

                    b.ToTable("WeeklyWinner");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.YearEndResults", b =>
                {
                    b.Property<int>("YearEndResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("YearEndResultId"));

                    b.Property<int>("GameTypeId")
                        .HasColumnType("int");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalEarned")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TotalPoints")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("YearEndResultId");

                    b.HasIndex("GameTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("YearEndResults");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.WeeklyStat", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.Models.Entities.Player", "Player")
                        .WithMany("WeeklyStats")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.WeeklyUserTeam", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.Models.Entities.Player", "Player")
                        .WithMany("WeeklyUserTeams")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", "UserProfile")
                        .WithMany("UserPicks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.WeeklyWinner", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.YearEndResults", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.Models.Entities.GameType", "GameType")
                        .WithMany()
                        .HasForeignKey("GameTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameType");

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("FantasyFootballBlazor.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.ApplicationUser", b =>
                {
                    b.Navigation("UserPicks");
                });

            modelBuilder.Entity("FantasyFootballBlazor.Data.Models.Entities.Player", b =>
                {
                    b.Navigation("WeeklyStats");

                    b.Navigation("WeeklyUserTeams");
                });
#pragma warning restore 612, 618
        }
    }
}
