using SQLite;

namespace MiniTrello.Models
{
    public class CardItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int BoardId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; } = 0;       // 0=ToDo, 1=InProgress, 2=Done
        public int Priority { get; set; } = 0;      // 0=Low, 1=Medium, 2=High
        public DateTime? DueDate { get; set; }
        public int Order { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
