﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DMART.Models;
using DMART.Models.Interfaces;

namespace DMART.Controllers
{
    public class AdminController : Controller
    {

        private readonly IProductRepository productRepo;
        private readonly ICategoryRepository categoryRepo;
        private readonly IWebHostEnvironment Environment;

        public AdminController(IProductRepository productRepository, ICategoryRepository _categoryRepo , IWebHostEnvironment environment)
        {
            productRepo = productRepository;
            Environment = environment;
            categoryRepo = _categoryRepo;
        }
        

        public ViewResult dashboard()
        {
            return View();
        }

        //upload image of to the server
        public String UploadImage(IFormFile image, string imageType) 
        {
            string wwwPath = this.Environment.WebRootPath;
            String folderName = "";

            //store the product and category images in their respective folder
            if (imageType=="product")
                folderName = "ProductImages";
            else
                folderName = "CategoryImages";

            //absolute path of images folder
            string path = Path.Combine(wwwPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            String fileName = Path.GetFileName(image.FileName);
            //TO MAKE FILENAME UNIQUE
            fileName = Guid.NewGuid().ToString() + "_" +  fileName;
           // absolute path of uploaded image file
            var pathWithFileName = Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(pathWithFileName, FileMode.Create))
            {
                image.CopyTo(stream);
                ViewBag.Message = "File uploaded successfully";
            }
            return "\\" + folderName +"\\" + fileName;
        }
        
        // to show sucess message upon addition of new product
        public ViewResult addProduct(bool isSuccess = false, int bookId = 0)
        {
            ViewBag.IsSuccess = false;
            ViewData["username"] = "Admin";
            ViewData["isLogin"]="true";
            ViewBag.IsSuccess = isSuccess;
            ViewBag.BookId = bookId;
            return View();
        }

        // add new product to database
        [HttpPost]
        public IActionResult addProduct(Product item, IFormFile postedImage)
        {
            ViewData["username"] = "Admin";
            ViewData["isLogin"]="true";
            if (postedImage!=null)
            {
                item.ImageUrl =  UploadImage(postedImage, "ProductImages"); //upload image along with saving its path to Database
                Console.WriteLine(item.ImageUrl);
            }
            else {
                item.ImageUrl = "\\ProductImages\\defaultItem.jpg";
            }
            item.CreatedById = HttpContext.Session.GetString("username");
            int id = productRepo.AddProduct(item);
            if (id > 0)
            {
                return RedirectToAction("addProduct", new { isSuccess = true, bookId = id });
            }
            return View();
        }
      
        [HttpGet]
        public ViewResult updateProduct(int productId)
        {
            ViewData["isLogin"] = "true";
            Product item = productRepo.GetProductById(productId);
            return View("updateProduct",item);
        }

        [HttpPost]
        public IActionResult updateProduct(Product item)
        {
           // Product item = productRepo.GetProductById(productId);
            Product updatedItem = productRepo.UpdateProduct(item);
            item.LastModifiedById = HttpContext.Session.GetString("username");
            return RedirectToAction("getAllProducts");
        }

            /*
            [HttpPost]
            public IActionResult updateProduct(Product item)
            {
                productRepo.GetProductById(item.Id);
                return View("showProduct", item); //show singleProduct
            }
            */
            public IActionResult deleteProduct(int productId)
            {
                Product item = productRepo.GetProductById(productId);
                productRepo.DeleteProduct(item);
                return RedirectToAction("getAllProducts");
            }

            /* public ViewResult showProduct(int id)
                    { }
                    */

            public ViewResult getAllProducts()
        {
            List<Product> items = productRepo.GetAllProducts();
            return View(items);
        }

        public ViewResult getProduct(int id) 
        {
            Product item = productRepo.GetProductById(id);
            //view that shows item not found
            if (item == null)
            {
                return View("Error");
            }
            return View(item);
        }

        
        public ViewResult addCategory(bool isSuccess = false, int bookId = 0)
        {
            ViewBag.IsSuccess = isSuccess;
            ViewBag.BookId = bookId;
            return View();
        }
        
        [HttpPost]
        public IActionResult addCategory(Category category, IFormFile postedImage)
        {
            ViewData["isLogin"]="true";
            if (postedImage!=null)
            {
                category.ImageUrl =  UploadImage(postedImage, "CategoryImages"); //upload image along with saving its path to Database
                Console.WriteLine(category.ImageUrl);
            }
            else
            {
                category.ImageUrl = "\\CategoryImages\\defaultCategory.jpg";
            }

            int id = categoryRepo.AddCategory(category);
            if (id > 0)
            {
                return RedirectToAction("addCategory", new { isSuccess = true, bookId = id });
            }

            return View();
        }
        

        

    }
}
