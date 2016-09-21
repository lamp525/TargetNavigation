using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;

namespace MB.BLL.Test
{
    [TestClass]
    public class FLowEditBLLTest
    {
        MB.BLL.FlowEditBLL flowBLL = new MB.BLL.FlowEditBLL();

        #region 保存流程设置信息 （SaveNodeLink）
     
        /// <summary>
        /// 流程信息为空、被删除节点出口ID为空、被删除条件ID为空
        /// </summary>
        [TestMethod]
        public void SaveNodeLink_Test001()
        {
            // 导入测试数据
           DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            //节点流程信息
            var linkInfo = new List<NodeLinkInfoModel>();
            var deleteLinkId = new int[0];
            var deleteConditionId = new int[0];

            flowBLL.SaveNodeLink(linkInfo, deleteLinkId, deleteConditionId);
        }

        /// <summary>
        /// 流程信息为空、被删除节点出口ID不为空、被删除条件ID不为空
        /// </summary>
        [TestMethod]
         public void SaveNodeLink_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            //节点流程信息
            var linkInfo = new List<NodeLinkInfoModel>();
            var deleteLinkId = new int[2]{1,2};
            var deleteConditionId = new int[2]{1,2};

            flowBLL.SaveNodeLink(linkInfo, deleteLinkId, deleteConditionId);
        }
        
        /// <summary>
        /// 流程信息不为空、被删除节点出口ID为空、被删除条件ID为空
        /// </summary>
        [TestMethod]
        public void SaveFlowNode_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            //节点流程信息
            var linkInfo = new List<NodeLinkInfoModel>();
            var deleteLinkId = new int[0];
            var deleteConditionId = new int[0];

            #region 测试数据

            #region 节点流程

