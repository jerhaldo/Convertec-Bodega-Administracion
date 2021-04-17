using Convertec_Bodega_Administracion.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convertec_Bodega_Administracion
{
    public partial class FormPrincipal : Form
    {
        private DataTable tableProductoHist;
        private Panel panelActive;
        private FontAwesome.Sharp.IconButton btnActive;
        private bool IEUnidad;

        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            timer1.Start();
            //CheckDBConnection(true);           
            this.BodyPanelSalidaIngreso.Dock = DockStyle.Fill;
            this.BodyPanelIngresoElementos.Dock = DockStyle.Fill;
            this.BodyPanelMovElementos.Dock = DockStyle.Fill;
            panelActive = BodyPanelMovElementos;
            btnActive = SidebarBtnMovElementos;
            PopulateProdHistTable();
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(BodyPanelMovElementos, SidebarBtnMovElementos);
        }

        //Metodos Generales
        #region Metodos Generales

        private void timer1_Tick(object sender, EventArgs e)
        {
            HlabelClock.Text = DateTime.Now.ToString("G");
        }

        /*private void CheckDBConnection(bool showError)
        {
            if (MovimientoBusiness.CheckDBConnection(showError))
            {
                HiconPbDataBase.IconColor = Color.FromArgb(138, 183, 30);
            }
            else
            {
                //MessageBox.Show("Error, No se ha podido establecer una conexión con la base de datos.", "Error de conexión.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HiconPbDataBase.IconColor = Color.FromArgb(255, 56, 0);
            }
        }*/

        /*private void IconPbDataBase_Click(object sender, EventArgs e)
        {
            //Forzar chequeo
            timer2.Stop();
            CheckDBConnection(true);
            timer2.Start();
        }*/
        private void AlertMessage(string message, MessageBoxIcon icon)
        {
            string caption = "Error Detectado en el Código de Producto";
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            //Muestra el MessageBox.
            MessageBox.Show(message, caption, buttons, icon);
        }
        private void SetDefaultPanelsAndButtons(Panel panel, FontAwesome.Sharp.IconButton btn)
        {
            panel.Visible = false;
            btn.BackColor = Color.FromArgb(49, 76, 76);
            btn.ForeColor = Color.DarkGray;
            btn.IconColor = Color.DarkGray;
        }

        private void SetVisible(Panel panel, FontAwesome.Sharp.IconButton btn)
        {
            panel.Visible = true;
            btn.BackColor = Color.FromArgb(39, 57, 58);
            btn.ForeColor = Color.FromArgb(138, 183, 30);
            btn.IconColor = Color.FromArgb(138, 183, 30);
            panelActive = panel;
            btnActive = btn;
        }
        private void CheckNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion
        //Fin Metodos Generales

        //Metodos Movimientos de Elementos
        #region Funciones Movimientos de Elementos

        private void SidebarBtnMovElementos_Click(object sender, EventArgs e)
        {
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(BodyPanelMovElementos, SidebarBtnMovElementos);
            PopulateProdHistTable();
        }

        private void PopulateProdHistTable()
        {
            tableProductoHist = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetProductosDetalle());
            MVdataGridViewProdHist.DataSource = tableProductoHist;
            FormatTableProdHist(MVdataGridViewProdHist);
        }

        private void FormatTableProdHist(DataGridView dgv)
        {
            dgv.Columns[0].HeaderText = "Fecha Compra";
            dgv.Columns[1].HeaderText = "Código";
            dgv.Columns[2].HeaderText = "Descripción";
            dgv.Columns[3].HeaderText = "Proveedor";
            dgv.Columns[4].HeaderText = "Marca";
            dgv.Columns[5].HeaderText = "Stock";
            dgv.Columns[6].HeaderText = "Valor";
            dgv.Columns[7].HeaderText = "Parte Plano";
            dgv.Columns[8].HeaderText = "OT";
            dgv.Columns["id_producto"].HeaderText = "ID";
            dgv.Columns["id_producto"].Visible = false;

            foreach (DataGridViewColumn col in MVdataGridViewProdHist.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void TableFilter(object sender, EventArgs e)
        {
            DataView dv = tableProductoHist.DefaultView;
            dv.RowFilter = "(cod_bodega LIKE '%" + MVtxtFiltroHist.Text + "%'"
                            + "or descripcion LIKE '%" + MVtxtFiltroHist.Text + "%'"
                            + "or nom_proveedor LIKE '%" + MVtxtFiltroHist.Text + "%'"
                            + "or nom_marca LIKE '%" + MVtxtFiltroHist.Text + "%'"
                            + "or stock LIKE '%" + MVtxtFiltroHist.Text + "%'"
                            + "or valor LIKE '%" + MVtxtFiltroHist.Text + "%'"
                            + "or parte_plano LIKE '%" + MVtxtFiltroHist.Text + "%')"
                            + "and ots LIKE '%" + MVtxtOtFilter.Text + "%'";
            MVdataGridViewProdHist.DataSource = dv;
        }

        private int SelectedRow()
        {
            if (MVdataGridViewProdHist.SelectedCells.Count == 1 || MVdataGridViewProdHist.SelectedRows.Count == 1)
            {
                int selectedrowindex = MVdataGridViewProdHist.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = MVdataGridViewProdHist.Rows[selectedrowindex];
                int cellValue = Int32.Parse(selectedRow.Cells["id_producto"].Value.ToString());
                return cellValue;
            }
            return -1;
        }

        private void CargarDatos(object sender, EventArgs e)
        {
            MVtxtOtHist.Clear();
            int id = SelectedRow();
            if (id != -1)
            {
                MVdataGridViewHistorial.DataSource = MovimientoBusiness.GetHistorial(id);
                foreach (DataGridViewRow row in MVdataGridViewHistorial.Rows)
                    if (row.Cells[5].Value != null)
                    {
                        //row.DefaultCellStyle.ForeColor = Color.FromArgb(138, 183, 30);
                        row.DefaultCellStyle.BackColor = Color.FromArgb(138, 183, 30);
                    }

                var img = MovimientoBusiness.GetImages(id);

                if (img != null)
                    MVpictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/imgProductos/" + img.image);
                else
                    MVpictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/logos/image-unavailable.png");

                foreach (Model.OtProducto ot in MovimientoBusiness.GetOtProducto(id))
                {
                    if (ot.ot != "")
                    {
                        MVtxtOtHist.Text = MVtxtOtHist.Text + ot.ot + Environment.NewLine;
                    }
                }
            }
            else
            {
                MVtxtOtHist.Clear();
                MVpictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/logos/image-unavailable.png");
            }
        }
        #endregion
        //Fin Metodos Movimientos de Elementos

        //Metodos Salidas e Ingresos de Elementos
        #region Metodos Salidas e Ingresos de Elementos

        public void PopulateDataSalidaIngreso()
        {
            SIdataGridViewSalidas.DataSource = MovimientoBusiness.GetMovimientosSalidas();
            SIdataGridViewIngresos.DataSource = MovimientoBusiness.GetMovimientosIngresos();
        }

        private void SidebarBtnSalidaIngreso_Click(object sender, EventArgs e)
        {
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            PopulateDataSalidaIngreso();
            SetVisible(BodyPanelSalidaIngreso, SidebarBtnSalidaIngreso);
        }

        #endregion
        //Fin Metodos Salidas e Ingresos de Elementos

        //Metodos Ingreso de Elementos
        #region Metodos Ingreso de Elementos

        private void CleanDataIE()
        {
            //IEcomboBoxDescripcion.Clear();
            IEtxtCant.Text = "1";
            IEtxtCodProv.Clear();
            IElblUnidad.Text = "";
            IElabelCodBodega.Text = "";
            IElabelPartePlano.Text = "";
            IElabelStock.Text = "";

            IEtxtDocumento.Clear();
            IEtxtOT.Clear();
            IEtxtValor.Text = "0";
            IEtxtValorUni.Text = "0";
            IEtxtObsIngreso.Clear();
            IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
        }

        private void SidebarBtnIngresarElemento_Click(object sender, EventArgs e)
        {
            AutoCompleteTextElemento();
            AutoCompleteTextProveedor();
            AutoCompleteTextMarca();
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(BodyPanelIngresoElementos, SidebarBtnIngresarElemento);
            IEcomboBoxDescripcion.Focus();
        }

        public void AutoCompleteTextProveedor()
        {
            List<Model.NombreIdProveedor> listProveedores = new List<Model.NombreIdProveedor>();
            foreach (Model.NombreIdProveedor proveedor in MovimientoBusiness.GetProveedorByName())
            {
                listProveedores.Add(new Model.NombreIdProveedor() { id_proveedor = proveedor.id_proveedor, nom_proveedor = proveedor.nom_proveedor });
            }
            
            IEcomboBoxProveedor.DisplayMember = "nom_proveedor";
            IEcomboBoxProveedor.ValueMember = "id_proveedor";
            IEcomboBoxProveedor.DataSource = listProveedores;
            IEcomboBoxProveedor.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        public void AutoCompleteTextMarca()
        {
            List<Model.NombreIdMarca> listMarcas = new List<Model.NombreIdMarca>();
            foreach (Model.NombreIdMarca marca in MovimientoBusiness.GetMarcaByName())
            {
                listMarcas.Add(new Model.NombreIdMarca() { id_marca = marca.id_marca, nom_marca = marca.nom_marca });
            }

            IEcomboBoxMarca.DisplayMember = "nom_marca";
            IEcomboBoxMarca.ValueMember = "id_marca";
            IEcomboBoxMarca.DataSource = listMarcas;
            IEcomboBoxMarca.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void AutoCompleteTextElemento()
        {
            List<Model.IdDescripcionElemento> listElementos = new List<Model.IdDescripcionElemento>();
            foreach (Model.IdDescripcionElemento elemento in MovimientoBusiness.GetElementoByName())
            {
                listElementos.Add(new Model.IdDescripcionElemento() { id_producto = elemento.id_producto, descripcion = elemento.descripcion });
            }
            IEcomboBoxDescripcion.DisplayMember = "descripcion";
            IEcomboBoxDescripcion.ValueMember = "id_producto";
            IEcomboBoxDescripcion.DataSource = listElementos;
            IEcomboBoxDescripcion.AutoCompleteSource = AutoCompleteSource.ListItems;

        }

        private void CheckFormat(object sender, EventArgs e)
        {
            if (IEcomboBoxDescripcion.SelectedIndex == -1 && IEcomboBoxDescripcion.Text != "")
            {
                string texto = IEcomboBoxDescripcion.Text;
                if (texto.EndsWith("/") || texto.EndsWith(@"\"))
                {
                    IEcomboBoxDescripcion.SelectedIndex = IEcomboBoxDescripcion.FindString(texto);
                }
            }
        }

        private void GetDescripcionElemento(object sender, EventArgs e)
        {
            if (IEcomboBoxDescripcion.SelectedValue != null && Convert.ToInt32(IEcomboBoxDescripcion.SelectedValue) == -1)
            {
                AlertMessage("Por favor seleccione un elemento.", MessageBoxIcon.Error);
                CleanDataIE();
                IEcomboBoxDescripcion.Focus();
            }
            else 
            { 
                //Chequea si existe conexión con la BD
                if (MovimientoBusiness.CheckDBConnection(true))
                {
                    if (MovimientoBusiness.CheckProductoByName(IEcomboBoxDescripcion.Text, true))
                    {
                        var de = MovimientoBusiness.GetDescripcionElemento(IEcomboBoxDescripcion.Text);
                        IEcomboBoxProveedor.SelectedIndex = IEcomboBoxProveedor.FindString(de.nom_proveedor);
                        IEcomboBoxMarca.SelectedIndex = IEcomboBoxMarca.FindString(de.nom_marca);
                        IEtxtValor.Text = de.valor.ToString();
                        IEtxtValorUni.Text = de.valor_unitario.ToString();
                        IElabelCodBodega.Text = de.cod_bodega.ToString();
                        IElabelPartePlano.Text = de.parte_plano;
                        IElabelStock.Text = de.stock.ToString();

                        this.IEUnidad = de.unidad;
                        if (this.IEUnidad)
                            IElblUnidad.Text = "Unidad";
                        else
                            IElblUnidad.Text = "Metros";


                        var img = MovimientoBusiness.GetImages(Int32.Parse(IEcomboBoxDescripcion.SelectedValue.ToString()));

                        if (img != null)
                            try
                            {
                                IEpictureBoxProducto.Image = Image.FromFile(@"D:\OneDrive\ConvertecImages\ImgProductos\" + img.image);
                            }
                            catch (System.IO.FileNotFoundException)
                            {
                                IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
                            }

                        else
                            IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;

                    }
                    else
                    {
                        AlertMessage("Error, el producto ingresado no se encuentra en el sistema.", MessageBoxIcon.Error);
                        CleanDataIE();
                        IEcomboBoxDescripcion.Focus();
                    }
                }
            }
            
        }

        private void IEbtnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEtxtValor.Text))
            {
                AlertMessage("Error, el valor se encuentra vacío.", MessageBoxIcon.Error);
                IEtxtValor.Focus();
            }
            else
            {
                foreach (DataGridViewRow row in IEdataGridViewProdEntrantes.Rows)
                {
                    if (row.Cells["id_producto"].Value == null)
                    {
                        //ADD
                        IEdataGridViewProdEntrantes.Rows.Add(
                            IEcomboBoxDescripcion.SelectedValue,
                            IEcomboBoxProveedor.SelectedValue,
                            IEcomboBoxMarca.SelectedValue,
                            DateTime.Now,
                            IElabelCodBodega.Text,
                            IEcomboBoxDescripcion.Text,
                            IEtxtCant.Text,
                            IElblUnidad.Text,
                            IEtxtValor.Text,
                            IEtxtValorUni.Text,
                            IEtxtOT.Text,
                            IEtxtDocumento.Text,
                            IEtxtCodProv.Text,
                            IEcomboBoxProveedor.Text,
                            IEcomboBoxMarca.Text,
                            IEtxtObsIngreso.Text);
                        CleanDataIE();
                        IEcomboBoxDescripcion.Focus();
                        break;
                    }
                    else if (row.Cells["id_producto"].Value.ToString().Equals(this.IEcomboBoxDescripcion.SelectedValue.ToString()))
                    {
                        //UPDATE
                        row.Cells["id_proveedor"].Value = IEcomboBoxProveedor.SelectedValue;
                        row.Cells["id_marca"].Value = IEcomboBoxMarca.SelectedValue;
                        row.Cells["fecha_mov"].Value = DateTime.Now;
                        row.Cells["cantidad"].Value = IEtxtCant.Text;
                        row.Cells["valor"].Value = IEtxtValor.Text;
                        row.Cells["valor_unitario"].Value = IEtxtValorUni.Text;
                        row.Cells["ot"].Value = IEtxtOT.Text;
                        row.Cells["documento"].Value = IEtxtDocumento.Text;
                        row.Cells["codigo_prov"].Value = IEtxtCodProv.Text;
                        row.Cells["proveedor"].Value = IEcomboBoxProveedor.Text;
                        row.Cells["marca"].Value = IEcomboBoxMarca.Text;
                        row.Cells["obs_mov"].Value = IEtxtObsIngreso.Text;
                        break;
                    }
                }
            }

        }

        private void IEBtnAgregarProveedor_Click(object sender, EventArgs e)
        {
            Views.AgregarProveedor frmAddProveedor = new Views.AgregarProveedor(this);
            frmAddProveedor.ShowDialog();
            frmAddProveedor.Dispose();
        }

        private void IEBtnAgregarMarca_Click(object sender, EventArgs e)
        {
            Views.AgregarMarca frmAddMarca = new Views.AgregarMarca(this);
            frmAddMarca.ShowDialog();
            frmAddMarca.Dispose();
        }

        private void IEbtnQuitar_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = IEdataGridViewProdEntrantes.CurrentCell.RowIndex;
                if (selectedIndex > -1)
                {
                    IEdataGridViewProdEntrantes.Rows.RemoveAt(selectedIndex);
                    IEdataGridViewProdEntrantes.Refresh();
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is NullReferenceException)
                {
                    MessageBox.Show("Debe seleccionar una fila para quitar.", "Error al quitar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                throw;
            }
        }

        private void CargarElementoEditar(DataGridViewRow row)
        {
            try
            {
                if (IEdataGridViewProdEntrantes.CurrentCell.RowIndex > -1)
                {
                    IEcomboBoxDescripcion.SelectedIndex = IEcomboBoxDescripcion.FindString(row.Cells["nombre_prod"].Value.ToString());
                    IEcomboBoxProveedor.SelectedIndex = IEcomboBoxProveedor.FindString(row.Cells["proveedor"].Value.ToString());
                    IEcomboBoxMarca.SelectedIndex = IEcomboBoxMarca.FindString(row.Cells["marca"].Value.ToString());
                    IEtxtValor.Text = row.Cells["valor"].Value.ToString();
                    IEtxtValorUni.Text = row.Cells["valor_unitario"].Value.ToString();
                    IElblUnidad.Text = row.Cells["unidad"].Value.ToString();

                    var img = MovimientoBusiness.GetImages(Int32.Parse(IEcomboBoxDescripcion.SelectedValue.ToString()));

                    if (img != null)
                        try
                        {
                            IEpictureBoxProducto.Image = Image.FromFile(@"D:\OneDrive\ConvertecImages\ImgProductos\" + img.image);
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
                        }

                    else
                        IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is NullReferenceException)
                {
                    MessageBox.Show("Debe seleccionar una fila para editar.", "Error al editar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                throw;
            }
        }

        private void IEdataGridViewProdEntrantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CargarElementoEditar(IEdataGridViewProdEntrantes.Rows[e.RowIndex]);
        }

        private void IECheckDecimalCantidad(object sender, EventArgs e)
        {
            if (!this.IEUnidad)
            {
                IEtxtCant.Text = IEtxtCant.Text.Replace(".", ",");
                if (!decimal.TryParse(IEtxtCant.Text, out decimal cantidad))
                {
                    AlertMessage("Por favor ingrese un número válido.", MessageBoxIcon.Error);
                    IEtxtCant.Text = "1";
                    IEtxtCant.Focus();
                }
            }
            CheckCantidad(Double.Parse(IEtxtCant.Text));
        }
        private void CheckCantidad(double cantidad)
        {
            if (!(cantidad > 0))
            {
                AlertMessage("Por favor ingrese un número mayor a 0.", MessageBoxIcon.Error);
                IEtxtCant.Clear();
                IEtxtCant.Focus();
            }
        }
        private void CheckUnidad(object sender, KeyPressEventArgs e)
        {
            if (this.IEUnidad)
            {
                CheckNumber(sender, e);
            }
        }
        private void IEtxtValor_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEtxtValor.Text))
            {
                IEiconErrorValor.Visible = true;
            }
        }
        private void IEtxtValor_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEtxtValor.Text))
            {

                IEiconErrorValor.Visible = true;
            }
            else
            {
                IEiconErrorValor.Visible = false;
            }
        }
        private void IEbtnConfirmar_Click(object sender, EventArgs e)
        {
            if (IEdataGridViewProdEntrantes.Rows != null && IEdataGridViewProdEntrantes.Rows.Count > 1)
            {
                List<Model.ProdEntrada> prodEntradaList = new List<Model.ProdEntrada>();

                foreach (DataGridViewRow row in IEdataGridViewProdEntrantes.Rows)
                {
                    if (row.Cells["id_producto"].Value != null)
                    {
                        Model.ProdEntrada prodEntrada = new Model.ProdEntrada
                        {
                            id_producto = Int32.Parse(row.Cells["id_producto"].Value.ToString()),
                            fecha_mov = DateTime.Parse(row.Cells["fecha_mov"].Value.ToString()),
                            ot = row.Cells["ot"].Value.ToString(),
                            cantidad = Double.Parse(row.Cells["cantidad"].Value.ToString()),
                            obs_mov = row.Cells["obs_mov"].Value.ToString(),
                            id_proveedor = Int32.Parse(row.Cells["id_proveedor"].Value.ToString()),
                            id_marca = Int32.Parse(row.Cells["id_marca"].Value.ToString()),
                            cod_prod_prov = row.Cells["codigo_prov"].Value.ToString(),
                            documento = row.Cells["documento"].Value.ToString(),
                            valor = Int32.Parse(row.Cells["valor"].Value.ToString())
                        };

                        if (row.Cells["valor_unitario"].Value.ToString() != null) {
                            prodEntrada.valor_unitario = 0;
                        }
                        else
                        {
                            prodEntrada.valor_unitario = Int32.Parse(row.Cells["valor_unitario"].Value.ToString());
                        }

                        prodEntradaList.Add(prodEntrada);
                    }
                }
                var result = MessageBox.Show("Desea finalizar y confirmar el proceso?"
                                , "Confirmación de guardado.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes && MovimientoBusiness.CheckDBConnection(true))
                {
                    MovimientoBusiness.InsertEntrada(prodEntradaList);
                    IEdataGridViewProdEntrantes.Rows.Clear();
                    CleanDataIE();
                }

            }
            else
            {
                AlertMessage("La lista de elementos se encuentra vacía, por favor ingrese elementos a la lista.", MessageBoxIcon.Error);
            }
        }

        private void IEbtnEditar_Click(object sender, EventArgs e)
        {
            CargarElementoEditar(IEdataGridViewProdEntrantes.CurrentRow);
        }

        #endregion
        //Fin Metodos Ingreso de Elementos

        //Metodos Crear Elemento
        #region Metodos Crear Elemento
        #endregion
        //Fin Metodos Crear Elemento
    }
}
