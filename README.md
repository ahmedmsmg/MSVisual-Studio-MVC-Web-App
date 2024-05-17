# MSVisual-Studio-MVC-Web-App

# Overview
The ContactManager web application is designed to manage contacts and their associated addresses. It allows users to create, read, update, and delete contacts. Each contact is associated with an address, and the application supports a fuzzy search functionality to filter contacts based on various fields.

# Features
Create, read, update, and delete contacts.
Manage addresses associated with contacts.
Fuzzy search functionality to filter contacts by various fields.
Real-time search filtering as the user types in the search box.
Technologies Used
ASP.NET Core 7.0
Entity Framework Core
SQL Server
Bootstrap (for styling)
JavaScript (for real-time search filtering)
Setup Instructions
Prerequisites
.NET 7.0 SDK
SQL Server

# Installation
Clone the repository:

git clone https://github.com/ahmedmsmg/MSVisual-Studio-MVC-Web-App.git
cd MSVisual-Studio-MVC-Web-App
Open the solution in Visual Studio.

# Database Setup
Configure the database connection string in appsettings.json in the ContactManager.Web project:

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ContactManagerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
Open the Package Manager Console and run the following commands to apply migrations and seed the database:

Add-Migration InitialCreate
Update-Database

# Project Structure
The project consists of three main parts:

ContactManager.Web: The main ASP.NET Core web application project.
ContactManager.Data: The data access layer, including Entity Framework Core models and DbContext.
ContactManager.Data.Models: The models for contacts and addresses.
Usage
Run the application:

dotnet run --project ContactManager.Web
Open a browser and navigate to https://localhost:5001 to view the application.

# Managing Contacts
To create a new contact, click on the "Create New" button, fill out the form, and submit.
To edit a contact, click on the "Edit" link next to the contact you want to update, modify the fields, and submit.
To delete a contact, click on the "Delete" link next to the contact you want to remove.
To view contact details, click on the "Details" link next to the contact you want to view.
Fuzzy Search Implementation
The fuzzy search functionality allows users to filter contacts based on various fields (FirstName, LastName, Street, City, State, PostalCode) as they type in the search box.

Controller Logic:

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
View Logic:

<p>
    <input type="text" id="searchBox" placeholder="Search..." onkeyup="searchContacts()" />
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.FirstName)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.LastName)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Address.City)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody id="contactsTable">
        @foreach (var item in Model)
        {
            <tr>
                <td>
                    @Html.DisplayFor(modelItem => item.FirstName)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.LastName)
                </td>
                <td>
                    @Html.DisplayFor(modelItem => item.Address.City)
                </td>
                <td>
                    <a asp-action="Edit" asp-route-id="@item.ContactId">Edit</a> |
                    <a asp-action="Details" asp-route-id="@item.ContactId">Details</a> |
                    <a asp-action="Delete" asp-route-id="@item.ContactId">Delete</a>
                </td>
            </tr>
        }
    </tbody>
</table>

@section Scripts {
    <script>
        function searchContacts() {
            var searchString = document.getElementById('searchBox').value;
            var url = '@Url.Action("Index", "Contacts")';
            url = url + '?searchString=' + encodeURIComponent(searchString);
            fetch(url)
                .then(response => response.text())
                .then(html => {
                    var parser = new DOMParser();
                    var doc = parser.parseFromString(html, 'text/html');
                    var newTableBody = doc.getElementById('contactsTable').innerHTML;
                    document.getElementById('contactsTable').innerHTML = newTableBody;
                });
        }
    </script>
}
# Contributing
If you would like to contribute to this project, please follow these steps:

Fork the repository.
Create a new branch: git checkout -b feature-branch-name.
Make your changes and commit them: git commit -m 'Add some feature'.
Push to the branch: git push origin feature-branch-name.
Submit a pull request.
