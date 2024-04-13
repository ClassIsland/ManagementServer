using System;
using System.Collections.Generic;
using ClassIsland.ManagementServer.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ClassIsland.ManagementServer.Server.Context;

public partial class ManagementServerContext : DbContext
{
    public ManagementServerContext()
    {
    }

    public ManagementServerContext(DbContextOptions<ManagementServerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientGroup> ClientGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Cuid)
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 36 });

            entity.ToTable("clients");

            entity.Property(e => e.Cuid)
                .HasColumnType("text")
                .HasColumnName("cuid");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Id)
                .HasColumnType("text")
                .HasColumnName("id");
            entity.Property(e => e.RegisterTime)
                .HasColumnType("datetime")
                .HasColumnName("register_time");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
