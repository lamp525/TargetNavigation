using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;

namespace MB.Web.Controllers
{
    public class RosterController : BaseController
    {
        private IRosterBLL RosterBLL { get; set; }

 

        //花名册编辑
        public ActionResult RosterEdit(int? id, int? RuserId)
        {
            ViewBag.RuserId = RuserId;
            if (id != null)
            {
                if (id != 0)
                {
                    ViewBag.type = id;
                }
            }
            else
            {
                ViewBag.type = 1;
            }
            return View();
        }

        //花名册首页
        public ActionResult RosterIndex()
        {
            ViewBag.userPage = int.Parse(ConfigurationManager.AppSettings["userPage"].ToString());
            return View();
        }

        /// <summary>
        /// 用户详细信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetRosterInfo(int? userId)
        {
            if (userId == null)
            {
                userId = int.Parse(Session["userId"].ToString());
            }
            var model = new RosterModel();
            model = RosterBLL.GetRosterInfo(userId.Value);
            var jsonResult = new JsonResultModel(JsonResultType.success, model, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取用户列表(用户)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetRosterList(string userName)
        {
            var rosterList = new List<RosterInfo>();
            rosterList = RosterBLL.GetRosterList(userName);
            var jsonResult = new JsonResultModel(JsonResultType.success, rosterList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 获取用户列表(组织)
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public string GetRosterOrgList(int currentPage, int orgId, string userName)
        {
            var rosterList = new List<RosterInfo>();
            rosterList = RosterBLL.GetRosterOrgList(currentPage, orgId, userName);
            var jsonResult = new JsonResultModel(JsonResultType.success, rosterList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 根据银行卡号查找银行名称
        /// </summary>
        /// <param name="bankNum"></param>
        /// <returns></returns>
        public string GetBankName(string bankNum)
        {
            var bankName = RosterBLL.GetBankName(bankNum);
            var jsonResult = new JsonResultModel(JsonResultType.success, bankName, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 根据身份证获取省份、城市、地区
        /// </summary>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        public string GetTownByidentityCard(string identityCard)
        {
            var strName = new List<data>();
            strName = RosterBLL.GetTownByidentityCard(identityCard);
            var jsonResult = new JsonResultModel(JsonResultType.success, strName, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 保存新建/更新用户信息
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="deleteStation"></param>
        /// <param name="newStation"></param>
        /// <returns></returns>
        public string SaveRosterInfo()
        {
            var conditionJson = Request.Form["data"];
            var rosterModel = JsonConvert.DeserializeObject<RosterModel>(conditionJson);

            var LoginUser = int.Parse(Session["userId"].ToString());
            var userId = 0;
            if (rosterModel != null)
            {
                userId = RosterBLL.SaveRosterInfo(rosterModel, LoginUser);
            }

            var jsonResult = new JsonResultModel(JsonResultType.success, userId, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 根据用户id更新状态
        /// 在职状态  1：转正 2：离职 3：退休 4：实习 5：试用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="workStatus"></param>
        /// <param name="validDate"></param>
        /// <returns></returns>
        public string UpdateWorkStatusById(int userId, int workStatus, DateTime validDate)
        {
            RosterBLL.UpdateWorkStatusById(userId, workStatus, validDate);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 删除用户 修改deleteflag==1
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string DeleteUser(int userId)
        {
            RosterBLL.DeleteUser(userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, null, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public string UpdatePassWord(int userId)
        {
            var str = RosterBLL.UpdatePassWord(userId);
            var jsonResult = new JsonResultModel(JsonResultType.success, str, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //导出用户信息
        public FileStreamResult ExportFile(int orgId, string userName = null)
        {
            var rosterList = new List<RosterModel>();
            rosterList = RosterBLL.ExportFile(orgId, userName);
            string path = string.Format(HttpRuntime.AppDomainAppPath, "temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            XSSFSheet sheet = null;
            XSSFWorkbook workbook = null;
            using (FileStream fs = new FileStream(HttpRuntime.AppDomainAppPath + "Template\\用户列表MOD.xlsx", FileMode.Open))
            {
                workbook = new XSSFWorkbook(fs);
            }
            // 获取excel的第一个sheet
            sheet = (XSSFSheet)workbook.GetSheetAt(0);
            int row = 2;
            if (rosterList.Count != 0)
            {
                foreach (var item in rosterList)
                {
                    if (item != null)
                    {
                        row++;
                        sheet.CopyRow(row - 1, row);
                        XSSFRow GetRow = (XSSFRow)sheet.GetRow(row);
                        GetRow.Cells[0].SetCellValue(row - 2);
                        //工号
                        GetRow.Cells[1].SetCellValue(item.userNumber);
                        //姓名
                        GetRow.Cells[2].SetCellValue(item.userName);
                        if (item.sex == true)
                        {
                            //性别
                            GetRow.Cells[3].SetCellValue("男");
                        }
                        else
                        {
                            //性别
                            GetRow.Cells[3].SetCellValue("女");
                        }
                        //民族
                        GetRow.Cells[4].SetCellValue(item.nation);
                        //政治面貌
                        GetRow.Cells[5].SetCellValue(item.political);
                        if (item.marriage == true)
                        {
                            //婚否
                            GetRow.Cells[6].SetCellValue("是");
                        }
                        else
                        {
                            GetRow.Cells[6].SetCellValue("否");
                        }
                        //手机1
                        GetRow.Cells[7].SetCellValue(item.mobile1);
                        //手机2
                        GetRow.Cells[8].SetCellValue(item.mobile2);
                        //实际住址
                        GetRow.Cells[9].SetCellValue(item.address);
                        //mo地点
                        GetRow.Cells[10].SetCellValue(item.workPlace);
                        //入职时间
                        if (item.entryTime != null)
                        {
                            GetRow.Cells[11].SetCellValue(item.entryTime.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            GetRow.Cells[11].SetCellValue(item.entryTime.ToString());
                        }
                        //试用期
                        GetRow.Cells[12].SetCellValue(item.probationaryPeriod.ToString());
                        //转正时间
                        if (item.positiveDate != null)
                        {
                            GetRow.Cells[13].SetCellValue(item.positiveDate.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            GetRow.Cells[13].SetCellValue(item.positiveDate.ToString());
                        }
                        //合同期
                        GetRow.Cells[14].SetCellValue(item.term.ToString());

                        //合同到期日
                        if (item.expiredDate != null)
                        {
                            GetRow.Cells[15].SetCellValue(item.expiredDate.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            GetRow.Cells[15].SetCellValue(item.expiredDate.ToString());
                        }
                        //备注
                        GetRow.Cells[16].SetCellValue(item.comment);
                        //生日
                        if (item.birthday != null)
                        {
                            GetRow.Cells[17].SetCellValue(item.birthday.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            GetRow.Cells[17].SetCellValue(item.birthday.ToString());
                        }
                        //户口性质
                        GetRow.Cells[18].SetCellValue(item.nature);
                        //籍贯
                        GetRow.Cells[19].SetCellValue(item.nativePlace);
                        //省份
                        GetRow.Cells[20].SetCellValue(item.province);
                        //城市
                        GetRow.Cells[21].SetCellValue(item.city);
                        //地区
                        GetRow.Cells[22].SetCellValue(item.district);
                        //毕业学校
                        GetRow.Cells[23].SetCellValue(item.school);
                        //专业
                        GetRow.Cells[24].SetCellValue(item.professional);
                        //学历
                        switch (item.education)
                        {
                            //高中
                            case 0: GetRow.Cells[25].SetCellValue("高中"); break;
                            //大专
                            case 1: GetRow.Cells[25].SetCellValue("大专"); break;
                            //本科
                            case 2: GetRow.Cells[25].SetCellValue("本科"); break;
                            //硕士
                            case 3: GetRow.Cells[25].SetCellValue("硕士"); break;
                            //博士
                            case 4: GetRow.Cells[25].SetCellValue("博士"); break;
                        }
                        //首次参加工作时间
                        if (item.firstWork != null)
                        {
                            GetRow.Cells[26].SetCellValue(item.firstWork.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            GetRow.Cells[26].SetCellValue(item.firstWork.ToString());
                        }
                        //资格证书
                        GetRow.Cells[27].SetCellValue(item.qualification);
                        //短号
                        GetRow.Cells[28].SetCellValue(item.cornet);
                        //紧急联系号码
                        GetRow.Cells[29].SetCellValue(item.emergencyNumber);
                        //银行卡号
                        GetRow.Cells[30].SetCellValue(item.bankCard);
                        //银行卡名
                        GetRow.Cells[31].SetCellValue(item.bankName);
                        //在职状态
                        switch (item.workStatus)
                        {
                            //转正
                            case 1: GetRow.Cells[32].SetCellValue("转正"); break;
                            //离职
                            case 2: GetRow.Cells[32].SetCellValue("离职"); break;
                            //退休
                            case 3: GetRow.Cells[32].SetCellValue("退休"); break;
                            //实习
                            case 4: GetRow.Cells[32].SetCellValue("实习"); break;
                            //试用
                            case 5: GetRow.Cells[32].SetCellValue("试用"); break;
                        }
                        //离职时间
                        if (item.quitTime != null)
                        {
                            GetRow.Cells[33].SetCellValue(item.quitTime.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            GetRow.Cells[33].SetCellValue(item.quitTime.ToString());
                        }
                        //现在工资
                        GetRow.Cells[34].SetCellValue(item.salary.ToString());
                    }
                }
            }
            string CopyPaht = path + "temp\\用户信息表" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            FileStream ms = new FileStream(CopyPaht, FileMode.Create, FileAccess.ReadWrite);
            workbook.Write(ms);
            return File(new FileStream(CopyPaht, FileMode.Open), "application/octet-stream", HttpUtility.UrlEncode("用户信息表" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx", System.Text.Encoding.UTF8));
        }

        /// <summary>
        /// 根据工号验证工号重复
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string UserNumBerIsHave(string num)
        {
            var str = RosterBLL.UserNumBerIsHave(num);
            var jsonResult = new JsonResultModel(JsonResultType.success, str, "正常");
            return JsonConvert.SerializeObject(jsonResult);
 
        }
    }
}