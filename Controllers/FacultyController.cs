using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoT2.Models;

namespace DemoT2.Controllers
{
    public class FacultyController : Controller
    {
        string username;
        string password;
        string loginname;
        string roleNamesString;
        string tablesString;
        List<string> roleNames;
        List<string> tables;
        // GET: Faculty
        // GET: Faculty
        public ActionResult SinhVien()
        {
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
        public ActionResult MonHoc()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy danh sách quyền của người dùng từ Session hoặc nguồn dữ liệu khác
            var rolePermissionsForTables = GetRolePermissionsForTables();

            // Kiểm tra quyền cho bảng "SinhVien"
            List<bool> monHocPermissions = CheckAvailablePermissionsForTable("MonHoc", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = monHocPermissions[0]; // Index 0 là quyền select

            if (hasSelectPermission)
            {
                // Nếu có quyền select, truy vấn dữ liệu từ cơ sở dữ liệu và hiển thị lên views
                List<MonHoc> MonHocData = GetMonHocData();
                ViewBag.RolePermissionsForTables = rolePermissionsForTables;
                return View(MonHocData);
            }
            else
            {
                // Nếu không có quyền select, trả về một thông báo lỗi
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập trang này.";
                return View("Error");
            }
        }
        public ActionResult GiaoVien()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Lấy danh sách quyền của người dùng từ Session hoặc nguồn dữ liệu khác
            var rolePermissionsForTables = GetRolePermissionsForTables();

            // Kiểm tra quyền cho bảng "SinhVien"
            List<bool> monHocPermissions = CheckAvailablePermissionsForTable("GiaoVien", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = monHocPermissions[0]; // Index 0 là quyền select

            if (hasSelectPermission)
            {
                // Nếu có quyền select, truy vấn dữ liệu từ cơ sở dữ liệu và hiển thị lên views
                List<GiaoVien> GiaoVienData = GetGiaoVienData();
                ViewBag.RolePermissionsForTables = rolePermissionsForTables;
                return View(GiaoVienData);
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
            List<bool> monHocPermissions = CheckAvailablePermissionsForTable("DangKy", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = monHocPermissions[0]; 

            if (hasSelectPermission)
            {
                // Nếu có quyền select, truy vấn dữ liệu từ cơ sở dữ liệu và hiển thị lên views
                List<DangKy> DangKyData = GetDangKyData();
                ViewBag.RolePermissionsForTables = rolePermissionsForTables;
                return View(DangKyData);
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
            List<bool> DangKyPermissions = CheckAvailablePermissionsForTable("Diem", rolePermissionsForTables);

            // Kiểm tra xem có quyền select không
            bool hasSelectPermission = DangKyPermissions[0]; // Index 0 là quyền select

            if (hasSelectPermission)
            {
                // Nếu có quyền select, truy vấn dữ liệu từ cơ sở dữ liệu và hiển thị lên views
                List<SvDiem> DangKyData = GetDiemSoData();
                ViewBag.RolePermissionsForTables = rolePermissionsForTables;
                return View(DangKyData);

                // Trả về view và truyền danh sách yourData
                return View();
            }
            else
            {
                // Nếu không có quyền select, trả về một thông báo lỗi
                ViewBag.ErrorMessage = "Bạn không có quyền truy cập trang này.";
                return View("Error");
            }
        }
        // Phương thức thêm mới một bản ghi vào bảng DangKy
        [HttpPost]
        public ActionResult InsertDangKy(DangKy dangKy)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Kiểm tra quyền INSERT cho bảng DangKy
            if (!CheckPermissionForAction("DangKy", "INSERT"))
            {
                ViewBag.ErrorMessage = "Bạn không có quyền thực hiện thao tác này.";
                return View("Error");
            }

            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (ModelState.IsValid)
            {
                try
                {
                    // Thực hiện thêm mới bản ghi vào bảng DangKy
                    // Ví dụ:
                    using (var dbContext = new QuanLySinhVienDbContext())
                     {
                        dbContext.DangKies.Add(dangKy);
                        dbContext.SaveChanges();
                     }

                    // Sau khi thêm mới thành công, có thể chuyển hướng đến trang khác hoặc hiển thị thông báo thành công
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Xử lý các ngoại lệ và hiển thị thông báo lỗi nếu cần thiết
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi thêm mới bản ghi vào bảng DangKy.";
                    return View("Error");
                }
            }
            else
            {
                // Nếu dữ liệu đầu vào không hợp lệ, hiển thị lại form nhập liệu với thông báo lỗi
                return View(dangKy);
            }

            // Sau khi thêm mới thành công, có thể chuyển hướng đến trang khác hoặc hiển thị thông báo thành công
            return RedirectToAction("Index", "Home");
        }
        // Phương thức kiểm tra cookie để xác định xem người dùng đã đăng nhập hay chưa
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
                SqlCommand command = new SqlCommand("SELECT * FROM SinhVien", sqlConn);
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
        private List<MonHoc> GetMonHocData()
        {
            List<MonHoc> MonHocData = new List<MonHoc>();

            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM MonHoc", sqlConn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    MonHoc monHoc = new MonHoc();
                    monHoc.MaMonHoc = Convert.ToInt32(reader["MaMonHoc"]);
                    monHoc.TenMonHoc = reader["TenMonHoc"].ToString();
                    monHoc.SoTinChi = (int)reader["SoTinChi"];
                    monHoc.Khoa = reader["Khoa"].ToString();

                    MonHocData.Add(monHoc);
                }

                reader.Close();
            }

            return MonHocData;
        }
        private List<GiaoVien> GetGiaoVienData()
        {
            List<GiaoVien> MonHocData = new List<GiaoVien>();

            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                int userID = Convert.ToInt32(Session["UserID"]);
                SqlCommand command = new SqlCommand("SELECT * FROM GiaoVien WHERE MaGiaoVien = @UserID", sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    GiaoVien giaoVien = new GiaoVien();
                    giaoVien.MaGiaoVien = Convert.ToInt32(reader["MaGiaoVien"]);
                    giaoVien.Ho = reader["Ho"].ToString();
                    giaoVien.Ten = reader["Ten"].ToString();
                    giaoVien.Khoa = reader["Khoa"].ToString();

                    MonHocData.Add(giaoVien);
                }

                reader.Close();
            }

            return MonHocData;
        }
        private List<DangKy> GetDangKyData()
        {
            List<DangKy> DangKyData = new List<DangKy>();

            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM DangKy", sqlConn);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DangKy dangKy = new DangKy();
                    dangKy.MaDangKy = Convert.ToInt32(reader["MaDangKy"]);
                    dangKy.MaSinhVien = Convert.ToInt32(reader["MaSinhVien"]);
                    dangKy.MaMonHoc = Convert.ToInt32(reader["MaMonHoc"]);
                    dangKy.NgayDangKy = Convert.ToDateTime(reader["NgayDangKy"]);

                    DangKyData.Add(dangKy);
                }

