using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FullTextSearch.Contracts;
using FullTextSearch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace FullTextSearch.Controllers
{
    public class FileController : Controller
    {
        private readonly IMongoCdnService cdn;
        private readonly IMessageService messageService;
        private readonly ILogger logger;

        public FileController(
            IMongoCdnService _cdn,
            IMessageService _messageService,
            ILogger<FileController> _logger)
        {
            cdn = _cdn;
            messageService = _messageService;
            logger = _logger;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IEnumerable<IFormFile> files)
        {
            try
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileId = await cdn.UploadAsync(formFile);

                        messageService.PublishMessage(fileId);
                    }
                }

                TempData[MessageConstant.SuccessMessage] = "Файловете са качени успешно";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "File/Upload");

                TempData[MessageConstant.ErrorMessage] = "Грешка при качване на файл";
            }

            return View();
        }
    }
}