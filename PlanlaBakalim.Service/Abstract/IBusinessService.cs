using PlanlaBakalim.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanlaBakalim.Service.Abstract
{
    public interface IBusinessService: IService<Business>
    {
        bool IsOpenNow(Business business);
        
        // Dashboard metodları
        Task<Business?> GetBusinessByOwnerIdAsync(int ownerId);
        Task<int> GetActiveServicesCountAsync(int businessId);
        Task<int> GetActiveEmployeesCountAsync(int businessId);
        
        // Son eklenen hizmetler ve çalışanlar
        Task<List<PlanlaBakalim.Core.Entities.BusinessService>> GetRecentServicesAsync(int businessId, int take = 3);
        Task<List<Employee>> GetRecentEmployeesAsync(int businessId, int take = 3);
    }
}
