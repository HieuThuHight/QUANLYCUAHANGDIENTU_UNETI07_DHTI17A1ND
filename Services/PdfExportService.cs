// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Xuat PDF hoa don

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Services
{
    public class PdfExportService
    {
        public byte[] XuatHoaDon(DonHang dh)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text($"HÓA ĐƠN #{dh.MaDonHang}")
                        .FontSize(20).Bold().AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Khách hàng: {dh.KhachHang?.HoTen}");
                        col.Item().Text($"Ngày đặt: {dh.NgayDat:dd/MM/yyyy HH:mm}");
                        col.Item().Text($"Địa chỉ: {dh.DiaChiGiaoHang}");
                        col.Item().PaddingTop(10);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.ConstantColumn(50);
                                c.ConstantColumn(100);
                                c.ConstantColumn(110);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Grey.Lighten2).Text("Sản phẩm").Bold();
                                h.Cell().Background(Colors.Grey.Lighten2).Text("SL").Bold();
                                h.Cell().Background(Colors.Grey.Lighten2).Text("Đơn giá").Bold();
                                h.Cell().Background(Colors.Grey.Lighten2).Text("Thành tiền").Bold();
                            });

                            foreach (var ct in dh.ChiTietDonHangs)
                            {
                                table.Cell().Text(ct.SanPham?.TenSanPham ?? "");
                                table.Cell().Text(ct.SoLuong.ToString());
                                table.Cell().Text(ct.DonGia.ToString("N0"));
                                table.Cell().Text(ct.ThanhTien.ToString("N0"));
                            }
                        });

                        col.Item().PaddingTop(10).AlignRight()
                            .Text($"Tổng: {dh.TongTien:N0} đ").Bold().FontSize(14);
                    });

                    page.Footer().AlignCenter()
                        .Text("Cảm ơn quý khách đã mua hàng tại UNETI Store!");
                });
            }).GeneratePdf();
        }
    }
}