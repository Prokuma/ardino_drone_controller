using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DroneController{
	
	public partial class IPPage : ContentPage{
		public Entry ipEntry;
		public Button connectButtonO;
		public Label stateLabelO;

		public static IPPage Instance;

		void Awake(){
			Instance = this;
		}

		public IPPage (){
			InitializeComponent ();
			Title = "Connection";
			//BackgroundColor = new Color (0,192/255,255/255);
			ipEntry = IPEntry;
			this.connectButtonO = connectButton;
			this.stateLabelO = stateLabel;
		}
	}
}

