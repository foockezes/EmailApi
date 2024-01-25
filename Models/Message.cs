using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MailProject.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SendedById { get; set; }
        public User SendedBy { get; set; }
        public List<CopyFor> SendedFor { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public List<MessageFile> Files { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
