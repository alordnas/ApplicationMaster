using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace ProRunner
{
	class CommandUtil
	{
		public static AboutWindow aboutWindow { get; set; }
		public static string COMMANDCONFIGFILE = AppDomain.CurrentDomain.BaseDirectory + "Command.xml";
		public static string PRODUCTPREP = AppDomain.CurrentDomain.BaseDirectory + "ProductPrep.unitypackage";
		public static string DESIGNPREP = AppDomain.CurrentDomain.BaseDirectory + "DesignPrep.unitypackage";
		public static string SENCE_PACKAGE = AppDomain.CurrentDomain.BaseDirectory + "Sence.unitypackage.unitypackage";
		public static string COMMAND_RECORD_FOLDER = AppDomain.CurrentDomain.BaseDirectory + "Command";

			//COMMAND xml
		public static string CMMAND_XML = AppDomain.CurrentDomain.BaseDirectory + "CommandManager.xml";
		public static string ExportFurnitur = "出口家具执行方案";
		public static string SceneReConnect = "家具项目连接方法";
		public static string Upload = "家具上传";
		public static string PhotoBaseOnCSVFile = "家具图片保存";
		public static string PhotoForAllFurniture = "所有家具图片";
		public static string UpdateFurnitures = "更新所有家具设计方案";
		public static string RevertFurnitureInScene = "设计项目方案恢复";
		public static string batchmodeQuit = "退出模式";
		public static string projectpath = "项目路径";
		public static string AssetsInteDesign = "设计资源包";
		public static string UploadAllSmartStore = "上传所有智能存储";
		public static string UpdateFurnitureVersion = "更新家具设计版本";


	}
}
