//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatingApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserFilter
    {
        public int FilterId { get; set; }
        public Nullable<bool> IsViewStyleClassic { get; set; }
        public string LocationName { get; set; }
        public Nullable<double> LocationLatitude { get; set; }
        public Nullable<double> LocationLongititude { get; set; }
        public string Gender { get; set; }
        public Nullable<double> Distance { get; set; }
        public string Sex { get; set; }
        public Nullable<int> AgeFrom { get; set; }
        public Nullable<int> AgeTo { get; set; }
        public string UserInterest { get; set; }
        public string UserEmail { get; set; }
    }
}
