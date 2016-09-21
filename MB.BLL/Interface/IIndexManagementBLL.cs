using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IIndexManagementBLL
    {
        /// 获取首页模块列表
        List<IndexModuleInfoModel> GetModuleList();

        /// 模块详情取得
        IndexModuleInfoModel GetModuleInfo(int moduleId);

        /// 新闻来源取得
        List<IndexTargetModel> GetIndexNews(int moduleId);

        /// 通知来源取得
        List<IndexTargetModel> GetIndexNotice(int moduleId);

        /// 文档来源取得
        List<IndexTargetModel> GetIndexDocument(int moduleId);

        /// 首页图像取得
        List<ImageModel> GetIndexImage(int moduleId);

        /// 统计对象取得
        List<IndexTargetModel> GetIndexStatistics(int moduleId);

        /// 添加模块
        void AddIndexModuleInfo(IndexModuleInfoModel moduleInfo, int userId);

        /// 更新模块
        void UpdateIndexModuleInfo(IndexModuleInfoModel moduleInfo, int userId);

        /// 删除首页模块
        void DeleteModule(int moduleId);

        /// 更新木块大小
        void UpdateModuleSize(IndexModuleModel moduleInfo, int userId);

        /// 删除首页图片
        void DeleteIndexImage(int moduleId, int imageId);
    }
}