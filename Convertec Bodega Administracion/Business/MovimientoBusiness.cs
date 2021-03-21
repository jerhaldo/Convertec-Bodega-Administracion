﻿using Convertec_Bodega_Administracion.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Convertec_Bodega_Administracion.Business
{
    class MovimientoBusiness
    {
        public static bool CheckDBConnection(bool showError)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State == ConnectionState.Open)
                    {
                        System.Console.WriteLine(@"INFO: ConnectionString: " + db.Database.Connection.ConnectionString
                            + "\n DataBase: " + db.Database.Connection.Database
                            + "\n DataSource: " + db.Database.Connection.DataSource
                            + "\n ServerVersion: " + db.Database.Connection.ServerVersion
                            + "\n TimeOut: " + db.Database.Connection.ConnectionTimeout);
                        db.Database.Connection.Close();
                        Cursor.Current = Cursors.Default;
                        return true;
                    }
                    else
                        return false;
                }
                catch (SqlException ex)
                {
                    Cursor.Current = Cursors.Default;
                    if (showError)
                    {
                        MessageBox.Show(ex.Message, "Error en conexión con el servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
            }
        }

        public static List<MovSalidasDataGridDTO> GetMovimientosSalidas()
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from m in db.Movimiento
                    join s in db.Salida_Prod on m.id_mov equals s.id_mov
                    join p in db.Producto on m.id_producto equals p.id_producto
                    join t in db.Trabajador on s.id_trabajador equals t.id_trabajador
                    orderby m.fecha_mov descending
                    select new MovSalidasDataGridDTO
                    {
                        cod_bodega = p.cod_bodega,
                        descripcion = p.descripcion,
                        cantidad = m.cantidad,
                        fecha_mov = m.fecha_mov,
                        ot = m.ot,
                        obs_mov = m.obs_mov,
                        nombre = t.nombre,
                        apellidos = t.apellidos
                    }
                ).Take(100).ToList();

                db.Dispose();

                return data;
            }
        }

        public static List<MovIngresoDataGridDTO> GetMovimientosIngresos()
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from m in db.Movimiento
                    join i in db.Ingreso_Prod on m.id_mov equals i.id_mov
                    join p in db.Producto on m.id_producto equals p.id_producto
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    join mr in db.Marca on p.id_marca equals mr.id_marca
                    orderby m.fecha_mov descending
                    select new MovIngresoDataGridDTO
                    {
                        cod_bodega = p.cod_bodega,
                        descripcion = p.descripcion,
                        cantidad = m.cantidad,
                        documento = i.documento,
                        valor = i.valor,
                        valor_unitario = i.valor_unitario,
                        nom_proveedor = pr.nom_proveedor,
                        nom_marca = mr.nom_marca,
                        fecha_mov = m.fecha_mov,
                        parte_plano = p.parte_plano,
                        ot = m.ot,
                        obs_mov = m.obs_mov
                    }
                ).Take(100).ToList();

                db.Dispose();

                return data;
            }
        }

        public static List<HistorialMovimientoTabla> GetHistorial(int id)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = new List<HistorialMovimientoTabla>();
                try
                {
                    data = (
                        from p in db.Producto
                        join m in db.Movimiento on p.id_producto equals m.id_producto
                        join i in db.Ingreso_Prod on m.id_mov equals i.id_mov
                            into Ingreso
                            from ii in Ingreso.DefaultIfEmpty()
                        join s in db.Salida_Prod on m.id_mov equals s.id_mov
                            into Salida
                            from ss in Salida.DefaultIfEmpty()
                        join pro in db.Proveedor on ii.id_proveedor equals pro.id_proveedor
                            into Proveedor
                            from pp in Proveedor.DefaultIfEmpty()
                        join mar in db.Marca on ii.id_marca equals mar.id_marca
                            into Marca
                            from mm in Marca.DefaultIfEmpty()
                        where p.id_producto == id
                        orderby m.fecha_mov descending
                        select new HistorialMovimientoTabla
                        {
                            fecha_mov = m.fecha_mov,
                            cantidad = m.cantidad,
                            ot = m.ot,
                            folio = ss.folio,
                            documento = ii.documento,
                            nom_proveedor = pp.nom_proveedor,
                            nom_marca = mm.nom_marca,
                            valor = ii.valor,
                            valor_unitario = ii.valor_unitario,
                            cod_prod_prov = ii.cod_prod_prov,
                        }
                    ).ToList();
                }
                catch (System.Data.Entity.Core.EntityException ex)
                {
                    System.Console.WriteLine("Property: {0} throws Error: {1}", ex.Source, ex.Message);
                }

                db.Dispose();
                return data;
            }
        }

        public static List<IdTrabajdor> GetIDs()
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = new List<IdTrabajdor>();
                try
                {
                    data = (
                        from t in db.Trabajador
                        select new IdTrabajdor
                        {
                            id_trabajador = t.id_trabajador
                        }
                    ).ToList();
                }
                catch (System.Data.Entity.Core.EntityException ex)
                {
                    System.Console.WriteLine("Property: {0} throws Error: {1}", ex.Source, ex.Message);
                }

                db.Dispose();
                return data;
            }
        }

        public static List<NumeroOt> GetOt()
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = new List<NumeroOt>();
                try
                {
                    data = (
                        from m in db.Movimiento
                        orderby m.ot
                        select new NumeroOt
                        {
                            ot = m.ot
                        }
                    ).ToList();
                }
                catch (System.Data.Entity.Core.EntityException ex)
                {
                    System.Console.WriteLine("Property: {0} throws Error: {1}", ex.Data, ex.Message);
                }

                db.Dispose();

                return data;
            }
        }

        public static NombreTrabajador GetNombre(int id)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from t in db.Trabajador
                    where t.id_trabajador == id
                    select new NombreTrabajador
                    {
                        nombre = t.nombre,
                        apellidos = t.apellidos
                    }
                ).SingleOrDefault();

                db.Dispose();

                return data;
            }
        }

        public static bool CheckId(int id)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var result = (from t in db.Trabajador
                              where t.id_trabajador == id
                              select t.id_trabajador).Any();

                return result;
            }
        }

        public static List<CodBodegaProducto> GetCodBodegaProductos(bool disponible)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                if (disponible)
                {
                    var data = (
                        from p in db.Producto
                        where p.cod_bodega != null
                        where p.borrado == false
                        select new CodBodegaProducto
                        {
                            cod_bodega = p.cod_bodega
                        }
                    ).ToList();

                    db.Dispose();
                    return data;

                }
                else
                {
                    var data = (
                        from p in db.Producto
                        where p.cod_bodega != null
                        select new CodBodegaProducto
                        {
                            cod_bodega = p.cod_bodega
                        }
                    ).ToList();

                    db.Dispose();
                    return data;
                }
            }
        }

        public static List<IdDescripcionElemento> GetElementoByName()
        {
            using (var db = new ConvertecBodegaEntities())
            {

                var data = (
                        from p in db.Producto
                        where p.cod_bodega != null
                        where p.borrado == false
                        select new IdDescripcionElemento
                        {
                            id_producto = p.id_producto,
                            descripcion = p.descripcion
                        }
                    ).ToList();

                db.Dispose();
                return data;

            }
        }

        public static DescProducto GetDescProductos(long cod)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    where p.cod_bodega == cod
                    where p.borrado == false
                    select new DescProducto
                    {
                        descripcion = p.descripcion,
                        nom_marca = m.nom_marca,
                        nom_proveedor = pr.nom_proveedor,
                        parte_plano = p.parte_plano,
                        obs = p.obs,
                        id_producto = p.id_producto,
                        stock = p.stock,
                        unidad = p.unidad
                    }
                ).SingleOrDefault();

                db.Dispose();

                return data;
            }
        }

        public static DescProductoDetalle GetDescProductosDetalle(long cod)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    where p.cod_bodega == cod
                    select new DescProductoDetalle
                    {
                        descripcion = p.descripcion,
                        nom_marca = m.nom_marca,
                        nom_proveedor = pr.nom_proveedor,
                        parte_plano = p.parte_plano,
                        obs = p.obs,
                        stock = p.stock,
                        ult_fecha_compra = p.ult_fecha_compra,
                        borrado = p.borrado
                    }
                ).SingleOrDefault();

                db.Dispose();

                return data;
            }
        }

        public static bool CheckProducto(long cod, bool disponible)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                if (disponible)
                {
                    var result = (from p in db.Producto
                                  where p.cod_bodega == cod
                                  where p.borrado == false
                                  select p.cod_bodega).Any();

                    return result;
                }
                else
                {
                    var result = (from p in db.Producto
                                  where p.cod_bodega == cod
                                  select p.cod_bodega).Any();

                    return result;
                }
            }
        }

        public static ImagesProducto GetImages(int id)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from p in db.Producto
                    join i in db.Imagen_Producto on p.id_producto equals i.id_producto
                    where p.id_producto == id
                    select new ImagesProducto
                    {
                        image = i.image
                    }
                ).SingleOrDefault();

                db.Dispose();

                return data;
            }
        }

        public static List<OtProducto> GetOtProducto(int id)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from m in db.Movimiento
                    where m.id_producto == id
                    select new OtProducto
                    {
                        ot = m.ot
                    }
                ).Distinct().ToList();

                db.Dispose();

                return data;
            }
        }

        public static void InsertSalida(List<ProdSalida> prodSalList)
        {
            ConvertecBodegaEntities db = new ConvertecBodegaEntities();
            foreach (ProdSalida prodsalida in prodSalList)
            {
                //INSERT MOVIMIENTO
                Movimiento mov = new Movimiento
                {
                    id_producto = prodsalida.id_producto,
                    fecha_mov = prodsalida.fecha_mov,
                    cantidad = prodsalida.cantidad,
                    ot = prodsalida.ot,
                    obs_mov = prodsalida.obs_mov
                };
                db.Movimiento.Add(mov);
                db.SaveChanges();

                //INSERT SALIDA
                Salida_Prod pd = new Salida_Prod
                {
                    id_mov = mov.id_mov,
                    id_trabajador = prodsalida.id_trabajador,
                    folio = prodsalida.folio
                };
                db.Salida_Prod.Add(pd);
                db.SaveChanges();

                //UPDATE STOCK DEL PRODUCTO
                try
                {
                    var producto = db.Producto.SingleOrDefault(p => p.id_producto == prodsalida.id_producto);
                    if (producto != null)
                    {
                        producto.stock -= prodsalida.cantidad;
                        db.SaveChanges();
                    }
                    else
                    {
                        System.Console.WriteLine("Error en Id producto");
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} throws Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }
        }

        public static bool GetDisponibilidad(double stock, double cant)
        {
            if ((stock - cant) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetFormatedOts(int id)
        {
            string formatedOt = "";
            foreach (OtProducto ot in GetOtProducto(id))
            {
                if (ot.ot != "")
                {
                    formatedOt = formatedOt + ot.ot + "/";
                }
            }
            if (formatedOt != "")
            {
                formatedOt = formatedOt.Remove(formatedOt.Length - 1);
            }
            return formatedOt;
        }

        public static List<ProductoDetalle> GetProductosDetalle()
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    select new ProductoDetalle
                    {
                        cod_bodega = p.cod_bodega,
                        descripcion = p.descripcion,
                        nom_marca = m.nom_marca,
                        nom_proveedor = pr.nom_proveedor,
                        stock = p.stock,
                        ult_fecha_compra = p.ult_fecha_compra,
                        valor = p.valor,
                        parte_plano = p.parte_plano,
                        id_producto = p.id_producto
                    }
                ).ToList();

                List<ProductoDetalle> listProd = new List<ProductoDetalle>();

                foreach (ProductoDetalle prod in data)
                {
                    var productoDetalle = new ProductoDetalle
                    {
                        cod_bodega = prod.cod_bodega,
                        descripcion = prod.descripcion,
                        nom_marca = prod.nom_marca,
                        nom_proveedor = prod.nom_proveedor,
                        stock = prod.stock,
                        ult_fecha_compra = prod.ult_fecha_compra,
                        valor = prod.valor,
                        parte_plano = prod.parte_plano,
                        ots = GetFormatedOts(prod.id_producto),
                        id_producto = prod.id_producto
                    };

                    listProd.Add(productoDetalle);
                }
                
                db.Dispose();

                return listProd;
            }
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
