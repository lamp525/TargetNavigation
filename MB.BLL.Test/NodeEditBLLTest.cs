using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;

namespace MB.BLL.Test
{
    [TestClass]
    public class NodeEditBLLTest
    {
        MB.BLL.NodeEditBLL nodeBLL = new MB.BLL.NodeEditBLL();

         #region 取得节点字段信息 ( GetNodeFieldList )
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeFieldList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            var temp = nodeBLL.GetNodeFieldList(999,1);
            Assert.AreEqual(temp.Count(), 0);
        }

        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeFieldList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            var temp = nodeBLL.GetNodeFieldList(1,1);

            var item = temp.First();

            Assert.AreEqual(item.nodeId, 1);
            Assert.AreEqual(item.controlId, "1");
            Assert.AreEqual(item.status, 0);
        }
        #endregion

        #region 保存节点信息设置 （SaveFlowNode）
        /// <summary>
        /// 流程节点信息为空、被删除节点ID为空、被删除操作ID为空
        /// </summary>
        [TestMethod]
        public void SaveFlowNode_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");
            
            //流程节点信息
            var nodeInfo = new List<NodeInfoModel>();
            var deleteNode = new int[0];
            var deleteOperateId = new int[0];

            nodeBLL.SaveFlowNode(nodeInfo, deleteNode, deleteOperateId);
        }

        /// <summary>
        /// 流程节点信息为空、被删除节点ID不为空、被删除操作ID不为空
        /// </summary>
        [TestMethod]
        public void SaveFlowNode_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            //流程节点信息
            var nodeInfo = new List<NodeInfoModel>();
            var deleteNode = new int[2] { 1, 2 };
            var deleteOperateId = new int[2] { 1, 2 };

            nodeBLL.SaveFlowNode(nodeInfo, deleteNode, deleteOperateId);
        } 

        /// <summary>
        /// 流程节点信息不为空、被删除节点ID为空、被删除操作ID为空
        /// </summary>
        [TestMethod]
        public void SaveFlowNode_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowBLLTestData.xlsx", "");

            //流程节点信息
            var nodeInfo = new List<NodeInfoModel>();
            var deleteNode = new int[0];
            var deleteOperateId = new int[0];

            #region 测试数据
            //流程节点信息
            var nodeModel1 = new NodeModel
            {
                nodeId = 1,
                nodeName = "更新咯",
                nodeType = 3,
                templateId = 1
            };

            //节点字段信息
            var nodeField1 = new NodeFieldModel
            {
                nodeId = 1,
                controlId = "1",
                status = 1
            };
            var nodeField2 = new NodeFieldModel
            {
                nodeId = 1,
                controlId = "2",
                status = 2
            };
            var nodeField3 = new NodeFieldModel
            {
                nodeId = 1,
                controlId = "3",
                status = 0
            };

            //流程节点信息
            var nodeModel999 = new NodeModel
            {
                nodeName = "插入呃！",
                nodeType = 1,
                templateId = 1
            };
            //流程节点信息
            var nodeField4 = new NodeFieldModel
            {
                controlId = "1",
                status = 1
            };
            var nodeField5 = new NodeFieldModel
            {
                controlId = "2",
                status = 2
            };
            var nodeField6 = new NodeFieldModel
            {
                controlId = "3",
                status = 0
            };

            //节点操作信息
            var nodeOperateModel1 = new NodeOperateModel
            {
                nodeId =1,
                batchCondition = 1,
                batchType = 1,
                condition = 1,
                type = 1,
                countersign = 0,
                targetId = new int?[2] { 1, 2 },                
                batchTargetId = new int?[2] { 1, 2 },               

            };

            var nodeOperateModel2 = new NodeOperateModel
            {
                nodeId =1,
                batchCondition = 2,
                batchType = 2,
                condition = 2,
                type = 2,
                countersign = 0,
                targetId = new int?[2] { 3, 4 },              
                batchTargetId = new int?[2] { 3, 4 },
               
            };

            var nodeOperateModel3 = new NodeOperateModel
            {
                batchCondition = 1,
                batchType = 1,
                condition = 1,
                type = 1,
                countersign = 0,
                targetId = new int?[2] { 5, 6 },              
                batchTargetId = new int?[2] { 5, 6 },               

            };

            var nodeOperateModel4 = new NodeOperateModel
            {
                batchCondition = 2,
                batchType = 2,
                condition = 2,
                type = 2,
                countersign = 0,
                targetId = new int?[2] { 7, 8 },                
                batchTargetId = new int?[2] { 7, 8 },
               
            };     

            var nodeInfoModel1 = new NodeInfoModel();
            
            nodeInfoModel1.node = nodeModel1;
            var nodeFieldList1 = new List<NodeFieldModel>();
            nodeFieldList1.Add(nodeField1);
            nodeFieldList1.Add(nodeField2);
            nodeFieldList1.Add(nodeField3);
            nodeInfoModel1.nodeField  = nodeFieldList1;
            var nodeOperateList1 = new List<NodeOperateModel>();
            nodeOperateList1.Add(nodeOperateModel1);
            nodeOperateList1.Add(nodeOperateModel2);
            nodeInfoModel1.nodeOperate= nodeOperateList1;

            var nodeInfoModel2 = new NodeInfoModel();

            nodeInfoModel2.node = nodeModel999;
            var nodeFieldList2 = new List<NodeFieldModel>();
            nodeFieldList2.Add(nodeField4);
            nodeFieldList2.Add(nodeField5);
            nodeFieldList2.Add(nodeField6);
            nodeInfoModel2.nodeField = nodeFieldList2;
            var nodeOperateList2 = new List<NodeOperateModel>();
            nodeOperateList2.Add(nodeOperateModel3);
            nodeOperateList2.Add(nodeOperateModel4);
            nodeInfoModel2.nodeOperate = nodeOperateList2;
              

            nodeInfo.Add(nodeInfoModel1);
            nodeInfo.Add(nodeInfoModel2);

            #endregion

            nodeBLL.SaveFlowNode(nodeInfo, deleteNode, deleteOperateId);

            #region 确认测试结果数据

            #region 流程节点表

            var nodeList = new List<tblFlowNode>();

            using (var db = new TargetNavigationDBEntities())
            {
                nodeList = db.tblFlowNode.Where(p => p.templateId == 1).ToList();
            }

            Assert.AreEqual(nodeList.Count, 5);
            //更新
            Assert.AreEqual(nodeList[0].nodeId, 1);
            Assert.AreEqual(nodeList[0].nodeName, "更新咯");
            Assert.AreEqual(nodeList[0].nodeType, 3);
            //插入
            Assert.AreEqual(nodeList[4].nodeId, 5);
            Assert.AreEqual(nodeList[4].nodeName, "插入呃！");
            Assert.AreEqual(nodeList[4].nodeType, 1);

            #endregion

            #region 节点字段表

            var fieldList = new List<tblNodeField>();

            using (var db = new TargetNavigationDBEntities())
            {
                fieldList = db.tblNodeField.ToList();
            }

            Assert.AreEqual(fieldList.Count, 9);
            //更新
            Assert.AreEqual(fieldList[0].nodeId, 1);
            Assert.AreEqual(fieldList[0].controlId, "1");
            Assert.AreEqual(fieldList[0].status, 1);
            //插入
            Assert.AreEqual(fieldList[8].nodeId, 5);
            Assert.AreEqual(fieldList[8].controlId, "3");
            Assert.AreEqual(fieldList[8].status, 0);

            #endregion

            #region 节点操作人表

            var nodeOperateList = new List<tblNodeOperate>();

            using (var db = new TargetNavigationDBEntities())
            {
                nodeOperateList = db.tblNodeOperate.ToList();
            }

            Assert.AreEqual(nodeOperateList.Count, 8);
            //插入
            Assert.AreEqual(nodeOperateList[7].operateId , 8);
            Assert.AreEqual(nodeOperateList[7].nodeId , 5);
            Assert.AreEqual(nodeOperateList[7].type , 2);
            Assert.AreEqual(nodeOperateList[7].condition , 2);
            Assert.AreEqual(nodeOperateList[7].countersign,0);
            Assert.AreEqual(nodeOperateList[7].batchType, 2);
            Assert.AreEqual(nodeOperateList[7].batchCondition, 2);

            #endregion

            #region 操作人结果表

            var operateResultList = new List<tblOperateResult >();

            using (var db = new TargetNavigationDBEntities())
            {
                operateResultList = db.tblOperateResult.ToList();
            }

            Assert.AreEqual(operateResultList.Count, 12);
            //插入
            Assert.AreEqual(operateResultList[11].operateId, 8);
            Assert.AreEqual(operateResultList[11].resultId, 12);
            Assert.AreEqual(operateResultList[11].targetId , 8);        

            #endregion

            #region 批次条件结果表

            var batchResultList = new List<tblBatchResult >();

            using (var db = new TargetNavigationDBEntities())
            {
                batchResultList = db.tblBatchResult.ToList();
            }

            Assert.AreEqual(batchResultList.Count, 12);
            //插入
            Assert.AreEqual(batchResultList[11].operateId, 8);
            Assert.AreEqual(batchResultList[11].resultId, 12);
            Assert.AreEqual(batchResultList[11].targetId, 8);

            #endregion

            #endregion
        }

        #endregion
    }
}
