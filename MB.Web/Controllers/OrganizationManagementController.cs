using MB.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MB.BLL;
using MB.Model;
using MB.Web.Common;
using Newtonsoft.Json;

namespace MB.Web.Controllers
{
    public class OrganizationManagementController : BaseController
    {
        //
        // GET: /OrganizationManagement/
        private IOrganizationManagementBLL OrganizationManagementBLL { get; set; }

    

        public ActionResult GetNewJob()
        {
            return View("~/Views/OrganizationManagement/_organizationjobNew.cshtml");
        }

        public ActionResult GetNewOrg()
        {
            return View("~/Views/OrganizationManagement/_organizationorgNew.cshtml");
        }

        public ActionResult OrganizationManagement()
        {
            return View();
        }

        //获取组织架构列表
        public string GetOrganizationListFiest(string name, int? parentId)
        {
            var orgList = OrganizationManagementBLL.GetOrganizationList(name, parentId);
            var jsonResult = new JsonResultModel(JsonResultType.success, orgList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取组织架构详情
        public string GetOrganizationInfo(int organizationId)
        {
            var orgModel = OrganizationManagementBLL.GetOrganizationInfo(organizationId);
            var jsonResult = new JsonResultModel(JsonResultType.success, orgModel, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //新建/更新组织架构
        public string SaveOrganization()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var orgInfo = JsonConvert.DeserializeObject<OrgModel>(dataJson);
            OrganizationManagementBLL.SaveOrganization(orgInfo, Convert.ToInt32(Session["userId"]));
            return ReturnJsonMsg(JsonResultType.success, null);
        }

        //删除组织架构
        public string DeleteOrganization()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var deleteIds = JsonConvert.DeserializeObject<int[]>(dataJson);
            var msg = OrganizationManagementBLL.DeleteOrganization(deleteIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, msg, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //组织架构排序
        public string OrderOrganization()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var orderList = JsonConvert.DeserializeObject<List<OrgModel>>(dataJson);
            OrganizationManagementBLL.OrderOrganization(orderList);
            return ReturnJsonMsg(JsonResultType.success, null);
        }

        //获取当前岗位已有人
        public string GetSharedUser(int stationid)
        {
            var userList = OrganizationManagementBLL.GetUserList(stationid);
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取当前所有人
        public string GetALLUser()
        {
            var userList = OrganizationManagementBLL.GetAllUser();
            var jsonResult = new JsonResultModel(JsonResultType.success, userList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取岗位列表
        public string GetStationList(string name, int? organizationId, int? stationId)
        {
            var stationList = OrganizationManagementBLL.GetStationList(name, organizationId, stationId);
            var jsonResult = new JsonResultModel(JsonResultType.success, stationList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取岗位详情
        public string GetStationInfo(int id)
        {
            var stationModel = OrganizationManagementBLL.GetStationInfo(id);
            var jsonResult = new JsonResultModel(JsonResultType.success, stationModel, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //删除岗位
        public string DeleteStation()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var deleteIds = JsonConvert.DeserializeObject<int[]>(dataJson);
            var msg = OrganizationManagementBLL.DeleteStation(deleteIds);
            var jsonResult = new JsonResultModel(JsonResultType.success, msg, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //新建/更新岗位
        public string SaveStation()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var stationInfo = JsonConvert.DeserializeObject<StationModel>(dataJson);
            var flag = OrganizationManagementBLL.SaveStation(stationInfo, Convert.ToInt32(Session["userId"]));
            var jsonResult = new JsonResultModel(JsonResultType.success, flag, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //添加岗位人员
        public string AddUser()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var stationInfo = JsonConvert.DeserializeObject<StationAddUserModel>(dataJson);
            if (stationInfo != null)
            {
                OrganizationManagementBLL.AddUser(stationInfo.stationid, stationInfo.addUser);
                return ReturnJsonMsg(JsonResultType.success, null);
            }
            else
            {
                return ReturnJsonMsg(JsonResultType.error, null);
            }
        }

        //添加岗位手册
        public string AddStationManual()
        {
            var dataJson = Request.Form["data"];
            if (dataJson == null) return ReturnJsonMsg(JsonResultType.error, null);
            var loopPlanList = JsonConvert.DeserializeObject<AddstationManualModel>(dataJson);
            OrganizationManagementBLL.AddStationManual(loopPlanList.deleteStation, loopPlanList.loopPlanList, Convert.ToInt32(Session["userId"]), loopPlanList.stationId);
            return ReturnJsonMsg(JsonResultType.success, null);
        }

        //获取岗位手册列表
        public string GetStationManual(int stationId)
        {
            var loopList = OrganizationManagementBLL.GetStationManual(stationId);
            var jsonResult = new JsonResultModel(JsonResultType.success, loopList, "正常", true);
            return JsonConvert.SerializeObject(jsonResult);
        }

        //获取组织架构
        public string GetOrganizationList(int? parent, int? organizationId)
        {
            var orgList = OrganizationManagementBLL.GetOrgListById(parent, organizationId);
            var jsonResult = new JsonResultModel(JsonResultType.success, orgList, "正常");
            return JsonConvert.SerializeObject(jsonResult);
        }

        //空数据返回的json串
        private string ReturnJsonMsg(JsonResultType flag, string msg)
        {
            var jsonResult = new JsonResultModel(flag, null, msg, true);
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}