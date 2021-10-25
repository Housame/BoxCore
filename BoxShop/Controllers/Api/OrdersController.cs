using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BoxShop.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BoxShop.Models;
using AutoMapper;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoxShop.Controllers.Api
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        #region Fields
        BoxShopContext context;
        #endregion

        #region Constructor
        public OrdersController(BoxShopContext context)
        {
            this.context = context;
        }
        #endregion
        // GET: api/values
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            var orders = context.Order.Include(item => item.OrderItem).ToList();
            return orders;          
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]AddOrderVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            context.PersistOrder(model);
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
