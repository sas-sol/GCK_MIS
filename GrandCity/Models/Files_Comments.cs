//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeherEstateDevelopers.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Files_Comments
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public long Userid { get; set; }
        public System.DateTime Date { get; set; }
        public string Msg_Type { get; set; }
        public Nullable<long> File_Id { get; set; }
        public Nullable<bool> SMS { get; set; }
    }
}
