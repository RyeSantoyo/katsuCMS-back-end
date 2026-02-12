using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using katsuCMS_backend.Models;
using katsuCMS_backend.Models.DTO.PurchaseOrder;

namespace katsuCMS_backend.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<object>> GetAllPurchaseOrdersAsync();
        Task<object?>GetByIdAsync(int id);
        Task<IEnumerable<object>>GetSupplierAsync();
        Task<IEnumerable<object>>GetProductsAsync();
        Task<IEnumerable<object>>GetSupplierByIdAsync(int id);
        Task<string>GeneratePONumberAsync();
        Task<(bool Success, string Message)> CreatePOAsync(PurchaseOrderDto dto);
        Task<(bool Success, string Message)> UpdatePOAsync(int id, PurchaseOrderStatus newStatus);
        Task<(bool Success, string Message)> DeletePOAsync(int id);
    }
}