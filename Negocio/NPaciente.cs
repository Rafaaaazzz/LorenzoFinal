using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Negocio
{
    public class NPaciente
    {
        private DPaciente dPaciente = new DPaciente();

        public String Registrar(Paciente paciente)
        {
            if (string.IsNullOrWhiteSpace(paciente.NombreCompleto))
            {
                return "Error: El nombre completo es obligatorio";
            }

            if (string.IsNullOrWhiteSpace(paciente.DNI))
            {
                return "Error: El DNI es obligatorio";
            }

            if (paciente.FechaNacimiento == default(DateTime))
            {
                return "Error: La fecha de nacimiento es obligatoria";
            }

            if (!ValidarFormatoDNI(paciente.DNI))
            {
                return "Error: El DNI debe tener 8 dígitos";
            }

            if (paciente.NombreCompleto.Length > 100)
            {
                return "Error: El nombre no puede exceder 100 caracteres";
            }

            if (paciente.DNI.Length > 12)
            {
                return "Error: El DNI no puede exceder 12 caracteres";
            }

            var edad = CalcularEdad(paciente.FechaNacimiento);
            if (edad < 0)
            {
                return "Error: La fecha de nacimiento no puede ser futura";
            }

            if (edad > 120)
            {
                return "Error: La fecha de nacimiento no es válida";
            }

            paciente.NombreCompleto = paciente.NombreCompleto.Trim();
            paciente.DNI = paciente.DNI.Trim();

            return dPaciente.Registrar(paciente);
        }

        public String Modificar(Paciente paciente)
        {
            if (paciente.PacienteId <= 0)
            {
                return "Error: ID del paciente inválido";
            }

            if (string.IsNullOrWhiteSpace(paciente.NombreCompleto))
            {
                return "Error: El nombre completo es obligatorio";
            }

            if (string.IsNullOrWhiteSpace(paciente.DNI))
            {
                return "Error: El DNI es obligatorio";
            }

            if (paciente.FechaNacimiento == default(DateTime))
            {
                return "Error: La fecha de nacimiento es obligatoria";
            }

            if (!ValidarFormatoDNI(paciente.DNI))
            {
                return "Error: El DNI debe tener 8 dígitos";
            }

            if (paciente.NombreCompleto.Length > 100)
            {
                return "Error: El nombre no puede exceder 100 caracteres";
            }

            if (paciente.DNI.Length > 12)
            {
                return "Error: El DNI no puede exceder 12 caracteres";
            }

            var edad = CalcularEdad(paciente.FechaNacimiento);
            if (edad < 0)
            {
                return "Error: La fecha de nacimiento no puede ser futura";
            }

            if (edad > 120)
            {
                return "Error: La fecha de nacimiento no es válida";
            }

            paciente.NombreCompleto = paciente.NombreCompleto.Trim();
            paciente.DNI = paciente.DNI.Trim();

            return dPaciente.Modificar(paciente);
        }

        public String Eliminar(int id)
        {
            if (id <= 0)
            {
                return "Error: ID del paciente inválido";
            }

            return dPaciente.Eliminar(id);
        }

        public List<Paciente> ListarTodo()
        {
            return dPaciente.ListarTodo();
        }

        public List<Paciente> ListarActivos()
        {
            return dPaciente.ListarActivos();
        }

        public Paciente ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return dPaciente.ObtenerPorId(id);
        }

        public List<Paciente> BuscarPorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return new List<Paciente>();
            }

            return dPaciente.BuscarPorDNI(dni.Trim());
        }

        public bool ValidarDNIUnico(string dni, int? pacienteId = null)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return false;
            }

            var pacientes = ListarTodo();
            dni = dni.Trim();

            if (pacienteId.HasValue)
            {
                return !pacientes.Any(p => p.DNI == dni && p.PacienteId != pacienteId.Value);
            }
            else
            {
                return !pacientes.Any(p => p.DNI == dni);
            }
        }

        public String CambiarEstado(int id, bool activo)
        {
            var paciente = ObtenerPorId(id);
            if (paciente == null)
            {
                return "Error: Paciente no encontrado";
            }

            paciente.Activo = activo;
            return Modificar(paciente);
        }

        public List<Paciente> BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return new List<Paciente>();
            }

            var todosLosPacientes = ListarTodo();
            nombre = nombre.Trim().ToLower();

            return todosLosPacientes
                   .Where(p => p.NombreCompleto.ToLower().Contains(nombre))
                   .OrderBy(p => p.NombreCompleto)
                   .ToList();
        }

        private bool ValidarFormatoDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                return false;

            return Regex.IsMatch(dni.Trim(), @"^\d{8}$");
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;

            if (fechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;

            return edad;
        }

        public int ObtenerEdad(int pacienteId)
        {
            var paciente = ObtenerPorId(pacienteId);
            if (paciente == null)
                return 0;

            return CalcularEdad(paciente.FechaNacimiento);
        }

        public string ObtenerEdadTexto(int pacienteId)
        {
            var edad = ObtenerEdad(pacienteId);
            if (edad == 0)
                return "N/A";

            return $"{edad} años";
        }
    }
}