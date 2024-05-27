using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoT2.Controllers
{
    public class AdminController : Controller
    {

        [HttpPost]
        public JsonResult EnableDiemFunction()
        {
            try
            {
                ExecuteSql("GRANT INSERT, UPDATE ON dbo.Diem TO Faculty");
                return Json(new { success = true, message = "Đã bật chức năng nhập điểm." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DisableDiemFunction()
        {
            try
            {
                ExecuteSql("REVOKE INSERT, UPDATE ON dbo.Diem TO Faculty");
                return Json(new { success = true, message = "Đã tắt chức năng nhập điểm." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EnableDangKyFunction()
        {
            try
            {
                ExecuteSql("GRANT INSERT, UPDATE ON dbo.DangKy TO Student");
                return Json(new { success = true, message = "Đã bật chức năng đăng ký môn học." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DisableDangKyFunction()
        {
            try
            {
                ExecuteSql("REVOKE INSERT, UPDATE ON dbo.DangKy TO Student");
                return Json(new { success = true, message = "Đã tắt chức năng đăng ký môn học." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        private void ExecuteSql(string sql)
        {
            string DangNhapConn = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(DangNhapConn))
            {
                sqlConn.Open();
                using (SqlCommand command = new SqlCommand(sql, sqlConn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}