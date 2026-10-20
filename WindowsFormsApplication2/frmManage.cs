using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DoorAccess2026;

namespace WindowsFormsApplication2
{
    public partial class frmManage : Form
    {
        //添加 10.20
        public const int SND_FILENAME = 0x00020000;
        public const int SND_ASYNC = 0x0001;
        public DateTime identifyFinishTimestamp = DateTime.Now;
        public const int IDENTIFY_SPAN_LIMIT = 2000;

        public Dictionary<int, Dictionary<string, string>> userInfo = new Dictionary
<int, Dictionary<string, string>>();
        DBService dbFace = DBService.getInstance("DoorAccess2026");


        public void InitUserInfo()
        {
            userInfo.Clear();
            //1. 从数据库中取出已注册的用户
            DataTable dt;
            dt = dbFace.GetDataTableBySql("select TBL_student.StudID, TBL_student.StuName, TBL_face.FaceCode from TBL_student, TBL_face where TBL_student.StudID=TBL_face.StuID");
            int count = dt.Rows.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    userInfo[i] = new Dictionary<string, string>(){{"ID",(dt.Rows[i]["StudID"]).ToString()},
                        {"Name",(dt.Rows[i]["StuName"]).ToString()}
                    };
                }
            }
        }
        public frmManage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            common.getmainform().Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void radWoman_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmManage_Load(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ds_stu1.Clear();
            //sqlDataAdapter1.Fill(ds_stu1);
            studentDataContext context = new studentDataContext();
            var q = context.TBL_student;
            dataGridView1.DataSource = q;
            ////增加数据
            //WindowsFormsApplication2.TBL_student student = new TBL_student();
            //{
            //    student.StudID = "20201001";
            //    student.StuName = "哈哈";
            //    student.Sex = "男";
            //    student.CreateTime = DateTime.Now.ToString();
            //};
            ////在上下文中注册新建的对象
            //context.TBL_student.InsertOnSubmit(student);
            ////提交执行
            //context.SubmitChanges();
            //dataGridView1.DataSource = q;


            ////改
            //var x = context.TBL_student.Where(p => p.StudID == "20242005");
            //if (x.Count() > 0)
            //{
            //    WindowsFormsApplication2.TBL_student data = x.First();
            //    data.Class = "2007Z";
            //}
            //context.SubmitChanges();
            //studentDataContext context_late = new studentDataContext();
            //dataGridView1.DataSource = context_late.TBL_studen

            ////查
            //var x = context.TBL_student.Where(p => p.StudID == "20242005");

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button_reflsh_Click(object sender, EventArgs e)
        {
            try
            {
                int result = sqlDataAdapter1.Update(ds_stu1, "TBL_student");
            }
            catch (Exception ex)
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btn_review_Click(object sender, EventArgs e)
        {
            studentDataContext context = new studentDataContext();
            var q = context.TBL_student;
            dataGridView2.DataSource = q;
        }

        private void btn_seek_Click(object sender, EventArgs e)
        {

        }

        private void bioFPCtrl1_EnrollNotify(object sender, int result, int face_index, uint face_quality, byte[] face_template, Bitmap face_image)
        {

        }

        private void bioFPCtrl1_IdentifyNotify(object sender, int result, int face_index)
        {

        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            if(btn_load.Text=="加载")
            {
                bioFPCtrl1.ModuleLoad();
                btn_load.Text = "卸载";
            }
            else if(btn_load.Text == "卸载")
            {
                bioFPCtrl1.ModuleLoad();
                btn_load.Text = "加载";
            }
            
        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
            if (row == null)
            {
                return;
            }
            txtID.Text = row.Cells["StudID"].Value == null ? "" : row.Cells["StudID"].Value.ToString();
            txtName.Text = row.Cells["StuName"].Value == null ? "" : row.Cells["StuName"].Value.ToString();
            cmbClass.Text = row.Cells["Class"].Value == null ? "" : row.Cells["Class"].Value.ToString();
            string sex = row.Cells["Sex"].Value == null ? "" : row.Cells["Sex"].Value.ToString();
            radMan.Checked = (sex == "男");
            radWoman.Checked = (sex == "女");
        }
    
    }
}
