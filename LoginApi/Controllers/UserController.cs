using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (_context.Users.Any(U => U.Email == request.Email))
            {
                return BadRequest("This Email Already Exist");
            }

            CreatePasswordHash(request.Password, out byte[] passwordhash, out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordhash,
                PasswordSalt = passwordSalt,
               VerificationToken = CreateRandomToken()
            };

             await  _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok("User successfully created!"); //Just using this for now............
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
           var user = _context.Users.FirstOrDefault(U => U.Email == request.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Password is incorrect.");
            }

            if (user.VerifiedAt == null)
            {
                return BadRequest("Not Verified!");
            }
            

            return Ok($"Welcome back,{user.Email}! :");
        }



        [HttpPost]
        [Route("verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = _context.Users.FirstOrDefault(U => U.VerificationToken == token);
            if (user == null)
            {
                return BadRequest("Invalid token.");
            }

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();


            return Ok($"User verified! :");
        }


        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(U => U.Email == email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddMinutes(240).ToString();
            await _context.SaveChangesAsync();


            return Ok("You may now reset your password.");
        }


        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(U => U.PasswordResetToken == request.Token);
            //if (user == null || user.ResetTokenExpires < DateTime.Now) //well look into thid lster
            if (user == null )
            {
                return BadRequest("Invalid Token.");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            
            await _context.SaveChangesAsync();


            return Ok("Password successfully reset.");
        }



        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordhash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                
              var ComputerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return ComputerHash.SequenceEqual(passwordhash);
            }
        }

        private  string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

















    }
}









