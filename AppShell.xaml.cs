using MiniTrello.Views;

namespace MiniTrello;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("boarddetail", typeof(BoardDetailPage));
        Routing.RegisterRoute("carddetail", typeof(CardDetailPage));
    }
}
