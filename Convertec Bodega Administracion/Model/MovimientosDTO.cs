using System;
using System.Collections.Generic;

namespace Convertec_Bodega_Administracion.Model
{
    class MovSalidasDataGridDTO
    {
        public Nullable<long> cod_bodega { get; set; }
        public string descripcion { get; set; }
        public double cantidad { get; set; }
        public System.DateTime fecha_mov { get; set; }
        public string ot { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string obs_mov { get; set; }

    }

    class MovIngresoDataGridDTO
    {
        public Nullable<long> cod_bodega { get; set; }
        public string descripcion { get; set; }
        public double cantidad { get; set; }
        public string documento { get; set; }
        public int valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
        public string nom_proveedor { get; set; }
        public string nom_marca { get; set; }
        public System.DateTime fecha_mov { get; set; }
        public string parte_plano { get; set; }
        public string ot { get; set; }
        public string obs_mov { get; set; }

    }

    class ProductoDetalle
    {
        public System.DateTime ult_fecha_compra { get; set; }
        public Nullable<long> cod_bodega { get; set; }
        public string descripcion { get; set; }
        public string nom_proveedor { get; set; }
        public string nom_marca { get; set; }
        public double stock { get; set; }
        public double stock_min { get; set; }
        public int valor { get; set; }
        public string parte_plano { get; set; }
        public string ots { get; set; }
        public int id_producto { get; set; }
    }

    class DescripcionProducto
    {
        public Nullable<long> cod_bodega { get; set; }
        public string descripcion { get; set; }
        public int id_producto { get; set; }
    }

    class OtProducto
    {
        public string ot { get; set; }
    }

    class ImagesProducto
    {
        public string image { get; set; }
    }

    class ProdEntrada
    {
        public int id_producto { get; set; }
        public System.DateTime fecha_mov { get; set; }
        public string ot { get; set; }
        public double cantidad { get; set; }
        public string obs_mov { get; set; }
        public int id_proveedor { get; set; }
        public int id_marca { get; set; }
        public string cod_prod_prov { get; set; }
        public string documento { get; set; }
        public int valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
    }

    class HistorialMovimientoTabla
    {
        public System.DateTime fecha_mov { get; set; }
        public double cantidad { get; set; }  
        public string ot { get; set; }
        public Nullable<int> folio { get; set; }
        public string documento { get; set; }
        public string nom_proveedor { get; set; }
        public string nom_marca { get; set; }
        public string cod_prod_prov { get; set; }
        public Nullable<int> valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
        public string obs_mov { get; set; }
        
    }

    class IdDescripcionElemento
    {
        public int id_producto { get; set; }
        public string descripcion { get; set; }
    }

    class DescripcionElemento
    {
        public int id_producto { get; set; }
        public Nullable<long> cod_bodega { get; set; }
        public string nom_proveedor { get; set; }
        public string nom_marca { get; set; }
        public string parte_plano { get; set; }
        public string obs { get; set; }
        public double stock { get; set; }
        public Nullable<int> valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
        public System.DateTime ult_fecha_compra { get; set; }
        public bool unidad { get; set; }

    }
    class NombreIdProveedor
    {
        public int id_proveedor { get; set; }
        public string nom_proveedor { get; set; }
    }
    class NombreIdMarca
    {
        public int id_marca { get; set; }
        public string nom_marca { get; set; }
    }

    class ElementoStockBodega
    {
        public Nullable<long> cod_bodega { get; set; }
        public string descripcion { get; set; }
        public double stock { get; set; }
        public double stock_min { get; set; }
        public string nom_proveedor { get; set; }
        public string nom_marca { get; set; }
        public Nullable<int> valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
    }

    class ElementoUtilizadoOT
    {
        public string parte_plano { get; set; }
        public string descripcion { get; set; }
        public double cantidad { get; set; }
        public string nom_proveedor { get; set; }
        public string nom_marca { get; set; }
        public Nullable<int> valor { get; set; }
        public Nullable<int> valor_unitario { get; set; }
    }
}
