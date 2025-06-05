using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Serilog;
using uploadyahua.Model;

namespace uploadyahua.Util
{
    /// <summary>
    /// 报表工具类，用于生成PDF报表和保存检验结果数据
    /// </summary>
    public class ReportUtil
    {
        #region PDF相关字段
        /// <summary>
        /// PDF文档对象
        /// </summary>
        PdfDocument Document;

        /// <summary>
        /// PDF页面对象
        /// </summary>
        PdfPage Page;

        /// <summary>
        /// PDF图形对象
        /// </summary>
        XGraphics Gfx;

        /// <summary>
        /// 主题颜色
        /// </summary>
        XColor ColorTheme;

        /// <summary>
        /// 灰色
        /// </summary>
        XColor ColorGray;

        /// <summary>
        /// 黑色
        /// </summary>
        XColor ColorBlack;

        /// <summary>
        /// PDF字体选项
        /// </summary>
        XPdfFontOptions options;

        /// <summary>
        /// 当前X坐标
        /// </summary>
        int Cur_x;

        /// <summary>
        /// 当前Y坐标
        /// </summary>
        int Cur_y;

        /// <summary>
        /// 最大Y坐标
        /// </summary>
        double Max_y;

        /// <summary>
        /// 垂直边距
        /// </summary>
        int MarginVer = 20;

        /// <summary>
        /// 水平边距
        /// </summary>
        int MarginHor = 20;

        /// <summary>
        /// 宽度
        /// </summary>
        double width;

        /// <summary>
        /// 高度
        /// </summary>
        double height;

        /// <summary>
        /// 普通字体名称
        /// </summary>
        string fontName = "YaHei.Consolas.1.12";

        /// <summary>
        /// 粗体字体名称
        /// </summary>
        string fontNameBold = "STSONG_Bold";
        #endregion

        #region 数据相关字段
        /// <summary>
        /// 检验结果列表
        /// </summary>
        List<TestResult> testResults;

        /// <summary>
        /// 项目映射列表
        /// </summary>
        List<Result> projectMaps;

        /// <summary>
        /// Logo路径
        /// </summary>
        string logoPath = "";

        /// <summary>
        /// 当前检验结果对象
        /// </summary>
        TestResult tr;

        #region 检验结果字段
        /// <summary>
        /// 样本编号
        /// </summary>
        string SampleNum;

        /// <summary>
        /// 样本类型
        /// </summary>
        string SampleType;

        /// <summary>
        /// 患者姓名
        /// </summary>
        string PatientName;

        /// <summary>
        /// 病历号
        /// </summary>
        string PatientNum;

        /// <summary>
        /// 年龄
        /// </summary>
        string Age;

        /// <summary>
        /// 性别
        /// </summary>
        string Gender;

        /// <summary>
        /// 出生日期
        /// </summary>
        string Birthday;

        /// <summary>
        /// 住院号
        /// </summary>
        string AdmissionNum;

        /// <summary>
        /// 床号
        /// </summary>
        string BedNum;

        /// <summary>
        /// 科室
        /// </summary>
        string Office;

        /// <summary>
        /// 医生姓名
        /// </summary>
        string DoctorName;

        /// <summary>
        /// 检验者
        /// </summary>
        string Proofer;

        /// <summary>
        /// 审核者
        /// </summary>
        string Auditor;

        /// <summary>
        /// 医院名字
        /// </summary>
        string Hospital;

        /// <summary>
        /// 就诊类型
        /// </summary>
        string VisitType;

        /// <summary>
        /// 送检日期
        /// </summary>
        string SubmissionDate;

        /// <summary>
        /// 检测日期
        /// </summary>
        string TestDate;
        /// <summary>
        /// 备注
        /// </summary>
        string Remark;
        /// <summary>
        /// 标题
        /// </summary>
        string Title;

        /// <summary>
        /// 提示
        /// </summary>
        string Hint;

        #endregion
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReportUtil() { }

        public string Print(TestResult tr, string selectedPrinter)
        {
            string path = Create(tr);
            if (string.IsNullOrEmpty(path))
            {
                //MessageBox.Show("打印失败 " + path);
            }
            else
            {
                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                doc.LoadFromFile(path);
                doc.PrintSettings.PrinterName = selectedPrinter;
                doc.Print();
            }
            return path;
        }

        /// <summary>
        /// 创建报表
        /// </summary>
        /// <param name="tr">检验结果对象</param>
        public string Create(TestResult tr)
        {
            InitData(tr);

            try
            {
                //1. 定义文档对象
                Document = new PdfDocument();
                NewPage();
                width = Page.Width.Point;
                height = Page.Height.Point;
                Max_y = Page.Height.Millimeter;
                //画笔
                ColorTheme = XColor.FromArgb(0x37, 0x00, 0xb3);
                ColorGray = XColor.FromArgb(0x99, 0x99, 0x99);
                ColorBlack = XColor.FromArgb(0x00, 0x00, 0x00);

                Draw();

                string path = GetSaveFilePath();
                Document.Save(path);
                return path;
            }
            catch (Exception ex)
            {
                Log.Debug("打印错误:" + ex.ToString());
                return "";
            }
            return "";
        }

