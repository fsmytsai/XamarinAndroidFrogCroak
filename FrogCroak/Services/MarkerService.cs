using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FrogCroak.Models;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace FrogCroak.Services
{
    public class MarkerService
    {
        public AllRequestResult GetMarkerList()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    
                    string result = client.DownloadString(
                        $"{Resource.String.BackEndPath}api/MarkerApi/GetMarkerList"
                    );

                    return new AllRequestResult
                    {
                        IsSuccess = true,
                        Result = result
                    };
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    var response = (HttpWebResponse)ex.Response;
                    if (response.StatusCode == HttpStatusCode.BadRequest) // HTTP 400
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            var content = reader.ReadToEnd();
                            string result = JsonConvert.DeserializeObject<String>(content);
                            return new AllRequestResult
                            {
                                IsSuccess = false,
                                Result = result
                            };
                        }
                    }
                }
                return new AllRequestResult
                {
                    IsSuccess = false,
                    Result = "請檢察網路連線"
                };
            }
        }
    }
}