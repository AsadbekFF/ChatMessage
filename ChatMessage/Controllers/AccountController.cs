using ChatMessage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMessage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext context = new(new DbContextOptions<ApplicationContext>());

        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var currentUserName = _userManager.GetUserAsync(User).Result.UserName;
            var CurUserSkip = _userManager.Users
                .ToList()
                .Where(x => x.UserName != currentUserName);
            ViewData["Users"] = CurUserSkip;
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (ModelState.IsValid)
            {
                // добавляем пользователя
                user.UserName = user.Name;
                if (!_userManager.Users.Any(x => x.Name == user.Name))
                {
                    var result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {
                        // установка куки
                        var userCur = await _userManager.FindByNameAsync(user.Name);
                        await _signInManager.SignInAsync(userCur, false);

                        ViewData["Users"] = _userManager.Users.ToList();
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    var userCur = await _userManager.FindByNameAsync(user.Name);
                    await _signInManager.SignInAsync(userCur, false);

                    ViewData["Users"] = _userManager.Users.ToList();
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(ChatModel chat)
        {
            chat.DateOfSendingMessage = DateTime.Now;
            context.Chats.Add(chat);
            context.SaveChanges();
            return OpenChatWithUser(chat.ReceiverId).Result;
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> OpenChatWithUser(string reciever)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var sender = context.Chats.ToList().Where(x => (x.ReceiverId == reciever 
            && x.UserId == currentUser.Id)
            || (x.UserId == reciever
            && x.ReceiverId == currentUser.Id)).OrderBy(x => 
            x.DateOfSendingMessage);
            var currentUserName = _userManager.GetUserAsync(User).Result.UserName;
            var CurUserSkip = _userManager.Users
                .ToList()
                .Where(x => x.UserName != currentUserName);
            ViewData["RecieverId"] = reciever;
            ViewData["Users"] = CurUserSkip;
            ViewData["ChatWithUser"] = sender;
            return View("Index");
        }
    }
}
