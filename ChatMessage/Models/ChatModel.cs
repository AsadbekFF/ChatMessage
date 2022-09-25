using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMessage.Models
{
    public class ChatModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ReceiverId { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }
        public DateTime DateOfSendingMessage { get; set; }
    }
}
