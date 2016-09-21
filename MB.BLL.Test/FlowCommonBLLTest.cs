using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MB.Model;
using MB.DAL;

namespace MB.BLL.Test
{
    [TestClass]
    public class FlowCommonBLLTest
    {
        MB.BLL.FlowCommonBLL flowCommonBLL = new FlowCommonBLL();

        #region 节点设置验证 ( CheckNode )
        /// <summary>
        /// 流程节点列表为空，节点操作人列表为空
        /// </summary>
        [TestMethod]
        public void CheckNode_Test001()
        {
            var nodeList = new List<NodeModel>();
            var nodeOperateList = new List<NodeOperateModel>();

            //var result = flowCommonBLL.CheckNode(nodeList, nodeOperateList);
            //Assert.IsFalse(result);
        }

        /// <summary>
        /// 流程节点列表不为空，节点操作人列表不为空 不存在节点类型为创建的节点
        /// </summary>
        [TestMethod]
        public void CheckNode_Test002()
        {
            var nodeList = new List<NodeModel>();
            var nodeOperateList = new List<NodeOperateModel>();

            var node1 = new NodeModel()
            {
                nodeId = 1,
                nodeName = "name1",
                nodeType = 2
            };
            nodeList.Add(node1);

            var nodeOperate1 = new NodeOperateModel()
            {
                nodeId = 1,
                countersign =0,
                 targetId = new int?[1]{1}
            };

            nodeOperateList.Add(nodeOperate1);
      

            //var result = flowCommonBLL.CheckNode(nodeList, nodeOperateList);
            //Assert.IsFalse(result);
        }

        /// <summary>
        /// 流程节点列表不为空，节点操作人列表不为空 不存在节点类型为归档的节点
        /// </summary>
        [TestMethod]
        public void CheckNode_Test003()
        {
            var nodeList = new List<NodeModel>();
            var nodeOperateList = new List<NodeOperateModel>();

            var node1 = new NodeModel()
            {
                nodeId = 1,
                nodeName = "name1",
                nodeType = 1
            };
            nodeList.Add(node1);

            var nodeOperate1 = new NodeOperateModel()
            {
                nodeId = 1,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };
            nodeOperateList.Add(nodeOperate1);

            var node2 = new NodeModel()
            {
                nodeId = 2,
                nodeName = "name2",
                nodeType = 2
            };
            nodeList.Add(node2);

            var nodeOperate2 = new NodeOperateModel()
            {
                nodeId = 2,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };

            nodeOperateList.Add(nodeOperate2);


            //var result = flowCommonBLL.CheckNode(nodeList, nodeOperateList);
            //Assert.IsFalse(result);
        }

        /// <summary>
        /// 流程节点列表不为空，节点操作人列表不为空 存在无操作人的节点
        /// </summary>
        [TestMethod]
        public void CheckNode_Test004()
        {
            var nodeList = new List<NodeModel>();
            var nodeOperateList = new List<NodeOperateModel>();

            var node1 = new NodeModel()
            {
                nodeId = 1,
                nodeName = "name1",
                nodeType = 1
            };
            nodeList.Add(node1);

            var nodeOperate1 = new NodeOperateModel()
            {
                nodeId = 2,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };
            nodeOperateList.Add(nodeOperate1);

            //var result = flowCommonBLL.CheckNode(nodeList, nodeOperateList);
            //Assert.IsFalse(result);
        }

        /// <summary>
        /// 流程节点列表不为空，节点操作人列表不为空 节点状态为“无”的节点存在多个操作人
        /// </summary>
        [TestMethod]
        public void CheckNode_Test005()
        {
            var nodeList = new List<NodeModel>();
            var nodeOperateList = new List<NodeOperateModel>();

            var node1 = new NodeModel()
            {
                nodeId = 1,
                nodeName = "name1",
                nodeType = 1
            };
            nodeList.Add(node1);

            var nodeOperate1 = new NodeOperateModel()
            {
                nodeId = 1,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };
            nodeOperateList.Add(nodeOperate1);

  
            var nodeOperate2 = new NodeOperateModel()
            {
                nodeId = 1,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };

            nodeOperateList.Add(nodeOperate2);


            //var result = flowCommonBLL.CheckNode(nodeList, nodeOperateList);
            //Assert.IsFalse(result);
        }

