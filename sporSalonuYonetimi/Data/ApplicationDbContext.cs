using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SporSalonuYonetimİ.Models;
using SporSalonuYonetimi.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SporSalonuYonetimi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<SporSalonu> SporSalonlari { get; set; }
        public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<Uye> Uyeler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Musaitlik> Musaitlikler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<Sertifika> Sertifikalar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // SporSalonu konfigürasyonu
            modelBuilder.Entity<SporSalonu>(entity =>
            {
                entity.HasIndex(e => e.Ad);
                entity.Property(e => e.OlusturmaTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Antrenor konfigürasyonu
            modelBuilder.Entity<Antrenor>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => new { e.SporSalonuId, e.Aktif });
                entity.Property(e => e.IseBaslamaTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(a => a.SporSalonu)
                    .WithMany(s => s.Antrenorler)
                    .HasForeignKey(a => a.SporSalonuId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Uye konfigürasyonu
            modelBuilder.Entity<Uye>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.UyelikBaslangic).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(u => u.User)
                    .WithOne(au => au.Uye)
                    .HasForeignKey<Uye>(u => u.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Hizmet konfigürasyonu
            modelBuilder.Entity<Hizmet>(entity =>
            {
                entity.HasIndex(e => new { e.SporSalonuId, e.Aktif });
                entity.Property(e => e.OlusturmaTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(h => h.SporSalonu)
                    .WithMany(s => s.Hizmetler)
                    .HasForeignKey(h => h.SporSalonuId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Antrenor)
                    .WithMany(a => a.Hizmetler)
                    .HasForeignKey(h => h.AntrenorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Musaitlik konfigürasyonu
            modelBuilder.Entity<Musaitlik>(entity =>
            {
                entity.HasIndex(e => new { e.AntrenorId, e.Gun, e.Aktif });
                entity.Property(e => e.OlusturmaTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(m => m.Antrenor)
                    .WithMany(a => a.Musaitlikler)
                    .HasForeignKey(m => m.AntrenorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Randevu konfigürasyonu
            modelBuilder.Entity<Randevu>(entity =>
            {
                entity.HasIndex(e => new { e.AntrenorId, e.RandevuTarihi, e.BaslangicSaati });
                entity.HasIndex(e => new { e.UyeId, e.Durum });
                entity.HasIndex(e => new { e.RandevuTarihi, e.Durum });
                entity.Property(e => e.OlusturmaTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(r => r.Uye)
                    .WithMany(u => u.Randevular)
                    .HasForeignKey(r => r.UyeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Antrenor)
                    .WithMany(a => a.Randevular)
                    .HasForeignKey(r => r.AntrenorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Hizmet)
                    .WithMany(h => h.Randevular)
                    .HasForeignKey(r => r.HizmetId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Sertifika konfigürasyonu
            modelBuilder.Entity<Sertifika>(entity =>
            {
                entity.HasIndex(e => new { e.AntrenorId, e.Aktif });

                entity.HasOne(s => s.Antrenor)
                    .WithMany(a => a.Sertifikalar)
                    .HasForeignKey(s => s.AntrenorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ApplicationUser konfigürasyonu
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.KayitTarihi).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
