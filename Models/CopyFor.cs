using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MailProject.Models
{
    public class CopyFor
    {
        public int Id { get; set; }
        public int TextId { get; set; }
        public Message Text { get; set; }
        public int SendedForId { get; set; }
        public User SendedFor { get; set; }
        public int Status { get; set; }
        public bool IsMain { get; set; }
        public bool IsRead { get; set; }
    }
}
