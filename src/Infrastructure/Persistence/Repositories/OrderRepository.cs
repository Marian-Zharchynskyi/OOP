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
            foreach (var product in order.Products)
            {
                var existingProduct = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == product.Id);

                if (existingProduct == null)
                {
                    await _context.Products.AddAsync(product);
                }
                else
                {
                    _context.Entry(product).State = EntityState.Modified;
                }
            }

            var existingOrder = _context.Orders.Local.FirstOrDefault(x => x.Id == order.Id);
            if (existingOrder != null)
            {
                _context.Entry(existingOrder).State = EntityState.Detached;
            }
            await _context.Orders.AddAsync(order);

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear(); 
            return order;
        }

        public async Task<Order> Update(Order order)
        {
            foreach (var product in order.Products)
            {
                var existingProduct = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == product.Id);

                if (existingProduct == null)
                {
                    await _context.Products.AddAsync(product);
                }
                else
                {
                    _context.Entry(product).State = EntityState.Modified;
                }
            }

            var existingOrder = _context.Orders.Local.FirstOrDefault(x => x.Id == order.Id);
            if (existingOrder != null)
            {
                _context.Entry(existingOrder).State = EntityState.Detached;
            }
 
            _context.Orders.Update(order);

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return order;
        }

        public async Task<Order> Delete(Order order)
        {
            var existingOrder = _context.Orders.Local.FirstOrDefault(x => x.Id == order.Id);
            if (existingOrder != null)
            {
                _context.Entry(existingOrder).State = EntityState.Detached;
            }
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
            return order;
        }
    }
}