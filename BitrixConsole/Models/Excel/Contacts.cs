using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitrixConsole.Models.Excel
{
    public class Contact
    {
        public int RowId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? MobilePhone { get; set; }
        public string? WorkPhone { get; set; }
        public string? WorkEmail { get; set; }
        public string? PersonalEmail { get; set; }
        public string? AreaOfResponsibility { get; set; }
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public string? LevelJobTitle { get; set; }
        public string? SourceOfInterest { get; set; }
        public string? Temp { get; set; }
        public bool IsMarkedAsGarbadge { get; set; } = false;
        public bool IsWasUpdateInBitrix { get; set; } = false;
        public bool IsCreatedInBitrix { get; set; } = false;
        public bool IsFullMatch { get; set; } = false;
        public bool IsPartialMatch { get; set; } = false;
    }
}
