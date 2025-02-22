﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Toeic.DataAccess;

#nullable disable

namespace Toeic.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.7.24405.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsFromAI")
                        .HasColumnType("bit");

                    b.Property<string>("MessageText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiverId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

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

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator().HasValue("IdentityUser");

                    b.UseTphMappingStrategy();
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

            modelBuilder.Entity("Toeic.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Noi_dung")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ten_bai")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("DATEADD(HOUR, 7, GETUTCDATE())");

                    b.HasKey("Id");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Toeic.Models.Cau_hoi_bai_tap_doc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cau_hoi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_4")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_dung")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Giai_thich")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Giai_thich_bai_doc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image_bai_doc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int>("Ma_bai_tap_docId")
                        .HasColumnType("int");

                    b.Property<int>("Thu_tu_cau")
                        .HasColumnType("int");

                    b.Property<string>("UserAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Ma_bai_tap_docId");

                    b.ToTable("Cauhoibaitapdocs");
                });

            modelBuilder.Entity("Toeic.Models.Cau_hoi_bai_tap_nge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Audio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cau_hoi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_4")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_dung")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Giai_thich")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int>("Ma_bai_tap_ngeId")
                        .HasColumnType("int");

                    b.Property<int>("Thu_tu_cau")
                        .HasColumnType("int");

                    b.Property<string>("Transcript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Ma_bai_tap_ngeId");

                    b.ToTable("Cauhoibaitapnges");
                });

            modelBuilder.Entity("Toeic.Models.Cau_hoi_bai_thi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Audio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cau_hoi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_2")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_3")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_4")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dap_an_dung")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Giai_thich_dap_an")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Ma_bai_thiId")
                        .HasColumnType("int");

                    b.Property<string>("QuestionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Thu_tu_cau")
                        .HasColumnType("int");

                    b.Property<string>("Transcript")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Ma_bai_thiId");

                    b.ToTable("Cauhoibaithis");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_ngu_phap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Noi_dung")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ten_bai")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mabainguphaps");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_tap_doc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ExcelFilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageBDFolderPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Part")
                        .HasColumnType("int");

                    b.Property<string>("Tieu_de")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mabaitapdocs");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_tap_nge", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AudioFolderPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExcelFilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageFolderPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Part")
                        .HasColumnType("int");

                    b.Property<string>("Tieu_de")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mabaitapnges");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_thi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AudioFolderPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExamType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExcelFilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageFolderPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tieu_de")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mabaithis");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_tu_vung", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AudioFolderPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExcelFilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageFolderPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ten_bai")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mabaituvungs");
                });

            modelBuilder.Entity("Toeic.Models.Noi_dung_bai_tu_vung", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Audio")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Ma_bai_tu_vungId")
                        .HasColumnType("int");

                    b.Property<string>("Nghia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phien_am")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("So_thu_tu")
                        .HasColumnType("int");

                    b.Property<string>("Tu_dong_nghia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tu_trai_nghia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tu_vung")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Vi_du")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Ma_bai_tu_vungId");

                    b.ToTable("Noidungbaituvungs");
                });

            modelBuilder.Entity("Toeic.Models.TestResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CorrectAnswers")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<int>("IncorrectAnswers")
                        .HasColumnType("int");

                    b.Property<int>("MabaithiId")
                        .HasColumnType("int");

                    b.Property<int>("SkippedQuestions")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("MabaithiId");

                    b.ToTable("TestResults");
                });

            modelBuilder.Entity("Toeic.Models.UserAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CauHoiId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<int>("TestResultId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CauHoiId");

                    b.HasIndex("TestResultId");

                    b.ToTable("UserAnswers");
                });

            modelBuilder.Entity("Toeic.Models.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.HasOne("Toeic.Models.ApplicationUser", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Toeic.Models.ApplicationUser", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
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
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
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

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Toeic.Models.Cau_hoi_bai_tap_doc", b =>
                {
                    b.HasOne("Toeic.Models.Ma_bai_tap_doc", "Ma_bai_tap_doc")
                        .WithMany("CauHoiBaiTapDocs")
                        .HasForeignKey("Ma_bai_tap_docId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ma_bai_tap_doc");
                });

            modelBuilder.Entity("Toeic.Models.Cau_hoi_bai_tap_nge", b =>
                {
                    b.HasOne("Toeic.Models.Ma_bai_tap_nge", "Ma_bai_tap_nge")
                        .WithMany("CauHoiBaiTapNges")
                        .HasForeignKey("Ma_bai_tap_ngeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ma_bai_tap_nge");
                });

            modelBuilder.Entity("Toeic.Models.Cau_hoi_bai_thi", b =>
                {
                    b.HasOne("Toeic.Models.Ma_bai_thi", "Ma_bai_thi")
                        .WithMany()
                        .HasForeignKey("Ma_bai_thiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ma_bai_thi");
                });

            modelBuilder.Entity("Toeic.Models.Noi_dung_bai_tu_vung", b =>
                {
                    b.HasOne("Toeic.Models.Ma_bai_tu_vung", "Ma_bai_tu_vung")
                        .WithMany("Noidungbaituvungs")
                        .HasForeignKey("Ma_bai_tu_vungId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ma_bai_tu_vung");
                });

            modelBuilder.Entity("Toeic.Models.TestResult", b =>
                {
                    b.HasOne("Toeic.Models.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Toeic.Models.Ma_bai_thi", "Mabaithi")
                        .WithMany()
                        .HasForeignKey("MabaithiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Mabaithi");
                });

            modelBuilder.Entity("Toeic.Models.UserAnswer", b =>
                {
                    b.HasOne("Toeic.Models.Cau_hoi_bai_thi", "CauHoi")
                        .WithMany()
                        .HasForeignKey("CauHoiId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Toeic.Models.TestResult", "TestResult")
                        .WithMany("UserAnswers")
                        .HasForeignKey("TestResultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CauHoi");

                    b.Navigation("TestResult");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_tap_doc", b =>
                {
                    b.Navigation("CauHoiBaiTapDocs");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_tap_nge", b =>
                {
                    b.Navigation("CauHoiBaiTapNges");
                });

            modelBuilder.Entity("Toeic.Models.Ma_bai_tu_vung", b =>
                {
                    b.Navigation("Noidungbaituvungs");
                });

            modelBuilder.Entity("Toeic.Models.TestResult", b =>
                {
                    b.Navigation("UserAnswers");
                });
#pragma warning restore 612, 618
        }
    }
}
