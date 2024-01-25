using System;

namespace MailProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Sname { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
        public int RoleId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
