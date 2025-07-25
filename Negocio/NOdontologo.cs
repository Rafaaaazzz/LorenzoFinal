using Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NOdontologo
    {
        private DOdontologo dOdontologo = new DOdontologo();

        public String Registrar(Odontologo odontologo)
        {
            if (string.IsNullOrWhiteSpace(odontologo.CodigoOdontologo))
            {
                return "Error: El código del odontólogo es obligatorio";
            }

            if (string.IsNullOrWhiteSpace(odontologo.NombreCompleto))
            {
                return "Error: El nombre completo es obligatorio";
            }

            if (odontologo.CodigoOdontologo.Length > 20)
            {
                return "Error: El código no puede exceder 20 caracteres";
            }

            if (odontologo.NombreCompleto.Length > 100)
            {
                return "Error: El nombre no puede exceder 100 caracteres";
            }

            odontologo.CodigoOdontologo = odontologo.CodigoOdontologo.Trim().ToUpper();
            odontologo.NombreCompleto = odontologo.NombreCompleto.Trim();

            return dOdontologo.Registrar(odontologo);
        }

        public String Modificar(Odontologo odontologo)
        {
            if (odontologo.OdontologoId <= 0)
            {
                return "Error: ID del odontólogo inválido";
            }

            if (string.IsNullOrWhiteSpace(odontologo.CodigoOdontologo))
            {
                return "Error: El código del odontólogo es obligatorio";
            }

            if (string.IsNullOrWhiteSpace(odontologo.NombreCompleto))
            {
                return "Error: El nombre completo es obligatorio";
            }

            if (odontologo.CodigoOdontologo.Length > 20)
            {
                return "Error: El código no puede exceder 20 caracteres";
            }

            if (odontologo.NombreCompleto.Length > 100)
            {
                return "Error: El nombre no puede exceder 100 caracteres";
            }

            odontologo.CodigoOdontologo = odontologo.CodigoOdontologo.Trim().ToUpper();
            odontologo.NombreCompleto = odontologo.NombreCompleto.Trim();

            return dOdontologo.Modificar(odontologo);
        }

        public String Eliminar(int id)
        {
            if (id <= 0)
            {
                return "Error: ID del odontólogo inválido";
            }

            return dOdontologo.Eliminar(id);
        }

        public List<Odontologo> ListarTodo()
        {
            return dOdontologo.ListarTodo();
        }

        public List<Odontologo> ListarActivos()
        {
            return dOdontologo.ListarActivos();
        }

        public Odontologo ObtenerPorId(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return dOdontologo.ObtenerPorId(id);
        }

        public bool ValidarCodigoUnico(string codigo, int? odontologoId = null)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return false;
            }

            var odontologos = ListarTodo();
            codigo = codigo.Trim().ToUpper();

            if (odontologoId.HasValue)
            {
                return !odontologos.Any(o => o.CodigoOdontologo.ToUpper() == codigo && o.OdontologoId != odontologoId.Value);
            }
            else
            {
                return !odontologos.Any(o => o.CodigoOdontologo.ToUpper() == codigo);
            }
        }

        public String CambiarEstado(int id, bool activo)
        {
            var odontologo = ObtenerPorId(id);
            if (odontologo == null)
            {
                return "Error: Odontólogo no encontrado";
            }

            odontologo.Activo = activo;
            return Modificar(odontologo);
        }

        public List<Odontologo> BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return new List<Odontologo>();
            }

            var todosLosOdontologos = ListarTodo();
            nombre = nombre.Trim().ToLower();

            return todosLosOdontologos
                   .Where(o => o.NombreCompleto.ToLower().Contains(nombre))
                   .OrderBy(o => o.NombreCompleto)
                   .ToList();
        }
    }
}