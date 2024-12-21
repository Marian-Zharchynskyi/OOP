﻿using Application.Abstraction.Interfaces.Queries;
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
                .AsNoTracking()
                .Include(o => o.Products)
                .ToListAsync();

            return orders;
        }

        public async Task<Order> GetById(Guid id)
        {
            return (await _context.Orders
                .AsNoTracking()
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id))!;
        }

        public async Task<Order> Add(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public async Task<Order> Update(Order order)
        {
             _context.Orders.Update(order);
            return order;
        }

        public async Task<Order> Delete(Order order)
        {
            _context.Orders.Remove(order);
            return order;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}