using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Mango.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            try
            {
                HttpClient client = httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Content-Type", "application/json");
                //token

                message.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data is not null)
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");

                HttpResponseMessage? apiResponse = null;

                switch (requestDto.ApiType)
                {
                    case Utility.SD.ApiTypes.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case Utility.SD.ApiTypes.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case Utility.SD.ApiTypes.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case Utility.SD.ApiTypes.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);

                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "not found"
                        };

                    case System.Net.HttpStatusCode.Forbidden:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Access Denied"
                        };

                    case System.Net.HttpStatusCode.Unauthorized:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Unauthorized"
                        };

                    case System.Net.HttpStatusCode.InternalServerError:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Internal Server Error"
                        };

                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
