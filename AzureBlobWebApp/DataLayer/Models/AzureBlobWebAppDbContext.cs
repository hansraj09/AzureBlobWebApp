using Microsoft.EntityFrameworkCore;

namespace AzureBlobWebApp.DataLayer.Models;

public partial class AzureBlobWebAppDbContext : DbContext
{
    public AzureBlobWebAppDbContext()
    {
    }

    public AzureBlobWebAppDbContext(DbContextOptions<AzureBlobWebAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Authorization> Authorizations { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }

    public virtual DbSet<Container> Containers { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Authorization>(entity =>
        {
            entity.HasKey(e => e.AuthorizationId).HasName("PK_Authorization_AuthorizationID");

            entity.ToTable("Authorization");

            entity.Property(e => e.AuthorizationId).HasColumnName("AuthorizationID");
            entity.Property(e => e.AuthorizationName).HasMaxLength(20);
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedUserId).HasColumnName("ModifiedUserID");

            entity.HasMany(d => d.Roles).WithMany(p => p.Authorizations)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleAuthorization",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_RoleAuthorization_RoleID"),
                    l => l.HasOne<Authorization>().WithMany()
                        .HasForeignKey("AuthorizationId")
                        .HasConstraintName("FK_RoleAuthorization_AuthorizationID"),
                    j =>
                    {
                        j.HasKey("AuthorizationId", "RoleId").HasName("PK_RoleAuthorization_AuthorizationIDRoleID");
                        j.ToTable("RoleAuthorization");
                        j.IndexerProperty<int>("AuthorizationId").HasColumnName("AuthorizationID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.ConfigId).HasName("PK_Configuration_ConfigID");

            entity.ToTable("Configuration");

            entity.Property(e => e.ConfigId).HasColumnName("ConfigID");
            entity.Property(e => e.ConfigName).HasMaxLength(20);
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedUserId).HasColumnName("ModifiedUserID");
            entity.Property(e => e.ConfigValue).HasMaxLength(500);
        });

        modelBuilder.Entity<Container>(entity =>
        {
            entity.HasKey(e => e.ContainerId).HasName("PK_Container_ContainerID");

            entity.ToTable("Container");

            entity.Property(e => e.ContainerId).HasColumnName("ContainerID");
            entity.Property(e => e.ContainerName).HasMaxLength(50);
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Containers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Container_UserID");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK_File_FileID");

            entity.ToTable("File");

            entity.Property(e => e.FileId).HasColumnName("FileID");
            entity.Property(e => e.ContainerId).HasColumnName("ContainerID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FileName).HasMaxLength(50);
            entity.Property(e => e.IsPublic).HasColumnName("isPublic");
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedUserId).HasColumnName("ModifiedUserID");
            entity.Property(e => e.Size).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Type).HasMaxLength(20);
            entity.Property(e => e.GUID).HasMaxLength(100);

            entity.HasOne(d => d.Container).WithMany(p => p.Files)
                .HasForeignKey(d => d.ContainerId)
                .HasConstraintName("FK_File_ContainerID");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK_RefreshToken_tokenID");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.TokenId).HasColumnName("TokenID");
            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_RefreshToken_UserID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Role_RoleID");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.ModifiedUserId).HasColumnName("ModifiedUserID");
            entity.Property(e => e.RoleName).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User_UserID");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.LastModified).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_UserRole_RoleID"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_UserRole_UserID"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK_UserRole_UserIDRoleID");
                        j.ToTable("UserRole");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
