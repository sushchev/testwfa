using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ximc;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        public Result res;

        public static API.LoggingCallback callback;
        public static void MyLog(API.LogLevel loglevel, string message, IntPtr user_data)
        {
            Console.WriteLine("MyLog {0}: {1}", loglevel, message);
        }
        public Form1()
        {
            InitializeComponent();

            DateTime time = DateTime.Now;
            Console.WriteLine("testapp CLR runtime version: " + Assembly.GetExecutingAssembly().ImageRuntimeVersion);
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.GetName().Name.Equals("ximcnet"))
                    Console.WriteLine("ximcnet CLR runtime version: " + a.ImageRuntimeVersion);
            Console.WriteLine("Current CLR runtime version: " + Environment.Version.ToString());
            callback = new API.LoggingCallback(MyLog);
            API.set_logging_callback(callback, IntPtr.Zero);
        }

        public int get_device_number(String dev)
        {
            for (int i = 0; i < 6; i++)
                if (dev == API.get_device_name(device_enumeration, i))
                    return i;
            return -1;
        }

        public void close_all()
        {
            for (int i = 0; i < API.get_device_count(device_enumeration); i++)
            {
                if (device_id[i] != -1)
                    API.close_device(ref device_id[i]);
            }
        }

        void update_chart()
        {
            int[] XY = new int[6];           
            status_t status;
            for (int i = 0; i < 6; i++)
            {
                res = API.get_status(device_id[get_device_number(device_name[i])], out status);
                XY[i] = status.CurPosition;              
            };
            if (chart1.Series[0].Points.Count < 1)
            {
                // If there are no other points
                chart1.Series[0].Points.AddXY(XY[0], XY[1]);
            } 
            else if (XY[0] != chart1.Series[0].Points.Last().XValue | XY[1] != chart1.Series[0].Points.Last().YValues.Last())
            {
                // If moving (last point and current point are different)
                chart1.Series[0].Points.AddXY(XY[0], XY[1]);
            };
            if (chart2.Series[0].Points.Count < 1)
            {
                chart2.Series[0].Points.AddXY(XY[2], XY[3]);
            }
            else if (XY[2] != chart2.Series[0].Points.Last().XValue | XY[3] != chart2.Series[0].Points.Last().YValues.Last())
            {
                chart2.Series[0].Points.AddXY(XY[2], XY[3]);
            };
            if (chart3.Series[0].Points.Count < 1)
            {
                chart3.Series[0].Points.AddXY(XY[4], XY[5]);
            }
            else if (XY[4] != chart3.Series[0].Points.Last().XValue | XY[5] != chart3.Series[0].Points.Last().YValues.Last())
            {
                chart3.Series[0].Points.AddXY(XY[4], XY[5]);
            }

            // Fixing zero x point Windows Forms issue
            if (chart1.Series[0].Points.Last().XValue == 0)
                chart1.Series[0].Points.Last().XValue = Double.Epsilon;
            if (chart2.Series[0].Points.Last().XValue == 0)
                chart2.Series[0].Points.Last().XValue = Double.Epsilon;
            if (chart3.Series[0].Points.Last().XValue == 0)
                chart3.Series[0].Points.Last().XValue = Double.Epsilon;
        }
        
        // Pointer to device enumeration structure
        IntPtr device_enumeration;
        // Program enables up to 6 devices
        // Names of devices
        String[] device_name = new String[6];
        // IDs of devices
        int[] device_id = new int[6] { -1, -1, -1, -1, -1, -1 };
        // Probe flags, used to enable various enumeration options
        const int probe_flags = (int)(Flags.ENUMERATE_PROBE | Flags.ENUMERATE_NETWORK);
        // Enumeration hint, currently used to indicate ip address for network enumeration
        String enumerate_hints = "addr=192.168.1.1,172.16.2.3";
        // String enumerate_hints = "addr="; // this hint will use broadcast enumeration, if ENUMERATE_NETWORK flag is enabled
        
        private void button2_Click(object sender, EventArgs e)
        {
            close_all();

            device_id = new int[6];
            device_name = new String[6];

            button4.Enabled = false;
            button5.Enabled = false;
            button7.Enabled = false;
            //  Sets bindy (network) keyfile. Must be called before any call to "enumerate_devices" or "open_device" if you
            //  wish to use network-attached controllers. Accepts both absolute and relative paths, relative paths are resolved
            //  relative to the process working directory. If you do not need network devices then "set_bindy_key" is optional.
            API.set_bindy_key("keyfile.sqlite");

            // Enumerates all devices
            device_enumeration = API.enumerate_devices(probe_flags, enumerate_hints);
            if (device_enumeration == null)
                throw new Exception("Error enumerating devices");

            // Gets found devices count
            int device_count = API.get_device_count(device_enumeration);

            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox9.Items.Clear();
            comboBox10.Items.Clear();
            comboBox11.Items.Clear();
            comboBox12.Items.Clear();
            comboBox13.Items.Clear();

            // List all found devices
            for (int i = 0; i < device_count; ++i)
            {
                // Gets device name

                String dev = API.get_device_name(device_enumeration, i);
                device_name[i] = dev;
                device_id[i] = API.open_device(device_name[i]);
                status_t status;
                res = API.get_status(device_id[i], out status);
                if (res != Result.ok)
                    throw new Exception("Error " + res.ToString());
                
                comboBox1.Items.Add(dev);
                comboBox2.Items.Add(dev);
                comboBox9.Items.Add(dev);
                comboBox10.Items.Add(dev);
                comboBox11.Items.Add(dev);
                comboBox12.Items.Add(dev);
                comboBox13.Items.Add(dev);

            }

            button4.Enabled = true;
            button5.Enabled = true;
            button7.Enabled = true;

            chart1.ChartAreas[0].AxisX.Title = device_name[0] + "\n";
            chart1.ChartAreas[0].AxisY.Title = device_name[1] + "\n";
            chart2.ChartAreas[0].AxisX.Title = device_name[2] + "\n";
            chart2.ChartAreas[0].AxisY.Title = device_name[3] + "\n";
            chart3.ChartAreas[0].AxisX.Title = device_name[4] + "\n";
            chart3.ChartAreas[0].AxisY.Title = device_name[5] + "\n";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < API.get_device_count(device_enumeration); i++)
            {
                // Stop device
                API.command_stop(device_id[i]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < API.get_device_count(device_enumeration); i++)
            {
                // Send "zero" command to device
                res = API.command_zero(device_id[i]);
                if (res != Result.ok)
                    throw new Exception("Error " + res.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Clear all charts
            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart3.Series[0].Points.Clear();
            update_chart();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            int text;
            if (comboBox1.SelectedItem != null && int.TryParse(textBox1.Text, out text))
            {
                API.command_move(device_id[get_device_number(comboBox1.SelectedItem.ToString())], text, 0);
                status_t status;
                res = API.get_status(device_id[get_device_number(comboBox1.SelectedItem.ToString())], out status);
                if (res != Result.ok)
                    throw new Exception("Error " + res.ToString());
            }
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new Point(e.X, e.Y);
            if (device_id[0] != -1)
                API.command_move(device_id[get_device_number(device_name[0])], (int)chart1.ChartAreas[0].AxisX.PixelPositionToValue(mousePoint.X), 0);
            if (device_id[1] != -1)
                API.command_move(device_id[get_device_number(device_name[1])], (int)chart1.ChartAreas[0].AxisY.PixelPositionToValue(mousePoint.Y), 0);
        }

        private void chartTimer_Tick_1(object sender, EventArgs e)
        {         
            update_chart();
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new Point(e.X, e.Y);
            if (!Double.IsNaN(chart1.ChartAreas[0].CursorX.Position) && !Double.IsNaN(chart1.ChartAreas[0].CursorY.Position))
            {
                chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
                chart1.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);
                label1.Text = "Position: " + (int)chart1.ChartAreas[0].AxisX.PixelPositionToValue(mousePoint.X) + "; " + (int)chart1.ChartAreas[0].AxisY.PixelPositionToValue(mousePoint.Y);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Remove first points 
            if (checkBox1.Checked)
            {
                if (chart1.Series[0].Points.Count > 1)
                {
                    chart1.Series[0].Points.RemoveAt(0);
                }
                if (chart2.Series[0].Points.Count > 1)
                {
                    chart2.Series[0].Points.RemoveAt(0);
                }
                if (chart3.Series[0].Points.Count > 1)
                {
                    chart3.Series[0].Points.RemoveAt(0);
                }
            }
        }

        private void chart2_MouseClick(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new Point(e.X, e.Y);
            if (device_id[2] != -1)
                API.command_move(device_id[get_device_number(device_name[2])], (int)chart2.ChartAreas[0].AxisX.PixelPositionToValue(mousePoint.X), 0);
            if (device_id[3] != -1)
                API.command_move(device_id[get_device_number(device_name[3])], (int)chart2.ChartAreas[0].AxisY.PixelPositionToValue(mousePoint.Y), 0);
        }

        private void chart3_MouseClick(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new Point(e.X, e.Y);
            if (device_id[4] != -1)
                API.command_move(device_id[get_device_number(device_name[4])], (int)chart3.ChartAreas[0].AxisX.PixelPositionToValue(mousePoint.X), 0);
            if (device_id[5] != -1)
                API.command_move(device_id[get_device_number(device_name[5])], (int)chart3.ChartAreas[0].AxisY.PixelPositionToValue(mousePoint.Y), 0);
        }

        private void chart3_MouseMove(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new Point(e.X, e.Y);
            if (!Double.IsNaN(chart1.ChartAreas[0].CursorX.Position) && !Double.IsNaN(chart1.ChartAreas[0].CursorY.Position))
            {
                chart3.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
                chart3.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);
                label3.Text = "Position: " + (int)chart3.ChartAreas[0].AxisX.PixelPositionToValue(mousePoint.X) + "; " + (int)chart3.ChartAreas[0].AxisY.PixelPositionToValue(mousePoint.Y);
            }
        }

        private void chart2_MouseMove(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new Point(e.X, e.Y);
            if (!Double.IsNaN(chart1.ChartAreas[0].CursorX.Position) && !Double.IsNaN(chart1.ChartAreas[0].CursorY.Position))
            {
                chart2.ChartAreas[0].CursorX.SetCursorPixelPosition(mousePoint, true);
                chart2.ChartAreas[0].CursorY.SetCursorPixelPosition(mousePoint, true);
                label2.Text = "Position: " + (int)chart2.ChartAreas[0].AxisX.PixelPositionToValue(mousePoint.X) + "; " + (int)chart2.ChartAreas[0].AxisY.PixelPositionToValue(mousePoint.Y);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox13.SelectedItem != null)
                device_name[0] = comboBox13.SelectedItem.ToString();
            if (comboBox12.SelectedItem != null)
                device_name[1] = comboBox12.SelectedItem.ToString();
            if (comboBox11.SelectedItem != null)
                device_name[2] = comboBox11.SelectedItem.ToString();
            if (comboBox10.SelectedItem != null)
                device_name[3] = comboBox10.SelectedItem.ToString();
            if (comboBox9.SelectedItem != null)
                device_name[4] = comboBox9.SelectedItem.ToString();
            if (comboBox2.SelectedItem != null)
                device_name[5] = comboBox2.SelectedItem.ToString();

            // фиксить null
            chart1.ChartAreas[0].AxisX.Title = device_name[0] + "\n";
            chart1.ChartAreas[0].AxisY.Title = device_name[1] + "\n";
            chart2.ChartAreas[0].AxisX.Title = device_name[2] + "\n";
            chart2.ChartAreas[0].AxisY.Title = device_name[3] + "\n";
            chart3.ChartAreas[0].AxisX.Title = device_name[4] + "\n";
            chart3.ChartAreas[0].AxisY.Title = device_name[5] + "\n";
        }
    }
}
