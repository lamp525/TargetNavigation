using System;
using System.Collections.Generic;
using System.Linq;
using MB.Common;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class FlowEntrustBLL : IFlowEntrustBLL
    {
        /// <summary>
        /// 获取委托列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<FlowEntrustModel> GetFlowEntrustList(string condition, int userId, DateTime start, DateTime end, DateTime stutsS, DateTime stutsE)
        {
            var docList = new List<FlowEntrustModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                docList = (from c in db.tblFlowEntrust
                           join u in db.tblUser on c.entrustUser equals u.userId into group1
                           join uu in db.tblUser on c.mandataryUser equals uu.userId
                           from u in group1.DefaultIfEmpty()
                           where c.deleteFlag == false && (c.createUser == userId || c.entrustUser == userId)
                           select new FlowEntrustModel
                           {
                               entrustId = c.entrustId,
                               entrustuserName = u.userName,
                               mandataryUserName = uu.userName,
                               number = c.number,
                               startTime = c.startTime,
                               endTime = c.endTime,
                               deleteFlag = c.deleteFlag,
                               updateTime = c.updateTime,
                               createTime = c.createTime,
                               createUser = c.createUser,
                               mandataryUser = c.mandataryUser,
                               isComplate = DateTime.Now < c.endTime ? false : true
                           }).Where(condition).Where("createTime >=@0 And createTime <@1 And endTime>=@2 And endTime<@3", start, end, stutsS, stutsE).ToList();
            }
            return docList;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FlowEntrustModel GetFlowById(int id)
        {
            var Entrust = new FlowEntrustModel();
            using (var db = new TargetNavigationDBEntities())
            {
                Entrust = (from c in db.tblFlowEntrust
                           join u in db.tblUser on c.mandataryUser equals u.userId
                           join uu in db.tblUser on c.entrustUser equals uu.userId
                           where c.deleteFlag == false && c.entrustId == id
                           select new FlowEntrustModel
                           {
                               endTime = c.endTime,
                               startTime = c.startTime,
                               entrustId = c.entrustId,
                               number = c.number,
                               mandataryUserName = u.userName,
                               entrustuserName = uu.userName
                           }).FirstOrDefault();
                if (Entrust != null)
                {
                    var TemList = (from t in db.tblTemplate
                                   join c in db.tblEntrustResult on t.templateId equals c.templateId
                                   where c.entrustId == Entrust.entrustId
                                   select new TemplateModel
                                   {
                                       templateId = t.templateId,
                                       templateName = t.templateName
                                   }).ToList();
                    if (TemList != null)
                    {
                        Entrust.entrusList = new List<TemplateModel>();
                        foreach (var item in TemList)
                        {
                            Entrust.entrusList.Add(item);
                        }
                    }
                }
            }
            return Entrust;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="Entrust"></param>
        /// <returns></returns>
        public bool AddNewFlowE(FlowEntrustModel Entrust)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var EnId = db.prcGetPrimaryKey("tblFlowEntrust", obj).FirstOrDefault().Value;
                var ENModel = new tblFlowEntrust
                {
                    endTime = Entrust.endTime,
                    entrustId = EnId,
                    entrustUser = Entrust.entrustUser,
                    mandataryUser = Entrust.mandataryUser,
                    number = Entrust.number,
                    startTime = Entrust.startTime,
                    deleteFlag = false,
                    createUser = Entrust.createUser,
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now,
                    updateUser = Entrust.createUser
                };
                db.tblFlowEntrust.Add(ENModel);
                foreach (var item in Entrust.templateId)
                {
                    var AsModel = new tblEntrustResult
                    {
                        entrustId = EnId,
                        templateId = item
                    };
                    db.tblEntrustResult.Add(AsModel);
                }
                db.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 收回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateFlowE(int id, int user)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                var olddata = db.tblFlowEntrust.Where(p => p.entrustId == id).FirstOrDefault();
                if (olddata != null)
                {
                    olddata.endTime = DateTime.Now;
                    olddata.updateUser = user;
                    olddata.updateTime = DateTime.Now;
                }
                db.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 获取分类
        /// </summary>
        /// <returns></returns>
        public List<TemplateCategoryModel> GetCategoryModel()
        {
            var fileDirectoryList = new List<TemplateCategoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                fileDirectoryList = (from c in db.tblTemplateCategory
                                     where !c.deleteFlag.Value
                                     select new TemplateCategoryModel
                                     {
                                         id = c.categoryId,
                                         name = c.categoryName,
                                         isCategory = true,
                                         isParent = true
                                     }).ToList();
                if (fileDirectoryList != null)
                {
                    foreach (var item in fileDirectoryList)
                    {
                        var Count = db.tblTemplate.Where(p => p.categoryId == item.categoryId).Count();
                        if (Count == 0)
                        {
                            fileDirectoryList.Remove(item);
                        }
                    }
                }
            }
            return fileDirectoryList;
        }

        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<TemplateModel> GetTemById(int id)
        {
            var temList = new List<TemplateModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                temList = (from c in db.tblTemplate
                           where !c.deleteFlag && c.categoryId == id
                           select new TemplateModel
                           {
                               id = c.templateId,
                               name = c.templateName,
                               isCategory = false
                           }).ToList();
            }

            return temList;
        }
    }
}