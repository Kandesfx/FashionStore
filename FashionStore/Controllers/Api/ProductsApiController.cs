//using System.Web.Http;
using FashionStore.Models.Entities;
using FashionStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FashionStore.Controllers.Api
{
    [RoutePrefix("api/products")]
    public class ProductsApiController : ApiController
    {
        private readonly IProductService _productService;

        public ProductsApiController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetProducts(
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? featured = null,
            int page = 1,
            int pageSize = 12)
        {
            try
            {
                IEnumerable<Product> products;

                if (categoryId.HasValue || minPrice.HasValue || maxPrice.HasValue || featured.HasValue)
                {
                    products = _productService.FilterProducts(categoryId, minPrice, maxPrice, featured);
                }
                else
                {
                    int totalCount;
                    products = _productService.GetProductsWithPaging(page, pageSize, out totalCount);
                }

                var result = products.Select(p => new
                {
                    id = p.Id,
                    productName = p.ProductName,
                    description = p.Description,
                    price = p.Price,
                    discountPrice = p.DiscountPrice,
                    finalPrice = p.FinalPrice,
                    categoryId = p.CategoryId,
                    categoryName = p.Category?.CategoryName,
                    imageUrl = p.ImageUrl,
                    stock = p.Stock,
                    featured = p.Featured,
                    createdDate = p.CreatedDate
                });

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Lấy danh sách sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/products/{id}
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetProduct(int id)
        {
            try
            {
                var product = _productService.GetById(id);
                if (product == null)
                {
                    return NotFound();
                }

                var result = new
                {
                    id = product.Id,
                    productName = product.ProductName,
                    description = product.Description,
                    price = product.Price,
                    discountPrice = product.DiscountPrice,
                    finalPrice = product.FinalPrice,
                    categoryId = product.CategoryId,
                    categoryName = product.Category?.CategoryName,
                    imageUrl = product.ImageUrl,
                    stock = product.Stock,
                    featured = product.Featured,
                    createdDate = product.CreatedDate
                };

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Lấy thông tin sản phẩm thành công"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/products/category/{categoryId}
        [HttpGet]
        [Route("category/{categoryId}")]
        public IHttpActionResult GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = _productService.GetProductsByCategory(categoryId);
                var result = products.Select(p => new
                {
                    id = p.Id,
                    productName = p.ProductName,
                    price = p.Price,
                    discountPrice = p.DiscountPrice,
                    finalPrice = p.FinalPrice,
                    imageUrl = p.ImageUrl,
                    stock = p.Stock
                });

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Lấy sản phẩm theo danh mục thành công"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/products/featured
        [HttpGet]
        [Route("featured")]
        public IHttpActionResult GetFeaturedProducts(int count = 8)
        {
            try
            {
                var products = _productService.GetFeaturedProducts(count);
                var result = products.Select(p => new
                {
                    id = p.Id,
                    productName = p.ProductName,
                    price = p.Price,
                    discountPrice = p.DiscountPrice,
                    finalPrice = p.FinalPrice,
                    imageUrl = p.ImageUrl
                });

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "Lấy sản phẩm nổi bật thành công"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

