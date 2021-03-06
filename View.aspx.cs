﻿using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

public partial class ASP_Project2_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Nickname"] != null)
        {
            recommend_label.Text = Session["Nickname"].ToString();
            idcheck.Value = Session["Nickname"].ToString();
        }
        else
        {
            Response.Write("<script type='text/javascript'>alert('로그인후 이용할 수 있습니다.');location.href=history.back(); </script>");
        }
        String savePath = @"images\";

        if (!IsPostBack)
        {
            MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mariadb"].ConnectionString);
            connection.Open();
            string UpdateQuery = "Update sellpost SET readcount=readcount + 1 where num=" + Request["No"];
            MySqlCommand command = new MySqlCommand(UpdateQuery, connection);
            command.ExecuteNonQuery();
            string SelectQuery = "Select * from sellpost where num = " + Request["No"];
            command = new MySqlCommand(SelectQuery, connection);
            MySqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                nickname.Text = reader["name"].ToString();
                writeday.Text = reader["writedate"].ToString();
                readcount.Text = reader["readcount"].ToString();
                title.Text = reader["title"].ToString();
                Contents.Text = reader["contents"].ToString();
                imageout.ImageUrl = savePath + reader["image"].ToString();
            }
            reader.Close();
            connection.Close();
            //댓글값이 게시글 번호와 포스트 넘버가 일치하게 맞춘뒤 가져오는구문
            string CommandText = "select * from reply where postnumber = " + Request["No"];

            Bind(CommandText);
        }
    }
    protected void back_board_Btn(object sender, EventArgs e)
    {
        Response.Redirect("sellpost.aspx");                                                  // 목록버튼 클릭시

    }
    public void Bind(string CommandTxt)
    {
        try
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["mariadb"].ConnectionString);
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cmd.Connection = conn;
            cmd.CommandText = CommandTxt;
            conn.Open();
            da.Fill(ds, "reply");
            conn.Close();
            reply_grid.DataSource = ds.Tables[0].DefaultView;
            DataBind();
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message.ToString());
        }

    }

    protected void View_Recom_Btn(object sender, EventArgs e)
    {
        MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mariadb"].ConnectionString);
        string getNumber = Request["No"];
        post_num.Value = getNumber;

        string InsertQuery = "Insert Into reply (nickname,comment,writedate,postnumber) Values ( @nickname, @comment,@writedate,@postnumber)";
        MySqlCommand command = new MySqlCommand(InsertQuery, connection);
        command.Parameters.AddWithValue("@nickname", recommend_label.Text);
        command.Parameters.AddWithValue("@comment", reply.Text);
        command.Parameters.AddWithValue("@writedate", DateTime.Now.ToShortDateString());
        command.Parameters.AddWithValue("@postnumber", post_num.Value);
        connection.Open();
        command.ExecuteNonQuery();

        connection.Close();
        Response.Write("<script type='text/javascript'>alert('댓글등록 성공'); </script>");  // 댓글등록 성공을 알림
        Response.Redirect(Request.Url.PathAndQuery);                                       // 페이지 새로고침
    }


    protected void Delete_Btn_Click(object sender, EventArgs e)
    {

        if (Session["super"].ToString() == "관리자")
        {
            Response.Write("<script type='text/javascript'>confirm('게시글을 삭제하시겠습니까?');location.href='sellpost.aspx';</script>");

            MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mariadb"].ConnectionString);

            string Sel_Sel = "select * from sellpost Where num = " + Request["No"];
            string Sel_Rep = "select * from reply Where postnumber = " + Request["No"];
            MySqlCommand command = new MySqlCommand(Sel_Sel, connection);
            MySqlCommand command2 = new MySqlCommand(Sel_Rep, connection);
            connection.Open();
            string Del_Sel = "Delete From sellpost where num = " + Request["No"];
            string Del_Rep = "Delete From reply where postnumber = " + Request["No"];
            MySqlCommand sell_Wri = new MySqlCommand(Del_Sel, connection);
            MySqlCommand reply_Wri = new MySqlCommand(Del_Rep, connection);
            sell_Wri.ExecuteNonQuery();
            reply_Wri.ExecuteNonQuery();
            connection.Close();
        }
        else
        {
            if (idcheck.Value == nickname.Text)
            {

                MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mariadb"].ConnectionString);
                string Sel_Sel = "select * from sellpost Where num = " + Request["No"];
                string Sel_Rep = "select * from reply Where postnumber = " + Request["No"];
                MySqlCommand command = new MySqlCommand(Sel_Sel, connection);
                MySqlCommand command2 = new MySqlCommand(Sel_Rep, connection);  
                connection.Open();
                string Del_Sel = "Delete From sellpost where num = " + Request["No"];
                string Del_Rep = "Delete From reply where postnumber = " + Request["No"];
                MySqlCommand sell_Wri = new MySqlCommand(Del_Sel, connection);
                MySqlCommand reply_Wri = new MySqlCommand(Del_Rep, connection);
                sell_Wri.ExecuteNonQuery();
                reply_Wri.ExecuteNonQuery();
                connection.Close();
                Response.Write("<script type='text/javascript'>alert('게시물이 삭제되었습니다.');location.href='sellpost.aspx';</script>");

            }
            else
            {
                Response.Write("<script type='text/javascript'>alert('타인이 작성한 게시글을 삭제할 수 없습니다.');window.history.go(-1);</script>");
            }
        }
    }

    protected void Buy_obj_Click(object sender, EventArgs e)
    {
        
    }
    protected void Update_Btn_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/ASP_Project2/Modify.aspx?No=" + Request["No"]);
    }
}


//}