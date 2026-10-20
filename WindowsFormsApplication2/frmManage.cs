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
            //修改10.20
            userInfo.Clear();
            //1. 从数据库中取出已注册的用户
            //DataTable dt;
            //dt = dbFace.GetDataTableBySql("select tbl_student.StuID, tbl_student.StuName, tbl_face.FaceCode from tbl_student, tbl_face where tbl_student.StuID=tbl_face.StuID");
            //int count = dt.Rows.Count;
            //if(count > 0)
            //{
            //    for(int i=0;i<count;i++)
            //    {
            //        userInfo[i] = new Dictionary<string, string>(){{"ID",(dt.Rows[i]["StuID"]).ToString()},
            //            {"Name",(dt.Rows[i]["StuName"]).ToString()}
            //        };
            //    }
            //}
            studentDataContext context = new studentDataContext();
            var query = from face in context.TBL_face
                        join stu in context.TBL_student
                        on face.StuID equals stu.StudID
                        where face.FaceCode != null
                        select new
                        {
                            StuID = stu.StudID,
                            StuName = stu.StuName,
                            FaceCode = face.FaceCode
                        };

            uint count = (uint)query.Count();
            if (count > 0)
            {
                int codeLen = bioFPCtrl1.GetFaceCodeLen();
                byte[] faceCode = new byte[count * codeLen];
                uint index = 0;
                uint[] Id = new uint[count];
                foreach (var data in query)
                {
                    Id[index] = index;
                    userInfo[(int)index] = new Dictionary<string, string>()
                    {
                            {"ID",data.StuID.ToString()},
                            {"Name",data.StuName.ToString()}
                    };
                    data.FaceCode.ToArray().CopyTo(faceCode, (index++) * codeLen);
                }
                bioFPCtrl1.SetFaceCode(count, faceCode, Id);
            }

            //userInfo.Clear();
            ////1. 从数据库中取出已注册的用户
            //DataTable dt;
            //dt = dbFace.GetDataTableBySql("select TBL_student.StudID, TBL_student.StuName, TBL_face.FaceCode from TBL_student, TBL_face where TBL_student.StudID=TBL_face.StuID");
            //int count = dt.Rows.Count;
            //if (count > 0)
            //{
            //    for (int i = 0; i < count; i++)
            //    {
            //        userInfo[i] = new Dictionary<string, string>(){{"ID",(dt.Rows[i]["StudID"]).ToString()},
            //            {"Name",(dt.Rows[i]["StuName"]).ToString()}
            //        };
            //    }
            //}
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
            if (txtID.Text.Length > 0)
            {
                bioFPCtrl1.Enroll();
            }
            else
            {
                MessageBox.Show("请输入学号");
            }
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
            if (btn_seek.Text == "识别")
            {
                bioFPCtrl1.Identify();
                btn_seek.Text = "停止";
            }
            else
            {
                bioFPCtrl1.Cancel();
                btn_seek.Text = "识别";
            }
        }

        private void bioFPCtrl1_EnrollNotify(object sender, int result, int face_index, uint face_quality, byte[] face_template, Bitmap face_image)
        {
            if (result > 0)
            {
                string userID = txtID.Text;
                studentDataContext context = new studentDataContext();

                //1、插入学生信息至 TBL_student 表
                TBL_student stu = new TBL_student();
                {
                    stu.StudID = userID;
                    stu.StuName = txtName.Text;
                    stu.Sex = radMan.Checked ? "男" : "女";
                    stu.Class = "202307Z";
                    stu.CreateTime = DateTime.Now.ToString();
                }
                context.TBL_student.InsertOnSubmit(stu);

                //2、插入人脸特征数据至 TBL_face 表
                TBL_face face = new TBL_face();
                {
                    face.StuID = userID;
                    face.FaceCode = face_template;
                }
                context.TBL_face.InsertOnSubmit(face);
                context.SubmitChanges();

                //保存图像
                face_image.Save(userID + ".jpg");
                MciPlaySound.PlaySound("sound\\zccg.wav", SND_FILENAME | SND_ASYNC);

                //加载编码
                var query = from TBL_face n in context.TBL_face
                            where n.TBL_student.StudID == n.StuID
                                  && n.FaceCode != null
                            select new
                            {
                                StuID = n.TBL_student.StudID,
                                StuName = n.TBL_student.StuName,
                                Sex = n.TBL_student.Sex,
                                FaceCode = n.FaceCode
                            };

                uint count = (uint)query.Count();
                if (count > 0)
                {
                    byte[] faceCode = new byte[count * face_template.Length];
                    uint index = 0;
                    uint[] Id = new uint[count];

                    foreach (var data in query)
                    {
                        Id[index] = index;
                        userInfo[(int)index] = new Dictionary<string, string>()
                {
                    {"ID",data.StuID.ToString()},
                    {"Name",data.StuName.ToString()}
                };

                        data.FaceCode.ToArray().CopyTo(
                            faceCode, (index++) * face_template.Length);
                    }

                    bioFPCtrl1.SetFaceCode(count, faceCode, Id);
                }
            }
            else
            {
                //重复注册
                if (result == FacePlatformCtrl.BioFPCtrl.BioFP_ERROR_ENROLL_DUPLICATE)
                {
                    if (face_index > -1)
                    {
                        MessageBox.Show("人脸已存在", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void bioFPCtrl1_IdentifyNotify(object sender, int result, int face_index)
        {
            TimeSpan span = DateTime.Now - identifyFinishTimestamp;
            if (span.TotalMilliseconds >= IDENTIFY_SPAN_LIMIT)
            {
                if (result > 0)
                {
                    identifyFinishTimestamp = DateTime.Now;
                    MciPlaySound.PlaySound("sound\\sbcg.wav", SND_FILENAME | SND_ASYNC);

                    if (face_index >= 0)
                    {
                        txtID.Text = userInfo[face_index]["ID"];
                        txtName.Text = userInfo[face_index]["Name"];
                        //txtID.Text = face_index.ToString();
                    }
                }
                else
                {
                    if (result == FacePlatformCtrl.BioFPCtrl.BioFP_ERROR_IDENTIFY_FAILED)
                    {
                        identifyFinishTimestamp = DateTime.Now;
                        MciPlaySound.PlaySound("sound\\sbsb.wav", SND_FILENAME | SND_ASYNC);
                    }
                }
            }
        }

        private void btn_load_Click(object sender, EventArgs e)
        {
            if(btn_load.Text=="加载")
            {
                bioFPCtrl1.ModuleLoad();
                InitUserInfo();
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
