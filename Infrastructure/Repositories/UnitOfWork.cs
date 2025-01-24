using System;
using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<string, object> _repositories;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<string, object>();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var typeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(typeName))
            {
                return (IGenericRepository<TEntity>)_repositories[typeName];
            }

            var repository = new GenericRepository<TEntity>(_context);
            _repositories.TryAdd(typeName, repository);
            return repository;
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
