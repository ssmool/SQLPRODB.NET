using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Papers;

namespace ReadBases.SqlDB
{
    public class SqlDB
    {
        private string file = "";
        private string[] bases;

        public SqlDB()
        {
        }

        private string[] OpenConfigureFile()
        {
            ReadPapers r = new ReadPapers();
            string[] request = r.ReadConfigureFile(this.file);
            return request;
        }

        private string[] OpenBase(string provider)
        {
            string[] request = new string[1];
            SqlConnection conn = new SqlConnection(provider);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PROC_SELECT_TABELAS";
            conn.Open();
            SqlDataReader sdr = cmd.ExecuteReader(CommandBehavior.Default);
            string[] temp = new string[1];
            int count = 0;
            try
            {
                while (sdr.HasRows)
                {
                    sdr.Read();
                    for (int tcount = 0; tcount <= count - 1; tcount++)
                        request[tcount] = temp[tcount];
                    request[request.Length - 1] = sdr["tab"].ToString();
                    temp = request;
                    request = new string[request.Length + 1];
                    count++;
                }
            }
            catch (Exception ex)
            {
            }
            return request;
        }

        public void GenerateProcedures(string dir, string provider)
        {
            WritePapers w = new WritePapers("c:\\teste.sql");
            SqlConnection conn;
            SqlCommand cmd;
            SqlCommand cmdexe;
            SqlDataAdapter dap;
            DataSet ds;
            string[] response;
            string sproc = "";
            string swproc = "";
            string tsproc = "";
            string tswproc = "";
            string iproc = "";
            string tiproc = "";
            string uproc = "";
            string tuproc = "";
            string proc = "";
            conn = new SqlConnection(provider);
            ds = new DataSet();
            response = GetTables(provider);
            for (int count = 0; count <= response.Length - 2; count++)
            {
                DataTable dr = new DataTable(response[count].ToString());
                cmd = new SqlCommand("SELECT * FROM " + response[count], conn);
                conn.Open();
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dr);
                ds.Tables.Add(dr);
                conn.Close();
            }
            for (int acount = 0; acount <= (ds.Tables.Count - 1); acount++)
            {
                //CREATE PROCEDURES OF SELECT
                sproc += "\n";
                sproc += "CREATE PROCEDURE [PROC_SELECT_" + response[acount].ToString().Replace(" ", "") + "](";
                sproc += "\n";
                sproc += "AS" + "\n" + "BEGIN";
                sproc += "\n";
                sproc += "SELECT * FROM " + response[acount].ToString().Replace(" ", "");
                sproc += "\n";
                sproc += "END\n);";
                sproc += "\n\n";

                //CREATE PROCEDURES OF SELECT
                swproc += "\n";
                swproc += "CREATE PROCEDURE [PROC_WSELECT_" + response[acount].ToString().Replace(" ", "") + "](";
                swproc += "\n";
                swproc += "@" + ds.Tables[acount].Columns[0].ToString() + 
                    " " + SqlDbType.Int.ToString();
                swproc += "\n";
                swproc += "AS" + "\n" + "BEGIN";
                swproc += "\n";
                swproc += "SELECT * FROM " + response[acount].ToString().Replace(" ", "");
                swproc += " WHERE " + ds.Tables[acount].Columns[0].ToString() + 
                    " = @" + ds.Tables[acount].Columns[0].ToString();
                swproc += "\n";
                swproc += "END\n);";
                swproc += "\n\n";

                //CREATE PROCEDURES OF INSERT
                //ITERACTION FOR PARAMETERS
                iproc += "\n\n";
                iproc += "CREATE PROCEDURE [PROC_INSERT_" + response[acount].ToString().Replace(" ", "") + "](";
                for (int aicount = 1; aicount <= (ds.Tables[acount].Columns.Count - 1);
                    aicount++)
                {
                    iproc += "\n" + "@" + ds.Tables[acount].Columns[aicount].ToString().Replace(" ", "");
                    iproc += " ";
                    string t = ds.Tables[acount].Columns[aicount].DataType.ToString();
                    switch(t)
                    {
                        case "System.Int32":
                            iproc += SqlDbType.Int.ToString();
                            break;
                        case "System.String":
                            iproc += SqlDbType.Char.ToString();
                            break;
                        case "System.DateTime":
                            iproc += SqlDbType.DateTime.ToString();
                            break;
                    }
                    if(aicount != (ds.Tables[acount].Columns.Count - 1))
                        iproc += ",";
                }
                iproc += "\n" + "AS" + "\n" + "BEGIN" + "\n";
                iproc += "INSERT INTO " + response[acount].Replace(" ", "") + "(";
                //ITERACTION FOR COLUMS IN TABLE
                for (int aiccount = 1; aiccount <= (ds.Tables[acount].Columns.Count - 1);
                    aiccount++)
                {
                    iproc += ds.Tables[acount].Columns[aiccount].ToString().Replace(" ", "");
                    if (aiccount != ds.Tables[acount].Columns.Count - 1)
                        iproc += ",";
                }
                iproc += ") VALUES (";
                //ITERACTION FOR PARAMETERS VALUES
                for (int aivcount = 1; aivcount <= (ds.Tables[acount].Columns.Count - 1);
                    aivcount++)
                {
                    iproc += "@" + ds.Tables[acount].Columns[aivcount].ToString().Replace(" ", "");
                    if (aivcount != ds.Tables[acount].Columns.Count - 1)
                        iproc += ",";
                }
                iproc += ")";
                iproc += "\n";
                iproc += "END\n);";
                iproc += "\n\n";

                //CREATE PROCEDURES OF UPDATE
                //ITERACTION FOR PARAMETERS
                uproc += "\n\n";
                uproc += "CREATE PROCEDURE [PROC_UPDATE_" + response[acount].ToString().Replace(" ", "") + "](";
                uproc += "\n";
                uproc += "@" + ds.Tables[acount].Columns[0].ToString() +
                    " " + SqlDbType.Int.ToString() + ",";
                for (int aicount = 1; aicount <= (ds.Tables[acount].Columns.Count - 1);
                    aicount++)
                {
                    uproc += "\n" + "@" + ds.Tables[acount].Columns[aicount].ToString().Replace(" ", "");
                    uproc += " ";
                    string t = ds.Tables[acount].Columns[aicount].DataType.ToString();
                    switch(t)
                    {
                        case "System.Int32":
                            uproc += SqlDbType.Int.ToString();
                            break;
                        case "System.String":
                            uproc += SqlDbType.Char.ToString();
                            break;
                        case "System.DateTime":
                            uproc += SqlDbType.DateTime.ToString();
                            break;
                    }
                    if(aicount != (ds.Tables[acount].Columns.Count - 1))
                        uproc += ",";
                }
                uproc += "\n" + "AS" + "\n" + "BEGIN" + "\n";
                uproc += "UPDATE " + response[acount].Replace(" ", "") + " SET ";
                //ITERACTION FOR COLUMS IN TABLE
                for (int aiccount = 1; aiccount <= (ds.Tables[acount].Columns.Count - 1);
                    aiccount++)
                {
                    uproc += ds.Tables[acount].Columns[aiccount].ToString().Replace(" ", "");
                    uproc += "=";
                    uproc += "@" + ds.Tables[acount].Columns[aiccount].ToString().Replace(" ", "");
                    if (aiccount != ds.Tables[acount].Columns.Count - 1)
                        uproc += ",";
                }
                uproc += " WHERE " + ds.Tables[acount].Columns[0].ToString() +
                    " = @" + ds.Tables[acount].Columns[0].ToString();
                uproc += "\n";
                uproc += "END\n);";
                uproc += "\n\n";
                tsproc += sproc;
                tswproc += swproc;
                tiproc += iproc;
                tuproc += uproc;
                conn.Open();
                cmdexe = new SqlCommand();
                cmdexe.Connection = conn;
                cmdexe.CommandType = CommandType.Text;
                cmdexe.CommandText = sproc;
                try
                {
                    cmdexe.ExecuteNonQuery();
                    cmdexe.CommandText = swproc;
                    cmdexe.ExecuteNonQuery();
                    cmdexe.CommandText = iproc;
                    cmdexe.ExecuteNonQuery();
                    cmdexe.CommandText = uproc;
                    cmdexe.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    WritePapers file = new WritePapers("c:\\ERROR_GENERATE_PROCEDURES.txt");
                    file.Write(ex.InnerException + "\n" + ex.Message + "\n" + ex.Source);
                }
                conn.Close();
                sproc = "";
                swproc = "";
                iproc = "";
                uproc = "";
                
            }
            proc = tsproc + "\n" + tiproc + "\n" + tuproc;
            w.Write(proc);
        }

        public string GetFile()
        {
            return file;
        }

        public string[] GetBases(string file)
        {
            this.file = file;
            string[] request = OpenConfigureFile();
            return request;
        }

        public string[] GetTables(string provider)
        {
            string[] request = OpenBase(provider);
            return request;
        }
    }
}
