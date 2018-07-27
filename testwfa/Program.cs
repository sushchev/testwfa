using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using ximc;

namespace WindowsFormsApplication1
{
   
    static class Program
    {     

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1_t = new Form1();
            try
            {              
                Application.Run(form1_t);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception " + e.Message);
                Console.WriteLine(e.StackTrace.ToString());
                MessageBox.Show("Exception " + e.Message);
            }
            finally
            {
                form1_t.close_all();                
            }                        
        }
    }
}
