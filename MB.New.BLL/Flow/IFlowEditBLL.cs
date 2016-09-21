using MB.DAL;
using MB.New.Common;
using MB.New.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MB.New.BLL
{
   public interface IFlowEditBLL
    {
        /// <summary>
        /// 取得节点字段信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name=""></param>
        /// <param name="templateId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        List<NodeFieldEditModel> GetNodeFieldList( TargetNavigationDBEntities db,  int templateId,int? nodeId);

  
    }
}
