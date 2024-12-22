using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository, IOrderQueries
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Order>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.Products)
                .AsNoTracking()
                .ToListAsync();

            return orders;
        }

        public async Task<Order> GetById(Guid id)
        {
            return (await _context.Orders
                .Include(o => o.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id))!;
        }

        public async Task<Order> Add(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return order;
        }

        public async Task<Order> Update(Order order)
        {
             _context.Orders.Update(order);
             await _context.SaveChangesAsync();
             _context.ChangeTracker.Clear();
            return order;
        }

        public async Task<Order> Delete(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return order;
        }
    }
}