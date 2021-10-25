using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdminPanel.Models.Entities
{
    public partial class AdminHubContext : DbContext
    {
        public virtual DbSet<Athlete> Athlete { get; set; }
        public virtual DbSet<AthleteInTeam> AthleteInTeam { get; set; }
        public virtual DbSet<Competition> Competition { get; set; }
        public virtual DbSet<Competitor> Competitor { get; set; }
        public virtual DbSet<CompetitorResult> CompetitorResult { get; set; }
        public virtual DbSet<CompEvent> CompEvent { get; set; }
        public virtual DbSet<Exercise> Exercise { get; set; }
        public virtual DbSet<Reservation> Reservation { get; set; }
        public virtual DbSet<SubCompetition> SubCompetition { get; set; }
        public virtual DbSet<SubEvent> SubEvent { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer(@"Data Source=boxstoresrv.database.windows.net;Initial Catalog=boxstoredb;Integrated Security=False;User ID=PuppetMaster;Password=!Silencio-&65?!;Connect Timeout=15;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Athlete>(entity =>
            {
                entity.ToTable("Athlete", "ehub");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(450);

                entity.Property(e => e.FirstName).HasMaxLength(450);

                entity.Property(e => e.Gender).HasMaxLength(450);

                entity.Property(e => e.LastName).HasMaxLength(450);

                entity.Property(e => e.Size).HasMaxLength(450);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Athlete)
                    .HasForeignKey<Athlete>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Athlete_ToCompetitor");
            });

            modelBuilder.Entity<AthleteInTeam>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.AthleteId });

                entity.ToTable("Athlete_In_Team", "ehub");

                entity.HasOne(d => d.Athlete)
                    .WithMany(p => p.AthleteInTeam)
                    .HasForeignKey(d => d.AthleteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Athlete_In_Team_ToAthlete");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.AthleteInTeam)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Athlete_In_Team_ToTeam");
            });

            modelBuilder.Entity<Competition>(entity =>
            {
                entity.ToTable("Competition", "ehub");

                entity.Property(e => e.CreatorId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<Competitor>(entity =>
            {
                entity.ToTable("Competitor", "ehub");

                entity.Property(e => e.Gender).HasMaxLength(450);

                entity.Property(e => e.Name).HasMaxLength(450);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<CompetitorResult>(entity =>
            {
                entity.HasKey(e => new { e.SubEventId, e.CompEventId, e.CompetitorId });

                entity.ToTable("Competitor_Result", "ehub");

                entity.Property(e => e.PointScore).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Score).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.WeightScore).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.CompEvent)
                    .WithMany(p => p.CompetitorResult)
                    .HasForeignKey(d => d.CompEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Competitor_Result_ToCompEvent");

                entity.HasOne(d => d.Competitor)
                    .WithMany(p => p.CompetitorResult)
                    .HasForeignKey(d => d.CompetitorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Competitor_Result_ToCompetitor");

                entity.HasOne(d => d.SubEvent)
                    .WithMany(p => p.CompetitorResult)
                    .HasForeignKey(d => d.SubEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Competitor_Result_ToEvent");
            });

            modelBuilder.Entity<CompEvent>(entity =>
            {
                entity.ToTable("CompEvent", "ehub");

                entity.HasOne(d => d.SubCompetition)
                    .WithMany(p => p.CompEvent)
                    .HasForeignKey(d => d.SubCompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompEvent_ToSubCompetition");
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.ToTable("Exercise", "ehub");

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.SubEvent)
                    .WithMany(p => p.Exercise)
                    .HasForeignKey(d => d.SubEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exercise_ToSubEvent");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.SubCompetitionId });

                entity.ToTable("Reservation", "ehub");

                entity.Property(e => e.PaymentSessionUrl).IsRequired();

                entity.Property(e => e.Reference).IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.SubCompetition)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.SubCompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompetitionUserIntersection_ToCompetition");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reservation)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompetitionUserIntersection_ToUser");
            });

            modelBuilder.Entity<SubCompetition>(entity =>
            {
                entity.ToTable("SubCompetition", "ehub");

                entity.Property(e => e.Difficulty)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.SubCompetition)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubComp_ToCompetition");
            });

            modelBuilder.Entity<SubEvent>(entity =>
            {
                entity.ToTable("SubEvent", "ehub");

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CompEvent)
                    .WithMany(p => p.SubEvent)
                    .HasForeignKey(d => d.CompEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubEvent_ToCompEvent");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team", "ehub");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Gender).HasMaxLength(450);

                entity.Property(e => e.TeamName).HasMaxLength(450);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Team)
                    .HasForeignKey<Team>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Team_ToCompetitor");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "ehub");

                entity.Property(e => e.Email).HasMaxLength(450);

                entity.Property(e => e.FirstName).HasMaxLength(450);

                entity.Property(e => e.Gender).HasMaxLength(450);

                entity.Property(e => e.HashId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.LastName).HasMaxLength(450);

                entity.Property(e => e.Location).HasMaxLength(450);

                entity.Property(e => e.Size).HasMaxLength(450);

                entity.Property(e => e.Team).HasMaxLength(450);
            });

            modelBuilder.Entity<UsersToSubCompetitionsAndCompetitors>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.SubCompetitionId, e.CompetitorId });

                entity.ToTable("Users_to_SubCompetitions_and_Competitors", "ehub");

                entity.HasOne(d => d.Competitor)
                    .WithMany(p => p.UsersToSubCompetitionsAndCompetitors)
                    .HasForeignKey(d => d.CompetitorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_to_SubCompetitions_and_Competitors_ToCompetitior");

                entity.HasOne(d => d.SubCompetition)
                    .WithMany(p => p.UsersToSubCompetitionsAndCompetitors)
                    .HasForeignKey(d => d.SubCompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_to_SubCompetitions_and_Competitors_ToSubCompetition");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersToSubCompetitionsAndCompetitors)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_to_SubCompetitions_and_Competitors_ToUser");
            });
        }
    }
}
