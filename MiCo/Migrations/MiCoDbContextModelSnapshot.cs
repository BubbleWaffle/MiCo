﻿// <auto-generated />
using System;
using MiCo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MiCo.Migrations
{
    [DbContext(typeof(MiCoDbContext))]
    partial class MiCoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MiCo.Models.Bans", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTimeOffset>("ban_date")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("ban_until")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("id_banned_user")
                        .HasColumnType("int");

                    b.Property<int>("id_moderator")
                        .HasColumnType("int");

                    b.Property<string>("reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("id_banned_user");

                    b.HasIndex("id_moderator");

                    b.ToTable("bans");
                });

            modelBuilder.Entity("MiCo.Models.Images", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("id_which_thread")
                        .HasColumnType("int");

                    b.Property<string>("image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("id_which_thread");

                    b.ToTable("images");
                });

            modelBuilder.Entity("MiCo.Models.Likes", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("id_thread")
                        .HasColumnType("int");

                    b.Property<int>("id_user")
                        .HasColumnType("int");

                    b.Property<int>("like_or_dislike")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("id_thread");

                    b.HasIndex("id_user");

                    b.ToTable("likes");
                });

            modelBuilder.Entity("MiCo.Models.Reports", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("id_reported_user")
                        .HasColumnType("int");

                    b.Property<int>("id_reporting_user")
                        .HasColumnType("int");

                    b.Property<string>("reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("report_date")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("id");

                    b.HasIndex("id_reported_user");

                    b.HasIndex("id_reporting_user");

                    b.ToTable("reports");
                });

            modelBuilder.Entity("MiCo.Models.Tags", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("tag")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("tags");
                });

            modelBuilder.Entity("MiCo.Models.ThreadTags", b =>
                {
                    b.Property<int>("id_thread")
                        .HasColumnType("int");

                    b.Property<int>("id_tag")
                        .HasColumnType("int");

                    b.HasKey("id_thread", "id_tag");

                    b.HasIndex("id_tag");

                    b.ToTable("thread_tags");
                });

            modelBuilder.Entity("MiCo.Models.Threads", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTimeOffset>("creation_date")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool>("deleted")
                        .HasColumnType("bit");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("id_OG_thread")
                        .HasColumnType("int");

                    b.Property<int>("id_author")
                        .HasColumnType("int");

                    b.Property<int?>("id_reply")
                        .HasColumnType("int");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("id_OG_thread");

                    b.HasIndex("id_author");

                    b.HasIndex("id_reply");

                    b.ToTable("threads");
                });

            modelBuilder.Entity("MiCo.Models.Users", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<DateTimeOffset>("creation_date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nickname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pfp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("role")
                        .HasColumnType("int");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.ToTable("users");

                    b.HasData(
                        new
                        {
                            id = 1,
                            creation_date = new DateTimeOffset(new DateTime(2024, 1, 3, 17, 44, 44, 101, DateTimeKind.Unspecified).AddTicks(8793), new TimeSpan(0, 0, 0, 0, 0)),
                            email = "admin@mail.mod",
                            login = "admin",
                            nickname = "admin",
                            password = "pEqXLhVK9wB5vMLHGrBypObadOdL1Ud7E2AEsNwMP5uiCyni",
                            role = 1,
                            status = 0
                        });
                });

            modelBuilder.Entity("MiCo.Models.Bans", b =>
                {
                    b.HasOne("MiCo.Models.Users", "banned_user")
                        .WithMany()
                        .HasForeignKey("id_banned_user")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MiCo.Models.Users", "moderator")
                        .WithMany()
                        .HasForeignKey("id_moderator")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("banned_user");

                    b.Navigation("moderator");
                });

            modelBuilder.Entity("MiCo.Models.Images", b =>
                {
                    b.HasOne("MiCo.Models.Threads", "which_thread")
                        .WithMany("thread_images")
                        .HasForeignKey("id_which_thread")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("which_thread");
                });

            modelBuilder.Entity("MiCo.Models.Likes", b =>
                {
                    b.HasOne("MiCo.Models.Threads", "thread")
                        .WithMany()
                        .HasForeignKey("id_thread")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MiCo.Models.Users", "user")
                        .WithMany()
                        .HasForeignKey("id_user")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("thread");

                    b.Navigation("user");
                });

            modelBuilder.Entity("MiCo.Models.Reports", b =>
                {
                    b.HasOne("MiCo.Models.Users", "reported_user")
                        .WithMany()
                        .HasForeignKey("id_reported_user")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("MiCo.Models.Users", "reporting_user")
                        .WithMany()
                        .HasForeignKey("id_reporting_user")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("reported_user");

                    b.Navigation("reporting_user");
                });

            modelBuilder.Entity("MiCo.Models.ThreadTags", b =>
                {
                    b.HasOne("MiCo.Models.Tags", "tag")
                        .WithMany("thread_tags")
                        .HasForeignKey("id_tag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiCo.Models.Threads", "thread")
                        .WithMany("thread_tags")
                        .HasForeignKey("id_thread")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("tag");

                    b.Navigation("thread");
                });

            modelBuilder.Entity("MiCo.Models.Threads", b =>
                {
                    b.HasOne("MiCo.Models.Threads", "OG_thread")
                        .WithMany()
                        .HasForeignKey("id_OG_thread");

                    b.HasOne("MiCo.Models.Users", "author")
                        .WithMany("user_threads")
                        .HasForeignKey("id_author")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MiCo.Models.Threads", "reply")
                        .WithMany()
                        .HasForeignKey("id_reply");

                    b.Navigation("OG_thread");

                    b.Navigation("author");

                    b.Navigation("reply");
                });

            modelBuilder.Entity("MiCo.Models.Tags", b =>
                {
                    b.Navigation("thread_tags");
                });

            modelBuilder.Entity("MiCo.Models.Threads", b =>
                {
                    b.Navigation("thread_images");

                    b.Navigation("thread_tags");
                });

            modelBuilder.Entity("MiCo.Models.Users", b =>
                {
                    b.Navigation("user_threads");
                });
#pragma warning restore 612, 618
        }
    }
}
