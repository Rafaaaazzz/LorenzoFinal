using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Datos;
using Negocio;

namespace Presentacion
{
    public partial class Form1 : Form
    {
        private NConsulta nConsulta = new NConsulta();
        private NOdontologo nOdontologo = new NOdontologo();
        private NPaciente nPaciente = new NPaciente();
        private Consulta consultaSeleccionada = null;

        public Form1()
        {
            InitializeComponent();
            CargarDatosIniciales();
        }

        private void CargarDatosIniciales()
        {
            CargarTiposConsulta();
            CargarOdontologos();
            CargarPacientes();
            CargarOdontologosReporte();
            MostrarConsultas();
            ConfigurarDateTimePickers();
        }

        private void ConfigurarDateTimePickers()
        {
            dtpFechaHoraConsulta.Format = DateTimePickerFormat.Custom;
            dtpFechaHoraConsulta.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpFechaHoraConsulta.ShowUpDown = false;
            dtpFechaHoraConsulta.Value = DateTime.Now.AddHours(2);

            dtpFechaInicio.Format = DateTimePickerFormat.Short;
            dtpFechaFin.Format = DateTimePickerFormat.Short;
            dtpFechaInicio.Value = DateTime.Today.AddDays(-30);
            dtpFechaFin.Value = DateTime.Today;

            dtpFechaInicioOdon.Format = DateTimePickerFormat.Short;
            dtpFechaFinOdon.Format = DateTimePickerFormat.Short;
            dtpFechaInicioOdon.Value = DateTime.Today.AddDays(-30);
            dtpFechaFinOdon.Value = DateTime.Today;
        }

