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

			Console.Write("Masukkan jumlah Tabel: ");
			string input = Console.ReadLine();
			int n = int.Parse(input);

			List<Tabel> tabels = GenerateListTabel(n);
			List<Relasi> relasis = GenerateRelasi(tabels);

			foreach (var item in relasis)
			{
				Console.WriteLine(item);
			}

			/*Console.Write("Masukkan Tabel start: ");
			string start = Console.ReadLine();
			Console.Write("Masukkan Tabel goal: ");
			string goal = Console.ReadLine();

			Console.Write("Masukkan Metode pencarian (Greedy / A*): ");
			string metode = Console.ReadLine();*/
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

		static void SearchingAstar(List<Tabel> tabels, List<Relasi> relasis, string start, string goal)
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
					List<Relasi> rs = GatherRelasi(relasis, closed, current);

					double lowest = -1;
					Tabel lowestTabel = null;

					foreach (var item in rs)
					{
						Console.WriteLine(item);

						Tabel reverse = current == item.Tabel1 ? item.Tabel2 : item.Tabel1;
						double heuristic = reverse.Jarak(tabelGoal);
						double real = current.Jarak(reverse);
						double hasil = real + heuristic;

						if (lowest == -1)
						{
							lowest = hasil;
							lowestTabel = reverse;
						}
						else if (tabelGoal == reverse)
						{
							lowest = hasil;
							lowestTabel = reverse;
							current = reverse;
							Console.WriteLine("Real Cost (g) = " + item.Tabel1.Nama + " - " + item.Tabel2.Nama + " ==> " + real);
							Console.WriteLine("Heursitik Cost (h) = " + item.Tabel1.Nama + " - " + item.Tabel2.Nama + " ==> " + heuristic);
							Console.WriteLine("results = g + h ==> " + hasil);
							Console.WriteLine("Found goal...");
							break;
						}
						else if (lowest > hasil)
						{
							lowest = hasil;
							lowestTabel = reverse;
						}
						Console.WriteLine("Real Cost (g) = " + item.Tabel1.Nama + " - " + item.Tabel2.Nama + " ==> " + real);
						Console.WriteLine("Heursitik Cost (h) = " + item.Tabel1.Nama + " - " + item.Tabel2.Nama + " ==> " + heuristic);
						Console.WriteLine("results = g + h ==> " + hasil);
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
				Tabel t = new Tabel(label, GenerateNumberMultipleFive(), GenerateNumberMultipleFive());

				list.Add(t);
			}
			return list;
		}

		static List<Relasi> GenerateRelasi(List<Tabel> tabels)
		{
			List<Relasi> relasis = new List<Relasi>();

			List<Tabel> copyTabel = new List<Tabel>(tabels);
			Random random = new Random();

			while (copyTabel.Count > 1)
			{

				Tabel t = copyTabel[0];
				int dest = copyTabel.Count == 2 ? 1 : random.Next(1, copyTabel.Count);

				Tabel destination = copyTabel[dest];

				t.Relasi.Add(destination);
				destination.Relasi.Add(t);

				Relasi relasi = new Relasi(t, destination);
				relasis.Add(relasi);

				if (destination.Relasi.Count >= destination.MaxRelasi)
				{
					copyTabel.RemoveAt(dest);
				}
				if (t.Relasi.Count >= t.MaxRelasi)
				{
					copyTabel.RemoveAt(0);
				}
				
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
		static List<Tabel> ReadFromTable()
		{
			List<Tabel> list = new List<Tabel>();

			var file = new FileInfo(DirectoryFolder + "/Table.xls");

			using (var package = new ExcelPackage(file))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet 1"];
				if (sheet == null) return list;

				for (int i = 2; sheet.Cells[$"A{i}"].Value != null; i++)
				{
					string namaTabel = sheet.Cells[$"A{i}"].Value.ToString();
					string rawX = sheet.Cells[$"B{i}"].Value.ToString();
					string rawY = sheet.Cells[$"C{i}"].Value.ToString();

					int x = int.Parse(rawX);
					int y = int.Parse(rawY);

					Tabel tabel = new Tabel(namaTabel, x, y);
					list.Add(tabel);
				}
			}

			return list;
		}

		static List<Relasi> ReadFromRelasi(List<Tabel> tabels)
		{
			// List relasi
			List<Relasi> list = new List<Relasi>();


			var file = new FileInfo(DirectoryFolder + "/Relasi.xls");

			using (var package = new ExcelPackage(file))
			{
				ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet 1"];
				if (sheet == null) return list;

				for (int i = 2; sheet.Cells[$"A{i}"].Value != null; i++)
				{
					// Ambil nama relasi dari file
					string namaRelasi = sheet.Cells[$"A{i}"].Value.ToString();
					// Ambil nama tabel 1 dari file
					string namaTabel1 = sheet.Cells[$"B{i}"].Value.ToString();
					// Ambil nama tabel 2 dari file
					string namaTabel2 = sheet.Cells[$"C{i}"].Value.ToString();
					// Ambil objek tabel 1 dari parameter tabels
					Tabel tabel1 = tabels.Find(x => x.Nama == namaTabel1);
					// Ambil objek tabel 2 dari parameter tabels
					Tabel tabel2 = tabels.Find(y => y.Nama == namaTabel2);
					// Buat objek relasi menggunakan (nama relasi, objek tabel 1, objek tabel 2)
					Relasi relasi = new Relasi(tabel1, tabel2);
					// Tambahkan tabel 2 ke relasi objek tabel 1
					tabel1.Relasi.Add(tabel2);
					// Tambahkan tabel 1 ke relasi objek tabel 2
					tabel2.Relasi.Add(tabel1);
					// Tambahkan objek relasi ke list relasi
					list.Add(relasi);
				}
			}
			return list;
		}
	}
}
