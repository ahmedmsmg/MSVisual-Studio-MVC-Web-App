using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Data.Models;

namespace ContactManager.Web.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ContactManagerContext _context;

        public ContactsController(ContactManagerContext context)
        {
            _context = context;
        }

        // GET: Contacts
        public async Task<IActionResult> Index(string searchString)
        {
            var contacts = _context.Contacts.Include(c => c.Address).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c => c.FirstName.Contains(searchString) ||
                                               c.LastName.Contains(searchString) ||
                                               c.Address.Street.Contains(searchString) ||
                                               c.Address.City.Contains(searchString) ||
                                               c.Address.State.Contains(searchString) ||
                                               c.Address.PostalCode.Contains(searchString));
            }

            return View(await contacts.ToListAsync());
        }


        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Address)
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "City");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,FirstName,LastName,Address")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                // Check if a contact with the same FirstName and LastName already exists
                if (_context.Contacts.Any(c => c.FirstName == contact.FirstName && c.LastName == contact.LastName))
                {
                    ModelState.AddModelError(string.Empty, "A contact with the same name already exists.");
                    ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "City", contact.AddressId);
                    return View(contact);
                }

                // Check if the address already exists
                var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a =>
                    a.Street == contact.Address.Street &&
                    a.City == contact.Address.City &&
                    a.State == contact.Address.State &&
                    a.PostalCode == contact.Address.PostalCode);

                if (existingAddress != null)
                {
                    // Use the existing address
                    contact.AddressId = existingAddress.AddressId;
                    contact.Address = null; // Set the Address navigation property to null to avoid creating a new address
                }
                else
                {
                    // Add the new address
                    _context.Addresses.Add(contact.Address);
                    await _context.SaveChangesAsync();
                    contact.AddressId = contact.Address.AddressId;
                }

                // Add the contact
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "City", contact.AddressId);
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                                        .Include(c => c.Address)
                                        .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,FirstName,LastName,Address")] Contact contact)
        {
            if (id != contact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if a contact with the same FirstName and LastName already exists, excluding the current contact being edited
                if (_context.Contacts.Any(c => c.FirstName == contact.FirstName && c.LastName == contact.LastName && c.ContactId != contact.ContactId))
                {
                    ModelState.AddModelError(string.Empty, "A contact with the same name already exists.");
                    return View(contact);
                }

                try
                {
                    // Check if the new address already exists
                    var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a =>
                        a.Street == contact.Address.Street &&
                        a.City == contact.Address.City &&
                        a.State == contact.Address.State &&
                        a.PostalCode == contact.Address.PostalCode);

                    if (existingAddress != null)
                    {
                        // Use the existing address
                        contact.AddressId = existingAddress.AddressId;
                        contact.Address = null; // Set the Address navigation property to null to avoid creating a new address
                    }
                    else
                    {
                        // Add the new address only once
                        var newAddress = new Address
                        {
                            Street = contact.Address.Street,
                            City = contact.Address.City,
                            State = contact.Address.State,
                            PostalCode = contact.Address.PostalCode
                        };
                        _context.Addresses.Add(newAddress);
                        await _context.SaveChangesAsync();
                        contact.AddressId = newAddress.AddressId;
                    }

                    // Update the contact
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ContactId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Address)
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ContactManagerContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
          return (_context.Contacts?.Any(e => e.ContactId == id)).GetValueOrDefault();
        }
    }
}
