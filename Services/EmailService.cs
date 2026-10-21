// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Gui email xac nhan, cap nhat trang thai, hoan hang, dat lai mat khau

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> GuiEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrEmpty(_config["EmailSettings:SenderEmail"]) ||
                    _config["EmailSettings:SenderEmail"] == "your-email@gmail.com")
                {
                    _logger.LogWarning("Chua cau hinh EmailSettings. Bo qua gui email.");
                    return false;
                }

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _config["EmailSettings:SenderName"] ?? "UNETI Store",
                    _config["EmailSettings:SenderEmail"]));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart("html") { Text = htmlBody };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _config["EmailSettings:SmtpServer"],
                    int.Parse(_config["EmailSettings:SmtpPort"]!),
                    SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(
                    _config["EmailSettings:SenderEmail"],
                    _config["EmailSettings:SenderPassword"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Đã gửi email đến {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email đến {To}", to);
                return false;
            }
        }

        // ============== ĐẶT HÀNG THÀNH CÔNG ==============
        public async Task GuiEmailDatHangThanhCong(DonHang dh, string emailKhach)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:auto;border:1px solid #ddd;border-radius:8px;overflow:hidden'>
                    <div style='background:#0d6efd;color:white;padding:20px;text-align:center'>
                        <h2>🎉 Đặt hàng thành công!</h2>
                    </div>
                    <div style='padding:20px'>
                        <p>Xin chào <b>{dh.KhachHang?.HoTen}</b>,</p>
                        <p>Cảm ơn bạn đã đặt hàng tại <b>Cửa hàng điện tử UNETI</b>.</p>
                        <table style='width:100%;border-collapse:collapse;margin:15px 0'>
                            <tr><td style='padding:8px;border-bottom:1px solid #eee'><b>Mã đơn:</b></td><td style='padding:8px;border-bottom:1px solid #eee'>#{dh.MaDonHang}</td></tr>
                            <tr><td style='padding:8px;border-bottom:1px solid #eee'><b>Ngày đặt:</b></td><td style='padding:8px;border-bottom:1px solid #eee'>{dh.NgayDat:dd/MM/yyyy HH:mm}</td></tr>
                            <tr><td style='padding:8px;border-bottom:1px solid #eee'><b>Địa chỉ giao:</b></td><td style='padding:8px;border-bottom:1px solid #eee'>{dh.DiaChiGiaoHang}</td></tr>
                            <tr><td style='padding:8px;background:#fff3cd'><b>Tổng thanh toán:</b></td><td style='padding:8px;background:#fff3cd;color:red;font-size:18px'><b>{dh.TongTien:N0} đ</b></td></tr>
                        </table>
                        <p>Trân trọng,<br><b>Cửa hàng điện tử UNETI</b></p>
                    </div>
                </div>";
            await GuiEmailAsync(emailKhach, $"Xác nhận đơn hàng #{dh.MaDonHang}", body);
        }

        // ============== CẬP NHẬT TRẠNG THÁI ==============
        public async Task GuiEmailCapNhatTrangThai(DonHang dh, string emailKhach, string trangThaiMoi)
        {
            var (tieuDe, mau, thongDiep) = trangThaiMoi switch
            {
                "DaXacNhan" => ("✅ Đơn hàng đã được xác nhận", "#17a2b8", "Đơn hàng đang chuẩn bị giao."),
                "DangGiao" => ("🚚 Đơn hàng đang được giao", "#007bff", "Đơn hàng đang trên đường đến bạn."),
                "DaHoanThanh" => ("🎉 Đơn hàng đã hoàn thành", "#28a745", "Đơn hàng đã giao thành công."),
                "DaHuy" => ("❌ Đơn hàng đã bị hủy", "#dc3545", "Đơn hàng đã bị hủy."),
                _ => ("Cập nhật đơn hàng", "#6c757d", "Đơn hàng vừa được cập nhật.")
            };
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:auto;border:1px solid #ddd;border-radius:8px;overflow:hidden'>
                    <div style='background:{mau};color:white;padding:20px;text-align:center'><h2>{tieuDe}</h2></div>
                    <div style='padding:20px'>
                        <p>Xin chào <b>{dh.KhachHang?.HoTen}</b>,</p>
                        <p>{thongDiep}</p>
                        <p><b>Mã đơn:</b> #{dh.MaDonHang}</p>
                        <p><b>Tổng tiền:</b> <span style='color:red'>{dh.TongTien:N0} đ</span></p>
                        <p>Trân trọng,<br><b>Cửa hàng điện tử UNETI</b></p>
                    </div>
                </div>";
            await GuiEmailAsync(emailKhach, $"[Đơn #{dh.MaDonHang}] {tieuDe}", body);
        }

        // ============== HOÀN HÀNG ==============
        public async Task GuiEmailHoanHang(YeuCauHoanHang yc, string emailKhach)
        {
            var (mau, tieuDe) = yc.TrangThai switch
            {
                "DaDuyet" => ("#28a745", "Yêu cầu hoàn hàng đã được chấp nhận"),
                "TuChoi" => ("#dc3545", "Yêu cầu hoàn hàng bị từ chối"),
                "DaHoanTien" => ("#fd7e14", "Đã hoàn tiền cho đơn hàng"),
                _ => ("#6c757d", "Cập nhật yêu cầu hoàn hàng")
            };
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:auto;border:1px solid #ddd;border-radius:8px;overflow:hidden'>
                    <div style='background:{mau};color:white;padding:20px;text-align:center'><h2>{tieuDe}</h2></div>
                    <div style='padding:20px'>
                        <p>Xin chào <b>{yc.DonHang?.KhachHang?.HoTen}</b>,</p>
                        <p>Yêu cầu hoàn hàng cho đơn <b>#{yc.MaDonHang}</b> đã được xử lý.</p>
                        <p><b>Số tiền hoàn:</b> <span style='color:red;font-size:18px'>{yc.SoTienHoan:N0} đ</span></p>
                        <p>Trân trọng,<br><b>Cửa hàng điện tử UNETI</b></p>
                    </div>
                </div>";
            await GuiEmailAsync(emailKhach, $"Hoàn hàng đơn #{yc.MaDonHang}", body);
        }

        // ============== ĐẶT LẠI MẬT KHẨU ==============
        public async Task GuiEmailDatLaiMatKhau(string to, string hoTen, string linkDatLai)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:auto;border:1px solid #ddd;border-radius:8px;overflow:hidden'>
                    <div style='background:#dc3545;color:white;padding:20px;text-align:center'><h2>🔐 Đặt lại mật khẩu</h2></div>
                    <div style='padding:20px'>
                        <p>Xin chào <b>{hoTen}</b>,</p>
                        <p>Bạn đã yêu cầu đặt lại mật khẩu tại <b>Cửa hàng điện tử UNETI</b>.</p>
                        <p>Nhấn nút bên dưới để đặt lại (hết hạn sau 15 phút):</p>
                        <div style='text-align:center;margin:25px 0'>
                            <a href='{linkDatLai}' style='background:#dc3545;color:white;padding:12px 30px;text-decoration:none;border-radius:6px;font-size:16px'>Đặt lại mật khẩu</a>
                        </div>
                        <p style='color:#6c757d;font-size:13px'>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                        <p>Trân trọng,<br><b>Cửa hàng điện tử UNETI</b></p>
                    </div>
                </div>";
            await GuiEmailAsync(to, "Đặt lại mật khẩu - UNETI Store", body);
        }
    }
}