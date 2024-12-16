using ECommerceCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceInfrastructure.Configurations.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
       new Category { Id = 1, Img = "https://still.az/wp-content/uploads/2021/04/1500-300x300.jpg", Name = "كنب" },
       new Category { Id = 2, Img = "https://st.hzcdn.com/fimgs/e4718fea03ad2931_6592-w186-h135-b1-p10--.jpg", Name = "غرف نوم" },
       new Category { Id = 3, Img = "https://www.shutterstock.com/image-photo/wooden-table-chairs-on-white-600nw-2168176325.jpg", Name = "سفرة" },
       new Category { Id = 4, Img = "https://st.hzcdn.com/fimgs/1cf15b22065759cb_3954-w186-h135-b1-p10--.jpg", Name = "اثاث حدائق" },
       new Category { Id = 5, Img = "https://png.pngtree.com/background/20231117/original/pngtree-vibrant-wooden-educational-toys-on-white-background-perfect-for-kids-picture-image_6297092.jpg", Name = "غرف اطفال" },
       new Category { Id = 6, Img = "https://st.hzcdn.com/fimgs/65a1489406b3edfc_8174-w186-h135-b1-p10--.jpg", Name = "مزنون" }
         );

        }
    }
}
