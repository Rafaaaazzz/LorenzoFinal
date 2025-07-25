using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class DConsulta
    {
        public String Registrar(Consulta consulta)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    if (consulta.FechaHoraConsulta < DateTime.Now)
                    {
                        return "Error: La fecha y hora de la consulta no puede ser menor a la fecha actual";
                    }

                    var odontologo = context.Odontologo.Find(consulta.OdontologoId);
                    var paciente = context.Paciente.Find(consulta.PacienteId);

                    if (odontologo == null || odontologo.Eliminado || !odontologo.Activo)
                    {
                        return "Error: El odontólogo seleccionado no está disponible";
                    }

                    if (paciente == null || paciente.Eliminado || !paciente.Activo)
                    {
                        return "Error: El paciente seleccionado no está disponible";
                    }

                    consulta.FechaRegistro = DateTime.Now;
                    consulta.Realizada = false;
                    consulta.Eliminado = false;

                    context.Consulta.Add(consulta);
                    context.SaveChanges();
                }
                return "Consulta registrada exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public String Modificar(Consulta consulta)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    Consulta consultaTemp = context.Consulta.Find(consulta.ConsultaId);
                    if (consultaTemp == null || consultaTemp.Eliminado)
                    {
                        return "Error: Consulta no encontrada";
                    }

                    if (consultaTemp.Realizada)
                    {
                        return "Error: No se puede modificar una consulta que ya fue realizada";
                    }

                    if (consulta.FechaHoraConsulta < DateTime.Now && !consulta.Realizada)
                    {
                        return "Error: La fecha y hora de la consulta no puede ser menor a la fecha actual";
                    }

                    var odontologo = context.Odontologo.Find(consulta.OdontologoId);
                    var paciente = context.Paciente.Find(consulta.PacienteId);

                    if (odontologo == null || odontologo.Eliminado || !odontologo.Activo)
                    {
                        return "Error: El odontólogo seleccionado no está disponible";
                    }

                    if (paciente == null || paciente.Eliminado || !paciente.Activo)
                    {
                        return "Error: El paciente seleccionado no está disponible";
                    }

                    consultaTemp.TipoConsulta = consulta.TipoConsulta;
                    consultaTemp.FechaHoraConsulta = consulta.FechaHoraConsulta;
                    consultaTemp.Realizada = consulta.Realizada;
                    consultaTemp.OdontologoId = consulta.OdontologoId;
                    consultaTemp.PacienteId = consulta.PacienteId;

                    context.SaveChanges();
                }
                return "Consulta modificada exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public String MarcarComoRealizada(int consultaId)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    Consulta consultaTemp = context.Consulta.Find(consultaId);
                    if (consultaTemp == null || consultaTemp.Eliminado)
                    {
                        return "Error: Consulta no encontrada";
                    }

                    consultaTemp.Realizada = true;
                    context.SaveChanges();
                }
                return "Consulta marcada como realizada";
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
                    Consulta consultaTemp = context.Consulta.Find(id);
                    if (consultaTemp == null || consultaTemp.Eliminado)
                    {
                        return "Error: Consulta no encontrada";
                    }

                    consultaTemp.Eliminado = true;
                    context.SaveChanges();
                }
                return "Consulta eliminada exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public List<Consulta> ListarTodo()
        {
            List<Consulta> consultas = new List<Consulta>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    consultas = context.Consulta
                                       .Include("Odontologo")
                                       .Include("Paciente")
                                       .Where(c => !c.Eliminado)
                                       .OrderByDescending(c => c.FechaHoraConsulta)
                                       .ToList();
                }
                return consultas;
            }
            catch (Exception ex)
            {
                return consultas;
            }
        }

        public List<Consulta> ListarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            List<Consulta> consultas = new List<Consulta>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    consultas = context.Consulta
                                       .Include("Odontologo")
                                       .Include("Paciente")
                                       .Where(c => !c.Eliminado
                                                  && c.FechaHoraConsulta >= fechaInicio
                                                  && c.FechaHoraConsulta <= fechaFin)
                                       .OrderBy(c => c.FechaHoraConsulta)
                                       .ToList();
                }
                return consultas;
            }
            catch (Exception ex)
            {
                return consultas;
            }
        }

        public int ContarConsultasRealizadasPorOdontologoEnRango(int odontologoId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    return context.Consulta
                                  .Count(c => !c.Eliminado
                                             && c.Realizada
                                             && c.OdontologoId == odontologoId
                                             && c.FechaHoraConsulta >= fechaInicio
                                             && c.FechaHoraConsulta <= fechaFin);

                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<Consulta> ListarConsultasRealizadasPorOdontologoEnRango(int odontologoId, DateTime fechaInicio, DateTime fechaFin)
        {
            List<Consulta> consultas = new List<Consulta>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    consultas = context.Consulta
                                       .Include("Odontologo")
                                       .Include("Paciente")
                                       .Where(c => !c.Eliminado
                                                  && c.Realizada
                                                  && c.OdontologoId == odontologoId
                                                  && c.FechaHoraConsulta >= fechaInicio
                                                  && c.FechaHoraConsulta <= fechaFin)
                                       .OrderBy(c => c.FechaHoraConsulta)
                                       .ToList();
                }
                return consultas;
            }
            catch (Exception ex)
            {
                return consultas;
            }
        }

        public Consulta ObtenerPorId(int id)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    return context.Consulta
                                  .Include("Odontologo")
                                  .Include("Paciente")
                                  .FirstOrDefault(c => c.ConsultaId == id && !c.Eliminado);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}