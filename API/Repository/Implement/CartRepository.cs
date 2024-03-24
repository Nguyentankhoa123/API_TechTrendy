using API.Data;
using API.Model.Dtos.CartDto;
using API.Model.Entity;
using API.Repositories;
using API.Services.Exceptions;
using API.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Implement
{
    public class CartRepository : ICartRepository
    {
        private readonly StoreContext _storeContext;
        private readonly IMapper _mapper;

        public CartRepository(StoreContext storeContext, IMapper mapper)
        {
            _storeContext = storeContext;
            _mapper = mapper;
        }

        public async Task<CartObjectResponse> AddAsync(string userId, int productId, int quantity)
        {
            var product = await _storeContext.Products.FindAsync(productId);

            if (product == null)
            {
                throw new NotFoundException("Product not found");
            }

            // Load the related Brand data
            await _storeContext.Entry(product)
                .Reference(p => p.Brand)
                .LoadAsync();

            // Load the related Category data
            await _storeContext.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            // Fetch the inventory for the product
            var inventory = await _storeContext.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == productId);

            // If there's not enough stock, throw an exception
            if (inventory == null || inventory.Stock < quantity)
            {
                throw new Exception("Not enough stock for this product.");
            }

            var cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            // Create a cart if the account does not have one
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = productId,
                            Quantity = quantity
                        }
                    }
                };
                _storeContext.Carts.Add(cart);
            }

            // Check if the product already exists in the cart
            else
            {
                var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });
                }
            }

            await _storeContext.SaveChangesAsync();


            var response = new CartObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Add to cart",
                Data = _mapper.Map<CartResponse>(cart)
            };

            return response;
        }

        public async Task<CartObjectResponse> GetCartAsync(string userId)
        {
            var cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                throw new NotFoundException("Cart not found");
            }


            var response = new CartObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Get all CartItem",
                Data = _mapper.Map<CartResponse>(cart)
            };
            return response;
        }

        public async Task<CartObjectResponse> DecreaseAsync(string userId, int productId, int quantity)
        {
            var cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                throw new NotFoundException("Cart not found");
            }
            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem == null)
            {
                throw new NotFoundException("CartItem not found");
            }
            if (quantity > existingItem.Quantity)
            {
                throw new BadRequestException("Quantity to remove is greater than existing quantity");
            }
            existingItem.Quantity -= quantity;


            if (existingItem.Quantity < 1)
            {
                throw new BadRequestException("Quantity cannot be less than 1");
            }

            await _storeContext.SaveChangesAsync();

            var response = new CartObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Product quantity has been successfully decreased",
                Data = _mapper.Map<CartResponse>(cart)
            };

            return response;
        }

        public async Task<CartObjectResponse> RemoveAsync(string userId, int productId)
        {
            var cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found");
            }
            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem == null)
            {
                throw new NotFoundException("CartItem not found");
            }

            cart.CartItems.Remove(existingItem);

            if (!cart.CartItems.Any())
            {
                _storeContext.Carts.Remove(cart);
            }


            await _storeContext.SaveChangesAsync();

            var response = new CartObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "Product has been removed from your cart successfully",
                Data = _mapper.Map<CartResponse>(cart)
            };

            return response;
        }

        public async Task<CartObjectResponse> RemoveAllAsync(string userId)
        {
            var cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Brand)
                .FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found");
            }

            cart.CartItems.Clear();

            // If the cart is empty, remove the cart
            if (!cart.CartItems.Any())
            {
                _storeContext.Carts.Remove(cart);
            }

            await _storeContext.SaveChangesAsync();

            var response = new CartObjectResponse
            {
                StatusCode = ResponseCode.OK,
                Message = "All products have been removed from the cart",
                Data = _mapper.Map<CartResponse>(cart)
            };

            return response;
        }
    }
}
