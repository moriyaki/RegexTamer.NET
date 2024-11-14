using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace RegexTamer.NET
{
    public interface ISettings
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
        /// Window width
        /// </summary>
        double WindowWidth { get; set; }
        /// <summary>
        /// Windoe Height
        /// </summary>
        double WindowHeight { get; set; }
        /// <summary>
        /// Culture Name for Language
        /// </summary>
        string CultureName { get; set; }
        /// <summary>
        /// LOad from Settings XML
        /// </summary>
        void LoadSettings();
        /// <summary>
        /// Save to Settings XML
        /// </summary>
        void SaveSettings();
    }

    public class Settings : ISettings
    {
        private readonly string appName = "RegexTamer.NET";
        private readonly string settingXMLFile = "settings.xml";
        private readonly string settingsFilePath;
        public Settings()
        {
            var localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
            settingsFilePath = Path.Combine(localAppDataPath, settingXMLFile);

            LoadSettings();
        }

        #region Property
        /// <summary>
        /// Window Top position
        /// </summary>
        private double _WindowTop = 100d;

        public double WindowTop
        {
            get => _WindowTop;
            set
            {
                if (_WindowTop == value) { return; }
                _WindowTop = value;
                SaveSettings();
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
                if (_WindowLeft == value) { return; }
                _WindowLeft = value;
                SaveSettings();
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
                if (_WindowWidth == value) { return; }
                _WindowWidth = value;
                SaveSettings();
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
                if (_WindowHeight == value) { return; }
                _WindowHeight = value;
                SaveSettings();
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
                if (_CultureName == value) { return; }
                _CultureName = value;
                SaveSettings();
            }
        }
        #endregion Property

        #region settings.xml load and save

        /// <summary>
        /// Load settings from settings.xml
        /// </summary>
        public void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                try
                {
                    // LOad settings.xml if exists
                    XDocument doc = XDocument.Load(settingsFilePath);
                    XElement? root = doc.Element("Settings");

                    if (root != null)
                    {
                        WindowTop = Convert.ToDouble(root.Element("WindowTop")?.Value);
                        WindowLeft = Convert.ToDouble(root.Element("WindowLeft")?.Value);
                        WindowWidth = Convert.ToDouble(root.Element("WindowWidth")?.Value);
                        WindowHeight = Convert.ToDouble(root.Element("WindowHeight")?.Value);
                        CultureName = root.Element("CultureName")?.Value ?? "en";
                    }
                }
                // if setings.xml not exist, use default
                catch (XmlException) { }
            }
            else
            {
                SaveSettings();
            }
        }

        /// <summary>
        /// Save settings to settings.xml
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                // save to settings.xml
                XDocument doc = new(
                new XElement("Settings",
                        new XElement("WindowTop", WindowTop),
                        new XElement("WindowLeft", WindowLeft),
                        new XElement("WindowWidth", WindowWidth),
                        new XElement("WindowHeight", WindowHeight),
                        new XElement("CultureName", CultureName)
                    )
                );

                // create direcory if does not exist directory
                string localAppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), appName);
                if (!Directory.Exists(localAppDataPath))
                {
                    Directory.CreateDirectory(localAppDataPath);
                }

                // Failed to save to save settings.xml
                doc.Save(settingsFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error : {ex.Message}");
            }
        }

        #endregion settings.xml load and save
    }
}
