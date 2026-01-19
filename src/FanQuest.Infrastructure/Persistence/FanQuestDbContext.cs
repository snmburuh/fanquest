using FanQuest.Domain.Entities;
using FanQuest.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FanQuest.Infrastructure.Persistence
{
    public class FanQuestDbContext : DbContext
    {
        public FanQuestDbContext(DbContextOptions<FanQuestDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Quest> Quests => Set<Quest>();
        public DbSet<Challenge> Challenges => Set<Challenge>();
        public DbSet<Participation> Participations => Set<Participation>();
        public DbSet<Completion> Completions => Set<Completion>();
        public DbSet<Reward> Rewards => Set<Reward>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
            });

            // Quest configuration
            modelBuilder.Entity<Quest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.City).HasMaxLength(100).IsRequired();
                entity.Property(e => e.EntryFee).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasMany(e => e.Challenges)
                    .WithOne()
                    .HasForeignKey(c => c.QuestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Challenge configuration (TPH - Table Per Hierarchy)
            modelBuilder.Entity<Challenge>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Type).HasConversion<string>();
                entity.HasDiscriminator(e => e.Type)
                    .HasValue<CheckInChallenge>(ChallengeType.CheckIn)
                    .HasValue<TimedChallenge>(ChallengeType.Timed)
                    .HasValue<ReactionChallenge>(ChallengeType.Reaction);
            });

            modelBuilder.Entity<CheckInChallenge>(entity =>
            {
                entity.Property(e => e.LocationName).HasMaxLength(200);
            });

            // Participation configuration
            modelBuilder.Entity<Participation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.QuestId }).IsUnique();

                entity.HasMany(e => e.Completions)
                    .WithOne()
                    .HasForeignKey(c => c.ParticipationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Completion configuration
            modelBuilder.Entity<Completion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ParticipationId, e.ChallengeId }).IsUnique();
            });

            // Reward configuration
            modelBuilder.Entity<Reward>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Value).HasPrecision(18, 2);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Type).HasConversion<string>();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.MpesaReceipt).HasMaxLength(100);
                entity.HasIndex(e => e.MpesaReceipt);
            });
        }
    }
}
