namespace Shop.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Shop.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Shop.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Shop.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            // default table values:
            context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole { Name = "Admin" }
            );

            // seed admin user
            if (!context.Users.Any(u => u.UserName == "admin@admin.rs"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "admin@admin.rs", Email = "admin@admin.rs", FirstName = "Admin", LastName = "User" };

                manager.Create(user, "ChangeThis!");
                manager.AddToRole(user.Id, "Admin");
            }


            context.OrderStatus.AddOrUpdate(o => o.Id,
                new OrderStatus { Id = 1, Name = "Open" },
                new OrderStatus { Id = 2, Name = "Canceled by Store" },
                new OrderStatus { Id = 3, Name = "Canceled by Customer" },
                new OrderStatus { Id = 4, Name = "Sent to shipping address" },
                new OrderStatus { Id = 5, Name = "Successfully completed" }
            );


            context.PaymentTypes.AddOrUpdate(p => p.Id,
                new PaymentType
                {
                    Id = 1,
                    Name = "Cash on Delivery",
                    Desc = "Payment method where you don't pay for your item until after you've received it.",
                    IsActive = true
                },
                new PaymentType
                {
                    Id = 2,
                    Name = "PayPal",
                    Desc = "PayPal is the faster, safer way to send money, make an online payment, receive money or set up a merchant account.",
                    IsActive = false
                },
                new PaymentType
                {
                    Id = 3,
                    Name = "Credit or Debit card",
                    Desc = "Make an online payment with your Credit or Debit card number.",
                    IsActive = false
                }
            );


            context.Currencies.AddOrUpdate(c => c.Id,
                new Currency { Id = 1, Name = "Euro", Sign = "€" }
            );
        }
    }
}
