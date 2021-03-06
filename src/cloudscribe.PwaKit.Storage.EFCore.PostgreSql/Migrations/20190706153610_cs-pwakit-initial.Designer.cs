﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using cloudscribe.PwaKit.Storage.EFCore.PostgreSql;

namespace cloudscribe.PwaKit.Storage.EFCore.PostgreSql.Migrations
{
    [DbContext(typeof(PwaDbContext))]
    [Migration("20190706153610_cs-pwakit-initial")]
    partial class cspwakitinitial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("cloudscribe.PwaKit.Models.PushDeviceSubscription", b =>
                {
                    b.Property<Guid>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("key");

                    b.Property<string>("Auth")
                        .HasColumnName("auth");

                    b.Property<string>("CreatedFromIpAddress")
                        .HasColumnName("created_from_ip_address");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnName("created_utc");

                    b.Property<string>("Endpoint")
                        .HasColumnName("endpoint");

                    b.Property<string>("P256DH")
                        .HasColumnName("p256_dh");

                    b.Property<string>("TenantId")
                        .HasColumnName("tenant_id")
                        .HasMaxLength(50);

                    b.Property<string>("UserAgent")
                        .HasColumnName("user_agent");

                    b.Property<string>("UserId")
                        .HasColumnName("user_id")
                        .HasMaxLength(50);

                    b.HasKey("Key")
                        .HasName("pk_cspwa_push_subscription");

                    b.HasIndex("Endpoint")
                        .HasName("ix_cspwa_push_subscription_endpoint");

                    b.HasIndex("TenantId")
                        .HasName("ix_cspwa_push_subscription_tenant_id");

                    b.HasIndex("UserId")
                        .HasName("ix_cspwa_push_subscription_user_id");

                    b.ToTable("cspwa_push_subscription");
                });
#pragma warning restore 612, 618
        }
    }
}
