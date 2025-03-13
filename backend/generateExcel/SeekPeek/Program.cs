using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using YourCSharpLibrary;

namespace YourCSharpLibrary
{

    public class ExcelGenerator
    {
        public static List<Answer> LoadJson(string jsonPath)
        {
            using (StreamReader r = new StreamReader(jsonPath))
            {
                string json = r.ReadToEnd();

                Console.WriteLine(json);
                List<Answer> items = JsonConvert.DeserializeObject<List<Answer>>(json);
                Console.WriteLine(items[0].question);

                return items;

            }
        }

        public class Answer
        {
            public string question;
            public string answer;
        }


        public static string generateExcel(string answersJson)
        {
            FileInfo inputFile = new FileInfo(Path.Join(Directory.GetCurrentDirectory(), "..", "obrazac.xlsx"));
            FileInfo outputFile = new FileInfo(Path.Join(Directory.GetCurrentDirectory(), "..", "obrazac-output.xlsx"));

            if (!inputFile.Exists)
            {
                Console.WriteLine("Excel file not found! Path: " + inputFile.FullName);
                return "";
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
                            targetCell.Style.WrapText = true;
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

                    List<Answer> items = LoadJson(answersJson);

                    //copied
                    for (int row = 1; row <= rows; row++)
                    {
                        for (int col = 1; col <= cols; col++)
                        {
                            var sourceCell = sheet.Cells[row, col];
                            var targetCell = newWorksheet.Cells[row, col];

                            bool exists = Array.Exists<Answer>(items.ToArray(), element => targetCell.Text.Contains(element.question));

                            if (exists) // Match the cell
                            {
                                Answer value = Array.Find<Answer>(items.ToArray(), element => targetCell.Text.Contains(element.question));

                                // TODO fix this mess
                                string[] answerSplit = value.answer.Split("\\n");
                                Console.WriteLine(answerSplit);
                                string formatedAnswer = string.Join("dujo ", answerSplit);
                                Console.WriteLine(formatedAnswer);

                                int targetCol = col + 4; // Move one column to the right
                                if (targetCol <= cols) // Ensure it's within bounds
                                {
                                    newWorksheet.Cells[row, targetCol].Value = formatedAnswer;
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
                Console.WriteLine("Excel file copied successfully with styles to: " + outputFile.Name);
            }
            return outputFile.Name;
        }

    }
}

class Program
{


    public static void Main(string[] args)
    {
        Console.WriteLine(args[0]);
        Console.WriteLine("Hello seekpeek");
        ExcelGenerator.generateExcel(args[0]);
    }
}

