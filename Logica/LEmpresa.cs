using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidad;
using Entidad.Estados;

namespace Logica
{
    public class LEmpresa:LLogica<Empresa>
    {
        public List<EREmpresa> ReporteListaEmpresaReportView()
        {
            List<EREmpresa> empresas = new List<EREmpresa>();

            try
            {
                using (var esquema = GetEsquema())
                {
                    var empresa = (from x in esquema.Empresa
                                       // where x.IdUsuario == idusuario
                                   select x).ToList();



                    foreach (var i in empresa)
                    {
                        EREmpresa e = new EREmpresa();

                        e.Sigla = i.Sigla;
                        e.Nombre = i.Nombre;
                        e.NIT = i.NIT;
                        e.Telefono = i.Telefono;
                        e.Direccion = i.Direccion;

                        empresas.Add(e);

                    }
                }


            }
            catch (Exception ex)
            {
                // throw new MensageException("Error no se puedo obtener la lista de empresas");
            }



            return empresas;
        }

        public List<Empresa> listarEmpresa(long idusuario)
        {
            using(var esquema = GetEsquema())
            {

                try
                {
                    var empresa = (from x in esquema.Empresa
                                  // where x.IdUsuario == idusuario
                                   select x).ToList();

                    List<Empresa> empresas = new List<Empresa>();

                    empresas = empresa;

                    return empresas;

                }catch(Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de empresas");
                }

                

            }
        }

      

        public Empresa obtenerEmpresa(long idempresa)
        {

            using (var esquema = GetEsquema())
            {

                try
                {

                    if(idempresa < 0)
                    {
                        throw new MensageException("Seleccione una empresa");
                    }

                    var empresa = (from x in esquema.Empresa
                                   where x.idEmpresa == idempresa
                                   select x).FirstOrDefault();


                    if(empresa == null)
                    {
                        throw new MensageException("No se puede obtener la empresa");
                    }

                    return empresa;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la empresas");
                }


            }
        }  

        public Empresa Registro (Empresa Entidad,long idmoneda)
        {
            using(var esquema = GetEsquema())
            {
                try
                {
                    Validacion(Entidad,idmoneda);

                    Empresa empresaexiste = (from x in esquema.Empresa
                                             where x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                             || x.NIT.Trim().ToUpper() == Entidad.NIT.Trim().ToUpper()
                                             ||  x.Sigla.Trim().ToUpper() == Entidad.Sigla.Trim().ToUpper()
                                             select x).FirstOrDefault();

                    validarExistencia(empresaexiste, Entidad);

                    esquema.Empresa.Add(Entidad);
                    esquema.SaveChanges();


                    EmpresaMoneda empresaMoneda = new EmpresaMoneda()
                    {
                        Activo = (int)EstadoMonedaEmpresa.Activo,
                        FechaRegistro = DateTime.Now,
                        IdEmpresa = Entidad.idEmpresa,
                        IdMonedaPrincipal = idmoneda,
                        IdUsuario = Entidad.IdUsuario

                    };

                    esquema.EmpresaMoneda.Add(empresaMoneda);
                    esquema.SaveChanges();    


                    for(int i = 0; i < 5; i++)
                    {

                        switch (i)
                        {
                            case 0:
                                Cuenta cuenta = new Cuenta()
                                {
                                    Nombre = "Activo",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,



                                };
                                cuenta = LCuenta.Logica.LCuenta.Registro(cuenta, 0, 1);
                             break;
                            case 1:
                                Cuenta cuenta1 = new Cuenta()
                                {
                                    Nombre = "Pasivo",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,



                                };
                                cuenta1 = LCuenta.Logica.LCuenta.Registro(cuenta1, 0, 1);
                                break;
                            case 2:
                                Cuenta cuenta2 = new Cuenta()
                                {
                                    Nombre = "Patrimonio",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,



                                };
                                cuenta2 = LCuenta.Logica.LCuenta.Registro(cuenta2, 0, 1);
                                break;
                            case 3:
                                Cuenta cuenta3 = new Cuenta()
                                {
                                    Nombre = "Ingresos",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,



                                };
                                cuenta3 = LCuenta.Logica.LCuenta.Registro(cuenta3, 0, 1);
                                break;
                            case 4:
                                Cuenta cuenta4 = new Cuenta()
                                {
                                    Nombre = "Egresos",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,



                                };
                                cuenta4 = LCuenta.Logica.LCuenta.Registro(cuenta4, 0, 1);

                                Cuenta cuentahijo1 = new Cuenta()
                                {
                                    Nombre = "Costos",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,
                                    IdCuentaPadre = cuenta4.idCuenta


                                };
                                cuentahijo1 = LCuenta.Logica.LCuenta.Registro(cuentahijo1, cuenta4.idCuenta, 0);

                                Cuenta cuentahijo2 = new Cuenta()
                                {
                                    Nombre = "Gastos",
                                    Codigo = "",
                                    Nivel = 0,
                                    TipoDeCuenta = (int)TipoCuenta.Global,
                                    IdUsuario = Entidad.IdUsuario,
                                    IdEmpresa = Entidad.idEmpresa,
                                    IdCuentaPadre = cuenta4.idCuenta


                                };
                                cuentahijo2 = LCuenta.Logica.LCuenta.Registro(cuentahijo2, cuenta4.idCuenta, 0);
                                break;
                        }
                      
                    }


                    return Entidad;

                }
                catch(MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
              
            }
        }

        public Empresa Editar(Empresa Entidad,long idmoneda)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    Validacion(Entidad,idmoneda);

                    Empresa empresaexiste = (from x in esquema.Empresa
                                             where x.idEmpresa != Entidad.idEmpresa
                                             && (x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                             || x.NIT.Trim().ToUpper() == Entidad.NIT.Trim().ToUpper()
                                             || x.Sigla.Trim().ToUpper() == Entidad.Sigla.Trim().ToUpper())
                                             select x).FirstOrDefault();

                    validarExistencia(empresaexiste, Entidad);


                    Empresa empresa = (from x in esquema.Empresa
                                   where x.idEmpresa == Entidad.idEmpresa
                                   select x).FirstOrDefault();
                    if (empresa == null)
                    {
                        throw new MensageException("No se puede obtener la empresa");
                    }

                    empresa.Nombre = Entidad.Nombre;
                    empresa.NIT = Entidad.NIT;
                    empresa.Sigla = Entidad.Sigla;
                    empresa.Telefono = Entidad.Telefono;
                    empresa.Correo = Entidad.Correo;
                    empresa.Niveles = Entidad.Niveles;
                    empresa.Direccion = Entidad.Direccion;
                   // empresa.IdUsuario = Entidad.IdUsuario;


                    var moneda = LMoneda.Logica.LMoneda.obtenerPrimerMonedadXempresa(empresa.idEmpresa);
                    if(moneda != null)
                    {
                        if(idmoneda != moneda.IdMonedaPrincipal)
                        {
                            throw new MensageException("No se puede cambiar la moneda principal por que ya existe un registro");
                        }

                    }
                    else
                    {
                        EmpresaMoneda empresaMoneda = new EmpresaMoneda()
                        {
                            Activo = (int)EstadoMonedaEmpresa.Activo,
                            FechaRegistro = DateTime.Now,
                            IdEmpresa = empresa.idEmpresa,
                            IdMonedaPrincipal = idmoneda,
                            IdUsuario = empresa.IdUsuario

                        };

                        esquema.EmpresaMoneda.Add(empresaMoneda);
                    }


                    esquema.SaveChanges();

                    return empresa;
                }
                catch(MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
        }

