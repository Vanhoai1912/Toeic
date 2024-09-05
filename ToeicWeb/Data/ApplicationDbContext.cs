using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToeicWeb.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ToeicWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Bai_tap_doc> Bai_tap_doc { get; set; }
        public DbSet<Cau_hoi_bai_tap_doc> Cau_hoi_bai_tap_doc { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bai_tap_doc>().HasData(
                new Bai_tap_doc { Id = 1, Part = "ETS 2024 - TEST 1 - PART 5"},
                new Bai_tap_doc { Id = 2, Part = "ETS 2024 - TEST 1 - PART 6"},
                new Bai_tap_doc { Id = 3, Part = "ETS 2024 - TEST 1 - PART 7"}
            );

        }
    }
}
