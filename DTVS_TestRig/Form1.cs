using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using timers = System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modbus.Device;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static System.Windows.Forms.LinkLabel;
using CsvHelper;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Emit;

namespace DTVS_TestRig
{
	public partial class Form1 : Form
	{
	   // string path = "ReadCoildata.json";
	  //  string output = "ReadCoildata.csv";
		public Form1()
		{
			InitializeComponent();
		}
		System.Timers.Timer timerMain = new System.Timers.Timer();
		ushort[] data;
		ushort[] readcoilval;
		string ipaddress = ConfigurationManager.AppSettings["IPAddress"];
		
		int port = Int32.Parse(ConfigurationManager.AppSettings["port"]);
		string testplanpath;

		// create client instance 
		MqttClient client = new MqttClient("00639yldbx.zohoiothub.com", 8883, true, null, null, MqttSslProtocols.TLSv1_1, null, null);
	  //  MqttClient client = new MqttClient("localhost", 1883, false, null, null, 0, null, null);
		string clientId = "810000000208520";
		string plantID = "401000000205006";
		//string clientId = Guid.NewGuid().ToString();
		string uname = "/00639yldbx.zohoiothub.com/v1/devices/810000000208520/connect";
		string pw = "9ed4d7186c2eba1b448c867b5bb2168468cedcb23fefb4ec48bef6211e884b44";
	   
	

		private void start_Click(object sender, EventArgs e)
		{

			timer1.Interval = 5 * 1000;
			timer1.Enabled = true;
			timerMain.Elapsed += new timers.ElapsedEventHandler(timer1_Tick);
			MqttProcessing();
			//getjsonlist();

			//MqttProcessing();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				TcpClient tcpclient = new TcpClient();
				ModbusIpMaster master = ModbusIpMaster.CreateIp(tcpclient);              
				tcpclient.Connect(ipaddress, port);
				master.WriteSingleRegister(2, 0);
				// data = master.ReadHoldingRegisters(0, 5);
				readcoilval = master.ReadHoldingRegisters(0, 10);
			  //  MessageBox.Show("0-"+readcoilval[0].ToString() + "\n" + "1-" + readcoilval[1].ToString() + "\n"  +"2-"+readcoilval[2].ToString() + "\n" + "3-" + readcoilval[3].ToString() + "\n" + "4-" + readcoilval[4].ToString() + "\n" + "5-" + readcoilval[5].ToString()+ "\n" + "6-" + readcoilval[6].ToString() + "\n" + "7-" + readcoilval[7].ToString() + "\n" + "8-" + readcoilval[8].ToString() + "\n" + "9-" + readcoilval[9].ToString());
				//read test request
			   if (readcoilval[1] ==1)
				  {
					getjsonlist();
					readcoilval[1] = 0;
				  }
				  master.WriteSingleRegister(1, 0);
			}

			catch(Exception ex) 
			{
				throw ex;
			}

		}

		private void button1_Click(object sender, EventArgs e)
		{
			testplanpath = ConfigurationManager.AppSettings["testreqpath"];
			ConvertCsvFileToJsonObject(testplanpath);

		}
	   
		public string ConvertCsvFileToJsonObject(string path)
		{
			var csv = new List<string[]>();
				var lines = File.ReadAllLines(path);
				if (lines.Length > 0)
				{
					MessageBox.Show("File Found & ready to read");
				}
				else
				{
					MessageBox.Show("File Not Found");
				}
				foreach (string line in lines)
					csv.Add(line.Split(','));

				var properties = lines[0].Split(',');

				var listObjResult = new List<Dictionary<string, string>>();
				TestPlan testcl = new TestPlan();
				for (int i = 1; i < lines.Length;)
				{
					var objResult = new Dictionary<string, string>();
					for (int j = 0; j < properties.Length; j++)
						objResult.Add(properties[j], csv[i][j]);

					listObjResult.Add(objResult);
					i++;
				}
				var status = Newtonsoft.Json.JsonConvert.SerializeObject(listObjResult);
				//
				var jsonresult = JsonConvert.SerializeObject(listObjResult);
				//MqttClient client = new MqttClient("localhost", 1883, false, null, null, 0, null, null);
				//string clientId = Guid.NewGuid().ToString();
				//client.Connect(clientId);
				ushort msgId = client.Publish("Result", // topic
				 Encoding.UTF8.GetBytes(jsonresult), // message body
				 MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
				 false);
				return JsonConvert.SerializeObject(listObjResult);
		   // }
		}
	   
		protected void MqttProcessing()
		{
			if (client.IsConnected == true)
			{
			}

			else
			{
				client.Connect(clientId, uname, pw);
			  //    client.Connect(clientId);
			}
			string topic = "/devices/810000000208520/configsettings";
			// register to message received 
			if (client.IsConnected == true)
			{
				client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
				client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
			}
			// subscribe to the topic "/home/temperature" with QoS 2 
		  

		}

