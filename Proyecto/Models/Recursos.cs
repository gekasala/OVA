﻿using Proyecto.Funciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto.Models
{
    public class Recursos
    {
        //Variables
        public int idRecurso { get; set; }
        public string nombre { get; set; }
        [AllowHtml]
        public string descripcion { get; set; }
        public string url { get; set; }
        public string archivo { get; set; }
        public string imagen { get; set; }
        public bool estado { get; set; }
        public int idUnidad { get; set; }
        public string nomUnidad { get; set; }
        public DateTime fecha { get; set; }
        public DateTime? fechaModifica { get; set; }
        public string userName { get; set; }
        public bool evidencia { get; set; }
        [AllowHtml]
        public string descEvidencia { get; set; }
        public int puntosRecurso { get; set; }
        public bool entregado { get; set; }
        public string tipoRecurso { get; set; }
        public HttpPostedFileBase fileArchivo { get; set; }
        public HttpPostedFileBase fileImagen { get; set; }

        private Conexion conexion;
        private SqlConnectionStringBuilder con;
        private List<SqlParameter> parametros;

        /// <summary>
        /// Método que lista todos los recursos crteados.
        /// </summary>
        /// <returns>lista de modelo recursos</returns>
        public List<Recursos> listarRecursos()
        {
            List<Recursos> recurso = new List<Recursos>();
            try
            {
                DataSet drecurso;
                DataTable dtrecurso;
                conexion = new Conexion();
                con = new SqlConnectionStringBuilder();
                con = conexion.ConexionSQLServer();
                ConSqlServer server = new ConSqlServer(con);
                parametros = new List<SqlParameter>();
                server.ejecutarQuery(@"SELECT R.*,U.nombre nomUnidad FROM Recursos R LEFT JOIN Unidades U ON R.idUnidad=U.idUnidad ORDER BY R.idUnidad,R.idRecurso", parametros, out drecurso);
                server.close();

                if (drecurso != null && drecurso.Tables[0].Rows.Count > 0)
                {
                    dtrecurso = new DataTable();
                    dtrecurso = drecurso.Tables[0];

                    recurso = dtrecurso.AsEnumerable().Select(r => new Recursos()
                    {
                        idRecurso = r.Field<int>("idRecurso"),
                        nombre = r.Field<string>("nombre"),
                        descripcion = r.Field<string>("descripcion"),
                        url = r.Field<string>("url"),
                        archivo = r.Field<string>("archivo"),
                        imagen = r.Field<string>("imagen"),
                        estado = r.Field<bool>("estado"),
                        idUnidad = r.Field<int>("idUnidad"),
                        nomUnidad = r.Field<string>("nomUnidad"),
                        fecha = r.Field<DateTime>("fecha"),
                        userName = r.Field<string>("userName"),
                        evidencia = r.Field<bool>("evidencia"),
                        tipoRecurso = r.Field<string>("tipoRecurso")
                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                Funcion.tareas.Add("Error [mensaje: " + ex.Message + "]");
                Funcion.write();
            }
            return recurso;
        }

        /// <summary>
        /// Método que permite guardar o actualizar recursos.
        /// </summary>
        /// <param name="Precurso">Argumento Precurso, modelo de datos Recursos.</param>
        /// <returns>retorna modelo recurso</returns>
        public Recursos gestionarrecurso(Recursos Precurso, string userName)
        {
            Recursos recurso = new Recursos();
            try
            {
                DataSet drecurso;
                DataTable dtrecurso;
                conexion = new Conexion();
                con = new SqlConnectionStringBuilder();
                con = conexion.ConexionSQLServer();
                ConSqlServer server = new ConSqlServer(con);
                parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@idrecurso", Precurso.idRecurso));
                parametros.Add(new SqlParameter("@nombre", Precurso.nombre));
                parametros.Add(new SqlParameter("@descripcion", String.IsNullOrWhiteSpace(Precurso.descripcion) ? DBNull.Value : (object)Precurso.descripcion));
                parametros.Add(new SqlParameter("@url", String.IsNullOrWhiteSpace(Precurso.url) ? DBNull.Value : (object)Precurso.url));
                parametros.Add(new SqlParameter("@imagen", String.IsNullOrWhiteSpace(Precurso.imagen) ? DBNull.Value : (object)Precurso.imagen));
                parametros.Add(new SqlParameter("@archivo", String.IsNullOrWhiteSpace(Precurso.archivo) ? DBNull.Value : (object)Precurso.archivo));
                parametros.Add(new SqlParameter("@idUnidad", Precurso.idUnidad));
                parametros.Add(new SqlParameter("@estado", Precurso.estado));
                parametros.Add(new SqlParameter("@fecha", Precurso.fecha));
                parametros.Add(new SqlParameter("@fechaModifica", DateTime.Now));
                parametros.Add(new SqlParameter("@userName", Precurso.userName));
                parametros.Add(new SqlParameter("@evidencia", Precurso.evidencia));
                parametros.Add(new SqlParameter("@descEvidencia", String.IsNullOrWhiteSpace(Precurso.descEvidencia) ? DBNull.Value : (object)Precurso.descEvidencia));
                parametros.Add(new SqlParameter("@puntosRecurso", Precurso.puntosRecurso));
                parametros.Add(new SqlParameter("@tipoRecurso", Precurso.tipoRecurso));
                drecurso = new DataSet();
                server.ejecutarQuery(@" IF EXISTS (SELECT * FROM Recursos WHERE idrecurso=@idrecurso) 
                                        BEGIN
                                           UPDATE Recursos
                                               SET nombre = @nombre,
                                                  descripcion = @descripcion,
                                                  url = @url,
                                                  archivo = @archivo,
                                                  imagen = @imagen,
                                                  estado = @estado,
                                                  idUnidad = @idUnidad,
                                                  fecha = @fecha,
                                                  fechaModifica = @fechaModifica,
                                                  userName = @userName,
                                                  evidencia = @evidencia,
                                                  descripcionEvidencia = @descEvidencia,
                                                  puntosRecurso = @puntosRecurso,
                                                  tipoRecurso = @tipoRecurso
                                             WHERE idrecurso=@idrecurso
                                            INSERT INTO Auditoria (tabla,registro,fecha,userName) VALUES ('Recursos','Actualizar recurso' + CAST(@idrecurso as VARCHAR),getdate(),'" + userName + @"')
                                        END
                                        ELSE
                                        BEGIN
                                            INSERT INTO Recursos
                                                   (nombre,descripcion,url,archivo,imagen,estado,idUnidad,fecha,userName,evidencia,descripcionEvidencia,puntosRecurso,tipoRecurso)
                                             VALUES
                                                   (@nombre,@descripcion,@url,@archivo,@imagen,@estado,@idUnidad,@fecha,@userName,@evidencia,@descEvidencia,@puntosRecurso,@tipoRecurso)
                                            INSERT INTO Auditoria (tabla,registro,fecha,userName) VALUES ('Recursos','Crear',getdate(),'" + userName + @"')                                        
                                        END SELECT * FROM Recursos", parametros, out drecurso);
                server.close();

                if (drecurso != null && drecurso.Tables[0].Rows.Count > 0)
                {
                    dtrecurso = new DataTable();
                    dtrecurso = drecurso.Tables[0];
                    recurso = dtrecurso.Rows.Cast<DataRow>().Select(r => new Recursos
                    {
                        idRecurso = r.Field<int>("idRecurso"),
                        nombre = r.Field<string>("nombre"),
                        descripcion = r.Field<string>("descripcion"),
                        url = r.Field<string>("url"),
                        archivo = r.Field<string>("archivo"),
                        imagen = r.Field<string>("imagen"),
                        estado = r.Field<bool>("estado"),
                        idUnidad = r.Field<int>("idUnidad"),
                        fecha = r.Field<DateTime>("fecha"),
                        userName = r.Field<string>("userName"),
                        evidencia = r.Field<bool>("evidencia"),
                        descEvidencia = r.Field<string>("descripcionEvidencia"),
                        puntosRecurso = r.Field<int>("puntosRecurso"),
                        tipoRecurso = r.Field<string>("tipoRecurso")
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Funcion.tareas.Add("Error [mensaje: " + ex.Message + "]");
                Funcion.write();
            }
            return recurso;
        }

        /// <summary>
        /// Método que permite buscar un recurso por id del recurso.
        /// </summary>
        /// <param name="idRecurso">Argumento idRecurso, idRecurso del recurso.</param>
        /// <returns>Retorna modelo recurso</returns>
        public Recursos BuscarRecursos(int idRecurso)
        {
            Recursos recurso = new Recursos();
            try
            {
                DataSet drecurso;
                DataTable dtrecurso;
                conexion = new Conexion();
                con = new SqlConnectionStringBuilder();
                con = conexion.ConexionSQLServer();
                ConSqlServer server = new ConSqlServer(con);
                parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@idrecurso", idRecurso));
                server.ejecutarQuery(@"SELECT * FROM Recursos WHERE idRecurso=@idRecurso", parametros, out drecurso);
                server.close();

                if (drecurso != null && drecurso.Tables[0].Rows.Count > 0)
                {
                    dtrecurso = new DataTable();
                    dtrecurso = drecurso.Tables[0];
                    recurso = dtrecurso.Rows.Cast<DataRow>().Select(r => new Recursos
                    {
                        idRecurso = r.Field<int>("idRecurso"),
                        nombre = r.Field<string>("nombre"),
                        descripcion = r.Field<string>("descripcion"),
                        url = r.Field<string>("url"),
                        archivo = r.Field<string>("archivo"),
                        imagen = r.Field<string>("imagen"),
                        estado = r.Field<bool>("estado"),
                        idUnidad = r.Field<int>("idUnidad"),
                        fecha = r.Field<DateTime>("fecha"),
                        userName = r.Field<string>("userName"),
                        evidencia = r.Field<bool>("evidencia"),
                        descEvidencia = r.Field<string>("descripcionEvidencia"),
                        puntosRecurso = r.Field<int>("puntosRecurso"),
                        tipoRecurso = r.Field<string>("tipoRecurso")
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Funcion.tareas.Add("Error [mensaje: " + ex.Message + "]");
                Funcion.write();
            }
            return recurso;
        }

        /// <summary>
        /// Método que permite cambiar el estado al recurso de Activo a inactivo o viceversa.
        /// </summary>
        /// <param name="idRecurso">Argumento idRecurso, idRecurso del recurso.</param>
        /// <returns>Retorna lista de modelo recursos</returns>
        public List<Recursos> cambiarestadorec(int idRecurso)
        {
            List<Recursos> recurso = new List<Recursos>();
            try
            {
                DataSet drecurso;
                DataTable dtrecurso;
                conexion = new Conexion();
                con = new SqlConnectionStringBuilder();
                con = conexion.ConexionSQLServer();
                ConSqlServer server = new ConSqlServer(con);
                parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@idrecurso", idRecurso));
                server.ejecutarQuery(@"UPDATE Recursos SET estado=CASE WHEN estado=1 THEN 0 ELSE 1 END WHERE idRecurso=@idrecurso
                                        SELECT R.*,U.nombre nomUnidad FROM Recursos R LEFT JOIN Unidades U ON R.idUnidad=U.idUnidad ORDER BY R.idUnidad,R.idRecurso
                                        INSERT INTO Auditoria (tabla,registro,fecha,userName) VALUES 
                                        ('Recursos',(select case when estado=1 then 'Activación ' else 'Desactivación ' end from  Recursos  WHERE idRecurso=@idrecurso) +' de recurso '+ CAST(@idRecurso as VARCHAR),getdate(),
                                        '" + userName + @"')", parametros, out drecurso);
                server.close();

                if (drecurso != null && drecurso.Tables[0].Rows.Count > 0)
                {
                    dtrecurso = new DataTable();
                    dtrecurso = drecurso.Tables[0];

                    recurso = dtrecurso.AsEnumerable().Select(r => new Recursos()
                    {
                        idRecurso = r.Field<int>("idRecurso"),
                        nombre = r.Field<string>("nombre"),
                        descripcion = r.Field<string>("descripcion"),
                        url = r.Field<string>("url"),
                        archivo = r.Field<string>("archivo"),
                        imagen = r.Field<string>("imagen"),
                        estado = r.Field<bool>("estado"),
                        idUnidad = r.Field<int>("idUnidad"),
                        nomUnidad = r.Field<string>("nomUnidad"),
                        fecha = r.Field<DateTime>("fecha"),
                        userName = r.Field<string>("userName"),
                        tipoRecurso = r.Field<string>("tipoRecurso")
                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                Funcion.tareas.Add("Error [mensaje: " + ex.Message + "]");
                Funcion.write();
            }
            return recurso;
        }

        /// <summary>
        /// Método que permite consultar las unidades creadas para llenar combo.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> comUnidades()
        {
            DataTable _unidades = new DataTable();
            conexion = new Conexion();
            con = new SqlConnectionStringBuilder();
            con = conexion.ConexionSQLServer();
            ConSqlServer server = new ConSqlServer(con);
            server.ejecutarQuery(@"select idUnidad,nombre from unidades where estado=1", new List<SqlParameter>(), out _unidades);
            return Combo(_unidades, "nombre", "idUnidad", null);
        }

        /// <summary>
        /// Método que permite consultar las unidades creadas para llenar combo.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> comUnidadesE()
        {
            DataTable _unidades = new DataTable();
            conexion = new Conexion();
            con = new SqlConnectionStringBuilder();
            con = conexion.ConexionSQLServer();
            ConSqlServer server = new ConSqlServer(con);
            server.ejecutarQuery(@"select distinct U.idUnidad,U.nombre from unidades U inner join Recursos R on U.idUnidad=R.idUnidad where U.estado=1 and r.evidencia=1", new List<SqlParameter>(), out _unidades);
            return Combo(_unidades, "nombre", "idUnidad", null);
        }

        /// <summary>
        /// Método que permite consultar recursos creados para llenar combo.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> comRecursos(int id = 0)
        {
            DataTable _recursos = new DataTable();
            conexion = new Conexion();
            con = new SqlConnectionStringBuilder();
            con = conexion.ConexionSQLServer();
            ConSqlServer server = new ConSqlServer(con);
            if (id == 0)
            {
                server.ejecutarQuery(@"select idRecurso,nombre from recursos where estado=1 and evidencia=1", new List<SqlParameter>(), out _recursos);
            }
            else
            {
                server.ejecutarQuery(@"select idRecurso,nombre from recursos where estado=1 and evidencia=1 and idunidad=" + id, new List<SqlParameter>(), out _recursos);
            }
            
            return Combo(_recursos, "nombre", "idRecurso", null);
        }

        /// <summary>
        /// Método que permite consultar usuarios creados para llenar combo.
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> comUsuarios(string id = "0")
        {
            DataTable _usuarios = new DataTable();
            conexion = new Conexion();
            con = new SqlConnectionStringBuilder();
            con = conexion.ConexionSQLServer();
            ConSqlServer server = new ConSqlServer(con);
            if (id == "0")
            {
                server.ejecutarQuery(@"select userName, (nombre + ' ' +apellidos) nombre from usuarios where estado=1 and idRol = 3", new List<SqlParameter>(), out _usuarios);
            }
            else
            {
                server.ejecutarQuery(@"select userName, (nombre + ' ' +apellidos) nombre from usuarios where estado=1 and idRol = 3 and userName=" + id, new List<SqlParameter>(), out _usuarios);
            }

            return Combo(_usuarios, "nombre", "userName", null);
        }

        /// <summary>
        /// Método que permite generar lista para llenar combo.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Combo(DataTable agOrigenDatos, string agDisplay, string agValue, string agSelectedValue)
        {
            return agOrigenDatos.Rows.Cast<DataRow>().Select(r => new SelectListItem
            {
                Text = r[agDisplay].ToString(),
                Value = r[agValue].ToString(),
                Selected = r[agValue].ToString().Equals(agSelectedValue)
            }).ToList();
        }
    }
}