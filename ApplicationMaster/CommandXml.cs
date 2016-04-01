
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml;

namespace ProRunner
{
	class CommandXml
	{
		public static XmlTextWriter Writer { get; set; }

		private static XmlDocument doc = null;

		public static void InitializeXmlDocument()
		{
			doc = new XmlDocument();
			doc.Load(CommandUtil.COMMANDCONFIGFILE);
		}

		public static void InitXmlConfigText()  //初始化配置文本
		{
			Writer = new XmlTextWriter(CommandUtil.COMMANDCONFIGFILE, System.Text.Encoding.UTF8);
			Writer.Formatting = Formatting.Indented;
			Writer.WriteStartElement("root");
			Writer.WriteStartElement("commands");
			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.ExportFurnitur);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod FurnitureProjectMethod.ExportFurnitureBaseOnCSVFile");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.SceneReConnect);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod FurnitureProjectMethod.SceneReConnect");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.Upload);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod FurnitureProjectMethod.UploadAllToLoal");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.PhotoBaseOnCSVFile);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod FurnitureProjectMethod.CapturePhotoBaseOnCSVFile");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.PhotoForAllFurniture);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod FurnitureProjectMethod.CapturePhotoForAllFurniture");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.UpdateFurnitures);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod DesignProjectMethod.UpdateFurnitures");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.RevertFurnitureInScene);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod DesignProjectMethod.RevertFurnitureInScene -quit");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.batchmodeQuit);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -batchmode -quit");
			Writer.WriteEndElement();


			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.projectpath);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath%");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.AssetsInteDesign);
			Writer.WriteAttributeString("value", @"%svn% update %filepath%\Assets\InteDesign");
			Writer.WriteEndElement();

			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.UploadAllSmartStore);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod FurnitureProjectMethod.UploadAll_SmartStore");
			Writer.WriteEndElement();


			Writer.WriteStartElement("commands");
			Writer.WriteAttributeString("key", CommandUtil.UpdateFurnitureVersion);
			Writer.WriteAttributeString("value", @"%unity% -projectpath %filepath% -executeMethod DesignProjectMethod.UpdateFurnitureVersion");
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
			XmlNodeList xmlNodeList = doc.GetElementsByTagName("commands");
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

		public static string GetAttribute(XmlNode xn, string attr)
		{
			XmlElement xe = ExchangeNodeElement(xn);
			return xe.GetAttribute(attr);
		}
		public static XmlElement ExchangeNodeElement(XmlNode xn)
		{
			return (XmlElement)xn;   //返回元素
		}

		public static ObservableCollection<Commands> Getcommands()   //返回集合
		{
			ObservableCollection<Commands> commands = new ObservableCollection<Commands>();

			if (doc == null)
			{
				InitializeXmlDocument();
			}
			XmlNodeList xmlNodeList = doc.GetElementsByTagName("commands");
			foreach (XmlNode node in xmlNodeList)   //遍历节点
			{
				Commands command = new Commands();
				XmlElement element = ExchangeNodeElement(node);
				if (element.HasAttribute("key"))
				{
					command.Key = element.GetAttribute("key");
				}
				if (element.HasAttribute("value"))
				{
					command.Value = element.GetAttribute("value");
				}
				commands.Add(command);
			
			}
			return commands;
		}

		public static void SaveCommands(ObservableCollection<Commands> command)
		{
			doc = new XmlDocument();
			doc.Load(CommandUtil.COMMANDCONFIGFILE);

			XmlNodeList xmlNodeList = doc.GetElementsByTagName("commands");
			XmlNode root = doc.SelectSingleNode("root");
			root.FirstChild.RemoveAll();

			foreach (Commands commands in command)
			{
				XmlElement element = doc.CreateElement("commands");
				element.SetAttribute("key", commands.Key);
				element.SetAttribute("value", commands.Value);
				root.FirstChild.AppendChild(element);
			}

			doc.Save(CommandUtil.COMMANDCONFIGFILE);
		}
	}
}
