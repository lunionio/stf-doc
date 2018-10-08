using Doc.Service.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Doc.Service.Services
{
    public class AuxNotStatic
    {
        public static async Task<MotorAuxViewModel> GetInfoMotorAux(string aux, int idcliente)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://inapi.mundowebpix.com.br:5400");
                    var url = "api/motoraux/acessarmotor/" + aux;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.Content.ReadAsStringAsync();
                        var lstData = JsonConvert.DeserializeObject<MotorAuxViewModel>(data.Result.ToString());
                        return lstData;
                    }
                    else
                        return new MotorAuxViewModel();
                }

            }
            catch (Exception ex)
            {
                return new MotorAuxViewModel();
            }
        }
    }
}