using System;
using System.Collections.Generic;
using System.Linq;
using MB.DAL;
using MB.Model;

namespace MB.BLL
{
    public class TemplateCategoryBLL : ITemplateCategoryBLL
    {
        /// <summary>
        /// 获取所有分类列表
        /// </summary>
        /// <returns></returns>
        public List<TemplateCategoryModel> GetCategoryList()
        {
            List<TemplateCategoryModel> CategoryList = new List<TemplateCategoryModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                CategoryList = (from t in db.tblTemplateCategory
                                where t.deleteFlag != true
                                orderby t.orderNum
                                select new TemplateCategoryModel
                                {
                                    categoryId = t.categoryId,
                                    categoryName = t.categoryName,
                                    system = t.system,
                                    orderNum = t.orderNum,
                                    comment = t.comment
                                }).ToList();
            }
            return CategoryList;
        }

        /// <summary>
        /// 获取分类详情
        /// </summary>
        /// <param name="caregoryId"></param>
        /// <returns></returns>
        public TemplateCategoryModel GetCareoryById(int caregoryId)
        {
            TemplateCategoryModel caregory = new TemplateCategoryModel();
            using (var db = new TargetNavigationDBEntities())
            {
                caregory = (from t in db.tblTemplateCategory
                            where t.deleteFlag != true && t.categoryId == caregoryId
                            select new TemplateCategoryModel
                            {
                                categoryId = t.categoryId,
                                categoryName = t.categoryName,
                                system = t.system,
                                comment = t.comment
                            }).FirstOrDefault();
            }
            return caregory;
        }

        /// <summary>
        /// 新增分类
        /// </summary>
        /// <param name="Category"></param>
        /// <returns></returns>
        public bool AddCareoryById(TemplateCategoryModel Category)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
                var caregoryId = db.prcGetPrimaryKey("tblTemplateCategory", obj).FirstOrDefault().Value;
                var orderNum = db.tblTemplateCategory.Count();
                var caregory = new tblTemplateCategory
                {
                    categoryId = caregoryId,
                    categoryName = Category.categoryName,
                    comment = Category.comment,
                    createTime = DateTime.Now,
                    createUser = Category.creatuser,
                    deleteFlag = false,
                    orderNum = orderNum + 1,
                    system = false,
                    updateTime = DateTime.Now,
                    updateUser = Category.creatuser
                };
                db.tblTemplateCategory.Add(caregory);
                db.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 分类修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Name"></param>
        /// <param name="commet"></param>
        /// <returns></returns>
        public bool UpdateCaregoryById(TemplateCategoryModel temCaregory)
        {
            var flag = false;
            using (var db = new TargetNavigationDBEntities())
            {
                var olddata = db.tblTemplateCategory.Where(p => p.categoryId == temCaregory.categoryId).FirstOrDefault();
                if (olddata != null)
                {
                    olddata.categoryName = temCaregory.categoryName;
                    olddata.comment = temCaregory.comment;
                    olddata.updateTime = DateTime.Now;
                    olddata.updateUser = temCaregory.upateuser;
                }
                db.SaveChanges();
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 分类删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCaregory(int id)
        {
            var flag = 1;
            using (var db = new TargetNavigationDBEntities())
            {
                var isuser = db.tblTemplate.Where(p => p.categoryId == id && p.deleteFlag == false).ToList();
                if (isuser.Count == 0)
                {
                    var olddata = db.tblTemplateCategory.Where(p => p.categoryId == id).FirstOrDefault();
                    if (olddata != null)
                    {
                        olddata.deleteFlag = true;
                    }
                    db.SaveChanges();
                    flag = 2;
                }
                else
                {
                    flag = 3;
                }
            }
            return flag;
        }

        /// <summary>
        /// 拖动排序
        /// </summary>
        /// <param name="oldnum"></param>
        public void UpdateOldNum(List<TemplateCategoryOlderModel> List)
        {
            using (var db = new TargetNavigationDBEntities())
            {
                foreach (var item in List)
                {
                    var Category = db.tblTemplateCategory.Where(p => p.categoryId == item.categoryId).FirstOrDefault();
                    if (Category != null)
                    {
                        Category.orderNum = item.orderNum;
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}