using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BTLQuanLyThuVien
{
    public partial class BaoCao : Form
    {
        public BaoCao()
        {
            InitializeComponent();
        }
        public void showReport(string reportFilename, string query)
        {
            CrystalDecisions.CrystalReports.Engine.ReportDocument
            rpt = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
            string reportFile = string.Format("{0}\\{1}", "C:\\Users\\PC\\source\\repos\\BTLQuanLyThuVien\\BTLQuanLyThuVien", reportFilename);
            rpt.Load(reportFile);

            // Cập Nhật Thông Số DB
            CrystalDecisions.Shared.TableLogOnInfo
            table = new CrystalDecisions.Shared.TableLogOnInfo();
            table.ConnectionInfo.ServerName = @"LAPTOP-4FH94HRM\SQLEXPRESS";
            table.ConnectionInfo.UserID = "sa";
            table.ConnectionInfo.Password = "minhle3108";
            table.ConnectionInfo.DatabaseName = "QLThuVien";
            foreach (CrystalDecisions.CrystalReports.Engine.Table tbl in rpt.Database.Tables)
            {
                tbl.ApplyLogOnInfo(table);
            }
            //Lọc bản ghi
            rpt.RecordSelectionFormula = query;
            //Tiêu Đề Report
            //rpt.SummaryInfo.ReportTitle = "Tiêu đề mới";
            //
            crvBaoCao.ReportSource = rpt;
            crvBaoCao.Refresh();
        }
    }
}
