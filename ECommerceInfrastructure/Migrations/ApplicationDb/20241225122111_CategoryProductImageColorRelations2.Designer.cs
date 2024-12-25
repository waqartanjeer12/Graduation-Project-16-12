﻿// <auto-generated />
using ECommerceInfrastructure.Configurations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ECommerceInfrastructure.Migrations.ApplicationDb
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241225122111_CategoryProductImageColorRelations2")]
    partial class CategoryProductImageColorRelations2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Color", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("ECommerceCore.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Img = "https://still.az/wp-content/uploads/2021/04/1500-300x300.jpg",
                            Name = "كنب"
                        },
                        new
                        {
                            Id = 2,
                            Img = "https://st.hzcdn.com/fimgs/e4718fea03ad2931_6592-w186-h135-b1-p10--.jpg",
                            Name = "غرف نوم"
                        },
                        new
                        {
                            Id = 3,
                            Img = "https://www.shutterstock.com/image-photo/wooden-table-chairs-on-white-600nw-2168176325.jpg",
                            Name = "سفرة"
                        },
                        new
                        {
                            Id = 4,
                            Img = "https://st.hzcdn.com/fimgs/1cf15b22065759cb_3954-w186-h135-b1-p10--.jpg",
                            Name = "اثاث حدائق"
                        },
                        new
                        {
                            Id = 5,
                            Img = "https://png.pngtree.com/background/20231117/original/pngtree-vibrant-wooden-educational-toys-on-white-background-perfect-for-kids-picture-image_6297092.jpg",
                            Name = "غرف اطفال"
                        },
                        new
                        {
                            Id = 6,
                            Img = "https://st.hzcdn.com/fimgs/65a1489406b3edfc_8174-w186-h135-b1-p10--.jpg",
                            Name = "مزنون"
                        });
                });

            modelBuilder.Entity("ECommerceCore.Models.ProductColor", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ColorId")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "ColorId");

                    b.HasIndex("ColorId");

                    b.ToTable("ProductColors");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int>("Inventory")
                        .HasColumnType("int");

                    b.Property<string>("MainImage")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<decimal?>("OriginalPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("ECommerceCore.Models.ProductColor", b =>
                {
                    b.HasOne("Color", "Color")
                        .WithMany("ProductColors")
                        .HasForeignKey("ColorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Product", "Product")
                        .WithMany("Colors")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Color");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.HasOne("ECommerceCore.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Product", null)
                        .WithMany("Products")
                        .HasForeignKey("ProductId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ProductImage", b =>
                {
                    b.HasOne("Product", "Product")
                        .WithMany("AdditionalImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Color", b =>
                {
                    b.Navigation("ProductColors");
                });

            modelBuilder.Entity("ECommerceCore.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.Navigation("AdditionalImages");

                    b.Navigation("Colors");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
