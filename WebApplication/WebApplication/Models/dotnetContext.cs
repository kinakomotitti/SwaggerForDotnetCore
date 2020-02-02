using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication.Models
{
    public partial class dotnetContext : DbContext
    {
        public dotnetContext()
        {
        }

        public dotnetContext(DbContextOptions<dotnetContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Nlog> Nlog { get; set; }
        public virtual DbSet<SerialTest> SerialTest { get; set; }
        public virtual DbSet<TransactTest> TransactTest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=dotnet;Username=postgres;Password=dotnet");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nlog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("nlog");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<SerialTest>(entity =>
            {
                entity.HasKey(e => e.BigserialId)
                    .HasName("serial_test_pkey");

                entity.ToTable("serial_test");

                entity.Property(e => e.BigserialId).HasColumnName("bigserial_id");

                entity.Property(e => e.SampleValue)
                    .HasColumnName("sample_value")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<TransactTest>(entity =>
            {
                entity.ToTable("transact_test");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("numeric");

                entity.Property(e => e.Text)
                    .HasColumnName("text")
                    .HasColumnType("character varying");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
