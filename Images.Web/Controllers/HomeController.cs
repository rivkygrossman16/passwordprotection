using Images.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Images.Data;

namespace Images.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Image; Integrated Security=true;";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string password)
        {

            string fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            var id = repo.AddImage(filePath, password, fileName);
            var imagePassword = repo.GetImagePasswordById(id);
            viewModel vm = new viewModel
            {
                id = id,
                password = imagePassword
            };
            return View(vm);
        }
        public IActionResult EnterPassword(int id, string password)

        {
            viewImage vi = new viewImage();
            vi.id = id;
            List<int> IdList = HttpContext.Session.Get<List<int>>("IdList");
            if (IdList == null)
            {
                IdList = new List<int>();
            }
            if(IdList.Contains(id))
                {
                vi.alreadyHasAccess = true;
            }
            
          
            if (password != null||vi.alreadyHasAccess)
            {
                var repo = new ImageRepository(_connectionString);
                var correctpassword = repo.GetImagePasswordById(id);
               
                if (correctpassword == password||vi.alreadyHasAccess)
                {
                    IdList.Add(id);
                    HttpContext.Session.Set("IdList", IdList);

                    repo.IncrementView(id);
                    Image image = repo.GetimageById(id);
                    vi.image = image;
                    vi.submittedCorrectPassword = true;
               }
              
                else
                {
                    TempData["Message"] = "Incorrect";
                    vi.message = (string)TempData["Message"];
                    vi.submittedCorrectPassword = false;
                   
                }
            }
            else
            {
                vi.submittedCorrectPassword = false;
            }

                return View(vi);

            }
        }
    }

