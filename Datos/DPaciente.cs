using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class DPaciente
    {
        public String Registrar(Paciente paciente)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    var existe = context.Paciente.Any(p => p.DNI == paciente.DNI && !p.Eliminado);
                    if (existe)
                    {
                        return "Error: El DNI del paciente ya existe";
                    }

                    paciente.FechaCreacion = DateTime.Now;
                    paciente.Activo = true;
                    paciente.Eliminado = false;

                    context.Paciente.Add(paciente);
                    context.SaveChanges();
                }
                return "Paciente registrado exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public String Modificar(Paciente paciente)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    var existe = context.Paciente.Any(p => p.DNI == paciente.DNI
                                                           && p.PacienteId != paciente.PacienteId
                                                           && !p.Eliminado);
                    if (existe)
                    {
                        return "Error: El DNI del paciente ya existe";
                    }

                    Paciente pacienteTemp = context.Paciente.Find(paciente.PacienteId);
                    if (pacienteTemp == null || pacienteTemp.Eliminado)
                    {
                        return "Error: Paciente no encontrado";
                    }

                    pacienteTemp.NombreCompleto = paciente.NombreCompleto;
                    pacienteTemp.DNI = paciente.DNI;
                    pacienteTemp.FechaNacimiento = paciente.FechaNacimiento;
                    pacienteTemp.Activo = paciente.Activo;

                    context.SaveChanges();
                }
                return "Paciente modificado exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public String Eliminar(int id)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    Paciente pacienteTemp = context.Paciente.Find(id);
                    if (pacienteTemp == null || pacienteTemp.Eliminado)
                    {
                        return "Error: Paciente no encontrado";
                    }

                    var tieneConsultas = context.Consulta.Any(c => c.PacienteId == id && !c.Eliminado);
                    if (tieneConsultas)
                    {
                        return "Error: No se puede eliminar. El paciente tiene consultas registradas";
                    }

                    pacienteTemp.Eliminado = true;
                    pacienteTemp.Activo = false;
                    context.SaveChanges();
                }
                return "Paciente eliminado exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public List<Paciente> ListarTodo()
        {
            List<Paciente> pacientes = new List<Paciente>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    pacientes = context.Paciente
                                       .Where(p => !p.Eliminado)
                                       .OrderBy(p => p.NombreCompleto)
                                       .ToList();
                }
                return pacientes;
            }
            catch (Exception ex)
            {
                return pacientes;
            }
        }

        public List<Paciente> ListarActivos()
        {
            List<Paciente> pacientes = new List<Paciente>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    pacientes = context.Paciente
                                       .Where(p => !p.Eliminado && p.Activo)
                                       .OrderBy(p => p.NombreCompleto)
                                       .ToList();
                }
                return pacientes;
            }
            catch (Exception ex)
            {
                return pacientes;
            }
        }

        public Paciente ObtenerPorId(int id)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    return context.Paciente
                                  .FirstOrDefault(p => p.PacienteId == id && !p.Eliminado);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Paciente> BuscarPorDNI(string dni)
        {
            List<Paciente> pacientes = new List<Paciente>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    pacientes = context.Paciente
                                       .Where(p => p.DNI.Contains(dni) && !p.Eliminado)
                                       .OrderBy(p => p.NombreCompleto)
                                       .ToList();
                }
                return pacientes;
            }
            catch (Exception ex)
            {
                return pacientes;
            }
        }
    }
}