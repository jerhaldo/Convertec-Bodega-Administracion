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
    
    public partial class Producto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Producto()
        {
            this.Imagen_Producto = new HashSet<Imagen_Producto>();
            this.Movimiento = new HashSet<Movimiento>();
        }
    
        public int id_producto { get; set; }
        public int id_proveedor { get; set; }
        public int id_marca { get; set; }
        public Nullable<long> cod_bodega { get; set; }
        public string descripcion { get; set; }
        public double stock { get; set; }
        public double stock_min { get; set; }
        public int valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
        public bool unidad { get; set; }
        public System.DateTime ult_fecha_compra { get; set; }
        public string parte_plano { get; set; }
        public string obs { get; set; }
        public bool borrado { get; set; }
        public bool importado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Imagen_Producto> Imagen_Producto { get; set; }
        public virtual Marca Marca { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Movimiento> Movimiento { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}
