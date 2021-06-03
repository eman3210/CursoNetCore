using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turnos.Models;

namespace Turnos.Controllers
{
    public class TurnoController : Controller
    {
        private readonly TurnosContext _context;
        private IConfiguration _configuration;

        public TurnoController(TurnosContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
           
            /* viewData: geneta listbox de medicos / desde el new hasta tolist, se listan todos los medicos / pero le decimos que solo extraiga de cada medico el id y el nombre completo, mediante select new */
            ViewData["IdMedico"] = new SelectList(from medico in _context.Medico.ToList() select new { IdMedico = medico.IdMedico, NombreCompleto = medico.Nombre + " " + medico.Apellido }, "IdMedico", "NombreCompleto");
            /* Generamos otro listbox ahora para los pacientes*/
            ViewData["IdPaciente"] = new SelectList(from paciente in _context.Paciente.ToList() select new { IdPaciente = paciente.IdPaciente, NombreCompleto = paciente.Nombre + " " + paciente.Apellido }, "IdPaciente", "NombreCompleto");
            
            return View();
        }

        public JsonResult ObtenerTurnos(int IdMedico) // devuelve Json porque mas tarde el fullCalendar lo pide en ese formato
        {
            List<Turno> turnos = new List<Turno>();
            turnos = _context.Turno.Where(t => t.IdMedico == IdMedico).ToList(); // llenamos el objeto turnos con todos los turnos del idmedico que le pasamos por aprametro

            return Json(turnos); // convierte a coleccion con formato Json
        }

        [HttpPost] 
        public JsonResult GrabarTurno(Turno turno)
        {
            var ok = false;
            try
            {
                _context.Turno.Add(turno);
                _context.SaveChanges();
                ok = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Excepcion encontrada", e);                
            }
            var jsonResult = new { ok = ok }; // creamos objeto jason donde informamos el booleano ok
            return Json(jsonResult); // convierte la variable a formato json
        }

        [HttpPost]
        public JsonResult EliminarTurno(int idTurno)
        {
            var ok = false;
            try
            {
                var turnoAEliminar = _context.Turno.Where(t => t.IdTurno == idTurno).FirstOrDefault();
                if (turnoAEliminar != null)
                {
                    _context.Turno.Remove(turnoAEliminar);
                    _context.SaveChanges();
                    ok = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Excepcion encontrada", e);
            }

            var jsonResult = new { ok = ok }; // creamos objeto jason donde informamos el booleano ok
            return Json(jsonResult); // convierte la variable a formato json
        }
    }
}
