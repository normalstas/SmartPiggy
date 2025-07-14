using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using smartPiggy.Models;

namespace smartPiggy.Servies
{
    class ExpendiServies
    {
		const string Url = "http://192.168.0.104:5262/api/ExpendiTables";

		public static T Get<T>(string id = null)
		{
			using (WebClient client = new WebClient())
			{
				client.Encoding = Encoding.UTF8;
				string url = string.IsNullOrEmpty(id) ? Url : $"{Url}/{id}";
				string response = client.DownloadString(url);
				return JsonConvert.DeserializeObject<T>(response);
			}
		}

		public static async Task<string> Post(ExpendiTable expendi)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Add(
					new MediaTypeWithQualityHeaderValue("application/json"));

				// Явно задаём camelCase для совместимости
				var json = JsonConvert.SerializeObject(expendi, new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					NullValueHandling = NullValueHandling.Ignore
				});

				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url, content);

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync();
					throw new Exception($"API Error: {response.StatusCode} - {error}");
				}

				return await response.Content.ReadAsStringAsync();
			}
		}

		public static string Put<T>(T editObject, string id)
		{
			WebClient client = new WebClient();

			client.Encoding = Encoding.UTF8;

			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

			string data = JsonConvert.SerializeObject(editObject);

			return client.UploadString(Url + "\\" + id.ToString(), "PUT", data);
		}

		public static string Delete<T>(T editObject, string id)
		{
			WebClient client = new WebClient();
			client.Encoding = Encoding.UTF8;
			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			string data = JsonConvert.SerializeObject(editObject);
			string result = client.UploadString(Url + "\\" + id.ToString(), "DELETE", data);
			return result;

		}
	}
}