            //节点流程信息
            var nodeLink1 = new NodeLinkModel
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeExitId = 2,
                status = 9999,
                templateId = 1
            };
            var nodeLink2 = new NodeLinkModel
            {             
                nodeEntryId = 88,
                nodeExitId = 99,
                status = 9999,
                templateId = 1
            };
            #endregion

            #region 流程条件
            //流程条件
            var linkConditionList1 = new List<LinkConditionModel>();
            var linkCondition1 = new LinkConditionModel
            {
                linkId = 1,
                serialNumber =111,
                type = 1,
                condition = 0,
                controlId = "插入1",
                targetId = new int?[2] { 11, 12 }

            };
            linkConditionList1.Add(linkCondition1);
            var linkCondition2 = new LinkConditionModel
            {
                linkId = 1,
                serialNumber = 222,
                type = 1,
                condition = 0,
                controlId = "插入2",
                targetId = new int?[2] { 21, 22 }
            };
            linkConditionList1.Add(linkCondition2);

            var linkConditionList2 = new List<LinkConditionModel>();
            var linkCondition3 = new LinkConditionModel
            {                
                type = 1,
                serialNumber = 333,
                condition = 0,
                controlId = "插入3",
                targetId = new int?[2] { 31, 32 }

            };
            linkConditionList2.Add(linkCondition3);
            var linkCondition4 = new LinkConditionModel
            {               
                type = 1,
                serialNumber = 444,
                condition = 0,
                controlId = "插入4",
                targetId = new int?[2] { 41, 42 }
            };
            linkConditionList2.Add(linkCondition4);

            #endregion

            #region 条件公式
            //条件公式信息
            var formulaList1 = new List<LinkFormulaModel>();
            var formula1 = new LinkFormulaModel
            {
                linkId = 1,
                operate = "(",
                orderNum = 1
            };
            formulaList1.Add(formula1);
            var formula2 = new LinkFormulaModel
            {
                linkId = 1,
                 serialNumber =111,
                orderNum = 2
            };
            formulaList1.Add(formula2);
            var formula3 = new LinkFormulaModel
            {
                linkId = 1,
                operate = "&",
                orderNum = 3
            };
            formulaList1.Add(formula3);
            var formula4 = new LinkFormulaModel
            {
                linkId = 1,
                serialNumber = 222,
                orderNum = 4
            };
            formulaList1.Add(formula4);
            var formula5 = new LinkFormulaModel
            {
                linkId = 1,
                operate = ")",
                orderNum = 5
            };
            formulaList1.Add(formula5);

            var formulaList2 = new List<LinkFormulaModel>();
            var formula11 = new LinkFormulaModel
            {
                linkId = 1,
                operate = "(",
                orderNum = 1
            };
            formulaList2.Add(formula11);
            var formula12 = new LinkFormulaModel
            {
                linkId = 1,
                serialNumber = 333,
                orderNum = 2
            };
            formulaList2.Add(formula12);
            var formula13 = new LinkFormulaModel
            {
                linkId = 1,
                operate = "&",
                orderNum = 3
            };
            formulaList2.Add(formula13);
            var formula14 = new LinkFormulaModel
            {
                linkId = 1,
                serialNumber = 444,
                orderNum = 4
            };
            formulaList2.Add(formula14);
            var formula15 = new LinkFormulaModel
            {
                linkId = 1,
                operate = ")",
                orderNum = 5
            };
            formulaList2.Add(formula15);
            #endregion
                        
            var nodeLinkInfo1 = new NodeLinkInfoModel();
            var nodeLinkInfo2 = new NodeLinkInfoModel();

            nodeLinkInfo1.nodeLinkMode = nodeLink1;
            nodeLinkInfo1.linkConditionList= linkConditionList1 ;
            nodeLinkInfo1.linkFormulaList = formulaList1;
            nodeLinkInfo2.nodeLinkMode = nodeLink2;
            nodeLinkInfo2.linkConditionList = linkConditionList2;
            nodeLinkInfo2.linkFormulaList = formulaList2;

            linkInfo.Add(nodeLinkInfo1);
            linkInfo.Add(nodeLinkInfo2);

            #endregion

            flowBLL.SaveNodeLink(linkInfo, deleteLinkId, deleteConditionId);

            #region 确认测试结果数据

            #region 节点流程表

            var linkList = new List<tblNodeLink>();

            using(var db = new TargetNavigationDBEntities())
            {
                linkList = db.tblNodeLink.Where(p => p.templateId == 1).ToList();
            }

            Assert.AreEqual(linkList.Count, 5);
            //更新
            Assert.AreEqual(linkList[0].linkId, 1);
            Assert.AreEqual(linkList[0].nodeEntry , 1);
            Assert.AreEqual(linkList[0].nodeExit, 2);
            Assert.AreEqual(linkList[0].status, 9999);

            //插入
            Assert.AreEqual(linkList[4].linkId, 5);
            Assert.AreEqual(linkList[4].nodeEntry, 88);
            Assert.AreEqual(linkList[4].nodeExit, 99);
            Assert.AreEqual(linkList[4].status, 9999);

            #endregion

            #region 流程条件表

            var conditionList = new List<tblLinkCondition>();

            using (var db = new TargetNavigationDBEntities())
            {
                conditionList = db.tblLinkCondition.ToList();
            }

            Assert.AreEqual(conditionList.Count, 20);

            Assert.AreEqual(conditionList[19].conditionId, 20);
            Assert.AreEqual(conditionList[19].linkId, 5);
            Assert.AreEqual(conditionList[19].type, 1);
            Assert.AreEqual(conditionList[19].controlId, "插入4");
            Assert.AreEqual(conditionList[19].condition, 0);
            Assert.IsNull (conditionList[19].value );          
            #endregion

            #region 条件结果表
            var resultList = new List<tblLinkResult>();

            using (var db = new TargetNavigationDBEntities())
            {
                resultList = db.tblLinkResult.ToList();
            }

            Assert.AreEqual(resultList.Count, 40);

            Assert.AreEqual(resultList[39].resultId, 40);
            Assert.AreEqual(resultList[39].conditionId, 20);
            Assert.AreEqual(resultList[39].targetId, 42); 

            #endregion

            #region 条件公式表
            var formulaList = new List<tblLinkFormula>();

            using (var db = new TargetNavigationDBEntities())
            {
                formulaList = db.tblLinkFormula.ToList();
            }

            Assert.AreEqual(formulaList.Count, 43);

            Assert.AreEqual(formulaList[38].formulaId, 50);
            Assert.AreEqual(formulaList[38].linkId, 5);
            Assert.IsNull(formulaList[38].conditionId);
            Assert.AreEqual(formulaList[38].operate, "(");
            Assert.AreEqual(formulaList[38].orderNum,1);

            Assert.AreEqual(formulaList[39].formulaId, 51);
            Assert.AreEqual(formulaList[39].linkId, 5);     
            Assert.AreEqual(formulaList[39].conditionId , 19);
            Assert.IsNull(formulaList[39].operate);
            Assert.AreEqual(formulaList[39].orderNum, 2);

            Assert.AreEqual(formulaList[40].formulaId, 52);
            Assert.AreEqual(formulaList[40].linkId, 5);
            Assert.IsNull(formulaList[40].conditionId);
            Assert.AreEqual(formulaList[40].operate, "&");
            Assert.AreEqual(formulaList[40].orderNum, 3);

            Assert.AreEqual(formulaList[41].formulaId, 53);
            Assert.AreEqual(formulaList[41].linkId, 5);
            Assert.AreEqual(formulaList[41].conditionId, 20);
            Assert.IsNull(formulaList[41].operate);
            Assert.AreEqual(formulaList[41].orderNum, 4);

            Assert.AreEqual(formulaList[42].formulaId, 54);
            Assert.AreEqual(formulaList[42].linkId, 5);
            Assert.IsNull(formulaList[42].conditionId);
            Assert.AreEqual(formulaList[42].operate, ")");
            Assert.AreEqual(formulaList[42].orderNum, 5);

            #endregion

            #endregion

        }

        #endregion            


    }
}
