Karachi Estate Hub
==================

Project Name
------------
Karachi Estate Hub

Technology Stack
----------------
- ASP.NET MVC 5
- .NET Framework 4.8
- Entity Framework 6 Code First
- SQL Server Express / LocalDB
- Razor Views
- HTML, CSS, JavaScript
- Bootstrap 5 CDN
- Tabler Icons CDN

Main Modules
------------
- Public homepage
- Property listings with filters and sorting
- Property detail page with gallery, contact buttons, save button, and inquiry form
- User registration and login
- Agent dashboard
- Add, edit, delete, and manage own properties
- Image upload for property listings
- Saved properties
- Inquiries
- Admin panel for property approval, feature, verify, reject, and user management

User Roles
----------
- Admin: manages users and all properties.
- Agent: adds and manages own properties and views inquiries.
- User: views listings, saves properties, and sends inquiries.
- Visitor: views public active listings.

Database Name
-------------
KarachiEstateHub

Default Login Accounts
----------------------
Admin:
Email: admin@karachiestatehub.pk
Password: Admin123

Agent:
Email: agent@karachiestatehub.pk
Password: Agent123

How To Run The Project
----------------------
1. Open KarachiEstateHub.sln in Visual Studio.
2. Restore NuGet packages if Visual Studio asks.
3. Make sure KarachiEstateHub is the startup project.
4. Check the Web.config connection string named KarachiEstateDbContext.
5. Run migrations/database update if the database is missing.
6. Start the project using IIS Express.

Important Routes
----------------
- / : Homepage
- /Listings : Public property listings
- /Listings?purpose=Sale : Buy properties
- /Listings?purpose=Rent : Rent properties
- /Listings/Details/1 : Property detail page
- /Account/Login : Login
- /Account/Register : Register
- /Listings/Create : Add property
- /Listings/MyProperties : Agent/Admin property management
- /Listings/MySaved : Saved properties
- /Dashboard : Agent dashboard
- /Dashboard/Inquiries : Agent inquiries
- /Admin : Admin dashboard
- /Admin/Properties : Admin property management
- /Admin/Users : Admin user management

Known Limitations
-----------------
- No online payment module.
- No real email sending.
- No Google Maps API integration.
- No real-time chat.
- No property reviews.
- Authentication uses simple Session values for student project simplicity.
- Uploaded images are stored in the local /Uploads/Properties folder.
