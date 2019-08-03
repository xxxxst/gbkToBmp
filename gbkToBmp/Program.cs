using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gbkToBmp {
	class Program {
		static void Main(string[] args) {
			// create gbk font bitmap
			new GbkToBmp().run();

			// create map of utf8 to gbk
			//new Utf8ToGbk().run();
		}
	}
}
