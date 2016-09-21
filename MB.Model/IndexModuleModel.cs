using System.Collections.Generic;

namespace MB.Model
{
    public class IndexModuleModel
    {
        /// <summary>模块ID</summary>
        public int? moduleId { get; set; }

        /// <summary>标题</summary>
        public string title { get; set; }

        /// <summary>表示标题</summary>
        public bool? displayTitle { get; set; }

        /// <summary>链接方式</summary>
        public bool? linkTarget { get; set; }

        /// <summary>显示条数</summary>
        public int? maxRow { get; set; }

        /// <summary>宽度</summary>
        public int? width { get; set; }

        /// <summary>长度</summary>
        public int? height { get; set; }

        /// <summary>表示类型</summary>
        public int type { get; set; }

        /// <summary>位置</summary>
        public string position { get; set; }

        /// <summary>默认显示</summary>
        public int? defaultEfficiency { get; set; }

        /// <summary>TOP</summary>
        public int? topDisplay { get; set; }

        /// <summary>显示曲线</summary>
        public int? topDisplayLine { get; set; }

        /// <summary>默认显示曲线</summary>
        public int? defaultLine { get; set; }

        /// <summary>功效价值</summary>
        public bool? efficiencyValue { get; set; }

        /// <summary>执行力</summary>
        public bool? executiveForce { get; set; }

        /// <summary>目标考核激励</summary>
        public bool? objective { get; set; }
    }

    public class IndexModuleInfoModel
    {
        /// <summary>首页模块</summary>
        public IndexModuleModel module { get; set; }

        /// <summary>首页目标（新闻来源、通知来源、文档来源、统计对象）</summary>
        public List<IndexTargetModel> target { get; set; }

        /// <summary>首页图像</summary>
        public List<ImageModel> image { get; set; }
    }
}