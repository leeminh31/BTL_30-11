using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BTLQuanLyThuVien
{
    public partial class QuanLyThuVien : Form
    {
        //public static string mataikhoan = "";
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["QLTV"].ToString();
        public QuanLyThuVien()
        {
            InitializeComponent();
        }

        //public QuanLyThuVien(string mataikhoan)
        //{
        //    InitializeComponent();
        //    this.mataikhoan = mataikhoan;
        //}

        private DataTable Get_Dulieu( string procname, string tablename)
        {
            SqlConnection cnn = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(procname, cnn);
            command.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable tbl = new DataTable(tablename);
            da.Fill(tbl);
            return tbl;
        }
        private void hienthi(string procname, string tablename, DataGridView dgv)
        {
            DataTable table = Get_Dulieu(procname,tablename);
            DataView view = new DataView(table);
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = view;
        }
        //private void kiemtraquyen() 
        //{
        //    string quyen = "";
        //    SqlConnection cnn = new SqlConnection(connectionString);
        //    SqlCommand command = new SqlCommand("sp_select_thuthu", cnn);
        //    command.CommandType = CommandType.StoredProcedure;
        //    SqlDataAdapter da = new SqlDataAdapter(command);
        //    DataTable tbl = new DataTable("tblThuThu");
        //    da.Fill(tbl);
        //    if( tbl.Select("select * from tblThuThu where FK_sMaTK='"+ mataikhoan+"'").Length>0)
        //        quyen = 

        //}
        private void Form1_Load(object sender, EventArgs e)
        {
            hienthi("sp_select_sach","tblSach",dgvDanhSachSach);
            hienthi("sp_select_theloai", "tblTheloai", dgvDanhSachTheLoai);
            hienthi("sp_select_sinhvien", "tblSinhVien", dgvDanhSachSinhVien);
            hienthi("sp_select_thuthu", "tblThuThu", dgvDanhSachThuThu);
            hienthi("sp_select_phieumuon", "tblPhieuMuon", dgvDanhSachPhieuMuon);
            hienthi("sp_select_phieumuonchitiet", "tblPhieuMuonChiTiet", dgvDanhSachPhieuMuonChiTiet);
        }
        
        //tab Phiếu Mượn
        private void btnThemPM_Click_1(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (btnThemPM.Text.CompareTo("Ghi Nhận")==0 )
                    {
                        cmd.CommandText = "sp_PhieuMuonUpdate";
                        //cmd.Parameters.AddWithValue("@PK_iKhachhangID", btnSuaPM.Tag);                        
                    }
                    else
                    {
                        cmd.CommandText = "sp_insertPM";
                        //cmd.Parameters.Add("@PK_iKhachhangID", SqlDbType.Int).Direction = ParameterDirection.Output;            
                    }
                    cmd.Parameters.AddWithValue("@PK_iMaPhieu", txtMaphieuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaTT", txtMathuthuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaSV", txtMasinhvienPM.Text);
                    cmd.Parameters.AddWithValue("@dNgayMuon", Convert.ToDateTime(dtpNgaymuon.Text));
                    cmd.Parameters.AddWithValue("@dNgayTra", Convert.ToDateTime(dtpNgaytra.Text));
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                    btnThemPM.Text = "Thêm";
                    btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = true;
                }
            }
            hienthi("sp_select_phieumuon", "tblPhieuMuon", dgvDanhSachPhieuMuon);
        }

        private void btnSuaPM_Click_1(object sender, EventArgs e)
        {
            DataView dvPhieuMuon = (DataView)dgvDanhSachPhieuMuon.DataSource;
            DataRowView row = dvPhieuMuon[dgvDanhSachPhieuMuon.CurrentRow.Index];
            txtMaphieuPM.Text = row["PK_iMaPhieu"].ToString();
            txtMathuthuPM.Text = row["FK_iMaTT"].ToString();
            txtMasinhvienPM.Text = row["FK_iMaSV"].ToString();
            dtpNgaymuon.Text = Convert.ToDateTime(row["dNgayMuon"]).ToString();
            dtpNgaytra.Text = Convert.ToDateTime(row["dNgayTra"]).ToString();
            btnThemPM.Text = "Ghi Nhận";
            btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = false;
        }
        
        private void btnXoaPM_Click(object sender, EventArgs e)
        {
            DataView dv = (DataView)dgvDanhSachPhieuMuon.DataSource;
            DataRowView row = dv[dgvDanhSachPhieuMuon.CurrentRow.Index];
            SqlConnection cnn = new SqlConnection(connectionString);
            cnn.Open();
            SqlCommand command = new SqlCommand(String.Format("Delete from tblPhieuMuon where PK_iMaPhieu = {0}", Convert.ToInt32(row["PK_iMaPhieu"].ToString())), cnn);
            int i = command.ExecuteNonQuery();
            if (i != 0)
            {
                dgvDanhSachPhieuMuon.Rows.RemoveAt(dgvDanhSachPhieuMuon.CurrentRow.Index);
                MessageBox.Show("Xóa Thành Công");
            }
            else
            {
                MessageBox.Show("Xóa Thất Bại");
            }
        }

        private void btnBaocaoPM_Click(object sender, EventArgs e)
        {
            BaoCao baoCao = new BaoCao();
            baoCao.showReport("PhieuMuon.rpt", "");
            this.Hide();
            baoCao.Show();
        }

        private void btnTimkiemPM_Click(object sender, EventArgs e)
        {
            string dieukienLoc = "PK_iMaPhieu>0";
            if (!string.IsNullOrEmpty(txtMaphieuPM.Text))
                dieukienLoc += string.Format(" AND PK_iMaPhieu = {0}", Convert.ToInt32(txtMaphieuPM.Text));
            //DialogResult timtheogioitinh = MessageBox.Show("Có tìm theo giới tính không ?", "Tìm Kiếm"
            //                                , MessageBoxButtons.YesNo);
            //if (timtheogioitinh == DialogResult.Yes)
            //    dieukienLoc += string.Format(" AND bGioitinh = {0}", rdNam.Checked);
            if (!string.IsNullOrEmpty(txtMathuthuPM.Text))
                dieukienLoc += string.Format(" AND FK_iMaTT = {0}", Convert.ToInt32(txtMathuthuPM.Text));
            if (!string.IsNullOrEmpty(txtMasinhvienPM.Text))
                dieukienLoc += string.Format(" AND FK_iMaSV = {0}", Convert.ToInt32(txtMasinhvienPM.Text));
            DataView dvPhieuMuon = (DataView)dgvDanhSachPhieuMuon.DataSource;
            dvPhieuMuon.RowFilter = dieukienLoc;
            dgvDanhSachPhieuMuon.DataSource = dvPhieuMuon;

        }

        private void btnThemPMCT_Click(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (btnThemPM.Text.CompareTo("Ghi Nhận") == 0)
                    {
                        cmd.CommandText = "sp_PhieuMuonUpdate";
                        //cmd.Parameters.AddWithValue("@PK_iKhachhangID", btnSuaPM.Tag);                        
                    }
                    else
                    {
                        cmd.CommandText = "sp_insertPM";
                        //cmd.Parameters.Add("@PK_iKhachhangID", SqlDbType.Int).Direction = ParameterDirection.Output;            
                    }
                    cmd.Parameters.AddWithValue("@PK_iMaPhieu", txtMaphieuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaTT", txtMathuthuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaSV", txtMasinhvienPM.Text);
                    cmd.Parameters.AddWithValue("@dNgayMuon", Convert.ToDateTime(dtpNgaymuon.Text));
                    cmd.Parameters.AddWithValue("@dNgayTra", Convert.ToDateTime(dtpNgaytra.Text));
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                    btnThemPM.Text = "Thêm";
                    btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = true;
                }
            }
            hienthi("sp_select_phieumuon", "tblPhieuMuon", dgvDanhSachPhieuMuon);
        }

        private void btnSuaPMCT_Click(object sender, EventArgs e)
        {
            DataView dvPhieuMuon = (DataView)dgvDanhSachPhieuMuon.DataSource;
            DataRowView row = dvPhieuMuon[dgvDanhSachPhieuMuon.CurrentRow.Index];
            txtMaphieuPM.Text = row["PK_iMaPhieu"].ToString();
            txtMathuthuPM.Text = row["FK_iMaTT"].ToString();
            txtMasinhvienPM.Text = row["FK_iMaSV"].ToString();
            dtpNgaymuon.Text = Convert.ToDateTime(row["dNgayMuon"]).ToString();
            dtpNgaytra.Text = Convert.ToDateTime(row["dNgayTra"]).ToString();
            btnThemPM.Text = "Ghi Nhận";
            btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = false;
        }

        private void btnXoaPMCT_Click(object sender, EventArgs e)
        {
            DataView dvPhieuMuon = (DataView)dgvDanhSachPhieuMuon.DataSource;
            DataRowView row = dvPhieuMuon[dgvDanhSachPhieuMuon.CurrentRow.Index];
            SqlConnection cnn = new SqlConnection(connectionString);
            cnn.Open();
            SqlCommand command = new SqlCommand("Delete from tblPhieuMuon where PK_iMaPhieu = " + Convert.ToInt32(row["PK_iMaPhieu"].ToString()), cnn);
            int i = command.ExecuteNonQuery();
            if (i != 0)
            {
                dgvDanhSachPhieuMuon.Rows.RemoveAt(dgvDanhSachPhieuMuon.CurrentRow.Index);
                MessageBox.Show("Xóa Thành Công");
            }
            else
            {
                MessageBox.Show("Xóa Thất Bại");
            }
        }

        private void btnBaoCaoPMCT_Click(object sender, EventArgs e)
        {
            BaoCao baoCao = new BaoCao();
            baoCao.showReport("PhieuMuon.rpt", "");
            this.Hide();
            baoCao.Show();
        }

        private void btnTimKiemPMCT_Click(object sender, EventArgs e)
        {
            string dieukienLoc = "iMaPhieu>0";
            if (!string.IsNullOrEmpty(txtMaPhieuPMCT.Text))
                dieukienLoc += string.Format(" AND iMaPhieu = {0}", Convert.ToInt32(txtMaPhieuPMCT.Text));
            //DialogResult timtheogioitinh = MessageBox.Show("Có tìm theo giới tính không ?", "Tìm Kiếm"
            //                                , MessageBoxButtons.YesNo);
            //if (timtheogioitinh == DialogResult.Yes)
            //    dieukienLoc += string.Format(" AND bGioitinh = {0}", rdNam.Checked);
            if (!string.IsNullOrEmpty(txtMaSachPMCT.Text))
                dieukienLoc += string.Format(" AND iMaSach = {0}", Convert.ToInt32(txtMaSachPMCT.Text));
            if (!string.IsNullOrEmpty(txtMasinhvienPM.Text))
                dieukienLoc += string.Format(" AND iSoluong = {0}", Convert.ToInt32(txtSoLuongPMCT.Text));
            DataView dvPhieuMuonChiTiet = (DataView)dgvDanhSachPhieuMuonChiTiet.DataSource;
            dvPhieuMuonChiTiet.RowFilter = dieukienLoc;
            dgvDanhSachPhieuMuonChiTiet.DataSource = dvPhieuMuonChiTiet;
        }

        private void btnThemTT_Click(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (btnThemPM.Text.CompareTo("Ghi Nhận") == 0)
                    {
                        cmd.CommandText = "sp_ThuThuUpdate";
                        //cmd.Parameters.AddWithValue("@PK_iKhachhangID", btnSuaPM.Tag);                        
                    }
                    else
                    {
                        cmd.CommandText = "sp_insertTT";
                        //cmd.Parameters.Add("@PK_iKhachhangID", SqlDbType.Int).Direction = ParameterDirection.Output;            
                    }
                    cmd.Parameters.AddWithValue("@PK_iMaTT", txtMathuthuTT.Text);
                    cmd.Parameters.AddWithValue("@FK_sMaTK", txtMataikhoanTT.Text);
                    cmd.Parameters.AddWithValue("@sHoTen", txtHotenTT.Text);
                    cmd.Parameters.AddWithValue("@sQuyen", txtQuyenTT.Text);
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                    btnThemPM.Text = "Thêm";
                    btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = true;
                }
            }
            hienthi("sp_select_thuthu", "tblThuThu", dgvDanhSachThuThu);
        }

        private void btnSuaTT_Click(object sender, EventArgs e)
        {

        }

        private void btnXoaTT_Click(object sender, EventArgs e)
        {

        }

        private void btnBaocaoTT_Click(object sender, EventArgs e)
        {

        }

        private void btnTimkiemTT_Click(object sender, EventArgs e)
        {

        }

        private void btnThemS_Click(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (btnThemPM.Text.CompareTo("Ghi Nhận") == 0)
                    {
                        cmd.CommandText = "sp_PhieuMuonUpdate";
                        //cmd.Parameters.AddWithValue("@PK_iKhachhangID", btnSuaPM.Tag);                        
                    }
                    else
                    {
                        cmd.CommandText = "sp_insertPM";
                        //cmd.Parameters.Add("@PK_iKhachhangID", SqlDbType.Int).Direction = ParameterDirection.Output;            
                    }
                    cmd.Parameters.AddWithValue("@PK_iMaPhieu", txtMaphieuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaTT", txtMathuthuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaSV", txtMasinhvienPM.Text);
                    cmd.Parameters.AddWithValue("@dNgayMuon", Convert.ToDateTime(dtpNgaymuon.Text));
                    cmd.Parameters.AddWithValue("@dNgayTra", Convert.ToDateTime(dtpNgaytra.Text));
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                    btnThemPM.Text = "Thêm";
                    btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = true;
                }
            }
            hienthi("sp_select_phieumuon", "tblPhieuMuon", dgvDanhSachPhieuMuon);
        }

        private void btnSuaS_Click(object sender, EventArgs e)
        {

        }

        private void btnXoaS_Click(object sender, EventArgs e)
        {

        }

        private void btnBaocaoS_Click(object sender, EventArgs e)
        {

        }

        private void btnTimkiemS_Click(object sender, EventArgs e)
        {

        }

        private void btnThemSV_Click(object sender, EventArgs e)
        {
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (btnThemPM.Text.CompareTo("Ghi Nhận") == 0)
                    {
                        cmd.CommandText = "sp_PhieuMuonUpdate";
                        //cmd.Parameters.AddWithValue("@PK_iKhachhangID", btnSuaPM.Tag);                        
                    }
                    else
                    {
                        cmd.CommandText = "sp_insertPM";
                        //cmd.Parameters.Add("@PK_iKhachhangID", SqlDbType.Int).Direction = ParameterDirection.Output;            
                    }
                    cmd.Parameters.AddWithValue("@PK_iMaPhieu", txtMaphieuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaTT", txtMathuthuPM.Text);
                    cmd.Parameters.AddWithValue("@FK_iMaSV", txtMasinhvienPM.Text);
                    cmd.Parameters.AddWithValue("@dNgayMuon", Convert.ToDateTime(dtpNgaymuon.Text));
                    cmd.Parameters.AddWithValue("@dNgayTra", Convert.ToDateTime(dtpNgaytra.Text));
                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();
                    btnThemPM.Text = "Thêm";
                    btnSuaPM.Enabled = btnTimkiemPM.Enabled = btnBaocaoPM.Enabled = btnXoaPM.Enabled = true;
                }
            }
            hienthi("sp_select_phieumuon", "tblPhieuMuon", dgvDanhSachPhieuMuon);
        }

        private void btnSuaSV_Click(object sender, EventArgs e)
        {

        }

        private void btnXoaSV_Click(object sender, EventArgs e)
        {

        }

        private void btnBaocaoSV_Click(object sender, EventArgs e)
        {

        }

        private void btnTimkiemSV_Click(object sender, EventArgs e)
        {

        }
    }
}
