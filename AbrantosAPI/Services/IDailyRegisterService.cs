using System.Collections.Generic;
using System.Threading.Tasks;
using AbrantosAPI.Models.Register;

namespace AbrantosAPI.Services
{
    public interface IDailyRegisterService
    {
        Task<DailyRegister> Create(DailyRegister register);
        Task<IList<DailyRegister>> Get();
        Task<DailyRegister> Get(long id);
        Task<DailyRegister> Update(DailyRegister register);
        Task Delete(long id);
    }
}