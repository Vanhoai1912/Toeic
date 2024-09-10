﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using ToeicWeb.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ToeicWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Ma_bai_tap_doc> Mabaitapdocs { get; set; }
        public virtual DbSet<Cau_hoi_bai_tap_doc> Cauhoibaitapdocs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ma_bai_tap_doc>().HasData(
                new Ma_bai_tap_doc { Id = 1, Part = "ETS 2024 - TEST 1 - PART 5" },
                new Ma_bai_tap_doc { Id = 2, Part = "ETS 2024 - TEST 1 - PART 6" },
                new Ma_bai_tap_doc { Id = 3, Part = "ETS 2024 - TEST 1 - PART 7" },
                new Ma_bai_tap_doc { Id = 4, Part = "ETS 2024 - TEST 2 - PART 5" },
                new Ma_bai_tap_doc { Id = 5, Part = "ETS 2024 - TEST 2 - PART 6" },
                new Ma_bai_tap_doc { Id = 6, Part = "ETS 2024 - TEST 2 - PART 7" }
            );
            modelBuilder.Entity<Cau_hoi_bai_tap_doc>().HasData(
                new Cau_hoi_bai_tap_doc
                {
                    Id = 1,
                    Cau_hoi = "H",
                    Dap_an_dung = "A",
                    Dap_an_1 = "A",
                    Dap_an_2 = "B",
                    Dap_an_3 = "C",
                    Dap_an_4 = "D",
                    Giai_thich = "A ĐÚNG",
                    Bai_doc = "",
                    Thu_tu_cau = 101,
                    Ma_bai_tap_docId = 1
                }
            );
        }
    }
}
