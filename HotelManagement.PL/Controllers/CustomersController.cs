using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace HotelManagement.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            if (customers == null || !customers.Any())
            {
                return View("no customers");
            }

            var customerViewModels = customers.Select(c => new CustomerViewModel
            {

                CustomerID = c.CustomerID,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                Nationality = c.Nationality,
                IDNumber = c.IDNumber
            }).ToList();

            return View(customerViewModels);

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customerExisting ID.");
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }
            var customerViewModel = new CustomerViewModel
            {

                CustomerID = customer.CustomerID,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Nationality = customer.Nationality,
                IDNumber = customer.IDNumber
            };

            return View(customerViewModel);
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Create: Customer data model was null during POST request.");
                return BadRequest("Customer data is null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create: Model state invalid for customer create. Errors: {Errors}",
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            try
            {
                var customerDomain = new Customer(
                    model.Firstname, model.Lastname, model.Email, model.PhoneNumber,
                    model.Address, model.Nationality, model.IDNumber
                );

                await _customerService.AddCustomerAsync(customerDomain);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
              
                _logger.LogError(ex, "Error creating customer. ViewModel: {@Model}", model); // {@Model} لتسجيل كائن المودل كاملاً
                ModelState.AddModelError("", "An unexpected error occurred while adding the customer. Please try again.");
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel model)
        {
            if (id != model.CustomerID)
            {
                return BadRequest("Customer ID mismatch.");
            }

            if (model == null)
            {
                return BadRequest("Customer data is null.");
            }
            if (ModelState.IsValid)
            {
                return View(model); // If the model state is invalid, return the view with the current model to show validation errors.
            }

            try
            {
                var customer = new Customer(
               customerID: model.CustomerID,
               firstname: model.Firstname,
               lastname: model.Lastname,
               email: model.Email,
               phoneNumber: model.PhoneNumber,
               address: model.Address,
               nationality: model.Nationality,
               iDNumber: model.IDNumber
                );

                // Update the customerExisting using the service
                await _customerService.UpdateCustomerAsync(customer);

                return RedirectToAction(nameof(Index));//"Go and make a new request to the Index action of the current controller."
            }
            catch (Exception ex)
            {
                // Add a generic error message to ModelState for display on the form
                ModelState.AddModelError("", "An unexpected error occurred while saving your changes. Please try again.");

                return View(model); // If an error occurs, return the view with the current model to show validation errors.
            }

        }

        // Action to display the edit form (responds to GET requests)
        [HttpGet] // Optional, as GET is default
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // Map domain model to ViewModel for displaying existing data in the form
            var model = new CustomerViewModel
            {
                CustomerID = customer.CustomerID,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Nationality = customer.Nationality,
                IDNumber = customer.IDNumber
            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            var customerViewModel = new CustomerViewModel
            {
                CustomerID = customer.CustomerID,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Nationality = customer.Nationality,
                IDNumber = customer.IDNumber
            };
            return View(customerViewModel);

        }



        [HttpPost, ActionName("Delete")] // ActionName("Delete") to resolve ambiguity if both methods are named "Delete"
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            try
            {
               
                await _customerService.DeleteCustomerAsync(id);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}"); // For API-like response
            }

            //  Redirect to the Index page after successful deletion
            return RedirectToAction(nameof(Index));
        }


        [HttpGet] //شرح ولماذا ؟؟؟
        public IActionResult Create()
        {
            return View(); // This returns the Create.cshtml view with an empty CustomerViewModel
        }

    }
}