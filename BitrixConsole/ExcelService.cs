using BitrixConsole.Models.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitrixConsole
{
    public class ContactWorkSheet : IDisposable
    {
        private ExcelWorksheet excelWorksheet;
        private readonly string pathfile;
        private readonly int numberWorkSheet;
        private FileInfo existingFile;
        private readonly ExcelPackage package;
        public ContactWorkSheet(string pathfile, int numberWorkSheet)
        {
            this.pathfile = pathfile;
            this.numberWorkSheet = numberWorkSheet;
            existingFile = new FileInfo(pathfile);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            package = new ExcelPackage(existingFile);
        }

        private Contact ReadContact(int row) => new()
        {
            RowId = row,
            LastName = (string?)excelWorksheet.Cells[row, 1].Value,
            FirstName = (string?)excelWorksheet.Cells[row, 2].Value,
            MiddleName = (string?)excelWorksheet.Cells[row, 3].Value,
            FullName = (string?)excelWorksheet.Cells[row, 4].Value,
            MobilePhone = (string?)excelWorksheet.Cells[row, 5].Value?.ToString(),
            WorkPhone = (string?)excelWorksheet.Cells[row, 6].Value?.ToString(),
            WorkEmail = (string?)excelWorksheet.Cells[row, 7].Value,
            PersonalEmail = (string?)excelWorksheet.Cells[row, 8].Value,
            AreaOfResponsibility = (string?)excelWorksheet.Cells[row, 9].Value,
            CompanyName = (string?)excelWorksheet.Cells[row, 10].Value,
            JobTitle = (string?)excelWorksheet.Cells[row, 11].Value,
            LevelJobTitle = (string?)excelWorksheet.Cells[row, 12].Value,
            SourceOfInterest = (string?)excelWorksheet.Cells[row, 13].Value,

            Temp = (string?)excelWorksheet.Cells[row, 14].Value,
            IsMarkedAsGarbadge = (string?)excelWorksheet.Cells[row, 15].Value?.ToString()?.ToLower() == "true" ? true : false,
            IsEmailMatch = (string?)excelWorksheet.Cells[row, 16].Value?.ToString()?.ToLower() == "true" ? true : false,
            IsFIOPhoneMatch = (string?)excelWorksheet.Cells[row, 17].Value?.ToString()?.ToLower() == "true" ? true : false,
            IdInBitrix = (string?)excelWorksheet.Cells[row, 18].Value?.ToString(),
            IsInvalidEmail = (string?)excelWorksheet.Cells[row, 19].Value?.ToString()?.ToLower() == "true" ? true : false,
        };
        public void WriteContact(Contact contact)
        {
            excelWorksheet = package.Workbook.Worksheets.ElementAt(numberWorkSheet);

            excelWorksheet.Cells[contact.RowId, 1].Value = contact.LastName;
            excelWorksheet.Cells[contact.RowId, 2].Value = contact.FirstName;
            excelWorksheet.Cells[contact.RowId, 3].Value = contact.MiddleName;
            excelWorksheet.Cells[contact.RowId, 4].Value = contact.FullName;
            excelWorksheet.Cells[contact.RowId, 5].Value = contact.MobilePhone;
            excelWorksheet.Cells[contact.RowId, 6].Value = contact.WorkPhone;
            excelWorksheet.Cells[contact.RowId, 7].Value = contact.WorkEmail;
            excelWorksheet.Cells[contact.RowId, 8].Value = contact.PersonalEmail;
            excelWorksheet.Cells[contact.RowId, 9].Value = contact.AreaOfResponsibility;
            excelWorksheet.Cells[contact.RowId, 10].Value = contact.CompanyName;
            excelWorksheet.Cells[contact.RowId, 11].Value = contact.JobTitle;
            excelWorksheet.Cells[contact.RowId, 12].Value = contact.LevelJobTitle;
            excelWorksheet.Cells[contact.RowId, 13].Value = contact.SourceOfInterest;

            excelWorksheet.Cells[contact.RowId, 14].Value = contact.Temp;
            excelWorksheet.Cells[contact.RowId, 15].Value = contact.IsMarkedAsGarbadge.ToString();
            excelWorksheet.Cells[contact.RowId, 16].Value = contact.IsEmailMatch.ToString();
            excelWorksheet.Cells[contact.RowId, 17].Value = contact.IsFIOPhoneMatch.ToString();
            excelWorksheet.Cells[contact.RowId, 18].Value = contact.IdInBitrix;
            excelWorksheet.Cells[contact.RowId, 19].Value = contact.IsInvalidEmail.ToString();
        }
        public IEnumerable<Contact> ReadAllContacts(int startRow = 2)
        {
            List<Contact> list = new();
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                excelWorksheet = package.Workbook.Worksheets.ElementAt(numberWorkSheet);
                int colCount = excelWorksheet.Dimension.End.Column;
                int rowCount = excelWorksheet.Dimension.End.Row;
                for (int numberRow = startRow; numberRow <= rowCount; numberRow++)
                    list.Add(ReadContact(numberRow));
            }
            return list;
        }
        public void Save() => package.Save();
        public void Dispose()
        {
            package.Dispose();
            excelWorksheet.Dispose();
            GC.Collect(GC.GetGeneration(this));
        }
    }
}
