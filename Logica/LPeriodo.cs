using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidad;
using Entidad.Estados;
namespace Logica
{
    public class LPeriodo : LLogica<Periodo>
    {
        public List<Periodo> listarPeriodo(long idgestion)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var periodo = (from x in esquema.Periodo
                                   where x.IdGestion == idgestion
                                   select x).ToList();

                    List<Periodo> periodos = new List<Periodo>();

                    periodos = periodo;

                    return periodos;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de periodos");
                }

            }
        }

        public Periodo obtenerPeriodo(long idperiodo)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var periodo = (from x in esquema.Periodo
                                   where x.idPeriodo == idperiodo
                                   select x).FirstOrDefault();

                 

                    return periodo;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de periodos");
                }

            }
        }



        public Periodo Registro(Periodo Entidad)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad);


                    var gestion = LGestion.Logica.LGestion.obtenerGestion(Entidad.IdGestion);

                    if(gestion == null)
                    {
                        throw new MensageException("No se pudo encontrar la gestion");
                    }

                    if( !(Entidad.FechaInicio >= gestion.FechaInicio && Entidad.FechaInicio <= gestion.FechaFin))
                    {
                        throw new MensageException("Las fechas tienen que estar dentro del rango de fechas de la "+gestion.Nombre);
                    }

                    if(!(Entidad.FechaFin >= gestion.FechaInicio && Entidad.FechaFin <= gestion.FechaFin))
                    {
                        throw new MensageException("Las fechas tienen que estar dentro del rango de fechas de la " + gestion.Nombre);
                    }


                  /*  var listaperiodo = (from x in esquema.Periodo
                                        where x.Estado == (int)EstadoPeriodo.Abierto
                                         && x.IdGestion == Entidad.IdGestion
                                        select x).ToList();

                   if (listaperiodo.Count() > 1)
                    {
                        throw new MensageException("Ya existe 2 periodos abiertos, no es posible registrar otra periodo");
                    }*/

                    Periodo periodoexiste = (from x in esquema.Periodo
                                             where x.IdGestion == Entidad.IdGestion
                                             && (x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                             || (Entidad.FechaInicio >= x.FechaInicio && Entidad.FechaInicio <= x.FechaFin)
                                             || (Entidad.FechaFin >= x.FechaInicio && Entidad.FechaFin <= x.FechaFin)
                                             || (x.FechaInicio >= Entidad.FechaInicio && x.FechaInicio <= Entidad.FechaFin)
                                             || (x.FechaFin >= Entidad.FechaInicio && x.FechaFin <= Entidad.FechaFin))
                                             select x).FirstOrDefault();

                    validarExistencia(periodoexiste, Entidad);


                    esquema.Periodo.Add(Entidad);
                    esquema.SaveChanges();

                    return Entidad;

                }
                catch (MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }


        public Periodo Editar(Periodo Entidad)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad);

                    var gestion = LGestion.Logica.LGestion.obtenerGestion(Entidad.IdGestion);

                    if (gestion == null)
                    {
                        throw new MensageException("No se pudo encontrar la gestion");
                    }

                    if (!(Entidad.FechaInicio >= gestion.FechaInicio && Entidad.FechaInicio <= gestion.FechaFin))
                    {
                        throw new MensageException("Las fechas tienen que estar dentro del rango de fechas de la " + gestion.Nombre);
                    }

                    if (!(Entidad.FechaFin >= gestion.FechaInicio && Entidad.FechaFin <= gestion.FechaFin))
                    {
                        throw new MensageException("Las fechas tienen que estar dentro del rango de fechas de la " + gestion.Nombre);
                    }


                    var periodo = (from x in esquema.Periodo
                                   where x.idPeriodo == Entidad.idPeriodo
                                   select x).FirstOrDefault();


                    if (periodo == null)
                    {
                        throw new MensageException("No se puedo obtener el periodo");

                    }

                    if (periodo.Estado == (int)EstadoGestion.Cerrado)
                    {
                        throw new MensageException("No se puede modificar un periodo cerrado");
                    }

                   /* var listaperiodo = (from x in esquema.Periodo
                                        where x.IdGestion != Entidad.IdGestion
                                         && x.Estado == (int)EstadoGestion.Abierto
                                         && x.IdGestion == Entidad.IdGestion
                                        select x).ToList();

                    if (listaperiodo.Count() > 1)
                    {
                        throw new MensageException("Ya existe 2 periodos abiertos, no es posible registrar otra periodo");
                    }*/

                    Periodo periodoexiste = (from x in esquema.Periodo
                                             where x.idPeriodo != Entidad.idPeriodo
                                             && x.IdGestion == Entidad.IdGestion
                                             && (x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                             || (Entidad.FechaInicio >= x.FechaInicio && Entidad.FechaInicio <= x.FechaFin)
                                             || (Entidad.FechaFin >= x.FechaInicio && Entidad.FechaFin <= x.FechaFin)
                                             || (x.FechaInicio >= Entidad.FechaInicio && x.FechaInicio <= Entidad.FechaFin)
                                             || (x.FechaFin >= Entidad.FechaInicio && x.FechaFin <= Entidad.FechaFin))
                                            
                                             select x).FirstOrDefault();

                    validarExistencia(periodoexiste, Entidad);



                    if (periodo.FechaInicio == Entidad.FechaInicio && periodo.FechaFin == Entidad.FechaFin)
                    {
                        periodo.Nombre = Entidad.Nombre;
                        periodo.IdUsuario = Entidad.IdUsuario;
                        esquema.SaveChanges();
                    }
                    else
                    {
                        /*if (periodo.Periodo.Count() > 0)
                        {
                            throw new MensageException("No se puede eliminar la gestion, tiene registrado un periodo");

                        }
                        else
                        {*/
                        periodo.Nombre = Entidad.Nombre;
                        periodo.FechaInicio = Entidad.FechaInicio;
                        periodo.FechaFin = Entidad.FechaFin;
                        periodo.IdUsuario = Entidad.IdUsuario;
                        esquema.SaveChanges();
                       // }
                    }


                   

                    return Entidad;

                }
                catch (MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        public void Eliminar(long idperiodo)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    var periodo = (from x in esquema.Periodo
                                   where x.idPeriodo == idperiodo
                                   select x).FirstOrDefault();

                    if (periodo == null)
                    {
                        throw new MensageException("No se puede obtener el periodo");
                    }

                    if (periodo.Estado == (int)EstadoPeriodo.Cerrado)
                    {
                        throw new MensageException("No se puede eliminar un periodo cerrado");
                    }

                   /* if (gestion.Periodo.Count() > 0)
                    {
                        throw new MensageException("No se puede eliminar la gestion, tiene registrado un periodo");

                    }
                    else
                    {*/
                        esquema.Periodo.Remove(periodo);
                        esquema.SaveChanges();
                   // }

                }
                catch (MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public List<ERPeriodo> ReportePeriodo(long idgestion)
        {
            List<ERPeriodo> periodos = new List<ERPeriodo>();

            try
            {
                using (var esquema = GetEsquema())
                {
                    var periodo = (from x in esquema.Periodo
                                   where x.IdGestion == idgestion
                                   select x).ToList();



                    foreach (var i in periodo)
                    {
                        ERPeriodo g = new ERPeriodo();
                        g.NombrePeriodo = i.Nombre;
                        g.FechaInicio = i.FechaInicio.ToString("dd/MM/yyyy");
                        g.FechaFin = i.FechaFin.ToString("dd/MM/yyyy");
                        if (i.Estado == (int)EstadoGestion.Abierto)
                        {
                            g.Estado = "Abierta";
                        }
                        else if (i.Estado == (int)EstadoGestion.Cerrado)
                        {
                            g.Estado = "Cerrado";
                        }
                        else
                        {
                            g.Estado = "";
                        }

                        periodos.Add(g);
                    }
                }


            }
            catch (Exception ex)
            {
                throw new MensageException("Error no se puedo obtener el reporte de periodos");
            }



            return periodos;
        }


        public List<ERDatosBasicoPeriodo> ReporteDatosBasicoPeriodo(string usuario, string empresa,string gestion)
        {
            try
            {


                List<ERDatosBasicoPeriodo> basicos = new List<ERDatosBasicoPeriodo>();
                ERDatosBasicoPeriodo eRDatosBasico = new ERDatosBasicoPeriodo();
                eRDatosBasico.Usuario = usuario;
                eRDatosBasico.NombreGestion = gestion;
                eRDatosBasico.NombreEmpresa = empresa;
                eRDatosBasico.FechaActual = DateTime.Now.ToString("dd/MM/yyyy");

                basicos.Add(eRDatosBasico);

                return basicos;
            }
            catch (Exception ex)
            {
                throw new MensageException("Ha ocurrido un error.");
            }
        }


        public void Validacion(Periodo Entidad)
        {

            if (string.IsNullOrEmpty(Entidad.Nombre))
            {
                throw new MensageException("El nombre es obligatorio.");
            }

            if (Entidad.FechaInicio > Entidad.FechaFin)
            {
                throw new MensageException("La fecha inicio debe ser menor a la fecha fin.");
            }

        }


        public void validarExistencia(Periodo Existe, Periodo Entidad)
        {
            if (Existe != null)
            {

                if (Existe.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper())
                {
                    throw new MensageException("Ya existe el nombre del periodo en esta gestion.");
                }

               if (Existe.FechaInicio == Entidad.FechaInicio)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

                if (Existe.FechaFin == Entidad.FechaInicio)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

                if (Existe.FechaInicio == Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

                if (Existe.FechaFin == Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

                if (Entidad.FechaInicio >= Existe.FechaInicio && Entidad.FechaInicio <= Existe.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

                if (Entidad.FechaFin >= Existe.FechaInicio && Entidad.FechaFin <= Existe.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }


                if (Existe.FechaInicio >= Entidad.FechaInicio && Existe.FechaInicio <= Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

                if (Existe.FechaFin >= Entidad.FechaInicio && Existe.FechaFin <= Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con en otro perido.");
                }

            }
        }


    }
}
