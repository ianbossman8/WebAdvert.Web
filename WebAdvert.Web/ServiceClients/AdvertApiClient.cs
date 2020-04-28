using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
        {
            _client = client;
            _configuration = configuration;
            _mapper = mapper;

            string createUrl = configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");
            _client.BaseAddress = new Uri(createUrl);
            _client.DefaultRequestHeaders.Add(name: "Content-type", value: "application/json");
        }

        public async Task<AdvertResponse> Create(CreateAdvertModel model)
        {
            AdvertModel advertApiModel = _mapper.Map<AdvertModel>(model);
            string jsonModel = JsonConvert.SerializeObject(advertApiModel);
            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress, new StringContent(jsonModel));
            string responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(continueOnCapturedContext: false);
            CreateAdvertResponse createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
            AdvertResponse advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);

            return advertResponse;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            ConfirmAdvertModel advertModel = _mapper.Map<ConfirmAdvertModel>(model);
            string jsonModel = JsonConvert.SerializeObject(advertModel);
            var response = await _client
                .PutAsync(
                    new Uri($"{_client.BaseAddress}/confirm"),
                    new StringContent(jsonModel, Encoding.UTF8, "application/json")
                    );

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