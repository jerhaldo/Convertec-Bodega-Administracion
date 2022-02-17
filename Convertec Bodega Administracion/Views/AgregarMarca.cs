using Convertec_Bodega_Administracion.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convertec_Bodega_Administracion.Views
{
    public partial class AgregarMarca : Form
    {
        private FormPrincipal principal;
        private ComboBox combo;

        public AgregarMarca(FormPrincipal Principal, ComboBox Combo)
        {
            InitializeComponent();
            principal = Principal;
            combo = Combo;

        }

        private void AlertMessage(string message, MessageBoxIcon icon)
        {
            string caption = "Error en Ingreso de Datos";
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            //Muestra el MessageBox.
            MessageBox.Show(message, caption, buttons, icon);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                var result = MessageBox.Show("Desea Guardar la Marca?", "Confirmación de guardado.",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes && principal.CheckDBConnection(false, true))
                {
                    Model.Marca marca = new Model.Marca
                    {
                        nom_marca = txtNombre.Text,

                    };
                    MovimientoBusiness.InsertMarca(marca);
                    principal.AutoCompleteTextMarca(combo);
                }
                this.Close();
            }
            else
            {
                AlertMessage("Por favor ingrese nombre de la marca.", MessageBoxIcon.Error);
                txtNombre.Focus();
            }
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {

                iconErrorNombre.Visible = true;
            }
            else
            {
                iconErrorNombre.Visible = false;
            }
        }

        private void txtNombre_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                iconErrorNombre.Visible = true;
            }
        }
    }
}
