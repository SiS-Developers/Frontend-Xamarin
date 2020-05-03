using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;
namespace StockUp.Test
{
	[TestFixture(Platform.Android)]
	
	/*
	Mainly able to test on Android because iOS
	testing requires developer account, which cost $100
	[TestFixture(Platform.iOS)]
	*/
	class TestRestServicesInterface
	{
		IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void AppLaunches()
        {
            LoginTest();
        }
		// Test of getting user data in to the application through 
		// correct credentials.
		[Test]
		public void GetUsersDataWithCorrectData(){ 
			//Arrange
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("ManageUser");
			app.Tap("Manage");

			//Assert
			var appResult = app.Query("resultManage");
			Debug.Write(appResult.First());
		}

		// Test of logging in to the application through 
		// incorrect credentials.
		[Test]
		public void GetUsersDataWithIncorrectData(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("Manage");

			//Assert
			var appResult = app.Query("resultManage");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetTicketsDataWithCorrectData(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");
			//Action
			app.Tap("Inventory");

			//Assert
			var appResult = app.Query("resultInventory");
			Debug.Write(appResult.First());
		}
		
		[Test]
		public void GetPreviousTicketsData(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");
			//Action
			app.Tap("Inventory");
			app.Tap("Serach")
			
			app.EntryText("Search", "Date-1")

			//Assert
			var appResult = app.Query("resultTickets");
			Debug.Write(appResult.First());
		}
		
		[Test]
		public void PostUserLogin(){ 
			//Arrange
			app.EntryText("email", "email");
			app.EntryText("pass", "password");

			//Action
			app.Tap("login");

			//Assert
			var appResult = app.Query("resultLogin");
			Debug.Write(appResult.First());
		}

			[Test]
		public void PostUserLoginWithIncorrectData(){ 
			//Arrange
			app.EntryText("email", "bad");
			app.EntryText("pass", "Bas_password");

			//Action
			app.Tap("login");

			//Assert
			var appResult = app.Query("resultLogin");
			Debug.Write(appResult.First());
		}

		[Test]
		public void PostUserLogout(){ 
			//Arrange
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");
			app.Tap("login");
			app.Tap("logout");
			//Action
			var logoutString = "URL" + "\\" + "email" + "\\" + "password" + "\\" + "id";
			
			//Assert
			var appResult = app.QueryFromHttp(logoutString);
			Debug.Write(appResult.First());
			Debug.Write(appResult.Second());
		}


		[Test]
		public void PostNewUser()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");
			//Arrange
			app.Tap("login");
			app.Tap("Manage");
			app.Tap("add");
			app.EntryText("email", "email0@gmail.com");
			app.EntryText("first", "first");
			app.EntryText("last", "last");
			app.EntryText("password", "email0");
			app.EntryText("confirm_password", "email0");

			//Action
			app.Tap("submit");

			//Assert
			var appResult = app.Query("newUserPost");
			Debug.Write(appResult.First());
		}

		[Test]
		public void DeleteUserData(l){ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");
			//Arrange
			app.Tap("login");
			app.Tap("Manage");
			app.Tap(userList.setSelected(0));
			//Assert
			var appResult = app.Query("removeUser");
			Debug.Write(appResult.First());
			Debug.Write(appResult.Second());
		}


		[Test]
		public void PostActivateTicket(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");
			//Action
			app.Tap("Activate_Ticket");
			app.EntryText("gameID", "0000");
			app.EntryText("packID", "000");
			app.EntryText("ticketID", "012");
			app.Tap("submit");
			//Assert
			var appResult = app.Query("resultOutput0");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetAllGames()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("Inventory");
			//Assert
			var appResult = app.Query("resultGroupTicketResults");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetInventory()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("Inventory");
			//Assert
			var appResult = app.Query("resultGroupTicketResults");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetDailyCounts()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("counts");
			app.Tap("stats");
			app.Tap("stats_dailyCounts");
			//Assert
			var appResult = app.Query("resultDailyCounts");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetWeeklyCounts()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("counts");
			app.Tap("stats");
			app.Tap("stats_weeklyCounts");
			//Assert
			var appResult = app.Query("resultWeeklyCounts");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetMonthlyCounts()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("counts");
			app.Tap("stats");
			app.Tap("statsMonthlyCounts");
			//Assert
			var appResult = app.Query("resultMonthlyCounts");
			Debug.Write(appResult.First());
		}

		[Test]
		public void PostSeedTicket(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("Inventory");
			app.Tap("add")
			app.EntryText("gameID", "1662");
			app.EntryText("packID", "145");
			app.EntryText("ticketID", "050");
			app.Tap("submit");
			//Assert
			var appResult = app.Query("resultSeedInventory");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetEndDayPreviousDayTickets(){ 
			//Arrange
			app.EntryText("email","emp0@gamil.com");
			app.EntryText("password","emp0");

			//Action
			app.Tap("EndDay");
			//Assert
			var appResult = app.Query("resultEndDayPreviousDay");
			Debug.Write(appResult);
		}

		[Test]
		public void PostStartDayTicket()
		{ 
			//Arrange
			app.EntryText("email","emp0@gamil.com");
			app.EntryText("password","emp0");

			//Action
			app.Tap("StartDay");
			//Assert
			var appResult = app.Query("resultStartDay");
			Debug.Write(appResult);
		}

		[Test]
		public void PostEndDayTicket()
		{ 
			//Arrange
			app.EntryText("email","emp0@gamil.com");
			app.EntryText("password","emp0");

			//Action
			app.Tap("EndDay");
			//Assert
			var appResult = app.Query("resultEndDayDay");
			Debug.Write(appResult);
		}

		[Test]
		public void GetGroupTickets(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com")
			app.EntryText("password","email0")

			//Action
			app.Tap("login");
			app.Tap("Inventory");
			var Serach = invtory.setSelected(0).getAll();
			app.tap("serach");
			app.EntryText("search",Search);

			//Assert
			var appResult = app.Query("resultGroupTicketResults");
			Debug.Write(appResult.First());
		}

		[Test]
		public void PostToDeletePack(){ 
			//Arrange
			app.EntryText("email","email0@gamil.com")
			app.EntryText("password","email0")

			//Action
			app.Tap("login");
			app.Tap("Inventory");
			app.tap(invtory.setSelected(0));
			app.tap("delete_pack");

			//Assert
			var appResult = app.Query("resultDeletePack");
			Debug.Write(appResult.First());
		}

		[Test]
		public void GetSparePacks()
		{ 
			//Arrange
			app.EntryText("email","email0@gamil.com");
			app.EntryText("password","email0");

			//Action
			app.Tap("login");
			app.Tap("Inventory");
			app.Tap("Search");
			app.EntryText("Search","pack_unactivated");
			//Assert
			var appResult = app.Query("resultUnactivated");
			Debug.Write(appResult.First());
		}
	}
}