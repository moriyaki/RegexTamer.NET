using System.Globalization;
using System.Resources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RegexTamer.NET
{
    #region リソースサービス

    public class ResourceService : ObservableObject
    {
        private readonly static ResourceManager resourceManager = new("RegexTamer.NET.Resources.Resource", typeof(ResourceService).Assembly);

        /// <summary>
        /// Change Language Culture
        /// </summary>
        /// <param name="cultureName">ex: "ja"</param>
        public static void ChangeCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentUICulture = culture;
        }

        /// <summary>
        /// Get Culture String
        /// </summary>
        /// <param name="key">Culture Key</param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            var str = resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? string.Empty;
            return str;
        }
    }

    #endregion リソースサービス
}
