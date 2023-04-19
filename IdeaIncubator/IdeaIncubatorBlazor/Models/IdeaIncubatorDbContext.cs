/*
 * Copyright (C) 2023 IKTSolution
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Microsoft.EntityFrameworkCore;

namespace IdeaIncubatorBlazor.Models;

public partial class IdeaIncubatorDbContext : DbContext
{
    private IConfigurationSection dbConfig = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables("IDEA_")
        .Build()
        .GetSection("DbConfig");

    public IdeaIncubatorDbContext()
    {
    }

    public IdeaIncubatorDbContext(DbContextOptions<IdeaIncubatorDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatGroup> ChatGroups { get; set; }

    public virtual DbSet<ChatGroupMember> ChatGroupMembers { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<ContentComment> ContentComments { get; set; }
    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentType> ContentTypes { get; set; }

    public virtual DbSet<Idea> Ideas { get; set; }

    public virtual DbSet<IdeaContent> IdeaContents { get; set; }

    public virtual DbSet<IdeaStatus> IdeaStatuses { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserIdea> UserIdeas { get; set; }

    public virtual DbSet<UserIdeaRole> UserIdeaRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(
                "Server=tcp:" + dbConfig.GetValue<string>("Server") + "," + dbConfig.GetValue<string>("ServerPort") + ";" +
                "Initial Catalog=IdeaIncubatorDB;Persist Security Info=False;" +
                "User ID=" + dbConfig.GetValue<string>("UserId") + ";" +
                "Password=" + dbConfig.GetValue<string>("Password") + ";" +
                "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"
            );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatGroupMember>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ChatGroupId });

            entity.ToTable("ChatGroupMember");

            entity.HasOne(d => d.ChatGroup).WithMany()
                .HasForeignKey(d => d.ChatGroupId)
                .HasConstraintName("FK_ChatGroupId_ChatGroup");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserId_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment");

            entity.ToTable("Comment");

            entity.HasOne(d => d.UserIdNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comment__User");
        });

        modelBuilder.Entity<ContentComment>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.CommentId }).HasName("PK_ContentComment");

            entity
                .ToTable("ContentComment");

            entity.HasOne(d => d.Content).WithMany()
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("FK_ContentId_ContentComment");

            entity.HasOne(d => d.Comment).WithMany()
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("FK_CommentId_ContentComment");
        });

        modelBuilder.Entity<ChatGroup>(entity =>
        {
            entity.HasKey(e => e.ChatGroupId).HasName("PK__ChatGrou__43595698B3F9BAD8");

            entity.ToTable("ChatGroup");

            entity.HasOne(d => d.Idea).WithMany(p => p.ChatGroups)
                .HasForeignKey(d => d.IdeaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatGroup_Idea");
        });

        modelBuilder.Entity<ChatGroupMember>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ChatGroupId });
            
            entity.ToTable("ChatGroupMember");

            entity.HasOne(d => d.ChatGroup).WithMany()
                .HasForeignKey(d => d.ChatGroupId)
                .HasConstraintName("FK_ChatGroupId_ChatGroup");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserId_User");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__Content__2907A81E20122D45");

            entity.ToTable("Content");

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ContentTypeNavigation).WithMany(p => p.Contents)
                .HasForeignKey(d => d.ContentType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Content__Content__6FE99F9F");

            entity.HasOne(d => d.WriterNavigation).WithMany(p => p.Contents)
                .HasForeignKey(d => d.Writer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Content__Writer__6FE99F9F");
        });

        modelBuilder.Entity<ContentType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__ContentT__516F03B5FFEECF5F");

            entity.ToTable("ContentType");

            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Idea>(entity =>
        {
            entity.HasKey(e => e.IdeaId).HasName("PK__Idea__FE218203F0F3C6DD");

            entity.ToTable("Idea");

            entity.Property(e => e.CreatedDate).HasColumnType("date");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Keywords)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Ideas)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Idea__Status__71D1E811");
        });

        modelBuilder.Entity<IdeaContent>(entity =>
        {
            entity.HasKey(e => new { e.ContentId, e.IdeaId }).HasName("PK_IdeaContent");

            entity
                .ToTable("IdeaContent");

            entity.HasOne(d => d.Content).WithMany()
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("FK_ContentId_IdeaContent");

            entity.HasOne(d => d.Idea).WithMany()
                .HasForeignKey(d => d.IdeaId)
                .HasConstraintName("FK_IdeaId_IdeaContent");
        });

        modelBuilder.Entity<IdeaStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Status__C8EE20635C70B2AD");

            entity.ToTable("IdeaStatus");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__C87C0C9C1EC7887F");

            entity.ToTable("Message");

            entity.Property(e => e.DateSent).HasColumnType("date");
            entity.Property(e => e.MessageText)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ChatGroup).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatGroupId)
                .HasConstraintName("FK_ChatGroupId_Message");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId_Message");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A5508AB64");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.RoleTitle)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CD32C6CD3");

            entity.ToTable("User");

            entity.HasIndex(e => e.EmailAddress, "UQ__User__49A14740705DDD0C").IsUnique();

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SkillSets)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserIdea>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.IdeaId });

            entity.ToTable("UserIdea");

            entity.HasOne(d => d.Idea).WithMany(p => p.UserIdeas)
                .HasForeignKey(d => d.IdeaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IdeaId");

            entity.HasOne(d => d.User).WithMany(p => p.UserIdeas)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId");
        });

        modelBuilder.Entity<UserIdeaRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.IdeaId, e.RoleId });

            entity.ToTable("UserIdeaRole");

            entity.HasOne(d => d.Idea).WithMany(p => p.UserIdeaRoles)
                .HasForeignKey(d => d.IdeaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IdeaId_UserIdeaRole");

            entity.HasOne(d => d.Role).WithMany(p => p.UserIdeaRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleId_UserIdeaRole");

            entity.HasOne(d => d.User).WithMany(p => p.UserIdeaRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId_UserIdeaRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
