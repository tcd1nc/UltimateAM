using System;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Data;

namespace AssetManager.ViewModels
{
    public class Photos :  ViewModelBase
    {
        public Photos() 
        {}

        //public Photos(int AssetID)
        //{
        //    int i =0;

        //    using (OleDbConnection CONN = new OleDbConnection(ConfigFileClass.DatabaseConnectionString))
        //    {
        //        CONN.Open();
        //        OleDbCommand oc = new OleDbCommand("SELECT AssetID, PhotoFileName FROM Photos WHERE AssetID =" + AssetID, CONN);
        //        OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //        while (or.Read())
        //        {                    
        //            this.Add(new Photo
        //            {
        //                ID = AssetID, 
        //                PhotoFileName = (or["PhotoFileName"] is DBNull) ? "" : or["PhotoFileName"].ToString() ,
        //                Index = i
        //            });
        //            i++;
        //        }
        //        or.Close();
              
        //    }
        //}        
    }

    

}
