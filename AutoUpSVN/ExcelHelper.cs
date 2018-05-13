using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpSVN
{

    public class ExcelHelper
    {
        /// <summary>
        /// 更新excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool ReadFromExcelFile(string filePath)
        {
            bool b = false;
            int startRow = 3;
            int startCell = 6;
            int endCell = 3;
            IWorkbook wk = null; 

            string extension = System.IO.Path.GetExtension(filePath);
            try
            {

                using (var fs = File.OpenRead(filePath))
                {
                    //FileStream fs = File.Open(filePath,FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    //FileStream fs = File.OpenRead(filePath);  OpenWrite
                    if (extension != null && extension.Equals(".xls"))
                    {
                        //把xls文件中的数据写入wk中
                        wk = new HSSFWorkbook(fs);
                    }
                    else
                    {
                        //把xlsx文件中的数据写入wk中
                        wk = new XSSFWorkbook(fs);
                    }
                    fs.Close();

                    //读取当前表数据
                    ISheet sheet = wk.GetSheetAt(0);
                    sheet.ForceFormulaRecalculation = true;
                    for (int i = startRow; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i); //读取当前行数据
                        if (row != null)
                        {
                            //项目
                            if (row.GetCell(0) == null || string.IsNullOrWhiteSpace(row.GetCell(0).ToString()) || row.GetCell(0).ToString().ToUpper().Contains("PM") == false)
                                continue;
                            Console.Write(i + "行:");

                            var gs = row.GetCell(14);
                            if (gs == null)
                            {
                                Console.Write(" O列的公式 =ROUND(100/项目总天数,0) 没有值,报表格式异常跳过 ");
                                return b;
                            }
                            if (gs.NumericCellValue == 0)
                            {
                                Console.Write(gs.NumericCellValue + " O列的公式没有值,报表格式异常跳过 ");
                                return b;
                            }
                            //工序计划交期
                            DateTime overdt = DateTime.MaxValue;
                            string overDT = row.GetCell(5).ToString();
                            if (string.IsNullOrWhiteSpace(overDT))
                            {
                                Console.Write("工序计划交期 没有值,报表格式异常跳过 ");
                                return b;
                            }
                            else
                            {
                                overdt = Convert.ToDateTime(overDT);
                            }
                            int len = startCell + endCell;
                            //LastCellNum 是当前行的总列数row.LastCellNum
                            for (int j = startCell; j < len; j++)
                            {
                                Console.Write(j + "列=  ");
                                //读取该行的第j列数据
                                if (row.GetCell(j + 1) != null && row.GetCell(j + 1).NumericCellValue > 0)
                                {
                                    double value = row.GetCell(j).NumericCellValue;
                                    Console.Write(value + "改为");

                                    double d = 0;
                                    if (j == 8)
                                    {
                                        //=IF((I5*100-O5)/100<0,0,((I5*100-O5)/100))
                                        d = (row.GetCell(j).NumericCellValue * 100 - gs.NumericCellValue) / 100; 
                                    }
                                    else
                                    {
                                        d = row.GetCell(j + 1).NumericCellValue;
                                    }
                                    Console.Write(d + " |   ");


                                    row.GetCell(j).SetCellValue(d > 0 ? d : 0);
                                }
                                else
                                {
                                    row.GetCell(j).SetCellValue(0);
                                    Console.Write("0 |   ");
                                }

                            }

                            if (row.GetCell(6).NumericCellValue <= 0)
                            {
                                if (overdt < DateTime.Now)
                                    row.GetCell(10).SetCellValue("测试中");
                                //else
                                //    row.GetCell(10).SetCellValue("已完成");
                            }
                            Console.WriteLine("\n");
                        }
                    }

                    b = true;
                    //模板自动公式需要激活
                    sheet.ForceFormulaRecalculation = true;
 
                    try
                    {
                        //转为字节数组    
                        var stream = new MemoryStream();
                        wk.Write(stream);
                        var buf = stream.ToArray();
                        //保存为Excel文件    
                        using (FileStream fs5 = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            fs5.Write(buf, 0, buf.Length);
                            fs5.Flush();
                        }
                        Console.Write("保存OK=" + filePath);
                    }
                    catch (Exception e)
                    {
                        b = false;
                        Logs.WriteLog(e);
                    }
                }
            }
            catch (Exception e)
            {
                b = false;
                Logs.WriteLog(e);
            }
            return b;
        }



    }
}
