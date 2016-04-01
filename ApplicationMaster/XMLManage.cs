using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;

namespace Casamia
{
    class XMLManage
    {
        public static XmlTextWriter Writer { get; set; }

        private static XmlDocument doc = null;

        public static void InitializeXmlDocument() 
        {

			try
			{
				doc = new XmlDocument();
				doc.Load(Util.RUNNER_CONFIG_FILE);
			}
			catch (System.Exception)
			{
				File.Delete(Util.RUNNER_CONFIG_FILE);

				WriteDafaultConfigText(Util.RUNNER_CONFIG_FILE);

				doc = new XmlDocument();

				doc.Load(Util.RUNNER_CONFIG_FILE);
			}
            
        }

        public static void WriteDafaultConfigText(string configPath)
        {
			Writer = new XmlTextWriter(configPath, System.Text.Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;

            Writer.WriteStartElement("root");
            Writer.WriteStartElement("Parameters");
            Writer.WriteStartElement("Parameter");
            Writer.WriteAttributeString("key", Util.UNITY);
            Writer.WriteAttributeString("value", @"C:\Program Files (x86)\Unity\Editor\Unity.exe");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Parameter");
            Writer.WriteAttributeString("key", Util.SVN);
            Writer.WriteAttributeString("value", @"C:\Program Files\TortoiseSVN\bin\svn.exe");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Parameter");
            Writer.WriteAttributeString("key", Util.PRODUCTPATH);
            Writer.WriteAttributeString("value", @"D:\Works\InteDesign_unity_v2\UnityProduct");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Parameter");
            Writer.WriteAttributeString("key", Util.DESIGNPATH);
            Writer.WriteAttributeString("value", @"D:\Works\InteDesign_unity_v2\UnityDesign");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Parameter");
            Writer.WriteAttributeString("key", Util.EXTERNALPATH);
            Writer.WriteAttributeString("value", "\"" + @"D:\Works\InteDesign_unity_v2\InetDesignScaffolding\Batch\FurnitureProjectExternals.txt" + "\"");
            Writer.WriteEndElement();

            Writer.WriteStartElement("Parameter");
            Writer.WriteAttributeString("key", Util.IGNOREPATTREN);
            Writer.WriteAttributeString("value", "\"" + @"D:\Works\InteDesign_unity_v2\InetDesignScaffolding\Batch\SVN_ProjectIgnore.txt" + "\"");
            Writer.WriteEndElement();

			Writer.WriteStartElement("Parameter");
			Writer.WriteAttributeString("key", Util.PRODUCTURL);
			Writer.WriteAttributeString("value", "http://fileserver:81/InteDesign_unity_v2/UnityProduct");
			Writer.WriteEndElement();

			Writer.WriteStartElement("Parameter");
			Writer.WriteAttributeString("key", Util.DESIGNURL);
			Writer.WriteAttributeString("value", "http://fileserver:81/InteDesign_unity_v2/UnityDesign");
			Writer.WriteEndElement();


			Writer.WriteStartElement("Parameter");
			Writer.WriteAttributeString("key", Util.MAXIMUM_EXECUTE_TIME);
			Writer.WriteAttributeString("value", "-1");
			Writer.WriteEndElement();

			

            Writer.WriteEndElement();
            Writer.WriteEndElement();

            Writer.Flush();
            Writer.Close(); 
        }

        public static string GetString(string key) 
        {
            if (doc == null)
            {
                InitializeXmlDocument();
            }
            XmlNodeList xmlNodeList = doc.GetElementsByTagName("Parameter");
            foreach (XmlNode node in xmlNodeList)
            {
                XmlElement element = ExchangeNodeElement(node);
                if (element.HasAttribute("key"))
                {
                    string compare = element.GetAttribute("key");
                    if (compare == key && element.HasAttribute("value"))
                    {
                        return element.GetAttribute("value");
                    }
                }
            }
            return null;
        }

        public static bool TryGetBool(string key, out bool b)
        {
            string value = GetString(key);

            if (bool.TryParse(value, out b)) 
            {
                return true;
            }

            return false;
        }

        public static void SaveBool(string key,bool value) 
        {
            Parameter parmater = new Parameter();
            parmater.Key = key;
            parmater.Value = value.ToString();

            SaveParameter(parmater);
        }

        public static string GetAttribute(XmlNode xn, string attr)
        {
            XmlElement xe = ExchangeNodeElement(xn);
            return xe.GetAttribute(attr);
        }

        public static XmlElement ExchangeNodeElement(XmlNode xn)
        {
            return (XmlElement)xn;
        }


        public static ObservableCollection<Parameter> GetParameters() 
        {
            ObservableCollection<Parameter> parameters = new ObservableCollection<Parameter>();

            if (doc == null)
            {
                InitializeXmlDocument();
            }
            XmlNodeList xmlNodeList = doc.GetElementsByTagName("Parameter");
            foreach (XmlNode node in xmlNodeList)
            {
                Parameter parameter = new Parameter();
                XmlElement element = ExchangeNodeElement(node);
                if (element.HasAttribute("key"))
                {
                    parameter.Key = element.GetAttribute("key");
                }
                if (element.HasAttribute("value"))
                {
                    parameter.Value = element.GetAttribute("value");
                }
                parameters.Add(parameter);
            }
            return parameters;
        }

        public static void SaveParameters(ObservableCollection<Parameter> parameters)
        {
            doc = new XmlDocument();
            doc.Load(Util.RUNNER_CONFIG_FILE);

            XmlNodeList xmlNodeList = doc.GetElementsByTagName("Parameter");
            XmlNode root = doc.SelectSingleNode("root");
            root.FirstChild.RemoveAll();

            foreach (Parameter parameter in parameters)
            {
                XmlElement element = doc.CreateElement("Parameter");
                element.SetAttribute("key",parameter.Key);
                element.SetAttribute("value",parameter.Value);
                root.FirstChild.AppendChild(element);
            }

            doc.Save(Util.RUNNER_CONFIG_FILE);
        }

        public static void SaveParameter(Parameter parameter)
        {
            var parameters = XMLManage.GetParameters();
            foreach (var _parameter in parameters)
            {
                if (_parameter.Key.Equals(parameter.Key))
                {
                    _parameter.Value = parameter.Value;

                    XMLManage.SaveParameters(parameters);

                    return;
                }
            }
            parameters.Add(parameter);
            XMLManage.SaveParameters(parameters);

        }
    }
}
