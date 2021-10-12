﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWebApiPrac.Models;

namespace SalesWebApiPrac.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderlinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderlinesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPut("update/{orderId}")]
        public async Task<IActionResult> UpdateOrderline(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                NotFound();
            }
            //Uses LinQ and places it in order total
            order.Total = (from ol in _context.Orderlines
                           where ol.OrderId == orderId
                           select new
                           {
                               LineTotal = ol.Quantity * ol.Price
                           }).Sum(x => x.LineTotal);
            await _context.SaveChangesAsync();
            return Ok();

        }

        // GET: api/Orderlines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderline>>> GetOrderline()
        {
            return await _context.Orderline.ToListAsync();
        }

        // GET: api/Orderlines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orderline>> GetOrderline(int id)
        {
            var orderline = await _context.Orderline.FindAsync(id);

            if (orderline == null)
            {
                return NotFound();
            }

            return orderline;
        }

        // PUT: api/Orderlines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderline(int id, Orderline orderline)
        {
            if (id != orderline.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderline).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderlineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orderlines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Orderline>> PostOrderline(Orderline orderline)
        {
            _context.Orderline.Add(orderline);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderline", new { id = orderline.Id }, orderline);
        }

        // DELETE: api/Orderlines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderline(int id)
        {
            var orderline = await _context.Orderline.FindAsync(id);
            if (orderline == null)
            {
                return NotFound();
            }

            _context.Orderline.Remove(orderline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderlineExists(int id)
        {
            return _context.Orderline.Any(e => e.Id == id);
        }
    }
}
