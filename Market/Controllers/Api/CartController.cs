using Market.BLL.Interfaces;
using Market.DAL.Enums;
using Market.Models;
using Market.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase, IDisposable
    {
        private readonly ICartManager _cartManager;

        public CartController(ICartManager cartManager)
        {
            _cartManager = cartManager;
        }

        /// <summary>
        /// Get product line from cart by specified id
        /// </summary>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductLineVM>> Get(int id)
        {
            var line = await _cartManager.ProductLine(id);

            if (line == null)
            {
                return NotFound();
            }

            return Ok(new ProductLineVM
            {
                Id = line.Id,
                Brand = line.Brand,
                Country = line.Country,
                Image = line.Image,
                Name = line.Name,
                Price = line.Price,
                Quantity = line.Quantity,
                Weight = line.Weight
            });
        }

        /// <summary>
        /// Get all products lines from cart
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductLineVM>>> Get()
        {
            var lines = await _cartManager.ProductsLines();

            return Ok(lines.Select(line => new ProductLineVM
            {
                Id = line.Id,
                Brand = line.Brand,
                Country = line.Country,
                Image = line.Image,
                Name = line.Name,
                Price = line.Price,
                Quantity = line.Quantity,
                Weight = line.Weight
            }));
        }

        /// <summary>
        /// Add or remove the specified quantity of product in cart
        /// </summary>
        /// <response code="400">Cart is empty</response>
        /// <response code="404">Product not found <br/> User not found <br/> Product not found in cart</response>
        /// <response code="500">Failed to remove product from cart. <br/> Failed to add product</response>
        [HttpPost, Consumes("application/json")]
        public async Task<ActionResult> Post([FromBody] ChangeLineVM model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            DAL.Results.OperationResult result = null;

            if ("remove".Equals(model.Operation, StringComparison.OrdinalIgnoreCase))
            {
                result = await _cartManager.Remove(model.Id, model.Quantity);
            }
            else if ("add".Equals(model.Operation, StringComparison.OrdinalIgnoreCase))
            {
                result = await _cartManager.Add(model.Id, model.Quantity);
            }
            else
            {
                return BadRequest("Invalid operation");
            }

            return result.Type switch
            {
                ResultType.Warning => StatusCode(404, result.BuildMessage()),
                ResultType.Error => StatusCode(500, result.BuildMessage()),
                ResultType.Info => StatusCode(400, result.BuildMessage()),

                _ => CreatedAtAction(nameof(Get), new { model.Id }),
            };
        }

        /// <summary>
        /// Remove product line from cart
        /// </summary>
        /// <response code="404">User not found</response>
        /// <response code="400">Cart is empty</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _cartManager.RemoveLine(id);

            return result.Type switch
            {
                ResultType.Warning => StatusCode(404, result.BuildMessage()),
                ResultType.Error => StatusCode(500, result.BuildMessage()),
                ResultType.Info => StatusCode(400, result.BuildMessage()),

                _ => Ok(),
            };
        }

        /// <summary>
        /// Clear cart
        /// </summary>\
        /// <response code="404">User not found</response>
        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            var result = await _cartManager.Clear();

            return result.Type switch
            {
                ResultType.Warning => StatusCode(404, result.BuildMessage()),
                ResultType.Error => StatusCode(500, result.BuildMessage()),
                ResultType.Info => StatusCode(400, result.BuildMessage()),

                _ => Ok(),
            };
        }

        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _cartManager.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
