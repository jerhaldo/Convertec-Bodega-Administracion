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
    
    public partial class Imagen_Producto
    {
        public int id_image { get; set; }
        public int id_producto { get; set; }
        public string image { get; set; }
    
        public virtual Producto Producto { get; set; }
    }
}
