using MailProject.Models;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

namespace MailProject.Requests
{
    public class MessageRequest
    {
        public int Id { get; set; }
        public int SendedById { get; set; }
        public User SendedBy { get; set; }
        public List<CopyFor> SendedFor { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<IFormFile> Files { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
