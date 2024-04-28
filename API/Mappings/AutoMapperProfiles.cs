using API.Model.Dtos.BlogDto;
using API.Model.Dtos.BrandDto;
using API.Model.Dtos.CartDto;
using API.Model.Dtos.CategoryDto;
using API.Model.Dtos.CommentDto;
using API.Model.Dtos.DiscountDto;
using API.Model.Dtos.InventoryDto;
using API.Model.Dtos.LaptopDto;
using API.Model.Dtos.OrderDto;
using API.Model.Dtos.ProductDto;
using API.Model.Dtos.TabletDto;
using API.Model.Dtos.User;
using API.Model.Entity;
using AutoMapper;

namespace API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Category, CategoryRequest>().ReverseMap();

            CreateMap<Brand, BrandRequest>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<Product, ProductResponse>().ReverseMap();

            CreateMap<Laptop, LaptopRequest>().ReverseMap();

            CreateMap<Laptop, LaptopResponse>().ReverseMap();

            CreateMap<Tablet, TabletRequest>().ReverseMap();

            CreateMap<Tablet, TabletResponse>().ReverseMap();

            CreateMap<ApplicationUser, SignUpRequest>().ReverseMap();

            CreateMap<UserAddress, UserAddressRequest>().ReverseMap();

            CreateMap<UserAddress, UserAddressResponse>().ReverseMap();


            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(d => d.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(d => d.PictureUrls, opt => opt.MapFrom(src => src.Product.PictureUrls))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(d => d.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ReverseMap();

            CreateMap<Cart, CartDto>().ReverseMap();

            CreateMap<Cart, CartResponse>().ReverseMap();

            CreateMap<Order, OrderRequest>()
                .ReverseMap();

            CreateMap<Order, OrderResponse>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.User.Address))
                .ReverseMap();

            CreateMap<UserAddress, UserAddressDto>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailDto>()
                .ReverseMap();

            CreateMap<Discount, DiscountRequest>().ReverseMap();

            CreateMap<Discount, DiscountResponse>().ReverseMap();

            CreateMap<Discount, DiscountProductResponse>().ReverseMap();

            CreateMap<Discount, DiscountUserResponse>().ReverseMap();

            CreateMap<Comment, CommentRequest>().ReverseMap();

            CreateMap<Comment, CommentResponse>().ReverseMap();

            CreateMap<Blog, BlogRequest>().ReverseMap();

            CreateMap<Blog, BlogResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ReverseMap();

            CreateMap<Inventory, InventoryResponse>().ReverseMap();
        }
    }
}
