using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models.ViewModels.PaymentMethodModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelManagement.Web.Controllers
{
    public class PaymentMethodController : Controller
    {

        private readonly ILogger<PaymentMethodController> _logger;
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(ILogger<PaymentMethodController> logger, IPaymentMethodService paymentMethodService)
        {
            _logger = logger;
            _paymentMethodService = paymentMethodService;
        }


        // GET: PaymentMethodController
        public async Task<ActionResult> Index()
        {
            var paymentMethods = await _paymentMethodService.GetAllPaymentMethodAsync();

            if (paymentMethods == null || !paymentMethods.Any())
            {
                _logger.LogWarning("No payment methods found.");
                return View("NotFound");
            }

            var viewModel =paymentMethods.Select(pm=> new PaymentMethodViewModel
            {
                MethodID= pm.MethodID,
                MethodName = pm.MethodName,
                Description = pm.Description??string.Empty,
                IsActive = pm.IsActive
            });

            return View(viewModel); 
        }

        // GET: PaymentMethodController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }

            var paymentMethod =await _paymentMethodService.GetPaymentMethodByIdAsync(id);
            if (paymentMethod == null)
            {
                _logger.LogWarning("Payment method with ID {Id} not found.", id);
                return NotFound();
            }

            var viewModel = new PaymentMethodViewModel
            {
                MethodID = paymentMethod.MethodID,
                MethodName = paymentMethod.MethodName,
                Description = paymentMethod.Description ?? string.Empty,
                IsActive = paymentMethod.IsActive
            };

            return View(viewModel); 
        }

        // GET: PaymentMethodController/Create
        public ActionResult Create()
        {
            var paymentModel = new PaymentMethodCreateEditViewModel
            {
                MethodName="",
                Description="",
                IsActive = false
            };
            return View(paymentModel);
        }

        // POST: PaymentMethodController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PaymentMethodCreateEditViewModel model)
        {

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid. Found {Count} validation errors");
                return View(model);
            }

            var paymentMethod = new PaymentMethod
            {
                MethodName = model.MethodName,
                Description = model.Description??string.Empty,
                IsActive = model.IsActive,

            };

            try
            {

                int? Id = await _paymentMethodService.AddPaymentMethodAsync(paymentMethod);
                if (Id == null)
                {
                    _logger.LogError("Create:Faild to add new Payment Method.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Create: Payment method create successfully.");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception ex) 
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                _logger.LogError(ex, "An unexpected error occurred while adding a payment method.");
                return View("Create", model);
            }
        }

        // GET: PaymentMethodController/Edit/5
        public ActionResult Edit(int id)
        {
          if(id<= 0)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }
            var paymentMethod = _paymentMethodService.GetPaymentMethodByIdAsync(id).Result;
            if (paymentMethod == null)
            {
                _logger.LogWarning("Payment method with ID {Id} not found.", id);
                return NotFound();
            }
            var viewModel = new PaymentMethodCreateEditViewModel
            {
                MethodID = paymentMethod.MethodID,
                MethodName = paymentMethod.MethodName,
                Description = paymentMethod.Description ?? string.Empty,
                IsActive = paymentMethod.IsActive
            };
            return View(viewModel);
        }

        // POST: PaymentMethodController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PaymentMethodCreateEditViewModel model)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid. Found {Count} validation errors", ModelState.ErrorCount);
                return View(model);
            }
            if (id <= 0)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }

            if (model.MethodID != id)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }

            var paymentMethod = new PaymentMethod
            {
                MethodID = id,
                MethodName = model.MethodName,
                Description = model.Description ?? string.Empty,
                IsActive = model.IsActive
            };


            try
            {
                bool isUpdate=await _paymentMethodService.UpdatePaymentMethodAsync(paymentMethod);
                if (!isUpdate)
                {
                    _logger.LogError("Update payment method faild.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Update Successfully.");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError($"Update payment method faild ,{ex.Message}.");
                return View(model);
            }
        }

        // GET: PaymentMethodController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
           if(id <= 0)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }
            var paymentMethod =await _paymentMethodService.GetPaymentMethodByIdAsync(id);
            if (paymentMethod == null)
            {
                _logger.LogWarning("Payment method with ID {Id} not found.", id);
                return NotFound();
            }
            var viewModel = new PaymentMethodViewModel
            {
                MethodID = paymentMethod.MethodID,
                MethodName = paymentMethod.MethodName,
                Description = paymentMethod.Description ?? string.Empty,
                IsActive = paymentMethod.IsActive
            };
            return View(viewModel);
        }

        // POST: PaymentMethodController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, PaymentMethodViewModel model)
        {
            if(id <= 0)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }

            if (model.MethodID != id)
            {
                _logger.LogError("Invalid payment method ID: {Id}", id);
                return NotFound();
            }

            try
            {
                bool isDeleted =await _paymentMethodService.DeletePaymentMethodAsync(id);
                if (!isDeleted)
                {
                    _logger.LogError("Delete payment method faild.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Delete Successfully.");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete payment method faild ,{ex.Message}.");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(model);
            }
        }
    }
}
