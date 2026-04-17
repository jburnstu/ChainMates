using ChainMates.Server.DTOs.Notification.Info;
using System.Text.Json;

namespace ChainMates.Server.DTOs.Author
{

    
    public class NotificationCreationDto
    {
        //public int Id { get; set; }
        //public DateTime DateCreated { get; set; }
        public int NotificationTypeId { get; set; }

        //public int RecipientAuthorId { get; set; }
        //public int InstigatorAuthorId { get; set; }

        public NotificationInfoDto Info { get; set; }

    }
}
