using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using wpf.Models;

namespace wpf.Control
{
    class ConectionConstrol
    {
        HttpClient httpClient;
        public string baseUrl =  "https://localhost:44365/"; //"http://98.254.97.228/"; 
        string url = System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location);
        public Token token;
        public ConectionConstrol()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public string login(string idConection, string Email, string Password)
        {
            string apiResponse = String.Empty;
            var payload = JsonConvert.SerializeObject(new { Email = Email, Password = Password, ConectionId = idConection });
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            using (var response = httpClient.PostAsync(baseUrl + "api/Qb/Login", c).Result)
            {
                apiResponse = response.Content.ReadAsStringAsync().Result;
                try
                {
                    token = JsonConvert.DeserializeObject<Token>(apiResponse);
                }
                catch (Exception)
                {
                    token = null;
                    return apiResponse;
                }
                if (token.token != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.token);
                    return "OK";
                }
            }
            return apiResponse;
        }
        public string RunQueryReturn(string result, string token, string funcion)
        {
            string payload;
            string apiResponse = String.Empty;
            switch (funcion)
            {
                case "getAllCustomers":
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Models.Customer.QBXML), new XmlRootAttribute("QBXML"));
                        StringReader stringReader = new StringReader(result);
                        var ObjqBXML = (Models.Customer.QBXML)serializer.Deserialize(stringReader);
                        var ObjqBXMLJson = JsonConvert.SerializeObject(ObjqBXML.QBXMLMsgsRs.CustomerQueryRs.CustomerRet);
                        payload = JsonConvert.SerializeObject(new { Body = ObjqBXMLJson, Token = token, Funcion = funcion });
                        break;
                    }
                default:
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(result);
                        var data = JsonConvert.SerializeXmlNode(doc);
                        payload = JsonConvert.SerializeObject(new { Body = data, Token = token , Funcion = "" });
                        break;
                    }
            }

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            using (var response = httpClient.PostAsync(baseUrl + "api/Qb/RequestReturn", c).Result)
            {
                apiResponse = response.Content.ReadAsStringAsync().Result;
            }
            return apiResponse;

            //string apiResponse = String.Empty;
            //var payload = JsonConvert.SerializeObject(new { Body = result, Token = token });
            //HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            //using (var response = httpClient.PostAsync(baseUrl + "api/Qb/RequestReturn", c).Result)
            //{
            //    apiResponse = response.Content.ReadAsStringAsync().Result;
            //}
            //return apiResponse;
        }

        public string AssamblyChequin()
        {
            string apiResponse;
            try
            {
                using (var response = httpClient.GetAsync(baseUrl + "api/Assambly/getAssablyVersion").Result)
                {
                    apiResponse = response.Content.ReadAsStringAsync().Result;
                    string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                    if (fileVersion != apiResponse && apiResponse != "")
                    {
                        if (File.Exists(url + "\\udqs.exe"))
                        {
                            System.Diagnostics.Process.Start(url + "\\udqs.exe");
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            return apiResponse;
        }
    }
}
