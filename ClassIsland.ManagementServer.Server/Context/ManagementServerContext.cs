using System;
using System.Collections.Generic;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClassIsland.ManagementServer.Server.Context;

public partial class ManagementServerContext : DbContext
{
    public ManagementServerContext(DbContextOptions<ManagementServerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientGroup> ClientGroups { get; set; }

    public virtual DbSet<ObjectUpdate> ObjectUpdates { get; set; }

    public virtual DbSet<ObjectsAssignee> ObjectsAssignees { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<ProfileClassplan> ProfileClassplans { get; set; }

    public virtual DbSet<ProfileClassplanClass> ProfileClassplanClasses { get; set; }

    public virtual DbSet<ProfileGroup> ProfileGroups { get; set; }

    public virtual DbSet<ProfileSubject> ProfileSubjects { get; set; }

    public virtual DbSet<ProfileTimelayout> ProfileTimelayouts { get; set; }

    public virtual DbSet<ProfileTimelayoutTimepoint> ProfileTimelayoutTimepoints { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Cuid).HasName("PRIMARY");

            entity.ToTable("clients");

            entity.HasIndex(e => e.GroupId, "fk_clients_client_groups_1");

            entity.Property(e => e.Cuid)
                .HasMaxLength(36)
                .HasColumnName("cuid");
            entity.Property(e => e.ClassplanVersion)
                .HasDefaultValueSql("'0'")
                .HasColumnName("classplan_version");
            entity.Property(e => e.DefaultSettingsVersion)
                .HasDefaultValueSql("'0'")
                .HasColumnName("defaultSettings_version");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Id)
                .HasColumnType("text")
                .HasColumnName("id");
            entity.Property(e => e.PolicyVersion)
                .HasDefaultValueSql("'0'")
                .HasColumnName("policy_version");
            entity.Property(e => e.RegisterTime)
                .HasColumnType("datetime")
                .HasColumnName("register_time");
            entity.Property(e => e.SubjectsVersion)
                .HasDefaultValueSql("'0'")
                .HasColumnName("subjects_version");
            entity.Property(e => e.TimeLayoutVersion)
                .HasDefaultValueSql("'0'")
                .HasColumnName("timeLayout_version");

            entity.HasOne(d => d.Group).WithMany(p => p.Clients)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_clients_client_groups_1");
        });

        modelBuilder.Entity<ClientGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PRIMARY");

            entity.ToTable("client_groups");

            entity.Property(e => e.GroupId)
                .ValueGeneratedNever()
                .HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ObjectUpdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("object_updates");

            entity.HasIndex(e => e.TargetCuid, "fk_object_updates_clients_1");

            entity.HasIndex(e => e.ObjectId, "fk_object_updates_settings_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ObjectId)
                .HasMaxLength(36)
                .HasColumnName("object_id");
            entity.Property(e => e.ObjectType).HasColumnName("object_type");
            entity.Property(e => e.TargetCuid)
                .HasMaxLength(36)
                .HasColumnName("target_cuid");
            entity.Property(e => e.UpdateTime)
                .HasColumnType("datetime")
                .HasColumnName("update_time");

            entity.HasOne(d => d.TargetCu).WithMany(p => p.ObjectUpdates)
                .HasForeignKey(d => d.TargetCuid)
                .HasConstraintName("fk_object_updates_clients_1");
        });

        modelBuilder.Entity<ObjectsAssignee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("objects_assignees");

            entity.HasIndex(e => e.TargetGroupId, "fk_objects_assignees_client_groups_1");

            entity.HasIndex(e => e.TargetClientCuid, "fk_objects_assignees_clients_1");

            entity.HasIndex(e => e.ObjectId, "fk_objects_assignees_profile_groups_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssigneeType).HasColumnName("assignee_type");
            entity.Property(e => e.ObjectId)
                .HasMaxLength(36)
                .HasColumnName("object_id");
            entity.Property(e => e.ObjectType).HasColumnName("object_type");
            entity.Property(e => e.TargetClientCuid)
                .HasMaxLength(36)
                .HasColumnName("target_client_cuid");
            entity.Property(e => e.TargetClientId)
                .HasMaxLength(36)
                .HasColumnName("target_client_id");
            entity.Property(e => e.TargetGroupId).HasColumnName("target_group_id");

            entity.HasOne(d => d.TargetClientCu).WithMany(p => p.ObjectsAssignees)
                .HasForeignKey(d => d.TargetClientCuid)
                .HasConstraintName("fk_objects_assignees_clients_1");

            entity.HasOne(d => d.TargetGroup).WithMany(p => p.ObjectsAssignees)
                .HasForeignKey(d => d.TargetGroupId)
                .HasConstraintName("fk_objects_assignees_client_groups_1");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("policies");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("json")
                .HasColumnName("content");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProfileClassplan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("profile_classplans");

            entity.HasIndex(e => e.GroupId, "fk_profile_classplans_profile_groups_1");

            entity.HasIndex(e => e.TimeLayoutId, "fk_profile_classplans_profile_timelayouts_1");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.AttachedObjects)
                .HasColumnType("json")
                .HasColumnName("attached_objects");
            entity.Property(e => e.GroupId)
                .HasMaxLength(36)
                .HasColumnName("group_id");
            entity.Property(e => e.IsEnabled).HasColumnName("is_enabled");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TimeLayoutId)
                .HasMaxLength(36)
                .HasColumnName("time_layout_id");
            entity.Property(e => e.WeekDay)
                .HasDefaultValueSql("'0'")
                .HasColumnName("week_day");
            entity.Property(e => e.WeekDiv)
                .HasDefaultValueSql("'0'")
                .HasColumnName("week_div");

            entity.HasOne(d => d.Group).WithMany(p => p.ProfileClassplans)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_profile_classplans_profile_groups_1");

            entity.HasOne(d => d.TimeLayout).WithMany(p => p.ProfileClassplans)
                .HasForeignKey(d => d.TimeLayoutId)
                .HasConstraintName("fk_profile_classplans_profile_timelayouts_1");
        });

        modelBuilder.Entity<ProfileClassplanClass>(entity =>
        {
            entity.HasKey(e => e.InternalId).HasName("PRIMARY");

            entity.ToTable("profile_classplan_classes");

            entity.HasIndex(e => e.ParentId, "fk_profile_classplan_classes_profile_classplans_1");

            entity.HasIndex(e => e.SubjectId, "fk_profile_classplan_classes_profile_subjects_1");

            entity.Property(e => e.InternalId).HasColumnName("internal_id");
            entity.Property(e => e.AttachedObjects)
                .HasColumnType("json")
                .HasColumnName("attached_objects");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.ParentId)
                .HasMaxLength(36)
                .HasColumnName("parent_id");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(36)
                .HasColumnName("subject_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.ProfileClassplanClasses)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("fk_profile_classplan_classes_profile_classplans_1");

            entity.HasOne(d => d.Subject).WithMany(p => p.ProfileClassplanClasses)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("fk_profile_classplan_classes_profile_subjects_1");
        });

        modelBuilder.Entity<ProfileGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("profile_groups");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProfileSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("profile_subjects");

            entity.HasIndex(e => e.GroupId, "fk_profile_subjects_profile_groups_1");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.AttachedObjects)
                .HasColumnType("json")
                .HasColumnName("attached_objects");
            entity.Property(e => e.GroupId)
                .HasMaxLength(36)
                .HasColumnName("group_id");
            entity.Property(e => e.Initials)
                .HasMaxLength(255)
                .HasColumnName("initials");
            entity.Property(e => e.IsOutDoor)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_out_door");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Group).WithMany(p => p.ProfileSubjects)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_profile_subjects_profile_groups_1");
        });

        modelBuilder.Entity<ProfileTimelayout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("profile_timelayouts");

            entity.HasIndex(e => e.GroupId, "fk_profile_timelayouts_profile_groups_1");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.AttachedObjects)
                .HasColumnType("json")
                .HasColumnName("attached_objects");
            entity.Property(e => e.GroupId)
                .HasMaxLength(36)
                .HasColumnName("group_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Group).WithMany(p => p.ProfileTimelayouts)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_profile_timelayouts_profile_groups_1");
        });

        modelBuilder.Entity<ProfileTimelayoutTimepoint>(entity =>
        {
            entity.HasKey(e => e.InternalId).HasName("PRIMARY");

            entity.ToTable("profile_timelayout_timepoint");

            entity.HasIndex(e => e.ParentId, "fk_profile_timelayout_timepoint_profile_timelayouts_1");

            entity.Property(e => e.InternalId).HasColumnName("internal_id");
            entity.Property(e => e.AttachedObjects)
                .HasColumnType("json")
                .HasColumnName("attached_objects");
            entity.Property(e => e.DefaultSubjectId)
                .HasMaxLength(36)
                .HasColumnName("default_subject_id");
            entity.Property(e => e.End)
                .HasColumnType("time")
                .HasColumnName("end");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.IsHideDefault).HasColumnName("is_hide_default");
            entity.Property(e => e.ParentId)
                .HasMaxLength(36)
                .HasColumnName("parent_id");
            entity.Property(e => e.Start)
                .HasColumnType("time")
                .HasColumnName("start");
            entity.Property(e => e.TimeType).HasColumnName("time_type");

            entity.HasOne(d => d.Parent).WithMany(p => p.ProfileTimelayoutTimepoints)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("fk_profile_timelayout_timepoint_profile_timelayouts_1");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("settings");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Settings)
                .HasColumnType("json")
                .HasColumnName("settings");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
