using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class FileController : Controller
    {
        private readonly AzureFileStorageService _fileStorageService;

        public FileController(
            AzureFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        // GET: /File
        public async Task<IActionResult> Index()
        {
            List<string> files =
                await _fileStorageService.GetFilesAsync();

            return View(files);
        }

        // POST: /File/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _fileStorageService.UploadFileAsync(file);

                TempData["Success"] =
                    $"File '{file.FileName}' uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"Upload failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /File/Download?fileName=example.pdf
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest();
            }

            Stream? stream =
                await _fileStorageService.DownloadFileAsync(fileName);

            if (stream == null)
            {
                return NotFound();
            }

            return File(
                stream,
                "application/octet-stream",
                fileName);
        }

        // POST: /File/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest();
            }

            try
            {
                await _fileStorageService.DeleteFileAsync(fileName);

                TempData["Success"] =
                    $"File '{fileName}' deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    $"Delete failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
