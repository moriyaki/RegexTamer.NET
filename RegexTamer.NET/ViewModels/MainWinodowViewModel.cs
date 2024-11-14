using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace RegexTamer.NET.ViewModels
{
    #region RegexErrorStatus

    public enum RegexErrorStatus
    {
        None,
        Empty,
        AlreadyRegistered,
        AlternationHasComment,
        AlternationHasMalformedCondition,
        AlternationHasMalformedReference,
        AlternationHasNamedCapture,
        AlternationHasTooManyConditions,
        AlternationHasUndefinedReference,
        CaptureGroupNameInvalid,
        CaptureGroupOfZero,
        ExclusionGroupNotLast,
        InsufficientClosingParentheses,
        InsufficientOpeningParentheses,
        InsufficientOrInvalidHexDigits,
        InvalidGroupingConstruct,
        InvalidUnicodePropertyEscape,
        MalformedNamedReference,
        MalformedUnicodePropertyEscape,
        MissingControlCharacter,
        NestedQuantifiersNotParenthesized,
        QuantifierAfterNothing,
        QuantifierOrCaptureGroupOutOfRange,
        ReversedCharacterRange,
        ReversedQuantifierRange,
        ShorthandClassInCharacterRange,
        UndefinedNamedReference,
        UndefinedNumberedReference,
        UnescapedEndingBackslash,
        Unknown,
        UnrecognizedControlCharacter,
        UnrecognizedEscape,
        UnrecognizedUnicodeProperty,
        UnterminatedBracket,
        UnterminatedComment,
    }
    #endregion RegexErrorStatus

    #region Interface
    public interface IMainWinodowViewModel
    {
        /// <summary>
        /// Window Top position
        /// </summary>
        double WindowTop { get; set; }

        /// <summary>
        /// Window Left position
        /// </summary>
        double WindowLeft { get; set; }

        /// <summary>
        /// Window Width
        /// </summary>
        double WindowWidth { get; set; }

        /// <summary>
        /// Window Height
        /// </summary>
        double WindowHeight { get; set; }

        /// <summary>
        /// Current Font Family
        /// </summary>
        public FontFamily CurrentFontFamily { get; set; }

        /// <summary>
        /// Current Font Size
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// Regular Expression Error Status
        /// </summary>
        RegexErrorStatus RegexErrorStatus { get; set; }

        /// <summary>
        /// Search string or Replace string modified
        /// </summary>
        void SearchAndReplaceModified();

        /// <summary>
        /// Replace Test Button Event or Cancel Replace Test Button Event
        /// </summary>
        RelayCommand ButtonReplaceTestOrCancelCommand { get; set; }
    }
    #endregion Interface

    public partial class MainWinodowViewModel : ObservableObject, IMainWinodowViewModel
    {
        #region Window Binding
        /// <summary>
        /// Window Top position
        /// </summary>
        private double _WindowTop = 100d;
        public double WindowTop
        {
            get => _WindowTop;
            set
            {
                SetProperty(ref _WindowTop, value);
                _Settings.WindowTop = value;
            }
        }

        /// <summary>
        /// Window Left position
        /// </summary>
        private double _WindowLeft = 100d;
        public double WindowLeft
        {
            get => _WindowLeft;
            set
            {
                SetProperty(ref _WindowLeft, value);
                _Settings.WindowLeft = value;
            }
        }

        /// <summary>
        /// Window Width
        /// </summary>
        private double _WindowWidth = 800d;
        public double WindowWidth
        {
            get => _WindowWidth;
            set
            {
                SetProperty(ref _WindowWidth, value);
                _Settings.WindowWidth = value;
            }
        }

        /// <summary>
        /// Window Height
        /// </summary>
        private double _WindowHeight = 450d;
        public double WindowHeight
        {
            get => _WindowHeight;
            set
            {
                SetProperty(ref _WindowHeight, value);
                _Settings.WindowHeight = value;
            }
        }

        /// <summary>
        /// Language culture name
        /// </summary>
        private string _CultureName = "en";
        public string CultureName
        {
            get => _CultureName;
            set
            {
                _CultureName = value;
                _Settings.CultureName = value;
            }
        }

        /// <summary>
        /// Current Font Family
        /// </summary>
        private FontFamily _CurrentFontFamily = SystemFonts.MessageFontFamily;
        public FontFamily CurrentFontFamily
        {
            get => _CurrentFontFamily;
            set
            {
                if (_CurrentFontFamily.Source == value.Source) { return; }
                SetProperty(ref _CurrentFontFamily, value);
                _Settings.CurrentFont = value;
            }
        }

        /// <summary>
        /// Current Font Size
        /// </summary>
        private double _FontSize = SystemFonts.MessageFontSize;

        public double FontSize
        {
            get => _FontSize;
            set
            {
                if (_FontSize == value) { return; }
                SetProperty(ref _FontSize, value);
                _Settings.FontSize = value;
            }
        }

        #endregion Window Binding

        #region Label Data Binding
        /// <summary>
        /// Menu - File String
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string MenuFile
        {
            get => ResourceService.GetString("MenuFile");
        }

        /// <summary>
        /// Menu - File - Open File String
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string MenuOpenFile
        {
            get => ResourceService.GetString("MenuOpenFile");
        }

        /// <summary>
        /// Menu - File - Write File String
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string MenuWriteFile
        {
            get => ResourceService.GetString("MenuWriteFile");
        }

        /// <summary>
        /// Menu - File - Exit String
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string MenuExit
        {
            get => ResourceService.GetString("MenuExit");
        }

        /// <summary>
        /// Menu - File - Exit String
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string MenuFont
        {
            get => ResourceService.GetString("MenuFont");
        }

        /// <summary>
        /// Label "Regular Expression"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelRegularExpression
        {
            get => ResourceService.GetString("Label_RegularExpression");
        }

        /// <summary>
        /// Label "Replace"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelReplace
        {
            get => ResourceService.GetString("Label_Replace");
        }

        /// <summary>
        /// Label "Regular Expression Error"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelRegexError
        {
            get => ResourceService.GetString("Label_RegexError");
        }

        /// <summary>
        /// Label "Target Data"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelTargetData
        {
            get => ResourceService.GetString("Label_TargetData");
        }

        /// <summary>
        /// Button "Search"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string ButtonSearch
        {
            get => ResourceService.GetString("Button_Search");
        }

        /// <summary>
        /// Button "Replace Test" or "Cancel Replace Test"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string ButtonReplaceTestOrCancel
        {
            get
            {
                if (_IsTestRunning)
                {
                    return ResourceService.GetString("Button_CancelReplaceTest");
                }
                else
                {
                    return ResourceService.GetString("Button_ReplaceTest");
                }
            }
        }

        /// <summary>
        /// Button "Execute Replace"
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string ButtonExecuteReplace
        {
            get => ResourceService.GetString("Button_ExecuteReplace");
        }
        #endregion Label Data Binding

        #region Data Binding
        /// <summary>
        /// Regular Expression String
        /// </summary>
        private string _SearchText = string.Empty;
        public string SearchText
        {
            get => _SearchText;
            set
            {
                SetProperty(ref _SearchText, value);
                SearchAndReplaceModified();
            }
        }

        /// <summary>
        /// Regular Expression Replace String
        /// </summary>
        private string _ReplaceText = string.Empty;
        public string ReplaceText
        {
            get => _ReplaceText;
            set
            {
                SetProperty(ref _ReplaceText, value);
                SearchAndReplaceModified();
            }
        }

        /// <summary>
        /// Regex Error Background Color
        /// </summary>
        [ObservableProperty]
        private Brush _RegexErrorBackground = Brushes.Transparent;

        /// <summary>
        /// Regular Expression Error Output String
        /// </summary>
        [ObservableProperty]
        private string _RegexErrorOutput = string.Empty;

        /// <summary>
        /// Regular Expression Error Status
        /// </summary>
        private RegexErrorStatus _RegexErrorStatus = RegexErrorStatus.None;

        public RegexErrorStatus RegexErrorStatus
        {
            get => _RegexErrorStatus;
            set
            {
                SetProperty(ref _RegexErrorStatus, value);
                if (value == RegexErrorStatus.None)
                {
                    RegexErrorBackground = Brushes.Transparent;
                    RegexErrorOutput = string.Empty;
                    return;
                }
                RegexErrorBackground = Brushes.Pink;
                switch (value)
                {
                    case RegexErrorStatus.Empty:
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_Empty")}";
                        break;
                    case RegexErrorStatus.AlternationHasMalformedCondition:
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_AlternationHasMalformedCondition")}";
                        break;

                    case RegexErrorStatus.AlternationHasMalformedReference:   // "(x)(?(3x|y)"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_AlternationHasMalformedReference")}";
                        break;

                    case RegexErrorStatus.AlternationHasNamedCapture:         // "(?(?<x>)true|false)"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_AlternationHasNamedCapture")}";
                        break;

                    case RegexErrorStatus.AlternationHasTooManyConditions:    // "(?(foo)a|b|c)"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_AlternationHasTooManyConditions")}";
                        break;

                    case RegexErrorStatus.AlternationHasUndefinedReference:   // "(?(1))"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_AlternationHasUndefinedReference")}";
                        break;

                    case RegexErrorStatus.CaptureGroupNameInvalid:            // "(?'x)"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_CaptureGroupNameInvalid")}";
                        break;

                    case RegexErrorStatus.CaptureGroupOfZero:                 // "(?'0'foo)"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_CaptureGroupOfZero")}";
                        break;

                    case RegexErrorStatus.ExclusionGroupNotLast:              // "[a-z-[xy]A]"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_ExclusionGroupNotLast")}";
                        break;

                    case RegexErrorStatus.InsufficientClosingParentheses:     // "(((foo))"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_InsufficientClosingParentheses")}";
                        break;

                    case RegexErrorStatus.InsufficientOpeningParentheses:     // "((foo)))"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_InsufficientOpeningParentheses")}";
                        break;

                    case RegexErrorStatus.InsufficientOrInvalidHexDigits:     // @"\xr"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_InsufficientOrInvalidHexDigits")}";
                        break;

                    case RegexErrorStatus.InvalidGroupingConstruct:           // "(?"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_InvalidGroupingConstruct")}";
                        break;

                    case RegexErrorStatus.InvalidUnicodePropertyEscape:       // @"\p{ L}"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_InvalidUnicodePropertyEscape")}";
                        break;

                    case RegexErrorStatus.MalformedNamedReference:            // @"\k<"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_MalformedNamedReference")}";
                        break;

                    case RegexErrorStatus.MalformedUnicodePropertyEscape:     // @"\p {L}"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_MalformedUnicodePropertyEscape")}";
                        break;

                    case RegexErrorStatus.MissingControlCharacter:            // @"\c"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_MissingControlCharacter")}";
                        break;

                    case RegexErrorStatus.NestedQuantifiersNotParenthesized:  // "abc**"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_NestedQuantifiersNotParenthesized")}";
                        break;

                    case RegexErrorStatus.QuantifierAfterNothing:             // "((*foo)bar)"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_QuantifierAfterNothing")}";
                        break;

                    case RegexErrorStatus.QuantifierOrCaptureGroupOutOfRange: // "x{234567899988}"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_QuantifierOrCaptureGroupOutOfRange")}";
                        break;

                    case RegexErrorStatus.ReversedCharacterRange:             // "[z-a]"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_ReversedCharacterRange")}";
                        break;

                    case RegexErrorStatus.ReversedQuantifierRange:            // "abc{3,0}"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_ReversedQuantifierRange")}";
                        break;

                    case RegexErrorStatus.ShorthandClassInCharacterRange:     // @"[a-\w]"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_ShorthandClassInCharacterRange")}";
                        break;

                    case RegexErrorStatus.UndefinedNamedReference:            // @"\k<x>"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UndefinedNamedReference")}";
                        break;

                    case RegexErrorStatus.UndefinedNumberedReference:         // @"(x)\2"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UndefinedNumberedReference")}";
                        break;

                    case RegexErrorStatus.UnescapedEndingBackslash:           // @"foo\"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UnescapedEndingBackslash")}";
                        break;

                    case RegexErrorStatus.Unknown:
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_Unknown")}";
                        break;

                    case RegexErrorStatus.UnrecognizedControlCharacter:       // @"\c!"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UnrecognizedControlCharacter")}";
                        break;

                    case RegexErrorStatus.UnrecognizedEscape:                 // @"\C"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UnrecognizedEscape")}";
                        break;

                    case RegexErrorStatus.UnrecognizedUnicodeProperty:        // @"\p{Lll}"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UnrecognizedUnicodeProperty")}";
                        break;

                    case RegexErrorStatus.UnterminatedBracket:                //  "[a-b"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UnterminatedBracket")}";
                        break;

                    case RegexErrorStatus.UnterminatedComment:                // "(?#comment .*"
                        RegexErrorOutput = $"{ResourceService.GetString("LabelRegexError_UnterminatedComment")}";
                        break;
                }
            }
        }

        private bool _IsTestRunning = false;
        #endregion Data Binding

        #region Event Define
        /// <summary>
        /// Language change to English
        /// </summary>
        public RelayCommand ToEnglish { get; set; }

        /// <summary>
        /// Language change to Jaspanese
        /// </summary>
        public RelayCommand ToJapanese { get; set; }

        /// <summary>
        /// Read from file
        /// </summary>
        public RelayCommand OpenFile { get; set; }

        /// <summary>
        /// Write to file
        /// </summary>
        public RelayCommand WriteFile { get; set; }

        /// <summary>
        /// Exit Application
        /// </summary>
        public RelayCommand CloseApplication { get; set; }

        /// <summary>
        /// Font Setting
        /// </summary>
        public RelayCommand FontSelect { get; set; }

        /// <summary>
        /// Replace Test Button Event or Cancel Replace Test Button Event
        /// </summary>
        public RelayCommand ButtonReplaceTestOrCancelCommand { get; set; }

        /// <summary>
        /// Execute Replace Button Event
        /// </summary>
        public RelayCommand ButtonExecuteReplaceCommand { get; set; }
        #endregion Event Define

        private readonly ISettings _Settings;
        private readonly IMessenger _Messenger;
        public MainWinodowViewModel(
            ISettings settings,
            IMessenger messenger)
        {
            _Settings = settings;
            _Settings.LoadSettings();
            _Messenger = messenger;

            WindowTop = _Settings.WindowTop;
            WindowLeft = _Settings.WindowLeft;
            WindowWidth = _Settings.WindowWidth;
            WindowHeight = _Settings.WindowHeight;
            CultureName = _Settings.CultureName;

            ToEnglish = new RelayCommand(() => ChangeCultureInfo("en"));
            ToJapanese = new RelayCommand(() => ChangeCultureInfo("ja"));
            OpenFile = new RelayCommand(() => _Messenger.Send<LoadFileMessage>(new LoadFileMessage()));
            WriteFile = new RelayCommand(() => _Messenger.Send<SaveFileMessage>(new SaveFileMessage()));
            CloseApplication = new RelayCommand(() => Application.Current.Shutdown());
            FontSelect = new RelayCommand(() =>
            {
                var fontSelectWindow = new FontSelect();
                fontSelectWindow.ShowDialog();
            });
            ButtonReplaceTestOrCancelCommand = new RelayCommand(
                () =>
                {
                    _Messenger.Send<ReplaceTestMessage>(new ReplaceTestMessage(_IsTestRunning, SearchText, ReplaceText));
                    _IsTestRunning = !_IsTestRunning;
                    OnPropertyChanged(nameof(ButtonReplaceTestOrCancel));
                    ButtonExecuteReplaceCommand?.NotifyCanExecuteChanged();
                },
                () => !string.IsNullOrEmpty(ReplaceText) && IsRegexConditionCorrent(SearchText)
            );
            ButtonExecuteReplaceCommand = new RelayCommand(
                () =>
                {
                    _Messenger.Send<FixReplaceMessage>(new FixReplaceMessage(SearchText, ReplaceText));
                    _IsTestRunning = !_IsTestRunning;
                    OnPropertyChanged(nameof(ButtonReplaceTestOrCancel));
                },
                () => !string.IsNullOrEmpty(ReplaceText) && _IsTestRunning && IsRegexConditionCorrent(SearchText)
            );

            _Messenger.Register<FontFamilyChangeMessage>(this, (_, m) => CurrentFontFamily = m.Font);
            _Messenger.Register<FontSizeChangeMessage>(this, (_, m) => FontSize = m.FontSize);

            CurrentFontFamily = _Settings.CurrentFont;
            FontSize = _Settings.FontSize;

            ChangeCultureInfo(CultureName);
            IsRegexConditionCorrent(SearchText);
        }

        /// <summary>
        /// Search string or Replace string modified
        /// </summary>
        public void SearchAndReplaceModified()
        {
            var searchText = IsRegexConditionCorrent(SearchText) ? SearchText : string.Empty;
            _Messenger.Send(new SearchTextMessage(searchText));
            ButtonReplaceTestOrCancelCommand.NotifyCanExecuteChanged();
            ButtonExecuteReplaceCommand.NotifyCanExecuteChanged();
        }

        #region Culture Changed
        /// <summary>
        /// Change the language culture and send a change notice to the properties
        /// </summary>
        /// <param name="cultureName"></param>
        private void ChangeCultureInfo(string cultureName)
        {
            ResourceService.ChangeCulture(cultureName);
            CultureName = cultureName;
            OnPropertyChanged(nameof(MenuFile));
            OnPropertyChanged(nameof(MenuOpenFile));
            OnPropertyChanged(nameof(MenuWriteFile));
            OnPropertyChanged(nameof(MenuExit));
            OnPropertyChanged(nameof(LabelRegularExpression));
            OnPropertyChanged(nameof(LabelReplace));
            OnPropertyChanged(nameof(LabelRegexError));
            OnPropertyChanged(nameof(LabelTargetData));
            OnPropertyChanged(nameof(ButtonSearch));
            OnPropertyChanged(nameof(ButtonReplaceTestOrCancel));
            OnPropertyChanged(nameof(ButtonExecuteReplace));
            IsRegexConditionCorrent(SearchText);
        }
        #endregion Culture Changed

        #region Regex Check
        public bool IsRegexConditionCorrent(string pattern)
        {
            // Regex pattern is inputed?
            if (string.IsNullOrEmpty(pattern))
            {
                RegexErrorStatus = RegexErrorStatus.Empty;
                return false;
            }

            // Regex Test
            try
            {
                var regex = new Regex(pattern);
            }
            catch (RegexParseException regException)
            {
                switch (regException.Error)
                {
                    case RegexParseError.AlternationHasComment:
                        RegexErrorStatus = RegexErrorStatus.AlternationHasComment;
                        return false;

                    case RegexParseError.AlternationHasMalformedCondition:
                        RegexErrorStatus = RegexErrorStatus.AlternationHasMalformedCondition;
                        return false;

                    case RegexParseError.AlternationHasMalformedReference:
                        RegexErrorStatus = RegexErrorStatus.AlternationHasMalformedReference;
                        return false;

                    case RegexParseError.AlternationHasNamedCapture:
                        RegexErrorStatus = RegexErrorStatus.AlternationHasNamedCapture;
                        return false;

                    case RegexParseError.AlternationHasTooManyConditions:
                        RegexErrorStatus = RegexErrorStatus.AlternationHasTooManyConditions;
                        return false;

                    case RegexParseError.AlternationHasUndefinedReference:
                        RegexErrorStatus = RegexErrorStatus.AlternationHasUndefinedReference;
                        return false;

                    case RegexParseError.CaptureGroupNameInvalid:
                        RegexErrorStatus = RegexErrorStatus.CaptureGroupNameInvalid;
                        return false;

                    case RegexParseError.CaptureGroupOfZero:
                        RegexErrorStatus = RegexErrorStatus.CaptureGroupOfZero;
                        return false;

                    case RegexParseError.ExclusionGroupNotLast:
                        RegexErrorStatus = RegexErrorStatus.ExclusionGroupNotLast;
                        return false;

                    case RegexParseError.InsufficientClosingParentheses:
                        RegexErrorStatus = RegexErrorStatus.InsufficientClosingParentheses;
                        return false;

                    case RegexParseError.InsufficientOpeningParentheses:
                        RegexErrorStatus = RegexErrorStatus.InsufficientOpeningParentheses;
                        return false;

                    case RegexParseError.InsufficientOrInvalidHexDigits:
                        RegexErrorStatus = RegexErrorStatus.InsufficientOrInvalidHexDigits;
                        return false;

                    case RegexParseError.InvalidGroupingConstruct:
                        RegexErrorStatus = RegexErrorStatus.InvalidGroupingConstruct;
                        return false;

                    case RegexParseError.InvalidUnicodePropertyEscape:
                        RegexErrorStatus = RegexErrorStatus.InvalidUnicodePropertyEscape;
                        return false;

                    case RegexParseError.MalformedNamedReference:
                        RegexErrorStatus = RegexErrorStatus.MalformedNamedReference;
                        return false;

                    case RegexParseError.MalformedUnicodePropertyEscape:
                        RegexErrorStatus = RegexErrorStatus.MalformedUnicodePropertyEscape;
                        return false;

                    case RegexParseError.NestedQuantifiersNotParenthesized:
                        RegexErrorStatus = RegexErrorStatus.NestedQuantifiersNotParenthesized;
                        return false;

                    case RegexParseError.MissingControlCharacter:
                        RegexErrorStatus = RegexErrorStatus.MissingControlCharacter;
                        return false;

                    case RegexParseError.QuantifierAfterNothing:
                        RegexErrorStatus = RegexErrorStatus.QuantifierAfterNothing;
                        return false;

                    case RegexParseError.QuantifierOrCaptureGroupOutOfRange:
                        RegexErrorStatus = RegexErrorStatus.QuantifierOrCaptureGroupOutOfRange;
                        return false;

                    case RegexParseError.ReversedCharacterRange:
                        RegexErrorStatus = RegexErrorStatus.ReversedCharacterRange;
                        return false;

                    case RegexParseError.ReversedQuantifierRange:
                        RegexErrorStatus = RegexErrorStatus.ReversedQuantifierRange;
                        return false;

                    case RegexParseError.ShorthandClassInCharacterRange:
                        RegexErrorStatus = RegexErrorStatus.ShorthandClassInCharacterRange;
                        return false;

                    case RegexParseError.UndefinedNamedReference:
                        RegexErrorStatus = RegexErrorStatus.UndefinedNamedReference;
                        return false;

                    case RegexParseError.UndefinedNumberedReference:
                        RegexErrorStatus = RegexErrorStatus.UndefinedNumberedReference;
                        return false;

                    case RegexParseError.UnescapedEndingBackslash:
                        RegexErrorStatus = RegexErrorStatus.UnescapedEndingBackslash;
                        return false;

                    case RegexParseError.Unknown:
                        RegexErrorStatus = RegexErrorStatus.Unknown;
                        return false;

                    case RegexParseError.UnrecognizedControlCharacter:
                        RegexErrorStatus = RegexErrorStatus.UnrecognizedControlCharacter;
                        return false;

                    case RegexParseError.UnrecognizedEscape:
                        RegexErrorStatus = RegexErrorStatus.UnrecognizedEscape;
                        return false;

                    case RegexParseError.UnrecognizedUnicodeProperty:
                        RegexErrorStatus = RegexErrorStatus.UnrecognizedUnicodeProperty;
                        return false;

                    case RegexParseError.UnterminatedBracket:
                        RegexErrorStatus = RegexErrorStatus.UnterminatedBracket;
                        return false;

                    case RegexParseError.UnterminatedComment:
                        RegexErrorStatus = RegexErrorStatus.UnterminatedComment;
                        return false;
                }
            }
            // Regex parameter is no probrem.
            RegexErrorStatus = RegexErrorStatus.None;
            return true;
        }
        #endregion Regex Check
    }
}