        private void CargarTiposConsulta()
        {
            try
            {
                cboTipoConsulta.Items.Clear();
                var tipos = nConsulta.ObtenerTiposConsulta();
                foreach (var tipo in tipos)
                {
                    cboTipoConsulta.Items.Add(tipo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar tipos de consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarOdontologos()
        {
            try
            {
                cboOdontologo.DataSource = null;
                var odontologos = nOdontologo.ListarActivos();
                cboOdontologo.DataSource = odontologos;
                cboOdontologo.DisplayMember = "NombreCompleto";
                cboOdontologo.ValueMember = "OdontologoId";
                cboOdontologo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar odontólogos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPacientes()
        {
            try
            {
                cboPaciente.DataSource = null;
                var pacientes = nPaciente.ListarActivos();
                cboPaciente.DataSource = pacientes;
                cboPaciente.DisplayMember = "NombreCompleto";
                cboPaciente.ValueMember = "PacienteId";
                cboPaciente.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pacientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarOdontologosReporte()
        {
            try
            {
                cboOdontologoReporte.DataSource = null;
                var odontologos = nOdontologo.ListarTodo();
                cboOdontologoReporte.DataSource = odontologos;
                cboOdontologoReporte.DisplayMember = "NombreCompleto";
                cboOdontologoReporte.ValueMember = "OdontologoId";
                cboOdontologoReporte.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar odontólogos para reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarConsultas()
        {
            try
            {
                dgvConsultas.DataSource = null;
                var consultas = nConsulta.ListarTodo();

                var consultasFormateadas = consultas.Select(c => new
                {
                    c.ConsultaId,
                    TipoConsulta = c.TipoConsulta,
                    FechaHora = c.FechaHoraConsulta.ToString("dd/MM/yyyy HH:mm"),
                    Paciente = c.Paciente?.NombreCompleto ?? "N/A",
                    Odontologo = c.Odontologo?.NombreCompleto ?? "N/A",
                    Estado = c.Realizada ? "Realizada" : "Pendiente",
                    FechaRegistro = c.FechaRegistro.ToString("dd/MM/yyyy HH:mm"),
                    c.Realizada,
                    c.OdontologoId,
                    c.PacienteId,
                    c.FechaHoraConsulta
                }).ToList();

                dgvConsultas.DataSource = consultasFormateadas;

                if (dgvConsultas.Columns["ConsultaId"] != null)
                    dgvConsultas.Columns["ConsultaId"].Visible = false;
                if (dgvConsultas.Columns["Realizada"] != null)
                    dgvConsultas.Columns["Realizada"].Visible = false;
                if (dgvConsultas.Columns["OdontologoId"] != null)
                    dgvConsultas.Columns["OdontologoId"].Visible = false;
                if (dgvConsultas.Columns["PacienteId"] != null)
                    dgvConsultas.Columns["PacienteId"].Visible = false;
                if (dgvConsultas.Columns["FechaHoraConsulta"] != null)
                    dgvConsultas.Columns["FechaHoraConsulta"].Visible = false;

                if (dgvConsultas.Columns["TipoConsulta"] != null)
                    dgvConsultas.Columns["TipoConsulta"].HeaderText = "Tipo Consulta";
                if (dgvConsultas.Columns["FechaHora"] != null)
                    dgvConsultas.Columns["FechaHora"].HeaderText = "Fecha y Hora";
                if (dgvConsultas.Columns["FechaRegistro"] != null)
                    dgvConsultas.Columns["FechaRegistro"].HeaderText = "Fecha Registro";

                lblTotalConsultas.Text = $"Total de consultas: {consultas.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar consultas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboTipoConsulta.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione un tipo de consulta", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboOdontologo.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un odontólogo", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboPaciente.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un paciente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Consulta consulta = new Consulta
                {
                    TipoConsulta = cboTipoConsulta.SelectedItem.ToString(),
                    FechaHoraConsulta = dtpFechaHoraConsulta.Value,
                    OdontologoId = (int)cboOdontologo.SelectedValue,
                    PacienteId = (int)cboPaciente.SelectedValue
                };

                string mensaje = nConsulta.Registrar(consulta);
                MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK,
                    mensaje.Contains("Error") ? MessageBoxIcon.Error : MessageBoxIcon.Information);

                if (!mensaje.Contains("Error"))
                {
                    LimpiarCampos();
                    MostrarConsultas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (consultaSeleccionada == null)
                {
                    MessageBox.Show("Seleccione una consulta para modificar", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboTipoConsulta.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione un tipo de consulta", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboOdontologo.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un odontólogo", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboPaciente.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un paciente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Consulta consulta = new Consulta
                {
                    ConsultaId = consultaSeleccionada.ConsultaId,
                    TipoConsulta = cboTipoConsulta.SelectedItem.ToString(),
                    FechaHoraConsulta = dtpFechaHoraConsulta.Value,
                    OdontologoId = (int)cboOdontologo.SelectedValue,
                    PacienteId = (int)cboPaciente.SelectedValue,
                    Realizada = chkRealizada.Checked
                };

                string mensaje = nConsulta.Modificar(consulta);
                MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK,
                    mensaje.Contains("Error") ? MessageBoxIcon.Error : MessageBoxIcon.Information);

                if (!mensaje.Contains("Error"))
                {
                    LimpiarCampos();
                    MostrarConsultas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (consultaSeleccionada == null)
                {
                    MessageBox.Show("Seleccione una consulta para eliminar", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult resultado = MessageBox.Show(
                    "¿Está seguro de que desea eliminar esta consulta?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    string mensaje = nConsulta.Eliminar(consultaSeleccionada.ConsultaId);
                    MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK,
                        mensaje.Contains("Error") ? MessageBoxIcon.Error : MessageBoxIcon.Information);

                    if (!mensaje.Contains("Error"))
                    {
                        LimpiarCampos();
                        MostrarConsultas();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvConsultas_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvConsultas.CurrentRow != null && dgvConsultas.CurrentRow.DataBoundItem != null)
                {
                    dynamic fila = dgvConsultas.CurrentRow.DataBoundItem;

                    consultaSeleccionada = new Consulta
                    {
                        ConsultaId = fila.ConsultaId,
                        TipoConsulta = fila.TipoConsulta,
                        FechaHoraConsulta = fila.FechaHoraConsulta,
                        OdontologoId = fila.OdontologoId,
                        PacienteId = fila.PacienteId,
                        Realizada = fila.Realizada
                    };

                    cboTipoConsulta.SelectedItem = consultaSeleccionada.TipoConsulta;
                    dtpFechaHoraConsulta.Value = consultaSeleccionada.FechaHoraConsulta;
                    cboOdontologo.SelectedValue = consultaSeleccionada.OdontologoId;
                    cboPaciente.SelectedValue = consultaSeleccionada.PacienteId;
                    chkRealizada.Checked = consultaSeleccionada.Realizada;

                    bool esRealizada = consultaSeleccionada.Realizada;
                    cboTipoConsulta.Enabled = !esRealizada;
                    dtpFechaHoraConsulta.Enabled = !esRealizada;
                    cboOdontologo.Enabled = !esRealizada;
                    cboPaciente.Enabled = !esRealizada;
                    btnModificar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar consulta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fechaInicio = dtpFechaInicio.Value.Date;
                DateTime fechaFin = dtpFechaFin.Value.Date;

                if (fechaInicio > fechaFin)
                {
                    MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha fin", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var consultas = nConsulta.ListarPorRangoFechas(fechaInicio, fechaFin);

                if (consultas.Count == 0)
                {
                    MessageBox.Show("No se encontraron consultas en el rango de fechas seleccionado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var consultasReporte = consultas.Select(c => new
                {
                    Fecha = c.FechaHoraConsulta.ToString("dd/MM/yyyy"),
                    Hora = c.FechaHoraConsulta.ToString("HH:mm"),
                    Paciente = c.Paciente?.NombreCompleto ?? "N/A",
                    Odontologo = c.Odontologo?.NombreCompleto ?? "N/A",
                    TipoConsulta = c.TipoConsulta,
                    Estado = c.Realizada ? "Realizada" : "Pendiente"
                }).ToList();

                dgvReporte.DataSource = consultasReporte;
                lblTotalReporte.Text = $"Total de consultas encontradas: {consultas.Count}";

                string reporteTexto = nConsulta.GenerarReporteConsultasPorRango(fechaInicio, fechaFin);
                txtReporte.Text = reporteTexto;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerarReporteOdontologo_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboOdontologoReporte.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un odontólogo", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime fechaInicio = dtpFechaInicioOdon.Value.Date;
                DateTime fechaFin = dtpFechaFinOdon.Value.Date;

                if (fechaInicio > fechaFin)
                {
                    MessageBox.Show("La fecha de inicio no puede ser mayor a la fecha fin", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int odontologoId = (int)cboOdontologoReporte.SelectedValue;
                int cantidad = nConsulta.ContarConsultasRealizadasPorOdontologoEnRango(odontologoId, fechaInicio, fechaFin);

                lblCantidadConsultas.Text = $"Consultas realizadas: {cantidad}";

                if (cantidad == 0)
                {
                    MessageBox.Show("El odontólogo no tiene consultas realizadas en el rango de fechas seleccionado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvReporteOdontologo.DataSource = null;
                    txtReporteOdontologo.Text = "";
                    return;
                }

                var consultas = nConsulta.ListarConsultasRealizadasPorOdontologoEnRango(odontologoId, fechaInicio, fechaFin);

                var consultasReporte = consultas.Select(c => new
                {
                    Fecha = c.FechaHoraConsulta.ToString("dd/MM/yyyy"),
                    Hora = c.FechaHoraConsulta.ToString("HH:mm"),
                    Paciente = c.Paciente?.NombreCompleto ?? "N/A",
                    TipoConsulta = c.TipoConsulta
                }).ToList();

                dgvReporteOdontologo.DataSource = consultasReporte;

                string reporteTexto = nConsulta.GenerarReporteOdontologoPorRango(odontologoId, fechaInicio, fechaFin);
                txtReporteOdontologo.Text = reporteTexto;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reporte de odontólogo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            cboTipoConsulta.SelectedIndex = -1;
            cboOdontologo.SelectedIndex = -1;
            cboPaciente.SelectedIndex = -1;
            dtpFechaHoraConsulta.Value = DateTime.Now.AddHours(2);
            chkRealizada.Checked = false;
            consultaSeleccionada = null;

            cboTipoConsulta.Enabled = true;
            dtpFechaHoraConsulta.Enabled = true;
            cboOdontologo.Enabled = true;
            cboPaciente.Enabled = true;
            btnModificar.Enabled = false;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarDatosIniciales();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Sistema de Gestión - Clínica Dental";
            this.WindowState = FormWindowState.Maximized;
            btnModificar.Enabled = false;
        }
    }
}