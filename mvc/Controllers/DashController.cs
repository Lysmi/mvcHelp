using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using mvc.Models;
using mvc.Interfaces;
using mvc.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using mvc.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace mvc.Controllers
{
    [Authorize]
    public class DashController : Controller
    {
        private readonly IAllProduct _allProduct;
        private readonly IAllCategory _allCategory;
        private readonly ApplicationDbContext _applicationDb;

        public DashController(ApplicationDbContext applicationDb, IAllProduct iAllProduct, IAllCategory iAllCategory)
        {
            _allProduct = iAllProduct;
            _allCategory = iAllCategory;
            _applicationDb = applicationDb;
        }

        ///*******************************************************************
        ///---------- �������� �������� ������� -----------
        /// <summary>
        /// Home Page Dashboard
        /// </summary>
        /// <returns></returns>
        [Route("/dash")]
        public IActionResult Index()
        {
            return View();      // ���� return View(); 
        }

        ///*******************************************************************
        ///---------- CRUD for Products -----------
        /// <summary>
        /// Show All Products
        /// </summary>
        /// <returns></returns>
        [Route("/dash/products")]
        public IActionResult Products()
        {
            ProductCategory prodcat = new ProductCategory
            {
                Cat = _allCategory.AllCategory,
                Prod = _allProduct.AllProduct
            };
            return View(prodcat);
        }

        /// <summary>
        /// Create New Product and Redirect To All
        /// </summary>
        /// <returns></returns>
        [Route("/dash/prodnew")]
        public async Task<IActionResult> ProdNew()
        {
            if (Request.Form["title"] != "")
            {
                //-- ������� ����� ������� --
                Product prod = new Product();
                prod.Title = Request.Form["title"];
                prod.Number = Convert.ToInt32(Request.Form["number"]);
                prod.Sku = Request.Form["sku"];
                prod.Dimension = Request.Form["dimension"];
                prod.Price = Convert.ToInt32(Request.Form["price"]);
                prod.PriceBig = Convert.ToInt32(Request.Form["pricebig"]);
                prod.Description = Request.Form["description"];
                prod.Summary = Request.Form["summary"];
                prod.Materials = Request.Form["materials"];
                if (Request.Form["isnew"] == "on") { prod.IsNew = true; }
                else { prod.IsNew = false; }
                if (Request.Form["ispublished"] == "on") { prod.IsPublished = true; }
                else { prod.IsPublished = false; }
                prod.CategoryId = Convert.ToInt32(Request.Form["category"]);

                try
                {
                    await _applicationDb.AddAsync(prod);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Product.");
                }

                int id = await _applicationDb.Product.MaxAsync(prod => prod.Id);

                //-- ������ ��������� � �������� --
                //- �������� �� �������                             
                Image imghome = new Image();
                imghome.ProductId = id;
                imghome.ImageTypeId = 1;
                imghome.Guide = Request.Form["imghome"];
                try
                {
                    await _applicationDb.AddAsync(imghome);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Image �������� �� ��������.");
                }

                //- �������� �� �������� ������ - ��� �����
                Image imgone = new Image();
                imgone.ProductId = id;
                imgone.ImageTypeId = 3;
                imgone.Guide = Request.Form["imgone"];

                try
                {
                    await _applicationDb.AddAsync(imgone);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Image �������� �� �������� ������ ��� �����.");
                }

                //- ����� �������� �� �������� ����� ������ - ������� nullable
                if (Request.Form["imggal"] != "" && Request.Form["quansel"] != "0")
                {
                    Image imggal = new Image();
                    imggal.ProductId = id;
                    imggal.ImageTypeId = 4; ;
                    imggal.Guide = Request.Form["imggal"];
                    imggal.Quan = Convert.ToInt32(Request.Form["quansel"]);

                    try
                    {
                        await _applicationDb.AddAsync(imggal);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Image �������� �� �������� ������ �������.");
                    }
                }

                //- ����� ������ � ����� - nullable                 
                if (Request.Form["video"] != "")
                {
                    Video video = new Video();
                    video.ProductId = id;
                    video.Guide = Request.Form["video"];

                    try
                    {
                        await _applicationDb.AddAsync(video);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Video.");
                    }
                }
            }
            return Redirect("~/Dash/Products");
        }

        /// <summary>
        /// Edit Product and Redirect To All
        /// </summary>
        /// <returns></returns>
        [Route("/dash/prodedit")]
        public async Task<IActionResult> ProdEdit()
        {
            if (Request.Form["id"] != "")
            {
                //-- ������ ��������� � ������� �� id --
                int id = Convert.ToInt32(Request.Form["id"]);

                //- � ��� �������
                Product prod = new Product { Id = id };
                // Product prod = _applicationDb.Product.Find(id);
                // var prod = await _applicationDb.Product.FirstOrDefaultAsync(p => p.Id == id);

                //- ������ ���������
                prod.Number = Convert.ToInt32(Request.Form["number"]);
                prod.Title = Request.Form["title"];
                prod.Sku = Request.Form["sku"];
                prod.Dimension = Request.Form["dimension"];
                prod.Price = Convert.ToInt32(Request.Form["price"]);
                prod.PriceBig = Convert.ToInt32(Request.Form["pricebig"]);
                prod.Description = Request.Form["description"];
                prod.Summary = Request.Form["summary"];
                prod.Materials = Request.Form["materials"];
                if (Request.Form["isnew"] == "on") { prod.IsNew = true; }
                else { prod.IsNew = false; }
                if (Request.Form["ispublished"] == "on") { prod.IsPublished = true; }
                else { prod.IsPublished = false; }
                prod.CategoryId = Convert.ToInt32(Request.Form["category"]);

                try
                {
                    _applicationDb.Update(prod);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Product.");
                }

                System.Diagnostics.Debug.WriteLine("����� � ������� imghomeid �� �����");
                System.Diagnostics.Debug.WriteLine(Request.Form["imghomeid"]);

                //-- ������ ��������� � �������� --

                //- �������� �� ������� - ���� - ������ ����                
                if (Request.Form["imghome"] != "" && Request.Form["imghomeid"] != "0")
                {
                    int imghomeid = Convert.ToInt32(Request.Form["imghomeid"]);
                    //int imghomeid = _applicationDb.Image.Where(i => i.ImageTypeId == 1)
                    //                                .Where(i => i.ProductId == id)
                    //                                .Select(i => i.Id)
                    //                                .FirstOrDefault();

                    Image my_imghome = new Image { Id = imghomeid };
                    my_imghome.Guide = Request.Form["imghome"];
                    my_imghome.ProductId = id;
                    my_imghome.ImageTypeId = 1;
                    my_imghome.Quan = 0;
                    try
                    {
                        _applicationDb.Update(my_imghome);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Image �������� �� ��������.");
                    }
                }

                //-�������� �� �������� ������ - ��� ����� - ���� - ������ ����
                if (Request.Form["imgone"] != "" && Request.Form["imgoneid"] != "0")
                {
                    int imgoneid = Convert.ToInt32(Request.Form["imgoneid"]);
                    //int imghomeid = _applicationDb.Image.Where(i => i.ImageTypeId == 1)
                    //                                .Where(i => i.ProductId == id)
                    //                                .Select(i => i.Id)
                    //                                .FirstOrDefault();

                    Image my_imgone = new Image { Id = imgoneid };
                    my_imgone.Guide = Request.Form["imgone"];
                    my_imgone.ProductId = id;
                    my_imgone.ImageTypeId = 3;
                    my_imgone.Quan = 0;
                    try
                    {
                        _applicationDb.Update(my_imgone);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Image �������� �� �������� ������ ��� �����.");
                    }
                }

                //-�������� �� �������� ������ - ������� - �� ���� - nullable
                if (Request.Form["imggal"] != "" && Request.Form["quansel"] != 0)
                {
                    int imggalid = Convert.ToInt32(Request.Form["imggalid"]);
                    Image my_imggal = new Image(); // ������� �����
                    if (imggalid != 0)
                    {
                        my_imggal.Id = imggalid; // �������� ������������ ���� ����
                    }

                    my_imggal.Guide = Request.Form["imggal"];
                    my_imggal.ProductId = id;
                    my_imggal.ImageTypeId = 4;
                    my_imggal.Quan = Convert.ToInt32(Request.Form["quansel"]);
                    try
                    {
                        _applicationDb.Update(my_imggal);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Image �������� �� �������� ������ �������.");
                    }
                }
                else
                {
                    // ������� ��� �������� ������
                    if (Request.Form["imggal"] == "" || Request.Form["quansel"] == 0)
                    {
                        int imggalid = Convert.ToInt32(Request.Form["imggalid"]);
                        if (imggalid != 0)
                        {
                            Image my_imggalid = new Image(); // 
                            my_imggalid.Id = imggalid; // ������� ������������ ���� ����

                            try
                            {
                                _applicationDb.Image.Remove(my_imggalid);
                                await _applicationDb.SaveChangesAsync();
                            }
                            catch (DbUpdateException /* ex */)
                            {
                                //Log the error (uncomment ex variable name and write a log.)
                                ModelState.AddModelError("", "�� ���������� ������� Image - �������� �� �������� ������ �������.");
                            }
                        }
                    }
                }

                //-- ������ ��������� � ����� - ���� ������ - nullable
                if (Request.Form["video"] != "" && Request.Form["videoid"] != "")
                {
                    int videoid = Convert.ToInt32(Request.Form["videoid"]);
                    Video my_video = new Video();
                    if (videoid != 0)
                    {
                        my_video.Id = videoid;
                    }

                    my_video.Guide = Request.Form["video"];
                    my_video.ProductId = id;

                    try
                    {
                        _applicationDb.Update(my_video);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Video.");
                    }
                }
                // �������� �����, ���� ���� ������ � � ���� ����� �����
                if (Request.Form["video"] == "" && Request.Form["videoid"] != "" && Request.Form["videoid"] != "0")
                {
                    int videoid = Convert.ToInt32(Request.Form["videoid"]);
                    Video my_video = new Video();
                    my_video.Id = videoid;

                    try
                    {
                        _applicationDb.Video.Remove(my_video);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������ ��������� � ������ Video.");
                    }
                }
            }
            return Redirect("~/Dash/Products");
        }

        /// <summary>
        /// Delete Product and Redirect To All
        /// </summary>
        /// <returns></returns>
        [Route("/dash/proddel/{id}")]
        public async Task<IActionResult> ProdDel(int id)
        {
            if (id != 0)
            {
                //-������� �����
                Product product = new Product { Id = id };
                try
                {
                    _applicationDb.Product.Remove(product);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ������� ������� � ������ Product.");
                }

                //-������� �����, ���� ����
                var video = await _applicationDb.Video.FirstOrDefaultAsync(v => v.ProductId == id);
                if (video != null)
                {
                    try
                    {
                        _applicationDb.Video.Remove(video);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������� ������ � ������ Video.");
                    }
                }

                //-������� ��� ��������
                //-�������� ������� ������� ��������
                var imghome = await _applicationDb.Image.Where(i => i.ImageTypeId == 1).FirstOrDefaultAsync(i => i.ProductId == id);
                if (imghome != null)
                {
                    try
                    {
                        _applicationDb.Image.Remove(imghome);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������� �������� �� �������� � ������ Image.");
                    }
                }

                //-�������� ������� ������� ��������
                var imgone = await _applicationDb.Image.Where(i => i.ImageTypeId == 3).FirstOrDefaultAsync(i => i.ProductId == id);
                if (imgone != null)
                {
                    try
                    {
                        _applicationDb.Image.Remove(imgone);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������� �������� �� �������� ������� ��� ����� � ������ Image.");
                    }
                }

                //-�������� ������� �� �������� ������
                var imggal = await _applicationDb.Image.Where(i => i.ImageTypeId == 4).FirstOrDefaultAsync(i => i.ProductId == id);
                if (imggal != null)
                {
                    try
                    {
                        _applicationDb.Image.Remove(imggal);
                        await _applicationDb.SaveChangesAsync();
                    }
                    catch (DbUpdateException /* ex */)
                    {
                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "�� ���������� ������� �������� �� �������� ������� ������� � ������ Image.");
                    }
                }
            }
            
            return Redirect("~/Dash/Products");
        }

        ///*******************************************************************
        ///---------- CRUD for Categories -----------
        /// <summary>
        /// Show All Categories
        /// </summary>
        /// <returns></returns>
        [Route("/dash/categories")]
        public IActionResult Categories()
        {
            var category = _allCategory.AllCategory;
            return View(category);
        }
        /// <summary>
        /// Create New Category and Redirect To All
        /// </summary>
        /// <returns></returns>
        [Route("/dash/catnew")]
        public async Task<IActionResult> CatNew()
        {
            if (Request.Form["title"] != "")
            {
                Category cat = new Category();
                cat.Title = Request.Form["title"];
                cat.Description = Request.Form["description"];
                cat.Slug = Request.Form["slug"];
                cat.Number = Convert.ToInt32(Request.Form["number"]);
                if (Request.Form["ispublished"] == "on") { cat.IsPublished = true; }
                else { cat.IsPublished = false; }
                try 
                {
                    await _applicationDb.Category.AddAsync(cat);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ������� ����� ������ � ������ Category.");
                }
            }
            return Redirect("~/Dash/Categories");
        }
        /// <summary>
        /// Edit Category and Redirect To All
        /// </summary>
        /// <returns></returns>
        [Route("/dash/catedit")]
        public async Task<IActionResult> CatEdit()
        {
            if (Request.Form.FirstOrDefault().Value != "")
            {
                int id = Convert.ToInt32(Request.Form["id"]);

                var cat = await _applicationDb.Category.FindAsync(id);

                cat.Title = Request.Form["title"];
                cat.Slug = Request.Form["slug"];
                cat.Description = Request.Form["description"];
                cat.Number = Convert.ToInt32(Request.Form["number"]);
                if (Request.Form["ispublished"] == "on") { cat.IsPublished = true; }
                else { cat.IsPublished = false; }
                try
                {
                    _applicationDb.Update(cat);
                    await _applicationDb.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "�� ���������� ��������������� ��������� �� ������ Category.");
                }
            }
            return Redirect("~/Dash/Categories");
        }
        /// <summary>
        /// Delete Category and Redirect To All
        /// </summary>
        /// <returns></returns>
        [Route("/dash/catdel/{id}")]
        public async Task<IActionResult> CatDel(int id)
        {
            var cat = await _applicationDb.Category.FindAsync(id);
            System.Diagnostics.Debug.WriteLine("Id ���������");
            System.Diagnostics.Debug.WriteLine(id);
            try
            {
                _applicationDb.Category.Remove(cat);
                await _applicationDb.SaveChangesAsync();
            }
            catch (DbUpdateException)  //ex 
            {
                //Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "�� ���������� ������� ��������� �� ������ Category.");
            }     
            return Redirect("~/Dash/Categories");
        }

        [Route("/dash/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}