		private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			var Session = "";
			Session = Encoding.UTF8.GetString(e.Message);
			jsonStringToCSV(Session);
			TcpClient tcpclient = new TcpClient();
			ModbusIpMaster master = ModbusIpMaster.CreateIp(tcpclient);
			tcpclient.Connect(ipaddress, port);
			master.WriteSingleRegister(2,1);

		}
		/// <summary>
		/// step 1 -Test Request : Read CSV at defined loc,convert to JSON & publish in MQTT
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public void getjsonlist()
		{
			try
			{
				string val = ConfigurationManager.AppSettings["testreqpath"];
				var csv = new List<string[]>();
				var lines = File.ReadAllLines(val);

				foreach (string line in lines)
					csv.Add(line.Split(','));

				var properties = lines[0].Split(',');
				var listObjResult = new List<Dictionary<string, string>>();
				TestClassMaster tcm = new TestClassMaster();
				TestRequest testreq = new TestRequest();
				var customer = lines[0].Split(':');
				var jobcode = lines[1].Split(':');
				var emailid = lines[2].Split(':');
				var mobile = lines[3].Split(':');
				var pump_type = lines[4].Split(':');
				var oem = lines[5].Split(':');
				var model = lines[6].Split(':');
				var no_of_inj = lines[7].Split(':');
				var part_number = lines[8].Split(':');
				var serial_number1 = lines[9].Split(':');
				var serial_number2 = lines[10].Split(':');
				var serial_number3 = lines[11].Split(':');
				var serial_number4 = lines[12].Split(':');
				var Current_I3C_inj1 = lines[13].Split(':');
				var Current_I3C_inj2 = lines[14].Split(':');
				var Current_I3C_inj3 = lines[15].Split(':');
				var Current_I3C_inj4 = lines[16].Split(':');
				var Request_type = lines[17].Split(':');
				//testreq.Customer = customer[1];
				testreq.technician_name = customer[1];
				testreq.job_code = jobcode[1];
				testreq.email_id = emailid[1];
				testreq.mobile = mobile[1];
				testreq.pump_type = pump_type[1];
				testreq.oem = oem[1];
				testreq.model = model[1];
				testreq.no_of_inj = no_of_inj[1];
				testreq.part_number = part_number[1];
				testreq.serial_number = serial_number1[1];
				testreq.serial_number2 = serial_number2[1];
				testreq.serial_number3 = serial_number3[1];
				testreq.serial_number4 = serial_number4[1];
				testreq.Current_I3C_inj1 = Current_I3C_inj1[1];
				testreq.Current_I3C_inj2 = Current_I3C_inj2[1];
				testreq.Current_I3C_inj3 = Current_I3C_inj3[1];
				testreq.Current_I3C_inj4 = Current_I3C_inj4[1];
				testreq.request_type = Request_type[1];
				//testreq.device_id = objResult.FirstOrDefault(kv => kv.Key == "device_id").Value;
				testreq.device_id = clientId;
				List<JsonSerializerSettings> settings = new List<JsonSerializerSettings>();
				var retVal = JsonConvert.SerializeObject(testreq);
				if (client.IsConnected == true)
				{
				}

				else
				{
					 client.Connect(clientId, uname, pw);
				   // client.Connect(clientId);
				}
				string topic = "/devices/810000000208520/telemetry";
				ushort msgId = client.Publish(topic, // topic
				 Encoding.UTF8.GetBytes(retVal), // message body

				 MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, // QoS level
				 false);

				//settings.NullValueHandling = NullValueHandling.Ignore;
				// return JsonConvert.SerializeObject(testreq);

				//if (File.Exists(val))
				//{
				//	File.Delete(val);
				//}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}


		/// <summary>
		/// Step 2 - Test Plan : Get Json from cloud through MQTT Subscription & convert to csv at defined location
		/// </summary>
		/// <param name="jsonContent"></param>
		public void jsonStringToCSV(string jsonContent)
		{
			var serializer = new JavaScriptSerializer();
			try
			{
               
                //for injector test plan
                string data = JsonConvert.SerializeObject(jsonContent);
				string data1 = data.Substring(76, 6);
				string[] data2 = data1.Split('\"');

				if (data2[0].Contains("Inje"))
                {
                    string val = ConfigurationManager.AppSettings["testreqsave"];
                    if (File.Exists(val))
                    {
                        File.Delete(val);
                    }
                    List<PayloadInjector> rp = JsonConvert.DeserializeObject<List<PayloadInjector>>(jsonContent);
					//string requesttype = rp[0].payload[0].test.testplans.Part_data.request_type;
					string testplanno = "Test Plan No :" + rp[0].payload[0].test.testplans.Part_data.Test_Plan_No;
					addline(testplanno);
					string oem = "OEM :" + rp[0].payload[0].test.testplans.Part_data.OEM;
					addline(oem);
					string partno = "Part No :" + rp[0].payload[0].test.testplans.Part_data.Part_Number;
					addline(partno);
					string name = "Name :" + rp[0].payload[0].test.testplans.Part_data.Name;
					addline(name);
					string slno = "Sl No"; addline1(slno);
					string testphase = "Test Phase"; addline1(testphase);
					string testdesc = "Test Description"; addline1(testdesc);
					string stepno = "Step No"; addline1(stepno);
					string speed = "Speed"; addline1(speed);
					string tolrpm = "Tolerance (rpm)"; addline1(tolrpm);
					string railpres = "Rail Pressure"; addline1(railpres);
					string tolbar = "Tol(bar)-RL"; addline1(tolbar);
					string duration = "Duration"; addline1(duration);
					string pulsemin = "Pulse(Min)"; addline1(pulsemin);
					string pulsemax = "Pulse(Max)"; addline1(pulsemax);
					string increment = "Increment"; addline1(increment);
					string freq = "Freq"; addline1(freq);
					string shots = "Shots"; addline1(shots);
					string resistance = "Resistance"; addline1(resistance);
					string tolerence_ohm = "Tolerence(ohm)"; addline1(tolerence_ohm);
					string inductance = "Inductance"; addline1(inductance);
					string tolerence_h = "Tolerence(H)"; addline1(tolerence_h);
					string delmin = "Delivery Flow(Min)"; addline1(delmin);
					string delmax = "Delivery Flow(Max)"; addline1(delmax);
					string deloff = "DelFlow OffSet"; addline1(deloff);
					string inletpres = "Inlet Pressure"; addline1(inletpres);
					string tolbaril = "Tol(bar)IL"; addline1(tolbaril);
					string interval = "Interval"; addline1(interval);
					string blmin = "B/L Flow(Min)"; addline1(blmin);
					string blmax = "B/L Flow(Max)"; addline1(blmax);
					string bloff = "B/L Flow OffSet"; addline1(bloff);
					string ilmin = "I/L Flow(Min)"; addline1(ilmin);
					string ilmax = "I/L Flow(Max)"; addline1(ilmax);
					string iloff = "I/L Flow OffSet"; addline1(iloff);
					string imvcurr = "IMV Current"; addline1(imvcurr);
					string tolma = "Tolerance (mA)"; addline1(tolma);
					string ramptime = "Ramp Time"; addline1(ramptime);
					string sampleavg = "Sample AVG"; addline1(sampleavg);
					string sampint = "Sample Interval"; addline1(sampint);
					for (int i = 0; i < rp[0].payload[0].test.testplans.test_plan.Count; i++)
					{  // foreach (var r in rp[0].payload[0].test.testplans.test_plan)// {
						string stepnoval = rp[0].payload[0].test.testplans.test_plan[i].Step_No;
						string slnoval = "\n" + stepnoval; addline1(slnoval);
						string testphaseval = rp[0].payload[0].test.testplans.test_plan[i].Test_Phase; addline1(testphaseval);
						string testdescval = rp[0].payload[0].test.testplans.test_plan[i].Test_Description; addline1(testdescval);
						addline1(stepnoval);
						string speedval = rp[0].payload[0].test.testplans.test_plan[i].Speed; addline1(speedval);
						string tolrpmval = rp[0].payload[0].test.testplans.test_plan[i].Tolerance_rpm; addline1(tolrpmval);
						string railpresval = rp[0].payload[0].test.testplans.test_plan[i].Rail_Pressure; addline1(railpresval);
						string tolbarval = rp[0].payload[0].test.testplans.test_plan[i].Tolerance_bar_RL; addline1(tolbarval);
						string durationval = rp[0].payload[0].test.testplans.test_plan[i].Duration; addline1(durationval);
						string pulseminval = rp[0].payload[0].test.testplans.test_plan[i].Pulse_Min; addline1(pulseminval);
						string pulsemaxval = rp[0].payload[0].test.testplans.test_plan[i].Pulse_Max; addline1(pulsemaxval);
						string incrementval = rp[0].payload[0].test.testplans.test_plan[i].Increment; addline1(incrementval);
						string freqval = rp[0].payload[0].test.testplans.test_plan[i].Frequency; addline1(freqval);
						string shotsval = rp[0].payload[0].test.testplans.test_plan[i].Shots; addline1(shotsval);
						string resistanceval = rp[0].payload[0].test.testplans.test_plan[i].Resistance; addline1(resistanceval);
						string tolerenceohmval = rp[0].payload[0].test.testplans.test_plan[i].Tolerance_ohm; addline1(tolerenceohmval);
						string inductanceval = rp[0].payload[0].test.testplans.test_plan[i].Inductance; addline1(inductanceval);
						string tolerencehval = rp[0].payload[0].test.testplans.test_plan[i].Tolerance_H; addline1(tolerencehval);
						string delminval = rp[0].payload[0].test.testplans.test_plan[i].Delivery_Flow_Min; addline1(delminval);
						string delmaxval = rp[0].payload[0].test.testplans.test_plan[i].Delivery_Flow_Max; addline1(delmaxval);
						string deloffval = rp[0].payload[0].test.testplans.test_plan[i].DelFlow_OffSet; addline1(deloffval);
						string inletpressureval = rp[0].payload[0].test.testplans.test_plan[i].Inlet_Pressure; addline1(inletpressureval);
						string tolbarilval = rp[0].payload[0].test.testplans.test_plan[i].Tol_bar_IL; addline1(tolbarilval);
						string intervalval = rp[0].payload[0].test.testplans.test_plan[i].Interval; addline1(intervalval);
						string blminval = rp[0].payload[0].test.testplans.test_plan[i].BL_Flow_Min; addline1(blminval);
						string blmaxval = rp[0].payload[0].test.testplans.test_plan[i].BL_Flow_Max; addline1(blmaxval);
						string bloffval = rp[0].payload[0].test.testplans.test_plan[i].BLFlow_OffSet; addline1(bloffval);
						string ilminval = rp[0].payload[0].test.testplans.test_plan[i].IL_Flow_Min; addline1(ilminval);
						string ilmaxval = rp[0].payload[0].test.testplans.test_plan[i].IL_Flow_Max; addline1(ilmaxval);
						string iloffval = rp[0].payload[0].test.testplans.test_plan[i].ILFlow_OffSet; addline1(iloffval);
						string imvcurrval = rp[0].payload[0].test.testplans.test_plan[i].IMV_Current; addline1(imvcurrval);
						string tolmaval = rp[0].payload[0].test.testplans.test_plan[i].Tolerance_mA; addline1(tolmaval);
						string ramptimeval = rp[0].payload[0].test.testplans.test_plan[i].Ramp_Time; addline1(ramptimeval);
						string sampleavgval = rp[0].payload[0].test.testplans.test_plan[i].Sample_AVG; addline1(sampleavgval);
						string sampintval = rp[0].payload[0].test.testplans.test_plan[i].Sample_Interval; addline1(sampintval); // }
					}
					string alarmconfig = "\n" + "Alarm Configurations";
					addline(alarmconfig);
					string ilpremin = "IL Pressure Min, " + rp[0].payload[0].test.testplans.alarm_configuration.il_pressure_min;
					addline(ilpremin);
					string ilpresmax = "IL Pressure Max ," + rp[0].payload[0].test.testplans.alarm_configuration.il_pressure_max;
					addline(ilpresmax);
					string blpresmin = "BL Pressure Min, " + rp[0].payload[0].test.testplans.alarm_configuration.bl_pressure_min;
					addline(blpresmin);
					string blpresmax = "BL Pressure Max, " + rp[0].payload[0].test.testplans.alarm_configuration.bl_pressure_max;
					addline(blpresmax);
					string iltempmin = "IL Temp Min, " + rp[0].payload[0].test.testplans.alarm_configuration.il_temp_min;
					addline(iltempmin);
					string iltempmax = "IL Temp Max, " + rp[0].payload[0].test.testplans.alarm_configuration.il_temp_max;
					addline(iltempmax);
					string bltempmin = "BL Temp Min ," + rp[0].payload[0].test.testplans.alarm_configuration.bl_temp_min;
					addline(bltempmin);
					string bltempmax = "BL Temp Max ," + rp[0].payload[0].test.testplans.alarm_configuration.bl_temp_max;
					addline(bltempmax);
					string cwtempmin = "CW Temp Min, " + rp[0].payload[0].test.testplans.alarm_configuration.cw_temp_min;
					addline(cwtempmin);
					string cwtempmax = "CW Temp Max ," + rp[0].payload[0].test.testplans.alarm_configuration.cw_temp_max;
					addline(cwtempmax);
					string dltempmin = "DL Temp Min ," + rp[0].payload[0].test.testplans.alarm_configuration.dl_temp_min;
					addline(dltempmin);
					string dltempmax = "DL Temp Max, " + rp[0].payload[0].test.testplans.alarm_configuration.dl_temp_max;
					addline(dltempmax);
					string dttempmin = "DT Temp Min ," + rp[0].payload[0].test.testplans.alarm_configuration.dt_temp_min;
					addline(dttempmin);
					string dttempmax = "DT Temp Max, " + rp[0].payload[0].test.testplans.alarm_configuration.dt_temp_max;
					addline(dttempmax);
					string tpmin = "TP Min, " + rp[0].payload[0].test.testplans.alarm_configuration.tp_min;
					addline(tpmin);
					string tpmax = "TP Max, " + rp[0].payload[0].test.testplans.alarm_configuration.tp_max;
					addline(tpmax);
					string totempmin = "TO Temp Min ," + rp[0].payload[0].test.testplans.alarm_configuration.to_temp_min;
					addline(totempmin);
					string totempmax = "TO Temp Max, " + rp[0].payload[0].test.testplans.alarm_configuration.to_temp_max;
					addline(totempmax);
					string rotation = "Rotation ," + rp[0].payload[0].test.testplans.alarm_configuration.rotation;
					addline(rotation);
				}
				else if (data2[0].Contains("Pump"))
				{
                    string val = ConfigurationManager.AppSettings["testreqsave"];
                    if (File.Exists(val))
                    {
                        File.Delete(val);
                    }
                    //for pump testplan
                    List<Payload> pl = JsonConvert.DeserializeObject<List<Payload>>(jsonContent);
				//list<payload> rp = jsonconvert.deserializeobject<list<payload>>(jsoncontent);
				string testplanno = "Test Plan No :" + pl[0].payload[0].test.testplans.Part_data.Test_Plan_No;
				addline(testplanno);
				string oem = "OEM :" + pl[0].payload[0].test.testplans.Part_data.OEM;
				addline(oem);
				string partno = "Part No :" + pl[0].payload[0].test.testplans.Part_data.Part_Number;
				addline(partno);
				string name = "Name :" + pl[0].payload[0].test.testplans.Part_data.Name;
				addline(name);
				string slno = "Sl No"; addline1(slno);
				string testphase = "Test Phase"; addline1(testphase);
				string testdesc = "Test Description"; addline1(testdesc);
				string stepno = "Step No"; addline1(stepno);
				string speed = "Speed"; addline1(speed);
				string tolrpm = "Tolerance (rpm)"; addline1(tolrpm);
				string railpres = "Rail Pressure"; addline1(railpres);
				string tolbar = "Tol(bar)"; addline1(tolbar);
				string duration = "Duration"; addline1(duration);
				string sampleavg = "Sample AVG"; addline1(sampleavg);
				string sampint = "Sample Interval"; addline1(sampint);
				string imvcurr = "IMV Current"; addline1(imvcurr);
				string tolma = "Tolerance (mA)"; addline1(tolma);
				string ramptime = "Ramp Time"; addline1(ramptime);
				string delmin = "Delivery Flow(min)"; addline1(delmin);
				string delmax = "Delivery Flow(max)"; addline1(delmax);
				string deloff = "Delflow Offset"; addline1(deloff);
				string blmin = "B/L Flow(min)"; addline1(blmin);
				string blmax = "B/L Flow(max)"; addline1(blmax);
				string bloff = "B/L Flow offset"; addline1(bloff);
				string ilmin = "I/L Flow(min)"; addline1(ilmin);
				string ilmax = "I/L Flow(max)"; addline1(ilmax);
				string iloff = "I/L Flow Offset"; addline1(iloff);
				string venmin = "Ventpres(min)"; addline1(venmin);
				string venmax = "Venpres(max)"; addline1(venmax);
				for (int i = 0; i < pl[0].payload[0].test.testplans.test_plan.Count; i++)
				{  // foreach (var r in rp[0].payload[0].test.testplans.test_plan)// {
					string stepnoval = pl[0].payload[0].test.testplans.test_plan[i].StepNo;
					string slnoval = "\n" + stepnoval; addline1(slnoval);
					string testphaseval = pl[0].payload[0].test.testplans.test_plan[i].Test_Phase; addline1(testphaseval);
					string testdescval = pl[0].payload[0].test.testplans.test_plan[i].Test_Description; addline1(testdescval);
					addline1(stepnoval);
					string speedval = pl[0].payload[0].test.testplans.test_plan[i].Speed; addline1(speedval);
					string tolrpmval = pl[0].payload[0].test.testplans.test_plan[i].Tolerance_rpm; addline1(tolrpmval);
					string railpresval = pl[0].payload[0].test.testplans.test_plan[i].Rail_Pressure; addline1(railpresval);
					string tolbarval = pl[0].payload[0].test.testplans.test_plan[i].Tolerance_bar; addline1(tolbarval);
					string durationval = pl[0].payload[0].test.testplans.test_plan[i].Duration; addline1(durationval);
					string sampleavgval = pl[0].payload[0].test.testplans.test_plan[i].Sample_AVG; addline1(sampleavgval);
					string sampintval = pl[0].payload[0].test.testplans.test_plan[i].Sample_Interval; addline1(sampintval);
					string imvcurrval = pl[0].payload[0].test.testplans.test_plan[i].IMV_Current; addline1(imvcurrval);
					string tolmaval = pl[0].payload[0].test.testplans.test_plan[i].Tolerance_mA; addline1(tolmaval);
					string ramptimeval = pl[0].payload[0].test.testplans.test_plan[i].Ramp_Time; addline1(ramptimeval);
					string delminval = pl[0].payload[0].test.testplans.test_plan[i].Delivery_Flow_Min; addline1(delminval);
					string delmaxval = pl[0].payload[0].test.testplans.test_plan[i].Delivery_Flow_Max; addline1(delmaxval);
					string deloffval = pl[0].payload[0].test.testplans.test_plan[i].DelFlow_OffSet; addline1(deloffval);
					string blminval = pl[0].payload[0].test.testplans.test_plan[i].BL_Flow_Min; addline1(blminval);
					string blmaxval = pl[0].payload[0].test.testplans.test_plan[i].BL_Flow_Max; addline1(blmaxval);
					string bloffval = pl[0].payload[0].test.testplans.test_plan[i].BL_Flow_OffSet; addline1(bloffval);
					string ilminval = pl[0].payload[0].test.testplans.test_plan[i].IL_Flow_Min; addline1(ilminval);
					string ilmaxval = pl[0].payload[0].test.testplans.test_plan[i].IL_Flow_Max; addline1(ilmaxval);
					string iloffval = pl[0].payload[0].test.testplans.test_plan[i].IL_Flow_OffSet; addline1(iloffval);
					string venminval = pl[0].payload[0].test.testplans.test_plan[i].VentPres_Min; addline1(venminval);
					string venmaxval = pl[0].payload[0].test.testplans.test_plan[i].VenPres_Max; addline1(venmaxval);  // }
				}
				string alarmconfig = "\n" + "Alarm Configurations";
				addline(alarmconfig);
				string ilpremin = "IL Pressure Min, " + pl[0].payload[0].test.testplans.alarm_configuration.il_pressure_min;
				addline(ilpremin);
				string ilpresmax = "IL Pressure Max, " + pl[0].payload[0].test.testplans.alarm_configuration.il_pressure_max;
				addline(ilpresmax);
				string blpresmin = "BL Pressure Min, " + pl[0].payload[0].test.testplans.alarm_configuration.bl_pressure_min;
				addline(blpresmin);
				string blpresmax = "BL Pressure Max, " + pl[0].payload[0].test.testplans.alarm_configuration.bl_pressure_max;
				addline(blpresmax);
				string iltempmin = "IL Temp Min, " + pl[0].payload[0].test.testplans.alarm_configuration.il_temp_min;
				addline(iltempmin);
				string iltempmax = "IL Temp Max, " + pl[0].payload[0].test.testplans.alarm_configuration.il_temp_max;
				addline(iltempmax);
				string bltempmin = "BL Temp Min, " + pl[0].payload[0].test.testplans.alarm_configuration.bl_temp_min;
				addline(bltempmin);
				string bltempmax = "BL Temp Max, " + pl[0].payload[0].test.testplans.alarm_configuration.bl_temp_max;
				addline(bltempmax);
				string cwtempmin = "CW Temp Min, " + pl[0].payload[0].test.testplans.alarm_configuration.cw_temp_min;
				addline(cwtempmin);
				string cwtempmax = "CW Temp Max, " + pl[0].payload[0].test.testplans.alarm_configuration.cw_temp_max;
				addline(cwtempmax);
				string dltempmin = "DL Temp Min, " + pl[0].payload[0].test.testplans.alarm_configuration.dl_temp_min;
				addline(dltempmin);
				string dltempmax = "DL Temp Max, " + pl[0].payload[0].test.testplans.alarm_configuration.dl_temp_max;
				addline(dltempmax);
				string dttempmin = "DT Temp Min, " + pl[0].payload[0].test.testplans.alarm_configuration.dt_temp_min;
				addline(dttempmin);
				string dttempmax = "DT Temp Max, " + pl[0].payload[0].test.testplans.alarm_configuration.dt_temp_max;
				addline(dttempmax);
				string tpmin = "TP Min, " + pl[0].payload[0].test.testplans.alarm_configuration.tp_min;
				addline(tpmin);
				string tpmax = "TP Max, " + pl[0].payload[0].test.testplans.alarm_configuration.tp_max;
				addline(tpmax);
				string totempmin = "TO Temp Min, " + pl[0].payload[0].test.testplans.alarm_configuration.to_temp_min;
				addline(totempmin);
				string totempmax = "TO Temp Max, " + pl[0].payload[0].test.testplans.alarm_configuration.to_temp_max;
				addline(totempmax);
				string rotation = "Rotation, " + pl[0].payload[0].test.testplans.alarm_configuration.rotation;
				addline(rotation);
			}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}


		/// <summary>
		/// step 3 - Test Result :  Read CSV at defined loc,convert to JSON & publish in MQTT
		/// </summary>
		public void csvtojsonandpublishmqtt()
		{
//for injector test result
			string val = ConfigurationManager.AppSettings["testresultpath"];
			var csv = new List<string[]>();
			var lines = File.ReadAllLines(val);
			//  var csv = from line in lines  select (line.Split(',')).ToArray();
			foreach (string line in lines)
				csv.Add(line.Split(','));
			var properties = lines[16].Split(',');
			var propertiesdata = lines[0].Split(':');
			var listObjResult = new List<Dictionary<string, string>>();
			TestResultInjector resultInjector = new TestResultInjector();
			List<TestResultDataInjector> datalist = new List<TestResultDataInjector>();
			var customerprop = lines[0].Split(':');
			var customername = customerprop[1].Trim(',');
			var jobcodeprop = lines[1].Split(':');
			var jobcode = jobcodeprop[1].Trim(',');
			var emailidprop = lines[2].Split(':');
			var emailid = emailidprop[1].Trim(',');
			var mobilenoprop = lines[3].Split(':');
			var mobileno = mobilenoprop[1].Trim(',');
			var oemprop = lines[4].Split(':');
			var oem = oemprop[1].Trim(',');
			var vehiclemodelprop = lines[5].Split(':');
			var vehiclemodel = vehiclemodelprop[1].Trim(',');
			var partnoprop = lines[6].Split(':');
			var partno = partnoprop[1].Trim(',');
			var injectortypeprop = lines[7].Split(':');
			var injectortype = injectortypeprop[1].Trim(',');
			var noofinjectorprop = lines[8].Split(':');
			var noofinjector = noofinjectorprop[1].Trim(',');
			var existinginjectorcodeprop = lines[9].Split(':');
			var existinginjectorcode = existinginjectorcodeprop[1].Trim(',');
			var injector1slnoprop= lines[10].Split(':');
			var injector1slno = injector1slnoprop[1].Trim(',');
			var injector2slnoprop = lines[11].Split(':');
			var injector2slno = injector2slnoprop[1].Trim(',');
			var injector3slnoprop = lines[12].Split(':');
			var injector3slno = injector3slnoprop[1].Trim(',');
			var injector4slnoprop = lines[13].Split(':');
			var injector4slno = injector4slnoprop[1].Trim(',');
			var resultprop = lines[14].Split(':');
			var result = resultprop[1].Trim(',');
			var staprop = lines[15].Split(':');
			var sta = staprop[1].Trim(',');
			resultInjector.customerDetails.CustomerName = customername;
			resultInjector.customerDetails.JobCode = jobcode;
			resultInjector.customerDetails.EmailID = emailid;
			resultInjector.customerDetails.MobileNo = mobileno;
			resultInjector.customerDetails.OEM = oem;
			resultInjector.customerDetails.VehicleModel = vehiclemodel;
			resultInjector.partDetails.PartNumber = partno;
			resultInjector.partDetails.InjectorType = injectortype;
			resultInjector.partDetails.NoofInjector = noofinjector;
			resultInjector.partDetails.ExistingInjectorCode = existinginjectorcode;
			resultInjector.partDetails.Injector1SlNo = injector1slno;
			resultInjector.partDetails.Injector2SlNo = injector2slno;
			resultInjector.partDetails.Injector3SlNo = injector3slno;
			resultInjector.partDetails.Injector4SlNo = injector4slno;
			resultInjector.status.Result = result;
			resultInjector.status.Status = sta;
			for (int i = 17; i < lines.Length;)
			{
				var objResult = new Dictionary<string, string>();
				TestResultDataInjector data = new TestResultDataInjector();
				// var so = objResult.TryGetValue("SlNo", out sn);
				for (int j = 0; j < properties.Length; j++)
					objResult.Add(properties[j], csv[i][j]);
				listObjResult.Add(objResult);
				var slno = objResult.FirstOrDefault(kv => kv.Key == "SlNo").Value;
				var stepno = objResult.FirstOrDefault(kv => kv.Key == "StepNo").Value;
				var testphase = objResult.FirstOrDefault(kv => kv.Key == "TestPhase").Value;
				var testdescription = objResult.FirstOrDefault(kv => kv.Key == "Test Description").Value;
				var speed = objResult.FirstOrDefault(kv => kv.Key == "Speed").Value;
				var railpressure = objResult.FirstOrDefault(kv => kv.Key == "RailPressure").Value;
				var railpressurefb = objResult.FirstOrDefault(kv => kv.Key == "RailPressure(FeedBack)").Value;
				var inlettemp = objResult.FirstOrDefault(kv => kv.Key == "Inlet Temp").Value;
				var pulse = objResult.FirstOrDefault(kv => kv.Key == "Pulse").Value;
				var saclelen = objResult.FirstOrDefault(kv => kv.Key == "ScaleLength").Value;
				var vol = objResult.FirstOrDefault(kv => kv.Key == "Volume").Value;
				var logdate = objResult.FirstOrDefault(kv => kv.Key == "LogDate").Value;
				data.Slno = slno;
				data.StepNo = stepno;
				data.TestPhase = testphase;
				data.TestDescription = testdescription;
				data.Speed = speed;
				data.RailPressure = railpressure;
				data.RailPressure_FeedBack = railpressurefb;
				data.InletTemp = inlettemp;
				data.Pulse = pulse;
				data.ScaleLength = saclelen;
				data.Volume = vol;
				data.LogDate = logdate;
				datalist.Add(data);
				i++;
			}



			//for Pump Test Result 
			//string val = ConfigurationManager.AppSettings["testresultpath"];
			//var csv = new List<string[]>();
			//var lines = File.ReadAllLines(val);
			////  var csv = from line in lines  select (line.Split(',')).ToArray();
			//foreach (string line in lines)
			//    csv.Add(line.Split(','));
			//var properties = lines[11].Split(',');
			//var propertiesdata = lines[0].Split(':');
			//var listObjResult = new List<Dictionary<string, string>>();
			//TestResultPump resultPump = new TestResultPump();
			//List<TestResultDataPump> datalist = new List<TestResultDataPump>();
			//var customerprop = lines[0].Split(':');
			//var customername = customerprop[1].Trim(',');
			//var jobcodeprop = lines[1].Split(':');
			//var jobcode = jobcodeprop[1].Trim(',');
			//var emailidprop = lines[2].Split(':');
			//var emailid = emailidprop[1].Trim(',');
			//var mobilenoprop = lines[3].Split(':');
			//var mobileno = mobilenoprop[1].Trim(',');
			//var oemprop = lines[4].Split(':');
			//var oem = oemprop[1].Trim(',');
			//var vehiclemodelprop = lines[5].Split(':');
			//var vehiclemodel = vehiclemodelprop[1].Trim(',');
			//var partnoprop = lines[6].Split(':');
			//var partno = partnoprop[1].Trim(',');
			//var pumptypeprop = lines[7].Split(':');
			//var pumptype = pumptypeprop[1].Trim(',');
			//var pumpserialnoprop = lines[8].Split(':');
			//var pumpserialno = pumpserialnoprop[1].Trim(',');
			//var resultprop = lines[9].Split(':');
			//var result = resultprop[1].Trim(',');
			//var staprop = lines[10].Split(':');
			//var sta = staprop[1].Trim(',');
			//resultPump.customerDetails.CustomerName = customername;
			//resultPump.customerDetails.JobCode = jobcode;
			//resultPump.customerDetails.EmailID = emailid;
			//resultPump.customerDetails.MobileNo = mobileno;
			//resultPump.customerDetails.OEM = oem;
			//resultPump.customerDetails.VehicleModel = vehiclemodel;
			//resultPump.partDetails.PartNumber = partno;
			//resultPump.partDetails.PumpType = pumptype;
			//resultPump.partDetails.PumpSerialNo = pumpserialno;
			//resultPump.status.Result = result;
			//resultPump.status.Status = sta;
			//for (int i = 12; i < lines.Length;)
			//{
			//    var objResult = new Dictionary<string, string>();
			//    TestResultDataPump data = new TestResultDataPump();
			//    // var so = objResult.TryGetValue("SlNo", out sn);
			//    for (int j = 0; j < properties.Length; j++)
			//        objResult.Add(properties[j], csv[i][j]);
			//    listObjResult.Add(objResult);
			//    var slno = objResult.FirstOrDefault(kv => kv.Key == "SlNo").Value;
			//    var stepno = objResult.FirstOrDefault(kv => kv.Key == "StepNo").Value;
			//    var testphase = objResult.FirstOrDefault(kv => kv.Key == "TestPhase").Value;
			//    var testdescription = objResult.FirstOrDefault(kv => kv.Key == "Test Description").Value;
			//    var speed = objResult.FirstOrDefault(kv => kv.Key == "Speed").Value;
			//    var railpressure = objResult.FirstOrDefault(kv => kv.Key == "RailPressure").Value;
			//    var imvcurrent = objResult.FirstOrDefault(kv => kv.Key == "IMVCurrent").Value;
			//    var inletpressure = objResult.FirstOrDefault(kv => kv.Key == "InletPressure").Value;
			//    var inletflow = objResult.FirstOrDefault(kv => kv.Key == "InletFlow").Value;
			//    var inlettemp = objResult.FirstOrDefault(kv => kv.Key == "InletTemp").Value;
			//    var blpressure = objResult.FirstOrDefault(kv => kv.Key == "BLPressure").Value;
			//    var blflow = objResult.FirstOrDefault(kv => kv.Key == "BLFlow").Value;
			//    var bltemp = objResult.FirstOrDefault(kv => kv.Key == "BLTemp").Value;
			//    var deliveryflow = objResult.FirstOrDefault(kv => kv.Key == "DeliveryFlow").Value;
			//    var deliverytemp = objResult.FirstOrDefault(kv => kv.Key == "DeliveryTemp").Value;
			//    var transpressure = objResult.FirstOrDefault(kv => kv.Key == "TransPressure").Value;
			//    var venturipressure = objResult.FirstOrDefault(kv => kv.Key == "VenturiPressure").Value;
			//    var lubeoilpressure = objResult.FirstOrDefault(kv => kv.Key == "LubeOilPressure").Value;
			//    var lubeoildtemp = objResult.FirstOrDefault(kv => kv.Key == "LubeoildTemp").Value;
			//    var skintemp = objResult.FirstOrDefault(kv => kv.Key == "SkinTemp").Value;
			//    var hpvcurrent = objResult.FirstOrDefault(kv => kv.Key == "HPVCurrent").Value;
			//    var logdate = objResult.FirstOrDefault(kv => kv.Key == "LogDate").Value;
			//    data.Slno = slno;
			//    data.StepNo = stepno;
			//    data.TestPhase = testphase;
			//    data.TestDescription = testdescription;
			//    data.Speed = speed;
			//    data.RailPressure = railpressure;
			//    data.IMVCurrent = imvcurrent;
			//    data.InletPressure = inletpressure;
			//    data.InletFlow = inletflow;
			//    data.InletTemp = inlettemp;
			//    data.BLPressure = blpressure;
			//    data.BLFlow = blflow;
			//    data.BLTemp = bltemp;
			//    data.DeliveryFlow = deliveryflow;
			//    data.DeliveryTemp = deliverytemp;
			//    data.TransPressure = transpressure;
			//    data.VenturiPressure = venturipressure;
			//    data.LubeOilPressure = lubeoilpressure;
			//    data.LubeoildTemp = lubeoildtemp;
			//    data.SkinTemp = skintemp;
			//    data.HPVCurrent = hpvcurrent;
			//    data.LogDate = logdate;
			//    datalist.Add(data);
			//    i++;
			//}
			//resultPump.dataPump.AddRange(datalist);
			////   testreq.device_id = objResult.FirstOrDefault(kv => kv.Key == "device_id").Value;   
			//var status = Newtonsoft.Json.JsonConvert.SerializeObject(resultPump);



			resultInjector.dataPump.AddRange(datalist);
			var status = Newtonsoft.Json.JsonConvert.SerializeObject(resultInjector);
			if (client.IsConnected == true)
			{
			}

			else
			{
				// client.Connect(clientId, uname, pw);
				client.Connect(clientId);
			}
			string topic = "/devices/810000000208520/telemetry";
			ushort msgId = client.Publish(topic, // topic
			 Encoding.UTF8.GetBytes(status), // message body

			 MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, // QoS level
			 false);

		}
		public static void addline1(string s)
		{
			string val = ConfigurationManager.AppSettings["testreqsave"];
			if (!File.Exists(val))
			{
				File.Create(val).Dispose();
			}
			s = s + ",";
			File.AppendAllText(val, s);
		}
		public static void addline(string s)
		{
			string val = ConfigurationManager.AppSettings["testreqsave"];
			if (!File.Exists(val))
			{
				File.Create(val).Dispose();
			}
			s = s + "\n";
			File.AppendAllText(val, s);
		}
		private void btn_Click_Click(object sender, EventArgs e)
		{
			// this.timer1_Tick(sender,e);
			//getjsonlist();
			var val = "";
			jsonStringToCSV(val);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//timer1.Interval = 5 * 1000;
			//timer1.Enabled = true;
			//timerMain.Elapsed += new timers.ElapsedEventHandler(timer1_Tick);
		   
		}

		private void button2_Click(object sender, EventArgs e)
		{
			csvtojsonandpublishmqtt();
			//string val = ConfigurationManager.AppSettings["testreqpath"];
			//using (StreamReader r = new StreamReader(val))
			//{
			//    string json = r.ReadToEnd();
			//    //  var data = JsonConvert.DeserializeObject<Payload>(json);
			//    jsonStringToCSV(json);

			//}
			// // getjsonlist();
		}
	  
	}
}
 