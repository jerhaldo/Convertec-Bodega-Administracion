using Convertec_Bodega_Administracion.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convertec_Bodega_Administracion
{
    public partial class FormPrincipal : Form
    {
        private DataTable tableProductoHist;
        private DataTable tableProducto;
        private Panel panelActive;
        private FontAwesome.Sharp.IconButton btnActive;
        private bool IEUnidad;
        private int anno;

        public FormPrincipal()
        {
            InitializeComponent();
            CultureInfo.CurrentCulture = new CultureInfo("es-CL", false);
            Console.WriteLine("CurrentCulture is now {0}.", CultureInfo.CurrentCulture.Name);
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            timer1.Start();
            //CheckDBConnection(false, true);
            this.BodyPanelSalidaIngreso.Dock = DockStyle.Fill;
            this.BodyPanelIngresoElementos.Dock = DockStyle.Fill;
            this.BodyPanelCrearModifElemento.Dock = DockStyle.Fill;
            this.BodyPanelMovElementos.Dock = DockStyle.Fill;
            this.BodyPanelInformes.Dock = DockStyle.Fill;
            panelActive = BodyPanelMovElementos;
            btnActive = SidebarBtnMovElementos;
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            PopulateProdHistTable();
            SetVisible(BodyPanelMovElementos, SidebarBtnMovElementos);
            anno = MVdateTimePickerFiltro.Value.Year;
        }

        //Metodos Generales
        #region Metodos Generales

        private void timer1_Tick(object sender, EventArgs e)
        {
            HlabelClock.Text = DateTime.Now.ToString("G");
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            Views.Configuraciones frmConfig = new Views.Configuraciones();
            frmConfig.ShowDialog();
            frmConfig.Dispose();
        }

        private void HbtnDatabase_Click(object sender, EventArgs e)
        {
            CheckDBConnection(true, true);
        }

        public bool CheckDBConnection(bool showSuccess, bool showError)
        {
            if (MovimientoBusiness.CheckDBConnection(showSuccess, showError))
            {
                HbtnDatabase.IconColor = Color.FromArgb(138, 183, 30);
                return true;
            }
            else
            {
                //MessageBox.Show("Error, No se ha podido establecer una conexión con la base de datos.", "Error de conexión.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                HbtnDatabase.IconColor = Color.FromArgb(255, 56, 0);
                return false;
            }
        }

        private void AlertMessage(string message, MessageBoxIcon icon)
        {
            string caption = "Error";
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            //Muestra el MessageBox.
            MessageBox.Show(message, caption, buttons, icon);
        }

        private void SetDefaultPanelsAndButtons(Panel panel, FontAwesome.Sharp.IconButton btn)
        {
            btn.BackColor = Color.FromArgb(49, 76, 76);
            btn.ForeColor = Color.DarkGray;
            btn.IconColor = Color.DarkGray;
            panel.Visible = false;
            ClearData();
        }

        private void SetVisible(Panel panel, FontAwesome.Sharp.IconButton btn)
        {
            btn.BackColor = Color.FromArgb(39, 57, 58);
            btn.ForeColor = Color.FromArgb(138, 183, 30);
            btn.IconColor = Color.FromArgb(138, 183, 30);
            panel.Visible = true;
            panelActive = panel;
            btnActive = btn;
        }

        private void ClearData()
        {
            if (panelActive != null)
            {
                switch (panelActive.Name)
                {
                    case "BodyPanelMovElementos":
                        MVdataGridViewHistorial.DataSource = null;
                        MVdataGridViewProdHist.DataSource = null;
                        MVtxtOtHist.Text = null;
                        break;
                    case "BodyPanelIngresoElementos":
                        IEdataGridViewProd.DataSource = null;
                        break;
                    case "BodyPanelSalidaIngreso":
                        SIdataGridViewIngresos.DataSource = null;
                        SIdataGridViewSalidas.DataSource = null;
                        break;
                    case "BodyPanelInformes":
                        INFdataGridViewListadoOT.DataSource = null;
                        INFdataGridViewStockBodega.DataSource = null;
                        INFdataGridViewStockBodegaImportacion.DataSource = null;
                        break;
                }
            }
        }

        private bool CheckNumber(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                return true;
            else
                return false;
        }


        private void CheckNumberOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CheckNumberDecimal(KeyPressEventArgs e, TextBox txt)
        {
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == ',') && (txt.Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void CheckCantidad(TextBox txt, double cantidad)
        {
            try
            {
                if (!(cantidad > 0))
                {
                    AlertMessage("Por favor ingrese un valor mayor a 0.", MessageBoxIcon.Error);
                    txt.Clear();
                    txt.Focus();
                }
            }
            catch (Exception)
            {
                AlertMessage("Por favor ingrese un valor.", MessageBoxIcon.Error);
                throw;
            }
        }

        private void ParseDecimal(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "1";
            }
            else
            {
                if (!decimal.TryParse(txt.Text, out decimal cantidad))
                {
                    AlertMessage("Por favor ingrese un valor válido.", MessageBoxIcon.Error);
                    txt.Text = "1";
                    txt.Focus();
                }
                CheckCantidad(txt, Double.Parse(txt.Text));
            }
        }



        #endregion
        //Fin Metodos Generales

        //Metodos Movimientos de Elementos
        #region Funciones Movimientos de Elementos

        private void SidebarBtnMovElementos_Click(object sender, EventArgs e)
        {
            if (btnActive.Name != SidebarBtnMovElementos.Name)
            {
                SetDefaultPanelsAndButtons(panelActive, btnActive);
                SetVisible(BodyPanelMovElementos, SidebarBtnMovElementos);
                if (BodyPanelMovElementos.Visible)
                {
                    PopulateProdHistTable();
                }
            }
        }

        private void PopulateProdHistTable()
        {
            if(CheckDBConnection(false, true))
            {                
                tableProductoHist = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetProductosDetalle());
                MVdataGridViewProdHist.DataSource = tableProductoHist;
                FormatTableProdHist(MVdataGridViewProdHist);
                
                foreach (DataGridViewRow row in MVdataGridViewProdHist.Rows)
                {
                    if (Double.Parse(row.Cells["stock"].Value.ToString()) <= Double.Parse(row.Cells["stock_min"].Value.ToString()))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 102, 102);
                    }
                }
            }
        }

        private void FormatTableProdHist(DataGridView dgv)
        {
            dgv.Columns[0].HeaderText = "Fecha Compra";
            dgv.Columns[1].HeaderText = "Código";
            dgv.Columns[2].HeaderText = "Descripción";
            dgv.Columns[3].HeaderText = "Proveedor";
            dgv.Columns[4].HeaderText = "Marca";
            dgv.Columns[5].HeaderText = "Stock";
            dgv.Columns[6].HeaderText = "Stock Min";
            dgv.Columns[7].HeaderText = "Valor";
            dgv.Columns[8].HeaderText = "Parte Plano";
            dgv.Columns[9].HeaderText = "OT";
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

        private void SelectionChangedCargarDatos(object sender, EventArgs e)
        {
            CargarDatos(MVdateTimePickerFiltro.Value.Year);
        }

        private void CargarDatos(int anno)
        {
            MVtxtOtHist.Clear();
            int id = SelectedRow();
            if (id != -1)
            {
                if (CheckDBConnection(false, true))
                {
                    MVdataGridViewHistorial.DataSource = MovimientoBusiness.GetHistorial(id, anno);
                    foreach (DataGridViewRow row in MVdataGridViewHistorial.Rows)
                    {
                        if (row.Cells[5].Value != null)
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(138, 183, 30);
                        }
                    }
                    var img = MovimientoBusiness.GetImages(id);

                    if (img != null)
                        MVpictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/imgProductos/" + img.image);
                    else
                        MVpictureBoxProducto.Image = MVpictureBoxProducto.InitialImage;

                    foreach (Model.OtProducto ot in MovimientoBusiness.GetOtProducto(id))
                    {
                        if (ot.ot != "")
                        {
                            MVtxtOtHist.Text = MVtxtOtHist.Text + ot.ot + Environment.NewLine;
                        }
                    }
                }
            }
            else
            {
                MVtxtOtHist.Clear();
                MVpictureBoxProducto.Image = MVpictureBoxProducto.InitialImage;
            }
        }

        private void MVdateTimePickerFiltro_ValueChanged(object sender, EventArgs e)
        {
            if(this.anno != MVdateTimePickerFiltro.Value.Year)
            {
                this.anno = MVdateTimePickerFiltro.Value.Year;
                CargarDatos(this.anno);
            }
            
        }

        #endregion
        //Fin Metodos Movimientos de Elementos

        //Metodos Salidas e Ingresos de Elementos
        #region Metodos Salidas e Ingresos de Elementos

        public void PopulateDataSalidaIngreso()
        {
            if (CheckDBConnection(false, true))
            {
                SIdataGridViewSalidas.DataSource = MovimientoBusiness.GetMovimientosSalidas();
                SIdataGridViewIngresos.DataSource = MovimientoBusiness.GetMovimientosIngresos();
            }
        }

        private void SidebarBtnSalidaIngreso_Click(object sender, EventArgs e)
        {
            if (btnActive.Name != SidebarBtnSalidaIngreso.Name)
            {
                PopulateDataSalidaIngreso();
                SetDefaultPanelsAndButtons(panelActive, btnActive);
                SetVisible(BodyPanelSalidaIngreso, SidebarBtnSalidaIngreso);
            }
        }

        #endregion
        //Fin Metodos Salidas e Ingresos de Elementos

        //Metodos Ingreso de Elementos
        #region Metodos Ingreso de Elementos
        private void SidebarBtnIngresarElemento_Click(object sender, EventArgs e)
        {
            if (btnActive.Name != SidebarBtnIngresarElemento.Name)
            {
                SetDefaultPanelsAndButtons(panelActive, btnActive);
                PopulateProdTable();
                AutoCompleteTextProveedor(IEcomboBoxProveedor);
                AutoCompleteTextMarca(IEcomboBoxMarca);
                SetVisible(BodyPanelIngresoElementos, SidebarBtnIngresarElemento);
                IEtxtDescripcion.Focus();
            }
        }

        private void CleanDataIE()
        {
            IEtxtDescripcion.Clear();
            IEtxtCodProv.Clear();
            IEtxtCant.Text = "1";
            IElblUnidad.Text = "";
            IElabelCodBodega.Text = "";
            IElabelPartePlano.Text = "";
            IElabelStock.Text = "";

            IEtxtDocumento.Clear();
            IEtxtOT.Clear();
            IEtxtObsIngreso.Clear();
            IEtxtValor.Text = "0";
            IEtxtValorUni.Text = "0";
            IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
        }

        private void FormatTableProd(DataGridView dgv)
        {
            dgv.Columns["cod_bodega"].HeaderText = "Código";
            dgv.Columns["descripcion"].HeaderText = "Descripción";
            dgv.Columns["id_producto"].HeaderText = "ID";
            dgv.Columns["id_producto"].Visible = false;

            foreach (DataGridViewColumn col in IEdataGridViewProd.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void IETableFilter(object sender, EventArgs e)
        {
            DataView dv = tableProducto.DefaultView;
            dv.RowFilter = "(cod_bodega LIKE '%" + IEtxtDescripcion.Text + "%'"
                            + "or descripcion LIKE '%" + IEtxtDescripcion.Text + "%')";
            IEdataGridViewProd.DataSource = dv;
        }

        private void PopulateProdTable()
        {
            if (CheckDBConnection(false, true))
            {
                tableProducto = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetProductos());
                IEdataGridViewProd.DataSource = tableProducto;
                FormatTableProd(IEdataGridViewProd);
            }
            else
            {
                Console.WriteLine("PopulateHistProdTable");
            }
        }

        private int IESelectedRow()
        {
            if (IEdataGridViewProd.SelectedRows.Count == 1)
            {
                int cellValue = Int32.Parse(
                                    IEdataGridViewProd.Rows[IEdataGridViewProd.SelectedCells[0].RowIndex]
                                    .Cells["id_producto"].Value.ToString());
                return cellValue;
            }
            return -1;
        }

        private void IECargarDatos(object sender, EventArgs e)
        {
            int id = IESelectedRow();
            if (id != -1)
            {
                if (CheckDBConnection(false, false))
                {
                    var de = MovimientoBusiness.GetDescripcionElemento(id);
                    IEcomboBoxProveedor.SelectedIndex = IEcomboBoxProveedor.FindString(de.nom_proveedor);
                    IEcomboBoxMarca.SelectedIndex = IEcomboBoxMarca.FindString(de.nom_marca);
                    IEtxtValor.Text = de.valor.ToString();
                    IEtxtValorUni.Text = de.valor_unitario.ToString();
                    IElabelCodBodega.Text = de.cod_bodega.ToString();
                    IElabelPartePlano.Text = de.parte_plano;
                    IElabelStock.Text = de.stock.ToString();

                    if (string.IsNullOrWhiteSpace(IEtxtValorUni.Text))
                    {
                        IEtxtValorUni.Text = "0";
                    }

                    this.IEUnidad = de.unidad;
                    if (this.IEUnidad)
                        IElblUnidad.Text = "Unidad";
                    else
                        IElblUnidad.Text = "Metros";
                    var img = MovimientoBusiness.GetImages(id);

                    if (img != null)
                        IEpictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/imgProductos/" + img.image);
                    else
                        IEpictureBoxProducto.Image = IEpictureBoxProducto.InitialImage;
                }
            }
            else
            {
                IEpictureBoxProducto.Image = IEpictureBoxProducto.InitialImage;
            }
        }

        public void AutoCompleteTextProveedor(ComboBox combo)
        {
            if (CheckDBConnection(false, false))
            {
                List<Model.NombreIdProveedor> listProveedores = new List<Model.NombreIdProveedor>();
                foreach (Model.NombreIdProveedor proveedor in MovimientoBusiness.GetProveedorByName())
                {
                    listProveedores.Add(new Model.NombreIdProveedor() { id_proveedor = proveedor.id_proveedor, nom_proveedor = proveedor.nom_proveedor });
                }

                combo.DisplayMember = "nom_proveedor";
                combo.ValueMember = "id_proveedor";
                combo.DataSource = listProveedores;
                combo.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }

        public void AutoCompleteTextMarca(ComboBox combo)
        {
            if (CheckDBConnection(false, false))
            {
                List<Model.NombreIdMarca> listMarcas = new List<Model.NombreIdMarca>();
                foreach (Model.NombreIdMarca marca in MovimientoBusiness.GetMarcaByName())
                {
                    listMarcas.Add(new Model.NombreIdMarca() { id_marca = marca.id_marca, nom_marca = marca.nom_marca });
                }

                combo.DisplayMember = "nom_marca";
                combo.ValueMember = "id_marca";
                combo.DataSource = listMarcas;
                combo.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }

        private void IEbtnAgregar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEtxtValor.Text))
            {
                AlertMessage("Error, el valor se encuentra vacío.", MessageBoxIcon.Error);
                IEtxtValor.Focus();
            }
            else if (string.IsNullOrWhiteSpace(IEtxtCant.Text))
            {
                AlertMessage("Error, la cantidad se encuentra vacía.", MessageBoxIcon.Error);
                IEtxtCant.Focus();
            } else if (string.IsNullOrWhiteSpace(IEtxtOT.Text))
            {
                AlertMessage("Error, la OT se encuentra sin asignar.", MessageBoxIcon.Error);
                IEtxtCant.Focus();
            }
            else
            {
                foreach (DataGridViewRow row in IEdataGridViewProdEntrantes.Rows)
                {
                    if (row.Cells["id_producto"].Value == null)
                    {
                        //ADD
                        IEdataGridViewProdEntrantes.Rows.Add(
                            IEdataGridViewProd.Rows[IEdataGridViewProd.SelectedRows[0].Index].Cells["id_producto"].Value.ToString(),
                            IEcomboBoxProveedor.SelectedValue,
                            IEcomboBoxMarca.SelectedValue,
                            DateTime.Now,
                            IElabelCodBodega.Text,
                            IEdataGridViewProd.Rows[IEdataGridViewProd.SelectedRows[0].Index].Cells["descripcion"].Value.ToString(),
                            IEtxtCant.Text,
                            IElblUnidad.Text,
                            IEtxtValor.Text,
                            IEtxtValorUni.Text,
                            IEtxtOT.Text,
                            IEtxtDocumento.Text,
                            IEtxtCodProv.Text,
                            IEcomboBoxProveedor.Text,
                            IEcomboBoxMarca.Text,
                            IEtxtObsIngreso.Text
                        );
                        break;
                    }
                    else if (row.Cells["id_producto"].Value.Equals(IESelectedRow().ToString()))
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
            IEtxtDescripcion.Focus();
            CleanDataIE();
        }

        private void IEBtnAgregarProveedor_Click(object sender, EventArgs e)
        {
            Views.AgregarProveedor frmAddProveedor = new Views.AgregarProveedor(this, IEcomboBoxProveedor);
            frmAddProveedor.ShowDialog();
            frmAddProveedor.Dispose();
        }

        private void IEBtnAgregarMarca_Click(object sender, EventArgs e)
        {
            Views.AgregarMarca frmAddMarca = new Views.AgregarMarca(this, IEcomboBoxMarca);
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
                    IEtxtDescripcion.Text = row.Cells["nombre_prod"].Value.ToString();
                    IEcomboBoxProveedor.SelectedIndex = IEcomboBoxProveedor.FindString(row.Cells["proveedor"].Value.ToString());
                    IEcomboBoxMarca.SelectedIndex = IEcomboBoxMarca.FindString(row.Cells["marca"].Value.ToString());
                    IEtxtValor.Text = row.Cells["valor"].Value.ToString();
                    IEtxtValorUni.Text = row.Cells["valor_unitario"].Value.ToString();
                    IElblUnidad.Text = row.Cells["unidad"].Value.ToString();
                    IEtxtDocumento.Text = row.Cells["documento"].Value.ToString();
                    IEtxtOT.Text = row.Cells["ot"].Value.ToString();
                    IElabelCodBodega.Text = row.Cells["cod_bodega"].Value.ToString();

                    if (CheckDBConnection(false, true))
                    {
                        var img = MovimientoBusiness.GetImages(IESelectedRow());

                        if (img != null)
                            try
                            {
                                IEpictureBoxProducto.Image = Image.FromFile(Properties.Settings.Default.ImagePath + "/" + img.image);
                            }
                            catch (System.IO.FileNotFoundException)
                            {
                                IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
                            }

                        else
                            IEpictureBoxProducto.Image = Properties.Resources.image_unavailable;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is NullReferenceException)
                {
                    MessageBox.Show("Debe seleccionar una fila para editar.", "Error al editar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        private void IEdataGridViewProdEntrantes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CargarElementoEditar(IEdataGridViewProdEntrantes.Rows[e.RowIndex]);
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
                            valor = Int32.Parse(row.Cells["valor"].Value.ToString()),
                            valor_unitario = Int32.Parse(row.Cells["valor_unitario"].Value.ToString())
                    };

                    prodEntradaList.Add(prodEntrada);
                    }
                }
                var result = MessageBox.Show("Desea finalizar y confirmar el proceso?"
                                , "Confirmación de guardado.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes && CheckDBConnection(false, true))
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

        private void IEParseDecimal(object sender, EventArgs e)
        {
            IEtxtCant.Text = IEtxtCant.Text.Replace(".", ",");
            ParseDecimal(IEtxtCant);
        }

        private void IECheckUnidad(object sender, KeyPressEventArgs e)
        {
            if (this.IEUnidad)
                CheckNumberOnly(sender, e);
            else
                CheckNumberDecimal(e, IEtxtCant);
        }

        private void IEtxtValorUni_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEtxtValorUni.Text))
            {
                IEtxtValorUni.Text = "0";
            }
        }

        private void IEcomboBoxProveedor_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEcomboBoxProveedor.Text))
            {
                AlertMessage("Error, Debe seleccionar un proveedor.", MessageBoxIcon.Error);
                IEcomboBoxProveedor.Focus();
            }
            else if(IEcomboBoxProveedor.FindStringExact(IEcomboBoxProveedor.Text) < 0)  //Si no se encuentra dentro de las opciones del combobox
            {
                AlertMessage("Error, Seleccione un proveedor válido.", MessageBoxIcon.Error);
                IEcomboBoxProveedor.Focus();
            }
        }

        private void IEcomboBoxMarca_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IEcomboBoxMarca.Text))
            {
                AlertMessage("Error, Debe seleccionar una marca.", MessageBoxIcon.Error);
                IEcomboBoxMarca.Focus();
            }
            else if(IEcomboBoxMarca.FindStringExact(IEcomboBoxMarca.Text) < 0)  //Si no se encuentra dentro de las opciones del combobox
            {
                AlertMessage("Error, Seleccione una marca válida.", MessageBoxIcon.Error);
                IEcomboBoxMarca.Focus();
            }
        }

        #endregion
        //Fin Metodos Ingreso de Elementos

        //Metodos Informes
        #region Metodos Informes

        private void SidebarBtnInformes_Click(object sender, EventArgs e)
        {
            if (btnActive.Name != SidebarBtnInformes.Name)
            {
                SetDefaultPanelsAndButtons(panelActive, btnActive);
                SetVisible(BodyPanelInformes, SidebarBtnInformes);
                if (BodyPanelInformes.Visible)
                {
                    PopulateStockBodegaTable();
                }

            }
        }

        private void PopulateStockBodegaTable()
        {
            if (CheckDBConnection(false, true))
            {
                INFdataGridViewStockBodega.DataSource = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetStockBodega(INFtxtFiltroProveedorStock.Text, INFtxtfiltroMarcaStock.Text, INFcheckBoxCriticosStock.Checked));
                INFCheckStockMin(INFdataGridViewStockBodega, "stock", "stock_min");
            }
        }

        private void PopulateStockImportacionBodegaTable()
        {
            if (CheckDBConnection(false, true))
            {
                INFdataGridViewStockBodegaImportacion.DataSource = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetStockBodegaImportacion(INFtxtFiltroProveedorStockImportacion.Text, INFtxtfiltroMarcaStockImportacion.Text, INFcheckBoxCriticosStockImport.Checked));
                INFCheckStockMin(INFdataGridViewStockBodegaImportacion, "stock_imp", "stock_min_imp");
            }
        }

        private void PopulateListadoOTTable()
        {
            if (CheckDBConnection(false, true))
            {
                INFdataGridViewListadoOT.DataSource = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetElementoUtilizadoOT(INFtxtOT.Text));
            }
        }

        private void INFCheckStockMin(DataGridView datagrid, string stock_col, string stock_min_col)
        {
            foreach (DataGridViewRow row in datagrid.Rows)
            {
                if (Double.Parse(row.Cells[stock_col].Value.ToString()) <= Double.Parse(row.Cells[stock_min_col].Value.ToString()))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 102, 102);
                }
            }
        }

        private void SelectedTabChanged(object sender, EventArgs e)
        {
            switch (INFtabControlInformes.SelectedIndex)
            {
                case 0:
                    PopulateStockBodegaTable();
                    break;
                case 1:
                    PopulateStockImportacionBodegaTable();
                    break;
                case 2:
                    INFtxtOT.Focus();
                    break;
            }
        }


        private void INFbtnBuscarOT_Click(object sender, EventArgs e)
        {
            PopulateListadoOTTable();
        }

        private void INFbtnBuscarStock_Click(object sender, EventArgs e)
        {
            PopulateStockBodegaTable();
        }

        private void INFcheckBoxCriticos_CheckedChanged(object sender, EventArgs e)
        {
            PopulateStockBodegaTable();
        }

        private void INFcheckBoxCriticosStockImport_CheckedChanged(object sender, EventArgs e)
        {
            PopulateStockImportacionBodegaTable();
        }

        private void INFbtnRestablecerFiltroStock_Click(object sender, EventArgs e)
        {
            INFtxtFiltroProveedorStock.Clear();
            INFtxtfiltroMarcaStock.Clear();
            INFcheckBoxCriticosStock.Checked = false;
            PopulateStockBodegaTable();
        }

        private void INFbtnBuscarStockImportacion_Click(object sender, EventArgs e)
        {
            PopulateStockImportacionBodegaTable();
        }

        private void INFbtnRestablecerFiltroStockImportacion_Click(object sender, EventArgs e)
        {
            INFtxtFiltroProveedorStockImportacion.Clear();
            INFtxtfiltroMarcaStockImportacion.Clear();
            PopulateStockImportacionBodegaTable();
        }

        private void exportToExcel(DataGridView dataGrid, Boolean isBodega)
        {
            if (dataGrid.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook wb = excelApp.Workbooks.Add(Type.Missing);
                //Microsoft.Office.Interop.Excel.Worksheet sheet = null;

                if (isBodega)
                {
                    for (int i = 1; i < dataGrid.Columns.Count + 1; i++)
                    {
                        excelApp.Cells[1, i] = dataGrid.Columns[i - 1].HeaderText;
                    }

                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGrid.Columns.Count; j++)
                        {
                            excelApp.Cells[i + 2, j + 1] = dataGrid.Rows[i].Cells[j].Value;
                        }
                    }

                    excelApp.Range["A1", ("H" + (dataGrid.Rows.Count + 1))].Borders.LineStyle = true;

                    excelApp.Columns.AutoFit();
                    var header = excelApp.Range["A1", "H1"];
                    var col_valor = excelApp.Range["G2"].EntireColumn;
                    var col_valor_uni = excelApp.Range["H2"].EntireColumn;
                    col_valor.NumberFormat = "[$$-es-CL] #,##0";
                    col_valor_uni.NumberFormat = "[$$-es-CL] #,##0";
                    header.Font.Bold = true;
                    header.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightGray;
                    excelApp.Visible = true;
                }
                else
                {
                    excelApp.Cells[1, 1] = "LISTADO DE PARTES";
                    excelApp.Cells[3, 1] = "OT";
                    excelApp.Cells[4, 1] = "EQUIPO";
                    excelApp.Cells[5, 1] = "FECHA";

                    excelApp.Cells[1, 1].Font.Bold = true;
                    excelApp.Cells[1, 1].Font.Underline = true;
                    excelApp.Range["A3", "B5"].Borders.LineStyle = true;
                    excelApp.Cells[3, 1].Font.Bold = true;
                    excelApp.Cells[4, 1].Font.Bold = true;
                    excelApp.Cells[5, 1].Font.Bold = true;

                    excelApp.Cells[3, 2] = INFtxtOT.Text;

                    excelApp.Cells[10, 1] = "Item";

                    int item = 1;
                    double total = 0;
                    double subtotal = 0;
                    int rows = INFdataGridViewListadoOT.Rows.Count;

                    for (int i = 1; i < INFdataGridViewListadoOT.Columns.Count + 1; i++)
                    {
                        excelApp.Cells[10, i + 1] = INFdataGridViewListadoOT.Columns[i - 1].HeaderText;
                    }
                    for (int i = 0; i < rows; i++)
                    {
                        excelApp.Cells[i + 11, 1] = item;
                        item += 1;

                        for (int j = 0; j < INFdataGridViewListadoOT.Columns.Count; j++)
                        {
                            excelApp.Cells[i + 11, j + 2] = INFdataGridViewListadoOT.Rows[i].Cells[j].Value;
                        }
                        subtotal = Double.Parse(INFdataGridViewListadoOT.Rows[i].Cells[5].Value.ToString()) * Double.Parse(INFdataGridViewListadoOT.Rows[i].Cells[6].Value.ToString());
                        excelApp.Cells[i + 11, 11] = subtotal;
                        total += subtotal;
                    }

                    excelApp.Columns.AutoFit();
                    var header = excelApp.Range["A10", "K10"];
                    var col_valor = excelApp.Range["F11"].EntireColumn;
                    var col_valor_uni = excelApp.Range["G11"].EntireColumn;
                    var col_sub_total = excelApp.Range["K11"].EntireColumn;
                    col_valor.NumberFormat = "[$$-es-CL] #,##0";
                    col_valor_uni.NumberFormat = "[$$-es-CL] #,##0";
                    col_sub_total.NumberFormat = "[$$-es-CL] #,##0";
                    header.Font.Bold = true;
                    header.Interior.Color = Microsoft.Office.Interop.Excel.XlRgbColor.rgbLightGray;

                    excelApp.Range["A10", ("K" + (rows + 10))].Borders.LineStyle = true;

                    excelApp.Range[("J" + (rows + 11)), ("K" + (rows + 11))].Borders.LineStyle = true;
                    excelApp.Range[("J" + (rows + 11)), ("K" + (rows + 11))].Font.Bold = true;
                    excelApp.Cells[(rows + 11), 10] = "Total";
                    excelApp.Cells[(rows + 11), 11] = total;

                    excelApp.Visible = true;
                }
            }
        }

        private void INFbtnExportarStock_Click(object sender, EventArgs e)
        {
            exportToExcel(INFdataGridViewStockBodega, true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void INFbtnExportarStockImportacion_Click(object sender, EventArgs e)
        {
            exportToExcel(INFdataGridViewStockBodegaImportacion, true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void INFbtnExportarListadoOT_Click(object sender, EventArgs e)
        {
            exportToExcel(INFdataGridViewListadoOT, false);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        private void EnterKeySearchInforme(KeyEventArgs e, int informe)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                switch (informe)
                {
                    case 0:
                        PopulateStockBodegaTable();
                        break;
                    case 1:
                        PopulateStockImportacionBodegaTable();
                        break;
                    case 2:
                        PopulateListadoOTTable();
                        break;
                    default:
                        break;
                }

            }
        }

        private void INFtxtFiltroProveedorStock_KeyDown(object sender, KeyEventArgs e)
        {
            EnterKeySearchInforme(e, 0);
        }

        private void INFtxtfiltroMarcaStock_KeyDown(object sender, KeyEventArgs e)
        {
            EnterKeySearchInforme(e, 0);
        }

        private void INFtxtFiltroProveedorStockImportacion_KeyDown(object sender, KeyEventArgs e)
        {
            EnterKeySearchInforme(e, 1);
        }

        private void INFtxtfiltroMarcaStockImportacion_KeyDown(object sender, KeyEventArgs e)
        {
            EnterKeySearchInforme(e, 1);
        }

        private void INFtxtOT_KeyDown(object sender, KeyEventArgs e)
        {
            EnterKeySearchInforme(e, 2);
        }

        #endregion
        //Fin Metodos Informes

        //Metodos Crear Modificar Elemento
        #region Metodos Crear Modificar Elemento

        private void SidebarBtnCrearElemento_Click(object sender, EventArgs e)
        {
            if (btnActive.Name != SidebarBtnCrearElemento.Name)
            {
                SetDefaultPanelsAndButtons(panelActive, btnActive);
                AutoCompleteTextProveedor(CEcomboBoxProv);
                AutoCompleteTextMarca(CEcomboBoxMarca);
                SetVisible(BodyPanelCrearModifElemento, SidebarBtnCrearElemento);
                CEtxtDescripcion.Focus();
            }
        }

        private void CEbtnAgregarImg_Click(object sender, EventArgs e)
        {
            CEopenFileDialogImagen.Filter = "Imágenes|*.jpg;*.jpeg;*.png;";

            if (CEopenFileDialogImagen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //string iName = CEopenFileDialogImagen.SafeFileName;
                    //string filepath = CEopenFileDialogImagen.FileName;
                    //System.IO.File.Copy(filepath, Properties.Settings.Default.ImagePath + "/" + iName);
                    CEpictureBoxElem.Load(CEopenFileDialogImagen.FileName);
                    Console.WriteLine(CEpictureBoxElem.ImageLocation);
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Hubo un problema al cargar el archivo.\n" + exp.Message);
                }
            }
            else
            {
                CEpictureBoxElem.Image = CEpictureBoxElem.InitialImage;
            }
        }

        private void CEbtnAgregarProv_Click(object sender, EventArgs e)
        {
            Views.AgregarProveedor frmAddProveedor = new Views.AgregarProveedor(this, CEcomboBoxProv);
            frmAddProveedor.ShowDialog();
            frmAddProveedor.Dispose();
        }

        private void CEbtnAgregarMarca_Click(object sender, EventArgs e)
        {
            Views.AgregarMarca frmAddMarca = new Views.AgregarMarca(this, CEcomboBoxMarca);
            frmAddMarca.ShowDialog();
            frmAddMarca.Dispose();
        }

        private void CEbtnCancelar_Click(object sender, EventArgs e)
        {
            CEtxtDescripcion.Clear();
            CEtxtCodBodega.Clear();
            CEtxtPartePlano.Clear();
            CEtxtValor.Text = "0";
            CEtxtValorUnitario.Text = "0";
            CEtxtStock.Text = "1";
            CEtxtStockMin.Text = "1";
            CEtxtDocumento.Clear();
            CEtxtCodProv.Clear();
            CEtxtObs.Clear();
            CEradioButtonMetros.Checked = true;
            CEpictureBoxElem.Image = CEpictureBoxElem.InitialImage;

            try
            {
                CEcomboBoxProv.SelectedIndex = 0;
                CEcomboBoxMarca.SelectedIndex = 0;
            }
            catch (Exception)
            {
                
            }
        }

        private void CEbtnAgregar_Click(object sender, EventArgs e)
        {

        }

        private void CEcheckUnidadStock(object sender, KeyPressEventArgs e)
        {
            if (CEradioButtonMetros.Checked)
                CheckNumberDecimal(e, CEtxtStock);
            else
                CheckNumberOnly(sender, e);
        }

        private void CEcheckUnidadStockMin(object sender, KeyPressEventArgs e)
        {
            if (CEradioButtonMetros.Checked)
            {
                CheckNumberDecimal(e, CEtxtStockMin);
            }
            else
                CheckNumberOnly(sender, e);
        }

        private void CEParseDecimal(object sender, EventArgs e)
        {
            ParseDecimal(CEtxtStock);
        }

        private void CECheckChange(object sender, EventArgs e)
        {
            CEtxtStock.Text = "1";
            CEtxtStockMin.Text = "1";
        }

        private void CMtxtValorUnitario_Leave(object sender, EventArgs e)
        {
            CEtxtValorUnitario.Text = "0";
        }

        #endregion
        //Fin Metodos Crear Modificar Elemento
    }
}
