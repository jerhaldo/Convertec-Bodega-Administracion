//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Convertec_Bodega_Administracion.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Movimiento
    {
        public long id_mov { get; set; }
        public int id_producto { get; set; }
        public System.DateTime fecha_mov { get; set; }
        public string ot { get; set; }
        public double cantidad { get; set; }
        public string obs_mov { get; set; }
    
        public virtual Ingreso_Prod Ingreso_Prod { get; set; }
        public virtual Producto Producto { get; set; }
        public virtual Salida_Prod Salida_Prod { get; set; }
    }
}