        private void NewPage()
        {
            //2. 新增一页
            Page = Document.AddPage();
            // 设置纸张大小
            Page.Size = PageSize.A4;
            //3. 创建一个绘图对象
            Gfx = XGraphics.FromPdfPage(Page);

            //定制化内容开始
            Cur_x = MarginHor;
            Cur_y = MarginVer;
        }

        private void Draw()
        {
            DrawPageTitle();
            DrawPatientInfo();
            DrawProjectInfo();
            DrawDate();
            DrawPageHint();
        }

        /// <summary>
        /// 绘制提示
        /// </summary>
        private void DrawPageHint()
        {
            XFont font = new XFont(fontNameBold, 14, XFontStyleEx.Regular);

            XSize HintSize = Gfx.MeasureString(Hint, font);
            Cur_y += ((int)HintSize.Height + 6);
            Gfx.DrawString(
                Hint,
                font,
                XBrushes.Black,
                new XPoint(width / 2 - (HintSize.Width / 2), Cur_y)
            );
            Cur_y += ((int)HintSize.Height + 6);
        }

        /// <summary>
        /// 绘制时间
        /// </summary>
        private void DrawDate()
        {
            float[] rowWidthWeight = new float[] { 0.3f, 0.3f, 0.7f, 0.3f, 0.3f, 0.7f };
            string[,] infos = new string[2, 6];

            infos[0, 0] = "";
            infos[0, 1] = "送检日期：";
            infos[0, 2] = SubmissionDate;
            infos[0, 3] = "";
            infos[0, 4] = "检验日期：";
            infos[0, 5] = TestDate;

            infos[1, 0] = "";
            infos[1, 1] = "检验者：";
            infos[1, 2] = Proofer;
            infos[1, 3] = "";
            infos[1, 4] = "审核者：";
            infos[1, 5] = Auditor;

            XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 1);
            Gfx.DrawLine(pen, Cur_x, Cur_y, Page.Width - MarginHor, Cur_y);
            DrawChartInfo(infos, rowWidthWeight, false, 20);
            // Gfx.DrawLine(pen, Cur_x, Cur_y, Page.Width - MarginHor, Cur_y);
        }

        /// <summary>
        /// 绘制项目结果信息
        /// </summary>
        private void DrawProjectInfo()
        {
            float[] rowWidthWeight = new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };

            int len = projectMaps.Count;
            string[,] infos = new string[len + 1, 5];

            infos[0, 0] = "检测编号";
            infos[0, 1] = "检测项目";
            infos[0, 2] = "检测值";
            infos[0, 3] = "检测结果";
            infos[0, 4] = "临界值";

            for (int i = 0; i < len; i++)
            {
                infos[i + 1, 0] = projectMaps[i].TestNum;
                infos[i + 1, 1] = projectMaps[i].TestItem;
                infos[i + 1, 2] = projectMaps[i].TestValue;
                infos[i + 1, 3] = projectMaps[i].TestResult;
                infos[i + 1, 4] = projectMaps[i].Reference;
            }

            XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 1);
            Gfx.DrawLine(pen, Cur_x, Cur_y, Page.Width - MarginHor, Cur_y);
            DrawChartInfo(infos, rowWidthWeight, false, 20);
            Gfx.DrawLine(pen, Cur_x, Cur_y, Page.Width - MarginHor, Cur_y);
        }

        /// <summary>
        /// 绘制个人信息
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="um"></param>
        private void DrawPatientInfo()
        {
            float[] rowWidthWeight = new float[] { 0.4f, 0.6f, 0.4f, 0.6f, 0.4f, 0.6f, 0.4f, 0.6f };
            string[,] infos = new string[3, 8];

            infos[0, 0] = "患者姓名：";
            infos[0, 1] = PatientName;
            infos[0, 2] = "病历号：";
            infos[0, 3] = PatientNum;
            infos[0, 4] = "床  号：";
            infos[0, 5] = BedNum;
            infos[0, 6] = "就诊类型：";
            infos[0, 7] = VisitType;

            infos[1, 0] = "性  别：";
            infos[1, 1] = Gender;
            infos[1, 2] = "送检医生：";
            infos[1, 3] = DoctorName;
            infos[1, 4] = "标本类型：";
            infos[1, 5] = SampleType;
            infos[1, 6] = "备  注：";
            infos[1, 7] = Remark;

            infos[2, 0] = "年  龄：";
            infos[2, 1] = Age;
            infos[2, 2] = "送检科室：";
            infos[2, 3] = Office;
            infos[2, 4] = "";
            infos[2, 5] = "";
            infos[2, 6] = "";
            infos[2, 7] = "";

            XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 1);
            //Gfx.DrawLine(pen, Cur_x, Cur_y, Page.Width - MarginHor, Cur_y);
            DrawChartInfo(infos, rowWidthWeight, false,20);
            Gfx.DrawLine(pen, Cur_x, Cur_y, Page.Width - MarginHor, Cur_y);
        }

        /**
*  绘制列表的，自己封装的
**/
        private void DrawChartInfo(string[,] infos, float[] weights, bool isBetweenLine,int rowHeight)
        {
            
            float[] rowWidthWeight = weights;
            //真实行宽
            double[] rowWidth = new double[rowWidthWeight.Length];
            float weightCount = 0;
            double width = Page.Width - MarginHor * 2;

            XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 1);

            if (isBetweenLine)
            {
                //绘制行中间横线
                for (int i = 0; i <= infos.GetLength(0); i++)
                {
                    //横线2
                    Gfx.DrawLine(
                        pen,
                        Cur_x,
                        Cur_y + (rowHeight * i),
                        Page.Width - MarginHor,
                        Cur_y + (rowHeight * i)
                    );
                }

                ////横线最下面的
                //gfx.DrawLine(pen, cur_x, cur_y + rowHeight * 2, page.Width - margin_left_right, cur_y + rowHeight * 2);

                //竖线最左
                Gfx.DrawLine(pen, Cur_x, Cur_y, Cur_x, Cur_y + rowHeight * 2);
            }
            for (int i = 0; i < rowWidthWeight.Length; i++)
            {
                weightCount += rowWidthWeight[i];
            }

            XFont font = new XFont(fontName, 10, XFontStyleEx.Regular);

            //竖线，中间的
            double leftOffset = 0;
            for (int i = 0; i < rowWidthWeight.Length; i++)
            {
                double left = leftOffset + MarginHor;
                rowWidth[i] = (width * (rowWidthWeight[i] / weightCount));

                for (int j = 0; j < infos.GetLength(0); j++)
                {
                    if (infos[j, i] != null)
                    {
                        //绘制信息
                        Gfx.DrawString(
                            infos[j, i],
                            font,
                            XBrushes.Black,
                            new XRect(left, Cur_y + rowHeight * j, rowWidth[i], rowHeight),
                            XStringFormats.CenterLeft
                        );
                    }
                    //Gfx.DrawString(infos[1, i], font, XBrushes.Black, new XRect(left, Cur_y + rowHeight, rowWidth[i], rowHeight), XStringFormats.Center);
                }
                leftOffset += rowWidth[i];
                if (isBetweenLine)
                {
                    //绘制竖线
                    Gfx.DrawLine(
                        pen,
                        MarginHor + leftOffset,
                        Cur_y,
                        MarginHor + leftOffset,
                        Cur_y + rowHeight * 2
                    );
                }
            }
            Cur_y += rowHeight * infos.GetLength(0);
        }

        /// <summary>
        /// 绘制标题
        /// </summary>
        private void DrawPageTitle()
        {
            XFont font = new XFont(fontNameBold, 14, XFontStyleEx.Bold);

            XPen pen = new XPen(XColor.FromKnownColor(XKnownColor.Black), 2);

            XSize HospitalNameSize = Gfx.MeasureString(Hospital, font);
            Gfx.DrawString(
                Hospital,
                font,
                XBrushes.Black,
                new XPoint(width / 2 - (HospitalNameSize.Width / 2), Cur_y)
            );
            Cur_y += ((int)HospitalNameSize.Height + 6);

            XSize titleSize = Gfx.MeasureString(Title, font);
            Gfx.DrawString(
                Title,
                font,
                XBrushes.Black,
                new XPoint(width / 2 - (titleSize.Width / 2), Cur_y)
            );
            Cur_y += ((int)titleSize.Height + 3);
        }

        /// <summary>
        /// 获取保存文件路径
        /// </summary>
        /// <returns>PDF文件保存路径</returns>
        private string GetSaveFilePath()
        {
            string folder = Directory.GetCurrentDirectory();
            folder += "\\PDF";
            string date = DateTime.Now.ToString("yyyy年MM月dd日");
            folder += "\\" + date;
            CreateDir(folder);
            string fileName = DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒") + ".pdf";
            return folder + "\\" + fileName;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="tr">检验结果对象</param>
        public void InitData(TestResult tr)
        {
            this.tr = tr;
            // 保存所有字段值
            SampleNum = tr.SampleNum;
            SampleType = tr.SampleType;
            PatientName = tr.PatientName;
            PatientNum = tr.PatientNum;
            Age = tr.Age;
            Gender = tr.Gender;
            Birthday = tr.Birthday;
            AdmissionNum = tr.AdmissionNum;
            BedNum = tr.BedNum;
            Office = tr.Office;
            DoctorName = tr.DoctorName;
            Proofer = tr.Proofer;
            Auditor = tr.Auditor;
            Hospital = tr.Hospital;
            VisitType = tr.VisitType;
            SubmissionDate = tr.SubmissionDate;
            TestDate = tr.TestDate;
            Remark = tr.Remark;
            projectMaps = tr.Result;
            Title = "检查报告单";
            Hint = "本检验结果仅对该样本负责";
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="folder">目录路径</param>
        private static void CreateDir(string folder)
        {
            DirectoryInfo root = new DirectoryInfo(folder);
            if (!root.Exists)
            {
                root.Create();
                return;
            }
        }
    }
}
