using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.PurchaseOrder;
using katsuCMS_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace katsuCMS_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPurchaseOrderService _poService;
        public PurchaseOrderController(ApplicationDbContext context, IPurchaseOrderService poService)
        {
            _context = context;
            _poService = poService;
        }

        #region Get
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetAll()
        {
            var pos = await _poService.GetAllPurchaseOrdersAsync();
            return Ok(new { message = "Purchase Orders retrieved successfully", data = pos });
        }

        [HttpGet("supplier")]

        public async Task<ActionResult> GetSuppliers()
        {
            var suppliers = await _poService.GetSupplierAsync();
            return Ok(new { message = "Suppliers retrieved successfully", data = suppliers });

        }
        [HttpGet("products")]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _poService.GetProductsAsync();
            return Ok(new { message = "Products retrieved successfully", data = products });

        }
        [HttpGet("productsupplier")]
        public async Task<ActionResult> GetProductBySupplier(int id)
        {
            var productSuppliers = await _poService.GetProductSuppliersAsync(id);
            return Ok(new{message = "Product-Supplier relationships retrieved successfully", data = productSuppliers});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            
            var poDto = await _poService.GetPOByIdAsync(id);
            if(poDto==null)
                return NotFound(new {message=$"Purchase Order with ID {id} not found"});

            return Ok(new { message = "Purchase Order retrieved successfully", data = poDto });
        }

        [HttpGet("GeneratePONumber")]
        public async Task<IActionResult> GetNextPONumber()
        {
            var poNumber = await _poService.GeneratePONumberAsync();
            return Ok(new { poNumber });
        }
        #endregion

        #region Post

        [HttpPost]

        public async Task<IActionResult> CreatePO([FromBody] PurchaseOrderDto pDto)
        {
            if (pDto == null) return BadRequest(new { message = "Invalid input: Request body is null" });

            var result = await _poService.CreatePOAsync(pDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return StatusCode(201, new { message = "Purchase Order Created Successfully" });
        }
        #endregion
        #region Patch

        // private bool IsValid(PurchaseOrderStatus current, PurchaseOrderStatus next)
        // {
        //     return current switch
        //     {
        //         PurchaseOrderStatus.Pending => next == PurchaseOrderStatus.Approved || next == PurchaseOrderStatus.Cancelled,
        //         PurchaseOrderStatus.Approved => next == PurchaseOrderStatus.Completed || next == PurchaseOrderStatus.Cancelled,
        //         PurchaseOrderStatus.Completed => next == PurchaseOrderStatus.Completed,
        //         _ => false
        //     };

        // }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] PurchaseOrderUpdateDto dto)
        {
                var po = await _poService.UpdatePOAsync(id, dto.Status);
                if (!po.Success)
                {
                    return BadRequest(new { message = po.Message });
                }
                return Ok(new { message = "Purchase Order status updated successfully" });
        }
        #endregion
        #region Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePo(int id)
        {
            var po = _context.PurchaseOrders
            .Include(po => po.PurchaseOrderDetails)
            .FirstOrDefault(po => po.Id == id);

            if (po == null) return BadRequest("Cannot delete this entry");
            if (po.Status != PurchaseOrderStatus.Pending)
            {
                return BadRequest(new { message = "Cannot delete Purchase Order unless it is Pending." });
            }

            _context.PurchaseOrderDetails.RemoveRange(po.PurchaseOrderDetails);
            _context.PurchaseOrders.Remove(po);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entry succesfully deleted" });
        }
    }
    #endregion
}