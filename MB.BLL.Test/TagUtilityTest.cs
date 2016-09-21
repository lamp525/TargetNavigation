using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MB.Model;
using MB.Common;

namespace MB.BLL.Test
{
    [TestClass]
    public class TagUtilityTest
    {

        //[TestMethod]
        //public void SaveSytemTag()
        //{
        //    string[] tagNames = new string[] { "白痴", "傻瓜", "六二" };

        //    foreach (var tagName in tagNames)
        //    {
        //        var result = TagUtility.SaveSystemTag(tagName);
        //    }

        //    //Assert.IsTrue(result);         
        //}

        [TestMethod]
        public void SaveTagPlan()
        {
            string[] tagNames = new string[] { "白痴", "傻瓜", "六二" };
            //TagUtility.SaveTagPlan(1, tagNames);
        }

        [TestMethod]
        public void GetTopTag()
        {
            var userId = 110;
            var num = 2;
            var result = TagUtility.GetTopTag(userId, num);
        }

        [TestMethod]
        public void TagSearchResult()
        {
            string[] tagNames = new string[] { "白痴", "傻瓜", "六二" };

            var searchResult = TagUtility.GetTagSearchResult(tagNames, ConstVar.TagType.Plan);
        }


        [TestMethod]
        public void ImportMostUsedTag()
        {        
            //var userId = 110;
            var tagNames = new string[] { "标签1", "标签2", "标签3", "标签4", "标签5", "标签6", "标签7", "标签8", "标签9","标签10","标签11"};
          //TagUtility.AddUserTagForTest(userId, tagNames);
        }

        [TestMethod]
        public void ImportRecentSearchTag()
        {
            var userId = 110;
            var tagNames = new string[] { "检索1", "检索2", "检索3", "检索4", "检索5", "检索6", "检索7", "检索8", "标签9", "检索10", "检索11" };
            TagUtility.SaveRecentSearchTag(userId, tagNames);
        }
    }
}
