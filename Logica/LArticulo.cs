using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidad;
using Datos;

namespace Logica
{
    public class LArticulo: LLogica<Articulo>
    {

        public List<Articulo> listarArticulo(long idempresa)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var articulos = (from x in esquema.Articulo
                                   where x.IdEmpresa == idempresa
                                   select x).ToList();


                    return articulos;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de gestiones");
                }

            }
        }



        public Articulo Registro(Articulo Entidad, List<ECategoriaJSON> categoriaJSON)
        {
            using (var esquema = GetEsquema())
            {
                try
                {

                    if (String.IsNullOrEmpty(Entidad.Nombre))
                    {
                        throw new MensageException("El nombre del articulo es obligatorio");
                    }

                    if (double.IsNaN(Entidad.PrecioVenta))
                    {
                        throw new MensageException("El precio del articulo es obligatorio");
                    }

                    if(categoriaJSON == null)
                    {
                        throw new MensageException("La categoria es obligatorio");
                    }


                    var validacion = (from x in esquema.Articulo
                                      where x.IdEmpresa == Entidad.IdEmpresa
                                      && x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                      select x).FirstOrDefault();

                    if(validacion != null)
                    {
                        throw new MensageException("Ya existe el nombre del articulo");
                    }


                    List<Categoria> categorias = new List<Categoria>();

                    foreach(var i in categoriaJSON)
                    {

                        Categoria categoria = new Categoria();

                        var cat = (from x in esquema.Categoria
                                         where x.IdCategoria == i.IdCategoria
                                         && x.IdEmpresa == Entidad.IdEmpresa
                                         select x).FirstOrDefault();

                        categorias.Add(cat);

                    }

                    Entidad.Categoria = categorias;
                    esquema.Articulo.Add(Entidad);
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

        public Articulo Modificar(Articulo Entidad, List<ECategoriaJSON> categoriaJSON)
        {
            using (var esquema = GetEsquema())
            {
                try
                {

                    if (String.IsNullOrEmpty(Entidad.Nombre))
                    {
                        throw new MensageException("El nombre del articulo es obligatorio");
                    }

                    if (double.IsNaN(Entidad.PrecioVenta))
                    {
                        throw new MensageException("El precio del articulo es obligatorio");
                    }

                    if (categoriaJSON == null)
                    {
                        throw new MensageException("La categoria es obligatorio");
                    }


                    var validacion = (from x in esquema.Articulo
                                      where x.IdEmpresa == Entidad.IdEmpresa
                                      && x.IdArticulo != Entidad.IdArticulo
                                      && x.Nombre.Trim().ToUpper() == Entidad.Nombre.Trim().ToUpper()
                                      select x).FirstOrDefault();

                    if (validacion != null)
                    {
                        throw new MensageException("Ya existe el nombre del articulo.");
                    }

                    var articulo = (from x in esquema.Articulo
                                    where x.IdArticulo == Entidad.IdArticulo
                                    && x.IdEmpresa == Entidad.IdEmpresa
                                    select x).FirstOrDefault();


                   

                    if (articulo != null)
                    {



                        List<Categoria> auxiliar = new List<Categoria>();
                        foreach(var i in articulo.Categoria)
                        {
                            Categoria c = new Categoria();
                            c = i;
                            auxiliar.Add(c);
                        }


                         foreach(var i in auxiliar)
                          {
                              articulo.Categoria.Remove(i);
                              esquema.SaveChanges();

                          }

                

                        List<Categoria> categorias = new List<Categoria>();

                        foreach (var i in categoriaJSON)
                        {

                            Categoria categoria = new Categoria();

                            var cat = (from x in esquema.Categoria
                                       where x.IdCategoria == i.IdCategoria
                                       && x.IdEmpresa == Entidad.IdEmpresa
                                       select x).FirstOrDefault();

                            categorias.Add(cat);

                        }


                        articulo.Nombre = Entidad.Nombre;
                        articulo.Descripcion = Entidad.Descripcion;
                      
                        articulo.Categoria =categorias;
                        esquema.SaveChanges();

                        return articulo;
                        //articulo.
                    }
                    else
                    {
                        throw new MensageException("No se pudo obtener el articulo.");
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


        public EArticulo ObtenerArticulo(long idarticulo, long idempresa)
        {
            using (var esquema = GetEsquema())
            {
                try
                {

                 

                    var articulo = (from x in esquema.Articulo
                                    where x.IdArticulo == idarticulo
                                    && x.IdEmpresa == idempresa
                                    select x).FirstOrDefault();


                    if(articulo != null)
                    {

                        EArticulo earticulo = new EArticulo();

                        earticulo.IdCategoria = articulo.IdArticulo;
                        earticulo.Nombre = articulo.Nombre;
                        earticulo.Descripcion = articulo.Descripcion;
                        earticulo.Precio = articulo.PrecioVenta;
                        earticulo.Categoria = new List<ECategoriaJSON>();

                        foreach(var i in articulo.Categoria)
                        {
                            ECategoriaJSON c = new ECategoriaJSON();
                            c.IdCategoria = i.IdCategoria;
                            earticulo.Categoria.Add(c);
                        }

                        return earticulo;

                    }
                    else
                    {
                        throw new MensageException("No se pudo obtener el articulo");
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

        public Articulo obtenerArticulo1(long idarticulo, long idempresa)
        {
            using (var esquema = GetEsquema())
            {
                try
                {



                    var articulo = (from x in esquema.Articulo
                                    where x.IdArticulo == idarticulo
                                    && x.IdEmpresa == idempresa
                                    select x).FirstOrDefault();


                    if (articulo != null)
                    {


                        return articulo;

                    }
                    else
                    {
                        throw new MensageException("No se pudo obtener el articulo");
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

        public Articulo Eliminar(long idarticulo, long idempresa)
        {
            using (var esquema = GetEsquema())
            {
                try
                {



                    var articulo = (from x in esquema.Articulo
                                    where x.IdArticulo == idarticulo
                                    && x.IdEmpresa == idempresa
                                    select x).FirstOrDefault();


                    if (articulo != null)
                    {


                        if(articulo.Lote.Count > 0)
                        {
                            throw new MensageException("El articulo ya esta relacionado con una nota");
                        }

                        List<Categoria> auxiliar = new List<Categoria>();
                        foreach (var i in articulo.Categoria)
                        {
                            Categoria c = new Categoria();
                            c = i;
                            auxiliar.Add(c);
                        }


                        foreach (var i in auxiliar)
                        {
                            articulo.Categoria.Remove(i);
                            esquema.SaveChanges();

                        }



                        esquema.Articulo.Remove(articulo);
                        esquema.SaveChanges();

                        return articulo;

                    }
                    else
                    {
                        throw new MensageException("No se pudo eliminar el articulo");
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


        public List<Lote> ListarLotes(long idarticulo)
        {
            using (var esquema = GetEsquema())
            {

                try
                {
                    var lotes = (from x in esquema.Lote
                                 where x.IdArticulo == idarticulo
                                 select x).ToList();

                    return lotes;

                }
                catch (Exception ex)
                {
                    throw new MensageException("Error no se puedo obtener la lista de categorias");
                }



            }
        }

    }
}
