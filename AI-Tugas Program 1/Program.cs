using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace AI_Tugas_Program_1
{

	internal class Program
	{

		// Legend:
		// Tabel -> Label_Tabel, x, y
		// Relasi -> Label_Relasi, Tabel 1, Tabel 2

		// TODO
		// 1. Input user untuk N tabel
		// 2. Buat generate tabel (jadiin .xls)
		// 3. Buat read tabel dari file .xls
		// 4. Buat class Kordinat
		// 5. Buat generate relasi (jadiin .xls)
		// 6. Method search ( A*, Greedy )

		class Kordinat
		{
			public Tabel Tabel { get; set; }
			public int X { get; set; }
			public int Y { get; set; }

			public Kordinat(Tabel tabel, int x, int y)
			{
				Tabel = tabel;
				X = x;
				Y = y;
			}
		}

		class Tabel
		{
			private static Random random = new Random();

			public string Nama { get; set; }
			public Kordinat Kordinat { get; set; }
			public List<Tabel> Relasi { get; set; }
			public int MaxRelasi { get; set; }

			public Tabel(string nama, int x, int y)
			{
				Nama = nama;
				Kordinat = new Kordinat(this, x, y);
				Relasi = new List<Tabel>();
				MaxRelasi = random.Next(3, 5);
			}

			public double Jarak(Tabel other)
			{
				// Jarak 2 titik: sqrt( (x2 - x1)**2 + (y2 - y1)**2 )
				double result = 0;

				int deltaX = other.Kordinat.X - Kordinat.X;
				int deltaY = other.Kordinat.Y - Kordinat.Y;

				result = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));

				return result;
			}

			public override string ToString()
			{
				string relasi = "";
				string space = "";
				foreach (Tabel other in Relasi)
				{
					relasi += space + other.Nama;
					space = ", ";
				}

				return $"[{Nama}: ({Kordinat.X}, {Kordinat.Y}), Relasi: ({relasi})]";
			}
		}

		class Relasi
		{
			public string Label { get; set; }
			public Tabel Tabel1 { get; set; }
			public Tabel Tabel2 { get; set; }

			public Relasi(Tabel tabel1, Tabel tabel2)
			{
				Tabel1 = tabel1;
				Tabel2 = tabel2;

				string[] split1 = tabel1.Nama.Split("_"), split2 = tabel2.Nama.Split("_");
				Label = $"Attribute_{split1[1]}_{split2[1]}";
			}

			public override string ToString()
			{
				return $"[{Label}: ({Tabel1.Nama}, {Tabel2.Nama})]";
			}
		}

		// MAIN-MAIN
		static void Main(string[] args)
		{

			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

			string input = Console.ReadLine();
			int n = Int32.Parse(input);

			List<Tabel> tabels = GenerateListTabel(n);
			List<Relasi> relasis = GenerateRelasi(tabels);

			SearchingGreedy(tabels, relasis, "Tabel_1", "Tabel_10");

			//GenerateFileTable(n, tabels);
			//GenerateFileRelation(relasis);
		}

		static Tabel GetTabel(List<Tabel> tabels, string nama) => tabels.Find(x => x.Nama.Equals(nama));	

		static void SearchingGreedy(List<Tabel> tabels, List<Relasi> relasis, string start, string goal)
		{
			Tabel tabelStart = GetTabel(tabels, start), tabelGoal = GetTabel(tabels, goal);

			List<Tabel> open = new List<Tabel>(), closed = new List<Tabel>();
			
			if (tabelStart != null && tabelGoal != null)
			{
				Tabel current = tabelStart;
				closed.Add(current);

				foreach (var item in relasis)
				{
					Console.WriteLine(item.ToString());
				}
				Console.WriteLine(" ");

				while (current != tabelGoal)
				{
					// Mengumpulkan relasi
					List<Relasi> rs = GatherRelasi(relasis, closed, current);

					// Variabel pembanding state old
					double lowest = -1;
					Tabel lowestTabel = null;
					foreach (var item in rs)
					{
						Console.WriteLine(item);

						Tabel reverse = current == item.Tabel1 ? item.Tabel2 : item.Tabel1;
						double value = reverse.Jarak(tabelGoal); // Nilai heuristik
						if (lowest == -1) // Jika state pertama kali jalan
						{
							lowest = value;
							lowestTabel = reverse;
						}else if (tabelGoal == reverse) // Node child adalah goal
						{
							lowest = value;
							lowestTabel = reverse;
							current = reverse;
							Console.WriteLine(item.Tabel1.Nama + " - " + item.Tabel2.Nama + " ==> " + value);
							Console.WriteLine("Found goal...");
							break;
						}else if (lowest > value) // Jika tidak ditemukan goal dan bandingkan nilai state old dengan new
						{
							lowest = value;
							lowestTabel = reverse;
						}
						Console.WriteLine(item.Tabel1.Nama + " - " + item.Tabel2.Nama + " ==> " + value);
					}
					Console.WriteLine("Lowest: " + lowestTabel);
					Console.WriteLine(" ");

					current = lowestTabel;
					closed.Add(current);
				}

				Console.WriteLine(" \nResult:");
				foreach (var item in closed)
				{
					Console.Write($"{item.Nama} - ");
				}
			}
		}

		static List<Relasi> GatherRelasi(List<Relasi> relasis, List<Tabel> closed, Tabel tabel)
		{
			return relasis.FindAll(x => (x.Tabel1 == tabel && tabel.Relasi.Contains(x.Tabel2) && !closed.Contains(x.Tabel2)) ||
										(x.Tabel2 == tabel && tabel.Relasi.Contains(x.Tabel1) && !closed.Contains(x.Tabel1)));
		}


		static string DirectoryFolder => Path.GetFullPath(Directory.GetCurrentDirectory() + "/../../../");

		static int GenerateNumberMultipleFive()
		{
			Random random = new Random();
			int num = random.Next(0, 200);

			int leftover = num % 5;

			int difference = 0;
			if (leftover > 0)
			{
				difference = 5 - leftover;
			}

			num += difference;

			return num;
		}

		static List<Tabel> GenerateListTabel(int n)
		{
			List<Tabel> list = new List<Tabel>();
			for (int j = 0; j < n; j++)
			{
				string label = $"Tabel_{j + 1}";
				string[] subs = label.Split(' ');
				Tabel t = new Tabel(label, GenerateNumberMultipleFive(), GenerateNumberMultipleFive());

				list.Add(t);
			}
			return list;
		}

		static List<Relasi> GenerateRelasi(List<Tabel> tabels)
		{
			List<Relasi> relasis = new List<Relasi>();
			Random random = new Random();

			List<Tabel> copyTabels = new List<Tabel>(tabels);
			foreach (var tabel in tabels)
			{
				if (!copyTabels.Contains(tabel)) continue;

				while (tabel.Relasi.Count < tabel.MaxRelasi)
				{
					if (copyTabels.Count <= 1) break;

					Tabel select;
					do
					{
						int rn = random.Next(0, copyTabels.Count);
						select = copyTabels[rn];
					} while (select == tabel || tabel.Relasi.Contains(select));

					tabel.Relasi.Add(select);
					select.Relasi.Add(tabel);

					if (select.Relasi.Count == select.MaxRelasi)
					{
						copyTabels.Remove(select);
					}

					Relasi relasi = new Relasi(tabel, select);
					relasis.Add(relasi);
				}

				copyTabels.Remove(tabel);
			}

			return relasis;
		}

		static void GenerateFileTable(int n, List<Tabel> tabels)
		{

			var file = new FileInfo(DirectoryFolder + "/Table.xls");

			using (var package = new ExcelPackage(file))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet 1"];
				if (sheet == null) sheet = package.Workbook.Worksheets.Add("Sheet 1");

				sheet.Cells["A1"].Value = "Table_Name";
				sheet.Cells["B1"].Value = "X";
				sheet.Cells["C1"].Value = "Y";

				for (int i = 0; i < tabels.Count; i++)
				{
					var tabel = tabels[i];
					int loc = i + 2;

					sheet.Cells[$"A{loc}"].Value = tabel.Nama;
					sheet.Cells[$"B{loc}"].Value = tabel.Kordinat.X;
					sheet.Cells[$"C{loc}"].Value = tabel.Kordinat.Y;
				}

				package.Save();
			}

		}

		static void GenerateFileRelation(List<Relasi> relasis)
		{
			var file = new FileInfo(DirectoryFolder + "/Relasi.xls");

			using (var package = new ExcelPackage(file))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet 1"];
				if (sheet == null) sheet = package.Workbook.Worksheets.Add("Sheet 1");

				sheet.Cells["A1"].Value = "Relasi_Name";
				sheet.Cells["B1"].Value = "Tabel_1";
				sheet.Cells["C1"].Value = "Tabel_2";


				for (int i = 0; i < relasis.Count; i++)
				{
					var relasi = relasis[i];
					int loc = i + 2;

					sheet.Cells[$"A{loc}"].Value = relasi.Label;
					sheet.Cells[$"B{loc}"].Value = relasi.Tabel1.Nama;
					sheet.Cells[$"C{loc}"].Value = relasi.Tabel2.Nama;
				}
				package.Save();
			}
		}
	}
}
