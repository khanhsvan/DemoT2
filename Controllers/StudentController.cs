using DemoT2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoT2.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult SinhVien()
        {
            string connectionString = Session["ConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(connectionString);
                sqlConn.Open();
                // Truy vấn danh sách các bảng có thể thao tác
                SqlCommand command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", sqlConn);

                SqlDataReader reader = command.ExecuteReader();
                List<string> tables = new List<string>();
                while (reader.Read())
                {
                    tables.Add(reader["TABLE_NAME"].ToString());
                }
                reader.Close();
                // Xử lý danh sách các bảng
                
            ViewBag.Tables = tables;
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy danh sách quyền của người dùng từ Session hoặc nguồn dữ liệu khác
            var rolePermissionsForTables = GetRolePermissionsForTables();

            // Kiểm tra quyền cho bảng "SinhVien"
            List<bool> sinhVienPermissions = CheckAvailablePermissionsForTable("SinhVien", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = sinhVienPermissions[0]; // Index 0 là quyền select

            if (hasSelectPermission)
            {
                // Nếu có quyền select, truy vấn dữ liệu từ cơ sở dữ liệu và hiển thị lên views
                List<SinhVien> sinhVienData = GetSinhVienData();
                ViewBag.RolePermissionsForTables = rolePermissionsForTables;
                return View(sinhVienData);
            }
            else
            {
                // Nếu không có quyền select, trả về một thông báo lỗi
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập trang này.";
                return View("Error");
            }
        }
        public ActionResult DangKy()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            // Lấy danh sách quyền của người dùng từ Session hoặc nguồn dữ liệu khác
            var rolePermissionsForTables = GetRolePermissionsForTables();

            // Kiểm tra quyền cho bảng "SinhVien"
            List<bool> DangKyPermissions = CheckAvailablePermissionsForTable("DangKy", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = DangKyPermissions[0]; // Index 0 là quyền select

            if (hasSelectPermission)
            {
                // Nếu có quyền select, truy vấn dữ liệu từ cơ sở dữ liệu và hiển thị lên views
                List<svDkModel> sinhVienData = GetMonHocData();
                ViewBag.RolePermissionsForTables = rolePermissionsForTables;
                return View(sinhVienData);
            }
            else
            {
                // Nếu không có quyền select, trả về một thông báo lỗi
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập trang này.";
                return View("Error");
            }
        }
        public ActionResult Diem()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            // Lấy danh sách quyền của người dùng từ Session hoặc nguồn dữ liệu khác
            var rolePermissionsForTables = GetRolePermissionsForTables();

            // Kiểm tra quyền cho bảng "SinhVien"
            List<bool> DangKyPermissions = CheckAvailablePermissionsForTable("DangKy", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = DangKyPermissions[0]; // Index 0 là quyền select

            if (hasSelectPermission)
            {
                List<SvDiem> yourData = new List<SvDiem>();

                // Thay đổi chuỗi kết nối cho phù hợp
                string connectionString = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString; ;
                int userID = Convert.ToInt32(Session["UserID"]);
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();

                    // Thực hiện truy vấn SQL
                    string sqlQuery = @"
                    SELECT 
                        D.[MaDangKy],
                        D.[MaGiaoVien],
                        D.[DiemSo],
                        D.[NgayNhapDiem],
                        MH.[TenMonHoc],
                        MH.[SoTinChi],
                        GV.[Ho] + ' ' + GV.[Ten] AS [TenGiaoVien]
                    FROM 
                        [QuanLySinhVien].[dbo].[Diem] D
                    JOIN
                        [QuanLySinhVien].[dbo].[DangKy] DK ON D.[MaDangKy] = DK.[MaDangKy]
                    JOIN
                        [QuanLySinhVien].[dbo].[MonHoc] MH ON DK.[MaMonHoc] = MH.[MaMonHoc]
                    LEFT JOIN
                        [QuanLySinhVien].[dbo].[GiaoVien] GV ON D.[MaGiaoVien] = GV.[MaGiaoVien]
                    Where D.[MaGiaoVien] = @UserID";

                    SqlCommand command = new SqlCommand(sqlQuery, sqlConn);
                    command.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader reader = command.ExecuteReader();

                    // Đọc dữ liệu từ SqlDataReader và thêm vào danh sách yourData
                    while (reader.Read())
                    {
                        SvDiem model = new SvDiem();
                        // Thực hiện việc gán dữ liệu từ SqlDataReader vào model của bạn
                        model.MaDangKy = Convert.ToInt32(reader["MaDangKy"]);
                        model.MaGiaoVien = Convert.ToInt32(reader["MaGiaoVien"]);
                        model.DiemSo = Convert.ToDouble(reader["DiemSo"]);
                        model.NgayNhapDiem = Convert.ToDateTime(reader["NgayNhapDiem"]);
                        model.TenMonHoc = reader["TenMonHoc"].ToString();
                        model.SoTinChi = Convert.ToInt32(reader["SoTinChi"]);
                        model.TenGiaoVien = reader["TenGiaoVien"].ToString();

                        yourData.Add(model);
                    }

                    reader.Close();
                }

                // Trả về view và truyền danh sách yourData
                return View(yourData);
            }
            else
            {
                // Nếu không có quyền select, trả về một thông báo lỗi
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập trang này.";
                return View("Error");
            }
        }
        private bool IsUserLoggedIn()
        {
            // Kiểm tra xem session "LoggedInUserInfo" có tồn tại hay không
            if (Session["LoggedInUserInfo"] != null)
            {
                // Session tồn tại, người dùng đã đăng nhập
                return true;
            }

            // Nếu session không tồn tại, hoặc thông tin đăng nhập không hợp lệ, trả về false
            return false;
        }
        // Phương thức truy vấn quyền của role đối với từng bảng và quyền thêm, sửa, xóa (CRUD)
        private Dictionary<string, List<string>> GetRolePermissionsForTables()
        {
            Dictionary<string, List<string>> rolePermissionsForTable = new Dictionary<string, List<string>>();

            // Kết nối đến cơ sở dữ liệu
            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                // Ép kiểu dữ liệu từ session thành kiểu dữ liệu mong muốn
                UserSessionInfo loggedInUserInfo = (UserSessionInfo)Session["LoggedInUserInfo"];

                // Sử dụng thông tin từ session
                string userName = loggedInUserInfo.UserName;
                string password = loggedInUserInfo.Password;
                string loginName = loggedInUserInfo.LoginName;
                List<string> roleNames = loggedInUserInfo.RoleNames;
                List<string> tables = loggedInUserInfo.Tables;
                // Truy vấn quyền của role đối với từng bảng
                foreach (var table in tables)
                {
                    List<string> permissionsForTable = new List<string>();

                    // Truy vấn quyền của mỗi role đối với bảng hiện tại
                    foreach (var roleName in roleNames)
                    {
                        SqlCommand command = new SqlCommand("SELECT permission_name FROM sys.database_permissions WHERE grantee_principal_id IN (SELECT principal_id FROM sys.database_principals WHERE name = @RoleName) AND major_id = OBJECT_ID(@TableName)", sqlConn);
                        command.Parameters.AddWithValue("@RoleName", roleName);
                        command.Parameters.AddWithValue("@TableName", table);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            permissionsForTable.Add(reader["permission_name"].ToString()); // Thêm quyền vào danh sách quyền của bảng
                        }
                        reader.Close();
                    }

                    // Lưu danh sách quyền của bảng vào Dictionary
                    rolePermissionsForTable[table] = permissionsForTable;
                }

                sqlConn.Close();
            }

            return rolePermissionsForTable;
        }
        private List<bool> CheckAvailablePermissionsForTable(string tableName, Dictionary<string, List<string>> rolePermissionsForTable)
        {
            List<bool> availablePermissions = new List<bool>();

            // Kiểm tra xem tên bảng có tồn tại trong rolePermissionsForTable không
            if (rolePermissionsForTable.ContainsKey(tableName))
            {
                // Lấy danh sách quyền của bảng từ rolePermissionsForTable
                List<string> permissions = rolePermissionsForTable[tableName];

                // Kiểm tra sự tồn tại của quyền select, edit, update, delete trong danh sách permissions
                bool hasSelectPermission = permissions.Contains("select", StringComparer.OrdinalIgnoreCase);
                bool hasEditPermission = permissions.Contains("edit", StringComparer.OrdinalIgnoreCase);
                bool hasUpdatePermission = permissions.Contains("update", StringComparer.OrdinalIgnoreCase);
                bool hasDeletePermission = permissions.Contains("delete", StringComparer.OrdinalIgnoreCase);

                // Thêm kết quả kiểm tra vào danh sách availablePermissions
                availablePermissions.Add(hasSelectPermission);
                availablePermissions.Add(hasEditPermission);
                availablePermissions.Add(hasUpdatePermission);
                availablePermissions.Add(hasDeletePermission);
            }
            else
            {
                // Nếu bảng không tồn tại trong rolePermissionsForTable, gán giá trị false cho tất cả quyền
                availablePermissions.AddRange(new bool[] { false, false, false, false });
            }

            return availablePermissions;
        }

        // Phương thức truy vấn dữ liệu sinh viên từ cơ sở dữ liệu
        private List<SinhVien> GetSinhVienData()
        {
            List<SinhVien> sinhVienData = new List<SinhVien>();

            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                int userID = Convert.ToInt32(Session["UserID"]);
                SqlCommand command = new SqlCommand("SELECT * FROM SinhVien WHERE MaSinhVien = @UserID", sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    SinhVien sinhVien = new SinhVien();
                    sinhVien.MaSinhVien = Convert.ToInt32(reader["MaSinhVien"]);
                    sinhVien.Ho = reader["Ho"].ToString();
                    sinhVien.Ten = reader["Ten"].ToString();
                    sinhVien.NgaySinh = Convert.ToDateTime(reader["NgaySinh"]);
                    sinhVien.GioiTinh = reader["GioiTinh"].ToString();
                    sinhVien.DiaChi = reader["DiaChi"].ToString();

                    sinhVienData.Add(sinhVien);
                }

                reader.Close();
            }

            return sinhVienData;
        }
        private List<svDkModel> GetMonHocData() { 
            List<svDkModel> sinhVienData = new List<svDkModel>();

            string connectionString = Session["ConnectionString"].ToString();
            string DangNhapConn = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

            using (SqlConnection sqlConnection = new SqlConnection(DangNhapConn))
            {
                sqlConnection.Open();
                int userID = Convert.ToInt32(Session["UserID"]);

                SqlCommand command = new SqlCommand("SELECT DK.[MaDangKy], DK.[MaSinhVien], DK.[MaMonHoc], DK.[NgayDangKy], MH.[TenMonHoc] AS[TenMonHoc] FROM [QuanLySinhVien].[dbo].[DangKy] AS DK JOIN [QuanLySinhVien].[dbo].[MonHoc] AS MH ON DK.[MaMonHoc] = MH.[MaMonHoc] WHERE DK.[MaSinhVien] = @UserID", sqlConnection);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    svDkModel sinhVien = new svDkModel();
                    sinhVien.MaSinhVien = Convert.ToInt32(reader["MaSinhVien"]);
                    sinhVien.MaMonHoc = Convert.ToInt32(reader["MaMonHoc"]);
                    sinhVien.MaDangKy = Convert.ToInt32(reader["MaDangKy"]);
                    sinhVien.NgayDangKy = Convert.ToDateTime(reader["NgayDangKy"]);
                    sinhVien.TenMonHoc = reader["TenMonHoc"].ToString();

                    sinhVienData.Add(sinhVien);
                }

                reader.Close();
            }

            return sinhVienData;
        }
    }
}