        public void Eliminar(long idempresa)
        {
            using(var esquema = GetEsquema())
            {
                try
                {
                    var empresa = (from x in esquema.Empresa
                                   where x.idEmpresa == idempresa
                                   select x).FirstOrDefault();

                    if(empresa == null)
                    {
                        throw new MensageException("No se puede obtener la empresa");
                    }

                    if(empresa.Gestion.Count() > 0)
                    {
                        throw new MensageException("No se puede eliminar la empresa por que ya tiene registada una gestion");
                        
                    }
                    else
                    {
                        esquema.Empresa.Remove(empresa);
                        esquema.SaveChanges();
                    }

                }catch(MensageException ex)
                {
                    throw new MensageException(ex.Message);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void Validacion(Empresa Entidad, long idMoneda)
        {

            if (string.IsNullOrEmpty(Entidad.Nombre))
            {
                throw new MensageException("El nombre es obligatorio.");
            }

            if (string.IsNullOrEmpty(Entidad.Sigla))
            {
                throw new MensageException("La sigla es obligatorio.");
            }
            if (!string.IsNullOrEmpty(Entidad.Correo))
            {
                try
                {
                    new MailAddress(Entidad.Correo);
                }
                catch(Exception ex)
                {
                    throw new MensageException("Ingrese un correo valido.");
                }
            }
            if (string.IsNullOrEmpty(Entidad.NIT))
            {
                throw new MensageException("El nit es obligatorio.");
            }

            if(idMoneda == -1)
            {
                throw new MensageException("Seleccione una moneda.");
            }
          /*  if (string.IsNullOrEmpty(Entidad.Direccion))
            {
                throw new MensageException("La direccion es obligatorio.");
            }*/
          /*  if (Entidad.Niveles<3 || Entidad.Niveles>8  )
            {
                throw new MensageException("El nivel tiene que ser entre 3 o 7.");
            }
            if (Entidad.Niveles < 3 || Entidad.Niveles > 8)
            {
                throw new MensageException("El nivel tiene que ser entre 3 o 7.");
            }*/


        }

        public void validarExistencia(Empresa Existe, Empresa Entidad)
        {
            if (Existe != null)
            {
               
                if (Existe.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper())
                {
                    throw new MensageException("Ya existe el nombre de la empresa.");
                }

                if (Existe.NIT.Trim().ToUpper() == Entidad.NIT.Trim().ToUpper())
                {
                    throw new MensageException("Ya existe el nit de la empresa.");
                }

                if (Existe.Sigla.Trim().ToUpper() == Entidad.Sigla.Trim().ToUpper())
                {
                    throw new MensageException("Ya existe la sigla de la empresa.");
                }

            }
        }


        public List<ERDatosBasico> ReporteDatosBasico(string usuario)
        {
            try
            {


                List<ERDatosBasico> basicos = new List<ERDatosBasico>();
                ERDatosBasico eRDatosBasico = new ERDatosBasico();
                eRDatosBasico.Usuario = usuario;
                eRDatosBasico.FechaActual = DateTime.Now.ToString("dd/MM/yyyy");

                basicos.Add(eRDatosBasico);

                return basicos;
            }
            catch (Exception ex)
            {
                throw new MensageException("Ha ocurrido un error.");
            }
        }


       



    }
}
