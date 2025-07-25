using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NConsulta
    {
        private DConsulta dConsulta = new DConsulta();
        private NOdontologo nOdontologo = new NOdontologo();
        private NPaciente nPaciente = new NPaciente();

        // Lista de tipos de consulta según el examen
        public List<string> ObtenerTiposConsulta()
        {
            return new List<string>
            {
                "Consulta Preventiva",
                "Consulta de Emergencia"
            };
        }

        public String Registrar(Consulta consulta)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(consulta.TipoConsulta))
            {
                return "Error: El tipo de consulta es obligatorio";
            }

            if (consulta.FechaHoraConsulta == default(DateTime))
            {
                return "Error: La fecha y hora de consulta son obligatorias";
            }

            if (consulta.OdontologoId <= 0)
            {
                return "Error: Debe seleccionar un odontólogo";
            }

            if (consulta.PacienteId <= 0)
            {
                return "Error: Debe seleccionar un paciente";
            }

            // Validar que el tipo de consulta sea válido
            if (!ObtenerTiposConsulta().Contains(consulta.TipoConsulta))
            {
                return "Error: Tipo de consulta no válido";
            }

            // Validar longitud del tipo de consulta
            if (consulta.TipoConsulta.Length > 30)
            {
                return "Error: El tipo de consulta no puede exceder 30 caracteres";
            }

            // Verificar que la fecha de consulta sea futura (al menos 1 hora desde ahora)
            if (consulta.FechaHoraConsulta <= DateTime.Now.AddHours(1))
            {
                return "Error: La consulta debe programarse con al menos 1 hora de anticipación";
            }

            // Verificar que no sea más de 1 año en el futuro
            if (consulta.FechaHoraConsulta > DateTime.Now.AddYears(1))
            {
                return "Error: No se pueden programar consultas con más de 1 año de anticipación";
            }

            // Verificar horarios de atención (ejemplo: 8:00 AM a 6:00 PM)
            var hora = consulta.FechaHoraConsulta.TimeOfDay;
            if (hora < new TimeSpan(8, 0, 0) || hora > new TimeSpan(18, 0, 0))
            {
                return "Error: Las consultas solo se pueden programar entre 8:00 AM y 6:00 PM";
            }

            // Verificar que no sea domingo
            if (consulta.FechaHoraConsulta.DayOfWeek == DayOfWeek.Sunday)
            {
                return "Error: No se pueden programar consultas los domingos";
            }

            // Verificar disponibilidad del odontólogo (que no tenga otra consulta en la misma hora)
            if (!VerificarDisponibilidadOdontologo(consulta.OdontologoId, consulta.FechaHoraConsulta))
            {
                return "Error: El odontólogo no está disponible en esa fecha y hora";
            }

            return dConsulta.Registrar(consulta);
        }

        public String Modificar(Consulta consulta)
        {
            // Validaciones de negocio
            if (consulta.ConsultaId <= 0)
            {
                return "Error: ID de consulta inválido";
            }

            if (string.IsNullOrWhiteSpace(consulta.TipoConsulta))
            {
                return "Error: El tipo de consulta es obligatorio";
            }

            if (consulta.FechaHoraConsulta == default(DateTime))
            {
                return "Error: La fecha y hora de consulta son obligatorias";
            }

            if (consulta.OdontologoId <= 0)
            {
                return "Error: Debe seleccionar un odontólogo";
            }

            if (consulta.PacienteId <= 0)
            {
                return "Error: Debe seleccionar un paciente";
            }

            // Validar que el tipo de consulta sea válido
            if (!ObtenerTiposConsulta().Contains(consulta.TipoConsulta))
            {
                return "Error: Tipo de consulta no válido";
            }

            // Obtener consulta actual para verificar estado
            var consultaActual = dConsulta.ObtenerPorId(consulta.ConsultaId);
            if (consultaActual == null)
            {
                return "Error: Consulta no encontrada";
            }

            // Si la consulta ya fue realizada, no se puede modificar
            if (consultaActual.Realizada)
            {
                return "Error: No se puede modificar una consulta que ya fue realizada";
            }

            // Si no está marcada como realizada, validar fecha futura
            if (!consulta.Realizada)
            {
                if (consulta.FechaHoraConsulta <= DateTime.Now.AddHours(1))
                {
                    return "Error: La consulta debe programarse con al menos 1 hora de anticipación";
                }

                // Verificar horarios de atención
                var hora = consulta.FechaHoraConsulta.TimeOfDay;
                if (hora < new TimeSpan(8, 0, 0) || hora > new TimeSpan(18, 0, 0))
                {
                    return "Error: Las consultas solo se pueden programar entre 8:00 AM y 6:00 PM";
                }

                // Verificar que no sea domingo
                if (consulta.FechaHoraConsulta.DayOfWeek == DayOfWeek.Sunday)
                {
                    return "Error: No se pueden programar consultas los domingos";
                }

                // Verificar disponibilidad del odontólogo (excluyendo la consulta actual)
                if (!VerificarDisponibilidadOdontologo(consulta.OdontologoId, consulta.FechaHoraConsulta, consulta.ConsultaId))
                {
                    return "Error: El odontólogo no está disponible en esa fecha y hora";
                }
            }

            return dConsulta.Modificar(consulta);
        }

        public String MarcarComoRealizada(int consultaId)
        {
            if (consultaId <= 0)
            {
                return "Error: ID de consulta inválido";
            }

            var consulta = dConsulta.ObtenerPorId(consultaId);
            if (consulta == null)
            {
                return "Error: Consulta no encontrada";
            }

            if (consulta.Realizada)
            {
                return "La consulta ya está marcada como realizada";
            }

            return dConsulta.MarcarComoRealizada(consultaId);
        }

        public String Eliminar(int id)
        {
            if (id <= 0)
            {
                return "Error: ID de consulta inválido";
            }

            return dConsulta.Eliminar(id);
        }

        public List<Consulta> ListarTodo()
        {
            return dConsulta.ListarTodo();
        }

        public List<Consulta> ListarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            if (fechaInicio > fechaFin)
            {
                return new List<Consulta>();
            }

            fechaInicio = fechaInicio.Date; 
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1); 

            return dConsulta.ListarPorRangoFechas(fechaInicio, fechaFin);
        }

        public int ContarConsultasRealizadasPorOdontologoEnRango(int odontologoId, DateTime fechaInicio, DateTime fechaFin)
        {
            if (odontologoId <= 0 || fechaInicio > fechaFin)
            {
                return 0;
            }

            fechaInicio = fechaInicio.Date;
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

            return dConsulta.ContarConsultasRealizadasPorOdontologoEnRango(odontologoId, fechaInicio, fechaFin);
        }

        public List<Consulta> ListarConsultasRealizadasPorOdontologoEnRango(int odontologoId, DateTime fechaInicio, DateTime fechaFin)
        {
            if (odontologoId <= 0 || fechaInicio > fechaFin)
            {
                return new List<Consulta>();
            }

            fechaInicio = fechaInicio.Date;
            fechaFin = fechaFin.Date.AddDays(1).AddTicks(-1);

            return dConsulta.ListarConsultasRealizadasPorOdontologoEnRango(odontologoId, fechaInicio, fechaFin);
        }

        public Consulta ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return dConsulta.ObtenerPorId(id);
        }

        public string GenerarReporteConsultasPorRango(DateTime fechaInicio, DateTime fechaFin)
        {
            var consultas = ListarPorRangoFechas(fechaInicio, fechaFin);

            var reporte = new StringBuilder();
            reporte.AppendLine($"REPORTE DE CONSULTAS - {fechaInicio:dd/MM/yyyy} AL {fechaFin:dd/MM/yyyy}");
            reporte.AppendLine(new string('=', 60));
            reporte.AppendLine($"Total de consultas: {consultas.Count}");
            reporte.AppendLine();

            foreach (var consulta in consultas)
            {
                reporte.AppendLine($"Fecha: {consulta.FechaHoraConsulta:dd/MM/yyyy HH:mm}");
                reporte.AppendLine($"Paciente: {consulta.Paciente?.NombreCompleto ?? "N/A"}");
                reporte.AppendLine($"Odontólogo: {consulta.Odontologo?.NombreCompleto ?? "N/A"}");
                reporte.AppendLine($"Tipo: {consulta.TipoConsulta}");
                reporte.AppendLine($"Estado: {(consulta.Realizada ? "Realizada" : "Pendiente")}");
                reporte.AppendLine(new string('-', 40));
            }

            return reporte.ToString();
        }

        public string GenerarReporteOdontologoPorRango(int odontologoId, DateTime fechaInicio, DateTime fechaFin)
        {
            var odontologo = nOdontologo.ObtenerPorId(odontologoId);
            if (odontologo == null)
            {
                return "Error: Odontólogo no encontrado";
            }

            var cantidad = ContarConsultasRealizadasPorOdontologoEnRango(odontologoId, fechaInicio, fechaFin);
            var consultas = ListarConsultasRealizadasPorOdontologoEnRango(odontologoId, fechaInicio, fechaFin);

            var reporte = new StringBuilder();
            reporte.AppendLine($"REPORTE DE CONSULTAS REALIZADAS");
            reporte.AppendLine($"Odontólogo: {odontologo.NombreCompleto}");
            reporte.AppendLine($"Período: {fechaInicio:dd/MM/yyyy} AL {fechaFin:dd/MM/yyyy}");
            reporte.AppendLine(new string('=', 60));
            reporte.AppendLine($"Total de consultas realizadas: {cantidad}");
            reporte.AppendLine();

            foreach (var consulta in consultas)
            {
                reporte.AppendLine($"Fecha: {consulta.FechaHoraConsulta:dd/MM/yyyy HH:mm}");
                reporte.AppendLine($"Paciente: {consulta.Paciente?.NombreCompleto ?? "N/A"}");
                reporte.AppendLine($"Tipo: {consulta.TipoConsulta}");
                reporte.AppendLine(new string('-', 30));
            }

            return reporte.ToString();
        }

        private bool VerificarDisponibilidadOdontologo(int odontologoId, DateTime fechaHora, int? consultaIdExcluir = null)
        {
            var todasLasConsultas = ListarTodo();

            var consultasConflictivas = todasLasConsultas.Where(c =>
                c.OdontologoId == odontologoId &&
                !c.Realizada &&
                (consultaIdExcluir == null || c.ConsultaId != consultaIdExcluir) &&
                Math.Abs((c.FechaHoraConsulta - fechaHora).TotalMinutes) < 60
            );

            return !consultasConflictivas.Any();
        }

        public List<Consulta> ListarPendientes()
        {
            return ListarTodo().Where(c => !c.Realizada).ToList();
        }

        public List<Consulta> ListarRealizadas()
        {
            return ListarTodo().Where(c => c.Realizada).ToList();
        }
    }
}