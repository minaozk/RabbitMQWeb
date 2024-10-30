using Microsoft.AspNetCore.Mvc;

namespace RabbitMQWeb.Watermark.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
