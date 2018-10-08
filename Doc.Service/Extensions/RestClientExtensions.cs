using RestSharp;
using System;
using System.Threading.Tasks;

namespace Doc.Service.Extensions
{
    public static class RestClientExtensions
    {
        //Really que eu precisei extender essa merda ????? ¯\_(ツ)_/¯
        public static Task<IRestResponse> ExecuteTaskAsync(this RestClient @this, RestRequest request)
        {
            if (@this == null)
                throw new NullReferenceException();

            var tcs = new TaskCompletionSource<IRestResponse>();

            @this.ExecuteAsync(request, (response) =>
            {
                if (response.ErrorException != null)
                    tcs.TrySetException(response.ErrorException);
                else
                    tcs.TrySetResult(response);
            });

            return tcs.Task;
        }
    }
}