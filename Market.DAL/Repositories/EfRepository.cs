using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Market.DAL.EF;
using Market.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Market.DAL.Repositories
{
    internal class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;

        public EfRepository(ApplicationDbContext dbContext)
        {
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity item)
        {
            await _dbSet.AddAsync(item);
        }

        public void Delete(TEntity item)
        {
            _dbSet.Remove(item);
        }

        public async Task<TEntity> GetAsync(int id) => await _dbSet.FindAsync(id);

        public void Update(TEntity item)
        {
            _dbSet.Update(item);
        }

        public IEnumerator<TEntity> GetEnumerator() => (_dbSet as IQueryable<TEntity>).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (_dbSet as IQueryable<TEntity>).GetEnumerator();

        public Type ElementType => (_dbSet as IQueryable<TEntity>).ElementType;

        public Expression Expression => (_dbSet as IQueryable<TEntity>).Expression;

        public IQueryProvider Provider => (_dbSet as IQueryable<TEntity>).Provider;
    }
}