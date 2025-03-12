// using OfficeOpenXml;
// using System;
// using System.IO;

// class Program
// {
//     static void Main()
//     {
//         // string filePath = @"obrazac.xlsx";
//         FileInfo fileInfo = new FileInfo("C:\\Users\\LARAZ-PC\\Desktop\\seekExcel\\obrazac.xlsx");


//         if (!fileInfo.Exists)
//         {
//             Console.WriteLine("Excel file not found!");
//             return;
//         }

//         using (var package = new ExcelPackage(fileInfo))
//         {
//             // Proveri da li ima worksheet-ova
//             if (package.Workbook.Worksheets.Count == 0)
//             {
//                 Console.WriteLine("No worksheets found in the Excel file.");
//                 return;
//             }

//             // Uzmi prvi sheet
//             var worksheet = package.Workbook.Worksheets[0];

//             if (worksheet.Dimension == null)
//             {
//                 Console.WriteLine("Worksheet is empty.");
//                 return;
//             }

//             for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
//             {
//                 double rowHeight = worksheet.Row(row).Height;
//                 Console.WriteLine($"Row {row} Height: {rowHeight}");
//             }

//             for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
//             {
//                 double colWidth = worksheet.Column(col).Width;
//                 Console.WriteLine($"Column {col} Width: {colWidth}");
//             }

//             for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
//             {
//                 for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
//                 {
//                     var cell = worksheet.Cells[row, col];

//                     if (cell.Merge)
//                     {
//                         Console.WriteLine($"Cell at ({row},{col}) is merged with {cell.Address}");
//                     }
//                     Console.WriteLine($"Cell ({row},{col}) Value: {cell.Text}");
//                 }
//             }
//         }
//     }
// }




using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;

namespace YourCSharpLibrary
{
    public class FibonacciCalculator
    {
        public static int Calculate(int n)
        {
            if (n <= 0)
                return 0;
            else if (n == 1)
                return 1;
            else
                return Calculate(n - 1) + Calculate(n - 2);
        }
    }
}

class Program
{

    public static List<Item> LoadJson()
    {
        using (StreamReader r = new StreamReader("C:\\Users\\LARAZ-PC\\Desktop\\seekExcel\\SeekPeek\\answers.json"))
        {
            string json = r.ReadToEnd();
            Console.WriteLine(json);
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
            Console.WriteLine(items[0].question);

            return items;
        }
    }

    public class Item
    {
        public string question;
        public string answer;
    }
    public static void Main()
    {

        // [DllExport("add", CallingConvention = CallingConvention.Cdecl)]
        // static int TestExport(int left, int right)
        // {
        //     return left + right;
        // }

        // string inputFilePath = @"obrazac.xlsx";   // Input Excel file
        // string outputFilePath = @"kopija_obrazac.xlsx";  // Output Excel file (copy)

        FileInfo inputFile = new FileInfo("C:\\Users\\LARAZ-PC\\Desktop\\seekExcel\\obrazac.xlsx");
        FileInfo outputFile = new FileInfo("C:\\Users\\LARAZ-PC\\Desktop\\seekExcel\\obrazacKopija.xlsx");

        if (!inputFile.Exists)
        {
            Console.WriteLine("Excel file not found! Path: " + inputFile.FullName);
            return;
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Required for EPPlus

        using (var package = new ExcelPackage(inputFile))
        using (var newPackage = new ExcelPackage())
        {
            foreach (var sheet in package.Workbook.Worksheets)
            {
                var newWorksheet = newPackage.Workbook.Worksheets.Add(sheet.Name);
                int rows = sheet.Dimension.Rows;
                int cols = sheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++)
                {
                    for (int col = 1; col <= cols; col++)
                    {
                        var sourceCell = sheet.Cells[row, col];
                        var targetCell = newWorksheet.Cells[row, col];

                        // Copy cell value
                        targetCell.Value = sourceCell.Value;

                        // Copy cell styles
                        targetCell.Style.Font.Bold = sourceCell.Style.Font.Bold;
                        targetCell.Style.Font.Italic = sourceCell.Style.Font.Italic;
                        targetCell.Style.Font.Size = sourceCell.Style.Font.Size;
                        targetCell.Style.Font.Name = sourceCell.Style.Font.Name;
                        // targetCell.Style.Font.Color.SetColor(sourceCell.Style.Font.Color.Rgb);
                        targetCell.Style.Fill.PatternType = sourceCell.Style.Fill.PatternType;
                        // targetCell.Style.Fill.BackgroundColor.SetColor(sourceCell.Style.Fill.BackgroundColor.Rgb);
                        targetCell.Style.HorizontalAlignment = sourceCell.Style.HorizontalAlignment;
                        targetCell.Style.VerticalAlignment = sourceCell.Style.VerticalAlignment;
                        targetCell.Style.Border.Top.Style = sourceCell.Style.Border.Top.Style;
                        targetCell.Style.Border.Bottom.Style = sourceCell.Style.Border.Bottom.Style;
                        targetCell.Style.Border.Left.Style = sourceCell.Style.Border.Left.Style;
                        targetCell.Style.Border.Right.Style = sourceCell.Style.Border.Right.Style;

                    }
                }

                // Copy column widths
                for (int col = 1; col <= cols; col++)
                {
                    newWorksheet.Column(col).Width = sheet.Column(col).Width;
                }

                // Copy row heights
                for (int row = 1; row <= rows; row++)
                {
                    newWorksheet.Row(row).Height = sheet.Row(row).Height;
                }

                // Copy merged cells
                foreach (var mergedCell in sheet.MergedCells)
                {
                    newWorksheet.Cells[mergedCell].Merge = true;
                }

                List<Item> items = LoadJson();

                //copied
                for (int row = 1; row <= rows; row++)
                {
                    for (int col = 1; col <= cols; col++)
                    {
                        var sourceCell = sheet.Cells[row, col];
                        var targetCell = newWorksheet.Cells[row, col];

                        bool exists = Array.Exists<Item>(items.ToArray(), element => targetCell.Text.Contains(element.question));

                        //if (targetCell.Text.Contains("Doprinos području")) // Match the cell
                        if (exists) // Match the cell
                        {
                            Item value = Array.Find<Item>(items.ToArray(), element => targetCell.Text.Contains(element.question));
                            int targetCol = col + 4; // Move one column to the right
                            if (targetCol <= cols) // Ensure it's within bounds
                            {
                                newWorksheet.Cells[row, targetCol].Value = value.answer;
                                Console.WriteLine($"Added text at ({row}, {targetCol})");
                            }
                        }
                    }
                }




                // // Copy images next fix
                // foreach (var drawing in sheet.Drawings)
                // {
                //     if (drawing is ExcelPicture excelPicture)
                //     {
                //         using (MemoryStream imgStream = new MemoryStream(excelPicture.Image.ImageBytes))
                //         {
                //             var copiedPicture = newWorksheet.Drawings.AddPicture(excelPicture.Name, imgStream);

                //             // Set the position and size to match the original
                //             copiedPicture.SetPosition(excelPicture.From.Row, excelPicture.From.RowOff,
                //                                       excelPicture.From.Column, excelPicture.From.ColumnOff);
                //             copiedPicture.SetSize(200, 100);
                //         }
                //     }
                // }
            }

            // Save the new file
            newPackage.SaveAs(outputFile);
            Console.WriteLine("Excel file copied successfully with styles to: " + outputFile.FullName);
        }
    }
}

