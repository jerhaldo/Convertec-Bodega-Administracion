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
            CheckDBConnection(true);
            this.panelSalidaIngreso.Dock = DockStyle.Fill;
            this.panelBodyIngreso.Dock = DockStyle.Fill;
            this.panelHistorialProd.Dock = DockStyle.Fill;

            panelActive = panelHistorialProd;
            btnActive = btnHistorial;
            PopulateProdHistTable();
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(panelHistorialProd, btnHistorial);
        }

        public void PopulateDataSalidaIngreso()
        {
            dataGridViewSalidas.DataSource = MovimientoBusiness.GetMovimientosSalidas();
            dataGridViewIngresos.DataSource = MovimientoBusiness.GetMovimientosIngresos();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelClock.Text = DateTime.Now.ToString("G");
        }

        private void CheckDBConnection(bool showError)
        {
            if (MovimientoBusiness.CheckDBConnection(showError))
            {
                iconPbDataBase.IconColor = Color.FromArgb(138, 183, 30);
                PopulateDataSalidaIngreso();
            }
            else
            {
                //MessageBox.Show("Error, No se ha podido establecer una conexión con la base de datos.", "Error de conexión.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                iconPbDataBase.IconColor = Color.FromArgb(255, 56, 0);
            }
        }

        private void IconPbDataBase_Click(object sender, EventArgs e)
        {
            //Forzar chequeo
            timer2.Stop();
            this.CheckDBConnection(true);
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //Cada 30 sec
            this.CheckDBConnection(false);
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

        private void btnPrincipal_Click(object sender, EventArgs e)
        {
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(panelSalidaIngreso, btnSalidaIngreso);
        }

        private void btnIngresarProd_Click(object sender, EventArgs e)
        {
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(panelBodyIngreso, btnIngresarProd);
        }

        private void PopulateProdHistTable()
        {
            tableProductoHist = MovimientoBusiness.ToDataTable(MovimientoBusiness.GetProductosDetalle());
            dataGridViewProdHist.DataSource = tableProductoHist;
            FormatTableProdHist(dataGridViewProdHist);
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            SetDefaultPanelsAndButtons(panelActive, btnActive);
            SetVisible(panelHistorialProd, btnHistorial);
            PopulateProdHistTable();
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

            foreach (DataGridViewColumn col in dataGridViewProdHist.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void TableFilter(object sender, EventArgs e)
        {
            DataView dv = tableProductoHist.DefaultView;
            dv.RowFilter = "(cod_bodega LIKE '%" + txtFiltroHist.Text + "%'"
                            + "or descripcion LIKE '%" + txtFiltroHist.Text + "%'"
                            + "or nom_proveedor LIKE '%" + txtFiltroHist.Text + "%'"
                            + "or nom_marca LIKE '%" + txtFiltroHist.Text + "%'"
                            + "or stock LIKE '%" + txtFiltroHist.Text + "%'"
                            + "or valor LIKE '%" + txtFiltroHist.Text + "%'"
                            + "or parte_plano LIKE '%" + txtFiltroHist.Text + "%')"
                            + "and ots LIKE '%" + txtOtFilter.Text + "%'";
            dataGridViewProdHist.DataSource = dv;
        }

        private int SelectedRow()
        {
            if (dataGridViewProdHist.SelectedCells.Count == 1 || dataGridViewProdHist.SelectedRows.Count == 1)
            {
                int selectedrowindex = dataGridViewProdHist.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridViewProdHist.Rows[selectedrowindex];
                int cellValue = Int32.Parse(selectedRow.Cells["id_producto"].Value.ToString());
                return cellValue;
            }
            return -1;
        }

        private void CargarDatos(object sender, EventArgs e)
        {
            txtOtHist.Clear();
            int id = SelectedRow();
            if (id != -1)
            {
                dataGridViewHistorial.DataSource = MovimientoBusiness.GetHistorial(id);
                foreach (DataGridViewRow row in dataGridViewHistorial.Rows)
                    if (row.Cells[5].Value != null)
                    {
                        //row.DefaultCellStyle.ForeColor = Color.FromArgb(138, 183, 30);
                        row.DefaultCellStyle.BackColor = Color.FromArgb(138, 183, 30);
                    }

                var img = MovimientoBusiness.GetImages(id);

                if (img != null)
                    pictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/imgProductos/" + img.image);
                else
                    pictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/logos/image-unavailable.png");

                foreach (Model.OtProducto ot in MovimientoBusiness.GetOtProducto(id))
                {
                    if (ot.ot != "")
                    {
                        txtOtHist.Text = txtOtHist.Text + ot.ot + Environment.NewLine;
                    }
                }
            }
            else
            {
                txtOtHist.Clear();
                pictureBoxProducto.Load(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "/Assets/logos/image-unavailable.png");
            }
        }
    }
}
