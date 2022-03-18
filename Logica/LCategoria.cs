using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidad;

namespace Logica
{
    public class LCategoria: LLogica<Categoria>
    {

        public List<ECategoria> listarCategorias(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var linqcategoria = (from x in esquema.Categoria
                                      where x.IdEmpresa == idempresa
                                      && x.IdCategoriaPadre == null
                                      select x).ToList();


                    List<ECategoria> categorias = new List<ECategoria>();



                    foreach (var i in linqcategoria)
                    {
                        ECategoria c = new ECategoria();
                        c.id = i.IdCategoria;
                        c.IdCategoria = i.IdCategoria;
                        c.Nombre = i.Nombre;
                        c.text = i.Nombre;

                        c.IdUsuario = i.IdUsuario;
                        c.IdEmpresa = i.IdEmpresa;
                        c.IdCategoriaPadre = i.IdCategoriaPadre;
                        c.children = CategoriaHijos(c.IdCategoria, idempresa);
                        categorias.Add(c);
                    }

                    return categorias;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de categorias");
                }



            }
        }


        public List<ECategoria> CategoriaHijos(long idPadre, long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var linqcategoria = (from x in esquema.Categoria
                                      where x.IdEmpresa == idempresa
                                      && x.IdCategoriaPadre == idPadre
                                      select x).ToList();


                    List<ECategoria> categorias = new List<ECategoria>();


                    //  empresas = empresa;


                    foreach (var i in linqcategoria)
                    {
                        ECategoria c = new ECategoria();
                        c.id = i.IdCategoria;
                        c.IdCategoria = i.IdCategoria;
                        c.Nombre = i.Nombre;
                        c.text = i.Nombre;
                      
                        c.IdUsuario = i.IdUsuario;
                        c.IdEmpresa = i.IdEmpresa;
                        c.IdCategoriaPadre = i.IdCategoriaPadre;
                        c.children = CategoriaHijos(c.IdCategoria, idempresa);
                        categorias.Add(c);
                    }

                    return categorias;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de categorias");
                }



            }
        }

        public Categoria obtenerCategoria(long idcategoria)
        {

            using (var esquema = GetEsquema())
            {

                try
                {
                    var categoria = (from x in esquema.Categoria
                                      where x.IdCategoria == idcategoria
                                      select x).FirstOrDefault();

                    if (categoria == null)
                    {
                        throw new MensageException("Error no se puedo obtener la cuenta");
                    }

                    return categoria;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la cuenta");
                }

            }
        }


        public Categoria eliminarCategoria(long idcategoria)
        {

            using (var esquema = GetEsquema())
            {

                try
                {
                    var categoria = (from x in esquema.Categoria
                                      where x.IdCategoria == idcategoria
                                      select x).FirstOrDefault();

                    if (categoria == null)
                    {
                        throw new MensageException("Error no se puedo obtener la cuenta");
                    }


                    if (categoria.Categoria1.Count() > 0)
                    {
                        throw new MensageException("No se puede eliminar la categoria de esta empresa esta relacionada");

                    }

                   

                    esquema.Categoria.Remove(categoria);
                    esquema.SaveChanges();


                    return categoria;

                }
                catch (Exception ex)
                {
                    throw new MensageException(ex.Message);
                }

            }
        }

        public Categoria ModificarCategoria(long idcategoria, string nombre, long idempresa)
        {

            using (var esquema = GetEsquema())
            {

                try
                {

                    var validar = (from x in esquema.Categoria
                                   where x.IdEmpresa == idempresa
                                   && x.IdCategoria != idcategoria
                                   && x.Nombre.Trim().ToUpper() == nombre.Trim().ToUpper()
                                   select x
                                   ).FirstOrDefault();

                    if (validar != null)
                    {
                        throw new MensageException("El nombre de la categoria en esta empresa ya existe");
                    }

                    var categoria = (from x in esquema.Categoria
                                      where x.IdCategoria == idcategoria
                                      select x).FirstOrDefault();

                    if (categoria == null)
                    {
                        throw new MensageException("Error no se puedo obtener la cuenta");
                    }

                    categoria.Nombre = nombre;
                    esquema.SaveChanges();

                    return categoria;

                }
                catch (Exception ex)
                {
                    throw new MensageException(ex.Message);
                }

            }
        }


        public Categoria Registro(Categoria Entidad, long idcategoria, int padre)
        {
            using (var esquema = GetEsquema())
            {
                try
                {

                

                    var validar = (from x in esquema.Categoria
                                   where x.IdEmpresa == Entidad.IdEmpresa
                                   && x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                   select x
                                    ).FirstOrDefault();

                    if (validar != null)
                    {
                        throw new MensageException("El nombre de la categoria en esta empresa ya existe");
                    }


                  /*  var linqcuenta = (from x in esquema.Cuenta
                                      where x.IdEmpresa == Entidad.IdEmpresa
                                      select x
                                    ).FirstOrDefault();*/

                

                        
                            if (padre == 0)
                            {
                                //hijos
                                var cuentapadre = (from x in esquema.Categoria
                                                   where x.IdEmpresa == Entidad.IdEmpresa
                                                   && x.IdCategoria == idcategoria
                                                   select x
                                                  ).FirstOrDefault();

                                if (cuentapadre != null)
                                {

                                    esquema.Categoria.Add(Entidad);
                                    esquema.SaveChanges();


                                }
                                else
                                {
                                  throw new MensageException("Error al registrar la categoria");
                                }


                            }
                            else if (padre == 1)
                            {
                                //padres

                                var linqpadres = (from x in esquema.Categoria
                                                  where x.IdEmpresa == Entidad.IdEmpresa
                                                  select x
                                                 ).FirstOrDefault();

                              //  if (linqpadres != null)
                               // {
                                    

                                    esquema.Categoria.Add(Entidad);
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

        public List<Categoria> listadosdecategorias(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var categorias = (from x in esquema.Categoria
                                         where x.IdEmpresa == idempresa
                                         select x).ToList();

                    return categorias;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de categorias");
                }



            }
        }

       

    }
}
