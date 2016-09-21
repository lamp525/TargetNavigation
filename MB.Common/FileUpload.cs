using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using MB.DAL;
using MB.Model;

namespace MB.Common
{
    public class FileUpload
    {
        #region 上传单个文件

        //接收上传

        //public List<AjaxUploadFileResult> UploadFile(HttpRequestBase Request)
        //{
        //    List<AjaxUploadFileResult> results = new List<AjaxUploadFileResult>();
        //    foreach (string file in Request.Files)
        //    {
        //        HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
        //        if (hpf.ContentLength == 0 || hpf == null)
        //        {
        //            continue;
        //        }

        //        var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
        //        string configString = ConfigurationManager.AppSettings["PlanUpLoadPath"].ToString();
        //        string pathForSaving = configString;
        //        //string pathForSaving = Server.MapPath(configString);
        //        if (this.CreateFolderIfNeeded(pathForSaving))
        //        {
        //            hpf.SaveAs(Path.Combine(pathForSaving, fileName));
        //            results.Add(new AjaxUploadFileResult()
        //            {
        //                //FilePath = String.Format(configString + "/{0}", fileName),
        //                //FileName = fileName,
        //                //IsValid = true,
        //                //Length = hpf.ContentLength,
        //                //Message = "上传成功",
        //                //Type = hpf.ContentType
        //            });
        //        }
        //    }
        //    return results;
        //}

        #endregion 上传单个文件

        #region 上传头像文件

        public AjaxUploadFileResult UploadHeadImg(HttpPostedFileBase hpf, UploadFilePath UplaodNum, int CreatUser, TargetNavigationDBEntities db)
        {
            AjaxUploadFileResult AUFR = new AjaxUploadFileResult();
            //Random rd = new Random();
            //int numName = rd.Next(1000, 9999);
            //var fileName = Guid.NewGuid().ToString() + hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));//使用GUID生成随机名
            //string pathForSaving = Server.MapPath(configString);//绝对路径
            //string displayName = hpf.FileName;
            string saveName = CreatUser + "Test";
            string extension = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
            var fileName = saveName + extension;//带后缀名上传
            //var fileName = saveName;//不带后缀名
            string pathForSaving = AppDomain.CurrentDomain.BaseDirectory + ConfigPath((int)UplaodNum);
            if (this.CreateFolderIfNeeded(pathForSaving))
            {
                hpf.SaveAs(Path.Combine(pathForSaving, fileName));
                var ajaxUploadFileResult = new AjaxUploadFileResult
                {
                    //releId = ReleId,
                    createUser = CreatUser,
                    //displayName = displayName,
                    saveName = saveName,
                    //extension = extension
                };
                AUFR = ajaxUploadFileResult;
            }
            return AUFR;
        }

        #endregion

        #region 上传多个文件

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="Request">HttpRequestBase 对象</param>
        /// <param name="UplaodNum">从枚举UpLoadFilePath中选取值</param>
        /// <param name="ReleId">关联ID</param>
        /// <param name="CreatUser">用户ID</param>
        /// <returns></returns>
        public AjaxUploadFileResult UploadMultipleFiles(HttpPostedFileBase hpf, object UplaodNum, int ReleId, int CreatUser, TargetNavigationDBEntities db)
        {
            AjaxUploadFileResult AUFR = new AjaxUploadFileResult();
            Random rd = new Random();
            int numName = rd.Next(1000, 9999);
            //var fileName = Guid.NewGuid().ToString() + hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));//使用GUID生成随机名
            //string pathForSaving = Server.MapPath(configString);//绝对路径
            string displayName = hpf.FileName;
            string saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
            string extension = hpf.FileName.Substring(hpf.FileName.LastIndexOf('.'));
            //var fileName = saveName + extension;//带后缀名上传
            var fileName = saveName;//不带后缀名
            string pathForSaving = ConfigPath(Convert.ToInt32(UplaodNum));
            if (this.CreateFolderIfNeeded(pathForSaving))
            {
                hpf.SaveAs(Path.Combine(pathForSaving, fileName));

                AUFR.releId = ReleId;
                AUFR.createUser = CreatUser;
                AUFR.displayName = displayName;
                AUFR.saveName = saveName;
                AUFR.extension = extension;
                //文件是否可预览标志
                AUFR.isPreviewable = FilePreview.IsPreviewable(extension);

            }
            return AUFR;
        }

        #endregion

        #region 共用方法

        /// <summary>
        /// 检查是否要创建上传文件夹，如果没有就创建
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    //处理异常
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 读取web.config中的上传地址  如果num不在枚举里面 则默认路径为计划路径
        /// </summary>
        /// <param name="num">路径编号</param>
        /// <returns></returns>
        public string ConfigPath(int num/*路径编号对应UpLoadFilePath中的枚举*/)
        {
            var configString = string.Empty;
            switch (num)
            {
                case 1:
                    configString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["NewsUpLoadPath"].ToString()).Replace("\\NewsManagement", null);
                    break;

                case 2:
                    configString = ConfigurationManager.AppSettings["DocumentUpLoadPath"].ToString();
                    break;

                case 3:
                    configString = ConfigurationManager.AppSettings["PlanUpLoadPath"].ToString();
                    break;

                case 4:
                    configString = ConfigurationManager.AppSettings["MineUpLoadPath"].ToString();
                    break;

                case 5:
                    configString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["HeadImageUpLoadPath"].ToString()).Replace("\\IMMessage", null);
                    break;

