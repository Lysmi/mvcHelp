using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Controllers
{    
    public class ProductController : Controller
    {
        private readonly IAllProduct _iProduct;
        private readonly IAllCategory _iAllCategory;        

        public ProductController( IAllProduct iProduct, IAllCategory iAllCategory)
        {        
            _iProduct = iProduct;
            _iAllCategory = iAllCategory;           
        }

        /// <summary>
        /// All Products Page
        /// </summary>
        /// <returns></returns>
        [Route("/products")]
        public ViewResult All()
        {
            ProductCategory prodcat = new ProductCategory
            {
                Cat = _iAllCategory.IsPublishedCategory,
                Prod = _iProduct.IsPublishedProduct
            };
            return View(prodcat);
        }

        /// <summary>
        /// One Product Page
        /// </summary>
        /// <returns></returns>
        [Route("/product/show/{id?}")]
        public ViewResult One(int id)
        {
            OneProductCategory prodcat = new OneProductCategory
            {
                Cat = _iAllCategory.IsPublishedCategory,
                Prod = _iProduct.GetObjectProduct(id)
            };
            return View(prodcat); 
        }
    }
}