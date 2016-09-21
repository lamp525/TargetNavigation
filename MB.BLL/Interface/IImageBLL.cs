using System.Collections.Generic;

using MB.Model;

namespace MB.BLL
{
    public interface IImageBLL
    {
        List<ImageManageModel> GetImageList();

        bool AddImage(List<ImageManageModel> addlist);
    }
}