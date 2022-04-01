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
			public string Nama { get; set; }
			public Kordinat Kordinat { get; set; }

			public Tabel(string nama, int x, int y)
			{
				Nama = nama;
				Kordinat = new Kordinat(this, x, y);
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
		}
		
		// MAIN-MAIN
		static void Main(string[] args)
		{
			string input = Console.ReadLine();
			int n = Int32.Parse(input);

			List<Tabel> tabels = GenerateListTabel(n);

			GenerateFileTable(n, tabels);
			GenerateFileRelation(n, tabels);
		}

		static string DirectoryFolder => Path.GetFullPath(Directory.GetCurrentDirectory() + "/../../../");

		static int GenerateNumberMultipleFive()
		{
			Random random = new Random();
			int num = random.Next(0, 200);
			int leftover = num % 5;

			num += leftover;

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

		static void GenerateFileTable(int n, List<Tabel> tabels)
		{

			var file = new FileInfo(DirectoryFolder + "/Table.xls");

			using (var package = new ExcelPackage(file))
			{
				var sheet = package.Workbook.Worksheets.Add("Sheet 1");

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

		static void GenerateFileRelation(int n, List<Tabel> tabels)
		{
			var file = new FileInfo(DirectoryFolder + "/Relasi.xls");
			

		}
	}
}
