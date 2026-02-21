using MiniTrello.ViewModels;

namespace MiniTrello.Views
{
    public partial class BoardDetailPage : ContentPage
    {
        public BoardDetailPage(BoardDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
