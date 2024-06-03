using ContactManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ContactManager.Data
{
    public class ContactManagerContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public ContactManagerContext(DbContextOptions<ContactManagerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Addresses)
                .WithOne(a => a.Contact)
                .HasForeignKey(a => a.ContactId);

            // Seed data for Contact
            modelBuilder.Entity<Contact>().HasData(
                new Contact
                {
                    ContactId = 1,
                    FirstName = "Sayed",
                    LastName = "Ghoneim"
                },
                new Contact
                {
                    ContactId = 2,
                    FirstName = "Mohamed",
                    LastName = "Ghoneim"
                }
            );

            // Seed data for Address
            modelBuilder.Entity<Address>().HasData(
                new Address
                {
                    AddressId = 1,
                    Street = "123 Main St",
                    City = "Anytown",
                    State = "AN",
                    PostalCode = "12345",
                    ContactId = 1
                },
                new Address
                {
                    AddressId = 2,
                    Street = "456 Oak St",
                    City = "Othertown",
                    State = "OT",
                    PostalCode = "67890",
                    ContactId = 2
                },
                new Address
                {
                    AddressId = 3,
                    Street = "789 Pine St",
                    City = "Sometown",
                    State = "ST",
                    PostalCode = "11223",
                    ContactId = 1
                }
            );
        }

        public void SeedData()
        {
            if (!Contacts.Any())
            {
                Contacts.AddRange(
                    new Contact
                    {
                        ContactId = 1,
                        FirstName = "Sayed",
                        LastName = "Ghoneim",
                        Addresses = new List<Address>
                        {
                            new Address
                            {
                                AddressId = 1,
                                Street = "123 Main St",
                                City = "Anytown",
                                State = "AN",
                                PostalCode = "12345"
                            },
                            new Address
                            {
                                AddressId = 3,
                                Street = "789 Pine St",
                                City = "Sometown",
                                State = "ST",
                                PostalCode = "11223"
                            }
                        }
                    },
                    new Contact
                    {
                        ContactId = 2,
                        FirstName = "Mohamed",
                        LastName = "Ghoneim",
                        Addresses = new List<Address>
                        {
                            new Address
                            {
                                AddressId = 2,
                                Street = "456 Oak St",
                                City = "Othertown",
                                State = "OT",
                                PostalCode = "67890"
                            }
                        }
                    }
                );

                SaveChanges();
            }
        }
    }
}
