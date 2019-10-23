﻿using Microsoft.EntityFrameworkCore;
using QuizApplication.Database.Models;

namespace QuizApplication.Database
{
    public partial class QuizDbContext : CommonDbContext
    {
        public QuizDbContext(DbContextOptions<QuizDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<QuizResponse> QuizResponse { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=QuizApplicationDb.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.Property(e => e.Text)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.HasOne(d => d.Question)
                      .WithMany(p => p.Answer)
                      .HasForeignKey(d => d.QuestionId)
                      .HasConstraintName("FK_Question_Answer");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Text)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.HasOne(d => d.CorrectAnswer)
                      .WithMany(p => p.QuestionNavigation)
                      .HasForeignKey(d => d.CorrectAnswerId)
                      .HasConstraintName("FK_Answer_Question");

                entity.HasOne(d => d.Quiz)
                      .WithMany(p => p.Question)
                      .HasForeignKey(d => d.QuizId)
                      .HasConstraintName("FK_Quiz_Question");
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(256);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
