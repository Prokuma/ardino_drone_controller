using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using Xamarin.Forms;

namespace DroneController
{
	public class App : Application
	{

		TcpClient client;

		public IPPage ipPage;
		public ControllerPage controllerPage;
		NavigationPage navigationPage;

		int throttleValue = 0;
		int rollValue = 50;
		int pitchValue = 50;
		int yawValue = 50;

		public App ()
		{
			ipPage = new IPPage ();
			controllerPage = new ControllerPage ();
			navigationPage = new NavigationPage (ipPage);
			navigationPage.BarBackgroundColor = new Color (0, 192.0/255.0, 255/255);
			navigationPage.BarTextColor = new Color (1, 1, 1);
			MainPage = navigationPage;
			ipPage.connectButtonO.Clicked += OnConnectClicked;

			controllerPage.throttleSliderO.ValueChanged += OnThrottleValueChanged;
			controllerPage.rollSliderO.ValueChanged += OnRollValueChanged;
			controllerPage.pitchSliderO.ValueChanged += OnPitchValueChanged;
			controllerPage.yawSliderO.ValueChanged += OnYawValueChanged;

			controllerPage.rollZeroButtonO.Clicked += OnRollZeroClicked;
			controllerPage.pitchZeroButtonO.Clicked += OnPitchZeroClicked;
			controllerPage.yawZeroButtonO.Clicked += OnYawZeroClicked;
		}

		private void OnThrottleValueChanged(object sender,  ValueChangedEventArgs e){throttleValue = (int)e.NewValue;SendData();}
		private void OnRollValueChanged(object sender,  ValueChangedEventArgs e){rollValue = (int)e.NewValue ;SendData ();}
		private void OnPitchValueChanged(object sender,  ValueChangedEventArgs e){pitchValue = (int)e.NewValue ;SendData ();}
		private void OnYawValueChanged(object sender,  ValueChangedEventArgs e){yawValue = (int)e.NewValue ;SendData ();}
		private void OnRollZeroClicked(object sender, EventArgs e){rollValue = 50;controllerPage.rollSliderO.Value = 50;SendData();}
		private void OnPitchZeroClicked(object sender, EventArgs e){pitchValue = 50;controllerPage.pitchSliderO.Value = 50;SendData();}
		private void OnYawZeroClicked(object sender, EventArgs e){yawValue = 50;controllerPage.yawSliderO.Value = 50;SendData();}

		protected override void OnStart () {
			// Handle when your app starts
		}

		protected override void OnSleep () {
			//client.Close();
		}

		protected override void OnResume () {
			//changeScene ();
		}

		async void changeScene(){
			bool isSuccess = ConnectToDrone(ipPage.ipEntry.Text);
			if (isSuccess) {
				
				await navigationPage.PushAsync (controllerPage);
			} else {
				await navigationPage.PushAsync (ipPage);
				ipPage.stateLabelO.Text = "Connect Error!";
			}
		}

		async void OnConnectClicked(object sender, EventArgs e){
			bool isSuccess = ConnectToDrone(ipPage.ipEntry.Text);
			if (isSuccess) {
				await navigationPage.PushAsync (controllerPage);
			} else {
				ipPage.stateLabelO.Text = "Connect Error!";
			}
		}

		bool ConnectToDrone(String IP){
			try{
				IPEndPoint ipEndPoint = new IPEndPoint (IPAddress.Parse (IP), 23);
				try{
					client = TimeOutSocket.Connect(ipEndPoint, 3000);
					return true;
				}catch{
					return false;
				}
			}catch{
				return false;
			}
		}

		/* Signal of Control
		 * T is Throttle
		 * R is Roll
		 * P is Pitch
		 * Y is Yaw
		 */
		void SendData(){
			byte[] data = {(byte)'T',(byte)throttleValue, (byte)'R', (byte)rollValue, (byte)'P', (byte)pitchValue, (byte)'Y', (byte)yawValue};
			client.GetStream().Write(data, 0, data.Length);
		}
	}

	class TimeOutSocket{
		private static bool IsConnectionSuccessful = false;
		private static Exception socketexception;
		private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);

		public static TcpClient Connect(IPEndPoint remoteEndPoint, int timeoutMSec){
			TimeoutObject.Reset();
			socketexception = null; 

			string serverip = Convert.ToString(remoteEndPoint.Address);
			int serverport = remoteEndPoint.Port;          
			TcpClient tcpclient = new TcpClient();

			tcpclient.BeginConnect(serverip, serverport,
				new AsyncCallback(CallBackMethod), tcpclient);

			if (TimeoutObject.WaitOne(timeoutMSec, false)){
				if (IsConnectionSuccessful){
					return tcpclient;
				}
				else{
					throw socketexception;
				}
			}
			else{
				tcpclient.Close();
				throw new TimeoutException("TimeOut Exception");
			}
		}
		private static void CallBackMethod(IAsyncResult asyncresult){
			try{
				IsConnectionSuccessful = false;
				TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

				if (tcpclient.Client != null){
					tcpclient.EndConnect(asyncresult);
					IsConnectionSuccessful = true;
				}
			}catch (Exception ex){
				IsConnectionSuccessful = false;
				socketexception = ex;
			}finally{
				TimeoutObject.Set();
			}
		}
	}
}


