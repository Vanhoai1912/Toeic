using Microsoft.AspNetCore.Identity;
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
        public virtual DbSet<Ma_bai_tap_nge> Mabaitapnges { get; set; }
        public virtual DbSet<Cau_hoi_bai_tap_nge> Cauhoibaitapnges { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ma_bai_tap_doc>().HasData(
                new Ma_bai_tap_doc { Id = 1, Part = 5, Tieu_de = "Tiêu đề", FilePath=""},
                new Ma_bai_tap_doc { Id = 2, Part = 6, Tieu_de = "Tiêu đề", FilePath = "" },
                new Ma_bai_tap_doc { Id = 3, Part = 7, Tieu_de = "Tiêu đề", FilePath = "" }
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
            modelBuilder.Entity<Ma_bai_tap_nge>().HasData(
                new Ma_bai_tap_nge { Id = 1, Part = 1, Tieu_de = "Tiêu đề" },
                new Ma_bai_tap_nge { Id = 2, Part = 2, Tieu_de = "Tiêu đề" },
                new Ma_bai_tap_nge { Id = 3, Part = 3, Tieu_de = "Tiêu đề" },
                new Ma_bai_tap_nge { Id = 4, Part = 4, Tieu_de = "Tiêu đề" }

            );
            modelBuilder.Entity<Cau_hoi_bai_tap_nge>().HasData(
                new Cau_hoi_bai_tap_nge
                {
                    Id = 1,
                    Thu_tu_cau = 1,
                    Audio = "",
                    Image = "",
                    Cau_hoi = "H",
                    Dap_an_dung = "A",
                    Dap_an_1 = "A",
                    Dap_an_2 = "B",
                    Dap_an_3 = "C",
                    Dap_an_4 = "D",
                    Giai_thich = "A ĐÚNG",
                    Ma_bai_tap_ngeId = 1
                }
            );
        }
    }
}
