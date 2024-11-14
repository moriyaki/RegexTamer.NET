using System.Windows;
using System.Windows.Media;

namespace RegexTamer.NET
{
    /// <summary>
    /// Message of Load File
    /// </summary>
    public class LoadFileMessage;

    /// <summary>
    /// Message of Save File
    /// </summary>
    public class SaveFileMessage;

    /// <summary>
    /// Message of Font Family and Font Size changed
    /// </summary>
    public class FontFamilyAndSizeChangedMessage;

    /// <summary>
    /// Message of Close Font Select window
    /// </summary>
    public class CloseFontSelect;

    /// <summary>
    /// Message of Search text to View
    /// </summary>
    public class SearchTextMessage
    {
        public string RegexText { get; set; } = string.Empty;
        public SearchTextMessage() { throw new NotImplementedException(); }
        public SearchTextMessage(string regexText)
        {
            RegexText = regexText;
        }
    }

    /// <summary>
    /// Message of Replace text to View
    /// </summary>
    public class ReplaceTestMessage
    {
        public bool IsReplaced { get; set; } = false;
        public string RegexPattern { get; set; } = string.Empty;
        public string ReplacementPattern { get; set; } = string.Empty;
        public ReplaceTestMessage() { throw new NotImplementedException(nameof(ReplaceTestMessage)); }
        public ReplaceTestMessage(bool isReplaces, string regexPattern, string replacementPattern)
        {
            IsReplaced = isReplaces;
            RegexPattern = regexPattern;
            ReplacementPattern = replacementPattern;
        }
    }
    /// <summary>
    /// Message of Replace Fix to View
    /// </summary>
    public class FixReplaceMessage
    {
        public string RegexPattern { get; set; } = string.Empty;
        public string ReplacementPattern { get; set; } = string.Empty;
        public FixReplaceMessage() { throw new NotImplementedException(nameof(FixReplaceMessage)); }
        public FixReplaceMessage(string regexPattern, string replacementPattern)
        {
            RegexPattern = regexPattern;
            ReplacementPattern = replacementPattern;
        }
    }

    /// <summary>
    /// Message of Font Family changed
    /// </summary>
    public class FontFamilyChangeMessage
    {
        public FontFamily Font = SystemFonts.MessageFontFamily;
        public FontFamilyChangeMessage() { throw new NotImplementedException(nameof(FontFamilyChangeMessage)); }
        public FontFamilyChangeMessage(FontFamily fontFamily)
        {
            Font = fontFamily;
        }
    }

    /// <summary>
    /// Message of Font Size changed
    /// </summary>
    public class FontSizeChangeMessage
    {
        public double FontSize = SystemFonts.MessageFontSize;
        public FontSizeChangeMessage() { throw new NotImplementedException(nameof(FontSizeChangeMessage)); }
        public FontSizeChangeMessage(double fontSize)
        {
            FontSize = fontSize;
        }
    }
}
