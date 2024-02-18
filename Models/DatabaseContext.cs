using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Absence> Absences { get; set; }

    public virtual DbSet<ApiKey> ApiKeys { get; set; }

    public virtual DbSet<BibNumber> BibNumbers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Classification> Classifications { get; set; }

    public virtual DbSet<Disqualification> Disqualifications { get; set; }

    public virtual DbSet<ManualSplitTime> ManualSplitTimes { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerProfile> PlayerProfiles { get; set; }

    public virtual DbSet<PlayerRegistration> PlayerRegistrations { get; set; }

    public virtual DbSet<PrismaMigration> PrismaMigrations { get; set; }

    public virtual DbSet<Race> Races { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SplitTime> SplitTimes { get; set; }

    public virtual DbSet<Stopwatch> Stopwatches { get; set; }

    public virtual DbSet<TimePenalty> TimePenalties { get; set; }

    public virtual DbSet<TimingPoint> TimingPoints { get; set; }

    public virtual DbSet<TimingPointAccessUrl> TimingPointAccessUrls { get; set; }

    public virtual DbSet<TimingPointOrder> TimingPointOrders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=database.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Absence>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.RaceId });

            entity.ToTable("Absence");

            entity.HasIndex(e => new { e.TimingPointId, e.BibNumber }, "Absence_timingPointId_bibNumber_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.BibNumber).HasColumnName("bibNumber");
            entity.Property(e => e.TimingPointId).HasColumnName("timingPointId");

            entity.HasOne(d => d.Race).WithMany(p => p.Absences)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.TimingPoint).WithMany(p => p.Absences)
                .HasForeignKey(d => d.TimingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Player).WithMany(p => p.Absences)
                .HasPrincipalKey(p => new { p.RaceId, p.BibNumber })
                .HasForeignKey(d => new { d.RaceId, d.BibNumber })
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("ApiKey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RaceId).HasColumnName("raceId");

            entity.HasOne(d => d.Race).WithMany(p => p.ApiKeys)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BibNumber>(entity =>
        {
            entity.ToTable("BibNumber");

            entity.HasIndex(e => new { e.RaceId, e.Number }, "BibNumber_raceId_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.RaceId).HasColumnName("raceId");

            entity.HasOne(d => d.Race).WithMany(p => p.BibNumbers)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassificationId).HasColumnName("classificationId");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IsSpecial)
                .HasColumnType("BOOLEAN")
                .HasColumnName("isSpecial");
            entity.Property(e => e.MaxAge).HasColumnName("maxAge");
            entity.Property(e => e.MinAge).HasColumnName("minAge");
            entity.Property(e => e.Name).HasColumnName("name");

            // entity.HasOne(d => d.Classification).WithMany(p => p.Categories).HasForeignKey(d => d.ClassificationId);
        });

        modelBuilder.Entity<Classification>(entity =>
        {
            entity.ToTable("Classification");

            entity.HasIndex(e => new { e.RaceId, e.Name }, "Classification_raceId_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RaceId).HasColumnName("raceId");

            entity.HasOne(d => d.Race).WithMany(p => p.Classifications)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Disqualification>(entity =>
        {
            entity.ToTable("Disqualification");

            entity.HasIndex(e => new { e.RaceId, e.BibNumber }, "Disqualification_raceId_bibNumber_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BibNumber).HasColumnName("bibNumber");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.Reason).HasColumnName("reason");

            entity.HasOne(d => d.Race).WithMany(p => p.Disqualifications)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ManualSplitTime>(entity =>
        {
            entity.ToTable("ManualSplitTime");

            entity.HasIndex(e => new { e.TimingPointId, e.Lap, e.BibNumber }, "ManualSplitTime_timingPointId_lap_bibNumber_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BibNumber).HasColumnName("bibNumber");
            entity.Property(e => e.Lap).HasColumnName("lap");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.Time)
                .HasColumnType("BIGINT")
                .HasColumnName("time");
            entity.Property(e => e.TimingPointId).HasColumnName("timingPointId");

            entity.HasOne(d => d.Race).WithMany(p => p.ManualSplitTimes)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.TimingPoint).WithMany(p => p.ManualSplitTimes)
                .HasForeignKey(d => d.TimingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Player).WithMany(p => p.ManualSplitTimes)
                .HasPrincipalKey(p => new { p.RaceId, p.BibNumber })
                .HasForeignKey(d => new { d.RaceId, d.BibNumber })
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Player");

            entity.HasIndex(e => new { e.RaceId, e.BibNumber }, "Player_raceId_bibNumber_key").IsUnique();

            entity.HasIndex(e => new { e.RaceId, e.PlayerRegistrationId }, "Player_raceId_playerRegistrationId_key").IsUnique();

            entity.HasIndex(e => new { e.RaceId, e.StartTime }, "Player_raceId_startTime_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BibNumber).HasColumnName("bibNumber");
            entity.Property(e => e.ClassificationId).HasColumnName("classificationId");
            entity.Property(e => e.PlayerProfileId).HasColumnName("playerProfileId");
            entity.Property(e => e.PlayerRegistrationId).HasColumnName("playerRegistrationId");
            entity.Property(e => e.PromotedByUserId).HasColumnName("promotedByUserId");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.StartTime).HasColumnName("startTime");

            entity.HasOne(d => d.Classification).WithMany(p => p.Players)
                .HasForeignKey(d => d.ClassificationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.PlayerProfile).WithMany(p => p.Players)
                .HasForeignKey(d => d.PlayerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.PlayerRegistration).WithMany(p => p.Players)
                .HasForeignKey(d => d.PlayerRegistrationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.PromotedByUser).WithMany(p => p.Players)
                .HasForeignKey(d => d.PromotedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Race).WithMany(p => p.Players)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlayerProfile>(entity =>
        {
            entity.ToTable("PlayerProfile");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BirthDate)
                .HasColumnType("DATETIME")
                .HasColumnName("birthDate");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.IcePhoneNumber).HasColumnName("icePhoneNumber");
            entity.Property(e => e.LastName).HasColumnName("lastName");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PhoneNumber).HasColumnName("phoneNumber");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.Team).HasColumnName("team");

            entity.HasOne(d => d.Race).WithMany(p => p.PlayerProfiles)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlayerRegistration>(entity =>
        {
            entity.ToTable("PlayerRegistration");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HasPaid)
                .HasColumnType("BOOLEAN")
                .HasColumnName("hasPaid");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("DATETIME")
                .HasColumnName("paymentDate");
            entity.Property(e => e.PlayerProfileId).HasColumnName("playerProfileId");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.RegistrationDate)
                .HasColumnType("DATETIME")
                .HasColumnName("registrationDate");

            entity.HasOne(d => d.PlayerProfile).WithMany(p => p.PlayerRegistrations)
                .HasForeignKey(d => d.PlayerProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Race).WithMany(p => p.PlayerRegistrations)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PrismaMigration>(entity =>
        {
            entity.ToTable("_prisma_migrations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AppliedStepsCount)
                .HasColumnType("INTEGER UNSIGNED")
                .HasColumnName("applied_steps_count");
            entity.Property(e => e.Checksum).HasColumnName("checksum");
            entity.Property(e => e.FinishedAt)
                .HasColumnType("DATETIME")
                .HasColumnName("finished_at");
            entity.Property(e => e.Logs).HasColumnName("logs");
            entity.Property(e => e.MigrationName).HasColumnName("migration_name");
            entity.Property(e => e.RolledBackAt)
                .HasColumnType("DATETIME")
                .HasColumnName("rolled_back_at");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("current_timestamp")
                .HasColumnType("DATETIME")
                .HasColumnName("started_at");
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.ToTable("Race");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("DATETIME")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EmailTemplate).HasColumnName("emailTemplate");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PlayersLimit).HasColumnName("playersLimit");
            entity.Property(e => e.RegistrationCutoff)
                .HasColumnType("DATETIME")
                .HasColumnName("registrationCutoff");
            entity.Property(e => e.RegistrationEnabled)
                .HasColumnType("BOOLEAN")
                .HasColumnName("registrationEnabled");
            entity.Property(e => e.SportKind).HasColumnName("sportKind");
            entity.Property(e => e.TermsUrl).HasColumnName("termsUrl");
            entity.Property(e => e.WebsiteUrl).HasColumnName("websiteUrl");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("Session");

            entity.HasIndex(e => e.SessionToken, "Session_sessionToken_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Expires)
                .HasColumnType("DATETIME")
                .HasColumnName("expires");
            entity.Property(e => e.SessionToken).HasColumnName("sessionToken");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Valid)
                .HasColumnType("BOOLEAN")
                .HasColumnName("valid");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<SplitTime>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.RaceId });

            entity.ToTable("SplitTime");

            entity.HasIndex(e => new { e.TimingPointId, e.Lap, e.BibNumber }, "SplitTime_timingPointId_lap_bibNumber_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.BibNumber).HasColumnName("bibNumber");
            entity.Property(e => e.Lap).HasColumnName("lap");
            entity.Property(e => e.Time)
                .HasColumnType("BIGINT")
                .HasColumnName("time");
            entity.Property(e => e.TimingPointId).HasColumnName("timingPointId");

            entity.HasOne(d => d.Race).WithMany(p => p.SplitTimes)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.TimingPoint).WithMany(p => p.SplitTimes)
                .HasForeignKey(d => d.TimingPointId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Player).WithMany(p => p.SplitTimes)
                .HasPrincipalKey(p => new { p.RaceId, p.BibNumber })
                .HasForeignKey(d => new { d.RaceId, d.BibNumber })
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Stopwatch>(entity =>
        {
            entity.HasKey(e => e.RaceId);

            entity.ToTable("Stopwatch");

            entity.Property(e => e.RaceId)
                .ValueGeneratedOnAdd()
                .HasColumnName("raceId");
            entity.Property(e => e.State).HasColumnName("state");

            entity.HasOne(d => d.Race).WithOne(p => p.Stopwatch)
                .HasForeignKey<Stopwatch>(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TimePenalty>(entity =>
        {
            entity.ToTable("TimePenalty");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BibNumber).HasColumnName("bibNumber");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasOne(d => d.Race).WithMany(p => p.TimePenalties)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TimingPoint>(entity =>
        {
            entity.ToTable("TimingPoint");

            entity.HasIndex(e => new { e.RaceId, e.Name }, "TimingPoint_raceId_name_key").IsUnique();

            entity.HasIndex(e => new { e.RaceId, e.ShortName }, "TimingPoint_raceId_shortName_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Laps).HasColumnName("laps");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.ShortName).HasColumnName("shortName");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Race).WithMany(p => p.TimingPoints)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TimingPointAccessUrl>(entity =>
        {
            entity.ToTable("TimingPointAccessUrl");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CanAccessOthers)
                .HasColumnType("BOOLEAN")
                .HasColumnName("canAccessOthers");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("DATETIME")
                .HasColumnName("expireDate");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.RaceId).HasColumnName("raceId");
            entity.Property(e => e.TimingPointId).HasColumnName("timingPointId");
            entity.Property(e => e.Token).HasColumnName("token");

            entity.HasOne(d => d.Race).WithMany(p => p.TimingPointAccessUrls)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.TimingPoint).WithMany(p => p.TimingPointAccessUrls)
                .HasForeignKey(d => d.TimingPointId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TimingPointOrder>(entity =>
        {
            entity.HasKey(e => e.RaceId);

            entity.ToTable("TimingPointOrder");

            entity.Property(e => e.RaceId)
                .ValueGeneratedOnAdd()
                .HasColumnName("raceId");
            entity.Property(e => e.Order).HasColumnName("order");

            entity.HasOne(d => d.Race).WithOne(p => p.TimingPointOrder)
                .HasForeignKey<TimingPointOrder>(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "User_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
