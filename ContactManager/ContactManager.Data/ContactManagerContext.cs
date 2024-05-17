using ContactManager.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                .HasOne(c => c.Address)
                .WithMany()
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public void SeedData()
        {
            var seedAddresses = new List<Address>
            {
                new Address
                {
                    Street = "123 Main St",
                    City = "Anytown",
                    State = "AN",
                    PostalCode = "12345"
                },
                new Address
                {
                    Street = "456 Oak St",
                    City = "Othertown",
                    State = "OT",
                    PostalCode = "67890"
                }
            };

            var seedContacts = new List<Contact>
            {
                new Contact
                {
                    FirstName = "Sayed",
                    LastName = "Ghoneim",
                    Address = GetOrCreateAddress(seedAddresses.First(a => a.Street == "123 Main St"))
                },
                new Contact
                {
                    FirstName = "Mohamed",
                    LastName = "Ghoneim",
                    Address = GetOrCreateAddress(seedAddresses.First(a => a.Street == "456 Oak St"))
                }
            };

            // Seed contacts
            foreach (var contact in seedContacts)
            {
                if (!Contacts.Any(c => c.FirstName == contact.FirstName && c.LastName == contact.LastName))
                {
                    Contacts.Add(contact);
                }
            }
            SaveChanges();
        }

        private Address GetOrCreateAddress(Address seedAddress)
        {
            var existingAddress = Addresses.FirstOrDefault(a =>
                a.Street == seedAddress.Street &&
                a.City == seedAddress.City &&
                a.State == seedAddress.State &&
                a.PostalCode == seedAddress.PostalCode);

            if (existingAddress != null)
            {
                return existingAddress;
            }
            else
            {
                Addresses.Add(seedAddress);
                SaveChanges();
                return seedAddress;
            }
        }
    }
}
