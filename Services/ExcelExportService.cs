// Họ và tên: Lưu Đức Huy
// Mã sinh viên: 23274800002
// Nội dung thực hiện: Xuat Excel

using ClosedXML.Excel;
using QuanLyCuaHangDienTu.Models.Entities;

namespace QuanLyCuaHangDienTu.Services
{
    public class ExcelExportService
    {
        public byte[] ExportSanPham(List<SanPham> list)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SanPham");

            ws.Cell(1, 1).Value = "Mã";
            ws.Cell(1, 2).Value = "Tên sản phẩm";
            ws.Cell(1, 3).Value = "Thương hiệu";
            ws.Cell(1, 4).Value = "Đơn giá";
            ws.Cell(1, 5).Value = "Tồn";

            var header = ws.Range(1, 1, 1, 5);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            for (int i = 0; i < list.Count; i++)
            {
                var sp = list[i];
                ws.Cell(i + 2, 1).Value = sp.MaSanPham;
                ws.Cell(i + 2, 2).Value = sp.TenSanPham;
                ws.Cell(i + 2, 3).Value = sp.ThuongHieu;
                ws.Cell(i + 2, 4).Value = sp.DonGia;
                ws.Cell(i + 2, 5).Value = sp.SoLuongTon;
            }
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}