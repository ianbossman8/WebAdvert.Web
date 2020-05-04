using System;
using System.IO;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;


namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient;
        private readonly IMapper _mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient;
            _mapper = mapper;
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
                CreateAdvertModel createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                createAdvertModel.UserName = User.Identity.Name;

                AdvertResponse apiCallResponse = await _advertApiClient.Create(createAdvertModel);
                string id = apiCallResponse.Id;

                if (imageFile != null)
                {
                    string fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    string filePath = $"{id}/{fileName}";

                    try
                    {
                        using (Stream readStream = imageFile.OpenReadStream())
                        {
                            bool result = await _fileUploader.UploadFileAsync(filePath, readStream);
                                
                            if (!result) throw new Exception("Could not upload the image to file repository. Please see the logs for details.");
                        }

                        ConfirmAdvertRequest confirmModel = new ConfirmAdvertRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Active
                        };

                        var canConfirm = await _advertApiClient.Confirm(confirmModel);

                        if (!canConfirm)
                        {
                            throw new Exception($"Cannot confirm advert of id ={id}");
                        }


                        return RedirectToAction("Index", controllerName: "Home");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        ConfirmAdvertRequest confirmModel = new ConfirmAdvertRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending
                        };

                        await _advertApiClient.Confirm(confirmModel);
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}