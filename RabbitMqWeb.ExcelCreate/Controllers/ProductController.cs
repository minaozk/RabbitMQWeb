using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMqWeb.ExcelCreate.Models;
using RabbitMqWeb.ExcelCreate.Services;

namespace RabbitMqWeb.ExcelCreate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext appDbContext, RabbitMQPublisher rabbitMQPublisher)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";
            var filePath = $"/files/{fileName}.xlsx";  // Örnek bir dosya yolu

            UserFile userFile = new UserFile()
            {
                UserId = user.Id,
                FileName = fileName,
                FilePath = filePath,  // FilePath değerini burada ayarlıyoruz
                FileStatus = FileStatus.Creating
            };

            await _appDbContext.AddAsync(userFile);
            await _appDbContext.SaveChangesAsync();

            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage()
            {
                FileId = userFile.Id.ToString(),

            });
            TempData["StartCreatingExcel"] = true;
            return RedirectToAction(nameof(Files));
        }


        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(await _appDbContext.UserFiles.Where(x => x.UserId == user.Id).ToListAsync());
        }
    }
}
