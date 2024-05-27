 using DemoT2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DemoT2.Controllers
{
    public class TaiKhoanController : Controller
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
            using (var db = new QuanLySinhVienDbContext())
            {
                var userAccount = db.TaiKhoans
                    .Where(t => t.UserName == user && t.PasswordHash == hashedPassword)
                    .Select(t => new
                    {
                        t.UserName,
                        t.PasswordHash,
                        MaSinhVien = t.SinhVien.MaSinhVien != null ? t.SinhVien.MaSinhVien : -1, // Default value for MaSinhVien
                        MaGiaoVien = t.GiaoVien.MaGiaoVien != null ? t.GiaoVien.MaGiaoVien : -1 // Default value for MaGiaoVien
                    })
                    .FirstOrDefault();

                if (userAccount != null)
                {
                    // Đăng nhập thành công
                    attempts = 0;
                    Session["LoginAttempts"] = attempts;
                    Session["ShowCaptcha"] = null;
                    string serverName = "LAVANKHANH\\SQLEXPRESS"; // Tên máy chủ
                    string databaseName = "QuanLySinhVien"; // Tên cơ sở dữ liệu

                    // Tạo chuỗi kết nối
                    string connectionString = $"Server={serverName}; Database={databaseName}; User ID={user}; Password={password};";
                    Session["ConnectionString"] = connectionString;

                    // Create and store TaiKhoan object in Session
                    TaiKhoan loggedInUser = new TaiKhoan
                    {
                        UserName = userAccount.UserName,
                        PasswordHash = userAccount.PasswordHash,
                        SinhVienId = userAccount.MaSinhVien,
                        GiaoVienId = userAccount.MaGiaoVien
                    };

                    Session["LoggedInUser"] = loggedInUser;
                    using (SqlConnection connection = new SqlConnection(DangNhapConn))
                    {
                        // Your SQL query
                        string query = @"
                            DECLARE @Username NVARCHAR(50) = @User;
                            SELECT 
                                CASE 
                                    WHEN TK.SinhVienId IS NOT NULL THEN TK.SinhVienId
                                    WHEN TK.GiaoVienId IS NOT NULL THEN TK.GiaoVienId
                                    ELSE -1 -- Giá trị mặc định hoặc 'Unknown'
                                END AS [UserID],
                                CASE 
                                    WHEN TK.SinhVienId IS NOT NULL THEN SV.Ho + ' ' + SV.Ten
                                    WHEN TK.GiaoVienId IS NOT NULL THEN GV.Ho + ' ' + GV.Ten
                                    ELSE 'Unknown'
                                END AS [HoTen],
                                CASE 
                                    WHEN TK.SinhVienId IS NOT NULL THEN 'SinhVien'
                                    WHEN TK.GiaoVienId IS NOT NULL THEN 'GiaoVien'
                                    ELSE 'Unknown'
                                END AS [LoaiNguoiDung]
                            FROM 
                                dbo.TaiKhoans TK
                            LEFT JOIN 
                                dbo.SinhVien SV ON TK.SinhVienId = SV.MaSinhVien
                            LEFT JOIN 
                                dbo.GiaoVien GV ON TK.GiaoVienId = GV.MaGiaoVien
                            WHERE 
                                TK.UserName = @Username";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@User", user);

                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                string hoTen = reader["HoTen"].ToString();
                                string loaiNguoiDung = reader["LoaiNguoiDung"].ToString();
                                int UserID = Convert.ToInt32(reader["UserID"]);
                                Session["hoTen"] = hoTen;
                                Session["UserID"] = UserID;
                            }
                            else
                            {
                                // No matching user found
                                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                                return View("DangNhap");
                            }
                        }
                    }


                    // Kiểm tra kết nối đến cơ sở dữ liệu
                    try
                        {
                            using (SqlConnection sqlConn = new SqlConnection(connectionString))
                            {
                                sqlConn.Open();
                                // Kết nối thành công
                                SqlConnection checking = new SqlConnection(DangNhapConn);
                                checking.Open();

                            // Truy vấn UserName và LoginName từ SQL Server
                            SqlCommand command = new SqlCommand("SELECT dp.name AS UserName, sp.name AS LoginName FROM sys.server_principals sp JOIN sys.database_principals dp ON sp.sid = dp.sid WHERE sp.type_desc = 'SQL_LOGIN' AND sp.name = @LoginName", sqlConn);
                            command.Parameters.AddWithValue("@LoginName", user);
                            SqlDataReader reader = command.ExecuteReader();
                            string userName = null;
                            string loginName = null;
                            if (reader.Read())
                            {
                                userName = reader["UserName"].ToString();
                                loginName = reader["LoginName"].ToString();
                            }
                            reader.Close();

                            // Truy vấn RoleName từ UserName
                            command = new SqlCommand("SELECT dp.name AS RoleName FROM sys.database_role_members drm JOIN sys.database_principals dp ON dp.principal_id = drm.role_principal_id JOIN sys.database_principals mp ON mp.principal_id = drm.member_principal_id WHERE mp.name = @UserName", sqlConn);
                            command.Parameters.AddWithValue("@UserName", userName);
                            reader = command.ExecuteReader();
                            List<string> roleNames = new List<string>();
                            while (reader.Read())
                            {
                                roleNames.Add(reader["RoleName"].ToString());
                            }
                            reader.Close();
                            ViewBag.UserName = userName;
                            ViewBag.LoginName = loginName;
                            ViewBag.RoleNames = roleNames;
                            // Truy vấn quyền của các role
                            foreach (var roleName in roleNames)
                            {
                                command = new SqlCommand("SELECT permission_name FROM sys.database_permissions WHERE grantee_principal_id IN (SELECT principal_id FROM sys.database_principals WHERE name = @RoleName)", sqlConn);
                                command.Parameters.AddWithValue("@RoleName", roleName);
                                reader = command.ExecuteReader();
                                List<string> permissions = new List<string>();
                                while (reader.Read())
                                {
                                    permissions.Add(reader["permission_name"].ToString());
                                }
                                reader.Close();
                                // Xử lý danh sách quyền
                                // Lưu danh sách quyền và bảng vào ViewBag
                                ViewBag.Permissions = permissions;
                            }

                            // Truy vấn danh sách các bảng có thể thao tác
                            command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", sqlConn);


                            reader = command.ExecuteReader();
                            List<string> tables = new List<string>();
                            while (reader.Read())
                            {
                                tables.Add(reader["TABLE_NAME"].ToString());
                            }
                            reader.Close();
                            // Xử lý danh sách các bảng
                            ViewBag.Tables = tables;
                            // Khởi tạo ViewBag.RolePermissionsForTable nếu nó chưa tồn tại
                            if (ViewBag.RolePermissionsForTable == null)
                            {
                                ViewBag.RolePermissionsForTable = new Dictionary<string, List<string>>();
                            }
                            // Truy vấn quyền của role đối với từng bảng
                            foreach (var table in tables)
{
                                List<string> permissionsForTable = new List<string>(); // Tạo danh sách quyền mới cho mỗi bảng
                                foreach (var roleName in roleNames)
                                {
                                    command = new SqlCommand("SELECT permission_name FROM sys.database_permissions WHERE grantee_principal_id IN (SELECT principal_id FROM sys.database_principals WHERE name = @RoleName) AND major_id = OBJECT_ID(@TableName)", sqlConn);
                                    command.Parameters.AddWithValue("@RoleName", roleName);
                                    command.Parameters.AddWithValue("@TableName", table);
                                    reader = command.ExecuteReader();
                                    while (reader.Read())
                                    {
                                        permissionsForTable.Add(reader["permission_name"].ToString()); // Thêm quyền vào danh sách quyền của bảng
                                    }
                                    reader.Close();
                                }
                                ViewBag.RolePermissionsForTable[table] = permissionsForTable; // Lưu danh sách quyền của bảng vào ViewBag dựa trên tên bảng
                            }
                            if (rememberMe)
                            {
                                // Tạo cookie để lưu trữ thông tin đăng nhập
                                HttpCookie cookie = new HttpCookie("Userdata");
                                cookie.Values["Username"] = loggedInUser.UserName;
                                cookie.Values["Password"] = password; // Cần mã hóa mật khẩu trước khi lưu trữ
                                cookie.Values["loginname"] = loginName;
                                cookie.Values["roleNames"] = string.Join(",", roleNames);
                                cookie.Values["tables"] = string.Join(",", tables);
                                cookie.Expires = DateTime.Now.AddDays(1);
                                Response.Cookies.Add(cookie);
                            }
                            // Lưu thông tin đăng nhập vào session
                            Session["LoggedInUserInfo"] = new UserSessionInfo
                            {
                                UserName = loggedInUser.UserName,
                                Password = password, // Chú ý: Tránh lưu mật khẩu trong session một cách rõ ràng, đây chỉ là một ví dụ đơn giản
                                LoginName = loginName,
                                RoleNames = roleNames,
                                Tables = tables
                            };


                            // Đóng kết nối
                            sqlConn.Close();

                            return View("TrangChucNang");
                            }
                        }
                        catch (SqlException ex)
                        {
                            StringBuilder errorMessage = new StringBuilder();
                            errorMessage.AppendLine("Kết nối cơ sở dữ liệu thất bại. Chi tiết lỗi:");
                            errorMessage.AppendLine("Message: " + ex.Message);
                            errorMessage.AppendLine("Error Number: " + ex.Number);

                            // Lặp qua mỗi lỗi trong SqlException.Errors và in ra thông tin chi tiết
                            foreach (SqlError error in ex.Errors)
                            {
                                errorMessage.AppendLine("Error: " + error.Message);
                                errorMessage.AppendLine("Server: " + error.Server);
                                errorMessage.AppendLine("Procedure: " + error.Procedure);
                                // Thêm các thuộc tính khác của SqlError mà bạn muốn in ra
                            }

                            // In ra StackTrace nếu bạn muốn
                            errorMessage.AppendLine("StackTrace: " + ex.StackTrace);

                            // Gán thông báo lỗi vào ViewBag để hiển thị trên View
                            ViewBag.error = errorMessage.ToString();

                            return View("DangNhap");
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
        }

        public ActionResult TrangChucNang()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(string user, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.error = "Mật khẩu và xác nhận mật khẩu không khớp.";
                return View("DangKy");
            }

            string hashedPassword = HashPassword(password);

            using (var db = new QuanLySinhVienDbContext())
            {
                if (db.TaiKhoans.Any(t => t.UserName == user))
                {
                    ViewBag.error = "Tài khoản đã tồn tại.";
                    return View("DangKy");
                }

                TaiKhoan newTaiKhoan = new TaiKhoan
                {
                    UserName = user,
                    PasswordHash = hashedPassword
                };

                db.TaiKhoans.Add(newTaiKhoan);
                db.SaveChanges();
            }

            return RedirectToAction("DangNhap");
        }

        public ActionResult Logout()
        {
            // Clear all session data
            Session.Clear();
            Session.Abandon();

            // Clear all cookies
            if (Request.Cookies != null)
            {
                var cookies = Request.Cookies.AllKeys;
                foreach (var cookie in cookies)
                {
                    Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                }
            }

            // Optionally, you can also add a message to indicate the user has logged out successfully
            TempData["LogoutMessage"] = "You have successfully logged out.";

            return RedirectToAction("Index", "Home");
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
