using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniTrello.Models;
using MiniTrello.Persistences.Contracts;

namespace MiniTrello.ViewModels
{
    public partial class CardDetailViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ICardItemPersistence _cardPersistence;

        public CardDetailViewModel(ICardItemPersistence cardPersistence)
        {
            _cardPersistence = cardPersistence;
        }

        [ObservableProperty]
        int boardId;

        [ObservableProperty]
        int cardId;

        [ObservableProperty]
        string cardTitle;

        [ObservableProperty]
        string description;

        [ObservableProperty]
        int selectedStatus;

        [ObservableProperty]
        int selectedPriority;

        [ObservableProperty]
        DateTime dueDate = DateTime.Today.AddDays(7);

        [ObservableProperty]
        bool isEditMode;

        public List<string> StatusOptions => new() { "Backlog", "In Progress", "Q.A", "Completed" };
        public List<string> PriorityOptions => new() { "Low", "Medium", "High" };

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("boardId"))
                BoardId = int.Parse(query["boardId"].ToString());
            if (query.ContainsKey("cardId"))
                CardId = int.Parse(query["cardId"].ToString());
        }

        [RelayCommand]
        async Task InitPage()
        {
            if (CardId > 0)
            {
                IsEditMode = true;
                Title = "Edit Card";
                var card = await _cardPersistence.GetCardAsync(CardId);
                if (card != null)
                {
                    CardTitle = card.Title;
                    Description = card.Description;
                    SelectedStatus = card.Status;
                    SelectedPriority = card.Priority;
                    DueDate = card.DueDate ?? DateTime.Today.AddDays(7);
                }
            }
            else
            {
                IsEditMode = false;
                Title = "New Card";
                DueDate = DateTime.Today.AddDays(7);
            }
        }

        [RelayCommand]
        async Task SaveCard()
        {
            if (string.IsNullOrWhiteSpace(CardTitle))
            {
                await Shell.Current.DisplayAlert("Validation", "Card title is required.", "OK");
                return;
            }

            if (IsEditMode)
            {
                var card = await _cardPersistence.GetCardAsync(CardId);
                card.Title = CardTitle;
                card.Description = Description;
                card.Status = SelectedStatus;
                card.Priority = SelectedPriority;
                card.DueDate = DueDate;
                await _cardPersistence.UpdateItemAsync(card);
            }
            else
            {
                var card = new CardItem
                {
                    BoardId = BoardId,
                    Title = CardTitle,
                    Description = Description ?? "",
                    Status = SelectedStatus,
                    Priority = SelectedPriority,
                    DueDate = DueDate
                };
                await _cardPersistence.AddItemAsync(card);
            }

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
