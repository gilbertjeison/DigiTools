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
    
    public partial class repuestos_utilizados
    {
        public int Id { get; set; }
        public Nullable<int> codigo_sap { get; set; }
        public string descripcion { get; set; }
        public Nullable<int> cantidad { get; set; }
        public Nullable<decimal> costo { get; set; }
        public Nullable<int> id_ewo { get; set; }
        public string image_path { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
        public string cantidad_ewo { get; set; }
    }
}
