using SQLite;

namespace MiniTrello.Models
{
    public class Board
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; } = 0;  // 0=Backlog, 1=InProgress, 2=QA, 3=Completed
        public string Color { get; set; } = "#4A90D9";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
