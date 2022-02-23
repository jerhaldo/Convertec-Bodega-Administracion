using Convertec_Bodega_Administracion.Model;
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
        public static bool CheckDBConnection(bool showSuccess, bool showError)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    db.Database.Connection.Open();
                    if (db.Database.Connection.State == ConnectionState.Open)
                    {
                        /*System.Console.WriteLine(@"INFO: ConnectionString: " + db.Database.Connection.ConnectionString
                            + "\n DataBase: " + db.Database.Connection.Database
                            + "\n DataSource: " + db.Database.Connection.DataSource
                            + "\n ServerVersion: " + db.Database.Connection.ServerVersion
                            + "\n TimeOut: " + db.Database.Connection.ConnectionTimeout);*/
                        db.Database.Connection.Close();
                        Cursor.Current = Cursors.Default;
                        if (showSuccess)
                        {
                            MessageBox.Show("La conexión al servidor funciona de forma correcta.", "Estado de conexión", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        return true;
                    }
                    else
                        return false;
                }
                catch (SqlException)
                {
                    Cursor.Current = Cursors.Default;
                    if (showError)
                    {
                        MessageBox.Show("Se ha generado un error con la conexión a la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public static List<HistorialMovimientoTabla> GetHistorial(int id, int anno)
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
                        join tra in db.Trabajador on ss.id_trabajador equals tra.id_trabajador
                            into Trabajador
                            from tt in Trabajador.DefaultIfEmpty()
                        where p.id_producto == id && m.fecha_mov.Year == anno
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
                            obs_mov = m.obs_mov,
                            trabajador = tt.nombre + " " + tt.apellidos
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
        public static List<IdDescripcionElemento> GetElementoByFilteredName(string filteredElement)
        {
            using (var db = new ConvertecBodegaEntities())
            {

                var data = (
                        from p in db.Producto
                        where p.descripcion.Contains(filteredElement)
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

        public static void InsertEntrada(List<ProdEntrada> prodEntradaList)
        {
            ConvertecBodegaEntities db = new ConvertecBodegaEntities();
            foreach (ProdEntrada pe in prodEntradaList)
            {
                //INSERT MOVIMIENTO
                Movimiento mov = new Movimiento
                {
                    id_producto = pe.id_producto,
                    fecha_mov = pe.fecha_mov,
                    cantidad = pe.cantidad,
                    ot = pe.ot,
                    obs_mov = pe.obs_mov
                };
                db.Movimiento.Add(mov);
                db.SaveChanges();

                //INSERT ENTRADA
                Ingreso_Prod pd = new Ingreso_Prod
                {
                    id_mov = mov.id_mov,
                    id_proveedor = pe.id_proveedor,
                    id_marca = pe.id_marca,
                    cod_prod_prov = pe.cod_prod_prov,
                    documento = pe.documento,
                    valor = pe.valor,
                    valor_unitario = pe.valor_unitario
                };
                db.Ingreso_Prod.Add(pd);
                db.SaveChanges();

                //UPDATE DATOS DEL PRODUCTO
                try
                {
                    var producto = db.Producto.SingleOrDefault(p => p.id_producto == pe.id_producto);
                    if (producto != null)
                    {
                        producto.stock += pe.cantidad;
                        producto.id_proveedor = pe.id_proveedor;
                        producto.id_marca = pe.id_marca;
                        producto.valor = pe.valor;
                        producto.valor_unitario = pe.valor_unitario;
                        producto.ult_fecha_compra = pe.fecha_mov;
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
                        stock_min = p.stock_min,
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
                        stock_min = prod.stock_min,
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

        public static List<DescripcionProducto> GetProductos()
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    select new DescripcionProducto
                    {
                        cod_bodega = p.cod_bodega,
                        descripcion = p.descripcion,
                        id_producto = p.id_producto
                    }
                ).ToList();

                List<DescripcionProducto> listProd = new List<DescripcionProducto>();

                foreach (DescripcionProducto prod in data)
                {
                    var descripcionProducto = new DescripcionProducto
                    {
                        cod_bodega = prod.cod_bodega,
                        descripcion = prod.descripcion,
                        id_producto = prod.id_producto
                    };

                    listProd.Add(descripcionProducto);
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

        public static DescripcionElemento GetDescripcionElemento(int id)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    where p.id_producto == id
                    select new DescripcionElemento
                    {
                        id_producto = p.id_producto,
                        cod_bodega = p.cod_bodega,
                        nom_marca = m.nom_marca,
                        nom_proveedor = pr.nom_proveedor,
                        parte_plano = p.parte_plano,
                        obs = p.obs,
                        stock = p.stock,
                        valor = p.valor,
                        valor_unitario = p.valor_unitario,
                        ult_fecha_compra = p.ult_fecha_compra,
                        unidad = p.unidad
                    }
                ).SingleOrDefault();

                db.Dispose();

                return data;
            }
        }
        public static bool CheckProductoByName(string descripcion, bool disponible)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                if (disponible)
                {
                    var result = (from p in db.Producto
                                  where p.descripcion == descripcion
                                  where p.borrado == false
                                  select p.cod_bodega).Any();

                    return result;
                }
                else
                {
                    var result = (from p in db.Producto
                                  where p.descripcion == descripcion
                                  select p.cod_bodega).Any();

                    return result;
                }
            }
        }

        public static List<NombreIdProveedor> GetProveedorByName()
        {
            using (var db = new ConvertecBodegaEntities())
            {

                var data = (
                        from p in db.Proveedor
                        select new NombreIdProveedor
                        {
                            id_proveedor = p.id_proveedor,
                            nom_proveedor = p.nom_proveedor
                        }
                    ).ToList();

                db.Dispose();
                return data;

            }
        }
        public static List<NombreIdMarca> GetMarcaByName()
        {
            using (var db = new ConvertecBodegaEntities())
            {

                var data = (
                        from m in db.Marca
                        select new NombreIdMarca
                        {
                            id_marca = m.id_marca,
                            nom_marca = m.nom_marca
                        }
                    ).ToList();

                db.Dispose();
                return data;

            }
        }
        public static void InsertProveedor(Proveedor prov)
        {
            ConvertecBodegaEntities db = new ConvertecBodegaEntities();
            Proveedor proveedor = new Proveedor
            {
                nom_proveedor = prov.nom_proveedor,
                telefono = prov.telefono,
                email = prov.email,
                vendedor = prov.vendedor,
                rut_empresa = prov.rut_empresa
            };
            db.Proveedor.Add(proveedor);
            db.SaveChanges();
        }

        public static void InsertMarca(Marca mar)
        {
            ConvertecBodegaEntities db = new ConvertecBodegaEntities();
            Marca marca = new Marca
            {
                nom_marca = mar.nom_marca,
            };
            db.Marca.Add(marca);
            db.SaveChanges();
        }

        public static List<ElementoStockBodega> GetStockBodega(string proveedor, string marca, bool criticos)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = new List<ElementoStockBodega>();

                if (criticos)
                {
                    data = (
                        from p in db.Producto
                        join m in db.Marca on p.id_marca equals m.id_marca
                        join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                        where p.cod_bodega != null && pr.nom_proveedor.Contains(proveedor) && m.nom_marca.Contains(marca) && p.stock <= p.stock_min
                        select new ElementoStockBodega
                        {
                            cod_bodega = p.cod_bodega,
                            descripcion = p.descripcion,
                            stock = p.stock,
                            stock_min = p.stock_min,
                            nom_marca = m.nom_marca,
                            nom_proveedor = pr.nom_proveedor,
                            valor = p.valor,
                            valor_unitario = p.valor_unitario
                        }
                    ).ToList();
                } else
                {
                    data = (
                        from p in db.Producto
                        join m in db.Marca on p.id_marca equals m.id_marca
                        join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                        where p.cod_bodega != null && pr.nom_proveedor.Contains(proveedor) && m.nom_marca.Contains(marca)
                        select new ElementoStockBodega
                        {
                            cod_bodega = p.cod_bodega,
                            descripcion = p.descripcion,
                            stock = p.stock,
                            stock_min = p.stock_min,
                            nom_marca = m.nom_marca,
                            nom_proveedor = pr.nom_proveedor,
                            valor = p.valor,
                            valor_unitario = p.valor_unitario
                        }
                    ).ToList();
                }


                List<ElementoStockBodega> listProd = new List<ElementoStockBodega>();

                foreach (ElementoStockBodega prod in data)
                {
                    var productoStockBodega = new ElementoStockBodega
                    {
                        cod_bodega = prod.cod_bodega,
                        descripcion = prod.descripcion,
                        stock = prod.stock,
                        stock_min = prod.stock_min,
                        nom_marca = prod.nom_marca,
                        nom_proveedor = prod.nom_proveedor,
                        valor = prod.valor,
                        valor_unitario = prod.valor_unitario
                    };

                    listProd.Add(productoStockBodega);
                }

                db.Dispose();

                return listProd;
            }
        }

        public static List<ElementoStockBodega> GetStockBodegaImportacion(string proveedor, string marca, bool criticos)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = new List<ElementoStockBodega>();



                if (criticos)
                {
                    data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    where p.cod_bodega != null && p.importado && pr.nom_proveedor.Contains(proveedor) && m.nom_marca.Contains(marca) && p.stock <= p.stock_min
                    select new ElementoStockBodega
                    {
                        cod_bodega = p.cod_bodega,
                        descripcion = p.descripcion,
                        stock = p.stock,
                        stock_min = p.stock_min,
                        nom_marca = m.nom_marca,
                        nom_proveedor = pr.nom_proveedor,
                        valor = p.valor,
                        valor_unitario = p.valor_unitario
                    }
                    ).ToList();
                }
                else
                {
                    data = (
                    from p in db.Producto
                    join m in db.Marca on p.id_marca equals m.id_marca
                    join pr in db.Proveedor on p.id_proveedor equals pr.id_proveedor
                    where p.cod_bodega != null && p.importado && pr.nom_proveedor.Contains(proveedor) && m.nom_marca.Contains(marca)
                    select new ElementoStockBodega
                    {
                        cod_bodega = p.cod_bodega,
                        descripcion = p.descripcion,
                        stock = p.stock,
                        stock_min = p.stock_min,
                        nom_marca = m.nom_marca,
                        nom_proveedor = pr.nom_proveedor,
                        valor = p.valor,
                        valor_unitario = p.valor_unitario
                    }
                    ).ToList();
                }

                List<ElementoStockBodega> listProd = new List<ElementoStockBodega>();
                foreach (ElementoStockBodega prod in data)
                {
                    var productoStockBodega = new ElementoStockBodega
                    {
                        cod_bodega = prod.cod_bodega,
                        descripcion = prod.descripcion,
                        stock = prod.stock,
                        stock_min = prod.stock_min,
                        nom_marca = prod.nom_marca,
                        nom_proveedor = prod.nom_proveedor,
                        valor = prod.valor,
                        valor_unitario = prod.valor_unitario
                    };

                    listProd.Add(productoStockBodega);
                }

                db.Dispose();

                return listProd;
            }
        }

        public static List<ElementoUtilizadoOT> GetElementoUtilizadoOT(string OT)
        {
            using (var db = new ConvertecBodegaEntities())
            {
                var data = (

                    from p in db.Producto
                    join m in db.Movimiento on p.id_producto equals m.id_producto
                    join s in db.Salida_Prod on m.id_mov equals s.id_mov
                    join ma in db.Marca on p.id_marca equals ma.id_marca
                    join pro in db.Proveedor on p.id_proveedor equals pro.id_proveedor
                    where p.cod_bodega != null && m.ot == OT
                    group m by new { p.parte_plano, p.descripcion, ma.nom_marca, pro.nom_proveedor, p.valor, p.valor_unitario } into g
                    select new ElementoUtilizadoOT
                    {
                        parte_plano = g.Key.parte_plano,
                        descripcion = g.Key.descripcion,
                        cantidad = g.Sum(i => i.cantidad),
                        nom_marca = g.Key.nom_marca,
                        nom_proveedor = g.Key.nom_proveedor,
                        valor = g.Key.valor,
                        valor_unitario = g.Key.valor_unitario
                    } 
                   
                ).ToList();

                List<ElementoUtilizadoOT> listProd = new List<ElementoUtilizadoOT>();

                foreach (ElementoUtilizadoOT prod in data)
                {
                    var elementoUtilizadoOT = new ElementoUtilizadoOT
                    {
                        parte_plano = prod.parte_plano,
                        descripcion = prod.descripcion,
                        cantidad = prod.cantidad,
                        nom_marca = prod.nom_marca,
                        nom_proveedor = prod.nom_proveedor,
                        valor = prod.valor,
                        valor_unitario = prod.valor_unitario
                    };

                    listProd.Add(elementoUtilizadoOT);
                }

                db.Dispose();

                return listProd;
            }
        }
    }
}
