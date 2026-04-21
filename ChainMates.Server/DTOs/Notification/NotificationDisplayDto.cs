using System.Text.Json;

namespace ChainMates.Server.DTOs.Notification
{
    public class NotificationDisplayDto
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int NotificationTypeId { get; set; }

        //public int RecipientAuthorId { get; set; }
        //public int InstigatorAuthorId { get; set; }

        public string Info { get; set; }

    }
}
