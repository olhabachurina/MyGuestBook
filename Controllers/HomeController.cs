using Microsoft.AspNetCore.Mvc;
using GuestBook.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GuestBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.Messages.Include(m => m.User).ToListAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(MessageViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                var newMessage = new Message
                {
                    Id_User = userId.Value,
                    Content = model.Content,
                    Email = model.Email,
                    WWW = model.WWW,
                    MessageDate = DateTime.Now,
                    User = user
                };

                _context.Messages.Add(newMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var messages = await _context.Messages.Include(m => m.User).ToListAsync();
            return View("Index", messages); 
        }

        public async Task<IActionResult> MyReviews()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var messages = await _context.Messages
                .Where(m => m.Id_User == userId)
                .Include(m => m.User)
                .ToListAsync();

            return View(messages);
        }
    }
}