using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpSVN
{

    public class SVNHelper
    {

        /* 
         * 混合模式程序集是针对“v2.0.50727”版的运行时生成的
         * 在app.config的configuration节点下添加:   <startup useLegacyV2RuntimeActivationPolicy="true"> 
         */
        readonly SvnClient _client = new SvnClient();

        /// <summary>
        ///  更新SVN
        /// </summary>
        /// <param name="path"></param>
        public void UpdateSvn(string path)
        {
            Console.WriteLine("更新SVN-------");
            _client.Update(path);


        }


        /// <summary>
        /// 加入版本控制 Add
        /// </summary>
        public void AddSvn()
        {
            using (SvnClient client = new SvnClient())
            {
                string path = "";//  
                SvnAddArgs args = new SvnAddArgs();
                args.Depth = SvnDepth.Empty;
                client.Add(path, args);
            }
        }

        /// <summary>
        ///  清理SVN
        /// </summary>
        /// <param name="path"></param>
        public void CleanSvn(string path)
        {
            _client.CleanUp(path);
        }

        /// <summary>
        /// 提交SVN
        /// </summary>
        /// <param name="path"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        public void CommitSvn(string path, string UserName, string Password)
        {
            //Authentication 身份验证
            _client.Authentication.Clear();
            _client.Authentication.UserNamePasswordHandlers
                += delegate (object obj, SharpSvn.Security.SvnUserNamePasswordEventArgs args)
                {
                    args.UserName = UserName;
                    args.Password = Password;
                };

            //_client.Authentication.SslServerTrustHandlers +=
            //delegate(object sender, SvnSslServerTrustEventArgs e)
            //{
            //    e.AcceptedFailures = e.Failures;
            //    e.Save = true; //证书 Save acceptance to authentication store
            //};

            var ca = new SvnCommitArgs { LogMessage = "提交信息" };
            //"svn log message created at " + DateTime.Now.ToString();
            bool action = _client.Commit(path, ca);
            if (action)
            {
                Console.WriteLine(" OK! 提交SVN成功> " + path);
            }
            else
            {
                Console.WriteLine(" ERR!!! 提交SVN失败> " + path);
            }

        }



        //处理有问题的文件
        public void QuestionFile(string path)
        {
            // string path = "D:\\Test";
            //SvnClient client = new SvnClient();
            SvnStatusArgs sa = new SvnStatusArgs();
            Collection<SvnStatusEventArgs> status;
            _client.GetStatus(path, sa, out status);
            foreach (var item in status)
            {
                string fPath = item.FullPath;
                if (item.LocalContentStatus != item.RemoteContentStatus)
                {
                    //被修改了的文件
                }
                if (!item.Versioned)
                {
                    //新增文件
                    _client.Add(fPath);
                }
                else if (item.Conflicted)
                {
                    //将冲突的文件用工作文件解决冲突
                    _client.Resolve(fPath, SvnAccept.Working);
                }
                else if (item.IsRemoteUpdated)
                {
                    //更新来自远程的新文件
                    _client.Update(fPath);
                }
                else if (item.LocalContentStatus == SvnStatus.Missing)
                {
                    //删除丢失的文件
                    _client.Delete(fPath);
                }
            }
        }

        //获取SVN上最新50条的提交日志信息
        public string GetSvnLog(string path)
        {
            //string path = "D:\\Test";
            //SvnClient client = new SvnClient();

            var logArgs = new SvnLogArgs { RetrieveAllProperties = false };
            //不检索所有属性
            Collection<SvnLogEventArgs> status;
            _client.GetLog(path, logArgs, out status);
            int messgNum = 0;
            string logText = "";
            string lastLog = "";
            foreach (var item in status)
            {
                if (messgNum > 50)
                    break;
                messgNum += 1;
                if (string.IsNullOrEmpty(item.LogMessage) || item.LogMessage == " " || lastLog == item.LogMessage)
                {
                    continue;
                }
                logText = item.Author + "：" + item.LogMessage + "\n" + logText;
                lastLog = item.LogMessage;
            }
            return logText;
        } 

    }

 
}
