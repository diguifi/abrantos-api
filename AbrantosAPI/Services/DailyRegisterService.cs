using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbrantosAPI.Data;
using AbrantosAPI.Models.Register;
using Microsoft.EntityFrameworkCore;

namespace AbrantosAPI.Services
{
    public class DailyRegisterService : IDailyRegisterService
    {
        private readonly AbrantosContext _context;
        public DailyRegisterService(AbrantosContext context)
        {
            _context = context;
        }
        public async Task<DailyRegister> Create(DailyRegister register)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                await _context.DailyRegister.AddAsync(register);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return register;
            }
        }

        public async Task<IList<DailyRegister>> Get()
        {
            var registers = await _context.DailyRegister.ToListAsync();

            if (registers == null)
                throw new NullReferenceException("No registers found");

            return registers;
        }

        public async Task<DailyRegister> Get(long id)
        {
            var register = await _context.DailyRegister.FirstOrDefaultAsync(e => e.Id == id);

            if (register == null)
                throw new KeyNotFoundException("No register was found with the given Id");

            return register;
        }

        public async Task<DailyRegister> Update(DailyRegister register)
        {
            var oldRegister = await Get(register.Id);

            oldRegister.Abrantos = register.Abrantos;
            oldRegister.Date = register.Date;

            _context.DailyRegister.Update(oldRegister);
            await _context.SaveChangesAsync();

            return register;
        }

        public async Task Delete(long id)
        {
            var register = await Get(id);

            _context.DailyRegister.Remove(register);
            await _context.SaveChangesAsync();
        }
    }
}