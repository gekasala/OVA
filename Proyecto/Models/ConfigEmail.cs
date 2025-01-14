﻿using Proyecto.Funciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Proyecto.Models
{
    public class ConfigEmail
    {
        public int idConfigEmail { get; set; }
        public string nombre { get; set; }
        public string servidor { get; set; }
        public string usuario { get; set; }
        public string clave { get; set; }
        public int puerto { get; set; }
        public bool ssl { get; set; }

        private Conexion conexion;
        private SqlConnectionStringBuilder con;
        private List<SqlParameter> parametros;

        /// <summary>
        /// Método que permite consultar configuracion de correo electronico en la base de datos.
        /// </summary>
        /// <returns>retorna modelo con la configuracion de email</returns>
        public ConfigEmail ConsultaConfiguracion()
        {
            ConfigEmail conf = new ConfigEmail();
            try
            {
                DataSet dconfiguracion = new DataSet();
                DataTable dtconfiguracion = new DataTable();
                conexion = new Conexion();
                con = new SqlConnectionStringBuilder();
                con = conexion.ConexionSQLServer();
                ConSqlServer server = new ConSqlServer(con);
                parametros = new List<SqlParameter>();
                server.ejecutarQuery(@"SELECT TOP 1 * FROM configEmail", parametros, out dconfiguracion);
                server.close();
                if (dconfiguracion != null && dconfiguracion.Tables[0].Rows.Count > 0)
                {
                    dtconfiguracion = dconfiguracion.Tables[0];
                    conf = dtconfiguracion.Rows.Cast<DataRow>().Select(r => new ConfigEmail
                    {
                        idConfigEmail = r.Field<int>("idConfigEmail"),
                        nombre = r.Field<string>("nombre"),
                        servidor = r.Field<string>("servidor"),
                        usuario = r.Field<string>("usuario"),
                        clave = r.Field<string>("clave"),
                        puerto = r.Field<int>("puerto"),
                        ssl = r.Field<bool>("ssl"),
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Funcion.tareas.Add("Error [mensaje: " + ex.Message + "]");
                Funcion.write();
            }
            return conf;
        }

        /// <summary>
        /// Método que peromite guiardar o actualizar configuracion de email.
        /// </summary>
        /// <param name="configuracion">Argumento configuracion, modelo de datos ConfigEmail.</param>
        /// <returns>Retorna modelo con la configuracion de email</returns>
        public ConfigEmail gestioanarConfiguracion(ConfigEmail configuracion, string userName)
        {
            ConfigEmail conf = new ConfigEmail();
            try
            {
                DataSet dconfiguracion = new DataSet();
                DataTable dtconfiguracion = new DataTable();
                conexion = new Conexion();
                con = new SqlConnectionStringBuilder();
                con = conexion.ConexionSQLServer();
                ConSqlServer server = new ConSqlServer(con);
                parametros = new List<SqlParameter>();
                parametros.Add(new SqlParameter("@idConfigEmail", configuracion.idConfigEmail));
                parametros.Add(new SqlParameter("@nombre", configuracion.nombre));
                parametros.Add(new SqlParameter("@servidor", configuracion.servidor));
                parametros.Add(new SqlParameter("@usuario", configuracion.usuario.ToLower()));
                parametros.Add(new SqlParameter("@clave", Funcion.stringBase64(configuracion.clave)));
                parametros.Add(new SqlParameter("@puerto", configuracion.puerto));
                parametros.Add(new SqlParameter("@ssl", configuracion.ssl));
                server.ejecutarQuery(@"IF EXISTS (SELECT * FROM configEmail WHERE idConfigEmail=@idConfigEmail) 
                                        BEGIN
	                                        UPDATE configEmail
		                                    SET nombre = @nombre,
		                                    servidor = @servidor,
		                                    usuario = @usuario,
		                                    clave = @clave,
		                                    puerto = @puerto,
		                                    ssl = @ssl
		                                    WHERE idConfigEmail=@idConfigEmail
                                            INSERT INTO Auditoria (tabla,registro,fecha,userName) VALUES ('configEmail','Actualizar',getdate()," + userName + @")
                                        END
                                        ELSE
                                        BEGIN
                                            INSERT INTO configEmail (nombre,servidor,usuario,clave,puerto,ssl) VALUES (@nombre,@servidor,@usuario,@clave,@puerto,@ssl)
                                            INSERT INTO Auditoria (tabla,registro,fecha,userName) VALUES ('configEmail','Creacion',getdate()," + userName + @")
                                        END 
                                            SELECT TOP 1 * FROM configEmail", parametros, out dconfiguracion);
                server.close();
                if (dconfiguracion != null && dconfiguracion.Tables[0].Rows.Count > 0)
                {
                    dtconfiguracion = dconfiguracion.Tables[0];
                    conf = dtconfiguracion.Rows.Cast<DataRow>().Select(r => new ConfigEmail
                    {
                        idConfigEmail = r.Field<int>("idConfigEmail"),
                        nombre = r.Field<string>("nombre"),
                        servidor = r.Field<string>("servidor"),
                        usuario = r.Field<string>("usuario"),
                        clave = r.Field<string>("clave"),
                        puerto = r.Field<int>("puerto"),
                        ssl = r.Field<bool>("ssl"),
                    }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Funcion.tareas.Add("Error [mensaje: " + ex.Message + "]");
                Funcion.write();
            }
            return conf;
        }

        /// <summary>
        /// Método que envia correo deprueba.
        /// </summary>
        /// <returns>Retorna repueta true o false si se envia o no prueba de correo</returns>
        public bool envioPrueba()
        {

            bool envio = false;
            DataSet dconfiguracion = new DataSet();
            DataTable dtconfiguracion = new DataTable();
            conexion = new Conexion();
            con = new SqlConnectionStringBuilder();
            con = conexion.ConexionSQLServer();
            ConSqlServer server = new ConSqlServer(con);
            parametros = new List<SqlParameter>();
            server.ejecutarQuery(@"SELECT TOP 1 * FROM configEmail", parametros, out dconfiguracion);
            server.close();
            if (dconfiguracion != null && dconfiguracion.Tables[0].Rows.Count > 0)
            {
                dtconfiguracion = dconfiguracion.Tables[0];
                EnvioCorreo em = null;
                string ErrorEm = "";
                em = new EnvioCorreo("Correo de prueba", "<body><p align = 'justify'>este es un correo de prueba <br/> Atentamente,<br/><b/><br/><br/><br/><br/><br/></p></body>");
                envio =  em.envio(out ErrorEm, dtconfiguracion.Rows[0].Field<string>("usuario"), true);
                
            }            
            return envio;
        }
    }
}