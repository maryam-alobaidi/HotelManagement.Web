using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Web.Models.ViewModels.InvoiceItem;
using HotelManagement.Web.Models.ViewModels.InvoiceModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HotelManagement.Web.Controllers
{
    public class InvoiceItemController : Controller
    {

        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceItemService _invoiceItemService;
        private readonly ILogger<InvoiceItemController> _logger;

        // Corrected constructor with dependency injection
        public InvoiceItemController(
            IInvoiceService invoiceService,
            IInvoiceItemService invoiceItemService,
            ILogger<InvoiceItemController> logger)
        {
            _invoiceService = invoiceService;
            _invoiceItemService = invoiceItemService;
            _logger = logger;
        }


        public async Task<ActionResult> Index()
        {
            var invoiceItems = await _invoiceItemService.GetAllInvoiceItemAsync();
            if (invoiceItems == null || !invoiceItems.Any())
            {
                _logger.LogWarning("No invoice items found.");
                return NotFound("No invoice items available.");
            }

            var invoiceItemViewModels = invoiceItems.Select(i=>new             {
                i.InvoiceItemID,
                i.InvoiceID,
                i.ItemDescription,
                i.Quantity,
                i.UnitPrice,
                i.ItemType,
                i.LineTotal,

            }).ToList();
         

            return View(invoiceItemViewModels);
        }

        // GET: InvoiceItemController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if(id<=0)
            {
                _logger.LogError("Invalid Invoice Item ID: {Id}", id);
                return BadRequest("Invalid Invoice Item ID.");
            }

            InvoiceItem? invoiceItem = await _invoiceItemService.GetInvoiceItemByIdAsync(id);
            if (invoiceItem == null)
            {
                _logger.LogWarning("Invoice Item with ID {Id} not found.", id);
                return NotFound($"Invoice Item with ID {id} not found.");
            }


            var invoiceItemViewModel = new InvoiceItemViewModel
            {
                InvoiceItemID = invoiceItem.InvoiceItemID,
                InvoiceID= invoiceItem.InvoiceID,
                ItemDescription= invoiceItem.ItemDescription,
                Quantity = invoiceItem.Quantity,
                UnitPrice = invoiceItem.UnitPrice,
                ItemType = invoiceItem.ItemType,
                LineTotal = invoiceItem.LineTotal
            };


            return View(invoiceItemViewModel);
        }

        // GET: InvoiceItemController/Create
        public async Task<ActionResult> Create()
        {

            var model = new InvoiceItemCreateEditViewModel
            {
                InvoiceID = 0, // Set default or fetch from context
                ItemDescription = string.Empty,
                Quantity = 0,
                UnitPrice = 0.0m,
                ItemType = 0,
                LineTotal = 0.0m
            };

            await PopulateInvoiceListAsync(model);


            return View(model);
        }

        // POST: InvoiceItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(InvoiceItemCreateEditViewModel  model)
        {

            if(!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid for Invoice Item creation.");
                return View(model);
            }
            if(model.InvoiceID <= 0)
            {
                _logger.LogError("Invalid Invoice ID: {InvoiceID}", model.InvoiceID);
                ModelState.AddModelError("InvoiceID", "Invalid Invoice ID.");
                return View(model);
            }
            if(model.Quantity <= 0)
            {
                _logger.LogError("Invalid Quantity: {Quantity}", model.Quantity);
                ModelState.AddModelError("Quantity", "Quantity must be greater than zero.");
                return View(model);
            }
            if(model.UnitPrice <= 0)
            {
                _logger.LogError("Invalid Unit Price: {UnitPrice}", model.UnitPrice);
                ModelState.AddModelError("UnitPrice", "Unit Price must be greater than zero.");
                return View(model);
            }


            if (model.ItemType < 0)
            {
                _logger.LogError("Invalid Item Type: {ItemType}", model.ItemType);
                ModelState.AddModelError("ItemType", "Invalid Item Type.");
                return View(model);
            }

            var invoiceItem = new InvoiceItem
            {
                InvoiceID = model.InvoiceID,
                ItemDescription = model.ItemDescription,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                ItemType =(InvoiceItemTypeEnum) model.ItemType
            };

            try
            {

                int? id= await _invoiceItemService.AddInvoiceItemAsync(invoiceItem);

                if( id == null)
                {
                    _logger.LogError("Failed to create Invoice Item.");
                    ModelState.AddModelError("", "Failed to create Invoice Item.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Invoice Item created successfully with ID: {Id}", id);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                _logger.LogError("An error occurred while creating the Invoice Item.");
                return View(model);
            }
        }

        // GET: InvoiceItemController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid invoice id for editing :{id} ,Id");
                return BadRequest("Edit :Invalid invoice id for editing.");
            }


            var invoiceItem = await _invoiceItemService.GetInvoiceItemByIdAsync(id);
            if(invoiceItem == null)
            {
                _logger.LogWarning($"Invoice Item with ID {id} not found for editing.");
                return NotFound($"Invoice Item with ID {id} not found.");
            }
            var model = new InvoiceItemCreateEditViewModel
            {
                InvoiceItemID = invoiceItem.InvoiceItemID,
                InvoiceID = invoiceItem.InvoiceID,
                ItemDescription = invoiceItem.ItemDescription,
                Quantity = invoiceItem.Quantity,
                UnitPrice = invoiceItem.UnitPrice,
                ItemType = (InvoiceItemTypeEnum)invoiceItem.ItemType,
                LineTotal = invoiceItem.LineTotal
            };

            return View(model);

        }

        // POST: InvoiceItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceItemCreateEditViewModel model)
        {
            if(id <= 0)
            {
                _logger.LogError($"Invalid invoice id for editing :{id} ,Id");
                return BadRequest("Edit :Invalid invoice id for editing.");
            }
            if(id != model.InvoiceItemID)
            {
                _logger.LogError($"Invoice Item ID mismatch: {id} vs {model.InvoiceItemID}");
                ModelState.AddModelError("", "Invoice Item ID mismatch.");
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid for Invoice Item editing.");
                return View(model);
            }
            if (model.InvoiceID <= 0)
            {
                _logger.LogError("Invalid Invoice ID: {InvoiceID}", model.InvoiceID);
                ModelState.AddModelError("InvoiceID", "Invalid Invoice ID.");
                return View(model);
            }

            if (model.Quantity <= 0)
            {
                _logger.LogError("Invalid Quantity: {Quantity}", model.Quantity);
                ModelState.AddModelError("Quantity", "Quantity must be greater than zero.");
                return View(model);
            }

            if (model.UnitPrice <= 0)
            {
                _logger.LogError("Invalid Unit Price: {UnitPrice}", model.UnitPrice);
                ModelState.AddModelError("UnitPrice", "Unit Price must be greater than zero.");
                return View(model);
            }
            if (model.ItemType < 0)
            {
                _logger.LogError("Invalid Item Type: {ItemType}", model.ItemType);
                ModelState.AddModelError("ItemType", "Invalid Item Type.");
                return View(model);
            }

            var invoiceItem = new InvoiceItem
            {
                InvoiceItemID = (int)model.InvoiceItemID,
                InvoiceID = model.InvoiceID,
                ItemDescription = model.ItemDescription,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                ItemType = (InvoiceItemTypeEnum)model.ItemType
            };

            try
            {

                bool isEdit= await _invoiceItemService.UpdateInvoiceItemAsync(invoiceItem);

                if( !isEdit)
                {
                    _logger.LogError("Failed to update Invoice Item with ID: {Id}", id);
                    ModelState.AddModelError("", "Failed to update Invoice Item.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Invoice Item with ID: {Id} updated successfully.", id);
                    return RedirectToAction(nameof(Index));
                }

                 
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Invoice Item with ID: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the Invoice Item.");
                return View(model);
            }
         
        }

        // GET: InvoiceItemController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Invalid Invoice Item ID: {Id}", id);
                return BadRequest("Invalid Invoice Item ID.");
            }

            InvoiceItem? invoiceItem = await _invoiceItemService.GetInvoiceItemByIdAsync(id);
            if (invoiceItem == null)
            {
                _logger.LogWarning("Invoice Item with ID {Id} not found.", id);
                return NotFound($"Invoice Item with ID {id} not found.");
            }


            var invoiceItemViewModel = new InvoiceItemViewModel
            {
                InvoiceItemID = invoiceItem.InvoiceItemID,
                InvoiceID = invoiceItem.InvoiceID,
                ItemDescription = invoiceItem.ItemDescription,
                Quantity = invoiceItem.Quantity,
                UnitPrice = invoiceItem.UnitPrice,
                ItemType = invoiceItem.ItemType,
                LineTotal = invoiceItem.LineTotal
            };


            return View(invoiceItemViewModel);
        }

        // POST: InvoiceItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id,InvoiceItemViewModel model)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid invoice id for editing :{id} ,Id");
                return BadRequest("Delete :Invalid invoice id for deleting.");
            }
            if (id != model.InvoiceItemID)
            {
                _logger.LogError($"Invoice Item ID mismatch: {id} vs {model.InvoiceItemID}");
                ModelState.AddModelError("Delete", "Invoice Item ID mismatch.");
                return View(model);
            }


            var invoiceItem = new InvoiceItem
            {
                InvoiceID = model.InvoiceID,
                ItemDescription = model.ItemDescription,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                ItemType = (InvoiceItemTypeEnum)model.ItemType
            };

            try
            {

                bool isDeleted = await _invoiceItemService.DeleteInvoiceItemAsync(id);
                if (!isDeleted)
                {
                    _logger.LogError("Failed to delete invoice item with ID: {Id}", id);
                    ModelState.AddModelError("", "Failed to delete invoice item. Please try again.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Invoice item with ID {Id} deleted successfully.", id);
                    return RedirectToAction(nameof(Index));
                }

            }
            catch(Exception ex) 
            {
                _logger.LogError($"{ex.Message}", ex);
                return View(model);
            }
        }

       private async Task PopulateInvoiceListAsync(InvoiceItemCreateEditViewModel model)
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            if (invoices == null || !invoices.Any())
            {
                _logger.LogWarning("No invoices found for dropdown.");
                model.InvoiceList = new List<SelectListItem>();
            }
            else
            {
                model.InvoiceList = invoices.Select(i => new SelectListItem
                {
                    Value = i.InvoiceID.ToString(),
                    Text = i.InvoiceID.ToString() // Assuming you want to display the Invoice ID in the dropdown
                }).ToList();
            }
        }
    }
}