                reader.Close();
            }

            return DangKyData;
        }
        private List<SvDiem> GetDiemSoData()
        {
            List<SvDiem> DangKyData = new List<SvDiem>();

            string connectionString = Session["ConnectionString"].ToString();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                int userID = Convert.ToInt32(Session["UserID"]);
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
                    Where D.[MaGiaoVien] =@UserID";
                SqlCommand command = new SqlCommand(sqlQuery, sqlConn);
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader reader = command.ExecuteReader();

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

                    DangKyData.Add(model);
                }

                reader.Close();
            }

            return DangKyData;
        }
        // Phương thức kiểm tra quyền cho một hành động cụ thể (INSERT, SELECT, UPDATE, DELETE) trên một bảng cụ thể
        private bool CheckPermissionForAction(string tableName, string action)
        {
            var rolePermissionsForTables = GetRolePermissionsForTables();
            List<bool> permissions = CheckAvailablePermissionsForTable(tableName, rolePermissionsForTables);
            int actionIndex = GetActionIndex(action);
            return permissions[actionIndex];
        }
        // Phương thức lấy index tương ứng với hành động
        private int GetActionIndex(string action)
        {
            switch (action.ToUpper())
            {
                case "INSERT":
                    return 0;
                case "SELECT":
                    return 1;
                case "UPDATE":
                    return 2;
                case "DELETE":
                    return 3;
                default:
                    return -1;
            }
        }
    }
}