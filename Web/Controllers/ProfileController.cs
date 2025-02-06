using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.Interfaces;

namespace Web.Controllers
{
    [Authorize] 
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService; 

        public ProfileController(IUnitOfWork unitOfWork, IEmailService emailService) 
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _unitOfWork.Repository<User>().GetByIDAsync(int.Parse(userId));
            if (user == null) return NotFound();

            return View(user); 
        }

        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string newEmail)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _unitOfWork.Repository<User>().GetByIDAsync(int.Parse(userId));
            if (user == null) return NotFound();

            var code = new Random().Next(100000, 999999).ToString();
            TempData["EmailCode"] = code;
            TempData["NewEmail"] = newEmail;

            await _emailService.SendEmailAsync(newEmail, "Email Recovery Code", $"Your recovery code: {code}");

            return RedirectToAction("ConfirmEmail");
        }

        [HttpGet]
        public IActionResult ConfirmEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            var storedCode = TempData["EmailCode"]?.ToString();
            var newEmail = TempData["NewEmail"]?.ToString();

            if (storedCode == null || storedCode != code)
            {
                ModelState.AddModelError("", "Invalid code.");
                return View();
            }

            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _unitOfWork.Repository<User>().GetByIDAsync(int.Parse(userId));
            if (user == null) return NotFound();

            user.Email = newEmail;
            await _unitOfWork.Repository<User>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Email updated successfully!";
            return RedirectToAction("Profile"); 
        }
    }
}
