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
    public class LGestion : LLogica<Gestion>
    {

        public List<Gestion> listarGestion(long idempresa,long idusuario)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var gestion = (from x in esquema.Gestion
                                   where x.IdEmpresa == idempresa
                                  // && x.IdUsuario == idusuario
                                   orderby x.IdGestion descending
                                   select x).ToList();

                    List<Gestion> gestiones = new List<Gestion>();

                    gestiones = gestion;

                    return gestiones;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de gestiones");
                }

            }
        }


        public Gestion Registro(Gestion Entidad)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad);


                    var listagestion = (from x in esquema.Gestion
                                        where x.Estado == (int)EstadoGestion.Abierto
                                         && x.IdEmpresa == Entidad.IdEmpresa
                                        select x).ToList();

                    if(listagestion.Count() > 1)
                    {
                        throw new MensageException("Ya existe 2 gestiones abiertas, no es posible registrar otra gestion");
                    }

                    Gestion gestionexiste = (from x in esquema.Gestion
                                             where x.IdEmpresa == Entidad.IdEmpresa 
                                             && ( x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                             || (Entidad.FechaInicio >= x.FechaInicio && Entidad.FechaInicio <= x.FechaFin )
                                             || (Entidad.FechaFin >= x.FechaInicio && Entidad.FechaFin <= x.FechaFin)
                                             || (x.FechaInicio >= Entidad.FechaInicio && x.FechaInicio <= Entidad.FechaFin)
                                             || (x.FechaFin >= Entidad.FechaInicio && x.FechaFin <= Entidad.FechaFin))
                                             select x).FirstOrDefault();

                    validarExistencia(gestionexiste, Entidad);

                   
                    esquema.Gestion.Add(Entidad);
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


        public void ValidarGestionesAbiertas(long idempresa)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                   


                    var listagestion = (from x in esquema.Gestion
                                        where x.Estado == (int)EstadoGestion.Abierto
                                         && x.IdEmpresa == idempresa
                                        select x).ToList();

                    if (listagestion.Count() > 1)
                    {
                        throw new MensageException("Ya existe 2 gestiones abiertas, no es posible registrar otra gestion");
                    }

                   

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


        public Gestion Editar(Gestion Entidad)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad);


                    var gestion = (from x in esquema.Gestion
                                   where x.IdGestion == Entidad.IdGestion
                                   select x).FirstOrDefault();


                    if (gestion == null)
                    {
                        throw new MensageException("No se puedo obtener la gestion");

                    }

                    if (gestion.Estado == (int)EstadoGestion.Cerrado)
                    {
                        throw new MensageException("No se puede modificar una gestion cerrada");
                    }

                    var listagestion = (from x in esquema.Gestion
                                        where x.IdGestion != Entidad.IdGestion
                                         && x.Estado == (int)EstadoGestion.Abierto
                                         && x.IdEmpresa == Entidad.IdEmpresa
                                        select x).ToList();

                    if (listagestion.Count() > 1)
                    {
                        throw new MensageException("Ya existe 2 gestiones abiertas, no es posible registrar otra gestion");
                    }

                    Gestion gestionexiste = (from x in esquema.Gestion
                                             where x.IdGestion != Entidad.IdGestion
                                             && x.IdEmpresa == Entidad.IdEmpresa
                                             && (x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                             || (Entidad.FechaInicio >= x.FechaInicio && Entidad.FechaInicio <= x.FechaFin)
                                             || (Entidad.FechaFin >= x.FechaInicio && Entidad.FechaFin <= x.FechaFin)
                                             || (x.FechaInicio >= Entidad.FechaInicio && x.FechaInicio <= Entidad.FechaFin)
                                             || (x.FechaFin >= Entidad.FechaInicio && x.FechaFin <= Entidad.FechaFin))
                                             select x).FirstOrDefault();

                    validarExistencia(gestionexiste, Entidad);

                  

                    if(gestion.FechaInicio == Entidad.FechaInicio && gestion.FechaFin == Entidad.FechaFin)
                    {
                        gestion.Nombre = Entidad.Nombre;
                        gestion.IdUsuario = Entidad.IdUsuario;
                        esquema.SaveChanges();
                    }
                    else
                    {
                        if (gestion.Periodo.Count() > 0)
                        {
                            throw new MensageException("No se puede eliminar la gestion, tiene registrado un periodo");

                        }
                        else
                        {
                            gestion.Nombre = Entidad.Nombre;
                            gestion.FechaInicio = Entidad.FechaInicio;
                            gestion.FechaFin = Entidad.FechaFin;
                            gestion.IdUsuario = Entidad.IdUsuario;
                            esquema.SaveChanges();
                        }
                    }
                  

                   // esquema.Gestion.Add(Entidad);
                  //  esquema.SaveChanges();

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


        public void Eliminar(long idgestion)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    var gestion = (from x in esquema.Gestion
                                   where x.IdGestion == idgestion
                                   select x).FirstOrDefault();

                    if (gestion == null)
                    {
                        throw new MensageException("No se puede obtener la gestion");
                    }

                    if(gestion.Estado == (int)EstadoGestion.Cerrado)
                    {
                        throw new MensageException("No se puede eliminar una gestion cerrada");
                    }

                    if (gestion.Periodo.Count() > 0)
                    {
                        throw new MensageException("No se puede eliminar la gestion, tiene registrado un periodo");

                    }
                    else
                    {
                        esquema.Gestion.Remove(gestion);
                        esquema.SaveChanges();
                    }

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


        public Gestion obtenerGestion(long idgestion)
        {

            using (var esquema = GetEsquema())
            {

                try
                {


                    var gestion = (from x in esquema.Gestion
                                   where x.IdGestion == idgestion
                                   select x).FirstOrDefault();


                    if (gestion == null)
                    {
                        throw new MensageException("No se puedo obtener la gestion");
                    }

                    return gestion;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la gestion");
                }


            }
        }


        public List<ERGestion> ReporteGestion(long idempresa)
        {
            List<ERGestion> gestiones = new List<ERGestion>();

            try
            {
                using (var esquema = GetEsquema())
                {
                    var gestion = (from x in esquema.Gestion
                                        where x.IdEmpresa == idempresa
                                   select x).ToList();



                    foreach (var i in gestion)
                    {
                        ERGestion g = new ERGestion();
                        g.NombreGestion = i.Nombre;
                        g.FechaInicio = i.FechaInicio.ToString("dd/MM/yyyy");
                        g.FechaFin = i.FechaFin.ToString("dd/MM/yyyy");
                        if(i.Estado == (int)EstadoGestion.Abierto)
                        {
                            g.Estado = "Abierta";
                        }
                        else if(i.Estado == (int)EstadoGestion.Cerrado)
                        {
                            g.Estado = "Cerrado";
                        }
                        else
                        {
                            g.Estado = "";
                        }

                        gestiones.Add(g);
                    }
                }


            }
            catch (Exception ex)
            {
                throw new MensageException("Error no se puedo obtener el reporte de gestiones");
            }



            return gestiones;
        }

        public List<ERDatosBasicoGestion> ReporteDatosBasicoGestion(string usuario,string empresa)
        {
            try
            {


                List<ERDatosBasicoGestion> basicos = new List<ERDatosBasicoGestion>();
                ERDatosBasicoGestion eRDatosBasico = new ERDatosBasicoGestion();
                eRDatosBasico.Usuario = usuario;
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


        public void Validacion(Gestion Entidad)
        {

            if (string.IsNullOrEmpty(Entidad.Nombre))
            {
                throw new MensageException("El nombre es obligatorio.");
            }

            if(Entidad.FechaInicio > Entidad.FechaFin)
            {
                throw new MensageException("La fecha inicio debe ser menor a la fecha fin.");
            }
 
        }


        public void validarExistencia(Gestion Existe, Gestion Entidad)
        {
            if (Existe != null)
            {

                if (Existe.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper())
                {
                    throw new MensageException("Ya existe el nombre de la gestion.");
                }

                if(Existe.FechaInicio == Entidad.FechaInicio)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if (Existe.FechaFin == Entidad.FechaInicio)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if(Existe.FechaInicio == Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if (Existe.FechaFin == Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if (Entidad.FechaInicio >= Existe.FechaInicio && Entidad.FechaInicio <= Existe.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if (Entidad.FechaFin >= Existe.FechaInicio && Entidad.FechaFin <= Existe.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if (Existe.FechaInicio >= Entidad.FechaInicio && Existe.FechaInicio <= Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

                if (Existe.FechaFin >= Entidad.FechaInicio && Existe.FechaFin <= Entidad.FechaFin)
                {
                    throw new MensageException("Existe solapamiento con otra gestion.");
                }

            }
        }


    }
}
