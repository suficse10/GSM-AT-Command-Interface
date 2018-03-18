using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wavecom
{
    public partial class Form1 : Form
    {
        private string sim_operator;
        private string topup_ussd;
        private string op_name;
        private SerialPort crnt_port;
        public static WebRequest g_request;
        public static WebRequest r_request;
        public static string g_Topup_Operator = "";
        public static string g_Topup_Number = "";
        public static string g_Topup_Amount = "";
        public static string g_Topup_ID = "";

        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.Add("Airtel");
            comboBox1.Items.Add("GP");
            comboBox1.Items.Add("Robi");
            comboBox1.Items.Add("Banglalink");
            comboBox1.Items.Add("Teletalk");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            buttonSend.Enabled = false;
        }

        public void func_selectmodem()
        {
            //bool l_return_flag = false;
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            tB_monitor.AppendText("\r\nThe following serial ports were found:\r\n");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                tB_monitor.AppendText(port + "\r\n");
                crnt_port = serial_conn(port);

                if (crnt_port.IsOpen == true)
                {
                    tB_monitor.AppendText("Trying to connect at " + crnt_port.PortName + "\r\n");
                    crnt_port.WriteLine("AT\r");
                    Thread.Sleep(2000);
                    string l_response = crnt_port.ReadExisting();
                    if (l_response.IndexOf("OK") >= 0)
                    {
                        bool op_check = func_CompareOperator();
                        if (op_check == true)
                        {
                            label_port.Text = crnt_port.PortName;
                            tB_monitor.AppendText("Connected to Port: " + crnt_port.PortName + "\r\n");
                            //l_return_flag = true;
                            buttonSend.Enabled = true;
                            break;
                        }
                    }
                    crnt_port.Close();
                }
            }
            //return l_return_flag;
        }

        public bool func_CompareOperator()
        {
            bool l_return = false;

            // Network Registration Check
            tB_monitor.AppendText("\r\n Operator Check : ");
            crnt_port.WriteLine("AT+COPS=?\r");
            Thread.Sleep(20000);
            string l_response1 = crnt_port.ReadExisting();
            if ((l_response1.Contains("OK") == true) && (l_response1.Contains("+COPS:") == true))   // Whether Registered
            {
                //tB_monitor.AppendText(l_response1 + "\r\n");
                if (l_response1.Contains(sim_operator) == true)
                {
                    tB_monitor.AppendText(op_name);
                    l_return = true;
                }
                else
                {
                    tB_monitor.AppendText("SIM not found\r\n");
                    l_return = false;
                }
            }
            return l_return;
        }

        private void btn_modem_Click(object sender, EventArgs e)
        {
            comboBox_func();
            func_selectmodem();
            Thread.Sleep(5000);
            func_fetchData();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {

            string boperator = topup_ussd;
            //string mobile_no = tbMobileNo.Text;
            //string amount = tbAmount.Text;
            string mobile_no = g_Topup_Number;

            int l_amt = (int)Math.Round(float.Parse(g_Topup_Amount));
            string amount = l_amt.ToString();

            string pin = tbPIN.Text;

            try
            {
                tB_monitor.AppendText("USSD Code Running\r\n");
                if (crnt_port.IsOpen == false)
                { crnt_port.Open(); }
                Thread.Sleep(1000);
                crnt_port.WriteLine("AT\r");
                Thread.Sleep(1000);
                //serialPort1.WriteLine("ATD01717096211\r");
                //crnt_port.WriteLine("AT+CUSD=1,\"*124#\",15\r");
                //crnt_port.WriteLine("AT+COPS=?\r");
                crnt_port.WriteLine("AT+CUSD=1,\"" + boperator + mobile_no + "*" + amount + "*" + pin + "#" + "\",15" + "\r");
                Thread.Sleep(10000);

                //while (serialPort1.BytesToRead > 0)
                //{
                string message = crnt_port.ReadExisting();
              //  string message = "+CUSD: 2,\"SUCCESSFUL, Recharge of 10Tk for 8801992245543, TXN number R180220.1730.2600bd\",15";
              //  string message = "+CUSD: 2,\"Shuvo vai successful, Recharge of 10Tk for 8801992245543, TXN number R180220.1730.2600bd\",15";
                tB_monitor.AppendText(message + "\r\n");
                tB_monitor.AppendText("Topup Response request sent" + "\r\n");
                API_helper.func_ProcessAPI("", 2, message);
                tB_monitor.AppendText("Status : " + API_helper.status_str + "\r\n");
                tB_monitor.AppendText("Transaction No : " + API_helper.tr_id_str + "\r\n");
                tB_monitor.AppendText("Requested URL : " + API_helper.trim_str + "\r\n");

                crnt_port.Close();

                //// Update recharge id 
                //string request_url = "http://139.59.18.48/quickload/apis/new_request_message/?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + "154" + "&transaction_id=" + "R123" + "&message=Hello" + "&sms=Hello" + "&status=" + "Success";
                //tB_monitor.AppendText("Requested URL : " + request_url + "\r\n");
                ////Thread.Sleep(5000);
                //r_request = WebRequest.Create(request_url);
                //string return_r = API_helper.func_GetResponse(r_request);
                //tB_monitor.AppendText("Request sent successful" + "\r\n");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private static SerialPort serial_conn(string port)
        {
            SerialPort serialPort1 = new SerialPort(port);
            serialPort1.BaudRate = 115200;
            //serialPort1.PortName = "COM5";
            serialPort1.DataBits = 8;
            serialPort1.WriteTimeout = 1000;
            serialPort1.ReadTimeout = 5000;
            //serialPort1.Encoding = Encoding.GetEncoding("iso-8859-1");
            serialPort1.Open();
            serialPort1.DtrEnable = true;
            serialPort1.RtsEnable = true;

            return serialPort1;
        }

        public void comboBox_func()
        {
            if (this.comboBox1.Text == "Banglalink")
            {
                sim_operator = "2,\"Banglalink\",\"Banglalink\",\"47003";
                topup_ussd = "*555*";
                op_name = "Banglalink" + "\r\n";
            }
            else if (this.comboBox1.Text == "GP")
            {
                sim_operator = "2,\"Grameenphone\",\"Grameenphone\",\"47001";
                topup_ussd = "*444*1*";
                op_name = "Grameenphone" + "\r\n";
            }
            else if (this.comboBox1.Text == "Robi")
            {
                sim_operator = "2,\"Robi\",\"Robi\",\"47002";
                topup_ussd = "*444*1*";
                op_name = "Robi" + "\r\n";
            }
            else if (this.comboBox1.Text == "Airtel")
            {
                sim_operator = "2,\"Robi\",\"Robi\",\"47002";
                topup_ussd = "*444*1*";
                op_name = "Airtel" + "\r\n";
            }
            else if (this.comboBox1.Text == "Teletalk")
            {
                sim_operator = "2,\"Teletalk\",\"Teletalk\",\"47004";
                topup_ussd = "*444*1*";
                op_name = "Teletalk" + "\r\n";
            }
        }

        public void func_fetchData()
        {
            //if ((GlobalVars.g_API_request_flag == true) && (GlobalVars.g_TopupCmd_StepCtr == 0) && (GlobalVars.g_Response_Topup_flag == false) && (GlobalVars.g_QueryCmd_Topup_flag == false))
            //{
            //    if ((GlobalVars.g_FetchRequest_flag == true) && (GlobalVars.g_NextTopup_flag == false))
            //    {
            g_request = WebRequest.Create("http://139.59.18.48/quickload/apis/new_request?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467");
            // g_request = WebRequest.Create("https://logicbd.000webhostapp.com/getdata.php");
            string l_resp = API_helper.func_GetResponse(g_request);
            //GlobalVars.g_ussd_log_sw.WriteLine("Response : " + l_resp);
            tB_monitor.AppendText("Response : " + l_resp + "\r\n");
            l_resp = l_resp.Replace("\"", " ").Trim();
            API_helper.func_ProcessAPI(l_resp, 1, "");  // Response string and Mode ie Pending request                    
                                                        //tB_monitor.AppendText(API_helper.trim_str + "\r\n");
            tB_monitor.AppendText("Topup Number : " + g_Topup_Number + "\r\n");
            tB_monitor.AppendText("Topup Amount : " + g_Topup_Amount + "\r\n");
            tB_monitor.AppendText("Topup Operator : " + g_Topup_Operator + "\r\n");
            //txt_Log.AppendText("Topup Pin : " + GlobalVars.g_QueryCmd_PIN + "\r\n");

            //if ((crnt_port.IsOpen == true) /*&& (GlobalVars.g_Topup_Operator.Contains("Robi") == true)*/)
            //{
            //    if ((GlobalVars.g_Topup_Number.Length >= 10) && (GlobalVars.g_Topup_Amount.Length >= 1))
            //    {
            //        GlobalVars.g_NextTopup_flag = true;
            //        GlobalVars.g_NextTopup_ctr = 0;
            //        GlobalVars.g_QueryCmd_Topup_flag = true;
            //        GlobalVars.g_TopupCmd_StepCtr = 1;
            //        //at + CUSD = 1,"*8383*2*01848236297*10*8094#",15    
            //        //at + cusd = 1,"*444*1*01610005003*10*8094* #",15
            //        //GlobalVars.g_serialport.WriteLine("AT+STSF=1\r");
            //        int l_amt = (int)Math.Round(float.Parse(g_Topup_Amount));
            //        string l_topup_str = "AT+CUSD=1,\"*999*" + g_Topup_Number + "*" + l_amt.ToString() + "*" + GlobalVars.g_QueryCmd_PIN + "#\", 15\r";
            //        tB_monitor.AppendText("AT+CUSD=1,\"*999*" + g_Topup_Number + "*" + l_amt.ToString() + "*" + "\r\n");
            //        //GlobalVars.g_ussd_log_sw.WriteLine(l_topup_str);
            //        //GlobalVars.g_serialport.WriteLine("AT + CUSD = 1, \"*222*01727623404*10*8094#\", 15\r");
            //        Thread.Sleep(5000);
            //        tB_monitor.AppendText("Request Sent" + "\r\n");
            //        crnt_port.WriteLine(l_topup_str);
            //    }
            //}
            //}
            //}
        }

    }
}
