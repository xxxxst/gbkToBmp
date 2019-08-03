using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gbkToBmp {
	/// <summary>create map of utf8 to gbk</summary>
	public class Utf8ToGbk {
		public void run() {
			Encoding egbk = Encoding.GetEncoding("GBK");

			int gbkRowStart = 0xa1;
			int gbkRowEnd = 0xf7;
			int gbkColStart = 0xa1;
			int gbkColEnd = 0xfe;

			byte[] bgbk = new byte[2];
			
			Dictionary<int, int> mapUtf8ToGbk = new Dictionary<int, int>();
			for (int r = gbkRowStart; r <= gbkRowEnd; ++r) {
				for (int c = gbkColStart; c <= gbkColEnd; ++c) {
					bgbk[0] = (byte)r;
					bgbk[1] = (byte)c;
					int iGbk = ((r << 8) | c);

					string str = egbk.GetString(bgbk);
					byte[] bUtf8 = Encoding.UTF8.GetBytes(str);
					if (bUtf8.Length != 3) {
						continue;
					}

					int sUtf8 = ((bUtf8[0] & 0xF) << 12)
						| ((bUtf8[1] & 0x3f) << 6)
						| ((bUtf8[2] & 0x3f));

					mapUtf8ToGbk[sUtf8] = iGbk;
				}
			}

			List<int> lstUtf8 = mapUtf8ToGbk.Keys.ToList();
			lstUtf8.Sort();

			string strTest = $"const uint16 arrUtf8ToGbkCount = {lstUtf8.Count};\r\n";
				strTest += "const uint16 arrUtf8ToGbk[] = {\r\n";

			for (int i = 0; i < lstUtf8.Count; ++i) {
				strTest += $"	0x{lstUtf8[i].ToString("X")}, 0x{mapUtf8ToGbk[lstUtf8[i]].ToString("X")},\r\n";
			}
			strTest += "};";
			
			Debug.WriteLine(strTest);
		}
	}
}
