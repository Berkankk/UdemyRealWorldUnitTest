
using Microsoft.EntityFrameworkCore;
using UdemyRealWorldUnitTest.WEB.Models;

namespace UdemyRealWorldUnitTest.WEB.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class //Where koşulu ile mutlaka bir class olması gerektiği yazdık
    {
        //Irepository interfaceni implement edeceğimz sınıfımız 

        private readonly UdemyUnitTestDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(UdemyUnitTestDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task Create(TEntity entity)
        {
            await _dbSet.AddAsync(entity); //Entityden gelen değeri ekle
            await _context.SaveChangesAsync(); //ve kaydet 
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity); //Entitde olanı sil
            _context.SaveChanges(); //kaydet async metotu yoktur 
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            _context.Update(entity).State = EntityState.Modified; //memoryde değişmiş bir data olduğundan bahsettik

            //_dbSet.Update(entity);
            _context.SaveChanges();
        }
    }
}
