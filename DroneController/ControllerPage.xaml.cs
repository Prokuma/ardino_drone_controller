using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DroneController
{
	public partial class ControllerPage : ContentPage
	{
		public Slider throttleSliderO;
		public Slider rollSliderO;
		public Slider pitchSliderO;
		public Slider yawSliderO;

		public Button rollZeroButtonO;
		public Button pitchZeroButtonO;
		public Button yawZeroButtonO;

		public static ControllerPage Instance;

		void Awake(){
			Instance = this;
		}

		public ControllerPage ()
		{
			InitializeComponent ();
			Title = "Control";
			this.throttleSliderO = throttleSlider;
			this.rollSliderO = rollSlider;
			this.pitchSliderO = pitchSlider;
			this.yawSliderO = yawSlider;

			this.rollZeroButtonO = rollZeroButton;
			this.pitchZeroButtonO = pitchZeroButton;
			this.yawZeroButtonO = yawZeroButton;
		}
	}
}

