using Microsoft.AspNetCore.Mvc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using NuGet.Protocol.Plugins;
using System.Data.Common;
using Humanizer;
using System;
using DFXScheme.Repository;
using Microsoft.ApplicationBlocks.Data;
using System.Web.Helpers;
using System.Configuration;
using Newtonsoft.Json;
using NuGet.DependencyResolver;

namespace DFXScheme.Controllers
{
    public class SchemeDefination : Controller
    {
        private readonly ILogger<SchemeDefination> _logger;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        DataSet ds = new DataSet();
        string sheetName="";
        SqlConnection connection = new SqlConnection("Data Source = 172.16.0.149; Initial Catalog = DFXScheme; User ID = canopus; Password = India@123");
        public SchemeDefination(ILogger<SchemeDefination> logger, IWebHostEnvironment _environment, IConfiguration _configuration)
        {
            _logger = logger;
            Environment = _environment;
            Configuration = _configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SchemeCreation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SchemeCreation(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                conString = this.Configuration.GetConnectionString("DBConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        #region TruncateRawtablebeforeupload
                        //bool abc = Truncate_Rawtable();
                        #endregion
                        #region InsertRawData
                        sqlBulkCopy.DestinationTableName = "dbo.RAW_DFX_SCHEME_DEFN";

                        sqlBulkCopy.ColumnMappings.Add("Business", "Business");
                        sqlBulkCopy.ColumnMappings.Add("SchemeType", "Scheme_Type");
                        sqlBulkCopy.ColumnMappings.Add("Scheme name", "Scheme_Name");
                        sqlBulkCopy.ColumnMappings.Add("CHANNEL", "Channel");
                        sqlBulkCopy.ColumnMappings.Add("Scheme Start Date", "Scheme_StartDate");
                        sqlBulkCopy.ColumnMappings.Add("Scheme End Date", "Scheme_EndDate");
                        sqlBulkCopy.ColumnMappings.Add("Customer", "Customer");
                        if (sheetName != "B2BValueCustomer$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("ZONE", "Zone");
                            sqlBulkCopy.ColumnMappings.Add("STATE", "State");
                            sqlBulkCopy.ColumnMappings.Add("Product Category", "Product_Category");
                        }
                        sqlBulkCopy.ColumnMappings.Add("Product Grade", "Product_Grade");
                        if (sheetName != "B2BValueCustomer$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("Product Hirerarchy", "Product_Hirerarchy");
                        }
                        if (sheetName == "B2CVolumeDD$" || sheetName == "B2CVolumeDI$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("Size", "Size");
                        }
                        if (sheetName != "B2BValueCustomer$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("Multiple Of", "MultipleOf");
                        }
                        sqlBulkCopy.ColumnMappings.Add("Target From", "Target_From");
                        sqlBulkCopy.ColumnMappings.Add("Target To", "Target_To");
                        sqlBulkCopy.ColumnMappings.Add("Payout %", "Payout_Perc");
                        if (sheetName == "B2CVolumeDD$" || sheetName == "B2CValueDD$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("Payout Volume", "Payout_Volume");
                        }
                        if (sheetName != "B2BValueCustomer$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("Payout Value", "Payout_Value");
                        }
                        sqlBulkCopy.ColumnMappings.Add("Payout Start Date", "Payout_StartDate");
                        sqlBulkCopy.ColumnMappings.Add("Payout End Date", "Payout_EndDate");
                        if (sheetName == "B2CVolumeDD$" || sheetName == "B2CValueDD$")
                        {
                            sqlBulkCopy.ColumnMappings.Add("Pending Enable", "Pending_Enable");
                            sqlBulkCopy.ColumnMappings.Add("Get SKU", "Get_SKU");
                        }
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                        #endregion
                        #region ValidatonCheck
                        DataTable validation_datatable=new DataTable();
                            string query = "exec P_Scheme_validate";
                            SqlConnection conn = new SqlConnection(conString);
                            SqlCommand cmd = new SqlCommand(query, conn);
                            conn.Open();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(validation_datatable);
                            conn.Close();
                            da.Dispose();

                        #endregion

                        #region InsertDataIntoDFXTable
                        //if (validation_datatable.Rows.Count>0)
                        //{


                        //}
                        //else
                        //{
                        //    sqlBulkCopy.DestinationTableName = "dbo.DFX_SCHEME_DEFN";

                        //    sqlBulkCopy.ColumnMappings.Add("Business", "Business");
                        //    sqlBulkCopy.ColumnMappings.Add("SchemeType", "Scheme_Type");
                        //    sqlBulkCopy.ColumnMappings.Add("Scheme name", "Scheme_Name");
                        //    sqlBulkCopy.ColumnMappings.Add("CHANNEL", "Channel");
                        //    sqlBulkCopy.ColumnMappings.Add("Scheme Start Date", "Scheme_StartDate");
                        //    sqlBulkCopy.ColumnMappings.Add("Scheme End Date", "Scheme_EndDate");
                        //    sqlBulkCopy.ColumnMappings.Add("Customer", "Customer");
                        //    if (sheetName != "B2BValueCustomer$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("ZONE", "Zone");
                        //        sqlBulkCopy.ColumnMappings.Add("STATE", "State");
                        //        sqlBulkCopy.ColumnMappings.Add("Product Category", "Product_Category");
                        //    }
                        //    sqlBulkCopy.ColumnMappings.Add("Product Grade", "Product_Grade");
                        //    if (sheetName != "B2BValueCustomer$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("Product Hirerarchy", "Product_Hirerarchy");
                        //    }
                        //    if (sheetName == "B2CVolumeDD$" || sheetName == "B2CVolumeDI$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("Size", "Size");
                        //    }
                        //    if (sheetName != "B2BValueCustomer$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("Multiple Of", "MultipleOf");
                        //    }
                        //    sqlBulkCopy.ColumnMappings.Add("Target From", "Target_From");
                        //    sqlBulkCopy.ColumnMappings.Add("Target To", "Target_To");
                        //    sqlBulkCopy.ColumnMappings.Add("Payout %", "Payout_Perc");
                        //    if (sheetName == "B2CVolumeDD$" || sheetName == "B2CValueDD$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("Payout Volume", "Payout_Volume");
                        //    }
                        //    if (sheetName != "B2BValueCustomer$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("Payout Value", "Payout_Value");
                        //    }
                        //    sqlBulkCopy.ColumnMappings.Add("Payout Start Date", "Payout_StartDate");
                        //    sqlBulkCopy.ColumnMappings.Add("Payout End Date", "Payout_EndDate");
                        //    if (sheetName == "B2CVolumeDD$" || sheetName == "B2CValueDD$")
                        //    {
                        //        sqlBulkCopy.ColumnMappings.Add("Pending Enable", "Pending_Enable");
                        //        sqlBulkCopy.ColumnMappings.Add("Get SKU", "Get_SKU");
                        //    }
                        //    con.Open();
                        //    sqlBulkCopy.WriteToServer(dt);
                        //    con.Close();
                            
                        //}
                        #endregion

                    }

                }
                
            }

            return View();
        }

        [HttpPost]
        public string GetRawData()
        {
            var list = Getallrawdata();
            var result = JsonConvert.SerializeObject(new { data = list });
            return result;
        }

        private object Getallrawdata()
        {
            string conString = this.Configuration.GetConnectionString("ExcelConString");
            conString = this.Configuration.GetConnectionString("DBConnection");
            DataTable validation_datatable = new DataTable();
            string query = "Select Business from RAW_DFX_SCHEME_DEFN";
            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(validation_datatable);
            return ds;
        }

        private bool Truncate_Rawtable()
        {
            SqlConnection connection = new SqlConnection("Data Source = 172.16.0.149; Initial Catalog = DFXScheme; User ID = canopus; Password = India@123");    
            string query = "TRUNCATE TABLE RAW_DFX_SCHEME_DEFN";
            SqlCommand cmdtruncate = new SqlCommand(query, connection);
            int exec=cmdtruncate.ExecuteNonQuery();
            if(exec>0)
            {
                return true;
            }
            else
            { 
                return false; 
            }
        }

    }

}
