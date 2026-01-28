using SMSWEBAPP.DAL;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SMSWEBAPP.Views.Admin
{
    public partial class StudentAttendanceReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAttendanceData();
            }
        }

        private void LoadAttendanceData()
        {

            using (SqlConnection conn = new SqlConnection(AppConnection.GetConnectionString()))
            {
                conn.Open();

                string colsQuery = @"
                    DECLARE @cols AS NVARCHAR(MAX), @query AS NVARCHAR(MAX);
                    SELECT @cols = STRING_AGG(QUOTENAME(ConcatLabel), ',') 
                                   WITHIN GROUP (ORDER BY AttendanceDate)
                    FROM (
                        SELECT DISTINCT 
                            AttendanceDate, 
                            ConcatLabel = CONVERT(varchar, AttendanceDate, 23) + ' (' + AttendanceWeek + ')'
                        FROM dbo.StudentAttendance
                        WHERE AttendanceDate IS NOT NULL AND AttendanceWeek IS NOT NULL
                    ) AS LabelTable;

                    SET @query = '
                    SELECT StudentBarcode, ' + @cols + '
                    FROM 
                    (
                        SELECT 
                            StudentBarcode, 
                            [Label] = CONVERT(varchar, AttendanceDate, 23) + '' ('' + AttendanceWeek + '')'',
                            Status
                        FROM dbo.StudentAttendance
                        WHERE AttendanceWeek IS NOT NULL
                    ) AS SourceTable
                    PIVOT
                    (
                        MAX(Status)
                        FOR [Label] IN (' + @cols + ')
                    ) AS PivotTable
                    ORDER BY StudentBarcode';

                    EXEC sp_executesql @query;
                ";

                SqlDataAdapter da = new SqlDataAdapter(colsQuery, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridViewAttendance.DataSource = dt;
                GridViewAttendance.DataBind();
            }
        }
    }
}
