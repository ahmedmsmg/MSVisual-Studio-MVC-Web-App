using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Data.Models;
using ContactManager.Web.ViewModels;

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
            var contacts = _context.Contacts.Include(c => c.Addresses).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c => c.FirstName.Contains(searchString) ||
                                               c.LastName.Contains(searchString) ||
                                               c.Addresses.Any(a => a.Street.Contains(searchString) ||
                                                                    a.City.Contains(searchString) ||
                                                                    a.State.Contains(searchString) ||
                                                                    a.PostalCode.Contains(searchString)));
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
                .Include(c => c.Addresses)
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
            var viewModel = new ContactViewModel();
            return View(viewModel);
        }

        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Addresses = viewModel.Addresses.Select(a => new Address
                    {
                        Street = a.Street,
                        City = a.City,
                        State = a.State,
                        PostalCode = a.PostalCode
                    }).ToList()
                };

                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                                        .Include(c => c.Addresses)
                                        .FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }

            var viewModel = new ContactViewModel
            {
                Id = contact.ContactId,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Addresses = contact.Addresses.Select(a => new AddressViewModel
                {
                    Id = a.AddressId,
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode
                }).ToList()
            };

            return View(viewModel);
        }


        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var contactToUpdate = await _context.Contacts
                    .Include(c => c.Addresses)
                    .FirstOrDefaultAsync(c => c.ContactId == id);

                if (contactToUpdate == null)
                {
                    return NotFound();
                }

                // Update contact properties
                contactToUpdate.FirstName = viewModel.FirstName;
                contactToUpdate.LastName = viewModel.LastName;

                
                foreach (var addressViewModel in viewModel.Addresses.Where(a => !a.Delete))
                {
                    if (addressViewModel.Id <= 0)
                    {
                        // Add new address
                        var newAddress = new Address
                        {
                            Street = addressViewModel.Street,
                            City = addressViewModel.City,
                            State = addressViewModel.State,
                            PostalCode = addressViewModel.PostalCode,
                            ContactId = contactToUpdate.ContactId
                        };
                        contactToUpdate.Addresses.Add(newAddress);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contactToUpdate.ContactId))
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

            return View(viewModel);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.Addresses)
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
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public async Task<IActionResult> UpdateAddress([FromBody] AddressViewModel addressViewModel)
        {
            if (ModelState.IsValid)
            {
                var address = await _context.Addresses.FindAsync(addressViewModel.Id);
                if (address != null)
                {
                    address.Street = addressViewModel.Street;
                    address.City = addressViewModel.City;
                    address.State = addressViewModel.State;
                    address.PostalCode = addressViewModel.PostalCode;

                    _context.Update(address);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }



    }
}
