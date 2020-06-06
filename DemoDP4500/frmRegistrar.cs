using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoDP4500
{
    public partial class frmRegistrar : Form
    {
        private DPFP.Template Template;
        private UsuariosDBEntities contexto;
        public frmRegistrar()
        {
            InitializeComponent();
        }

        private void btnRegistrarHuella_Click(object sender, EventArgs e)
        {
            CapturarHuella capturar = new CapturarHuella();
            capturar.OnTemplate += this.OnTemplate;
            capturar.ShowDialog();
        }

        private void OnTemplate(DPFP.Template template)
        {
            this.Invoke(new Function(delegate ()
            {
                Template = template;
                btnAgregar.Enabled = (Template != null);
                if (Template != null)
                {
                    MessageBox.Show("The fingerprint template is ready for fingerprint verification.", "Fingerprint Enrollment");
                    txtHuella.Text = "Huella capturada correctamente";
                }
                else
                {
                    MessageBox.Show("The fingerprint template is not valid. Repeat fingerprint enrollment.", "Fingerprint Enrollment");
                }
            }));
        }

        private void frmRegistrar_Load(object sender, EventArgs e)
        {
            contexto = new UsuariosDBEntities();
            Listar();
        }

        private void Limpiar()
        {
            txtNombre.Text = "";
        }

        private void Listar()
        {
            try
            {
                var empleados = from emp in contexto.Empleadoes
                                select new
                                {
                                    ID = emp.Id,
                                    EMPLEADO = emp.Nombre
                                };
                if (empleados!=null)
                {
                    dgvListar.DataSource = empleados.ToList();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] streamHuella = Template.Bytes;
                Empleado empleado = new Empleado()
                {
                    Nombre = txtNombre.Text,
                    Huella = streamHuella
                };

                contexto.Empleadoes.Add(empleado);
                contexto.SaveChanges();
                MessageBox.Show("Registro agregado a la BD correctamente");
                Limpiar();
                Listar();
                Template = null;
                btnAgregar.Enabled = false;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
