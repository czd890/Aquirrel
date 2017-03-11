using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Aquirrel.EntityFramework.Test;

namespace Aquirrel.EntityFramework.Test.Migrations
{
    [DbContext(typeof(TestDbContext))]
    [Migration("20170311171412_hhh")]
    partial class hhh
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Aquirrel.EntityFramework.Test.ModelA", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32);

                    b.Property<string>("AId")
                        .HasMaxLength(32);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("LastModfiyDate");

                    b.Property<string>("Name")
                        .HasMaxLength(32);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken();

                    b.HasKey("Id");

                    b.HasIndex("AId");

                    b.ToTable("ModelA");
                });

            modelBuilder.Entity("Aquirrel.EntityFramework.Test.ModelB", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32);

                    b.Property<string>("AId")
                        .HasMaxLength(32);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Desc")
                        .HasMaxLength(22);

                    b.Property<DateTime>("LastModfiyDate");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken();

                    b.HasKey("Id");

                    b.ToTable("ModelB");
                });

            modelBuilder.Entity("Aquirrel.EntityFramework.Test.ModelC", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("Date");

                    b.Property<string>("HH")
                        .HasMaxLength(32);

                    b.Property<DateTime>("LastModfiyDate");

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken();

                    b.HasKey("Id");

                    b.ToTable("ModelC");
                });

            modelBuilder.Entity("Aquirrel.EntityFramework.Test.ModelA", b =>
                {
                    b.HasOne("Aquirrel.EntityFramework.Test.ModelB")
                        .WithMany("ModelA")
                        .HasForeignKey("AId");
                });
        }
    }
}
