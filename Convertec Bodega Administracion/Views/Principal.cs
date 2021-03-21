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

        private void SidebarBtnIngresarElemento_Click(object sender, EventArgs e)
        {
            AutoCompleteTextElemento();
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(BodyPanelIngresoElementos, SidebarBtnIngresarElemento);
        }

        private void AutoCompleteTextElemento()
        {
            AutoCompleteStringCollection collElemento = new AutoCompleteStringCollection();
            foreach (Model.IdDescripcionElemento elemento in MovimientoBusiness.GetElementoByName())
            {
                collElemento.Add(elemento.descripcion);
            }

            IEtxtDescripcion.AutoCompleteCustomSource = collElemento;

        }

        #endregion
        //Fin Metodos Ingreso de Elementos

        //Metodos Crear Elemento
        #region Metodos Crear Elemento
        #endregion
        //Fin Metodos Crear Elemento
    }
}
