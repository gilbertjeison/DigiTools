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
    
    public partial class sop_pasos
    {
        public int id { get; set; }
        public Nullable<int> num_paso { get; set; }
        public string descripcion { get; set; }
        public string image_path { get; set; }
        public string duracion { get; set; }
        public Nullable<int> codigo_herramienta { get; set; }
        public Nullable<int> id_smp { get; set; }
    }
}
