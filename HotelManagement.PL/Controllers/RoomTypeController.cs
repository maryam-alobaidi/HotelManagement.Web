using HotelManagement.BLL.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Web.Models;
using HotelManagement.Web.Models.ViewModels.RoomTypeModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Controllers
{
    public class RoomTypeController : Controller
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(IRoomTypeService roomTypeService, ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var roomTypes=await _roomTypeService.GetAllRoomTypesAsync();
                if(roomTypes == null || !roomTypes.Any())
                {
                    _logger.LogWarning("No room types found.");
                    return View("NotFound");
                }

                var roomTypeViewModels = roomTypes.Select(rt => new RoomTypeViewModel
                {
                    RoomTypeID = rt.RoomTypeID,
                    TypeName = rt.TypeName,
                    Description = rt.Description,
                    BasePrice = rt.BasePrice,
                    MaxOccupancy = rt.MaxOccupancy,
                    Amenities = rt.Amenities

                }).ToList();

                return View(roomTypeViewModels);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching room types.");
                return View("Error", "An error occurred while fetching room types." );

            }
        }

        // GET: RoomTypeController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Invalid room type ID: {RoomTypeID}", id);
                return NotFound();
            }

            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
            if(roomType == null)
            {
                _logger.LogWarning("Room type with ID {RoomTypeID} not found.", id);
                return NotFound();
            }

            var roomTypeViewModel = new RoomTypeViewModel
            {
                RoomTypeID = roomType.RoomTypeID,
                TypeName = roomType.TypeName,
                Description = roomType.Description,
                BasePrice = roomType.BasePrice,
                MaxOccupancy = roomType.MaxOccupancy,
                Amenities = roomType.Amenities
            };
            return View (roomTypeViewModel);

        }

        // GET: RoomTypeController/Create
        public async Task<IActionResult> Create()
        {
            var roomTypeCreateEditViewModel = new RoomTypeCreateEditViewModel
            {
                RoomTypeID = 0, // For create, ID is usually 0 or not set
                TypeName = string.Empty,
                Description = string.Empty,
                BasePrice = 0.0m,
                MaxOccupancy = 1,
                Amenities = string.Empty
            };

            return View(roomTypeCreateEditViewModel);
        }

        // POST: RoomTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomTypeCreateEditViewModel model)
        {

            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating room type.");
                return View(model);
            }
            var roomType = new RoomType
            {
                RoomTypeID = model.RoomTypeID,
                TypeName = model.TypeName,
                Description = model.Description,
                BasePrice = model.BasePrice,
                MaxOccupancy = model.MaxOccupancy,  
                Amenities = model.Amenities
            };


            try
            {
                int? id=await _roomTypeService.AddNewRoomTypeAsync(roomType);
                if(id == null)
                {
                    _logger.LogError("Failed to create room type.");
                    ModelState.AddModelError("", "Failed to create room type.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Room type created successfully with ID: {RoomTypeID}", id);
                    return RedirectToAction(nameof(Details), new { id = id });
                  
                }

            }
            catch
            {
                _logger.LogError("An error occurred while creating the room type.");
                return View(model);
            }
        }

        // GET: RoomTypeController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Invalid room type ID: {RoomTypeID}", id);
                return NotFound();
            }

            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
            if(roomType == null)
            {
                _logger.LogWarning("Room type with ID {RoomTypeID} not found.", id);
                return NotFound();
            }
            var roomTypeEditViewModel = new RoomTypeCreateEditViewModel
            {
                RoomTypeID = roomType.RoomTypeID,
                TypeName = roomType.TypeName,
                Description = roomType.Description,
                BasePrice = roomType.BasePrice,
                MaxOccupancy = roomType.MaxOccupancy,
                Amenities = roomType.Amenities
            };

            return View(roomTypeEditViewModel);
        }

        // POST: RoomTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoomTypeCreateEditViewModel model)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Invalid room type ID: {RoomTypeID}", id);
                return NotFound();
            }
            if(id != model.RoomTypeID)
            {
                _logger.LogWarning("Room type ID mismatch: {RoomTypeID} vs {ModelRoomTypeID}", id, model.RoomTypeID);
                return BadRequest("Room type ID mismatch.");
            }

            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for editing room type.");
                return View(model);
            }

            var roomType = new RoomType
            {
                RoomTypeID = model.RoomTypeID,
                TypeName = model.TypeName,
                Description = model.Description,
                BasePrice = model.BasePrice,
                MaxOccupancy = model.MaxOccupancy,
                Amenities = model.Amenities
            };


            try
            {
                bool isUpdated = await _roomTypeService.UpdateRoomTypeAsync(roomType);
                if(!isUpdated)
                {
                    _logger.LogError("Failed to update room type with ID: {RoomTypeID}", id);
                    ModelState.AddModelError("", "Failed to update room type.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Room type with ID {RoomTypeID} updated successfully.", id);
                    TempData["SuccessMessage"] = "Room type updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View(model);
            }
        }

        // GET: RoomTypeController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Invalid room type ID: {RoomTypeID}", id);
                return NotFound();
            }


            var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
            if (roomType == null)
            {
                _logger.LogWarning("Room type with ID {RoomTypeID} not found.", id);
                return NotFound();
            }


            var roomTypeViewModel = new RoomTypeViewModel
            {
                RoomTypeID = roomType.RoomTypeID,
                TypeName = roomType.TypeName,
                Description = roomType.Description,
                BasePrice = roomType.BasePrice,
                MaxOccupancy = roomType.MaxOccupancy,
                Amenities = roomType.Amenities
            };

            return View(roomTypeViewModel);
        }

     
        // POST: RoomTypeController/Delete/5
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid room type ID: {RoomTypeID}", id);
                return NotFound();
            }

            try
            {
                bool isDeleted = await _roomTypeService.DeleteRoomTypeAsync(id);
                if (isDeleted)
                {
                    _logger.LogInformation("Room type with ID {RoomTypeID} deleted successfully.", id);
                    TempData["SuccessMessage"] = "Room type deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Failed to delete room type with ID {RoomTypeID}.", id);
                    return BadRequest("Failed to delete room type.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the room type with ID {RoomTypeID}.", id);
                return BadRequest("An error occurred while deleting the room type.");
            }
        }
    }
}
