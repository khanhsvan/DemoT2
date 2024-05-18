using DemoT2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace DemoT2.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            List<TaiKhoan> taiKhoans = new List<TaiKhoan>();
            string connString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query = "SELECT taikhoan, hoten, sodienthoai, ngaysinh, email FROM TaiKhoan";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        TaiKhoan tk = new TaiKhoan
                        {
                            taikhoan = reader["taikhoan"].ToString(),
                            hoten = reader["hoten"].ToString(),
                            sodienthoai = reader["sodienthoai"].ToString(),
                            ngaysinh = Convert.ToDateTime(reader["ngaysinh"]),
                            email = reader["email"].ToString()
                        };
                        taiKhoans.Add(tk);
                    }
                }
                catch (SqlException ex)
                {
                    // Handle exception if needed
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            return View(taiKhoans);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    string query = "INSERT INTO TaiKhoan (taikhoan, matkhau, hoten, sodienthoai, ngaysinh, email, role_id) " +
                                   "VALUES (@taikhoan, @matkhau, @hoten, @sodienthoai, @ngaysinh, @email, " +
                                   "(SELECT role_id FROM Role WHERE role_name = 'user'))";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@taikhoan", model.taikhoan);
                    command.Parameters.AddWithValue("@matkhau", HashPassword(model.matkhau)); // Mã hóa mật khẩu trước khi lưu trữ
                    command.Parameters.AddWithValue("@hoten", model.hoten);
                    command.Parameters.AddWithValue("@sodienthoai", model.sodienthoai);
                    command.Parameters.AddWithValue("@ngaysinh", model.ngaysinh);
                    command.Parameters.AddWithValue("@email", model.email);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        // Đăng ký thành công
                        return RedirectToAction("Index");
                    }
                    catch (SqlException ex)
                    {
                        ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                    }
                }
            }

            // Nếu model không hợp lệ hoặc có lỗi xảy ra
            return View(model);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(string id)
        {
            TaiKhoan taiKhoan = null;
            string connString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query = "SELECT taikhoan, hoten, sodienthoai, ngaysinh, email FROM TaiKhoan WHERE taikhoan = @taikhoan";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@taikhoan", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        taiKhoan = new TaiKhoan
                        {
                            taikhoan = reader["taikhoan"].ToString(),
                            hoten = reader["hoten"].ToString(),
                            sodienthoai = reader["sodienthoai"].ToString(),
                            ngaysinh = Convert.ToDateTime(reader["ngaysinh"]),
                            email = reader["email"].ToString()
                        };
                    }
                }
                catch (SqlException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            return taiKhoan != null ? View(taiKhoan) : (ActionResult)HttpNotFound();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    string query = "UPDATE TaiKhoan SET hoten = @hoten, sodienthoai = @sodienthoai, ngaysinh = @ngaysinh, email = @email WHERE taikhoan = @taikhoan";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@taikhoan", model.taikhoan);
                    command.Parameters.AddWithValue("@hoten", model.hoten);
                    command.Parameters.AddWithValue("@sodienthoai", model.sodienthoai);
                    command.Parameters.AddWithValue("@ngaysinh", model.ngaysinh);
                    command.Parameters.AddWithValue("@email", model.email);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        // Cập nhật thành công
                        return RedirectToAction("Index");
                    }
                    catch (SqlException ex)
                    {
                        ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                    }
                }
            }

            return View(model);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(string id)
        {
            TaiKhoan taiKhoan = null;
            string connString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query = "SELECT taikhoan, hoten, sodienthoai, ngaysinh, email FROM TaiKhoan WHERE taikhoan = @taikhoan";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@taikhoan", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        taiKhoan = new TaiKhoan
                        {
                            taikhoan = reader["taikhoan"].ToString(),
                            hoten = reader["hoten"].ToString(),
                            sodienthoai = reader["sodienthoai"].ToString(),
                            ngaysinh = Convert.ToDateTime(reader["ngaysinh"]),
                            email = reader["email"].ToString()
                        };
                    }
                }
                catch (SqlException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            return taiKhoan != null ? View(taiKhoan) : (ActionResult)HttpNotFound();
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            string connString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                string query = "DELETE FROM TaiKhoan WHERE taikhoan = @taikhoan";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@taikhoan", id);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    // Xóa thành công
                    return RedirectToAction("Index");
                }
                catch (SqlException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View();
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
