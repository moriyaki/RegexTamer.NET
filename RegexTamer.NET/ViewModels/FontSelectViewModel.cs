using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace RegexTamer.NET.ViewModels
{
    public interface IFontSelectViewModel;
    public class FontSelectViewModel : ObservableObject, IFontSelectViewModel
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
        /// <summary>
        /// Window Title
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelFontSelectWindowTitle { get => $"{ResourceService.GetString("Label_WindowTitle")}{ResourceService.GetString("Label_FontSelectWindow")}"; }

        /// <summary>
        /// Label - Font string
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelFontString { get => ResourceService.GetString("Label_FontString"); }

        /// <summary>
        /// Label - Font Size string
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelFontSizeString { get => ResourceService.GetString("Label_FontSize"); }

        /// <summary>
        /// Label - Font Size string
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelFontSample { get => ResourceService.GetString("Label_FontSample"); }

        /// <summary>
        /// Label - Font Accept
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
        public string LabelFontAccept { get => ResourceService.GetString("Label_FontAccept"); }

        /// <summary>
        /// Collection of Font Famiries
        /// </summary>
        public ObservableCollection<FontFamily> FontFamilies { get; }

        /// <summary>
        /// Collection of Font Size
        /// </summary>
        public ObservableCollection<FontSize> FontSizes { get; } = [];

        /// <summary>
        /// Accept Font change
        /// </summary>
        public RelayCommand FontAccept { get; set; }

        private readonly ISettings _Settings;
        private readonly IMessenger _Messenger;
        public FontSelectViewModel(
            ISettings settings,
            IMessenger messenger)
        {
            _Settings = settings;
            _Messenger = messenger;

            WindowTop = _Settings.WindowTop;
            WindowLeft = _Settings.WindowLeft;
            WindowWidth = _Settings.WindowWidth;
            WindowHeight = _Settings.WindowHeight;
            CurrentFontFamily = _Settings.CurrentFont;
            FontSize = _Settings.FontSize;

            FontFamilies = new ObservableCollection<FontFamily>(GetSortedFontFamilies());

            foreach (var fontSize in _Settings.GetSelectableFontSize())
            {
                FontSizes.Add(new FontSize(fontSize));
            }

            FontAccept = new RelayCommand(() =>
            {
                _Settings.CurrentFont = CurrentFontFamily;
                _Settings.FontSize = FontSize;
                _Messenger.Send<FontFamilyAndSizeChangedMessage>(new FontFamilyAndSizeChangedMessage());
                _Messenger.Send<CloseFontSelect>(new CloseFontSelect());
            });
        }

        /// <summary>
        /// Get Fonts (Engllish and Japanese)
        /// </summary>
        /// <returns>Sorted Font list</returns>
        private static IEnumerable<FontFamily> GetSortedFontFamilies()
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            CultureInfo cultureUS = new("en-US");

            // Used to determine duplicate font names
            List<string> uriName = [];
            // List of acquired fonts
            IList<FontFamily> fontFamilyList = [];
            foreach (var font in Fonts.SystemFontFamilies)
            {
                var typefaces = font.GetTypefaces();
                foreach (var typeface in typefaces)
                {
                    _ = typeface.TryGetGlyphTypeface(out GlyphTypeface glyphType);
                    if (glyphType == null) continue;

                    // If the font does not have a Japanese name, the English name
                    string fontName = glyphType.Win32FamilyNames[culture] ?? glyphType.Win32FamilyNames[cultureUS];

                    // Duplicate judgment by font name
                    var uri = glyphType.FontUri;
                    if (!uriName.Any(f => f == fontName))
                    {
                        uriName.Add(fontName);
                        fontFamilyList.Add(new(uri, fontName));
                    }
                }
            }
            return fontFamilyList.OrderBy(family => family.Source);
        }
    }
}
