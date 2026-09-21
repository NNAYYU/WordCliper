using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using WOrdCliper.Core;
using System.Runtime.CompilerServices;
namespace WordCliper.ViewModels;
//データと画面の同期
public class MainViewModel : INotifyPropertyChanged
{
    private readonly string _storagePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "pinned_words.json"
    );

    //画面変更の通知
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<string> _pinnedItems = new();
    public ObservableCollection<string> PinnedItems
    {
        get => _pinnedItems;
        set
        {
            if (_pinnedItems != value)
            {

                _pinnedItems = value;
                OnPropertyChanged();
            }
        }
    }
    //入力テキストのプロパティ
    private string _inputText = string.Empty;
    public string InputText
    {
        get => _inputText;
        set
        {
            if (_inputText != value)
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }
    }
    //コマンドプロパティ
    public ICommand AddPinnedWordCommand { get; }
    public ICommand RemovePinnedWordCommand { get; }

    public MainViewModel()
    {
        AddPinnedWordCommand = new RelayCommand(AddPinnedWord);
        RemovePinnedWordCommand = new RelayCommand<string>(RemovePinnedWord);

        LoadPinnedWords();
    }
    public void LoadPinnedWords()
    {
        string[] loaded = Strage.load_word(_storagePath);
        PinnedItems = new ObservableCollection<string>(loaded);
    }

    private void AddPinnedWord()
    {
        if (string.IsNullOrWhiteSpace(InputText)) return;

        string[] updatedArray = TextFilter.sanitizeAndDeduplicate(InputText, PinnedItems);
        PinnedItems = new ObservableCollection<string>(updatedArray);
        Strage.save_word(_storagePath, PinnedItems);

        InputText = string.Empty;
    }
    private void RemovePinnedWord(string? item)
    {
        if (item != null && PinnedItems.Contains(item))
        {
            PinnedItems.Remove(item);
            Strage.save_word(_storagePath, PinnedItems);
        }
    }
    //イベントあったときのメソッド
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
