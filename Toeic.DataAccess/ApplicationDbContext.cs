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
        public virtual DbSet<Ma_bai_thi> Mabaithis { get; set; }
        public virtual DbSet<Cau_hoi_bai_thi> Cauhoibaithis { get; set; }
        public virtual DbSet<TestResult> TestResults { get; set; }
        public virtual DbSet<UserAnswer> UserAnswers { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<UserAnswer>()
                .HasOne(u => u.TestResult)
                .WithMany(t => t.UserAnswers)
                .HasForeignKey(u => u.TestResultId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa cascade

            modelBuilder.Entity<UserAnswer>()
                .HasOne(u => u.CauHoi)
                .WithMany()
                .HasForeignKey(u => u.CauHoiId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa cascade

            modelBuilder.Entity<TestResult>()
       .HasOne(tr => tr.Mabaithi) // Mối quan hệ với MaBaiThi
       .WithMany() // Nếu bạn không cần truy cập từ MaBaiThi đến TestResults
       .HasForeignKey(tr => tr.MabaithiId)
       .OnDelete(DeleteBehavior.Cascade); // Cho phép xóa cascade

            modelBuilder.Entity<UserAnswer>()
                .HasOne(u => u.TestResult)
                .WithMany(t => t.UserAnswers)
                .HasForeignKey(u => u.TestResultId)
                .OnDelete(DeleteBehavior.Cascade); // Cho phép xóa cascade
        }

    }
}
