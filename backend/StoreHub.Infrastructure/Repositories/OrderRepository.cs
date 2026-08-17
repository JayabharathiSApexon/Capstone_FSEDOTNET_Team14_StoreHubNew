using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreHub.Application.Interfaces.Repositories;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;

namespace StoreHub.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(order => order.UserId == userId)
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(order => order.TrackingHistory.OrderBy(t => t.StatusDate))
                .FirstOrDefaultAsync(order => order.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();
        }

        public async Task SaveOrderStatusChangeAsync(Order order, OrderTrackingHistory trackingHistory)
        {
            _context.Orders.Update(order);

            await _context.OrderTrackingHistory.AddAsync(trackingHistory);

            await _context.SaveChangesAsync();
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
