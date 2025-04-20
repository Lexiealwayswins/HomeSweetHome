using System.ComponentModel.DataAnnotations;

namespace HomeSweetHome.Models
{
    public class ContactMessage
    {
        [Key] // 明确指定 MessageId 为主键
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int PostId { get; set; }
        public string PostType { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public string SentDate { get; set; } = string.Empty; // 改为 string 类型，与数据库匹配

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        // 添加一个辅助属性，用于转换为 DateTime
        public DateTime SentDateAsDateTime
        {
            get => DateTime.TryParse(SentDate, out var result) ? result : DateTime.MinValue;
            set => SentDate = value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}