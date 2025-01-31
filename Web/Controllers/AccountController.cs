using Application.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private static readonly Regex EmailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

        public AccountController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            if (!ModelState.IsValid || !ValidateRegistration(model))
                return View(model);

            var userRepository = _unitOfWork.Repository<User>();

            if (await UserExistsAsync(model.Email, userRepository))
            {
                ModelState.AddModelError("Email", "This email is already registered. Please use a different email or log in.");
                return View(model);
            }

            await CreateUserAsync(model, userRepository);

            var createdUser = await userRepository.GetAsync(u => u.Email == model.Email);
            SetAuthToken(createdUser.First());

            return RedirectToAction("ViewingSessions", "ViewingSessions");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userRepository = _unitOfWork.Repository<User>();
            var user = await FindUserByLoginAsync(model.Login, userRepository);

            if (!ValidateLogin(user, model.Password))
                return View(model);

            SetAuthToken(user);
            return RedirectToAction("ViewingSessions", "ViewingSessions");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("ViewingSessions", "ViewingSessions");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ProtectedResource() =>
            Ok(new { Message = "You have accessed a protected resource!", User = User.Identity?.Name });

        [Authorize]
        public IActionResult UserProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var userType = User.FindFirst("UserType")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            return Ok(new { UserId = userId, UserType = userType, Email = email });
        }

        private bool ValidateRegistration(RegisterUserDto model)
        {
            if (model.Password.Length < 6)
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");

            if (model.Password != model.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");

            if (!EmailRegex.IsMatch(model.Email))
                ModelState.AddModelError("Email", "Invalid email format.");

            return ModelState.IsValid;
        }

        private async Task<bool> UserExistsAsync(string email, IGenericRepository<User> userRepository) =>
            (await userRepository.GetAsync(u => u.Email == email)).Any();

        private async Task<User?> FindUserByLoginAsync(string login, IGenericRepository<User> userRepository) =>
            (await userRepository.GetAsync(u => u.Email == login || u.PhoneNumber == login)).FirstOrDefault();

        private bool ValidateLogin(User? user, string password)
        {
            if (user == null)
            {
                ModelState.AddModelError("Login", "User with this email or phone number does not exist.");
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.HashPassword))
            {
                ModelState.AddModelError("Password", "Invalid password.");
                return false;
            }

            return true;
        }

        private async Task CreateUserAsync(RegisterUserDto model, IGenericRepository<User> userRepository)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
                DateOfBirthday = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                TypeId = 2
            };

            await userRepository.InsertAsync(user);
            await _unitOfWork.SaveAsync();
        }

        private void SetAuthToken(User user)
        {
            var token = GenerateJwtToken(user);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(2)
            });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var userType = user.TypeId == 2 ? "User" : "Admin";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserType", userType)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
