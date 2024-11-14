using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using RegexTamer.NET.ViewModels;

namespace RegexTamer.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<IMainWinodowViewModel>();
            var _Messenger = Ioc.Default.GetService<IMessenger>() ?? throw new NullReferenceException(nameof(IMessenger));
            _Messenger.Register<SearchTextMessage>(this, (_, m) => HighlightText(m.Text));
            DataObject.AddPastingHandler(SearchOrReplaceRichBox, OnRichTextBoxPasting);
        }

        /// <summary>
        /// Remove formatting when pasting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRichTextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.SourceDataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }
            var newText = e.SourceDataObject.GetData(DataFormats.Text) as string;
            if (string.IsNullOrEmpty(newText)) return;

            // Only Plain Text Pasting
            e.CancelCommand();
            //SearchOrReplaceRichBox.CaretPosition.InsertTextInRun(text);
            var selectedTextRange = new TextRange(SearchOrReplaceRichBox.Selection.Start, SearchOrReplaceRichBox.Selection.End)
            {
                Text = newText
            };
            selectedTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
            var _MainWindowViewModel = Ioc.Default.GetService<IMainWinodowViewModel>() ?? throw new NullReferenceException(nameof(IMainWinodowViewModel));
            _MainWindowViewModel.SearchAndReplaceModified();
        }

        /// <summary>
        /// Highlight Regex search pattern
        /// </summary>
        /// <param name="regexPattern">Regex pattern</param>
        public void HighlightText(string regexPattern)
        {
            // 既存のハイライトをクリア
            ClearHighlight();

            if (string.IsNullOrEmpty(regexPattern)) return;
            var regex = new Regex(regexPattern);

            TextPointer current = SearchOrReplaceRichBox.Document.ContentStart;
            while (current?.CompareTo(SearchOrReplaceRichBox.Document.ContentEnd) < 0)
            {
                var text = current.GetTextInRun(LogicalDirection.Forward);

                foreach (Match match in regex.Matches(text))
                {
                    HighlightOccurrencesInRun(current, match.Value);
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        /// <summary>
        ///  Highlight keyword
        /// </summary>
        /// <param name="current">TextPointer</param>
        /// <param name="keyword">Highlight keyword</param>
        private static void HighlightOccurrencesInRun(TextPointer current, string keyword)
        {
            if (current.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.Text) return;

            string text = current.GetTextInRun(LogicalDirection.Forward);

            int index = text.IndexOf(keyword);
            if (index < 0) return;

            var start = current.GetPositionAtOffset(index);
            if (start == null) return;

            var end = start.GetPositionAtOffset(keyword.Length);
            if (end == null) return;

            var highlightRange = new TextRange(start, end);
            highlightRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Cyan);
        }

        /// <summary>
        /// Clear Regex search text highlight
        /// </summary>
        private void ClearHighlight()
        {
            var clearRange = new TextRange(SearchOrReplaceRichBox.Document.ContentStart, SearchOrReplaceRichBox.Document.ContentEnd);
            clearRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
        }
    }
}