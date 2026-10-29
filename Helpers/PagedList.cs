// ============================================================================
// DỰ ÁN: QUẢN LÝ CỬA HÀNG ĐIỆN TỬ - UNETI (NHÓM 07 - DHTI17A1ND)
// PHÂN HỆ: PHẦN CHUNG (FOUNDATION & SHARED INFRASTRUCTURE)
// NHÓM SINH VIÊN THỰC HIỆN:
//   1. Đỗ Văn Hiếu    - 23203100018 - DHTI17A1ND - dvhieu.dhti17a1nd@sv.uneti.edu.vn (Trưởng nhóm)
//   2. Lưu Đức Huy    - 23274800002 - DHTI17A2ND - ldhuy.dhti17a2nd@sv.uneti.edu.vn
//   3. Vương Quang Đạt - 23203100036 - DHTI17A1ND - vqdat.dhti17a1nd@sv.uneti.edu.vn
//   4. Phạm Thái Hoàng - 23203100039 - DHTI17A1ND - pthoang.dhti17a1nd@sv.uneti.edu.vn
// NỘI DUNG: Cấu trúc dữ liệu phân trang generic PagedList<T> hỗ trợ truy vấn tối ưu cơ sở dữ liệu
// ============================================================================

using Microsoft.EntityFrameworkCore;

namespace QuanLyCuaHangDienTu.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }
        public int PageSize { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalItems = count;
            PageSize = pageSize;
            AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var count = await source.CountAsync();
            var items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}