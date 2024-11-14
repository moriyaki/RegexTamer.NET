using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
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
            _Messenger.Register<LoadFileMessage>(this, (_, _) => LoadRichTextBoxContentWithDialog());
            _Messenger.Register<SaveFileMessage>(this, (_, _) => SaveRichTextBoxContentWithDialog());
            _Messenger.Register<SearchTextMessage>(this, (_, m) => HighlightText(m.RegexText));
            _Messenger.Register<ReplaceTestMessage>(this, (_, m) =>
            {
                if (!m.IsReplaced)
                {
                    ReplaceOriginalContent(m.RegexPattern, m.ReplacementPattern, false);
                }
                else
                {
                    RestoreOriginalContent(m.RegexPattern);
                }
            });
            _Messenger.Register<FixReplaceMessage>(this, (_, m) => ReplaceOriginalContent(m.RegexPattern, m.ReplacementPattern, true));
            _Messenger.Register<FontFamilyAndSizeChangedMessage>(this, (_, _) => SetDefaultLineSpacingAndFontSettings());
            SetDefaultLineSpacingAndFontSettings();
            DataObject.AddPastingHandler(SearchOrReplaceRichBox, OnRichTextBoxPasting);
        }

        /// <summary>
        /// Use Ctrl + mouse wheel to zoom in and out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                var _Settings = Ioc.Default.GetService<ISettings>() ?? throw new NullReferenceException(nameof(ISettings));
                if (e.Delta > 0) { _Settings.FontSizeLarge(); }
                else { _Settings.FontSizeSmall(); }
                SetDefaultLineSpacingAndFontSettings();
                e.Handled = true;
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }

        /// <summary>
        /// Set Default line space
        /// </summary>
        private void SetDefaultLineSpacingAndFontSettings()
        {
            var _Settings = Ioc.Default.GetService<ISettings>() ?? throw new NullReferenceException(nameof(ISettings));
            foreach (Block block in SearchOrReplaceRichBox.Document.Blocks)
            {
                if (block is Paragraph paragraph)
                {
                    paragraph.LineHeight = 1.0;
                    paragraph.Margin = new Thickness(0);
                    paragraph.FontFamily = _Settings.CurrentFont;
                    paragraph.FontSize = _Settings.FontSize;
                }
            }
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
            SetDefaultLineSpacingAndFontSettings();
            var _MainWindowViewModel = Ioc.Default.GetService<IMainWinodowViewModel>() ?? throw new NullReferenceException(nameof(IMainWinodowViewModel));
            _MainWindowViewModel.SearchAndReplaceModified();
        }

        /// <summary>
        /// Highlight Regex search pattern
        /// </summary>
        /// <param name="regexPattern">Regex pattern</param>
        private void HighlightText(string regexPattern)
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
            SetDefaultLineSpacingAndFontSettings();
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

        private string originalContent = string.Empty;
        /// <summary>
        /// Replace original target text
        /// </summary>
        /// <param name="regexPattern">Regex pattern</param>
        /// <param name="replacementPattern">Replace text pattern<</param>
        /// <param name="isFixed">Is replaced text fixed?</param>
        private void ReplaceOriginalContent(string regexPattern, string replacementPattern, bool isFixed)
        {
            var range = new TextRange(SearchOrReplaceRichBox.Document.ContentStart, SearchOrReplaceRichBox.Document.ContentEnd);
            originalContent = range.Text;

            var replacedContents = new HashSet<string>();
            string result = Regex.Replace(range.Text, regexPattern, match =>
            {
                string replaced = match.Result(replacementPattern);
                replacedContents.Add(replaced);
                return replaced;
            });

            range.Text = Regex.Replace(range.Text, regexPattern, replacementPattern);

            TextPointer current = SearchOrReplaceRichBox.Document.ContentStart;
            while (current?.CompareTo(SearchOrReplaceRichBox.Document.ContentEnd) < 0)
            {
                var text = current.GetTextInRun(LogicalDirection.Forward);

                foreach (string replaced in replacedContents)
                {
                    HighlightOccurrencesInRun(current, replaced);
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }

            SetDefaultLineSpacingAndFontSettings();
            if (!isFixed) return;

            replacedContents.Clear();
        }
        /*
        The price of the book is $20, while the notebook costs $10.
        */

        /// <summary>
        /// Restore original tartet text
        /// </summary>
        private void RestoreOriginalContent(string regexPattern)
        {
            if (string.IsNullOrEmpty(originalContent)) return;
            _ = new TextRange(SearchOrReplaceRichBox.Document.ContentStart, SearchOrReplaceRichBox.Document.ContentEnd)
            {
                Text = originalContent
            };

            HighlightText(regexPattern);
        }

        /// <summary>
        /// Load data from file to RichTextBox
        /// </summary>
        private void LoadRichTextBoxContentWithDialog()
        {
            // OpenFileDialogを使ってファイルを選択
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                var range = new TextRange(SearchOrReplaceRichBox.Document.ContentStart, SearchOrReplaceRichBox.Document.ContentEnd);
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                range.Load(fileStream, DataFormats.Text);
                SetDefaultLineSpacingAndFontSettings();
            }
        }

        /// <summary>
        /// Save data from RichTextBox to file
        /// </summary>
        private void SaveRichTextBoxContentWithDialog()
        {
            // SaveFileDialogを使って保存先を選択
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                var range = new TextRange(SearchOrReplaceRichBox.Document.ContentStart, SearchOrReplaceRichBox.Document.ContentEnd);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                range.Save(fileStream, DataFormats.Text);
            }
        }

        private void TextBoxReplace_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var _MainWindowViewModel = Ioc.Default.GetService<IMainWinodowViewModel>() ?? throw new NullReferenceException(nameof(IMainWinodowViewModel));
                _MainWindowViewModel.ButtonReplaceTestOrCancelCommand.Execute(this);
            }
        }
    }
}