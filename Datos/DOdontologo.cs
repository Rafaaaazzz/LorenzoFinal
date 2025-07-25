using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class DOdontologo
    {
        public String Registrar(Odontologo odontologo)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    var existe = context.Odontologo.Any(o => o.CodigoOdontologo == odontologo.CodigoOdontologo && !o.Eliminado);
                    if (existe)
                    {
                        return "Error: El código del odontólogo ya existe";
                    }

                    odontologo.FechaCreacion = DateTime.Now;
                    odontologo.Activo = true;
                    odontologo.Eliminado = false;

                    context.Odontologo.Add(odontologo);
                    context.SaveChanges();
                }
                return "Odontólogo registrado exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public String Modificar(Odontologo odontologo)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    var existe = context.Odontologo.Any(o => o.CodigoOdontologo == odontologo.CodigoOdontologo
                                                            && o.OdontologoId != odontologo.OdontologoId
                                                            && !o.Eliminado);
                    if (existe)
                    {
                        return "Error: El código del odontólogo ya existe";
                    }

                    Odontologo odontologoTemp = context.Odontologo.Find(odontologo.OdontologoId);
                    if (odontologoTemp == null || odontologoTemp.Eliminado)
                    {
                        return "Error: Odontólogo no encontrado";
                    }

                    odontologoTemp.CodigoOdontologo = odontologo.CodigoOdontologo;
                    odontologoTemp.NombreCompleto = odontologo.NombreCompleto;
                    odontologoTemp.Activo = odontologo.Activo;

                    context.SaveChanges();
                }
                return "Odontólogo modificado exitosamente";
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
                    Odontologo odontologoTemp = context.Odontologo.Find(id);
                    if (odontologoTemp == null || odontologoTemp.Eliminado)
                    {
                        return "Error: Odontólogo no encontrado";
                    }

                    var tieneConsultas = context.Consulta.Any(c => c.OdontologoId == id && !c.Eliminado);
                    if (tieneConsultas)
                    {
                        return "Error: No se puede eliminar. El odontólogo tiene consultas registradas";
                    }

                    odontologoTemp.Eliminado = true;
                    odontologoTemp.Activo = false;
                    context.SaveChanges();
                }
                return "Odontólogo eliminado exitosamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public List<Odontologo> ListarTodo()
        {
            List<Odontologo> odontologos = new List<Odontologo>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    odontologos = context.Odontologo
                                         .Where(o => !o.Eliminado)
                                         .OrderBy(o => o.NombreCompleto)
                                         .ToList();
                }
                return odontologos;
            }
            catch (Exception ex)
            {
                return odontologos;
            }
        }

        public List<Odontologo> ListarActivos()
        {
            List<Odontologo> odontologos = new List<Odontologo>();
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    odontologos = context.Odontologo
                                         .Where(o => !o.Eliminado && o.Activo)
                                         .OrderBy(o => o.NombreCompleto)
                                         .ToList();
                }
                return odontologos;
            }
            catch (Exception ex)
            {
                return odontologos;
            }
        }

        public Odontologo ObtenerPorId(int id)
        {
            try
            {
                using (var context = new EF_ClinicaDentalEntities())
                {
                    return context.Odontologo
                                  .FirstOrDefault(o => o.OdontologoId == id && !o.Eliminado);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}