using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using DemoT2.Models;

namespace DemoT2.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (Session["LoggedInUser"] == null && Request.Cookies["UserLogin"] != null)
            {
                string username = Request.Cookies["UserLogin"].Values["Username"];
                string password = Request.Cookies["UserLogin"].Values["Password"];

                string DangNhapConn = ConfigurationManager.ConnectionStrings["DangNhap"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(DangNhapConn))
                {
                    string query = @"
                        SELECT tk.taikhoan, tk.matkhau, tk.hoten, tk.sodienthoai, tk.ngaysinh, tk.email, r.role_name 
                        FROM TaiKhoan tk
                        JOIN Role r ON tk.role_id = r.role_id
                        WHERE tk.taikhoan = @username AND tk.matkhau = @password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
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
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Log the error or handle it as needed
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
