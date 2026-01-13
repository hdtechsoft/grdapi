using System;
using Microsoft.Data.SqlClient;
using grdApi.Data;
using grdApi.Models;
using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace grdApi.Models
{
     public class DataAccess
    {
        public MerchDataResponse fnGetMerchData(  MerchDataRequest request, string connstr)
        {
            
            MerchDataResponse _out= new MerchDataResponse();
                _out.error_code = "form data access page";

            try {
                 using (SqlConnection con = new SqlConnection(connstr)) {
                    using (SqlCommand cmd = new SqlCommand("sp_Add_contact", con)) {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = request.Category;
                    cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = request.Category;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _out.error_desc=ex.Message.ToString();
            }
                
            return _out;
        }  

        public clsoAttendance fnGetAttendanceDtl(  clsiAttendanceDtl request, string connstr)
        {            
            clsoAttendance _out= new clsoAttendance();
             
            _out.error_code = "";
            _out.error_desc = "";
            try {

                using var con = new SqlConnection(connstr);
                 
                using var cmd = new SqlCommand("dbo.usp_get_stud_attendance", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                cmd.Parameters.Add("@course_id", SqlDbType.Int).Value = request.course_id;
                cmd.Parameters.Add("@year_no", SqlDbType.Int).Value = request.year_no;

                con.Open();

                using var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                
                // Optional: cache ordinals for performance

                int ordSno = -1;
                string ordRoll = string.Empty;
                string ordName = string.Empty;
                bool ordinalsInit = false;


                while (reader.Read())
                {
                     if (!ordinalsInit)
                    {
                        ordSno = reader.GetOrdinal("sno");
                        ordRoll = Convert.ToString(reader.GetOrdinal("roll_no"));
                        ordName = Convert.ToString(reader.GetOrdinal("stud_name"));
                        ordinalsInit = true;
                    }
                    
                    var row = new clsoAttendanceDtl
                    {
                        // sno = reader.IsDBNull(ordSno) ? 0 : reader.GetInt32(ordSno),
                        // roll_no = reader.IsDBNull(ordRoll) ? null : reader.GetString(ordRoll),
                        // stud_name = reader.IsDBNull(ordName) ? null : reader.GetString(ordName),

                        sno =  Convert.ToInt32(reader.GetInt32("sno")),
                        roll_no = Convert.ToString(reader.GetString("roll_no")),
                        stud_name = Convert.ToString(reader.GetString("stud_name")),
                        course_id = Convert.ToInt32(reader.GetInt32("course_id")),
                        year_no = Convert.ToInt32(reader.GetInt32("year_no")),
                        
                    };

                    _out.AttDtl.Add(row);
 
 
                }
            }
            catch (Exception ex)
            {
                _out.error_code="-1";
                _out.error_desc = ex.Message.ToString();
            }
                
            return _out;
        } 



    }
}
