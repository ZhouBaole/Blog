using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class messeus
    {
        public messeus(Message messages, Usersd users)
        {
            this.messages = messages;
            this.users = users;
        }
        public messeus()
        {
           
        }

        public Message messages { get; set; }
        public Usersd users { get; set; }
    }
}