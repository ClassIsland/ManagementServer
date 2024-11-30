using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClassIsland.ManagementServer.Server.Entities;
using ClassIsland.ManagementServer.Server.Models.Identity;
using ClassIsland.Shared.Models.Management;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassIsland.ManagementServer.Server.Context;

public partial class ManagementServerContext : IdentityDbContext<User>
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

    public virtual DbSet<ProfileClassPlanClass> ProfileClassplanClasses { get; set; }

    public virtual DbSet<ProfileGroup> ProfileGroups { get; set; }

    public virtual DbSet<ProfileSubject> ProfileSubjects { get; set; }

    public virtual DbSet<ProfileTimeLayout> ProfileTimelayouts { get; set; }

    public virtual DbSet<ProfileTimeLayoutTimePoint> ProfileTimelayoutTimepoints { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapJsonConverter(modelBuilder.Entity<ProfileClassplan>().Property(e => e.AttachedObjects));
        MapJsonConverter(modelBuilder.Entity<ProfileSubject>().Property(e => e.AttachedObjects));
        MapJsonConverter(modelBuilder.Entity<ProfileTimeLayout>().Property(e => e.AttachedObjects));
        MapJsonConverter(modelBuilder.Entity<ProfileClassPlanClass>().Property(e => e.AttachedObjects));
        MapJsonConverter(modelBuilder.Entity<ProfileTimeLayoutTimePoint>().Property(e => e.AttachedObjects));
        modelBuilder.Entity<Policy>()
            .Property(e => e.Content)
            .HasColumnType("json")
            .HasConversion(
                x => JsonSerializer.Serialize(x, JsonSerializerOptions.Default) ?? "{}",
                x => JsonSerializer.Deserialize<ManagementPolicy>(x, JsonSerializerOptions.Default) ?? new());
        base.OnModelCreating(modelBuilder);
        return;

        void MapJsonConverter(PropertyBuilder<Dictionary<string, object?>> builder) => builder
            .HasColumnType("json")
            .HasConversion(
                x => JsonSerializer.Serialize(x, JsonSerializerOptions.Default) ?? "{}",
                x => JsonSerializer.Deserialize<Dictionary<string, object?>>(x, JsonSerializerOptions.Default) ?? new());
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
