using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Model;
using MB.DAL;
using MB.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MB.BLL.Test
{
    //流程首页单测
    [TestClass]
    public class FlowIndexBLLTest
    {
        private FlowIndexBLL flowIndexBll = new FlowIndexBLL();

        #region 获取模板分类列表（GetTemplateCategoryList）
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetTemplateCategoryList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateCategoryList_Test001");

            var list = flowIndexBll.GetTemplateCategoryList();
            Assert.AreEqual(list.Count,0);
        }
        /// <summary>
        /// db中有相关数据
        /// </summary>
        [TestMethod]
        public void GetTemplateCategoryList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateCategoryList_Test002");

            var list = flowIndexBll.GetTemplateCategoryList();
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].categoryId,1);
            Assert.AreEqual(list[0].categoryName, "病假");
        }
        #endregion

        #region 获取模板列表(GetTemplateList)
         /// <summary>
        /// db中没有相关数据
        /// </summary>
        //[TestMethod]
        //public void GetTemplateList_Test001()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx");

        //    var list = flowIndexBll.GetTemplateList();
        //    Assert.AreEqual(list.Count,0);
        //}

        /// <summary>
        /// db中有相关数据
        /// </summary>
        [TestMethod]
        public void GetTemplateList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateList_Test002");

            //var list = flowIndexBll.GetTemplateList(2);
            //Assert.AreEqual(list.Count, 2);
            //Assert.AreEqual(list[0].templateId,1);
            //Assert.AreEqual(list[0].templateName, "病假");
            //Assert.AreEqual(list[0].description, "请病假");
        }
        #endregion

        #region 获取模板html(GetTemplateHtml)
        /// <summary>
        /// db中没有数据
        /// </summary>
        [TestMethod]
        public void GetTemplateHtml_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateHtml_Test001");

            var model = flowIndexBll.GetTemplateHtml(2);
            Assert.AreEqual(model,null);
        }

        /// <summary>
        /// db中有数据
        /// </summary>
        [TestMethod]
        public void GetTemplateHtml_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateHtml_Test002");

            var model = flowIndexBll.GetTemplateHtml(3);
            Assert.AreNotEqual(model, null);
            Assert.AreEqual(model.defaultTitle,false);
            Assert.AreEqual(model.contents, "mubiaodaohang");
        }
        #endregion

        #region 新建流程(AddFlow)
        ///// <summary>
        ///// db中没有数据
        ///// </summary>
        //[TestMethod]
        //public void AddFlow_Test001()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "AddFlow_Test001");
        //    var flowModel = new AddFormContentModel { 
        //        templateId=1
        //    };

        //    flowIndexBll.AddFlow(flowModel,1);
        //    using (var db=new TargetNavigationDBEntities())
        //    {
        //        var list = db.tblUserForm;
        //        Assert.AreEqual(list.Count(),0);
        //    }
        //}

        /// <summary>
        /// db中有数据
        /// </summary>
        //[TestMethod]
        //public void AddFlow_Test002()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "AddFlow_Test002");
        //    var flowModel = new AddFormContentModel
        //    {
        //        templateId = 1,
        //        organizationId = 2,
        //        title = "请病假",
        //        urgency = 1,
        //        createUser = 247,
        //        controlValue = new List<ControlModel> { 
        //          new ControlModel{
        //            controlId="xxc_input",
        //            rowNumberList=new List<ControlDetailModel>
        //            {
        //                new ControlDetailModel
        //                {
        //                    rowNumber=1,
        //                    detailValue=new string[]{
        //                        "中心小学"
        //                    }
                            
        //                }
                        
        //            }
        //          },
        //          new ControlModel{
        //            controlId="xxc_span",
        //            rowNumberList=new List<ControlDetailModel>
        //            {
        //                new ControlDetailModel
        //                {
        //                    rowNumber=1,
        //                    detailValue=new string[]
        //                    {
        //                        "我要请病假"
        //                    }
        //                }
                        
        //            }
        //          }

        //        }

        //    };

        //    flowIndexBll.AddFlow(flowModel);
        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        //验证表单
        //        var list = db.tblUserForm.ToList();
        //        Assert.AreEqual(list.Count(), 1);
        //        Assert.AreEqual(list[0].templateId,1);
        //        Assert.AreEqual(list[0].organizationId, 2);
        //        Assert.AreEqual(list[0].title ,"请病假");
        //        Assert.AreEqual(list[0].urgency , 1);
        //        Assert.AreEqual(list[0].createUser , 247);
        //        Assert.AreEqual(list[0].updateUser, 247);

        //        //验证表单详细表
        //        var formId=list[0].formId;
        //        var detailList = db.tblFormDetail.Where(p => p.formId == formId).ToList();
        //        Assert.AreEqual(detailList.Count,2);
        //        Assert.AreEqual(detailList[0].controlId ,"xxc_input");
        //        Assert.AreEqual(detailList[0].rowNumber,1);

        //        //验证表单内容表
        //        var contentList = db.tblFormContent.ToList();
        //        Assert.AreEqual(contentList.Count, 2);
        //        Assert.AreEqual(contentList[0].controlValue, "中心小学");

        //        //验证表单抄送表
        //        var duplicateList = db.tblFormDuplicate.ToList() ;
        //        Assert.AreEqual(duplicateList.Count,1);
        //        Assert.AreEqual(duplicateList[0].formId,list[0].formId);
        //        Assert.AreEqual(duplicateList[0].userId,247);
        //        Assert.AreEqual(duplicateList[0].nodeId, 1);
        //        Assert.AreEqual(duplicateList[0].alreadyRead, 1);
        //    }
        //}
        #endregion

        #region 获取待提交或已提交的流程列表(GetUnSubmitFlowList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetUnSubmitFlowList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetUnSubmitFlowList_Test001");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            //var list = flowIndexBll.GetUnSubmitFlowList(247, conditionString, start, end, 1, 1);
            //Assert.AreEqual(list.Count,0);
        }

        /// <summary>
        /// 测试待提交列表
        /// </summary>
        [TestMethod]
        public void GetUnSubmitFlowList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetUnSubmitFlowList_Test002");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            //var list = flowIndexBll.GetUnSubmitFlowList(247, conditionString, start, end, 1, 1);
            //Assert.AreEqual(list.Count, 2);
            //Assert.AreEqual(list[0].formId,1);
            //Assert.AreEqual(list[0].templateId, 4);
            //Assert.AreEqual(list[0].organizationId, 1);
            //Assert.AreEqual(list[0].title, "待提交");
            //Assert.AreEqual(list[0].urgency, 2);
            //Assert.AreEqual(list[0].status, 1);
            //Assert.AreEqual(list[0].currentNode, 1);
            //Assert.AreEqual(list[0].archive, false);
            //Assert.AreEqual(list[0].createUser, 247);
            //Assert.AreEqual(list[0].createUserName, "徐晓催");
            //Assert.AreEqual(list[0].img, string.Empty);

            ////验证操作人集合
            //Assert.AreEqual(list[0].operate.Count,2);
            //Assert.AreEqual(list[0].operate[0].id, 110);
            //Assert.AreEqual(list[0].operate[0].name, "梁良");
        }
        /// <summary>
        /// 测试已提交提交列表
        /// </summary>
        [TestMethod]
        public void GetUnSubmitFlowList_Test003()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetUnSubmitFlowList_Test003");

            //var conditionString = " createUser==247 ";
            var start = DateTime.Parse("2015-7-12");
            var end = DateTime.Now;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                },
                new Sort{
                    type=0,
                    direct=0
                }
            };
            //var list = flowIndexBll.GetUnSubmitFlowList(247, conditionString, start, end, 1, 2);
            //Assert.AreEqual(list.Count, 2);
            //Assert.AreEqual(list[0].formId, 4);
            //Assert.AreEqual(list[0].templateId, 7);
            //Assert.AreEqual(list[0].organizationId, 1);
            //Assert.AreEqual(list[0].title, "已提交1");
            //Assert.AreEqual(list[0].urgency, 3);
            //Assert.AreEqual(list[0].status, 2);
            //Assert.AreEqual(list[0].currentNode, 3);
            //Assert.AreEqual(list[0].archive, false);
            //Assert.AreEqual(list[0].createUser, 247);
            //Assert.AreEqual(list[0].createUserName, "徐晓催");
            //Assert.AreEqual(list[0].img, string.Empty);

            ////验证操作人集合
            //Assert.AreEqual(list[0].operate.Count, 3);
            //Assert.AreEqual(list[0].operate[0].id, 110);
            //Assert.AreEqual(list[0].operate[0].name, "梁良");
        }
        #endregion

        #region 获取待处理状态列表(含待审核和待查阅)(GetUnCheckFlowList)
        /// <summary>
        /// DB中没有相数据
        /// </summary>
        [TestMethod]
        public void GetUnCheckFlowList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetUnCheckFlowList_Test001");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            var list = flowIndexBll.GetUnCheckFlowList(247, conditionString, start, end, 1);
            Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// 查询待处理的列表
        /// </summary>
        [TestMethod]
        public void GetUnCheckFlowList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetUnCheckFlowList_Test002");

            var conditionString = " createUser==110 ";
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1, 
                    direct=1
                }
            };
            var list = flowIndexBll.GetUnCheckFlowList(247, conditionString, start, end, 1);
            Assert.AreEqual(list.Count, 3);
            Assert.AreEqual(list[0].formId, 7);
            Assert.AreEqual(list[0].templateId, 8);
            Assert.AreEqual(list[0].organizationId, 1);
            Assert.AreEqual(list[0].title, "待处理");
            Assert.AreEqual(list[0].urgency, 3);
            Assert.AreEqual(list[0].status, 2);
            Assert.AreEqual(list[0].currentNode, 3);
            Assert.AreEqual(list[0].archive, false);
            Assert.AreEqual(list[0].createUser, 110);
            Assert.AreEqual(list[0].createUserName, "梁良");
            Assert.AreEqual(list[0].img, string.Empty);

            //验证操作人集合
            Assert.AreEqual(list[0].operate.Count, 1);
            Assert.AreEqual(list[0].operate[0].id, 247);
            Assert.AreEqual(list[0].operate[0].name, "徐晓催");
        }
        #endregion

        #region 获取已处理状态列表(GetCheckedFlowList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetCheckedFlowList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetCheckedFlowList_Test001");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            var list = flowIndexBll.GetCheckedFlowList(247, conditionString, start, end, 1);
            Assert.AreEqual(list.Count, 0);
        }
        /// <summary>
        /// 查询已处理的流程数据,包括委托给登录用户的流程
        /// </summary>
        [TestMethod]
        public void GetCheckedFlowList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetCheckedFlowList_Test002");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            var list = flowIndexBll.GetCheckedFlowList(247, conditionString, start, end, 1);
            Assert.AreEqual(list.Count, 2);
            Assert.AreEqual(list[0].formId, 10);
            Assert.AreEqual(list[0].templateId, 11);
            Assert.AreEqual(list[0].organizationId, 1);
            Assert.AreEqual(list[0].title, "已处理");
            Assert.AreEqual(list[0].urgency, 3);
            Assert.AreEqual(list[0].status, 2);
            Assert.AreEqual(list[0].currentNode, 3);
            Assert.AreEqual(list[0].archive, false);
            Assert.AreEqual(list[0].createUser, 110);
            Assert.AreEqual(list[0].createUserName, "梁良");
            Assert.AreEqual(list[0].img, string.Empty);

            //验证操作人集合
            Assert.AreEqual(list[0].operate.Count, 1);
            Assert.AreEqual(list[0].operate[0].id, 247);
            Assert.AreEqual(list[0].operate[0].name, "徐晓催");
        }
        #endregion

        #region 获取已办结状态列表(GetCompletedFlowList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetCompletedFlowList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetCompletedFlowList_Test001");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            //var list = flowIndexBll.GetCompletedFlowList(247, conditionString, start, end, 1);
            //Assert.AreEqual(list.Count, 0);
        }

        /// <summary>
        /// 查询已办结的流程数据,包括委托给登录用户的流程
        /// </summary>
        [TestMethod]
        public void GetCompletedFlowList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetCompletedFlowList_Test002");

            var conditionString = string.Empty;
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;
            var sort = new List<Sort> { 
                new Sort{
                    type=1,
                    direct=1
                }
            };
            //var list = flowIndexBll.GetCompletedFlowList(247, conditionString, start, end, 1);
            //Assert.AreEqual(list.Count, 4);
            //Assert.AreEqual(list[0].formId, 13);
            //Assert.AreEqual(list[0].templateId, 14);
            //Assert.AreEqual(list[0].organizationId, 1);
            //Assert.AreEqual(list[0].title, "已办结");
            //Assert.AreEqual(list[0].urgency, 3);
            //Assert.AreEqual(list[0].status, 50);
            //Assert.AreEqual(list[0].currentNode, 3);
            //Assert.AreEqual(list[0].archive, false);
            //Assert.AreEqual(list[0].createUser, 247);
            //Assert.AreEqual(list[0].createUserName, "徐晓催");
            //Assert.AreEqual(list[0].img, string.Empty);

            ////验证操作人集合
            //Assert.AreEqual(list[0].operate, null);
            //Assert.AreEqual(list[0].operate[0].id, 247);
            //Assert.AreEqual(list[0].operate[0].name, "徐晓催");
        }
        #endregion

        #region 人员模糊查询(SelectUserList)
        /// <summary>
        /// DB中没有数据
        /// </summary>
        [TestMethod]
        public void SelectUserList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "SelectUserList_Test001");

            var list = flowIndexBll.SelectUserList("xxc", true);
            Assert.AreEqual(list.Count,0);
        }

        /// <summary>
        /// DB中存在数据
        /// </summary>
        [TestMethod]
        public void SelectUserList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "SelectUserList_Test002");

            var list = flowIndexBll.SelectUserList("徐", true);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].id,247);
            Assert.AreEqual(list[0].name, "徐晓催");
            Assert.AreEqual(list[0].img, string.Empty);
        }
        #endregion

        #region 获取流程详情(GetFlowDetailListById)
        /// <summary>
        /// DB中没有数据
        /// </summary>
        [TestMethod]
        public void GetFlowDetailListById_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetFlowDetailListById_Test001");

            var model = flowIndexBll.GetFlowDetailListById(1,1);
            Assert.AreEqual(model,null);
        }

        /// <summary>
        /// DB中有数据
        /// </summary>
        [TestMethod]
        public void GetFlowDetailListById_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetFlowDetailListById_Test002");

            var model = flowIndexBll.GetFlowDetailListById(17,1);
            Assert.AreEqual(model.templateId, 18);
            Assert.AreEqual(model.templateName, "获取详情");
            Assert.AreEqual(model.organizationId, 1);
            Assert.AreEqual(model.title, "获取详情");
            Assert.AreEqual(model.urgency, 3);
            Assert.AreEqual(model.status, 50);
            Assert.AreEqual(model.createUser, 247);
            Assert.AreEqual(model.createUserName, "徐晓催");

            //验证控件的值
            Assert.AreEqual(model.controlValue.Count,3);
            Assert.AreEqual(model.controlValue[0].controlId, "xxc_span");
            Assert.AreEqual(model.controlValue[0].rowNumber, 1);
            Assert.AreEqual(model.controlValue[0].detailValue.Length,1 );
            Assert.AreEqual(model.controlValue[0].detailValue[0], "金额");
        }
        #endregion

        #region 退回操作(TurnBack)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        //[TestMethod]
        //public void TurnBack_Test001()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "TurnBack_Test001");

        //    flowIndexBll.TurnBack(1, 1, 1, 247);

        //    using (var db=new TargetNavigationDBEntities())
        //    {
        //        var list = db.tblUserForm;
        //        Assert.AreEqual(list.Count(),0);
        //    }
        //}

        /// <summary>
        /// db中有相关数据，并进行退回操作
        /// </summary>
        //[TestMethod]
        //public void TurnBack_Test002()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "TurnBack_Test002");

        //    flowIndexBll.TurnBack(3,19,18,247);

        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        //验证用户表单表
        //        var model = db.tblUserForm.FirstOrDefault();
        //        Assert.AreNotEqual(model, null);
        //        Assert.AreEqual(model.currentNode, 1);
        //        Assert.AreEqual(model.status, 1);
        //        //验证表单抄送表

        //        var list = db.tblFormDuplicate.Where(p=>p.formId==18&&p.nodeId==1).ToList();
        //        Assert.AreEqual(list.Count,1);
        //        Assert.AreEqual(list[0].formId,18);
        //        Assert.AreEqual(list[0].userId, 247);
        //        Assert.AreEqual(list[0].nodeId, 1);
        //        Assert.AreEqual(list[0].alreadyRead, 1);
        //        //验证表单流程表
        //        var flowList = db.tblFormFlow.Where(p => p.formId == 18 && p.nodeId == 3).ToList();
        //        Assert.AreEqual(flowList.Count,1);
        //        Assert.AreEqual(flowList[0].formId, 18);
        //        Assert.AreEqual(flowList[0].nodeId, 3);
        //        Assert.AreEqual(flowList[0].result, 3);
        //        Assert.AreEqual(flowList[0].contents, "");
        //        Assert.AreEqual(flowList[0].entrustUser, null);
        //        Assert.AreEqual(flowList[0].createUser, 247);
        //        Assert.AreEqual(flowList[0].updateUser, 247);
        //    }
        //}
        #endregion

        #region 撤回操作(BackFirstNode)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void BackFirstNode_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "BackFirstNode_Test001");

            flowIndexBll.BackFirstNode(1, 1, 1, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                var list = db.tblUserForm;
                Assert.AreEqual(list.Count(), 0);
            }
        }

        /// <summary>
        /// db中有相关数据，并进行撤回操作
        /// </summary>
        [TestMethod]
        public void BackFirstNode_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "BackFirstNode_Test002");

            flowIndexBll.BackFirstNode(3, 20, 19, 247);

            using (var db = new TargetNavigationDBEntities())
            {
                //验证用户表单表
                var model = db.tblUserForm.FirstOrDefault();
                Assert.AreNotEqual(model, null);
                Assert.AreEqual(model.currentNode, 1);
                Assert.AreEqual(model.status, 1);
                //验证表单抄送表
                var list = db.tblFormDuplicate.Where(p => p.formId == 19 && p.nodeId == 1).ToList();
                Assert.AreEqual(list.Count, 1);
                Assert.AreEqual(list[0].formId, 19);
                Assert.AreEqual(list[0].userId, 247);
                Assert.AreEqual(list[0].nodeId, 1);
                Assert.AreEqual(list[0].alreadyRead, 1);
                //验证表单流程表
                var flowList = db.tblFormFlow.Where(p => p.formId == 19 && p.nodeId == 3).ToList();
                Assert.AreEqual(flowList.Count, 1);
                Assert.AreEqual(flowList[0].formId, 19);
                Assert.AreEqual(flowList[0].nodeId, 3);
                Assert.AreEqual(flowList[0].result, 4);
                Assert.AreEqual(flowList[0].contents, "");
                Assert.AreEqual(flowList[0].entrustUser, null);
                Assert.AreEqual(flowList[0].createUser, 247);
                Assert.AreEqual(flowList[0].updateUser, 247);
            }
        }
        #endregion

        #region 表单抄送(DuplicateForm)
        /// <summary>
        /// 表单抄送
        /// </summary>
        [TestMethod]
        public void DuplicateForm_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "DuplicateForm_Test001");
            var duplicateId = new int[] {247,110,249 };
            flowIndexBll.DuplicateForm(duplicateId, 1, 1, 1, 247);

            using (var db=new TargetNavigationDBEntities())
            {
                var dupList = db.tblFormDuplicate.ToList();

                Assert.AreEqual(dupList.Count,3);
                Assert.AreEqual(dupList[0].formId, 1);
                Assert.AreEqual(dupList[0].userId, 110);
                Assert.AreEqual(dupList[0].nodeId, 1);
                Assert.AreEqual(dupList[0].alreadyRead, 0);
            }
        }
        #endregion

        #region 被抄送人填写意见(AddContents)
        /// <summary>
        /// 被抄送人填写意见
        /// </summary>
        [TestMethod]
        public void AddContents_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "AddContents_Test001");
            var duplicateId = new int[] { 247, 110, 249 };
            flowIndexBll.AddContents(20,1,"同意",247);

            using (var db = new TargetNavigationDBEntities())
            {
                //验证表单流程表
                var formFlowList = db.tblFormFlow.ToList();
                Assert.AreEqual(formFlowList.Count, 1);
                Assert.AreEqual(formFlowList[0].formId, 20);
                Assert.AreEqual(formFlowList[0].nodeId, 1);
                Assert.AreEqual(formFlowList[0].result, 6);
                Assert.AreEqual(formFlowList[0].nodeId, 1);
                Assert.AreEqual(formFlowList[0].contents, "同意");
                Assert.AreEqual(formFlowList[0].createUser, 247);

            }
        }
        #endregion

        #region 同意操作(AgreeFormFlow)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        //[TestMethod]
        //public void AgreeFormFlow_Test001()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "AgreeFormFlow_Test001");

        //    flowIndexBll.AgreeFormFlow(1, 1, 1,247);

        //    using (var db=new TargetNavigationDBEntities())
        //    {
        //        var list = db.tblUserForm;
        //        Assert.AreEqual(list.Count(),0);
        //    }
        //}
        ///// <summary>
        ///// db中有相关数据
        ///// </summary>
        //[TestMethod]
        //public void AgreeFormFlow_Test002()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "AgreeFormFlow_Test002");

        //    flowIndexBll.AgreeFormFlow(21, 20, 3,"", 247);

        //    using (var db = new TargetNavigationDBEntities())
        //    {
        //        //验证用户表单表
        //        var list = db.tblUserForm.ToList();
        //        Assert.AreEqual(list.Count(), 1);
        //        Assert.AreEqual(list[0].formId,20);
        //        Assert.AreEqual(list[0].currentNode,2);
        //        Assert.AreEqual(list[0].status, 2);

        //        //验证表单抄送表
        //        var dupList = db.tblFormDuplicate.ToList();
        //        Assert.AreEqual(dupList.Count(),1);
        //        Assert.AreEqual(dupList[0].nodeId,2);
        //        Assert.AreEqual(dupList[0].alreadyRead,2);

        //        //验证表单流程表
        //        var flowList = db.tblFormFlow.ToList();
        //        Assert.AreEqual(flowList.Count,1);
        //        Assert.AreEqual(flowList[0].nodeId, 3);
        //        Assert.AreEqual(flowList[0].formId, 20);
        //        Assert.AreEqual(flowList[0].result, 2);
        //        Assert.AreEqual(flowList[0].contents, string.Empty);
        //        Assert.AreEqual(flowList[0].entrustUser, null);
        //        Assert.AreEqual(flowList[0].createUser, 247);
        //    }
        //}
        #endregion

        #region 获取饼图统计数量(GetFlowProcessList)
        /// <summary>
        /// db中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetFlowProcessList_Test001()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetFlowProcessList_Test001");

            //var list = flowIndexBll.GetFlowProcessList(2015, 7, 247);
            //Assert.AreEqual(list.Count,4);
            //Assert.AreEqual(list[0].count, 0);
            //Assert.AreEqual(list[1].count, 0);
            //Assert.AreEqual(list[2].count, 0);
            //Assert.AreEqual(list[3].count, 0);
        }

        /// <summary>
        /// db中有相关数据
        /// </summary>
        [TestMethod]
        public void GetFlowProcessList_Test002()
        {
            DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetFlowProcessList_Test002");

            //var list = flowIndexBll.GetFlowProcessList(2015, 7, 247);
            //Assert.AreEqual(list.Count, 4);
            //Assert.AreEqual(list[0].count, 2);
            //Assert.AreEqual(list[1].count, 13);
            //Assert.AreEqual(list[2].count, 2);
            //Assert.AreEqual(list[3].count, 2);
        }
        #endregion

        #region 获取模板详情(GetTemplateInfoById)
         /// <summary>
        /// db中没有相关数据
        /// </summary>
        //[TestMethod]
        //public void GetTemplateInfoById_Test001()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateInfoById_Test001");

        //    var model = flowIndexBll.GetTemplateInfoById(21);
        //    Assert.AreEqual(model,null);
        //}

        /// <summary>
        /// db中有相关数据
        /// </summary>
        //[TestMethod]
        //public void GetTemplateInfoById_Test002()
        //{
        //    DataUtility.InsertDataBase("FlowIndexBLLTestData.xlsx", "GetTemplateInfoById_Test002");

        //    var model = flowIndexBll.GetTemplateInfoById(21);
        //    Assert.AreNotEqual(model, null);
        //    //验证模板信息
        //    Assert.AreEqual(model.template.categoryId,1);
        //    Assert.AreEqual(model.template.contents, "mubiaodaohang");
        //    Assert.AreEqual(model.template.defaultTitle, 0);
        //    Assert.AreEqual(model.template.description, "我同意该流程");
        //    Assert.AreEqual(model.template.id, 0);
        //    Assert.AreEqual(model.template.isCategory, false);
        //    Assert.AreEqual(model.template.name, null);
        //    Assert.AreEqual(model.template.templateName, "同意操作");

        //    //验证控件信息表
        //    Assert.AreEqual(model.controlInfo.Count,3);
        //    Assert.AreEqual(model.controlInfo[0].formula, null);
        //    Assert.AreEqual(model.controlInfo[0].item, null);
        //    Assert.AreEqual(model.controlInfo[0].control.color, null);
        //    Assert.AreEqual(model.controlInfo[0].control.columnIndex, null);
        //    Assert.AreEqual(model.controlInfo[0].control.columnStatistics, 0);
        //    Assert.AreEqual(model.controlInfo[0].control.controlId, "1");
        //    Assert.AreEqual(model.controlInfo[0].control.controlType, 1);
        //    Assert.AreEqual(model.controlInfo[0].control.defaultRowNum, null);
        //    Assert.AreEqual(model.controlInfo[0].control.description, null);
        //    Assert.AreEqual(model.controlInfo[0].control.lineType, null);
        //    Assert.AreEqual(model.controlInfo[0].control.linked, null);
        //    Assert.AreEqual(model.controlInfo[0].control.loaded, 1);
        //    Assert.AreEqual(model.controlInfo[0].control.require, 1);
        //    Assert.AreEqual(model.controlInfo[0].control.size, 100);
        //    Assert.AreEqual(model.controlInfo[0].control.status, 0);
        //    Assert.AreEqual(model.controlInfo[0].control.templateId, 21);
        //    Assert.AreEqual(model.controlInfo[0].control.title, null);
        //}
        #endregion
    }
}
