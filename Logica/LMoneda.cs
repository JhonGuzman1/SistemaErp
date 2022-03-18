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
    public class LMoneda:LLogica<Moneda>
    {


        public List<EmpresaMoneda> listarMonedaXEmpresa(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<EmpresaMoneda> emonedas = new List<EmpresaMoneda>(); 

                    var  moneda = (from x in esquema.EmpresaMoneda
                                  where x.IdEmpresa == idempresa
                                  orderby x.FechaRegistro descending
                                  select x).ToList();

                    
                    foreach(var i in moneda)
                    {
                        EmpresaMoneda e = new EmpresaMoneda();
                        e.Cambio = i.Cambio;
                        e.Activo = i.Activo;
                        e.Empresa = i.Empresa;
                        e.FechaRegistro = i.FechaRegistro;
                        e.IdEmpresa = i.IdEmpresa;
                        e.IdEmpresaMoneda = i.IdEmpresaMoneda;
                        e.IdMonedaAlternativa = i.IdMonedaAlternativa;
                        e.IdMonedaPrincipal = i.IdMonedaPrincipal;
                        e.IdUsuario = i.IdUsuario;
                        e.Moneda = i.Moneda;
                        e.Moneda1 = i.Moneda1;
                        e.Usuario = i.Usuario;
                        emonedas.Add(e);
                    }

                    return emonedas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de monedad");
                }



            }
        }
        public List<Moneda> listarMoneda()
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var moneda = (from x in esquema.Moneda
                                   select x).ToList();

                    List<Moneda> monedas = new List<Moneda>();

                    monedas = moneda;

                    return monedas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de monedad");
                }



            }
        }

        public Moneda obtenerMoneda(long idmoneda)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var moneda = (from x in esquema.Moneda
                                  where x.idMoneda == idmoneda
                                  select x).FirstOrDefault();

               
                    return moneda;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de monedad");
                }



            }
        }


        public List<Moneda> listarMonedaAlterntaiva(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var principal = (from x in esquema.EmpresaMoneda
                                     where x.IdEmpresa == idempresa
                                     select x).FirstOrDefault();

                    var moneda = (from x in esquema.Moneda
                                  where x.idMoneda != principal.IdMonedaPrincipal
                                  select x).ToList();

                    List<Moneda> monedas = new List<Moneda>();

                    monedas = moneda;

                    return monedas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de monedad");
                }



            }
        }

        public List<Moneda> listarMonedaPrincipal(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var principal = (from x in esquema.EmpresaMoneda
                                     where x.IdEmpresa == idempresa
                                     select x).FirstOrDefault();

                    var moneda = (from x in esquema.Moneda
                                  where x.idMoneda == principal.IdMonedaPrincipal
                                  select x).ToList();

                    List<Moneda> monedas = new List<Moneda>();

                    monedas = moneda;

                    return monedas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de monedad");
                }



            }
        }

        public EmpresaMoneda obtenerMonedadXempresa(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var moneda = (from x in esquema.EmpresaMoneda
                                  where x.IdEmpresa == idempresa
                                  && x.Activo == (int)EstadoMonedaEmpresa.Activo
                                  select x).FirstOrDefault();

                   

                    return moneda;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la moneda");
                }



            }
        }

        public EmpresaMoneda obtenerPrimerMonedadXempresa(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var moneda = (from x in esquema.EmpresaMoneda
                                  where x.IdEmpresa == idempresa
                                  select x).FirstOrDefault();



                    return moneda;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la moneda");
                }



            }
        }

        public EmpresaMoneda Registro(EmpresaMoneda Entidad)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad);


                    var monedaActiva = (from x in esquema.EmpresaMoneda
                                        where x.IdEmpresa == Entidad.IdEmpresa
                                        && x.Activo == (int)EstadoMonedaEmpresa.Activo
                                        select x).FirstOrDefault();


                    if (monedaActiva.Cambio == null)
                    {
                        monedaActiva.Activo = (int)EstadoMonedaEmpresa.NoActivo;
                        esquema.SaveChanges();
                        esquema.EmpresaMoneda.Add(Entidad);
                        esquema.SaveChanges();
                    }
                    else
                    {

                        if(monedaActiva.Cambio != Entidad.Cambio)
                        {

                            var comprobante = (from x in esquema.Comprobante
                                               where x.IdEmpresa == Entidad.IdEmpresa
                                               select x).FirstOrDefault();

                            if(comprobante != null)
                            {
                                if(Entidad.IdMonedaAlternativa == monedaActiva.IdMonedaAlternativa)
                                {
                                    monedaActiva.Activo = (int)EstadoMonedaEmpresa.NoActivo;
                                    esquema.SaveChanges();
                                    esquema.EmpresaMoneda.Add(Entidad);
                                    esquema.SaveChanges();
                                }
                                else
                                {
                                    throw new MensageException("No se puede cambiar la moneda alternativa ya exite un registro de comprbante");
                                }
                             
                            }
                            else
                            {

                                monedaActiva.Activo = (int)EstadoMonedaEmpresa.NoActivo;
                                esquema.SaveChanges();
                                esquema.EmpresaMoneda.Add(Entidad);
                                esquema.SaveChanges();
                            }

                        
                        }
                        else
                        {
                            throw new MensageException("Ya existe una moneda activa con el mismo cambio");
                        }


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


        public void Validacion(EmpresaMoneda Entidad)
        {

            if (Entidad.Cambio == null || Entidad.Cambio <= 0)
            {
                throw new MensageException("El cambio debe ser mayor a 0");
            }

            if(Entidad.IdMonedaAlternativa == -1)
            {
                throw new MensageException("Seleccione una moneda alternativa");
            }

        }


        public List<EmpresaMoneda> listarMonedaActivaXEmpresa(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<EmpresaMoneda> emonedas = new List<EmpresaMoneda>();

                    var moneda = (from x in esquema.EmpresaMoneda
                                  where x.IdEmpresa == idempresa
                                  && x.Activo == (int)EstadoMonedaEmpresa.Activo
                                  orderby x.FechaRegistro descending
                                  select x).ToList();


                    foreach (var i in moneda)
                    {
                        EmpresaMoneda e = new EmpresaMoneda();
                        e.Cambio = i.Cambio;
                        e.Activo = i.Activo;
                        e.Empresa = i.Empresa;
                        e.FechaRegistro = i.FechaRegistro;
                        e.IdEmpresa = i.IdEmpresa;
                        e.IdEmpresaMoneda = i.IdEmpresaMoneda;
                        e.IdMonedaAlternativa = i.IdMonedaAlternativa;
                        e.IdMonedaPrincipal = i.IdMonedaPrincipal;
                        e.IdUsuario = i.IdUsuario;
                        e.Moneda = i.Moneda;
                        e.Moneda1 = i.Moneda1;
                        e.Usuario = i.Usuario;
                        emonedas.Add(e);
                    }

                    return emonedas;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de monedad");
                }



            }
        }



    }
}
