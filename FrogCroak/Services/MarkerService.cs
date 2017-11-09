using System.Text;
using FrogCroak.Models;
using System.Net;
using Newtonsoft.Json;

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
                    string url = $"{CPSharedService.BackEndPath}api/MarkerApi/GetMarkerList";
                    string result = client.DownloadString(
                        url
                    );

                    MyMarkers myMarkers = JsonConvert.DeserializeObject<MyMarkers>(result);

                    return new AllRequestResult
                    {
                        IsSuccess = true,
                        Result = myMarkers
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