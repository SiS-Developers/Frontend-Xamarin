﻿using System;
using Newtonsoft.Json;

namespace StockUp.Model
{
    public class TicketData
    {
        [JsonProperty("Game")]
        public int Game { set { Id += value.ToString(); } }

        [JsonProperty("Pack")]
        public int Pack { set { Id += value.ToString(); } }

        [JsonProperty("Nbr")]
        public int Nbr { set { Id += value.ToString(); } }

        [JsonProperty("Name")]
        public String Name { get; set; }

        [JsonProperty("Store")]
        public String Store { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("Activated")]
        public int Activated { get; set; } // tiny int 0 1

        [JsonProperty("LoadedPack")]
        public int LoadedPack { get; set; } // tiny int 0 1

        [JsonProperty("Price")]
        public int Price { get; set; }

        [JsonProperty("Start_Inv")]
        public int Start_Inv { get; set; } // tiny int 0 1

        [JsonProperty("End_Inv")]
        public int End_Inv { get; set; } // tiny int 0 1

        [JsonProperty("Emp_id")]
        public String Emp_id { get; set; }

        // for UI
        public String Id { get; set; }
        public String IconSource = "Status_Green.png";
    }
}
