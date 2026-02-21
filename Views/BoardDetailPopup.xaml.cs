using CommunityToolkit.Maui.Views;
using MiniTrello.Models;

namespace MiniTrello.Views
{
    public partial class BoardDetailPopup : Popup
    {
        private static readonly string[] StatusNames = { "Backlog", "In Progress", "Q.A", "Completed" };
        private static readonly string[] StatusColors = { "#A0AEC0", "#63B3ED", "#F6AD55", "#68D391" };

        public BoardDetailPopup(Board board)
        {
            InitializeComponent();

            BoardNameLabel.Text = board.Name;
            DescriptionLabel.Text = string.IsNullOrWhiteSpace(board.Description) ? "-" : board.Description;

            var statusIndex = board.Status >= 0 && board.Status < StatusNames.Length ? board.Status : 0;
            StatusLabel.Text = StatusNames[statusIndex];
            StatusBadge.BackgroundColor = Color.FromArgb(StatusColors[statusIndex]);
            HeaderFrame.BackgroundColor = Color.FromArgb("#2D2D44");

            CreatedLabel.Text = board.CreatedDate.ToString("dd MMM yyyy, HH:mm");
            ModifiedLabel.Text = board.ModifiedDate.ToString("dd MMM yyyy, HH:mm");
        }

        private void OnOpenClicked(object sender, EventArgs e)
        {
            Close("open");
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            Close("edit");
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            Close(null);
        }
    }
}
