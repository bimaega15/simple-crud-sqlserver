using MiniTrello.ViewModels;

namespace MiniTrello.Views
{
    public partial class CardDetailPage : ContentPage
    {
        public CardDetailPage(CardDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
