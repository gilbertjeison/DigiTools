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
    
    public partial class lista_acciones
    {
        public int Id { get; set; }
        public string accion { get; set; }
        public string tipo_accion { get; set; }
        public string responsable { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> id_ewo { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
    }
}
