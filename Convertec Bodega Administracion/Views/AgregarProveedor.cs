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
    public partial class AgregarProveedor : Form
    {
        private string patronEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private FormPrincipal principal;
        private bool errorEmail = false;

        public AgregarProveedor(FormPrincipal Principal)
        {
            InitializeComponent();
            principal = Principal;

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
                if (!errorEmail)
                {
                    var result = MessageBox.Show("Desea Guardar al Proveedor?", "Confirmación de guardado.",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes && MovimientoBusiness.CheckDBConnection(true))
                    {
                        Model.Proveedor proveedor = new Model.Proveedor
                        {
                            nom_proveedor = txtNombre.Text,
                            telefono = txtTelefono.Text,
                            email = txtEmail.Text,
                            vendedor = txtVendedor.Text,
                            rut_empresa = txtRutEmp.Text
                        };
                        MovimientoBusiness.InsertProveedor(proveedor);
                        principal.AutoCompleteTextProveedor();
                    }
                    this.Close();
                }
                else 
                {
                    AlertMessage("Por favor ingrese email valido.", MessageBoxIcon.Error);
                    txtEmail.Focus();
                }
            }
            else
            {
                AlertMessage("Por favor ingrese nombre de proveedor.", MessageBoxIcon.Error);
                txtNombre.Focus();
            }
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                iconErrorEmail.Visible = false;
                errorEmail = false;
            }
            else
            {
                if (!Regex.IsMatch(txtEmail.Text, patronEmail, RegexOptions.IgnoreCase))
                {
                    iconErrorEmail.Visible = true;
                    errorEmail = true;

                }
                else
                {
                    iconErrorEmail.Visible = false;
                    errorEmail = false;
                }
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
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
