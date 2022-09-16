﻿// <auto-generated />
using Files.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Files.Migrations
{
    [DbContext(typeof(FilesDatabaseContext))]
    partial class FilesDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("FilesDatabaseContext")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Files.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Extension")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserExternalId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Files", "FilesDatabaseContext");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Extension = ".mp4",
                            ExternalId = "724df22d-0e4e-4fd1-9e0e-e2cc5cfd8b8d",
                            Name = "Default mp4",
                            Path = "/default.mp4",
                            UserExternalId = "f07af1d1-7b23-445a-ae46-a418799ef6ba"
                        },
                        new
                        {
                            Id = 2,
                            Extension = ".png",
                            ExternalId = "df10063b-7486-4fd1-8ded-acf96cd2dd1f",
                            Name = "Default png",
                            Path = "/default.png",
                            UserExternalId = "f07af1d1-7b23-445a-ae46-a418799ef6ba"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}