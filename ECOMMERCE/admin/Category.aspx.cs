using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ECOMMERCE.admin
{
    public partial class Category : System.Web.UI.Page
    {
        SqlConnection conn;
        private SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["breadCumbTitle"] = "Manage Category";
            Session["breadCumbPage"] = "Category";
            lbMsg.Visible = false;
            getCategories();
        }
        void getCategories()
        {
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("category_crud", con);
            cmd.Parameters.AddWithValue("@Action", "GETALL");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rCategory.DataSource = dt;
            rCategory.DataBind();
        }
        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty, fileExtension = string.Empty;
            bool isValidToExcute = false;
            int categoryId = Convert.ToInt32(hfcategoryId.Value);
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("category_crud", con);
            cmd.Parameters.AddWithValue("@Action", categoryId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@categoryID", categoryId);
            cmd.Parameters.AddWithValue("@categoryNAME", txtCategoryName.Text.Trim());
            cmd.Parameters.AddWithValue("@ISACTIVE", cbIsActive.Checked);
            if (fuCategoryImage.HasFile)
            {
                if (Utils.isValidExtension(fuCategoryImage.FileName))
                {
                    string newImageName = Utils.getUniqueId();
                    fileExtension = Path.GetExtension(fuCategoryImage.FileName);
                    imagePath = "Images/Category/" + newImageName.ToString() + fileExtension;
                    fuCategoryImage.PostedFile.SaveAs(Server.MapPath("~/Images/Category/") + newImageName.ToString() + fileExtension);
                    cmd.Parameters.AddWithValue("@categoryIMAGEURL", imagePath);
                    isValidToExcute |= true;
                }
                else
                {
                    lbMsg.Visible = false;
                    lbMsg.Text = "Please select .jpg, .jpeg or.png image";
                    lbMsg.CssClass = "alert alert-danger";
                    isValidToExcute = false;

                }
            }
            else
            {
                lbMsg.Visible = true;
            }
            if (isValidToExcute)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    actionName = categoryId == 0 ? " inserted " : " updated ";
                    lbMsg.Visible = true;
                    lbMsg.Text = "Category" + actionName + "successfully!";
                    lbMsg.CssClass = "alert alert-success";
                    getCategories();
                    clear();
                }
                catch (Exception ex)
                {
                    lbMsg.Visible = true;
                    lbMsg.Text = "Error~" + ex.Message;
                    lbMsg.CssClass = "alert alert-danger";
                }
                finally
                {
                    con.Close();
                }

            }
        }



        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();

        }
        void clear()
        {
            txtCategoryName.Text = string.Empty;
            cbIsActive.Checked = false;
            hfcategoryId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            imagePreview.ImageUrl = string.Empty;
        }
        protected void rCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lbMsg.Visible = false;
            if (e.CommandName == "edit")
            {
                con = new SqlConnection(Utils.getConnection());
                cmd = new SqlCommand("category_crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@CategoryID", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                txtCategoryName.Text = dt.Rows[0]["CategoryNAME"].ToString();
                cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["ISACTIVE"]);
                imagePreview.ImageUrl = string.IsNullOrEmpty(dt.Rows[0]["CategoryIMAGEURL"].ToString()) ? "../Images/No_image.png" : "../" + dt.Rows[0]["CategoryIMAGEURL"].ToString();
                imagePreview.Height = 200;
                imagePreview.Width = 200;
                hfcategoryId.Value = dt.Rows[0]["CategoryID"].ToString();
                btnAddOrUpdate.Text = "Update";
            }
            else if (e.CommandName == "delete")
            {
                con = new SqlConnection(Utils.getConnection());
                cmd = new SqlCommand("category_crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@CategoryID", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lbMsg.Visible = true;
                    lbMsg.Text = "Category Deleted successfully!";
                    lbMsg.CssClass = "alert alert-success";
                    getCategories();
                }
                catch (Exception ex)
                {
                    lbMsg.Visible = true;
                    lbMsg.Text = "Error~" + ex.Message;
                    lbMsg.CssClass = "alert alert-danger";
                }
                finally
                {
                    con.Close();
                }

            }

        }
    }
}
    
