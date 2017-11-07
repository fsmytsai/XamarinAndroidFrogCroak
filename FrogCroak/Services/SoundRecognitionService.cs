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
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace FrogCroak.Services
{
    public class SoundRecognitionService
    {
        public async Task<AllRequestResult> SoundRecognition(string FromFilePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "audio/x-wav";
                    byte[] result = await client.UploadFileTaskAsync(Resource.String.BackEndPath + "Api/Frog/SoundRecognition", FromFilePath);
                    string Frog = JsonConvert.DeserializeObject<String>(Encoding.Default.GetString(result));
                    return new AllRequestResult
                    {
                        IsSuccess = true,
                        Result = Frog
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