using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniTrello.Models;
using MiniTrello.Persistences.Contracts;
using MiniTrello.Views;
using System.Collections.ObjectModel;

namespace MiniTrello.ViewModels
{
    public partial class BoardListViewModel : ViewModelBase
    {
        private readonly IBoardPersistence _boardPersistence;
        private readonly ICardItemPersistence _cardPersistence;

        public BoardListViewModel(IBoardPersistence boardPersistence, ICardItemPersistence cardPersistence)
        {
            Title = "My Boards";
            _boardPersistence = boardPersistence;
            _cardPersistence = cardPersistence;
        }

        public ObservableCollection<Board> Boards { get; set; } = new();
        public ObservableCollection<Board> BacklogBoards { get; set; } = new();
        public ObservableCollection<Board> InProgressBoards { get; set; } = new();
        public ObservableCollection<Board> QaBoards { get; set; } = new();
        public ObservableCollection<Board> CompletedBoards { get; set; } = new();

        [ObservableProperty]
        bool isRefreshing;

        // Drag & Drop state
        private Board? _draggingBoard;

        [ObservableProperty]
        string backlogDropColor = "#1A1A2E";

        [ObservableProperty]
        string inProgressDropColor = "#1A1A2E";

        [ObservableProperty]
        string qaDropColor = "#1A1A2E";

        [ObservableProperty]
        string completedDropColor = "#1A1A2E";

        [RelayCommand]
        void DragStarting(Board board)
        {
            _draggingBoard = board;
        }

        [RelayCommand]
        async Task Drop(object parameter)
        {
            if (_draggingBoard == null) return;

            if (!int.TryParse(parameter?.ToString(), out int targetStatus))
            {
                _draggingBoard = null;
                ResetDropHighlights();
                return;
            }

            if (_draggingBoard.Status == targetStatus)
            {
                _draggingBoard = null;
                ResetDropHighlights();
                return;
            }

            _draggingBoard.Status = targetStatus;
            await _boardPersistence.UpdateItemAsync(_draggingBoard);
            _draggingBoard = null;
            ResetDropHighlights();
            await LoadBoards();
        }

        [RelayCommand]
        void DragOver(object parameter)
        {
            ResetDropHighlights();
            switch (parameter?.ToString())
            {
                case "0": BacklogDropColor = "#252540"; break;
                case "1": InProgressDropColor = "#252540"; break;
                case "2": QaDropColor = "#252540"; break;
                case "3": CompletedDropColor = "#252540"; break;
            }
        }

        [RelayCommand]
        void DragLeave(object parameter)
        {
            switch (parameter?.ToString())
            {
                case "0": BacklogDropColor = "#1A1A2E"; break;
                case "1": InProgressDropColor = "#1A1A2E"; break;
                case "2": QaDropColor = "#1A1A2E"; break;
                case "3": CompletedDropColor = "#1A1A2E"; break;
            }
        }

        private void ResetDropHighlights()
        {
            BacklogDropColor = "#1A1A2E";
            InProgressDropColor = "#1A1A2E";
            QaDropColor = "#1A1A2E";
            CompletedDropColor = "#1A1A2E";
        }

        [RelayCommand]
        async Task InitPage()
        {
            await LoadBoards();
        }

        [RelayCommand]
        async Task LoadBoards()
        {
            IsBusy = true;
            try
            {
                var items = await _boardPersistence.GetItemsAsync();
                var allList = items.ToList();

                Boards.Clear();
                BacklogBoards.Clear();
                InProgressBoards.Clear();
                QaBoards.Clear();
                CompletedBoards.Clear();

                var sorted = allList.OrderByDescending(b => b.CreatedDate);
                foreach (var item in sorted)
                {
                    Boards.Add(item);
                    switch (item.Status)
                    {
                        case 1: InProgressBoards.Add(item); break;
                        case 2: QaBoards.Add(item); break;
                        case 3: CompletedBoards.Add(item); break;
                        default: BacklogBoards.Add(item); break;
                    }
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task AddBoard()
        {
            var popup = new BoardFormPopup("New Board");
            var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as BoardFormResult;
            if (result == null) return;

            var board = new Board
            {
                Name = result.Name,
                Description = result.Description,
                Status = result.Status,
                Color = GetRandomColor()
            };
            await _boardPersistence.AddItemAsync(board);
            await LoadBoards();
        }

        [RelayCommand]
        async Task EditBoard(Board board)
        {
            if (board == null) return;

            var popup = new BoardFormPopup("Edit Board", board.Name, board.Description, board.Status);
            var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as BoardFormResult;
            if (result == null) return;

            board.Name = result.Name;
            board.Description = result.Description;
            board.Status = result.Status;
            await _boardPersistence.UpdateItemAsync(board);
            await LoadBoards();
        }

        [RelayCommand]
        async Task DeleteBoard(Board board)
        {
            if (board == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Delete Board",
                $"Are you sure you want to delete '{board.Name}' and all its cards?", "Yes", "No");
            if (!confirm) return;

            // Delete all cards in the board first
            var cards = await _cardPersistence.GetCardsByBoardAsync(board.Id);
            foreach (var card in cards)
                await _cardPersistence.DeleteCardAsync(card.Id);

            await _boardPersistence.DeleteBoardAsync(board.Id);
            await LoadBoards();
        }

        [RelayCommand]
        async Task OpenBoard(Board board)
        {
            if (board == null) return;

            var popup = new BoardDetailPopup(board);
            var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup) as string;

            if (result == "open")
            {
                await Shell.Current.GoToAsync($"boarddetail?boardId={board.Id}&boardName={Uri.EscapeDataString(board.Name)}&boardColor={Uri.EscapeDataString(board.Color)}");
            }
            else if (result == "edit")
            {
                await EditBoard(board);
            }
        }

        private string GetRandomColor()
        {
            var colors = new[] { "#6C5CE7", "#00B894", "#E17055", "#0984E3", "#FDCB6E", "#E84393", "#00CEC9", "#636E72" };
            return colors[Random.Shared.Next(colors.Length)];
        }
    }
}
