using CommunityToolkit.Maui.Views;

namespace MiniTrello.Views
{
    public partial class BoardFormPopup : Popup
    {
        public BoardFormPopup(string title = "New Board", string name = "", string description = "", int status = 0)
        {
            InitializeComponent();
            PopupTitle.Text = title;
            NameEntry.Text = name;
            DescriptionEditor.Text = description;
            StatusPicker.SelectedIndex = status;
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text))
            {
                NameEntry.Placeholder = "Board name is required!";
                return;
            }

            Close(new BoardFormResult
            {
                Name = NameEntry.Text.Trim(),
                Description = DescriptionEditor.Text?.Trim() ?? "",
                Status = StatusPicker.SelectedIndex >= 0 ? StatusPicker.SelectedIndex : 0
            });
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(null);
        }
    }

    public class BoardFormResult
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
