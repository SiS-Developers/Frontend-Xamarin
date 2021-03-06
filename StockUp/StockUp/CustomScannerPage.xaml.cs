﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using StockUp.Model;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using ZXing.Net.Mobile.Forms;

namespace StockUp
{
	public partial class CustomScannerPage : ContentPage
	{

		ZXingScannerView Zxing;
		ZXingDefaultOverlay Overlay;
		RestService _restService;
		public static string state; 

		public CustomScannerPage () : base ()
		{
			InitializeComponent();
			On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);

			Zxing = new ZXingScannerView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				AutomationId = "zxingScannerView",
			};
			Zxing.OnScanResult += (result) =>
				Device.BeginInvokeOnMainThread(async () => {

                    if (result.Text.Length > 14)
                    {
			            LogTicket(result.Text);
                    }
                    else
                    {
				        await DisplayAlert("Failed", "Make sure to include all 0's in your 14 digit barcode.", "OK");
                    }
				});

			Overlay = new ZXingDefaultOverlay
			{
				TopText = "Hold your phone up to the barcode",
				BottomText = "Scanning will happen automatically",
				ShowFlashButton = Zxing.HasTorch,
				AutomationId = "zxingDefaultOverlay",
			};
			Overlay.FlashButtonClicked += (sender, e) => {
				Zxing.IsTorchOn = !Zxing.IsTorchOn;
			};

			// The root page of your application
			ScannerGrid.Children.Add(Zxing);
			ScannerGrid.Children.Add(Overlay);
			//Content = grid;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Zxing.IsScanning = true;
		}

		protected override void OnDisappearing()
		{
			Zxing.IsScanning = false;

			base.OnDisappearing();
		}

		void Submit_Clicked(System.Object sender, System.EventArgs e)
		{
			Debug.Write("\nFIRST");
			Debug.Write("\nSECOND");

			string result = GameEntry.Text + PackEntry.Text + NbrEntry.Text;
			Debug.Write("\nresult: " + result);
            if (result.Length > 14)
            {
			    LogTicket(result);
            }
            else
            {
				DisplayAlert("Failed", "Make sure to include all 0's in your 14 digit barcode.", "OK");
            }
		}

		private async void LogTicket(String result)
		{
			// Stop analysis until we navigate away so we don't keep reading barcodes
			Zxing.IsAnalyzing = false;

			TicketData ticket;
			ticket = new TicketData
			{
				Game = Constants.GetGameNum(result),
				Pack = Constants.GetPackNum(result),
				Nbr = Constants.GetNbrNum(result),
				Emp_id = Constants.UserData.Emp_id
			};
			ticket.Id = Constants.CreateTicketId(ticket.Game, ticket.Pack, ticket.Nbr);
			ticket.Name = Constants.gamesAndNames[ticket.Game];
			ticket.Price = Constants.gamesAndPrices[ticket.Game];

			String[] actionButtons;
			// Show pop up
			if (Constants.State.Equals(Constants.Activate))
			{
				actionButtons = new string[] { "Done" };

			}
			else
			{
				actionButtons = new string[] { "Confirm", "Done" };
			}
			_restService = new RestService();
			HttpResponseMessage response;
			String content;
			var action = await DisplayActionSheet("Scanned Barcode\n" + result, "Redo", "Cancel", actionButtons);
			switch (action)
			{
				case "Redo":
					Zxing.IsAnalyzing = true;
					break;
				case "Confirm":
					switch (Constants.State)
					{
						case Constants.Start:
							for (int i = 0; i < Constants.startTickets.Count; i++)
							{
								if (Constants.startTickets[i].Id.Equals(ticket.Id))
								{
									Constants.startTickets[i].isScanned = true;
								}
							}
							break;
						case Constants.Activate:
							break;
						case Constants.End:
							ticket.isScanned = true;
							ticket.Status = "Scanned";
							Constants.endTickets.Add(ticket);
							break;
						case Constants.Inventory:
							response = await _restService.PostSeedTicket(ticket.Game.ToString(), ticket.Pack.ToString(), ticket.ToString(), Constants.UserData.Emp_id);
							content = await response.Content.ReadAsStringAsync();
							if (response.IsSuccessStatusCode)
							{
								await DisplayAlert("Success", "Seeded ticket " + ticket.Id, "OK");
							}
							else
							{
								await DisplayAlert("Failure", "Could not seed ticket", "OK");
							}
							break;
					}
					await DisplayAlert("Added ticket", ticket.Name, "OK");
					Zxing.IsAnalyzing = true;
					break;
				case "Done":
					switch (Constants.State)
					{
						case Constants.Start:
							for (int i = 0; i < Constants.startTickets.Count; i++)
							{
								Debug.Write("\nComparing Scan: " + Constants.startTickets[i].Id + "\n\tWith ticket: " + ticket.Id);
								if (Constants.startTickets[i].Id.Equals(ticket.Id))
								{
									Constants.startTickets[i].isScanned = true;
									Debug.Write("Checking bool: " + Constants.startTickets[i].isScanned);
									break;
								}
							}
							break;
						case Constants.Activate:
							response = await _restService.PostActivateTicket(ticket.Game.ToString(), ticket.Pack.ToString(), ticket.ToString(), Constants.UserData.Emp_id);
							content = await response.Content.ReadAsStringAsync();
							if (response.IsSuccessStatusCode)
							{
								await DisplayAlert("Success", "Activated ticket", "OK");
							}
							else
							{
								await DisplayAlert("Failure", "Could not activate ticket", "OK");
							}
							break;
						case Constants.End:
							ticket.isScanned = true;
							ticket.Status = "Scanned";
							Constants.endTickets.Add(ticket);
							await DisplayAlert("Added ticket", ticket.Name, "OK");
							break;
						case Constants.Inventory:
							response = await _restService.PostSeedTicket(ticket.Game.ToString(), ticket.Pack.ToString(), ticket.ToString(), Constants.UserData.Emp_id);
							content = await response.Content.ReadAsStringAsync();
							if (response.IsSuccessStatusCode)
							{
								await DisplayAlert("Success", "Seeded ticket " + ticket.Id, "OK");
							}
							else
							{
								await DisplayAlert("Failure", "Could not seed ticket", "OK");
							}
							break;
					}
					await Navigation.PopModalAsync();
					break;
			}
		}
    }
}
