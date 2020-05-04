using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly string _baseAddress;
        private readonly HttpClient _client;
        private IMapper _mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
            _baseAddress = configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
        }

        public async Task<AdvertResponse> Create(CreateAdvertModel model)
        {
            AdvertModel advertApiModel = _mapper.Map<AdvertModel>(model);

            string jsonModel = JsonConvert.SerializeObject(advertApiModel);

            HttpResponseMessage response = await _client.PostAsync(
                new Uri($"{_baseAddress}/create"),
                new StringContent(jsonModel, Encoding.UTF8, "application/json")
            );

            string createAdvertResponse = await response.Content.ReadAsStringAsync();
            AdvertResponse jsonResponse = JsonConvert.DeserializeObject<AdvertResponse>(createAdvertResponse);

            AdvertResponse advertResponse = _mapper.Map<AdvertResponse>(jsonResponse);

            return advertResponse;
        }

        public async Task<bool> Confirm(ConfirmAdvertRequest model)
        {
            ConfirmAdvertModel advertModel = _mapper.Map<ConfirmAdvertModel>(model);

            string jsonModel = JsonConvert.SerializeObject(advertModel);

            HttpResponseMessage response = await _client.PutAsync(
                new Uri($"{_baseAddress}/confirm"),
                new StringContent(jsonModel, Encoding.UTF8, "application/json")
            );

            Console.WriteLine(response);
            return response.StatusCode == HttpStatusCode.OK;
        }

        //public async Task<List<Advertisement>> GetAllAsync()
        //{
        //    var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/all")).ConfigureAwait(false);
        //    var allAdvertModels = await apiCallResponse.Content.ReadAsAsync<List<AdvertModel>>().ConfigureAwait(false);
        //    return allAdvertModels.Select(x => _mapper.Map<Advertisement>(x)).ToList();
        //}

        //public async Task<Advertisement> GetAsync(string advertId)
        //{
        //    var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/{advertId}")).ConfigureAwait(false);
        //    var fullAdvert = await apiCallResponse.Content.ReadAsAsync<AdvertModel>().ConfigureAwait(false);
        //    return _mapper.Map<Advertisement>(fullAdvert);
        //}
    }
}