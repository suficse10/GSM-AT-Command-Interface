using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wavecom
{
    class API_helper
    {
        public static string trim_str;
        public static string status_str;
        public static string tr_id_str;
        public static WebRequest return_request;

        public static void func_ProcessAPI(string l_response, int l_mode, string l_sms_str)
        {
            try
            {
                string l_request_id = "";
                string l_request_number = "";
                string l_request_amount = "";
                string l_request_mobiletype = "";
                string l_request_operator = "";

                l_response = l_response.Replace("[", " ").Trim();
                l_response = l_response.Replace("{", " ").Trim();
                l_response = l_response.Replace("]", " ").Trim();
                l_response = l_response.Replace("}", " ").Trim();
                l_response = l_response.Replace("\"", " ").Trim();
                l_response = l_response.Replace(":", " ").Trim();
                trim_str = l_response;
                //GlobalVars.g_ussd_log_sw.WriteLine("Process : " + l_response + ", Mode : " + l_mode + ", SMS :" + l_sms_str);
                switch (l_mode)
                {
                    case 1:
                        //[{"recharge_id":"1","number":"01727623404","amount":"12.0000","type":"Prepaid","operator":"GrameenPhone"}]
                        //[{ recharge_id : 1 , number : 01727623404 , amount : 12.0000 , type : Prepaid , operator : GrameenPhone }]
                        //[{"recharge_id":"1","number":"01727623404","amount":"12.0000","type":"Prepaid"},
                        // {"recharge_id":"2","number":"01721829178","amount":"20.0000","type":"Prepaid"},
                        // {"recharge_id":"3","number":"01727623404","amount":"15.0000","type":"Prepaid"}]
                        //if ((l_response.Contains("recharge_id") == true) && (l_response.Contains("number") == true) && (l_response.Contains("Prepaid") == true) && (l_response.Contains("operator") == true))
                        //{
                        string[] l_request_arr = l_response.Split(',');

                        //if (l_request_arr.Length >= 5)
                        if (l_request_arr.Length >= 2)
                        {
                            l_request_id = l_request_arr[0];
                            l_request_number = l_request_arr[1];
                            l_request_amount = l_request_arr[2];
                            l_request_mobiletype = l_request_arr[3];
                            l_request_operator = l_request_arr[4];

                            l_request_id = l_request_id.Replace("recharge_id", " ").Trim();
                            l_request_number = l_request_number.Replace("number", " ").Trim();
                            l_request_amount = l_request_amount.Replace("amount", " ").Trim();
                            l_request_mobiletype = l_request_mobiletype.Replace("type", " ").Trim();
                            l_request_operator = l_request_operator.Replace("operator", " ").Trim();

                            //ID: [{"recharge_id":"1"
                            //Number: "number":"01727623404"
                            //Amount: "amount":"12.0000"
                            //Type: "type":"Prepaid"
                            //Operator: "operator":"robi"}]

                            Form1.g_Topup_ID = l_request_id;
                            Form1.g_Topup_Number = l_request_number;
                            Form1.g_Topup_Amount = l_request_amount;
                            Form1.g_Topup_Operator = l_request_operator;

                        }
                        //}
                        //else
                        //{
                        //    GlobalVars.g_ussd_log_sw.WriteLine("ID : " + l_request_id); GlobalVars.g_Topup_ID = "0";
                        //    GlobalVars.g_ussd_log_sw.WriteLine("Number : " + l_request_number); GlobalVars.g_Topup_Number = "0";
                        //    GlobalVars.g_ussd_log_sw.WriteLine("Amount :" + l_request_amount); GlobalVars.g_Topup_Amount = "0";
                        //    GlobalVars.g_ussd_log_sw.WriteLine("Type :" + l_request_mobiletype);
                        //    GlobalVars.g_ussd_log_sw.WriteLine("Operator :" + l_request_operator); GlobalVars.g_Topup_Operator = "0";
                        //}
                        break;

                    case 2:  // Validate the recharge/topup operation
                        if (l_sms_str.Length > 10)
                        {
                            // l_sms_str = 210:Recharge 10 Tk to 01848236297 is successful. Transaction number is R170904.1323.110088.Your new balance is 3399.03 Tk.Thank You.
                            // 210:Recharge 10 Tk to 1848236297 is successful. Transaction number is R171103.1056.25004f.Your new balance is 2026.46 Tk. Thank You.
                            //@211:Additional commission of 0.78 for transfer ID R171117.1059.230099 has been credited for customer 1848236297. Your new balance is 1310.60 and adjustme
                            //   txt_Log.AppendText("Topup Response request sent" + "\r\n");
                            //   GlobalVars.g_ussd_log_sw.WriteLine("Topup Response request sent" + "\r\n");

                            int l_bal_sop = l_sms_str.IndexOf("balance is");
                            //int l_bal_eop = l_sms_str.IndexOf("Tk.", l_bal_sop);
                            //int l_bal_eop2 = -1; string l_balance = "";
                            int l_bal_eop = l_sms_str.IndexOf(" ", l_bal_sop + 11);
                            //int l_bal_eop2 = -1; 
                            string l_balance = "";
                            l_balance = l_sms_str.Substring(l_bal_sop + 11, l_bal_eop - l_bal_sop - 11).Trim();
                            //if (l_bal_eop == -1)
                            //{
                            //    l_bal_eop2 = l_sms_str.IndexOf("and adjustme");
                            //    //int l_trans_eop = l_response.IndexOf("is underprocess");
                            //    l_balance = l_sms_str.Substring(l_bal_sop + 10, l_bal_eop - l_bal_sop - 10).Trim();
                            //    //string l_transaction = l_response.Substring(l_trans_sop + 14, l_trans_eop - l_trans_sop - 14).Trim();
                            //}

                            //   GlobalVars.g_ussd_log_sw.WriteLine("Bal : " + l_balance.Replace("TK", " ").Trim() + "\r\n");
                            //   txt_Log.AppendText("Bal : " + l_balance.Replace("TK", " ").Trim() + "\r\n");
                            //txt_Log.AppendText("TransID : " + l_transaction + "\r\n");
                            string l_bal_val = l_balance.Replace("TK", " ").Trim();
                            //   GlobalVars.g_modem_Balance_str = l_bal_val;
                            //   lbl_Balance1.Text = l_bal_val; lbl_Balance1.Update(); lbl_Balance1.Refresh();
                            //   lbl_Balance2.Text = l_bal_val; lbl_Balance2.Update(); lbl_Balance2.Refresh();

                            string l_status = "";
                            string l_sms_ID = "";
                            // SMS details
                            if (((l_sms_str.Contains("SUCCESSFUL") == true) || (l_sms_str.Contains("successful") == true)) && ((l_sms_str.Contains("Transaction number") == true) || (l_sms_str.Contains("TXN number") == true)))
                            {
                                l_status = "Success";
                                status_str = l_status;
                                if (l_sms_str.Contains("Transaction number") == true)
                                {
                                    int l_sms_trans_sop = l_sms_str.IndexOf("Transaction number ");
                                    int l_sms_trans_eop = l_sms_str.IndexOf("\",15");
                                    l_sms_ID = l_sms_str.Substring(l_sms_trans_sop + 18, l_sms_trans_eop - l_sms_trans_sop - 18).Trim();
                                }
                                else if (l_sms_str.Contains("TXN number") == true)
                                {
                                    int l_sms_trans_sop = l_sms_str.IndexOf("TXN number ");
                                    int l_sms_trans_eop = l_sms_str.IndexOf("\",15");
                                    l_sms_ID = l_sms_str.Substring(l_sms_trans_sop + 11, l_sms_trans_eop - l_sms_trans_sop - 11).Trim();
                                }
                                tr_id_str = l_sms_ID;

                                Thread.Sleep(5000);
                                // Update recharge id 
                                string request_url = "http://139.59.18.48/quickload/apis/new_request_message/?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + Form1.g_Topup_ID + "&transaction_id=" + l_sms_ID + "&message=Hello" + "&sms=Hello" + "&status=" + l_status;
                                trim_str = request_url;
                                return_request = WebRequest.Create(request_url);
                                //     return_request = WebRequest.Create("http://139.59.18.48/quickload/apis/new_request_message/?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + "152" + "&transaction_id=" + l_sms_ID + "&message=" + l_sms_str + "&sms=" + l_sms_str + "&status=" + l_status);
                                /*"http:139.59.18.48/quickload/apis/new_request?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + GlobalVars.g_Topup_ID); */
                                string l_resp = func_GetResponse(return_request);

                           //     break;
                                //   GlobalVars.g_ussd_log_sw.WriteLine("Verification Response : " + l_resp);
                                //   txt_Log.AppendText("Reply Response : " + l_resp + "\r\n");
                            }
                            //        if ((l_sms_str.Contains("has been credited") == true) && (l_sms_str.Contains("transfer ID") == true))
                            //        {
                            //            l_status = "Success";
                            //            int l_sms_trans_sop = l_sms_str.IndexOf("transfer ID");
                            //            int l_sms_trans_eop = l_sms_str.IndexOf("has been credited", l_sms_trans_sop + 11);
                            //            l_sms_ID = l_sms_str.Substring(l_sms_trans_sop + 11, l_sms_trans_eop - l_sms_trans_sop - 11).Trim();
                            //            txt_Log.AppendText("SMS TransID : " + l_sms_ID + "\r\n");
                            //            GlobalVars.g_ussd_log_sw.WriteLine("SMS TransID : " + l_sms_ID + "\r\n");
                            //            GlobalVars.g_ussd_log_sw.WriteLine("API : " + "http://139.59.18.48/quickload/apis/new_request_message/?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + GlobalVars.g_Topup_ID + "&transaction_id=" + l_sms_ID + "&message=" + l_sms_str + "&sms=" + l_sms_str + "&status=" + l_status);
                            //            GlobalVars.g_request = WebRequest.Create("http://139.59.18.48/quickload/apis/new_request_message/?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + GlobalVars.g_Topup_ID + "&transaction_id=" + l_sms_ID + "&message=" + l_sms_str + "&sms=" + l_sms_str + "&status=" + l_status);
                            //            /*"http:139.59.18.48/quickload/apis/new_request?userid=opmodem&apikey=20170729190020597c86e4d6bfe2906874467&recharge_id=" + GlobalVars.g_Topup_ID); */
                            //            string l_resp = func_GetResponse();
                            //            GlobalVars.g_ussd_log_sw.WriteLine("Verification Response : " + l_resp);
                            //            txt_Log.AppendText("Reply Response : " + l_resp + "\r\n");
                            //        }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                //GlobalVars.g_ussd_log_sw.WriteLine("Exceptio ProcessAPI : " + ex.Message);
            }
        }

        public static string func_GetResponse(WebRequest request)
        {

            // Get the original response.
            WebResponse response = request.GetResponse();

            string g_status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            Stream g_dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(g_dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            g_dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
}
