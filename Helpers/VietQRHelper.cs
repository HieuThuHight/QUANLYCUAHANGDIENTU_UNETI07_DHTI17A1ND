namespace QuanLyCuaHangDienTu.Helpers
{
    public static class VietQRHelper
    {
        public static string GeneratePayload(string bin, string stk, long amount, string memo)
        {
            var f00 = "000201";
            var f01 = "010212"; // 12 = Dynamic QR (có số tiền động)
            
            // Tag 38 (Merchant Account Info - Napas)
            var sub00 = "0010A000000727";
            var sub01Val = "0006" + bin + "01" + stk.Length.ToString("D2") + stk;
            var sub01 = "01" + sub01Val.Length.ToString("D2") + sub01Val;
            var sub02 = "0208QRIBFTTA";
            var f38Val = sub00 + sub01 + sub02;
            var f38 = "38" + f38Val.Length.ToString("D2") + f38Val;
            
            var f53 = "5303704"; // Tiền tệ VND
            var f54 = "54" + amount.ToString().Length.ToString("D2") + amount;
            var f58 = "5802VN";
            
            var f62 = "";
            if (!string.IsNullOrEmpty(memo))
            {
                var sub08 = "08" + memo.Length.ToString("D2") + memo;
                f62 = "62" + sub08.Length.ToString("D2") + sub08;
            }
            
            var raw = f00 + f01 + f38 + f53 + f54 + f58 + f62 + "6304";
            var crc = ComputeCRC16(raw);
            return raw + crc;
        }

        private static string ComputeCRC16(string data)
        {
            ushort crc = 0xFFFF;
            foreach (var c in data)
            {
                crc ^= (ushort)((byte)c << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort)(crc << 1);
                }
            }
            return crc.ToString("X4");
        }
    }
}
