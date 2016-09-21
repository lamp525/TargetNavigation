using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.Client.Common
{
    public static class AnalysisHelper
    {
        #region 解析第一次登录的消息
        /// <summary>
        /// 解析第一次登录的消息
        /// </summary>
        /// <param name="msg">消息包</param>
        /// <param name="protocol">协议头</param>
        /// <returns></returns>
        public static string AnalysisFirstLoginMsg(string operateMsg, string protocol)
        {
            var showMsg = string.Empty;
            if (protocol == Protocol.ClientLoginProtocol.PHC)
            {
                var operateArray = operateMsg.Split('*');
                if (operateArray.Length > 0)
                {
                    showMsg = "您今天需要完成以下计划：\n";
                    for (int i = 0; i < operateArray.Length; i++)
                    {
                        showMsg += (i + 1) + "、" + operateArray[i] + "\n\n";
                    }
                }
            }
            else
            {
                showMsg = "您今天还未提交计划，请记得及时提交！\n";
            }
            return showMsg;
        }
        #endregion

        #region 解析即时提醒的信息
        /// <summary>
        /// 解析即时提醒的信息
        /// </summary>
        /// <param name="warningArry">提示信息集合</param>
        /// <returns>拼接完成的即时提醒信息</returns>
        public static string AnalysisIM(string[] operates)
        {
            var returnMsg = string.Empty;
            if (operates.Length >= 4)
            {
                var userName = operates[2];
                var protocol = operates[3];
                if (protocol == Protocol.ClientActualTimeProtocol.PUC)
                {
                    returnMsg += userName + "的计划已提交，需要您及时审核，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.PUA)
                {
                    returnMsg += userName + "的计划已完成，需要您及时确认，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.PAU)
                {
                    returnMsg += userName + "的计划看来是写错了，需要您同意他修改，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.PAS)
                {
                    returnMsg += userName + "的计划真的不用做了吗？点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.OUC)
                {
                    returnMsg += userName + "的目标已提交，需要您及时审核，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.OUA)
                {
                    returnMsg += userName + "的目标已完成，需要您及时确认，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.FUS)
                {
                    returnMsg += userName + "的流程需要您提交信息，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.FUC)
                {
                    returnMsg += userName + "的流程需要您及时审核，点此查看！\n";
                }
            }
            else
            {
                var protocol = operates[2];
                if (protocol == Protocol.ClientActualTimeProtocol.PUS)
                {
                    returnMsg += "您已成功新建计划，请及时提交！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.PNS)
                {
                    returnMsg += "您的计划已审核通过，请您尽快提交，点此查看！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.OUS)
                {
                    returnMsg += "您已成功新建目标，请及时提交！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.ONS)
                {
                    returnMsg += "您的目标已审核通过，请及时提交！\n";
                }
                else if (protocol == Protocol.ClientActualTimeProtocol.FUS)
                {
                    returnMsg += "您的流程已新建成功，请及时提交！\n";
                }
            }
            return returnMsg;
        }
        #endregion

        #region 解析下班前请求的消息
        /// <summary>
        /// 解析下班前请求的消息
        /// </summary>
        /// <param name="msg">消息包</param>
        /// <returns></returns>
        public static string AnalysisBeforeOLMsg(string[] msgArray)
        {
            var showMsg = string.Empty;
            if (msgArray.Length < 3)
            {
                if (msgArray[1] == Protocol.ClientBeforeOLProtocol.PNB)
                {
                    showMsg = "赶紧梳理下今天完成的工作，提交计划吧！\n";
                }
                else if (msgArray[1] == Protocol.ClientBeforeOLProtocol.TUE)
                {
                    showMsg = "您今天的工时不足8小时，要加油喽！\n";
                }
                else if (msgArray[1] == Protocol.ClientBeforeOLProtocol.THE)
                {
                    showMsg = "您非常努力，已经超过了90%的同志，请继续保持！\n";
                }
                else if (msgArray[1] == Protocol.ClientBeforeOLProtocol.TOE)
                {
                    showMsg = "您今天已超负荷完成工作，辛苦了！\n";
                }
            }
            else
            {
                var operateArray = msgArray[2].Split('*');
                if (operateArray.Length > 0)
                {
                    showMsg = "今天还有以下计划还未完成，要加班喽：\n";
                    for (int i = 0; i < operateArray.Length; i++)
                    {
                        showMsg += (i + 1) + "、" + operateArray[i] + "\n";
                    }
                }
            }
            return showMsg;

        }
        #endregion

        #region 解析随机信息
        /// <summary>
        /// 解析随机信息
        /// </summary>
        /// <param name="msg">消息包</param>
        /// <returns></returns>
        public static string AnalysisRandomMsg(string msg)
        {
            var showMsg = string.Empty;
            if (msg == Protocol.ClientRandomProtocol.TYN)
            {
                showMsg = "您昨天的工时不足，今天恐怕要加油喽!";
            }
            else if (msg == Protocol.ClientRandomProtocol.TYZ)
            {
                showMsg = "您昨天没有工作提交，领导要担心了哦!";
            }
            else if (msg == Protocol.ClientRandomProtocol.TWN)
            {
                showMsg = "您这周的工时不足，要抓紧赶超了哦!";
            }
            else if (msg == Protocol.ClientRandomProtocol.TWZ)
            {
                showMsg = "真不敢相信您一周都没有工时，领导要批评了哦!";
            }
            else if (msg == Protocol.ClientRandomProtocol.PUC)
            {
                showMsg = "您的下属们正在抱怨了，赶紧审批下他们的计划吧!";
            }
            else if (msg == Protocol.ClientRandomProtocol.PUA)
            {
                showMsg = "您的团队成员已经出色地完成任务，快来审查吧!";
            }
            return showMsg;
        }
        #endregion
    }
}
