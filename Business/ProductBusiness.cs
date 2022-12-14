using System;
using System.Collections.Generic;
using MySqlConnector;
using Domain;
using Data;

namespace Business
{
    public class ProductBusiness
    {
        public List<Product> Show()
        {
            List<Product> list = new List<Product>();
            AccessData data = new AccessData();
            try
            {
                //Se setea la query para traer los productos 
                data.setQuery("Select * from Products"); //-> Despues cambiar esto por un StoredProcedure
                //string sp = ""; 
                //data.setProcedure(sp);
                data.executeQuery();

                while (data.Reader.Read())
                {
                    //Se cargan los productos de la base // Se deberian verificar nulls? 
                    Product aux = new Product();
                    aux.Id = Convert.ToInt16(data.Reader["ID"]);
                    aux.Name = data.Reader["Name"].ToString();
                    aux.Description = data.Reader["Description"].ToString();
                    aux.Price = Convert.ToDecimal(data.Reader["Price"]);
                    aux.Stock = Convert.ToInt16(data.Reader["Stock"]);
                    aux.IdSize = (int)data.Reader["IdSize"];
                    aux.IdColor = (int)data.Reader["IdColor"];
                    aux.ImageUrl = data.Reader["ImageUrl"].ToString() == "" ? "../Extras/Images/Products/NoImage.png" : data.Reader["ImageUrl"].ToString();
                    aux.IsActive = Convert.ToBoolean(data.Reader["IsActive"]);
                    //Se agrega el registro leído a la lista de productos
                    list.Add(aux);
                }
                //devuelvo listado de productos
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //se cierra la conexión a DB
                data.closeConnection();
            }
        }

