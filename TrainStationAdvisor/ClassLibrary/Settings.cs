using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TrainStationAdvisor.ClassLibrary
{
    public class Settings
    {
        private static Dictionary<string,string> m_DefaultSettings;
        private static XmlDocument m_settings;
        private static string m_key = @"/configuration/appSettings/add[@key='{0}']/@value";

        private static XmlDocument settings
        {
            get
            {
                if (null == m_settings)
                {
                    m_settings = new XmlDocument();
                }
                return m_settings;
            }
        }

        private static string SettingsPath
        {
            get
            {
                string _settingsPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                _settingsPath += @"\Settings.xml";
                return _settingsPath;
            }
        }

		static Settings()
		{
            LoadDefaultSettings();


            if (!File.Exists(SettingsPath))
            {
                CreateDefaultSettings();
            }

            LoadSettingsFromFile();
		}

        private static void LoadDefaultSettings()
        {
            m_DefaultSettings = new Dictionary<string, string>();

            m_DefaultSettings.Add("AlertDistance", "3");
            m_DefaultSettings.Add("SendSMS", "false");
            m_DefaultSettings.Add("LastStationFromRoute", "true");
            m_DefaultSettings.Add("AutomaticStartLocationService", "true");
            m_DefaultSettings.Add("UseGps", "false");
            m_DefaultSettings.Add("DebugOn", "true");
            m_DefaultSettings.Add("AlwaysOn", "false");
            m_DefaultSettings.Add("Unattended", "true");
        }

        private static void LoadSettingsFromFile()
        {
            settings.Load(SettingsPath);
        }

        private static void CreateDefaultSettings()
        {
            using(XmlTextWriter tw = new XmlTextWriter(SettingsPath, UTF8Encoding.UTF8))
            {
                tw.WriteStartDocument();
                tw.WriteStartElement("configuration");
                tw.WriteStartElement("appSettings");
                foreach (KeyValuePair<string,string> _Item in m_DefaultSettings)
                {
                    tw.WriteStartElement("add");
                    tw.WriteStartAttribute("key", string.Empty);
                    tw.WriteRaw(_Item.Key.ToString());
                    tw.WriteEndAttribute();
                    tw.WriteStartAttribute("value", string.Empty);
                    tw.WriteRaw(_Item.Value.ToString());
                    tw.WriteEndAttribute();
                    tw.WriteEndElement();
                }
                tw.WriteEndElement();
                tw.WriteEndElement();
                tw.Close();
            }
        }

        private static XmlNode GetNode(string ANode)
        {
            return settings.DocumentElement.SelectSingleNode(ANode);
        }

        public static string GetProperty(string name)
        {
            string _Value = string.Empty;
            string _Key = string.Format(m_key, name);
            XmlNode _Node = GetNode(_Key);
            if (null != _Node)
                _Value = _Node.InnerText;
            else
            {
                //In case of no values found on the xml document, check then Default settings to 
                //see if exists one and return that value, if doesnt exits then return string.Empty
                m_DefaultSettings.TryGetValue(name, out _Value);
            }

            return _Value;
        }

        public static void SetProperty(string name, string value)
        {
            string _Key = string.Format(m_key, name);
            XmlNode _Node = GetNode(_Key);
            if (null != _Node)
            {
                _Node.InnerText = value;
            }
        }

        public static void Update()
        {
            settings.Save(SettingsPath);
        }

    }

}
