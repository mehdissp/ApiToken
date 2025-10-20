using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWorkRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task SaveChanges(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
