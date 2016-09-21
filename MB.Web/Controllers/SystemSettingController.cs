using MB.Web.Models;
using System;
using System.Configuration;
using System.Web.Mvc;
using System.Xml;
using MB.Model;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class SystemSettingController : Controller
    {
        //
        // GET: /SystemSetting/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SystemSetting()
        {
            return View();
        }

        //系统设置
        public string SystemSet()
        {
            var data = Request.Form["data"];
            var systemData = JsonConvert.DeserializeObject<SystemUploadPath>(data);
            if (systemData != null)
            {
                XmlDocument doc = new XmlDocument();
                string appdomainconfigfile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                doc.Load(appdomainconfigfile);
                if (systemData.pageNum != null && systemData.pageNum != "0")
                {
                    XmlNode node1 = doc.SelectSingleNode(@"//add[@key='pageNum']");
                    XmlElement ele1 = (XmlElement)node1;
                    ele1.SetAttribute("value", systemData.pageNum.ToString());
                }
                if (systemData.userPage != null && systemData.userPage != "0")
                {
                    XmlNode node2 = doc.SelectSingleNode(@"//add[@key='userPage']");
                    XmlElement ele2 = (XmlElement)node2;
                    ele2.SetAttribute("value", systemData.userPage.ToString());
                }
                if (systemData.completeQuantity != null && systemData.completeQuantity != "0")
                {
                    XmlNode node3 = doc.SelectSingleNode(@"//add[@key='completeQuantity']");
                    XmlElement ele3 = (XmlElement)node3;
                    ele3.SetAttribute("value", systemData.completeQuantity.ToString());
                }
                if (systemData.completeQuality != null && systemData.completeQuality != "0")
                {
                    XmlNode node4 = doc.SelectSingleNode(@"//add[@key='completeQuality']");
                    XmlElement ele4 = (XmlElement)node4;
                    ele4.SetAttribute("value", systemData.completeQuality.ToString());
                }
                if (systemData.completeTime != null && systemData.completeTime != "0")
                {
                    XmlNode node5 = doc.SelectSingleNode(@"//add[@key='completeTime']");
                    XmlElement ele5 = (XmlElement)node5;
                    ele5.SetAttribute("value", systemData.completeTime.ToString());
                }
                if (systemData.MessagePath != null && systemData.MessagePath != "")
                {
                    XmlNode node6 = doc.SelectSingleNode(@"//add[@key='MessagePath']");
                    XmlElement ele6 = (XmlElement)node6;
                    ele6.SetAttribute("value", systemData.MessagePath);
                }
                if (systemData.InputErrorValidate != null && systemData.InputErrorValidate != "0")
                {
                    XmlNode node7 = doc.SelectSingleNode(@"//add[@key='InputErrorValidate']");
                    XmlElement ele7 = (XmlElement)node7;
                    ele7.SetAttribute("value", systemData.InputErrorValidate.ToString());
                }
                if (systemData.NewsUpLoadPath != null && systemData.NewsUpLoadPath != "")
                {
                    XmlNode node8 = doc.SelectSingleNode(@"//add[@key='NewsUpLoadPath']");
                    XmlElement ele8 = (XmlElement)node8;
                    ele8.SetAttribute("value", systemData.NewsUpLoadPath.ToString());
                }
                if (systemData.DocumentUpLoadPath != null && systemData.DocumentUpLoadPath != "")
                {
                    XmlNode node9 = doc.SelectSingleNode(@"//add[@key='DocumentUpLoadPath']");
                    XmlElement ele9 = (XmlElement)node9;
                    ele9.SetAttribute("value", systemData.DocumentUpLoadPath.ToString());
                }
                if (systemData.PlanTemplate != null && systemData.PlanTemplate != "")
                {
                    XmlNode node10 = doc.SelectSingleNode(@"//add[@key='PlanTemplate']");
                    XmlElement ele10 = (XmlElement)node10;
                    ele10.SetAttribute("value", systemData.PlanTemplate.ToString());
                }
                if (systemData.MineUpLoadPath != null && systemData.MineUpLoadPath != "")
                {
                    XmlNode node11 = doc.SelectSingleNode(@"//add[@key='MineUpLoadPath']");
                    XmlElement ele11 = (XmlElement)node11;
                    ele11.SetAttribute("value", systemData.MineUpLoadPath.ToString());
                }
                if (systemData.HeadImageUpLoadPath != null && systemData.HeadImageUpLoadPath != "")
                {
                    XmlNode node12 = doc.SelectSingleNode(@"//add[@key='HeadImageUpLoadPath']");
                    XmlElement ele12 = (XmlElement)node12;
                    ele12.SetAttribute("value", systemData.HeadImageUpLoadPath.ToString());
                }
                if (systemData.IMUploadPath != null && systemData.IMUploadPath != "")
                {
                    XmlNode node13 = doc.SelectSingleNode(@"//add[@key='IMUploadPath']");
                    XmlElement ele13 = (XmlElement)node13;
                    ele13.SetAttribute("value", systemData.IMUploadPath.ToString());
                }
                if (systemData.ObjectiveUploadPath != null && systemData.ObjectiveUploadPath != "")
                {
                    XmlNode node14 = doc.SelectSingleNode(@"//add[@key='ObjectiveUploadPath']");
                    XmlElement ele14 = (XmlElement)node14;
                    ele14.SetAttribute("value", systemData.ObjectiveUploadPath.ToString());
                }
                if (systemData.FlowIndexUploadPath != null && systemData.FlowIndexUploadPath != "")
                {
                    XmlNode node15 = doc.SelectSingleNode(@"//add[@key='FlowIndexUploadPath']");
                    XmlElement ele15 = (XmlElement)node15;
                    ele15.SetAttribute("value", systemData.FlowIndexUploadPath.ToString());
                }
                if (systemData.MeetingUpLoadPath != null && systemData.MeetingUpLoadPath != "")
                {
                    XmlNode node16 = doc.SelectSingleNode(@"//add[@key='MeetingUpLoadPath']");
                    XmlElement ele16 = (XmlElement)node16;
                    ele16.SetAttribute("value", systemData.MeetingUpLoadPath.ToString());
                }
                if (systemData.ConvertFilePath != null && systemData.ConvertFilePath != "")
                {
                    XmlNode node17 = doc.SelectSingleNode(@"//add[@key='ConvertFilePath']");
                    XmlElement ele17 = (XmlElement)node17;
                    ele17.SetAttribute("value", systemData.ConvertFilePath.ToString());
                }
                if (systemData.IMHost != null && systemData.IMHost != "")
                {
                    XmlNode node18 = doc.SelectSingleNode(@"//add[@key='IMHost']");
                    XmlElement ele18 = (XmlElement)node18;
                    ele18.SetAttribute("value", systemData.IMHost.ToString());
                }
                if (systemData.PlanTemplate != null && systemData.PlanTemplate != "")
                {
                    XmlNode node19 = doc.SelectSingleNode(@"//add[@key='PlanTemplate']");
                    XmlElement ele19 = (XmlElement)node19;
                    ele19.SetAttribute("value", systemData.PlanTemplate.ToString());
                }
                if (systemData.maxQuantity != null && systemData.maxQuantity != "0")
                {
                    XmlNode node20 = doc.SelectSingleNode(@"//add[@key='maxQuantity']");
                    XmlElement ele20 = (XmlElement)node20;
                    ele20.SetAttribute("value", systemData.maxQuantity.ToString());
                }
                if (systemData.maxQuality != null && systemData.maxQuality != "0")
                {
                    XmlNode node21 = doc.SelectSingleNode(@"//add[@key='maxQuality']");
                    XmlElement ele21 = (XmlElement)node21;
                    ele21.SetAttribute("value", systemData.maxQuality.ToString());
                }
                if (systemData.maxTime != null && systemData.maxTime != "0")
                {
                    XmlNode node22 = doc.SelectSingleNode(@"//add[@key='maxTime']");
                    XmlElement ele22 = (XmlElement)node22;
                    ele22.SetAttribute("value", systemData.maxTime.ToString());
                }
                doc.Save(appdomainconfigfile);
                ConfigurationManager.RefreshSection("appSettings");
            }
            var result = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(result);
        }
    }
}