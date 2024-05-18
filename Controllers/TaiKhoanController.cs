using DemoT2.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DemoT2.Controllers
{
    public class TaiKhoanController : BaseController
    {
        // GET: TaiKhoan
        public ActionResult DangNhap()
        {
            return View();
        }

        // GET: DangKy
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string user, string password, string captcha, bool rememberMe = false)
        {
            int maxAttempts = 5;
            int attempts = Session["LoginAttempts"] != null ? (int)Session["LoginAttempts"] : 0;
            string savedCaptcha = Session["Captcha"] as string ?? string.Empty;
//
            if (attempts >= maxAttempts && string.IsNullOrEmpty(savedCaptcha) && string.IsNullOrEmpty(captcha))
            {
                ViewBag.ShowCaptcha = true;
                ViewBag.CaptchaError = "Vui lòng nhập mã Captcha!";
                return View("DangNhap");
            }

            if (attempts >= maxAttempts && !string.IsNullOrEmpty(savedCaptcha) && captcha != savedCaptcha)
            {
                ViewBag.ShowCaptcha = true;
                ViewBag.CaptchaError = "Mã Captcha không chính xác!";
                return View("DangNhap");
            }


            string hashedPassword = HashPassword(password); // Mã hóa mật khẩu nhập vào

            string DangNhapConn = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(DangNhapConn))
            {
                string query = @"
            SELECT tk.taikhoan, tk.matkhau, tk.hoten, tk.sodienthoai, tk.ngaysinh, tk.email, r.role_name
            FROM TaiKhoan tk
            JOIN Role r ON tk.role_id = r.role_id
            WHERE tk.taikhoan = @username AND tk.matkhau = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", user);
                command.Parameters.AddWithValue("@password", hashedPassword);
                System.Console.WriteLine(hashedPassword);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        // Đăng nhập thành công
                        attempts = 0;
                        Session["LoginAttempts"] = attempts;
                        Session["ShowCaptcha"] = null;

                        // Login successful - create TaiKhoan object
                        TaiKhoan loggedInUser = new TaiKhoan();

                        // Fill TaiKhoan object with data from SqlDataReader
                        while (reader.Read())
                        {
                            loggedInUser.taikhoan = reader["taikhoan"].ToString();
                            loggedInUser.matkhau = reader["matkhau"].ToString();
                            loggedInUser.hoten = reader["hoten"].ToString();
                            loggedInUser.sodienthoai = reader["sodienthoai"].ToString();
                            loggedInUser.ngaysinh = Convert.ToDateTime(reader["ngaysinh"]);
                            loggedInUser.email = reader["email"].ToString();
                            loggedInUser.role = reader["role_name"].ToString();
                        }

                        // Store loggedInUser object in Session
                        Session["LoggedInUser"] = loggedInUser;

                        if (rememberMe)
                        {
                            // Tạo cookie để lưu trữ thông tin đăng nhập
                            HttpCookie cookie = new HttpCookie("UserLogin");
                            cookie.Values["Username"] = loggedInUser.taikhoan;
                            cookie.Values["Password"] = hashedPassword; // Cần mã hóa mật khẩu trước khi lưu trữ
                            cookie.Expires = DateTime.Now.AddDays(30);
                            Response.Cookies.Add(cookie);
                        }

                        // Redirect to the appropriate dashboard based on role
                        if (loggedInUser.role == "admin")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (loggedInUser.role == "user")
                        {
                            return RedirectToAction("Index", "User");
                        }
                        else if(loggedInUser.role == "manager")
                        {
                            // Handle other roles or default
                            return RedirectToAction("Index", "Manager");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        // Đăng nhập không thành công
                        attempts++;

                        if (attempts >= maxAttempts)
                        {
                            // Nếu vượt quá số lần đăng nhập sai
                            Session["ShowCaptcha"] = true; // Lưu session để hiển thị Captcha
                            ViewBag.ShowCaptcha = true; // Hiển thị Captcha trong View
                        }
                        ViewBag.error = "Sai Tài khoản hoặc mật khẩu";
                        Session["LoginAttempts"] = attempts;
                        return View("DangNhap");
                    }
                }
                catch (SqlException ex)
                {
                    // Pass the error message to ErrorView
                    ErrorViewModel errorModel = new ErrorViewModel();
                    errorModel.ErrorMessage = ex.Message; // You can modify this to include additional error information if needed

                    return View("ErrorView", errorModel);
                }
                finally
                {
                    connection.Close(); // Close the connection after execution
                }
            }
        }



        public ActionResult Logout()
        {
            // Xóa session
            Session["LoggedInUser"] = null;
            Session["LoginAttempts"] = 0;

            // Xóa cookie nếu có
            if (Request.Cookies["UserLogin"] != null)
            {
                HttpCookie cookie = new HttpCookie("UserLogin");
                cookie.Expires = DateTime.Now.AddDays(-1); // Đặt ngày hết hạn trong quá khứ để xóa cookie
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public ActionResult DangKy(TaiKhoan model)
        {
            if (ModelState.IsValid)
            {
                string DangKyConn = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(DangKyConn))
                {
                    string query = "INSERT INTO TaiKhoan (taikhoan, matkhau, hoten, sodienthoai, ngaysinh, email, role_id) " +
                                   "VALUES (@taikhoan, @matkhau, @hoten, @sodienthoai, @ngaysinh, @email, " +
                                   "(SELECT role_id FROM Role WHERE role_name = @role_name))";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@taikhoan", model.taikhoan);
                    command.Parameters.AddWithValue("@matkhau", HashPassword(model.matkhau)); // Mã hóa mật khẩu trước khi lưu trữ
                    command.Parameters.AddWithValue("@hoten", model.hoten);
                    command.Parameters.AddWithValue("@sodienthoai", model.sodienthoai);
                    command.Parameters.AddWithValue("@ngaysinh", model.ngaysinh);
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@role_name", "user");

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        // Đăng ký thành công
                        ViewBag.SuccessMessage = "Đăng ký thành công!";
                        return View();
                    }
                    catch (SqlException ex)
                    {
                        ModelState.AddModelError("", "Đã có lỗi xảy ra: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            // Nếu model không hợp lệ hoặc có lỗi xảy ra
            return View(model);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
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
