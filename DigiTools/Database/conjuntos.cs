//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigiTools.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class conjuntos
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public string image_path { get; set; }
        public Nullable<int> id_sistema { get; set; }
        public Nullable<int> id_smp { get; set; }
    }
}