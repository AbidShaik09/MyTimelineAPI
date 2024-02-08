using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTimeline.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyTimeline.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {



            // Check if the username already exists
            if (null != _db.Users.FirstOrDefault(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists");
            }
            if (null != _db.Users.FirstOrDefault(u => u.Email == user.Email))
            {
                return BadRequest("Email is already registered");
            }

            // Hash the password
            user.Password = ComputeSha256Hash(user.Password);

            // Add the user to the database
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok();
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDetails login)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            // Check if the user exists and the password is correct
            if (user == null || user.Password != ComputeSha256Hash(login.Password))
            {
                return Unauthorized();
            }

            return Ok("");
        }

        // A simple SHA256 hash function for password hashing
        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public class LoginDetails
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }



    }

}
