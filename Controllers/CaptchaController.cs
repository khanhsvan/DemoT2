using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoT2.Controllers
{
    // GET: CapCha
    public class CaptchaController : Controller
    {
        public ActionResult TaoCaptCha()
        {
            string captchaCode = GenerateRandomCode(6); // Tạo mã Captcha ngẫu nhiên
            Session["Captcha"] = captchaCode; // Lưu mã Captcha vào Session

            using (MemoryStream mem = new MemoryStream())
            {
                using (Bitmap bitmap = new Bitmap(130, 36))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.White);
                        g.DrawString(captchaCode, new Font("Arial", 16), Brushes.Black, new PointF(10, 3));

                        // Lưu hình ảnh Captcha vào MemoryStream
                        bitmap.Save(mem, ImageFormat.Png);
                        byte[] img = mem.ToArray();

                        return File(img, "image/png"); // Trả về hình ảnh Captcha
                    }
                }
            }
        }

        // Hàm tạo mã Captcha ngẫu nhiên
        private string GenerateRandomCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] captchaChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                captchaChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(captchaChars);
        }
    }
}