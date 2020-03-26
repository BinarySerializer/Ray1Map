using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine {
	public class PS1_VRAM {
		const int page_height = 256;
		const int page_width = 128; // for 8-bit CLUT.
		public int skippedPagesX = 0;
		public Page[][] pages = new Page[2][]; // y, x
		public int currentXPage = 0;
		public int currentYPage = 0;
		public int nextYInPage = 0;

		public void AddData(byte[] data, int width) {
			if (data == null) return;
			/*if (pages[currentYPage] == null || pages[currentYPage].Length == 0) {
				pages[currentYPage] = new Page[currentXPage + (width / page_width)];
			}
			if (pages[currentYPage].Length < currentXPage + (width / page_width)) {
				Array.Resize(ref pages[currentYPage], currentXPage + (width / page_width));
			}*/
			int height = data.Length / width + ((data.Length % width != 0) ? 1 : 0);
			if (height > 0 && width > 0) {
				int xInPage = 0, yInPage = 0;
				int curPageX = currentXPage, curPageY = currentYPage, curStartPageX = currentXPage;
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						curPageX = currentXPage + (x / page_width);
						curStartPageX = currentXPage;
						curPageY = currentYPage + ((nextYInPage + y) / page_height);
						xInPage = x % page_width;
						yInPage = (nextYInPage + y) % page_height;
						while (curPageY > 1) {
							// Wrap
							curPageY -= 2;
							curPageX += (width / page_width);
							curStartPageX += (width / page_width);
						}
						if (pages[curPageY] == null || pages[curPageY].Length == 0) {
							pages[curPageY] = new Page[curPageX];
						}
						if (pages[curPageY].Length < curPageX + 1) {
							Array.Resize(ref pages[curPageY], curPageX + 1);
						}
						if (pages[curPageY][curPageX] == null) {
							//UnityEngine.Debug.Log("Created page " + curPageX + "," + curPageY);
							pages[curPageY][curPageX] = new Page();
						}
						pages[curPageY][curPageX].SetByte(xInPage, yInPage, data[y * width + x]);
					}
				}
				currentXPage = curStartPageX;
				currentYPage = curPageY;


				nextYInPage = yInPage + 1;
				if (nextYInPage >= page_height) {
					// Change page
					nextYInPage -= page_height;
					currentYPage++;
					if (currentYPage > 1) {
						//  Wrap
						currentXPage += (width / page_width);
						currentYPage -= 2;
					}
				}
			}
			//UnityEngine.Debug.Log(currentXPage + " - " + currentYPage + " - " + curPageX + " - " + ((width / page_width) - 1));
			/*if (data != null) {
				if (pages.Count == 0) {
					pages.Add(new Page(pageWidth));
				}
				byte[] newData = pages.Last().AddData(data);
				while (newData != null) {
					pages.Add(new Page(pageWidth));
					newData = pages.Last().AddData(data);
				}
			}*/
		}

		private Page GetPage(int x, int y) {
			try {
				if (x >= pages[y].Length) return null;
				return pages[y][x];
			} catch (Exception) {
				UnityEngine.Debug.LogError(x + " - " + y);
				throw;
			}
		}

		public byte GetPixel(int pageX, int pageY, int x, int y) {
			//UnityEngine.Debug.Log(pageX + " - " + pageY + " - " + x + " - " + y);
			pageX -= skippedPagesX; // We're not loading backgrounds for now
			Page page = GetPage(pageX, pageY);
			while (x >= page_width) {
				pageX++;
				x -= page_width;
				page = GetPage(pageX, pageY);
			}
			//UnityEngine.Debug.Log(pageX + " - " + pageY + " - " + x + " - " + y);
			if (page == null) return 0;
			return page.GetByte(x, y);
		}

		public class Page {
			public byte[] data = new byte[page_width * page_height];
			public byte GetByte(int x, int y) {
				return data[y * page_width + x];
			}
			public void SetByte(int x, int y, byte value) {
				data[y * page_width + x] = value;
			}
			/*public int width;
			public byte[] data;

			public Page(int width) {
				this.width = width;
			}

			public byte GetByte(int x, int y) {
				return data[y * width + x];
			}

			// We assume that page data is 8bit colors
			public byte[] AddData(byte[] data) {
				int curDataCount = data?.Length ?? 0;
				int newDataCount = Math.Min(curDataCount + data.Length, width * 256);
				int dataToCopy = newDataCount - curDataCount;
				if (dataToCopy > 0) {
					if (this.data == null) {
						this.data = new byte[newDataCount];
					} else {
						Array.Resize(ref this.data, newDataCount);
					}
					Array.Copy(data, 0, this.data, curDataCount, dataToCopy);
					if (data.Length > dataToCopy) {
						byte[] newData = new byte[data.Length - dataToCopy];
						Array.Copy(data, dataToCopy, newData, 0, newData.Length);
						return newData;
					} else {
						return null;
					}
				}
				return data;
			}*/
		}
	}
}
