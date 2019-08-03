using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace gbkToBmp {
	/// <summary>create gbk font bitmap</summary>
	public class GbkToBmp {
		public void run() {
			try {
				_run();
			} catch (Exception ex) {
				Debug.WriteLine(ex.ToString());
			}
		}

		void _run() {
			const int fontGridSize = 16;
			const int fontSize = 12;
			const int halfGap = (fontGridSize - fontSize) / 2;
			const int gbkRowStart = 0xa1;
			const int gbkRowEnd = 0xf7;
			const int gbkColStart = 0xa1;
			const int gbkColEnd = 0xfe;
			const int rowCount = (gbkRowEnd - gbkRowStart + 1);
			const int colCount = (gbkColEnd - gbkColStart + 1);
			int w = fontGridSize * colCount;
			int h = fontGridSize * rowCount;
			//int w = 16;
			//int h = 16;

			byte[] bgbk = new byte[2];

			Encoding egbk = Encoding.GetEncoding("GBK");

			Font f = new Font("宋体", fontSize, GraphicsUnit.Pixel);
			SolidBrush whiteBrush = new SolidBrush(Color.White);
			//SolidBrush whiteBrush = new SolidBrush(Color.FromArgb(255, 8, 8, 8));
			SolidBrush blackBrush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));

			Rectangle rect = new Rectangle(0, 0, w, h);

			byte[] binData = new byte[w * h];

			using (Bitmap bitmap = new Bitmap(w, h)) {
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.Clear(blackBrush.Color);
				//graphics.DrawString("_", f, whiteBrush, -2, 2);

				//write font to bitmap
				for (int i = gbkRowStart; i <= gbkRowEnd; ++i) {
					if (i >= 0xaa && i <= 0xaf) {
						continue;
					}

					for (int j = gbkColStart; j <= gbkColEnd; ++j) {
						bgbk[0] = (byte)i;
						bgbk[1] = (byte)j;
						
						string str = egbk.GetString(bgbk);

						int x = -2 + (j - gbkColStart) * fontGridSize;
						int y = halfGap + (gbkRowEnd - i) * fontGridSize;
						graphics.DrawString(str, f, whiteBrush, x, y);
					}
				}

				byte[] bmpTemp = new byte[w*h*4];
				BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
				Marshal.Copy(bmpData.Scan0, bmpTemp, 0, bmpTemp.Length);
				bitmap.UnlockBits(bmpData);

				//translate bitmap data to bin data
				for (int i = gbkRowStart; i <= gbkRowEnd; ++i) {
					for (int j = gbkColStart; j <= gbkColEnd; ++j) {
						for (int r = 0; r < fontGridSize; ++r) {
							for (int c = 0; c < fontGridSize; ++c) {
								int iStart = i - gbkRowStart;
								int jStart = j - gbkColStart;

								int tmpIdx = ((rowCount - iStart - 1) * fontGridSize + r) * (colCount * fontGridSize * 4) + (jStart * fontGridSize + c) * 4;

								int binIdx = (iStart * colCount + jStart) * (fontGridSize * fontGridSize) + (r * fontGridSize + c);

								int tmpData = bmpTemp[tmpIdx] * 8 / 255;
								binData[binIdx] = (byte)(tmpData > 8 ? 8 : tmpData);
							}
						}
					}
				}

				// save to bin file
				using (FileStream fs = new FileStream("gbk.bin", FileMode.OpenOrCreate, FileAccess.Write))
				using (BinaryWriter bw = new BinaryWriter(fs)) {
					bw.Write(binData);
				}

				//save to bmp file
				//bitmap.Save("aa.bmp", ImageFormat.Bmp);
				bitmap.Save("gbk.bmp", ImageFormat.Bmp);
			}

		}
		
	}
}
