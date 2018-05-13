using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpSVN
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Logs.AddLog("-----------------开始运行");

                var con = new ConfigManager();
                var s = new SVNHelper();
                var e = new ExcelHelper();

                string path = con.GetAppSetting("xlsPath");
                if (string.IsNullOrWhiteSpace(path))
                {
                    Logs.AddLog("xlsPath 报表路径为空");
                    return;
                }
                string user = con.GetAppSetting("svnUser");
                if (string.IsNullOrWhiteSpace(user))
                {
                    Logs.AddLog("svnUser svn登录账号为空");
                    return;
                }
                string pass = con.GetAppSetting("svnPass");
                if (string.IsNullOrWhiteSpace(pass))
                {
                    Logs.AddLog("svnPass svn登录密码为空");
                    return;
                }
                //更新svn
                s.UpdateSvn(path);

                //更新excel
                if (e.ReadFromExcelFile(path))
                {
                    //上传excel
                    s.CommitSvn(path, user, pass);
                }
            }
            catch (Exception e)
            {
                Logs.WriteLog(e);
            }
        }
    }
}
