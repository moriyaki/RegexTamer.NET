namespace RegexTamer.NET
{
    /// <summary>
    /// 検索文字列を送るメッセージ
    /// </summary>
    public class SearchTextMessage
    {
        public string Text { get; set; } = string.Empty;
        public SearchTextMessage() { throw new NotImplementedException(); }
        public SearchTextMessage(string text)
        {
            Text = text;
        }
    }
}
