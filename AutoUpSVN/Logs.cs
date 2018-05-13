using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpSVN
{
    public class Logs
    {
        const  string ConfigPATH = @"D:";// Environment.CurrentDirectory;

        /// <summary>
        /// 将异常打印到LOG文件
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="LogAddress">日志文件地址</param>
        public static void LogMsg(string msg, string LogAddress = "")
        {
            Console.Write(msg); 
            string path = ConfigPATH + "\\log";
            if (!Directory.Exists(path))//判断是否有该文件              
                Directory.CreateDirectory(path);
            string logFileName = path + "\\Analysis.log";//生成日志文件  
            if (!File.Exists(logFileName))//判断日志文件是否为当天  
                File.Create(logFileName).Close();//创建文件  
            //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
            if (LogAddress == "")
            {
                //Environment.CurrentDirectory + 
                LogAddress = //"E:\\tempCode\\Logs\\" 
                    ConfigPATH +
                    DateTime.Now.Year + '-' +
                    DateTime.Now.Month + '-' +
                    DateTime.Now.Day + "_Log.log";
            }
            //把异常信息输出到文件
            StreamWriter fs = new StreamWriter(LogAddress, true);
            fs.WriteLine("当前时间：" + DateTime.Now.ToString());
            fs.WriteLine("信息：" + msg);
            fs.WriteLine();
            fs.Close();
        }
        public static void AddLog(string msg)
        {
            try
            {
                Console.Write(msg);
                //string ConfigPATH = Environment.CurrentDirectory;
                string path = ConfigPATH + "\\log";
                if (!Directory.Exists(path))//判断是否有该文件              
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\Analysis.log";//生成日志文件  
                if (!File.Exists(logFileName))//判断日志文件是否为当天  
                    File.Create(logFileName).Close();//创建文件  

                StreamWriter writer = File.AppendText(logFileName);//文件中添加文件流  
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);
                writer.Flush();
                writer.Close();
                Console.Write(logFileName);
            }
            catch (Exception e)
            {
                string path = Path.Combine("./log");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string logFileName = path + "\\Analysis" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                if (!File.Exists(logFileName))
                    File.Create(logFileName);
                StreamWriter writer = File.AppendText(logFileName);
                writer.WriteLine("");
                writer.WriteLine(DateTime.Now.ToString("日志记录错误HH:mm:ss") + " " + e.Message + " " + msg);
                writer.Flush();
                writer.Close();
            }
        }
        /// <summary>
        /// 将异常打印到LOG文件
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="LogAddress">日志文件地址</param>
        public static void WriteLog(Exception ex)
        {
            Console.WriteLine(ex.Message);

            string path = ConfigPATH + "\\log";
            if (!Directory.Exists(path))//判断是否有该文件              
                Directory.CreateDirectory(path);
            string logFileName = path + "\\Analysis.log";//生成日志文件  
            if (!File.Exists(logFileName))//判断日志文件是否为当天  
                File.Create(logFileName).Close();//创建文件  

            //把异常信息输出到文件
            StreamWriter fs = File.AppendText(logFileName);//文件中添加文件流  
            fs.WriteLine("当前时间：" + DateTime.Now.ToString());
            fs.WriteLine("异常信息：" + ex.Message);
            fs.WriteLine("异常对象：" + ex.Source);
            fs.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
            fs.WriteLine("触发方法：" + ex.TargetSite);
            fs.WriteLine();
            fs.Close();
            Console.Write(logFileName);
        }
    }
}
