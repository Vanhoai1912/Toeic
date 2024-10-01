using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Toeic.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Toeic.DataAccess
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
        public virtual DbSet<Ma_bai_tu_vung> Mabaituvungs { get; set; }
        public virtual DbSet<Noi_dung_bai_tu_vung> Noidungbaituvungs { get; set; }
        public virtual DbSet<Ma_bai_ngu_phap> Mabainguphaps { get; set; }
        public virtual DbSet<Noi_dung_bai_ngu_phap> Noidungbainguphaps { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

           
        }
    }
}
