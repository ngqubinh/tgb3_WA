using Application.Interfaces.Management;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Management
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly ShelkobyPutDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericService(ShelkobyPutDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    throw new Exception($"Unable to find {id}");
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var idObject = await _dbSet.FindAsync(id);
            if (idObject == null)
            {
                throw new Exception("No id object found");
            }

            return idObject;
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
