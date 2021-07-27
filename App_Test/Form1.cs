using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace App_Test
{
    public partial class Form1 : Form
    {
        //input data receive from serialport
        string sttheta;
        string stpsi;
        string stphi;
        string staddtheta;
        string staddphi;
        string longitude;
        string latitude;
        string altitude;       
        string SavePath1;
        string standby, cds, power;
        string date;
        string sdcard;
                

        public Form1()
        {
            InitializeComponent();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEventHandler);

        }
        
        private void DataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sData = sender as SerialPort;
                string recvData = sData.ReadLine();
               
                SerialData.Invoke((MethodInvoker)delegate { SerialData.AppendText(recvData); });
            }
            catch { }
        }

        private void btnConn_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbPort.Text != "")
                {
                    if (cbBaud.Text != "")
                    {
                        serialPort1.PortName = cbPort.Text;
                        serialPort1.BaudRate = Convert.ToInt32(cbBaud.Text);
                        serialPort1.Parity = Parity.None;
                        serialPort1.StopBits = StopBits.One;
                        serialPort1.DataBits = 8;
                        serialPort1.Handshake = Handshake.None;
                        serialPort1.RtsEnable = true;

                        if (serialPort1.IsOpen) return;
                        serialPort1.Open();
                        btnConn.Enabled = false;
                        btnDisConn.Enabled = true;
                        //


                        cbBaud.Enabled = false;
                        cbPort.Enabled = false;

                        btnExit.Enabled = false;

                    }
                    else
                        return;
                }
                else
                    return;
            }
            catch
            {
                return;
            }
        }

        //Connect botton//
        private void btnDisConn_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen == false) return;
                serialPort1.Close();
                btnConn.Enabled = true;
                btnDisConn.Enabled = false;
                //


                cbBaud.Enabled = true;
                cbPort.Enabled = true;

                btnExit.Enabled = true;
            }
            catch
            {
                return;
            }
        }

        //Serial data receive//
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //split data receive from serialport
                string[] arrList = serialPort1.ReadLine().Split(',');
                date = arrList[0];
                cds = arrList[1];                
                power = arrList[2];
                stpsi = arrList[3];// Psi value.
                sttheta = arrList[4];// Theta value.
                stphi = arrList[5];// Phi value.
                latitude = arrList[6];
                longitude = arrList[7];
                altitude = arrList[8];
                sdcard = arrList[9];
                standby = arrList[10];
            }
            catch
            {
                return;
            }

        }


        int intlen = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (btnConn.Enabled == false)
            {
                Draw(stpsi, sttheta, stphi, staddtheta, staddphi, altitude, longitude, latitude, power, cds);
                //display value on graph
                // text color and value
                lblpsi.ForeColor = Color.Red;
                lblpsi.Text = stpsi;

                lbltheta.ForeColor = Color.Red;
                lbltheta.Text = sttheta;

                lblphi.ForeColor = Color.Red;
                lblphi.Text = stphi;

                lblpower.ForeColor = Color.Red;
                lblpower.Text = power;

                lblcds.ForeColor = Color.Red;
                lblcds.Text = cds;

                lblstate.ForeColor = Color.Red;
                lblstate.Text = standby;

                txtLat.ForeColor = Color.Black;
                txtLat.Text = latitude;
                txtLong.ForeColor = Color.Black;
                txtLong.Text = longitude;
                txtAltitude.ForeColor = Color.Black;
                txtAltitude.Text = altitude;
                txtDate.ForeColor = Color.Black;
                txtDate.Text = date;
            }
            //auto detect COM port//
            string[] ports = SerialPort.GetPortNames();
            if (intlen != ports.Length)
            {
                intlen = ports.Length;
                cbPort.Items.Clear();
                for (int j = 0; j < intlen; j++)
                {
                    cbPort.Items.Add(ports[j]);
                }
            }
        }
        //
        int TickStart1;
        int TickStart2;
        int TickStart3;
        int TickStart4;
        int TickStart5;

        //
        private void Form1_Load(object sender, EventArgs e)
        {            
            cbBaud.Enabled = true;
            cbPort.Enabled = true;
            //Disable button control


            //Load value//            
            cbBaud.Items.Add(4800);
            cbBaud.Items.Add(9600);
            cbBaud.Items.Add(19200);
            cbBaud.Items.Add(38400);
            cbBaud.Items.Add(57600);
            
            //
            btnDisConn.Enabled = false;

            //Show contents of graph
            //Name graph
            //Value x,y axis..
            GraphPane myPane1 = zedGraphControl1.GraphPane;
            myPane1.Title.Text = "Pitch Angle Value ";
            myPane1.XAxis.Title.Text = "Time, Seconds";
            myPane1.YAxis.Title.Text = "Angle, Deg";

            RollingPointPairList list1 = new RollingPointPairList(60000);
            LineItem Curve1 = myPane1.AddCurve("Pitch ", list1, Color.Red, SymbolType.None);

            myPane1.XAxis.Scale.Min = 0;
            myPane1.XAxis.Scale.Max = 10;
            myPane1.YAxis.Scale.Min = -2000;
            myPane1.YAxis.Scale.Max = 2000;

            zedGraphControl1.AxisChange();
            TickStart1 = Environment.TickCount;

            /*Display the graph 2 of contents*/
            //Psidot graph//
            GraphPane myPane2 = zedGraphControl2.GraphPane;
            myPane2.Title.Text = "Roll Angle Value";
            myPane2.XAxis.Title.Text = "Time, Seconds";
            myPane2.YAxis.Title.Text = "Angle, Deg";

            RollingPointPairList list2 = new RollingPointPairList(60000);
            LineItem Curve2 = myPane2.AddCurve("Roll", list2, Color.Red, SymbolType.None);
            //

            myPane2.XAxis.Scale.Min = 0;
            myPane2.XAxis.Scale.Max = 10;
            myPane2.YAxis.Scale.Min = -500;
            myPane2.YAxis.Scale.Max = 500;

            zedGraphControl2.AxisChange();
            TickStart2 = Environment.TickCount;

            /*Display the graph 3 of contents*/
            //Theta graph//
            GraphPane myPane3 = zedGraphControl3.GraphPane;
            myPane3.Title.Text = "Yaw Angle Value";
            myPane3.XAxis.Title.Text = "Time, Seconds";
            myPane3.YAxis.Title.Text = "Angle, Deg";

            RollingPointPairList list3 = new RollingPointPairList(60000);
            LineItem Curve3 = myPane3.AddCurve("Yaw", list3, Color.Red, SymbolType.None);
            

            myPane3.XAxis.Scale.Min = 0;
            myPane3.XAxis.Scale.Max = 10;
            myPane3.YAxis.Scale.Min = -3000;
            myPane3.YAxis.Scale.Max = 3000;

            zedGraphControl3.AxisChange();
            TickStart3 = Environment.TickCount;

            GraphPane myPane4 = zedGraphControl4.GraphPane;
            myPane4.Title.Text = "Electrical Power";
            myPane4.XAxis.Title.Text = "Time, Seconds";
            myPane4.YAxis.Title.Text = "Electrical Power, mW";

            RollingPointPairList list4 = new RollingPointPairList(60000);
            LineItem Curve4 = myPane4.AddCurve("Electrical Power", list4, Color.Red, SymbolType.None);

            myPane4.XAxis.Scale.Min = 0;
            myPane4.XAxis.Scale.Max = 10;
            myPane4.YAxis.Scale.Min = 0;
            myPane4.YAxis.Scale.Max = 500;

            zedGraphControl4.AxisChange();
            TickStart4 = Environment.TickCount;

            GraphPane myPane5 = zedGraphControl5.GraphPane;
            myPane5.Title.Text = "Cds Value ";
            myPane5.XAxis.Title.Text = "Time, Seconds";
            myPane5.YAxis.Title.Text = "Cds";

            RollingPointPairList list5 = new RollingPointPairList(60000);
            LineItem Curve5 = myPane5.AddCurve("Cds ", list5, Color.Red, SymbolType.None);

            myPane5.XAxis.Scale.Min = 0;
            myPane5.XAxis.Scale.Max = 10;
            myPane5.YAxis.Scale.Min = 0;
            myPane5.YAxis.Scale.Max = 1500;

            zedGraphControl5.AxisChange();
            TickStart5 = Environment.TickCount;

        }
        //Draw line in the zedgraph using string data    //
        //receive from serialport convert to double value//
        private void Draw(string inpsi, string intheta, string inphi, string inaddtheta, string inaddphi, string inlatitude, string inlongitude, string inaltitude, string inpower, string incds)
        {
            double _psi;
            double _theta;
            double _phi;
            double _addtheta;
            double _addphi;
            double _latitude;
            double _longitude;
            double _altitude;            
            double _power;
            double _cds;

            double.TryParse(inpsi, out _psi);//Convert tring to double//
            double.TryParse(intheta, out _theta);//Convert tring to double//
            double.TryParse(inphi, out _phi);//Convert tring to double//
            double.TryParse(inaddtheta, out _addtheta);
            double.TryParse(inaddphi, out _addphi);
            double.TryParse(inlatitude, out _latitude);
            double.TryParse(inlongitude, out _longitude);
            double.TryParse(inaltitude, out _altitude);           
            double.TryParse(inpower, out _power);
            double.TryParse(incds, out _cds);
            

            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;
            if (zedGraphControl2.GraphPane.CurveList.Count <= 0)
                return;
            if (zedGraphControl3.GraphPane.CurveList.Count <= 0)
                return;
            if (zedGraphControl4.GraphPane.CurveList.Count <= 0)
                return;
            if (zedGraphControl5.GraphPane.CurveList.Count <= 0)
                return;

            LineItem curve1 = zedGraphControl1.GraphPane.CurveList[0] as LineItem;
            LineItem curve2 = zedGraphControl2.GraphPane.CurveList[0] as LineItem;
            LineItem curve3 = zedGraphControl3.GraphPane.CurveList[0] as LineItem;
            LineItem curve4 = zedGraphControl4.GraphPane.CurveList[0] as LineItem;
            LineItem curve5 = zedGraphControl5.GraphPane.CurveList[0] as LineItem;

            if (curve1 == null)
                return;
            if (curve2 == null)
                return;
            if (curve3 == null)
                return;
            if (curve4 == null)
                return;
            if (curve5 == null)
                return;

            //
            IPointListEdit list1 = curve1.Points as IPointListEdit;
            IPointListEdit list2 = curve2.Points as IPointListEdit;
            IPointListEdit list3 = curve3.Points as IPointListEdit;
            IPointListEdit list4 = curve4.Points as IPointListEdit;
            IPointListEdit list5 = curve5.Points as IPointListEdit;
            //


            //
            if (list1 == null)
                return;
            if (list2 == null)
                return;
            if (list3 == null)
                return;
            if (list4 == null)
                return;
            if (list5 == null)
                return;
            //


            //
            double time1 = (Environment.TickCount - TickStart1) / 1000.0;
            double time2 = (Environment.TickCount - TickStart2) / 1000.0;
            double time3 = (Environment.TickCount - TickStart3) / 1000.0;
            double time4 = (Environment.TickCount - TickStart4) / 1000.0;
            double time5 = (Environment.TickCount - TickStart5) / 1000.0;

            //
            list1.Add(time1, _psi);
            list2.Add(time2, _theta);
            list3.Add(time3, _phi);
            list4.Add(time4, _power);
            list5.Add(time5, _cds);
            //


            //
            Scale xScale1 = zedGraphControl1.GraphPane.XAxis.Scale;
            Scale xScale2 = zedGraphControl2.GraphPane.XAxis.Scale;
            Scale xScale3 = zedGraphControl3.GraphPane.XAxis.Scale;
            Scale xScale4 = zedGraphControl4.GraphPane.XAxis.Scale;
            Scale xScale5 = zedGraphControl5.GraphPane.XAxis.Scale;

            //
            Scale yScale1 = zedGraphControl1.GraphPane.YAxis.Scale;
            Scale yScale2 = zedGraphControl2.GraphPane.YAxis.Scale;
            Scale yScale3 = zedGraphControl3.GraphPane.YAxis.Scale;
            Scale yScale4 = zedGraphControl4.GraphPane.YAxis.Scale;
            Scale yScale5 = zedGraphControl5.GraphPane.YAxis.Scale;

            //
            if (time1 > xScale1.Max - xScale1.MajorStep)
            {
                xScale1.Max = time1 + xScale1.MajorStep;
                xScale1.Min = xScale1.Max - 30;//Auto scale x axis in limit time
            }
            if (time2 > xScale2.Max - xScale2.MajorStep)
            {
                xScale2.Max = time2 + xScale2.MajorStep;
                xScale2.Min = xScale2.Max - 30;
            }
            if (time3 > xScale3.Max - xScale3.MajorStep)
            {
                xScale3.Max = time3 + xScale3.MajorStep;
                xScale3.Min = xScale3.Max - 30;
            }
            if (time4 > xScale4.Max - xScale4.MajorStep)
            {
                xScale4.Max = time4 + xScale4.MajorStep;
                xScale4.Min = xScale4.Max - 30;
            }
            if (time5 > xScale5.Max - xScale5.MajorStep)
            {
                xScale5.Max = time5 + xScale5.MajorStep;
                xScale5.Min = xScale5.Max - 30;
            }
            //
            zedGraphControl1.AxisChange();
            zedGraphControl2.AxisChange();
            zedGraphControl3.AxisChange();
            zedGraphControl4.AxisChange();
            zedGraphControl5.AxisChange();
            //
            zedGraphControl1.Invalidate();
            zedGraphControl2.Invalidate();
            zedGraphControl3.Invalidate();
            zedGraphControl4.Invalidate();
            zedGraphControl5.Invalidate();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            Close();
        }


        private void SerialData_TextChanged(object sender, EventArgs e)
        {
            SerialData.SelectionStart = SerialData.Text.Length;
            SerialData.ScrollToCaret();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog1.ShowDialog();
                SavePath1 = saveFileDialog1.FileName;
                StreamWriter sw = new StreamWriter(SavePath1, true);
                sw.Write(SerialData.Text);
                sw.Close();
                sw.Dispose();
            }
            catch { }
        }

        private void Case_1_Click(object sender, EventArgs e)
        {
            serialPort1.Write("1");
        }

        private void Case_4_Click(object sender, EventArgs e)
        {
            serialPort1.Write("4");
        }

        private void Case_5_Click(object sender, EventArgs e)
        {
            serialPort1.Write("5");
        }

        private void Case_2_Click(object sender, EventArgs e)
        {
            serialPort1.Write("2");
        }

        private void Case_3_Click(object sender, EventArgs e)
        {
            serialPort1.Write("3");
        }
    }
}
