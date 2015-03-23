using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json; //Json De/serialization

namespace SEUWlanAuthen
{
    public partial class FormSEUAuthen : Form
    {
        private string myHttpResponse;
        private bool userLoaded = false;//indicate user load from json file
        private User jsonUser = new User("0", "0");
        private string jsonPath = "config.json";
        public FormSEUAuthen()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //修改About标签的字体样式--加下划线
            Font labelAboutFont = labelAbout.Font;
            Font aFont = new Font(labelAboutFont, FontStyle.Underline);
            labelAbout.Font = aFont;
            labelAbout.Text = ""; //不显示该标签

            labelIP.Text = "";
            labelLocation.Text = "";

            if(File.Exists(jsonPath) ==true)
            {
                string jsonReadStr = File.ReadAllText(jsonPath);
                try
                {
                    jsonUser.DeSerialize(jsonReadStr);
                }
                catch(Exception ex)
                {
                    labelIP.Text = "config.json文件异常!";
                    labelLocation.Text = "请手动删除该文件!";
                }

                textBoxStuID.Text = jsonUser.StuID;
                textBoxPwd.Text = jsonUser.Pwd;
                buttonLogin_Click(null, null);//autologin
            }
        }

        private void labelAbout_Click(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            User thisUser;
            if (userLoaded == true)
            {
                thisUser = new User(jsonUser);
            }
            else
            {
                thisUser = new User(textBoxStuID.Text, textBoxPwd.Text);
            }
            
            string url = "https://w.seu.edu.cn/portal/login.php";
            WebRequest myWebRequest = WebRequest.Create(url);
            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";
            string requestBody = String.Format( "username={0}&password={1}",thisUser.StuID, thisUser.Pwd); //post data including ID & password
            byte[] postData = Encoding.UTF8.GetBytes(requestBody);

            System.IO.Stream requestStream = myWebRequest.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

            HttpWebResponse myWebResponse;
            string response;
            try
            {
                myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader responseReader = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                response = responseReader.ReadToEnd();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            myHttpResponse = response;
            ResponseJson myResponseJson = JsonConvert.DeserializeObject<ResponseJson>(response);
            if(myResponseJson.error == null) // authen success
            {
                labelIP.Text = "登陆成功"+"IP:"+myResponseJson.login_ip;
                labelLocation.Text = myResponseJson.login_location;
                
                //save user information
                
                if (userLoaded == true)
                {
                    if (thisUser.IsEqual(jsonUser) == false)
                    {
                        //将用户信息写入磁盘
                        File.WriteAllText(jsonPath, thisUser.Serialize());
                    }
                }
                else
                {
                    //将用户信息写入磁盘
                    File.WriteAllText(jsonPath, thisUser.Serialize());
                }
            }
            else
            {
                labelIP.Text = "登陆失败";
                labelLocation.Text = "请检查用户名与密码!";
            }
            

        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            string url = "https://w.seu.edu.cn/portal/logout.php";
            WebRequest myWebRequest = WebRequest.Create(url);
            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";
            string requestBody = "";
            byte[] postData = Encoding.UTF8.GetBytes(requestBody);

            System.IO.Stream requestStream = myWebRequest.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();

            HttpWebResponse myWebResponse;
            string response;
            try
            {
                myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
                StreamReader responseReader = new StreamReader(myWebResponse.GetResponseStream(), Encoding.UTF8);
                response = responseReader.ReadToEnd();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
                response = ex.Message;
                labelIP.Text = response;
                return;
            }
            myHttpResponse = response;
            ResponseJson myResponseJson = JsonConvert.DeserializeObject<ResponseJson>(response);
            if (myResponseJson.error == null) // authen success
            {
                labelIP.Text = "IP: " + myResponseJson.login_ip +"  "+ myResponseJson.success;
                labelLocation.Text ="地点: "+ myResponseJson.login_location;
            }
            else
            {
                labelIP.Text = "发生错误";
            }
            
        }

        private void FormSEUAuthen_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            AboutBox myAboutBox = new AboutBox();
            myAboutBox.Show();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void buttonMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
