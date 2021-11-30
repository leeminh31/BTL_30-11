using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BTLQuanLyThuVien
{
    public partial class DangNhap : Form
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["QLTV"].ConnectionString;
        public DangNhap()
        {
            InitializeComponent();
        }

        private void btnDangnhap_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try { 
                connection.Open();
                string ten = txtTendangnhap.Text;
                string matkhau = txtMatkhau.Text;
                string query = string.Format("Select * from tblDangNhap where sTenDangNhap = '{0}' and sMatKhau = '{1}'", ten, matkhau);
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.HasRows)
                {
                    this.Hide();
                    QuanLyThuVien quanLyThuVien = new QuanLyThuVien();
                    quanLyThuVien.Show();                   
                }
                else
                {
                    MessageBox.Show("Đăng Nhập Thất Bại");
                }
            }  
            catch(Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
