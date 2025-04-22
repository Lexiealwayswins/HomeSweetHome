using System.ComponentModel.DataAnnotations;

namespace HomeSweetHome.Models
{
    public class ContactMessage
    {
        [Key] 
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int PostId { get; set; }
        public string PostType { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public string SentDate { get; set; } = string.Empty; 

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        public DateTime SentDateAsDateTime
        {
            get => DateTime.TryParse(SentDate, out var result) ? result : DateTime.MinValue;
            set => SentDate = value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}