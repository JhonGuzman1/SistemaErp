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
    public class LComprobante: LLogica<Comprobante>
    {

        public List<Comprobante> listarComprobante(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    List<Comprobante> comprobante = new List<Comprobante>();

                 
                    var comprobantes = (from x in esquema.Comprobante
                                   where x.IdEmpresa == idempresa
                                   select x).ToList();

                    
                    foreach(var i in comprobantes)
                    {
                        Comprobante c = new Comprobante();
                        c.DetalleComprobante = i.DetalleComprobante;
                        c.Empresa = i.Empresa;
                        c.Estado = i.Estado;
                        c.Fecha = i.Fecha;
                        c.Glosa = i.Glosa;
                        c.IdComprobante = i.IdComprobante;
                        c.IdEmpresa = i.IdEmpresa;
                        c.IdMoneda = i.IdMoneda;
                        c.IdUsuario = i.IdUsuario;
                        c.Moneda = i.Moneda;
                        c.Serie = i.Serie;
                        c.TipoCambio = i.TipoCambio;
                        c.TipoComprobante = i.TipoComprobante;
                        c.Usuario = i.Usuario;

                        comprobante.Add(c);
                    }


                    return comprobante;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de comprobantes");
                }

            }
        }

        public Comprobante ObtenerComprobante(long idempresa,long idcomprobante)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    Comprobante c = new Comprobante();


                    var comprobantes = (from x in esquema.Comprobante
                                        where x.IdEmpresa == idempresa
                                        && x.IdComprobante == idcomprobante
                                        select x).FirstOrDefault();


                     if(comprobantes != null)
                     {
                        c.DetalleComprobante = comprobantes.DetalleComprobante;
                        c.Empresa = comprobantes.Empresa;
                        c.Estado = comprobantes.Estado;
                        c.Fecha = comprobantes.Fecha;
                        c.Glosa = comprobantes.Glosa;
                        c.IdComprobante = comprobantes.IdComprobante;
                        c.IdEmpresa = comprobantes.IdEmpresa;
                        c.IdMoneda = comprobantes.IdMoneda;
                        c.IdUsuario = comprobantes.IdUsuario;
                        c.Moneda = comprobantes.Moneda;
                        c.Serie = comprobantes.Serie;
                        c.TipoCambio = comprobantes.TipoCambio;
                        c.TipoComprobante = comprobantes.TipoComprobante;
                        c.Usuario = comprobantes.Usuario;
                     }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener el comprobantes");
                    }
                       

                     

                    return c;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener el comprobantes");
                }

            }
        }

        public List<EDetalleComprobante> listarDetalleComprobanteXComprobante(long idcomprobante,long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    List<EDetalleComprobante> detalleComprobantes = new List<EDetalleComprobante>();


                    var dcomprobantes = (from x in esquema.DetalleComprobante
                                        where x.IdComprobante == idcomprobante
                                        select x).ToList();

                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

                    foreach (var i in dcomprobantes)
                    {

                        EDetalleComprobante e = new EDetalleComprobante();

                        e.IdCuenta = i.IdCuenta;
                        e.Cuenta = i.Cuenta.Codigo + " " + i.Cuenta.Nombre;
                        e.Glosa = i.Glosa;

                        if(i.Comprobante.IdMoneda == moneda.IdMonedaPrincipal)
                        {
                            e.Debe = i.MontoDebe;
                            e.Haber = i.MontoHaber;
                        }
                        else
                        {
                            e.Debe = i.MontoDebeAlt;
                            e.Haber = i.MontoHaberAlt;
                        }

                        detalleComprobantes.Add(e);

                    }

                    return detalleComprobantes;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
                }

            }
        }


        public List<ELibroDiario> reporteLibroDiario(long idgestion, long idperiodo, long idempresa, long idmoneda)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<ELibroDiario> libroDiarios = new List<ELibroDiario>();

                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

                    if (idperiodo == 0)
                    {
                        var gestion = (from x in esquema.Gestion
                                       where x.IdGestion == idgestion
                                       select x).FirstOrDefault();

                        var comprobante = (from x in esquema.Comprobante
                                           where x.IdEmpresa == idempresa
                                           && x.Fecha >= gestion.FechaInicio && x.Fecha <= gestion.FechaFin
                                           && x.Estado != (int)EstadoComprobante.Anualdo
                                           orderby x.Fecha ascending
                                           select x).ToList();



                        foreach (var i in comprobante)
                        {
                            ELibroDiario e = new ELibroDiario();
                            e.Fecha = i.Fecha.ToString("dd/MM/yyyy");
                            e.Cuenta = i.Glosa;
                            libroDiarios.Add(e);

                            var detallecomprobante = (from x in esquema.DetalleComprobante
                                                      where x.IdComprobante == i.IdComprobante
                                                      orderby x.MontoDebe descending
                                                      select x).ToList();

                            foreach (var j in detallecomprobante)
                            {

                                ELibroDiario d = new ELibroDiario();
                                if (j.MontoDebe > 0)
                                {
                                    d.CodigoCuenta = j.Cuenta.Codigo;
                                    d.Cuenta = j.Cuenta.Nombre;
                                }
                                else
                                {
                                    d.CodigoCuenta = "  " + j.Cuenta.Codigo;
                                    d.Cuenta = "  " + j.Cuenta.Nombre;
                                }

                                if (moneda.IdMonedaPrincipal == idmoneda)
                                {
                                    d.Debe = j.MontoDebe;
                                    d.Haber = j.MontoHaber;
                                }
                                else
                                {
                                    d.Debe = j.MontoDebeAlt;
                                    d.Haber = j.MontoHaberAlt;
                                }

                                libroDiarios.Add(d);

                            }

                        }

                    }
                    else
                    {

                        var periodo = (from x in esquema.Periodo
                                       where x.idPeriodo == idperiodo
                                       select x).FirstOrDefault();

                        var comprobante = (from x in esquema.Comprobante
                                            where x.IdEmpresa == idempresa
                                            && x.Fecha >= periodo.FechaInicio && x.Fecha <= periodo.FechaFin
                                            orderby x.Fecha ascending
                                            select x).ToList();

                     

                        foreach (var i in comprobante)
                        {
                            ELibroDiario e = new ELibroDiario();
                            e.Fecha = i.Fecha.ToString("dd/MM/yyyy");
                            e.Cuenta = i.Glosa;
                            libroDiarios.Add(e);

                            var detallecomprobante = (from x in esquema.DetalleComprobante
                                                      where x.IdComprobante == i.IdComprobante
                                                      orderby x.MontoDebe descending
                                                      select x).ToList();

                            foreach (var j in detallecomprobante )
                            {

                                ELibroDiario d = new ELibroDiario();
                                if(j.MontoDebe > 0)
                                {
                                    d.CodigoCuenta = j.Cuenta.Codigo;
                                    d.Cuenta = j.Cuenta.Nombre;
                                }
                                else
                                {
                                    d.CodigoCuenta = "  "+j.Cuenta.Codigo;
                                    d.Cuenta = "  "+j.Cuenta.Nombre;
                                }
                               
                                if(moneda.IdMonedaPrincipal == idmoneda)
                                {
                                    d.Debe = j.MontoDebe;
                                    d.Haber = j.MontoHaber;
                                }
                                else
                                {
                                    d.Debe = j.MontoDebeAlt;
                                    d.Haber = j.MontoHaberAlt;
                                }

                                libroDiarios.Add(d);

                            }

                        }


                    }

                    return libroDiarios;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
                }

            }
        }

        public List<EDetalleTotal> TotalDetalle(List<EDetalleComprobante> detalleComprobantes)
        {


            try
            {

                List<EDetalleTotal> eDetalleTotals = new List<EDetalleTotal>();
                EDetalleTotal detalleTotal = new EDetalleTotal();
                detalleTotal.TotalDebe = 0;
                detalleTotal.TotalHaber = 0;


                foreach(var i in detalleComprobantes)
                {
                    detalleTotal.TotalDebe = detalleTotal.TotalDebe + i.Debe;
                    detalleTotal.TotalHaber = detalleTotal.TotalHaber + i.Haber;
                }

                eDetalleTotals.Add(detalleTotal);

                return eDetalleTotals;

                
            }catch(Exception ex)
            {
                throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
            }

        }

        public List<EDetalleTotal> TotalLibroDiario(List<ELibroDiario> detalleComprobantes)
        {


            try
            {

                List<EDetalleTotal> eDetalleTotals = new List<EDetalleTotal>();
                EDetalleTotal detalleTotal = new EDetalleTotal();
                detalleTotal.TotalDebe = 0;
                detalleTotal.TotalHaber = 0;


                foreach (var i in detalleComprobantes)
                {
                    detalleTotal.TotalDebe = detalleTotal.TotalDebe + i.Debe;
                    detalleTotal.TotalHaber = detalleTotal.TotalHaber + i.Haber;
                }

                eDetalleTotals.Add(detalleTotal);

                return eDetalleTotals;


            }
            catch (Exception ex)
            {
                throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
            }

        }

        public List<EComprobante> ObtenerComprobanteReporte(long idempresa, long idcomprobante)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<EComprobante> eComprobantes = new List<EComprobante>();
                    EComprobante c = new EComprobante();


                    var comprobantes = (from x in esquema.Comprobante
                                        where x.IdEmpresa == idempresa
                                        && x.IdComprobante == idcomprobante
                                        select x).FirstOrDefault();


                    if (comprobantes != null)
                    {

                        c.Serie = comprobantes.Serie;
                        c.Glosa = comprobantes.Glosa;
                        c.TipoCambio = comprobantes.TipoCambio;
                        c.Moneda = comprobantes.Moneda.Abreviatura;
                        c.Fecha = comprobantes.Fecha.ToString("dd/MM/yyyy");

                        switch (comprobantes.Estado)
                        {
                            case (int)EstadoComprobante.Abierto:
                                c.Estado = "Abierto";
                                break;
                            case (int)EstadoComprobante.Cerrado:
                                c.Estado = "Cerrado";
                                break;
                            case (int)EstadoComprobante.Anualdo:
                                c.Estado = "Anulado";
                                break;
                        }

                        switch (comprobantes.TipoComprobante)
                        {
                            case (int)TipoComprobante.Apertura:
                                c.TipoComprobante = "Apertura";
                                break;
                            case (int)TipoComprobante.Egreso:
                                c.TipoComprobante = "Egreso";
                                break;
                            case (int)TipoComprobante.Ingreso:
                                c.TipoComprobante = "Ingreso";
                                break;
                            case (int)TipoComprobante.Traspaso:
                                c.TipoComprobante = "Traspaso";
                                break;
                            case (int)TipoComprobante.Ajuste:
                                c.TipoComprobante = "Ajuste";
                                break;


                        }


                    }
                    else
                    {
                        throw new MensageException("Error no se puedo obtener el comprobantes");
                    }



                    eComprobantes.Add(c);
                    return eComprobantes;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener el comprobantes");
                }

            }
        }

        public long obtenerSerie(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {

                    long Serie = 1;

                 


                    var comprobantes = (from x in esquema.Comprobante
                                        where x.IdEmpresa == idempresa
                                        orderby x.IdComprobante descending
                                        select x).FirstOrDefault();

                    if(comprobantes != null)
                    {
                        Serie = Serie + comprobantes.Serie;
                    }


                    return Serie;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de gestiones");
                }

            }
        }


        public Comprobante Registro(Comprobante Entidad, List<EDetalleComprobante> eDetalleComprobante,EDetalleTotal DetalleTotal)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad);
                    ValidacionDetalleTotal(DetalleTotal);


                    var gestion = (from x in esquema.Gestion
                                   where Entidad.Fecha >= x.FechaInicio
                                   && Entidad.Fecha <= x.FechaFin
                                   && x.IdEmpresa == Entidad.IdEmpresa
                                   select x).FirstOrDefault();

                    if(gestion != null)
                    {

                        var periodo = (from x in esquema.Periodo
                                       where Entidad.Fecha >= x.FechaInicio
                                       && Entidad.Fecha <= x.FechaFin
                                       && x.IdGestion == gestion.IdGestion
                                       select x).FirstOrDefault();
                        if(periodo != null)
                        {

                            var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(Entidad.IdEmpresa);
                            

                            if (Entidad.TipoComprobante == (int)TipoComprobante.Apertura)
                            {

                                var comprobante = (from x in esquema.Comprobante
                                                   where x.TipoComprobante == (int)TipoComprobante.Apertura
                                                   && x.Estado != (int)EstadoComprobante.Anualdo
                                                   && x.IdEmpresa == Entidad.IdEmpresa
                                                   && x.Fecha >= gestion.FechaInicio
                                                   && x.Fecha <= gestion.FechaFin
                                                   select x).FirstOrDefault();
                                if(comprobante != null)
                                {
                                    /* var gestioncomprobante = (from x in esquema.Gestion
                                                    where Entidad.Fecha >= x.FechaInicio
                                                    && Entidad.Fecha <= x.FechaFin
                                                    && x.IdEmpresa == Entidad.IdEmpresa
                                                    select x).FirstOrDefault();
                                   
                                    if (gestioncomprobante != null)
                                    {*/
                                        throw new MensageException("Ya existe un comprobante de apertura en la gestion");
                                   /* }
                                    else
                                    {
                                        esquema.Comprobante.Add(Entidad);
                                        esquema.SaveChanges();
                                        int contador = 0;

                                        

                                        foreach(var i in eDetalleComprobante)
                                        {
                                            DetalleComprobante d = new DetalleComprobante();

                                            contador = contador + 1;
                                            d.Numero = contador;
                                            d.Glosa = i.Glosa;
                                            d.IdCuenta = i.IdCuenta;
                                            d.IdComprobante = Entidad.IdComprobante;
                                            d.IdUsuario = Entidad.IdUsuario;
                                            
                                            if(moneda.IdMonedaPrincipal == Entidad.IdMoneda)
                                            {
                                                d.MontoDebe = i.Debe;
                                                d.MontoHaber = i.Haber;
                                                d.MontoDebeAlt = Math.Round((i.Debe / Entidad.TipoCambio),2);
                                                d.MontoHaberAlt = Math.Round((i.Haber / Entidad.TipoCambio),2);
                                                
                                            }
                                            else
                                            {
                                                d.MontoDebe = Math.Round((i.Debe * Entidad.TipoCambio),2);
                                                d.MontoHaber = Math.Round((i.Haber * Entidad.TipoCambio),2);
                                                d.MontoDebeAlt = i.Debe;
                                                d.MontoHaberAlt = i.Haber;
                                            }

                                            esquema.DetalleComprobante.Add(d);
                                            esquema.SaveChanges();


                                        }


                                    }*/
                                }
                                else
                                {
                                    esquema.Comprobante.Add(Entidad);
                                    esquema.SaveChanges();
                                    int contador = 0;



                                    foreach (var i in eDetalleComprobante)
                                    {
                                        DetalleComprobante d = new DetalleComprobante();

                                        contador = contador + 1;
                                        d.Numero = contador;
                                        d.Glosa = i.Glosa;
                                        d.IdCuenta = i.IdCuenta;
                                        d.IdComprobante = Entidad.IdComprobante;
                                        d.IdUsuario = Entidad.IdUsuario;

                                        if (moneda.IdMonedaPrincipal == Entidad.IdMoneda)
                                        {
                                            d.MontoDebe = i.Debe;
                                            d.MontoHaber = i.Haber;
                                            d.MontoDebeAlt = Math.Round((i.Debe / Entidad.TipoCambio), 2);
                                            d.MontoHaberAlt = Math.Round((i.Haber / Entidad.TipoCambio), 2);

                                        }
                                        else
                                        {
                                            d.MontoDebe = Math.Round((i.Debe * Entidad.TipoCambio), 2);
                                            d.MontoHaber = Math.Round((i.Haber * Entidad.TipoCambio), 2);
                                            d.MontoDebeAlt = i.Debe;
                                            d.MontoHaberAlt = i.Haber;
                                        }

                                        esquema.DetalleComprobante.Add(d);
                                        esquema.SaveChanges();


                                    }
                                }

                            }
                            else
                            {

                                esquema.Comprobante.Add(Entidad);
                                esquema.SaveChanges();
                                int contador = 0;



                                foreach (var i in eDetalleComprobante)
                                {
                                    DetalleComprobante d = new DetalleComprobante();

                                    contador = contador + 1;
                                    d.Numero = contador;
                                    d.Glosa = i.Glosa;
                                    d.IdCuenta = i.IdCuenta;
                                    d.IdComprobante = Entidad.IdComprobante;
                                    d.IdUsuario = Entidad.IdUsuario;

                                    if (moneda.IdMonedaPrincipal == Entidad.IdMoneda)
                                    {
                                        d.MontoDebe = i.Debe;
                                        d.MontoHaber = i.Haber;
                                     
                                        d.MontoDebeAlt = Math.Round((i.Debe / Entidad.TipoCambio), 2);
                                        d.MontoHaberAlt = Math.Round((i.Haber / Entidad.TipoCambio), 2);
                                    }
                                    else
                                    {
                                        d.MontoDebe = Math.Round((i.Debe * Entidad.TipoCambio), 2);
                                        d.MontoHaber = Math.Round((i.Haber * Entidad.TipoCambio), 2);
                                        d.MontoDebeAlt = i.Debe;
                                        d.MontoHaberAlt = i.Haber;
                                    }

                                    esquema.DetalleComprobante.Add(d);
                                    esquema.SaveChanges();


                                }

                            }


                        }
                        else
                        {
                            throw new MensageException("No existe un periodo para la fecha ingresada.");
                        }

                    }
                    else
                    {
                        throw new MensageException("No existe una gestion para la fecha ingresada.");
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

        public Comprobante Modificar(Comprobante Entidad, List<EDetalleComprobante> eDetalleComprobante, EDetalleTotal DetalleTotal)
        {
            using (var esquema = GetEsquema())
            {
                try
                {

                    var nota = LNotaCompra.Logica.LNotaCompra.ObtenerNotaGeneralXComprobante(Entidad.IdEmpresa, Entidad.IdComprobante);

                    if (nota != null)
                    {
                        throw new MensageException("No se puedo editar el comprobante por que esta relacionado con una nota");
                    }

                    Validacion(Entidad);
                    ValidacionDetalleTotal(DetalleTotal);


                    var gestion = (from x in esquema.Gestion
                                   where Entidad.Fecha >= x.FechaInicio
                                   && Entidad.Fecha <= x.FechaFin
                                   && x.IdEmpresa == Entidad.IdEmpresa
                                   select x).FirstOrDefault();

                    if (gestion != null)
                    {

                        var periodo = (from x in esquema.Periodo
                                       where Entidad.Fecha >= x.FechaInicio
                                       && Entidad.Fecha <= x.FechaFin
                                       && x.IdGestion == gestion.IdGestion
                                       select x).FirstOrDefault();
                        if (periodo != null)
                        {
                            var modcomprobante = (from x in esquema.Comprobante
                                                  where x.IdComprobante == Entidad.IdComprobante
                                                  select x).FirstOrDefault();
                            if(modcomprobante == null)
                            {
                                throw new MensageException("No se pudo encontrar el comprobante");
                            }

                            var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(Entidad.IdEmpresa);


                            if (Entidad.TipoComprobante == (int)TipoComprobante.Apertura)
                            {

                                var comprobante = (from x in esquema.Comprobante
                                                   where x.TipoComprobante == (int)TipoComprobante.Apertura
                                                   && x.Estado != (int)EstadoComprobante.Anualdo
                                                   && x.IdEmpresa == Entidad.IdEmpresa
                                                   select x).FirstOrDefault();
                                if (comprobante != null)
                                {
                                    var gestioncomprobante = (from x in esquema.Gestion
                                                              where comprobante.Fecha >= x.FechaInicio
                                                              && comprobante.Fecha <= x.FechaFin
                                                              && x.IdEmpresa == Entidad.IdEmpresa
                                                              select x).FirstOrDefault();

                                    if (gestioncomprobante != null)
                                    {
                                        if(modcomprobante.IdComprobante != comprobante.IdComprobante)
                                        {
                                            throw new MensageException("Ya existe un comprobante de apertura en la gestion");
                                        }
                                        else
                                        {

                                            modcomprobante.IdMoneda = Entidad.IdMoneda;
                                            modcomprobante.Glosa = Entidad.Glosa;
                                            modcomprobante.Fecha = Entidad.Fecha;
                                            modcomprobante.TipoCambio = Entidad.TipoCambio;
                                            modcomprobante.TipoComprobante = Entidad.TipoComprobante;
                                            esquema.SaveChanges();
                                            int contador = 0;



                                            foreach (var i in eDetalleComprobante)
                                            {
                                                DetalleComprobante d = new DetalleComprobante();

                                                contador = contador + 1;
                                                d.Numero = contador;
                                                d.Glosa = i.Glosa;
                                                d.IdCuenta = i.IdCuenta;
                                                d.IdComprobante = Entidad.IdComprobante;
                                                d.IdUsuario = Entidad.IdUsuario;

                                                if (moneda.IdMonedaPrincipal == Entidad.IdMoneda)
                                                {
                                                    d.MontoDebe = i.Debe;
                                                    d.MontoHaber = i.Haber;
                                                    d.MontoDebeAlt = Math.Round((i.Debe / Entidad.TipoCambio), 2);
                                                    d.MontoHaberAlt = Math.Round((i.Haber / Entidad.TipoCambio), 2);

                                                }
                                                else
                                                {
                                                    d.MontoDebe = Math.Round((i.Debe * Entidad.TipoCambio), 2);
                                                    d.MontoHaber = Math.Round((i.Haber * Entidad.TipoCambio), 2);
                                                    d.MontoDebeAlt = i.Debe;
                                                    d.MontoHaberAlt = i.Haber;
                                                }

                                                var detallecomprobante = (from x in esquema.DetalleComprobante
                                                                          where x.IdComprobante == Entidad.IdComprobante
                                                                          && x.IdCuenta == i.IdCuenta
                                                                          select x).FirstOrDefault();

                                                if (detallecomprobante != null)
                                                {
                                                    detallecomprobante.Numero = d.Numero;
                                                    detallecomprobante.Glosa = d.Glosa;
                                                    detallecomprobante.IdCuenta = d.IdCuenta;
                                                    detallecomprobante.MontoDebe = d.MontoDebe;
                                                    detallecomprobante.MontoHaber = d.MontoHaber;
                                                    detallecomprobante.MontoDebeAlt = d.MontoDebeAlt;
                                                    detallecomprobante.MontoHaberAlt = d.MontoHaberAlt;
                                                    esquema.SaveChanges();
                                                }
                                                else
                                                {
                                                    esquema.DetalleComprobante.Add(d);
                                                    esquema.SaveChanges();
                                                }

                                            }

                                            if (eDetalleComprobante.Count > 0)
                                            {
                                                EliminarDetalleComprobante(eDetalleComprobante, esquema, Entidad.IdComprobante);
                                            }

                                        }
                                      
                                    }
                                    else
                                    {
                                        modcomprobante.IdMoneda = Entidad.IdMoneda;
                                        modcomprobante.Glosa = Entidad.Glosa;
                                        modcomprobante.Fecha = Entidad.Fecha;
                                        modcomprobante.TipoCambio = Entidad.TipoCambio;
                                        modcomprobante.TipoComprobante = Entidad.TipoComprobante;
                                        esquema.SaveChanges();
                                        int contador = 0;



                                        foreach (var i in eDetalleComprobante)
                                        {
                                            DetalleComprobante d = new DetalleComprobante();

                                            contador = contador + 1;
                                            d.Numero = contador;
                                            d.Glosa = i.Glosa;
                                            d.IdCuenta = i.IdCuenta;
                                            d.IdComprobante = Entidad.IdComprobante;
                                            d.IdUsuario = Entidad.IdUsuario;

                                            if (moneda.IdMonedaPrincipal == Entidad.IdMoneda)
                                            {
                                                d.MontoDebe = i.Debe;
                                                d.MontoHaber = i.Haber;
                                                d.MontoDebeAlt = Math.Round((i.Debe / Entidad.TipoCambio), 2);
                                                d.MontoHaberAlt = Math.Round((i.Haber / Entidad.TipoCambio), 2);

                                            }
                                            else
                                            {
                                                d.MontoDebe = Math.Round((i.Debe * Entidad.TipoCambio), 2);
                                                d.MontoHaber = Math.Round((i.Haber * Entidad.TipoCambio), 2);
                                                d.MontoDebeAlt = i.Debe;
                                                d.MontoHaberAlt = i.Haber;
                                            }

                                            var detallecomprobante = (from x in esquema.DetalleComprobante
                                                                      where x.IdComprobante == Entidad.IdComprobante
                                                                      && x.IdCuenta == i.IdCuenta
                                                                      select x).FirstOrDefault();

                                            if(detallecomprobante != null)
                                            {
                                                detallecomprobante.Numero = d.Numero;
                                                detallecomprobante.Glosa = d.Glosa;
                                                detallecomprobante.IdCuenta = d.IdCuenta;
                                                detallecomprobante.MontoDebe = d.MontoDebe;
                                                detallecomprobante.MontoHaber = d.MontoHaber;
                                                detallecomprobante.MontoDebeAlt = d.MontoDebeAlt;
                                                detallecomprobante.MontoHaberAlt = d.MontoHaberAlt;
                                                esquema.SaveChanges();
                                            }
                                            else
                                            {
                                                esquema.DetalleComprobante.Add(d);
                                                esquema.SaveChanges();
                                            }

                                        }

                                        if(eDetalleComprobante.Count > 0)
                                        {
                                            EliminarDetalleComprobante(eDetalleComprobante, esquema, Entidad.IdComprobante);
                                        }



                                    }
                                }

                            }
                            else
                            {

                                modcomprobante.IdMoneda = Entidad.IdMoneda;
                                modcomprobante.Glosa = Entidad.Glosa;
                                modcomprobante.Fecha = Entidad.Fecha;
                                modcomprobante.TipoCambio = Entidad.TipoCambio;
                                modcomprobante.TipoComprobante = Entidad.TipoComprobante;
                                esquema.SaveChanges();
                                int contador = 0;



                                foreach (var i in eDetalleComprobante)
                                {
                                    DetalleComprobante d = new DetalleComprobante();

                                    contador = contador + 1;
                                    d.Numero = contador;
                                    d.Glosa = i.Glosa;
                                    d.IdCuenta = i.IdCuenta;
                                    d.IdComprobante = Entidad.IdComprobante;
                                    d.IdUsuario = Entidad.IdUsuario;

                                    if (moneda.IdMonedaPrincipal == Entidad.IdMoneda)
                                    {
                                        d.MontoDebe = i.Debe;
                                        d.MontoHaber = i.Haber;
                                        d.MontoDebeAlt = Math.Round((i.Debe / Entidad.TipoCambio), 2);
                                        d.MontoHaberAlt = Math.Round((i.Haber / Entidad.TipoCambio), 2);

                                    }
                                    else
                                    {
                                        d.MontoDebe = Math.Round((i.Debe * Entidad.TipoCambio), 2);
                                        d.MontoHaber = Math.Round((i.Haber * Entidad.TipoCambio), 2);
                                        d.MontoDebeAlt = i.Debe;
                                        d.MontoHaberAlt = i.Haber;
                                    }

                                    var detallecomprobante = (from x in esquema.DetalleComprobante
                                                              where x.IdComprobante == Entidad.IdComprobante
                                                              && x.IdCuenta == i.IdCuenta
                                                              select x).FirstOrDefault();

                                    if (detallecomprobante != null)
                                    {
                                        detallecomprobante.Numero = d.Numero;
                                        detallecomprobante.Glosa = d.Glosa;
                                        detallecomprobante.IdCuenta = d.IdCuenta;
                                        detallecomprobante.MontoDebe = d.MontoDebe;
                                        detallecomprobante.MontoHaber = d.MontoHaber;
                                        detallecomprobante.MontoDebeAlt = d.MontoDebeAlt;
                                        detallecomprobante.MontoHaberAlt = d.MontoHaberAlt;
                                        esquema.SaveChanges();
                                    }
                                    else
                                    {
                                        esquema.DetalleComprobante.Add(d);
                                        esquema.SaveChanges();
                                    }

                                }

                                if (eDetalleComprobante.Count > 0)
                                {
                                    EliminarDetalleComprobante(eDetalleComprobante, esquema, Entidad.IdComprobante);
                                }

                            }


                        }
                        else
                        {
                            throw new MensageException("No existe un periodo para la fecha ingresada.");
                        }

                    }
                    else
                    {
                        throw new MensageException("No existe una gestion para la fecha ingresada.");
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

        public Comprobante AnularComprobante(long idcomprobante)
        {
            using (var esquema = GetEsquema())
            {
                try
                {
                    var comprobante = (from x in esquema.Comprobante
                                       where x.IdComprobante == idcomprobante
                                       select x).FirstOrDefault();

                    if(comprobante != null)
                    {

                        var nota = LNotaCompra.Logica.LNotaCompra.ObtenerNotaGeneralXComprobante(comprobante.IdEmpresa, comprobante.IdComprobante);

                        if(nota != null)
                        {
                            throw new MensageException("No se puedo anular el comprobante por que esta relacionado con una nota");
                        }

                        comprobante.Estado = (int)EstadoComprobante.Anualdo;
                        esquema.SaveChanges();
                    }
                    else
                    {
                        throw new MensageException("No se puedo obtener el comprobante");
                    }

                    return comprobante;
                }
                catch (MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
            }
             
        }

        public void EliminarDetalleComprobante(List<EDetalleComprobante> detalleComprobantes, dbSistemaErpEntities esquema,long idcomprobante)
        {
            try
            {
                List<long> idcuenta = new List<long>();

                foreach(var i in detalleComprobantes)
                {
                    idcuenta.Add(i.IdCuenta);
                }

                var dcomprobante = (from x in esquema.DetalleComprobante
                                    where x.IdComprobante == idcomprobante
                                    && !idcuenta.Contains(x.IdCuenta)
                                    select x
                                    ).ToList();

                foreach(var i in dcomprobante)
                {

                    esquema.DetalleComprobante.Remove(i);
                    esquema.SaveChanges();

                }

            }
            catch
            {
                throw new MensageException("Error al modificar el detalle comprobante");
            }
        }


        public List<ERDatosBasicoCuenta> ReporteDatosBasicoComprobante(string usuario, string empresa)
        {
            try
            {


                List<ERDatosBasicoCuenta> basicos = new List<ERDatosBasicoCuenta>();
                ERDatosBasicoCuenta eRDatosBasico = new ERDatosBasicoCuenta();
                eRDatosBasico.Usuario = usuario;
                //   eRDatosBasico.NombreGestion = gestion;
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

        public List<ERDatosBasicoComprobante> ReporteDatosBasico(string usuario, string empresa,long idmoneda, long idgestion, long idperiodo)
        {
            try
            {
                List<ERDatosBasicoComprobante> basicos = new List<ERDatosBasicoComprobante>();
                ERDatosBasicoComprobante eRDatosBasico = new ERDatosBasicoComprobante();

                var moneda = LMoneda.Logica.LMoneda.obtenerMoneda(idmoneda);
                var gestion = LGestion.Logica.LGestion.obtenerGestion(idgestion);

                var periodo = new Periodo();

                if(idperiodo == 0)
                {
                    eRDatosBasico.Usuario = usuario;
                    eRDatosBasico.NombreGestion = gestion.Nombre;
                    eRDatosBasico.NombrePeriodo = "Todos";
                    eRDatosBasico.NombreEmpresa = empresa;
                    eRDatosBasico.FechaActual = DateTime.Now.ToString("dd/MM/yyyy");
                    eRDatosBasico.Moneda = moneda.Abreviatura;

                }
                else
                {
                    periodo = LPeriodo.Logica.LPeriodo.obtenerPeriodo(idperiodo);
                    eRDatosBasico.Usuario = usuario;
                    eRDatosBasico.NombreGestion = gestion.Nombre;
                    eRDatosBasico.NombrePeriodo = periodo.Nombre;
                    eRDatosBasico.NombreEmpresa = empresa;
                    eRDatosBasico.FechaActual = DateTime.Now.ToString("dd/MM/yyyy");
                    eRDatosBasico.Moneda = moneda.Abreviatura;
                }

             
                basicos.Add(eRDatosBasico);

                return basicos;
            }
            catch (Exception ex)
            {
                throw new MensageException("Ha ocurrido un error.");
            }
        }
        public void Validacion(Comprobante Entidad)
        {

            if (Entidad.TipoCambio <= 0)
            {
                throw new MensageException("El cambio debe ser mayor a 0");
            }

        }

        public void ValidacionDetalleTotal(EDetalleTotal Entidad)
        {
            if (Entidad.TotalDebe != Entidad.TotalHaber)
            {
                throw new MensageException("El total debe y el total haber no son iguales");
            }
        }



        public List<ESumasSaldos> reporteSumasSaldos(long idgestion, long idempresa, long idmoneda)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<ESumasSaldos> ListSumasSaldos = new List<ESumasSaldos>();

                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

                   
                        var gestion = (from x in esquema.Gestion
                                       where x.IdGestion == idgestion
                                       select x).FirstOrDefault();


                       var cuentas = LCuenta.Logica.LCuenta.listarCuentaDetalle(idempresa);

                        

                        foreach(var i in cuentas)
                        {

                            ESumasSaldos sumasSaldos = new ESumasSaldos();
                            sumasSaldos.Cuenta = i.Codigo + " " + i.Nombre;
                            sumasSaldos.SaldosDebe = 0;
                            sumasSaldos.SaldoHaber = 0;
                            sumasSaldos.SumasDebe = 0;
                            sumasSaldos.SumasHaber = 0;

                            var detallecomprobante = (from x in esquema.DetalleComprobante
                                                           where x.IdCuenta == i.idCuenta
                                                           && x.Comprobante.Fecha >= gestion.FechaInicio 
                                                           && x.Comprobante.Fecha <= gestion.FechaFin
                                                           && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                      select x).ToList();
                            
                           foreach(var j in detallecomprobante)
                           {



                                if (moneda.IdMonedaPrincipal == idmoneda)
                                {
                                    sumasSaldos.SumasDebe = Math.Round((sumasSaldos.SumasDebe + j.MontoDebe ), 2) ;
                                    sumasSaldos.SumasHaber = Math.Round((sumasSaldos.SumasHaber + j.MontoHaber), 2);
                                    

                                }
                                else
                                {
                                   sumasSaldos.SumasDebe = Math.Round((sumasSaldos.SumasDebe + j.MontoDebeAlt), 2);
                                   sumasSaldos.SumasHaber = Math.Round((sumasSaldos.SumasHaber + j.MontoHaberAlt), 2);

                                }

                           }

                           if(sumasSaldos.SumasDebe > 0 || sumasSaldos.SumasHaber > 0)
                           {

                                double saldoTotal = Math.Round((sumasSaldos.SumasDebe - sumasSaldos.SumasHaber), 2);
                                if(saldoTotal > 0)
                                {
                                     sumasSaldos.SaldosDebe = saldoTotal;
                                }
                                else
                                {
                                    sumasSaldos.SaldoHaber = saldoTotal * -1;
                                }


                                 ListSumasSaldos.Add(sumasSaldos);
                           }


                        }

                      


                    return ListSumasSaldos;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
                }

            }
        }

        public List<EDetalleTotalSumasSaldos> TotalSumasSaldos(List<ESumasSaldos> detalleComprobantes)
        {


            try
            {

                List<EDetalleTotalSumasSaldos> eDetalleTotals = new List<EDetalleTotalSumasSaldos>();
                EDetalleTotalSumasSaldos detalleTotal = new EDetalleTotalSumasSaldos();
                detalleTotal.TotalSaldoHaber = 0;
                detalleTotal.TotalSaldosDebe = 0;
                detalleTotal.TotalSumasDebe = 0;
                detalleTotal.TotalSumasHaber = 0;


                foreach (var i in detalleComprobantes)
                {
                    detalleTotal.TotalSaldoHaber = Math.Round((detalleTotal.TotalSaldoHaber + i.SaldoHaber), 2); 
                    detalleTotal.TotalSaldosDebe = Math.Round((detalleTotal.TotalSaldosDebe + i.SaldosDebe), 2); ;
                    detalleTotal.TotalSumasDebe = Math.Round((detalleTotal.TotalSumasDebe + i.SumasDebe), 2); ;
                    detalleTotal.TotalSumasHaber = Math.Round((detalleTotal.TotalSumasHaber + i.SumasHaber), 2); ;
                }

                if(detalleTotal.TotalSumasDebe < 0)
                {
                    detalleTotal.TotalSumasDebe = detalleTotal.TotalSumasDebe * -1;
                }

                eDetalleTotals.Add(detalleTotal);

                return eDetalleTotals;


            }
            catch (Exception ex)
            {
                throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
            }

        }


        public List<ELibroMayoCabezera> reporteLibroMayorCabezera(long idgestion, long idperiodo, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<ELibroMayoCabezera> libroMayoCabezeras = new List<ELibroMayoCabezera>();
                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

                    if (idperiodo == 0)
                    {
                        var gestion = (from x in esquema.Gestion
                                       where x.IdGestion == idgestion
                                       select x).FirstOrDefault();

                        /*var comprobante = (from x in esquema.Comprobante
                                           where x.IdEmpresa == idempresa
                                           && x.Fecha >= gestion.FechaInicio && x.Fecha <= gestion.FechaFin
                                           orderby x.Fecha ascending
                                           select x).ToList();*/
                        var cuentadetalle = LCuenta.Logica.LCuenta.listarCuentaDetalle(idempresa);


                        foreach (var i in cuentadetalle)
                        {


                            var detallecomprobante = (from x in esquema.DetalleComprobante
                                                      where x.Comprobante.Fecha >= gestion.FechaInicio 
                                                      && x.Comprobante.Fecha <= gestion.FechaFin
                                                      && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                      && x.IdCuenta == i.idCuenta
                                                      select x).FirstOrDefault();


                            if(detallecomprobante != null)
                            {
                                ELibroMayoCabezera m = new ELibroMayoCabezera();
                                m.IdCuenta = i.idCuenta;
                                m.Cuenta = i.Codigo + " " + i.Nombre;
                                libroMayoCabezeras.Add(m);
                            }

                            
                        }

                    }
                    else
                    {

                        var periodo = (from x in esquema.Periodo
                                       where x.idPeriodo == idperiodo
                                       select x).FirstOrDefault();

                        var cuentadetalle = LCuenta.Logica.LCuenta.listarCuentaDetalle(idempresa);


                        foreach (var i in cuentadetalle)
                        {


                            var detallecomprobante = (from x in esquema.DetalleComprobante
                                                      where x.Comprobante.Fecha >= periodo.FechaInicio
                                                      && x.Comprobante.Fecha <= periodo.FechaFin
                                                      && x.IdCuenta == i.idCuenta
                                                      select x).FirstOrDefault();


                            if (detallecomprobante != null)
                            {
                                ELibroMayoCabezera m = new ELibroMayoCabezera();
                                m.IdCuenta = i.idCuenta;
                                m.Cuenta = i.Codigo + " " + i.Nombre;
                                libroMayoCabezeras.Add(m);
                            }


                        }

                     

                    }

                    return libroMayoCabezeras;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
                }

            }
        }

        public List<ELibroMayor> reporteLibroMayor(long idcuenta,long idgestion, long idperiodo, long idempresa,long idmoneda)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<ELibroMayor> libroMayor = new List<ELibroMayor>();
                   // List<ELibroMayoCabezera> libroMayoCabezeras = new List<ELibroMayoCabezera>();
                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

                    if (idperiodo == 0)
                    {
                        var gestion = (from x in esquema.Gestion
                                       where x.IdGestion == idgestion
                                       select x).FirstOrDefault();

                        /*var comprobante = (from x in esquema.Comprobante
                                           where x.IdEmpresa == idempresa
                                           && x.Fecha >= gestion.FechaInicio && x.Fecha <= gestion.FechaFin
                                           orderby x.Fecha ascending
                                           select x).ToList();*/
                        //  var cuentadetalle = LCuenta.Logica.LCuenta.listarCuentaDetalle(idempresa);


                        //  foreach (var i in cuentadetalle)
                        // {
                        /*
                        
                       
                         */
                       
                        var detallecomprobante = (from x in esquema.DetalleComprobante
                                                      where x.Comprobante.Fecha >= gestion.FechaInicio
                                                      && x.Comprobante.Fecha <= gestion.FechaFin
                                                      && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                      && x.IdCuenta == idcuenta
                                                      select x).ToList();

                        double saldo = 0;

                        foreach (var i in detallecomprobante)
                        {
                            ELibroMayor m = new ELibroMayor();
                            m.IdCuenta = i.IdCuenta ;
                            m.Cuenta = i.Cuenta.Codigo + " "+ i.Cuenta.Nombre;

                            m.Fecha = i.Comprobante.Fecha.ToString("dd/MM/yyyy");
                            if (moneda.IdMonedaPrincipal == idmoneda)
                            {

                                m.NroComprobante = i.Comprobante.Serie;
                                switch (i.Comprobante.TipoComprobante)
                                {
                                    case (int)TipoComprobante.Apertura:
                                        m.Tipo = "Apertura";
                                        break;
                                    case (int)TipoComprobante.Egreso:
                                        m.Tipo = "Egreso";
                                        break;
                                    case (int)TipoComprobante.Ingreso:
                                        m.Tipo = "Ingreso";
                                        break;
                                    case (int)TipoComprobante.Traspaso:
                                        m.Tipo = "Traspaso";
                                        break;
                                    case (int)TipoComprobante.Ajuste:
                                        m.Tipo = "Ajuste";
                                        break;


                                }
                                m.Glosa = i.Comprobante.Glosa;

                                m.Debe = i.MontoDebe;
                                m.Haber = i.MontoHaber;

                                if (m.Debe > 0)
                                {
                                    saldo = saldo + m.Debe;
                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }
                                    //m.Saldo = saldo;

                                }
                                else
                                {
                                    saldo = saldo - m.Haber;

                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }



                                }

                            }
                            else
                            {
                                m.NroComprobante = i.Comprobante.Serie;
                                switch (i.Comprobante.TipoComprobante)
                                {
                                    case (int)TipoComprobante.Apertura:
                                        m.Tipo = "Apertura";
                                        break;
                                    case (int)TipoComprobante.Egreso:
                                        m.Tipo = "Egreso";
                                        break;
                                    case (int)TipoComprobante.Ingreso:
                                        m.Tipo = "Ingreso";
                                        break;
                                    case (int)TipoComprobante.Traspaso:
                                        m.Tipo = "Traspaso";
                                        break;
                                    case (int)TipoComprobante.Ajuste:
                                        m.Tipo = "Ajuste";
                                        break;


                                }
                                m.Glosa = i.Comprobante.Glosa;
                                m.Debe = i.MontoDebeAlt;
                                m.Haber = i.MontoHaberAlt;

                                if (m.Debe > 0)
                                {
                                    saldo = Math.Round((saldo + m.Debe), 2) ;
                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }
                                    //m.Saldo = saldo;

                                }
                                else
                                {
                                    saldo = Math.Round((saldo - m.Haber), 2);

                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }



                                }

                             
                            }

                            libroMayor.Add(m);


                        }

                     

                    }
                    else
                    {

                        var periodo = (from x in esquema.Periodo
                                       where x.idPeriodo == idperiodo
                                       select x).FirstOrDefault();

                        var detallecomprobante = (from x in esquema.DetalleComprobante
                                                  where x.Comprobante.Fecha >= periodo.FechaInicio
                                                  && x.Comprobante.Fecha <= periodo.FechaFin
                                                  && x.IdCuenta == idcuenta
                                                  select x).ToList();

                        double saldo = 0;

                        foreach (var i in detallecomprobante)
                        {
                            ELibroMayor m = new ELibroMayor();
                            m.IdCuenta = i.IdCuenta;
                            m.Cuenta = i.Cuenta.Codigo+ i.Cuenta.Nombre;
                            m.Glosa = i.Comprobante.Glosa;
                            m.Fecha = i.Comprobante.Fecha.ToString("dd/MM/yyyy");
                            if (moneda.IdEmpresa == idmoneda)
                            {

                                m.NroComprobante = i.Comprobante.Serie;
                                switch (i.Comprobante.TipoComprobante)
                                {
                                    case (int)TipoComprobante.Apertura:
                                        m.Tipo = "Apertura";
                                        break;
                                    case (int)TipoComprobante.Egreso:
                                        m.Tipo = "Egreso";
                                        break;
                                    case (int)TipoComprobante.Ingreso:
                                        m.Tipo = "Ingreso";
                                        break;
                                    case (int)TipoComprobante.Traspaso:
                                        m.Tipo = "Traspaso";
                                        break;
                                    case (int)TipoComprobante.Ajuste:
                                        m.Tipo = "Ajuste";
                                        break;


                                }

                                m.Debe = i.MontoDebe;
                                m.Haber = i.MontoHaber;

                                if (m.Debe > 0)
                                {
                                    saldo = saldo + m.Debe;
                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }
                                    //m.Saldo = saldo;

                                }
                                else
                                {
                                    saldo = saldo - m.Haber;

                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }



                                }

                            }
                            else
                            {
                                m.NroComprobante = i.Comprobante.Serie;
                                switch (i.Comprobante.TipoComprobante)
                                {
                                    case (int)TipoComprobante.Apertura:
                                        m.Tipo = "Apertura";
                                        break;
                                    case (int)TipoComprobante.Egreso:
                                        m.Tipo = "Egreso";
                                        break;
                                    case (int)TipoComprobante.Ingreso:
                                        m.Tipo = "Ingreso";
                                        break;
                                    case (int)TipoComprobante.Traspaso:
                                        m.Tipo = "Traspaso";
                                        break;
                                    case (int)TipoComprobante.Ajuste:
                                        m.Tipo = "Ajuste";
                                        break;


                                }

                                m.Debe = i.MontoDebeAlt;
                                m.Haber = i.MontoHaberAlt;

                                if (m.Debe > 0)
                                {
                                    saldo = Math.Round((saldo + m.Debe), 2);
                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }
                                    //m.Saldo = saldo;

                                }
                                else
                                {
                                    saldo = Math.Round((saldo - m.Haber), 2);

                                    if (saldo >= 0)
                                    {
                                        m.Saldo = saldo;
                                    }
                                    else
                                    {
                                        m.Saldo = saldo * -1;
                                    }



                                }


                            }
                            libroMayor.Add(m);



                        }



                    }

                    return libroMayor;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
                }

            }
        }



        public List<EEstadoResultado> CabezaraEstadoResultado(long idgestion, long idempresa, int tipocomprobante)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    List<EEstadoResultado> libroMayoCabezeras = new List<EEstadoResultado>();
                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

                   
                        var gestion = (from x in esquema.Gestion
                                       where x.IdGestion == idgestion
                                       select x).FirstOrDefault();

                        /*var comprobante = (from x in esquema.Comprobante
                                           where x.IdEmpresa == idempresa
                                           && x.Fecha >= gestion.FechaInicio && x.Fecha <= gestion.FechaFin
                                           orderby x.Fecha ascending
                                           select x).ToList();*/

                        if(tipocomprobante == (int)TipoEstadoResultado.Ingreso)
                        {

                            var cuentadetalle = (from x in esquema.Cuenta
                                                 where x.TipoDeCuenta == (int)TipoCuenta.Detalle
                                                 && x.IdEmpresa == idempresa
                                                 && x.Codigo.StartsWith("4.")
                                                 select x).ToList();


                            foreach (var i in cuentadetalle)
                            {




                                var detallecomprobante = (from x in esquema.DetalleComprobante
                                                          where x.Cuenta.idCuenta == i.idCuenta 
                                                          && x.Comprobante.Fecha >= gestion.FechaInicio
                                                          && x.Comprobante.Fecha <= gestion.FechaFin
                                                          && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                          select x).FirstOrDefault();


                                if (detallecomprobante != null)
                                {
                                    EEstadoResultado m = new EEstadoResultado();
                                    m.IdCuenta = i.idCuenta;
                                    m.Cuenta = i.Codigo + " " + i.Nombre;
                                    libroMayoCabezeras.Add(m);
                                }


                            }


                        }
                        else
                        {



                            var cuentadetalle = (from x in esquema.Cuenta
                                                 where x.TipoDeCuenta == (int)TipoCuenta.Detalle
                                                  && x.IdEmpresa == idempresa
                                                 && x.Codigo.StartsWith("5.")
                                                 select x).ToList();


                            foreach (var i in cuentadetalle)
                            {




                                var detallecomprobante = (from x in esquema.DetalleComprobante
                                                          where x.Comprobante.Fecha >= gestion.FechaInicio
                                                          && x.Comprobante.Fecha <= gestion.FechaFin
                                                          && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                          && x.IdCuenta == i.idCuenta
                                                          select x).FirstOrDefault();


                                if (detallecomprobante != null)
                                {
                                    EEstadoResultado m = new EEstadoResultado();
                                    m.IdCuenta = i.idCuenta;
                                    m.Cuenta = i.Codigo + " " + i.Nombre;
                                    libroMayoCabezeras.Add(m);
                                }


                            }
                        }

                       
                   
                   

                    return libroMayoCabezeras;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de estado de resultado");
                }

            }
        }

        public List<EEstadoResultado> EstadoResultado(long idcuenta, long idgestion, long idempresa, long idmoneda, int tipocombrobante)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    
                    List<EEstadoResultado> lista = new List<EEstadoResultado>();
                    var moneda = LMoneda.Logica.LMoneda.obtenerMonedadXempresa(idempresa);

              
                        var gestion = (from x in esquema.Gestion
                                       where x.IdGestion == idgestion
                                       select x).FirstOrDefault();

                        
                      

                        


                    if (tipocombrobante == (int)TipoEstadoResultado.Ingreso)
                    {
                        //Ingreso

                        var detallecomprobante = (from x in esquema.DetalleComprobante
                                                  where x.Comprobante.Fecha >= gestion.FechaInicio
                                                  && x.Comprobante.Fecha <= gestion.FechaFin
                                                  && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                  && x.Cuenta.TipoDeCuenta == (int)TipoCuenta.Detalle
                                                  && x.Cuenta.Codigo.StartsWith("4.")
                                                  && x.IdCuenta == idcuenta
                                                  select x).ToList();

                        double TotalDebe = 0;
                        double TotalHaber = 0;



                        EEstadoResultado r = new EEstadoResultado();



                        r.IdCuenta = detallecomprobante.FirstOrDefault().IdCuenta;
                        r.Cuenta = detallecomprobante.FirstOrDefault().Cuenta.Codigo + " " + detallecomprobante.FirstOrDefault().Cuenta.Nombre;


                        foreach (var i in detallecomprobante)
                        {



                            if (moneda.IdMonedaPrincipal == idmoneda)
                            {



                                TotalDebe = Math.Round((TotalDebe + i.MontoDebe), 2);
                                TotalHaber = Math.Round((TotalHaber + i.MontoHaber), 2);


                            }
                            else
                            {
                                TotalDebe = Math.Round((TotalDebe + i.MontoDebeAlt), 2);
                                TotalHaber = Math.Round((TotalHaber + i.MontoHaberAlt), 2);

                            }
                        }

                        r.TipoComprobante = "Ingreso";
                        r.Total = Math.Round((TotalHaber - TotalDebe), 2);
                        lista.Add(r);
                    }
                    else
                    {

                        //Egreso

                        var detallecomprobante = (from x in esquema.DetalleComprobante
                                                  where x.Comprobante.Fecha >= gestion.FechaInicio
                                                  && x.Comprobante.Fecha <= gestion.FechaFin
                                                  && x.Comprobante.Estado != (int)EstadoComprobante.Anualdo
                                                  && x.Cuenta.TipoDeCuenta == (int)TipoCuenta.Detalle
                                                  && x.Cuenta.Codigo.StartsWith("5.")
                                                  && x.IdCuenta == idcuenta
                                                  select x).ToList();

                        double TotalDebe = 0;
                        double TotalHaber = 0;



                        EEstadoResultado r = new EEstadoResultado();



                        r.IdCuenta = detallecomprobante.FirstOrDefault().IdCuenta;
                        r.Cuenta = detallecomprobante.FirstOrDefault().Cuenta.Codigo + " " + detallecomprobante.FirstOrDefault().Cuenta.Nombre;


                        foreach (var i in detallecomprobante)
                        {



                            if (moneda.IdMonedaPrincipal == idmoneda)
                            {



                                TotalDebe = Math.Round((TotalDebe + i.MontoDebe), 2);
                                TotalHaber = Math.Round((TotalHaber + i.MontoHaber), 2);


                            }
                            else
                            {
                                TotalDebe = Math.Round((TotalDebe + i.MontoDebeAlt), 2);
                                TotalHaber = Math.Round((TotalHaber + i.MontoHaberAlt), 2);

                            }
                        }
                        r.TipoComprobante = "Egreso";
                        r.Total = Math.Round((TotalDebe - TotalHaber), 2);
                        lista.Add(r);
                    }



                   
                    return lista;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de detalle comprobantes");
                }

            }
        }

       
        public List<ETotalEstado> totalEstados(double total)
        {

            List<ETotalEstado> totalEstados = new List<ETotalEstado>();

            ETotalEstado eTotal = new ETotalEstado();

            eTotal.Total = total;

            totalEstados.Add(eTotal);

            return totalEstados;

        }


    }
}
