using System;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using FrogCroak.Models;
using System.Threading.Tasks;

namespace FrogCroak.Services
{
    public class ChatService
    {
        public async Task<AllRequestResult> UploadImage(string FromFilePath, string ContentType)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "image/" + ContentType.ToLower();
                    byte[] result = await client.UploadFileTaskAsync(CPSharedService.BackEndPath + "api/ImageApi/UploadImage", FromFilePath);
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
                return new AllRequestResult
                {
                    IsSuccess = false,
                    Result = ex
                };
            }
        }

        public async Task<AllRequestResult> CreateMessage(string Content)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string result = await client.UploadStringTaskAsync(
                        $"{CPSharedService.BackEndPath}api/MessageApi/CreateMessage",
                        $"Content={Content}"
                    );

                    result = JsonConvert.DeserializeObject<String>(result);

                    return new AllRequestResult
                    {
                        IsSuccess = true,
                        Result = result
                    };
                }
            }
            catch (WebException ex)
            {
                return new AllRequestResult
                {
                    IsSuccess = false,
                    Result = ex
                };
            }
        }
    }
}