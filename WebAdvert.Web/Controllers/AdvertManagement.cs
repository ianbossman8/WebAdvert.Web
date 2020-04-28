using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;

using WebAdvert.Web.Services;


namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;

        public AdvertManagementController(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                string id = "11111";
                string fileName = "";

                if (imageFile != null)
                {
                    fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    string filePath = $"{id}/{fileName}";

                    try
                    {
                        using (Stream readStream = imageFile.OpenReadStream())
                        {
                            bool result = await _fileUploader.UploadFileAsync(filePath, readStream);
                                
                            if (!result) throw new Exception("Could not upload the image to file repository. Please see the logs for details.");
                        }

                        return RedirectToAction("Index", controllerName: "Home");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }


                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}