        /// <summary>
        /// 流程节点列表不为空，节点操作人列表不为空 流程节点设置正确
        /// </summary>
        [TestMethod]
        public void CheckNode_Test006()
        {
            var nodeList = new List<NodeModel>();
            var nodeOperateList = new List<NodeOperateModel>();

            var node1 = new NodeModel()
            {
                nodeId = 1,
                nodeName = "name1",
                nodeType = 1
            };
            nodeList.Add(node1);

            var nodeOperate1 = new NodeOperateModel()
            {
                nodeId = 1,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };
            nodeOperateList.Add(nodeOperate1);

            var node2 = new NodeModel()
            {
                nodeId = 2,
                nodeName = "name2",
                nodeType = 2
            };
            nodeList.Add(node2);

            var nodeOperate2 = new NodeOperateModel()
            {
                nodeId = 2,
                countersign = 0,
                targetId = new int?[1] { 1 }
            };
            nodeOperateList.Add(nodeOperate2);

            var node3 = new NodeModel()
            {
                nodeId = 3,
                nodeName = "name3",
                nodeType = 3
            };
            nodeList.Add(node3);

            var nodeOperate3 = new NodeOperateModel()
            {
                nodeId = 3,
                countersign = 1,
                targetId = new int?[2] { 1,2 }
            };
            nodeOperateList.Add(nodeOperate3);

            var node4 = new NodeModel()
            {
                nodeId = 4,
                nodeName = "name4",
                nodeType = 4
            };
            nodeList.Add(node4);

            var nodeOperate4 = new NodeOperateModel()
            {
                nodeId = 4,        
                countersign =1,
                targetId = new int?[1] { 1 }
            };
            nodeOperateList.Add(nodeOperate4);


            //var result = flowCommonBLL.CheckNode(nodeList, nodeOperateList);
            //Assert.IsTrue(result);
        }
        #endregion

        #region 流程设置验证 ( CheckFlow )
        /// <summary>
        /// 节点流程列表为空
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test001()
        {
            var linkList = new List<NodeLinkModel>();

            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 入口不存在“创建类型”的类型
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test002()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "不是创建",
                nodeEntryType = 99,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 1
            };

            var link2 = new NodeLinkModel()
            {
                linkId = 2,
                nodeEntryId = 2,
                nodeEntryName = "提交",
                nodeEntryType = 2,
                nodeExitId = 3,
                nodeExitName = "审批1",
                nodeExitType = 3,
                status = 1
            };

            var link3 = new NodeLinkModel()
            {
                linkId = 3,
                nodeEntryId = 3,
                nodeEntryName = "审批1",
                nodeEntryType = 3,
                nodeExitId = 4,
                nodeExitName = "审批2",
                nodeExitType = 3,
                status = 1
            };

            var link4 = new NodeLinkModel()
            {
                linkId = 4,
                nodeEntryId = 4,
                nodeEntryName = "审批2",
                nodeEntryType = 3,
                nodeExitId = 5,
                nodeExitName = "归档",
                nodeExitType = 4,
                status = 1
            };

            var link5 = new NodeLinkModel()
            {
                linkId = 5,
                nodeEntryId = 3,
                nodeEntryName = "审批1",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 0
            };

            var link6 = new NodeLinkModel()
            {
                linkId = 6,
                nodeEntryId = 4,
                nodeEntryName = "审批2",
                nodeEntryType = 3,
                nodeExitId = 3,
                nodeExitName = "审批1",
                nodeExitType = 3,
                status = 0
            };
            linkList.Add(link1);
            linkList.Add(link2);
            linkList.Add(link3);
            linkList.Add(link4);
            linkList.Add(link5);
            linkList.Add(link6);

            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 出口不存在“归档类型”的类型
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test003()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "创建",
                nodeEntryType = 1,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 1
            };

            var link2 = new NodeLinkModel()
            {
                linkId = 2,
                nodeEntryId = 2,
                nodeEntryName = "提交",
                nodeEntryType = 2,
                nodeExitId = 3,
                nodeExitName = "审批1",
                nodeExitType = 3,
                status = 1
            };

            var link3 = new NodeLinkModel()
            {
                linkId = 3,
                nodeEntryId = 3,
                nodeEntryName = "审批1",
                nodeEntryType = 3,
                nodeExitId = 4,
                nodeExitName = "审批2",
                nodeExitType = 3,
                status = 1
            };

