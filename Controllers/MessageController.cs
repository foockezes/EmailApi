using MailProject.Helpers;
using MailProject.Models;
using MailProject.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MailProject.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public MessageController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //[Authorize]
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromForm] MessageRequest request)
        {
            Answer answer = new();

            try
                {
                Message newMessage = new()
                {
                    Text = request.Text,
                    Title = request.Title,
                    Status = request.Status,
                    CreatedAt = DateTime.Now,
                    //SendedById = request.SendedBy.Id,
                };

                if(request.Files != null && request.Files.Count > 0)
                {
                    newMessage.Files = new List<MessageFile>();
                    Function fnc = new();
                    foreach (var file in request.Files)
                    {
                        var uniqueFileName = fnc.GetUniqueFileName(file.FileName);
                        var uploads = Path.Combine("wwwroot", "uploads"); // Adjust the path as needed
                        var filePath = Path.Combine(uploads, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        MessageFile mFile = new()
                        {
                            Name = uniqueFileName
                        };
                        // Link the file path to the Message entity
                        newMessage.Files.Add(mFile);
                    }
                }

                await _dbContext.Message.AddAsync(newMessage);
                await _dbContext.SaveChangesAsync();

                foreach (var sendFor in request.SendedFor)
                {
                    CopyFor copy = new()
                    {
                        //TextId = newMessage.Id,
                        Status = sendFor.Status,
                        //SendedForId = sendFor.SendedFor.Id,
                        IsMain = sendFor.IsMain,
                        IsRead = sendFor.IsRead
                    };
                    await _dbContext.CopyFor.AddAsync(copy);
                }
                await _dbContext.SaveChangesAsync();
                answer.Details = newMessage;
                return Ok(answer);
            }
            catch (System.Exception ex)
            {
                answer.status = 400;
                answer.title = "Something went wrong";
                answer.Details = ex.Message + "\n" + ex.StackTrace;

                return BadRequest(answer);
            }
        }
        [Authorize]
        [HttpGet("inbox")]
        public async Task<IActionResult> Inbox()
        {
            Answer answer = new();
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                List<Message> result = await _dbContext.Message.Where(m => m.SendedFor.Any(s => s.SendedFor.Id.ToString() == identity.Name)).Include(m => m.SendedFor).ToListAsync();
                answer.Details = result;
                return Ok(answer);
            }
            catch (Exception ex)
            {
                answer.status = 400;
                answer.title = "Something went wrong";
                answer.Details = ex.Message;
                return Ok(answer);
            }
        }
        [Authorize]
        [HttpGet("sent")]
        public async Task<IActionResult> Sent()
        {
            Answer answer = new();
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                List<Message> result = await _dbContext.Message.Where(m => m.SendedBy.Id.ToString() == identity.Name).Include(m => m.SendedFor).ToListAsync();
                answer.Details = result;
                return Ok(answer);
            }
            catch (Exception ex)
            {
                answer.status = 400;
                answer.title = "Something went wrong";
                answer.Details = ex.Message;
                return Ok(answer);
            }
        }
        [HttpPost("test")]
        public async Task<IActionResult> Test([FromForm] MessageRequest request)
        {
            Answer answer = new();
            try
            {
                Message newMessage = new()
                {
                    Text = "test",
                    Title = "Test",
                    Status = 1,
                    CreatedAt = DateTime.Now,
                    //SendedById = request.SendedBy.Id,
                };

                if (request.Files != null && request.Files.Count > 0)
                {
                    newMessage.Files = new List<MessageFile>();
                    Function fnc = new();
                    foreach (var file in request.Files)
                    {
                        var uniqueFileName = fnc.GetUniqueFileName(file.FileName);
                        var uploads = Path.Combine("wwwroot", "uploads"); // Adjust the path as needed
                        var filePath = Path.Combine(uploads, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        MessageFile mFile = new()
                        {
                            Name = uniqueFileName
                        };
                        // Link the file path to the Message entity
                        newMessage.Files.Add(mFile);
                    }
                }
                answer.Details = newMessage;
                return Ok(answer);
            }
            catch (Exception ex)
            {
                answer.status = 400;
                answer.title = "Something went wrong";
                answer.Details = ex.Message;
                return Ok(answer);
            }
        }
    }
}
