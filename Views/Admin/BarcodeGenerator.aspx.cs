using System;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using ZXing.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;

namespace SMSWEBAPP.Views.Admin
{
    public partial class BarcodeGenerator : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            string prefix = txtPrefix.Text;
            string suffix = txtSuffix.Text;
            int start, end;

            if (int.TryParse(txtStart.Text, out start) && int.TryParse(txtEnd.Text, out end))
            {
                phBarcodes.Controls.Clear();

                Panel pnlRow = new Panel();
                pnlRow.CssClass = "d-flex flex-wrap";
                int barcodeCount = 0;

                for (int i = start; i <= end; i++)
                {
                    string barcodeValue = prefix + i.ToString() + suffix;
                    BarcodeWriter writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_128, Options = new EncodingOptions { Width = 150, Height = 60, PureBarcode = false } };
                    System.Drawing.Bitmap barcodeBitmap = writer.Write(barcodeValue);

                    Panel pnlBarcode = new Panel();
                    pnlBarcode.CssClass = "mr-2 mb-2 text-center";

                    System.Web.UI.WebControls.Image imgBarcode = new System.Web.UI.WebControls.Image();
                    imgBarcode.ImageUrl = "data:image/png;base64," + ToBase64(barcodeBitmap);
                    pnlBarcode.Controls.Add(imgBarcode);

                    pnlRow.Controls.Add(pnlBarcode);
                    barcodeCount++;

                    if (barcodeCount % 5 == 0)
                    {
                        phBarcodes.Controls.Add(pnlRow);
                        pnlRow = new Panel();
                        pnlRow.CssClass = "d-flex flex-wrap";
                    }
                }

                if (barcodeCount % 5 != 0)
                {
                    phBarcodes.Controls.Add(pnlRow);
                }

                Session["Prefix"] = prefix;
                Session["Suffix"] = suffix;
                Session["Start"] = start;
                Session["End"] = end;
            }
            else
            {
                // Handle invalid input
            }
        }

        protected void btnExportToPdf_Click(object sender, EventArgs e)
        {
            string prefix = Session["Prefix"].ToString();
            string suffix = Session["Suffix"].ToString();
            int start = Convert.ToInt32(Session["Start"]);
            int end = Convert.ToInt32(Session["End"]);

            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;

            int cellCount = 0;
            int rowCount = 0;

            for (int i = start; i <= end; i++)
            {
                string barcodeValue = prefix + i.ToString() + suffix;
                BarcodeWriter writerBarcode = new BarcodeWriter() { Format = BarcodeFormat.CODE_128, Options = new EncodingOptions { Width = 150, Height = 60, PureBarcode = false } };
                System.Drawing.Bitmap barcodeBitmap = writerBarcode.Write(barcodeValue);

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(barcodeBitmap, System.Drawing.Imaging.ImageFormat.Png);
                img.ScaleToFit(120, 80);

                PdfPCell cell = new PdfPCell();
                cell.AddElement(img);
                cell.Border = Rectangle.BOX;
                cell.BorderWidth = 0.5f;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cellCount++;

                if (cellCount % 5 == 0)
                {
                    table.CompleteRow();
                    rowCount++;
                    if (rowCount % 1 == 0)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            PdfPCell emptyCell = new PdfPCell();
                            emptyCell.Border = Rectangle.NO_BORDER;
                            emptyCell.FixedHeight = 20;
                            table.AddCell(emptyCell);
                        }
                        table.CompleteRow();
                    }
                }
            }

            // Add empty cells to complete the last row
            while (cellCount % 5 != 0)
            {
                PdfPCell cell = new PdfPCell();
                cell.Border = Rectangle.BOX;
                table.AddCell(cell);
                cellCount++;
            }

            table.CompleteRow();
            document.Add(table);
            document.Close();
            byte[] byteInfo = ms.ToArray();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=barcodes.pdf");
            Response.BinaryWrite(byteInfo);
            Response.Flush();
            Response.End();
        }
        private string ToBase64(System.Drawing.Bitmap bitmap)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}