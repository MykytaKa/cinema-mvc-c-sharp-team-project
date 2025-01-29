using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class PasswordRecoveryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public PasswordRecoveryController(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult EnterEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnterEmail(string email)
        {
            var userRepository = _unitOfWork.Repository<User>();
            var user = (await userRepository.GetAsync(u => u.Email == email)).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError("", "Email not found.");
                return View();
            }

            var code = new Random().Next(100000, 999999).ToString();
            TempData["RecoveryCode"] = code;
            TempData["UserEmail"] = email;

            await _emailService.SendEmailAsync(email, "Password Recovery Code", $"Your recovery code: {code}");

            return RedirectToAction("VerifyCode");
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyCode(string code)
        {
            var storedCode = TempData["RecoveryCode"]?.ToString();

            if (storedCode == null || storedCode != code)
            {
                ModelState.AddModelError("", "Invalid code.");
                return View();
            }

            return RedirectToAction("ResetPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string newPassword)
        {
            var email = TempData["UserEmail"]?.ToString();

            if (email == null)
            {
                ModelState.AddModelError("", "Session expired. Please start again.");
                return RedirectToAction("EnterEmail");
            }

            var userRepository = _unitOfWork.Repository<User>();
            var user = (await userRepository.GetAsync(u => u.Email == email)).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return RedirectToAction("EnterEmail");
            }

            user.HashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "Password successfully changed.";
            return RedirectToAction("Login", "Account");
        }
    }
}
