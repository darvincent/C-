using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SLS
{
    class SQLConnectionPool
    {
        private string dataSource;
        private string userID;
        private string password;
        private string initialCatalog;
        private int connectTimeout;
        
        private int minConnectionNo;
        private int maxConnectionNo;
        private List<SqlConnection> freeConnections;
        private List<SqlConnection> allConnections;


        public SQLConnectionPool(int MinConNo, int MaxConNo, INIClass configFile)
        {
            this.minConnectionNo = MinConNo;
            this.maxConnectionNo = MaxConNo;

            this.dataSource = configFile.Read("DATABASE", "ip");
            this.userID = configFile.Read("DATABASE", "user");
            this.password = configFile.Read("DATABASE", "password");
            this.initialCatalog = configFile.Read("DATABASE", "db");
            this.connectTimeout = 30;
        }


        public void Inital()
        {
            freeConnections = new List<SqlConnection>();
            allConnections = new List<SqlConnection>();
            for (int i = 0; i < this.minConnectionNo; i++)
            {
                try
                {
                    SqlConnection con = newConnection();
                    freeConnections.Add(con);
                    allConnections.Add(con);
                }
                catch (Exception e)
                {
                    Server.logger.Error(e);
                    break;
                }
            }
            Server.logger.Info("Inital Connection pool Done!");
        }

        public SqlConnection GetConnction()
        {
            SqlConnection con = null;
            bool result = true;
            if (freeConnections.Count > 0)
            {
                con = freeConnections[0];
                freeConnections.RemoveAt(0);
            }
            else if (allConnections.Count < maxConnectionNo)
            {
                try
                {
                    con = newConnection();
                    allConnections.Add(con);
                }
                catch (Exception e)
                {
                    Server.logger.Error(e);
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            if (result)
            {
                Server.logger.Info("One connection was applied for from Connection pool! Remain "
                        + freeConnections.Count + " connections");
            }
            return con;
        }

        public void releaseConnection(SqlConnection con)
        {
            if (con != null)
            {
                freeConnections.Add(con);
                Server.logger.Info("One connection was released to Connection pool! Remain "
                        + freeConnections.Count + " connections");
            }
        }

        public void releaseAllConnections() {
		for (int i = 0; i < allConnections.Count; i++) {
			try {
                allConnections[i].Close();
			} catch (Exception e) {
				Server.logger.Info("Close one SQL server connection failed");
				Server.logger.Error(e);
			}
		}
	}

        public SqlConnection newConnection()
        {
            try
            {
                StringBuilder sb = new StringBuilder("Data Source=");
                sb.Append(dataSource).Append(";user id =").Append(userID).Append(";password=").Append(password);
                sb.Append(";Initial Catalog=").Append(initialCatalog).Append(";Connect Timeout=").Append(connectTimeout);
                SqlConnection sqlConnection = new SqlConnection(sb.ToString());
                sqlConnection.Open();
                return sqlConnection;
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
                return null;
            }
        }

    }
}
