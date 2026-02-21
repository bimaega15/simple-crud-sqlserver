using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniTrello.Models;
using MiniTrello.Persistences.Contracts;
using System.Collections.ObjectModel;

namespace MiniTrello.ViewModels
{
    public partial class BoardDetailViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ICardItemPersistence _cardPersistence;

        public BoardDetailViewModel(ICardItemPersistence cardPersistence)
        {
            _cardPersistence = cardPersistence;
        }

        [ObservableProperty]
        int boardId;

        [ObservableProperty]
        string boardColor = "#4A90D9";

        [ObservableProperty]
        int selectedTabIndex;

        [ObservableProperty]
        int backlogCount;

        [ObservableProperty]
        int inProgressCount;

        [ObservableProperty]
        int qaCount;

        [ObservableProperty]
        int completedCount;

        public ObservableCollection<CardItem> Cards { get; set; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("boardId"))
                BoardId = int.Parse(query["boardId"].ToString());
            if (query.ContainsKey("boardName"))
                Title = Uri.UnescapeDataString(query["boardName"].ToString());
            if (query.ContainsKey("boardColor"))
                BoardColor = Uri.UnescapeDataString(query["boardColor"].ToString());
        }

        [RelayCommand]
        async Task InitPage()
        {
            await LoadCards();
        }

        [RelayCommand]
        async Task LoadCards()
        {
            IsBusy = true;
            try
            {
                var allCards = await _cardPersistence.GetCardsByBoardAsync(BoardId);
                var allList = allCards.ToList();

                // Update counts
                BacklogCount = allList.Count(c => c.Status == 0);
                InProgressCount = allList.Count(c => c.Status == 1);
                QaCount = allList.Count(c => c.Status == 2);
                CompletedCount = allList.Count(c => c.Status == 3);

                // Filter by selected tab
                var filtered = allList.Where(c => c.Status == SelectedTabIndex).OrderBy(c => c.Order);
                Cards.Clear();
                foreach (var card in filtered)
                    Cards.Add(card);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task TabChanged(object param)
        {
            if (param != null && int.TryParse(param.ToString(), out int tabIndex))
                SelectedTabIndex = tabIndex;
            await LoadCards();
        }

        [RelayCommand]
        async Task AddCard()
        {
            await Shell.Current.GoToAsync($"carddetail?boardId={BoardId}&cardId=0");
        }

        [RelayCommand]
        async Task EditCard(CardItem card)
        {
            if (card == null) return;
            await Shell.Current.GoToAsync($"carddetail?boardId={BoardId}&cardId={card.Id}");
        }

        [RelayCommand]
        async Task DeleteCard(CardItem card)
        {
            if (card == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Delete Card",
                $"Delete '{card.Title}'?", "Yes", "No");
            if (!confirm) return;
            await _cardPersistence.DeleteCardAsync(card.Id);
            await LoadCards();
        }

        [RelayCommand]
        async Task MoveCardForward(CardItem card)
        {
            if (card == null || card.Status >= 3) return;
            await _cardPersistence.UpdateCardStatusAsync(card.Id, card.Status + 1);
            await LoadCards();
        }

        [RelayCommand]
        async Task MoveCardBackward(CardItem card)
        {
            if (card == null || card.Status <= 0) return;
            await _cardPersistence.UpdateCardStatusAsync(card.Id, card.Status - 1);
            await LoadCards();
        }
    }
}