                case 6:
                    configString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["IMUploadPath"].ToString()).Replace("\\IMMessage", null);
                    break;

                case 7:
                    configString = ConfigurationManager.AppSettings["ObjectiveUploadPath"].ToString();
                    break;

                case 8:
                    configString = ConfigurationManager.AppSettings["FlowIndexUploadPath"].ToString();
                    break;

                case 9:
                    configString = ConfigurationManager.AppSettings["MeetingUpLoadPath"].ToString();
                    break;

                case 10:
                    configString = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ConvertFilePath"].ToString()).Replace("\\Shared", null);
                    break;

                default: break;
            }
            return configString;
        }

        #endregion

        #region  附件表数据插入 和 删除

        /// <summary>
        /// 向计划附件表中插入一条数据
        /// </summary>
        /// <param name="ajaxUploadFileResult">AjaxUploadFileResult</param>
        /// <returns>true/false</returns>
        public tblPlanAttachment InsertPlanAttachment(AjaxUploadFileResult ajaxUploadFileResult, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblPlanAttachment", obj).FirstOrDefault().Value;
            var Attachment = new DAL.tblPlanAttachment
            {
                attachmentId = operId,
                planId = ajaxUploadFileResult.releId,
                displayName = ajaxUploadFileResult.displayName,
                createUser = ajaxUploadFileResult.createUser,
                updateUser = ajaxUploadFileResult.createUser,
                deleteFlag = false,
                createTime = DateTime.Now,
                updateTime = DateTime.Now,
                extension = ajaxUploadFileResult.extension,
                saveName = ajaxUploadFileResult.saveName,
                isPreviewable = ajaxUploadFileResult.isPreviewable
            };
            db.tblPlanAttachment.Add(Attachment);
            return Attachment;
        }

        /// <summary>
        /// 根据附件表ID 真实删除服务器上的附件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="uplaodNum">附件绝对路径，从枚举UpLoadFilePath中选取值</param>
        /// <param name="db">TargetNavigationDBEntities</param>
        public void DeletePlanAttachmentById(int attachmentId, object uplaodNum, TargetNavigationDBEntities db)
        {
            string pathForSaving = ConfigPath(Convert.ToInt32(uplaodNum));
            tblPlanAttachment planAttachment = new tblPlanAttachment();
            planAttachment = db.tblPlanAttachment.Where(a => a.attachmentId == attachmentId).FirstOrDefault();
            //string name = planAttachment.saveName+planAttachment.extension;//有后缀名删除
            string name = planAttachment.saveName;//无后缀名删除
            if (!string.IsNullOrEmpty(name))
            {
                System.IO.File.Delete(Path.Combine(pathForSaving, name));
            }
            db.tblPlanAttachment.Remove(planAttachment);
        }

        /// <summary>
        ///  真实删除服务器上循环计划的附件
        /// </summary>
        /// <param name="attachmentId">附件ID</param>
        /// <param name="uplaodNum">附件绝对路径，从枚举UpLoadFilePath中选取值</param>
        /// <param name="db">TargetNavigationDBEntities</param>
        public void DeleteLoopPlanAttachmentById(int attachmentId, object uplaodNum, TargetNavigationDBEntities db)
        {
            var pathForSaving = ConfigPath(Convert.ToInt32(uplaodNum));
            var planAttachment = db.tblLoopplanAttachment.Where(a => a.attachmentId == attachmentId).FirstOrDefault();
            //string name = planAttachment.saveName+planAttachment.extension;//有后缀名删除
            string name = planAttachment.saveName;//无后缀名删除
            if (!string.IsNullOrEmpty(name))
            {
                System.IO.File.Delete(Path.Combine(pathForSaving, name));
            }
            db.tblLoopplanAttachment.Remove(planAttachment);
        }

        /// <summary>
        /// 向意见附件表中插入一条数据
        /// </summary>
        /// <param name="ajaxUploadFileResult">AjaxUploadFileResult</param>
        /// <returns>true/false</returns>
        public void InsertSuggestionAttachment(AjaxUploadFileResult ajaxUploadFileResult, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblSuggestionAttachment", obj).FirstOrDefault().Value;
            var Attachment = new DAL.tblSuggestionAttachment
            {
                attachmentId = operId,
                suggestionId = ajaxUploadFileResult.releId,
                displayName = ajaxUploadFileResult.displayName,
                createUser = ajaxUploadFileResult.createUser,
                updateUser = ajaxUploadFileResult.createUser,
                deleteFlag = false,
                createTime = DateTime.Now,
                updateTime = DateTime.Now,
                extension = ajaxUploadFileResult.extension,
                saveName = ajaxUploadFileResult.saveName
            };
            db.tblSuggestionAttachment.Add(Attachment);
        }

        /// <summary>
        /// 根据评论附件表ID 真实删除服务器上的附件
        /// </summary>
        /// <param name="attachmentId">评论附件ID</param>
        /// <param name="uplaodNum">附件绝对路径，从枚举UpLoadFilePath中选取值</param>
        /// <param name="db">TargetNavigationDBEntities</param>
        public void DeleteSuggestionAttachmentById(int attachmentId, object uplaodNum, TargetNavigationDBEntities db)
        {
            string pathForSaving = ConfigPath(Convert.ToInt32(uplaodNum));
            tblSuggestionAttachment suggestionAttachment = new tblSuggestionAttachment();
            suggestionAttachment = db.tblSuggestionAttachment.Where(a => a.attachmentId == attachmentId).FirstOrDefault();
            string name = suggestionAttachment.saveName + suggestionAttachment.extension;
            if (!string.IsNullOrEmpty(name))
            {
                System.IO.File.Delete(Path.Combine(pathForSaving, name));
            }
            db.tblSuggestionAttachment.Remove(suggestionAttachment);
        }

        /// <summary>
        /// 向循环计划附件表中插入一条数据
        /// </summary>
        /// <param name="ajaxUploadFileResult">AjaxUploadFileResult</param>
        /// <returns>true/false</returns>
        public void InsertLoopplanAttachment(AjaxUploadFileResult ajaxUploadFileResult, TargetNavigationDBEntities db)
        {
            System.Data.Entity.Core.Objects.ObjectParameter obj = new System.Data.Entity.Core.Objects.ObjectParameter("num", -1);
            var operId = db.prcGetPrimaryKey("tblLoopplanAttachment", obj).FirstOrDefault().Value;
            var Attachment = new DAL.tblLoopplanAttachment
            {
                attachmentId = operId,
                loopId = ajaxUploadFileResult.releId,
                displayName = ajaxUploadFileResult.displayName,
                createUser = ajaxUploadFileResult.createUser,
                updateUser = ajaxUploadFileResult.createUser,
                deleteFlag = false,
                createTime = DateTime.Now,
                updateTime = DateTime.Now,
                extension = ajaxUploadFileResult.extension,
                saveName = ajaxUploadFileResult.saveName
            };
            db.tblLoopplanAttachment.Add(Attachment);
        }

        /// <summary>
        /// 根据循环附件表ID 真实删除服务器上的附件
        /// </summary>
        /// <param name="attachmentId">循环附件ID</param>
        /// <param name="uplaodNum">附件绝对路径，从枚举UpLoadFilePath中选取值</param>
        /// <param name="db">TargetNavigationDBEntities</param>
        public void DeleteLoopplanAttachmentById(int attachmentId, object uplaodNum, TargetNavigationDBEntities db)
        {
            string pathForSaving = ConfigPath(Convert.ToInt32(uplaodNum));
            tblLoopplanAttachment loopplanAttachment = new tblLoopplanAttachment();
            loopplanAttachment = db.tblLoopplanAttachment.Where(a => a.attachmentId == attachmentId).FirstOrDefault();
            string name = loopplanAttachment.saveName + loopplanAttachment.extension;
            if (!string.IsNullOrEmpty(name))
            {
                System.IO.File.Delete(Path.Combine(pathForSaving, name));
            }
            db.tblLoopplanAttachment.Remove(loopplanAttachment);
        }

        #endregion

        #region 上传文件

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="hpf">文件</param>
        /// <param name="uploadType">
        /// 1：新闻图片上传
        /// 2：公司文档上传
        /// 3：计划附件上传
        /// 4：个人文档上传
        /// 5：用户头像上传
        /// 7:目标文档上传
        /// </param>
        /// <returns></returns>
        public UploadFileModel UploadFile(HttpPostedFileBase hpf, int uploadType)
        {
            var fileModel = new UploadFileModel();

            if (hpf.ContentLength == 0 || hpf == null)
            {
                return null;
            }
            int numName = StringUtils.GetRandom();
            string displayName = hpf.FileName;
            string saveName = DateTime.Now.ToString("yyyyMMddhhmmss") + numName.ToString();
            string extension = System.IO.Path.GetExtension(displayName).Substring(1).ToLower();
            string pathForSaving = this.ConfigPath(uploadType);

            if (CreateFolderIfNeeded(pathForSaving))
            {
                if (uploadType == 1 || uploadType == 5 || uploadType == 6)
                {
                    hpf.SaveAs(Path.Combine(pathForSaving, saveName + "." + extension));
                }
                else
                {
                    hpf.SaveAs(Path.Combine(pathForSaving, saveName));
                }
                fileModel.saveName = saveName;
                fileModel.displayName = displayName;
                fileModel.extension = extension;
            }

            return fileModel;
        }


        #endregion
    }
}