            var link4 = new NodeLinkModel()
            {
                linkId = 4,
                nodeEntryId = 4,
                nodeEntryName = "审批2",
                nodeEntryType = 3,
                nodeExitId = 5,
                nodeExitName = "不是归档",
                nodeExitType =99,
                status = 1
            };

            var link5 = new NodeLinkModel()
            {
                linkId = 5,
                nodeEntryId = 3,
                nodeEntryName = "审批1",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 0
            };

            var link6 = new NodeLinkModel()
            {
                linkId = 6,
                nodeEntryId = 4,
                nodeEntryName = "审批2",
                nodeEntryType = 3,
                nodeExitId = 3,
                nodeExitName = "审批1",
                nodeExitType = 3,
                status = 0
            };

            linkList.Add(link1);
            linkList.Add(link2);
            linkList.Add(link3);
            linkList.Add(link4);
            linkList.Add(link5);
            linkList.Add(link6);

            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 入口节点类型为”归档“
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test004()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "归档",
                nodeEntryType = 4,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 1
            };
            linkList.Add(link1);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 出口节点类型为“创建”
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test005()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "审批",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "创建",
                nodeExitType = 1,
                status = 1
            };
            linkList.Add(link1);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 入口节点类型为"提交"时，节点出口状态为不通过
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test006()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "提交",
                nodeEntryType = 2,
                nodeExitId = 2,
                nodeExitName = "审批",
                nodeExitType = 3,
                status = 0
            };
            linkList.Add(link1);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 出口节点类型为"归档"时，节点出口状态为不通过
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test007()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "审批",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "归档",
                nodeExitType = 4,
                status = 0
            };
            linkList.Add(link1);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 “提交”节点不同时存在于入口和出口节点中
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test008()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "提交",
                nodeEntryType = 2,
                nodeExitId = 2,
                nodeExitName = "审批",
                nodeExitType = 3,
                status = 1
            };
            linkList.Add(link1);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 “审批”节点不同时存在于入口和出口节点中
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test009()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "审批",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "审批",
                nodeExitType = 3,
                status = 1
            };
            linkList.Add(link1);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }

        /// <summary>
        /// 节点流程列表不为空 入口节点类型为"审批"时，不同时存在通过和不通过
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test010()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "审批",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "审批",
                nodeExitType = 3,
                status = 1
            };
            var link2 = new NodeLinkModel()
            {
                linkId = 2,
                nodeEntryId = 2,
                nodeEntryName = "审批",
                nodeEntryType = 3,
                nodeExitId = 1,
                nodeExitName = "审批",
                nodeExitType = 3,
                status = 1
            };
            linkList.Add(link1);
            linkList.Add(link2);
            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsFalse(result);
        }


        /// <summary>
        /// 流程设置正确
        /// </summary>
        [TestMethod]
        public void CheckFlow_Test011()
        {
            var linkList = new List<NodeLinkModel>();

            var link1 = new NodeLinkModel()
            {
                linkId = 1,
                nodeEntryId = 1,
                nodeEntryName = "创建",
                nodeEntryType = 1,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 1
            };

            var link2 = new NodeLinkModel()
            {
                linkId = 2,
                nodeEntryId = 2,
                nodeEntryName = "提交",
                nodeEntryType = 2,
                nodeExitId = 3,
                nodeExitName = "审批1",
                nodeExitType = 3,
                status = 1
            };

            var link3 = new NodeLinkModel()
            {
                linkId = 3,
                nodeEntryId = 3,
                nodeEntryName = "审批1",
                nodeEntryType = 3,
                nodeExitId = 4,
                nodeExitName = "审批2",
                nodeExitType = 3,
                status = 1
            };

            var link4 = new NodeLinkModel()
            {
                linkId = 4,
                nodeEntryId = 4,
                nodeEntryName = "审批2",
                nodeEntryType = 3,
                nodeExitId = 5,
                nodeExitName = "归档",
                nodeExitType = 4,
                status = 1
            };

            var link5 = new NodeLinkModel()
            {
                linkId = 5,
                nodeEntryId = 3,
                nodeEntryName = "审批1",
                nodeEntryType = 3,
                nodeExitId = 2,
                nodeExitName = "提交",
                nodeExitType = 2,
                status = 0
            };

            var link6 = new NodeLinkModel()
            {
                linkId = 6,
                nodeEntryId = 4,
                nodeEntryName = "审批2",
                nodeEntryType = 3,
                nodeExitId = 3,
                nodeExitName = "审批1",
                nodeExitType = 3,
                status = 0
            };
            linkList.Add(link1);
            linkList.Add(link2);
            linkList.Add(link3);
            linkList.Add(link4);
            linkList.Add(link5);
            linkList.Add(link6);

            var result = flowCommonBLL.CheckFlow(linkList);
            Assert.IsTrue(result);
        }
        #endregion

        #region 取得节点流程 ( GetNodeLinkList )
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeLinkList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetNodeLinkList_Test001");
            var linkList = new List<NodeLinkModel>();

            linkList = flowCommonBLL.GetNodeLinkList(22);
            Assert.AreEqual(linkList.Count, 0);
        }

        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeLinkList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetNodeLinkList_Test002");
            var linkList = new List<NodeLinkModel>();

            linkList = flowCommonBLL.GetNodeLinkList(1);

            Assert.AreEqual(linkList.Count, 4);
            Assert.AreEqual(linkList[0].linkId, 1);
            Assert.AreEqual(linkList[0].nodeEntryId, 1);
            Assert.AreEqual(linkList[0].nodeEntryName, "申请人");
            Assert.AreEqual(linkList[0].nodeEntryType, 1);
            Assert.AreEqual(linkList[0].nodeExitId, 2);
            Assert.AreEqual(linkList[0].nodeExitName, "直接上级");
            Assert.AreEqual(linkList[0].nodeExitType, 2);
            Assert.AreEqual(linkList[0].status, 1);

        }
        #endregion

        #region 取得流程条件信息 （GetLinkConditionInfo）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetLinkConditionInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetLinkConditionInfo_Test001");
            var conditionList = new List<LinkConditionModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                conditionList = flowCommonBLL.GetLinkConditionInfo(db, 999, 1);
            }
            Assert.AreEqual(conditionList.Count, 0);
        }

        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetLinkConditionInfo_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetLinkConditionInfo_Test002");
            var conditionList = new List<LinkConditionModel>();

            conditionList = flowCommonBLL.GetLinkConditionInfo(new TargetNavigationDBEntities(), 1, 1);

            Assert.AreEqual(conditionList.Count, 5);
            Assert.AreEqual(conditionList[0].conditionId, 1);
            Assert.AreEqual(conditionList[0].linkId, 1);
            Assert.AreEqual(conditionList[0].type, 1);
            Assert.AreEqual(conditionList[0].condition, 1);
            Assert.AreEqual(conditionList[0].controlId, "");
            Assert.AreEqual(conditionList[0].controlTitle, null);
            Assert.AreEqual(conditionList[0].controlType, null);
            Assert.AreEqual(conditionList[0].serialNumber, null);
            Assert.AreEqual(conditionList[0].targetId[0], 3);
            Assert.AreEqual(conditionList[0].targetName[0], "资本运营");

            Assert.AreEqual(conditionList[4].conditionId, 5);
            Assert.AreEqual(conditionList[4].linkId, 1);
            Assert.AreEqual(conditionList[4].type, 99);
            Assert.AreEqual(conditionList[4].condition, 2);
            Assert.AreEqual(conditionList[4].controlId, string.Empty);
            Assert.AreEqual(conditionList[4].controlTitle, null);
            Assert.AreEqual(conditionList[4].controlType, null);
            Assert.AreEqual(conditionList[4].serialNumber, null);
            Assert.AreEqual(conditionList[4].targetId.Length, 0);
            Assert.AreEqual(conditionList[4].targetName, null);
        }

        #endregion

        #region 取得节点操作人信息 ( GetNodeOperateList )
                        
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeOperateList_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetNodeOperateList_Test001");

            var operateList = new List<NodeOperateModel>();
            operateList = flowCommonBLL.GetTemplateNodeOperateList(999);
            Assert.AreEqual(operateList.Count, 0);
        }
      
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeOperateList_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetNodeOperateList_Test002");

            var operateList = new List<NodeOperateModel>();
            var userInfo = new UserInfoSimpleModel();

            using (var db = new TargetNavigationDBEntities())
            {
                //operateList = flowCommonBLL.GetNodeOperateList(db, 999, userInfo);
            }

            Assert.AreEqual(operateList.Count, 0);
        }
          
        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeOperateList_Test003()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetNodeOperateList_Test003");

            var operateList = new List<NodeOperateModel>();

           operateList = flowCommonBLL.GetTemplateNodeOperateList(3);

            Assert.AreEqual(operateList.Count, 5);

            var info = operateList[0];

            Assert.AreEqual(info.batchCondition, 1);
            Assert.AreEqual(info.batchTargetId[0], 3);
            Assert.AreEqual(info.batchTargetName[0], "资本运营");
            Assert.AreEqual(info.batchType, 1);
            Assert.AreEqual(info.condition, 1);
            Assert.AreEqual(info.countersign, 0);
            Assert.AreEqual(info.nodeId, 3);
            Assert.AreEqual(info.operateId, 1);
            Assert.AreEqual(info.targetId[0], 3);
            Assert.AreEqual(info.targetName[0], "资本运营");
            Assert.AreEqual(info.type, 1);
        }

        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetNodeOperateList_Test004()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetNodeOperateList_Test004");

            var operateList = new List<NodeOperateModel>();
            var userInfo = new UserInfoSimpleModel
           {
               organizationId = 3,
               stationId = 2,
               userId = 44
           };

            using (var db = new TargetNavigationDBEntities())
            {
              //  operateList = flowCommonBLL.GetNodeOperateList(db, 3, userInfo);
            }
            Assert.AreEqual(operateList.Count, 10);

            var info = operateList[0];

            Assert.AreEqual(info.batchCondition, 1);
            Assert.AreEqual(info.batchTargetId, null);
            Assert.AreEqual(info.batchTargetName ,null );
            Assert.AreEqual(info.batchType, 3);
            Assert.AreEqual(info.condition, 1);
            Assert.AreEqual(info.countersign, 0);
            Assert.AreEqual(info.nodeId, 3);
            Assert.AreEqual(info.operateId, 1);
            Assert.AreEqual(info.targetId[0], 3);
            Assert.AreEqual(info.targetName[0], "周东兵");            

        }
        #endregion

        #region 取得流程条件公式信息 （GetLinkFormulaInfo）
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetLinkFormulaInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetLinkFormulaInfo_Test001");
            var formulaList = new List<LinkFormulaModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                formulaList = flowCommonBLL.GetLinkFormulaInfo(db, 999);
            }
            Assert.AreEqual(formulaList.Count, 0);
        }

        /// <summary>
        /// DB中有相关数据
        /// </summary>
        [TestMethod]
        public void GetLinkFormulaInfo_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetLinkFormulaInfo_Test002");
            var formulaList = new List<LinkFormulaModel>();
            using (var db = new TargetNavigationDBEntities())
            {
                formulaList = flowCommonBLL.GetLinkFormulaInfo(db, 1);
            }
            Assert.AreEqual(formulaList.Count, 11);

            var info = formulaList[0];


            Assert.AreEqual(info.conditionId, null);
            Assert.AreEqual(info.displayText, string.Empty);
            Assert.AreEqual(info.formulaId, 1);
            Assert.AreEqual(info.linkId, 1);
            Assert.AreEqual(info.operate, "(");
            Assert.AreEqual(info.orderNum, 1);
            Assert.AreEqual(info.serialNumber, null);
        }

        #endregion

        #region 取得表单创建用户信息 ( GetCreateUserInfo )
        /// <summary>
        /// DB中没有相关数据
        /// </summary>
        [TestMethod]
        public void GetCreateUserInfo_Test001()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetCreateUserInfo_Test001");
            var userInfo = new UserInfoSimpleModel();

            using(var db = new TargetNavigationDBEntities())
            {
                userInfo = flowCommonBLL.GetCreateUserInfo(db,999);
            }
            Assert.IsNull(userInfo);
        }

        /// <summary>
        /// DB中存在相关数据
        /// </summary>
        [TestMethod]
        public void GetCreateUserInfo_Test002()
        {
            // 导入测试数据
            DataUtility.InsertDataBase("FLowCommonBLLTestData.xlsx", "GetCreateUserInfo_Test002");
            var userInfo = new UserInfoSimpleModel();

            using (var db = new TargetNavigationDBEntities())
            {
                userInfo = flowCommonBLL.GetCreateUserInfo(db, 47);
            }
            Assert.AreEqual(userInfo.smallImage, "smallImage");
            Assert.AreEqual(userInfo.userId, 47);
            Assert.AreEqual(userInfo.userName, "章镇");
            Assert.AreEqual(userInfo.stationId, 4);
            Assert.AreEqual(userInfo.organizationId, 1);

        }
        #endregion

    }
}
