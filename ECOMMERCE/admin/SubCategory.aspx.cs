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
    public partial class SubCategory : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["breadCumbTitle"] = "Manage Sub-Category";
            Session["breadCumbPage"] = "Sub-Category";
            if (!IsPostBack)
            {
                getCategories();
                getSubCategories();
            }
            lbMsg.Visible = false;

        }
        void getCategories()
        {
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("Category_crud", con);
            cmd.Parameters.AddWithValue("@Action", "GETALL");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            ddlCategory.DataSource = dt;
            ddlCategory.DataTextField = "CategoryName";
            ddlCategory.DataValueField = "CategoryId";
            ddlCategory.DataBind();
        }

        void getSubCategories()
        {
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("SubCategory_crud", con);
            cmd.Parameters.AddWithValue("@Action", "GETALL");
            cmd.CommandType = CommandType.StoredProcedure;
            sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            rSubCategory.DataSource = dt;
            rSubCategory.DataBind();
        }
        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty;
            int subCategoryId = Convert.ToInt32(hfSubCategoryId.Value);
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("Subcategory_crud", con);
            cmd.Parameters.AddWithValue("@Action", subCategoryId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@SubCategoryID", subCategoryId);
            cmd.Parameters.AddWithValue("@SubCategoryNAME", txtSubCategoryName.Text.Trim());
            cmd.Parameters.AddWithValue("@CategoryID",Convert.ToInt32(ddlCategory.SelectedValue));
            cmd.Parameters.AddWithValue("@ISACTIVE", cbIsActive.Checked);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                actionName = subCategoryId == 0 ? " inserted " : " updated ";
                lbMsg.Visible = true;
                lbMsg.Text = "Sub-Category" + actionName + "successfully!";
                lbMsg.CssClass = "alert alert-success";
                getSubCategories();
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


        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();

        }
        void clear()
        {
            txtSubCategoryName.Text = string.Empty;
            cbIsActive.Checked = false;
            hfSubCategoryId.Value = "0";
            btnAddOrUpdate.Text = "Add";
            ddlCategory.ClearSelection();
        }
        protected void rSubCategory_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lbMsg.Visible = false;
            if (e.CommandName == "edit")
            {
                con = new SqlConnection(Utils.getConnection());
                cmd = new SqlCommand("SubCategory_crud", con);
                cmd.Parameters.AddWithValue("@Action", "GETBYID");
                cmd.Parameters.AddWithValue("@SubCategoryID", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                sda = new SqlDataAdapter(cmd);
                dt = new DataTable();
                sda.Fill(dt);
                txtSubCategoryName.Text = dt.Rows[0]["SubCategoryNAME"].ToString();
                cbIsActive.Checked = Convert.ToBoolean(dt.Rows[0]["ISACTIVE"]);
                ddlCategory.SelectedValue = dt.Rows[0]["CategoryID"].ToString();
                hfSubCategoryId.Value = dt.Rows[0]["CategoryID"].ToString();
                btnAddOrUpdate.Text = "Update";
            }
            else if (e.CommandName == "delete")
            {
                con = new SqlConnection(Utils.getConnection());
                cmd = new SqlCommand("SubCategory_crud", con);
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@SubCategoryID", e.CommandArgument);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    lbMsg.Visible = true;
                    lbMsg.Text = "Sub-Category Deleted successfully!";
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