        public Product GetProduct(int id)
        {
            AccessData data = new AccessData();
            try
            {
                //Se setea la query para traer los productos 
                data.setQuery("Select * from Products where ID = " + id + ";");
                //string sp = ""; 
                //data.setProcedure(sp);
                data.executeQuery();

                Product aux = new Product();
                while (data.Reader.Read())
                {
                    //Se cargan los productos de la base // Se deberian verificar nulls? 
                    aux.Id = Convert.ToInt16(data.Reader["ID"]);
                    aux.Name = data.Reader["Name"].ToString();
                    aux.Description = data.Reader["Description"].ToString();
                    aux.Price = Convert.ToDecimal(data.Reader["Price"]);
                    aux.Stock = Convert.ToInt16(data.Reader["Stock"]);
                    aux.IdSize = (int)data.Reader["IdSize"];
                    aux.IdColor = (int)data.Reader["IdColor"];
                    aux.ImageUrl = data.Reader["ImageUrl"].ToString();
                    aux.IsActive = Convert.ToBoolean(data.Reader["IsActive"]);
                }

                //devuelvo listado de productos
                return aux;
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {   //se cierra la conexión a DB
                data.closeConnection();
            }
        }
        public void Add(Product newProduct)
        {
            //Se abre la conexión a DB
            AccessData data = new AccessData();

            try
            {   //Se inserta en DB los data cargados 
                data.SetParameter("@Name", newProduct.Name);
                data.SetParameter("@Description", newProduct.Description);
                data.SetParameter("@Price", newProduct.Price);
                data.SetParameter("@Stock", newProduct.Stock);
                data.SetParameter("@IdSize", newProduct.IdSize);
                data.SetParameter("@IdColor", newProduct.IdColor);
                data.SetParameter("@IsActive", newProduct.IsActive);
                data.SetParameter("@ImageUrl", newProduct.ImageUrl);
                data.setQuery("Insert into Products (Name, Description, Price, Stock, IdSize, IdColor, IsActive, ImageUrl) values (@Name, @Description, @Price, @Stock, @IdSize, @IdColor, @IsActive, @ImaegenUrl);");
                data.executeQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //Se abre la conexión a DB
                data.closeConnection();
            }
        }

        public void Modify(Product modProduct)
        {
            //Se abre la conexión a DB
            AccessData data = new AccessData();

            try
            {   //Se inserta en DB los data cargados en la plantilla "modificar"
                data.SetParameter("@Id", modProduct.Id);
                data.SetParameter("@Name", modProduct.Name);
                data.SetParameter("@Description", modProduct.Description);
                data.SetParameter("@Price", modProduct.Price);
                data.SetParameter("@Stock", modProduct.Stock);
                data.SetParameter("@IdSize", modProduct.IdSize);
                data.SetParameter("@IdColor", modProduct.IdColor);
                data.SetParameter("@IsActive", modProduct.IsActive);
                data.SetParameter("@ImageUrl", modProduct.ImageUrl);
                data.setQuery("update Products set Name=@Name, Description=@Description, Price=@Price, Stock=@Stock, IdSize=@IdSize, IdColor=@IdColor, IsActive=@IsActive) WHERE ID = @Id;");
                data.executeQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //Se cierra la conexión a DB
                data.closeConnection();
            }
        }

        public void Delete(int id)
        {
            AccessData data = new AccessData();
            try
            {   //Se elimina el registro
                data.SetParameter("@Id", id);
                data.setQuery("Update from Products set IsActive = 0 where Id=@Id");
                data.executeQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //Se abre la conexión a DB
                data.closeConnection();
            }
        }

        /*public List<Product> Filter(string searchBy, string when, string filter, string state="3")
        {
            List<Product> list = new List<Product>();
            AccessData data = new AccessData();

            string query = "Select * from Products where ";

            try
            {
                //Filtro de precio por separado para controlar números
                if (searchBy == "Precio")
                {
                    switch (when)
                    {
                        case "Mayor o igual que":
                            query += "Price >= " + filter;
                            break;
                        case "Menor o igual que":
                            query += "Price <= " + filter;
                            break;
                        //case "Igual a":
                        //    query += "P.Precio = " + filter;
                        //    break;
                    }
                }

                //filtro simplificado para la busqueda de nombres y/o
                else
                {
                    string column = "";
                    switch (searchBy)
                    {
                        case "Código":
                            column = "Id like '%" + filter + "%'";
                            break;
                        case "Nombre":
                            column = "Name like '%" + filter +"%'";
                            break;
                        case "Color":
                            column = "Color like '%" + filter + "%'";
                            break;
                        case "Talle":
                            column = "Size like '%" + filter + "%'";
                            break;
                    }
                    //switch (searchBy)
                    //{
                    //    case "Igual a":
                    //        query += column + " like '" + filter + "'";
                    //        break;
                    //    case "Contiene":
                    //        query += column + " like '%" + filter + "%'";
                    //        break;
                    //    case "Comienza con":
                    //        query += column + " like '" + filter + "%'";
                    //        break;
                    //    case "Termina con":
                    //        query += column + " like '%" + filter + "'";
                    //        break;
                    //
                    query += column;
                }

                if (state == "Activo")
                {
                    query += " and State= 1";
                }

                else if(state == "Inactivo")
                {
                    query += " and State= 0";
                }

                

                data.setQuery(query);
                data.executeQuery();
                while (data.Reader.Read())
                {
                    //Se cargan los articulos de la base
                    Product aux = new Product();
                    aux.Id = Convert.ToInt16(data.Reader["Id"]);
                    aux.Name = data.Reader["Name"].ToString();
                    aux.Description = data.Reader["Description"].ToString();
                    aux.Price = Convert.ToDecimal(data.Reader["Price"]);
                    aux.Stock = Convert.ToInt16(data.Reader["Stock"]);
                    aux.Size = (int)data.Reader["Size"];
                    aux.Color = data.Reader["Color"].ToString();
                    aux.State = Convert.ToBoolean(data.Reader["State"]);

                    //Se agrega el registro leído a la lista de articulos
                    list.Add(aux);

                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {   //se cierra la conexión a DB
                data.closeConnection();
            }
        }*/
    }
}

