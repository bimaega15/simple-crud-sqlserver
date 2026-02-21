using MiniTrello.ViewModels;

namespace MiniTrello.Views
{
    public partial class BoardListPage : ContentPage
    {
        public BoardListPage(BoardListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
