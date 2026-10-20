using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class common
    {
        public static frmMain main = null;
        public static frmManage manage = null;
        public static frmMain getmainform()
        {
            if(main == null)
            {
                main = new frmMain();
                return main;
            }
            else
            {
                return main;
            }
        }
        public static frmManage getmanageform()
        {
            if (manage == null)
            {
                manage= new frmManage();
                return manage;
            }
            else
            {
                return manage;
            }
        }
    }

}
