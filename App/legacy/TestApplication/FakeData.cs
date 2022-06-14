using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TestApplication
{
    public partial class Values
    {
        [JsonProperty ("data")]
        public Dictionary<string, Datum> Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty ("value")]
        public string Value { get; set; }

        [JsonProperty ("delta")]
        public string Delta { get; set; }
    }

    public static class TestData
	{
        public static string GetJson ()
		{
            // Go go fake business logic!
            Values v = new Values {
                Data = new Dictionary<string, Datum> {
                    { "2020-07-01",  new Datum { Value = "50.34", Delta = "-1.68"} },
                    { "2020-07-02",  new Datum { Value = "51.99", Delta = "-0.03"} },
                    { "2020-07-03",  new Datum { Value = "51.56", Delta = "-0.46"} },
                }
            };

            return JsonConvert.SerializeObject (v);
        }
	}
}
