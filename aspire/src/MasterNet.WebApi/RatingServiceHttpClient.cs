using MasterNet.Application.Contracts;

namespace MasterNet.WebApi;

public class RatingServiceHttpClient(
    HttpClient httpClient,
    ILogger<RatingServiceHttpClient> logger
) : IRatingServiceClient
{

    public Task<int> GetRating(string id) =>
        httpClient.GetFromJsonAsync<int>($"/ratings?id={id}");


    public Task SendRating(string id, int rating)
    {
        logger.LogInformation(
            "Enviando una calificacion al curso {id} de {rating}", 
            id, 
            rating
         );

        return httpClient.PostAsJsonAsync("/ratings", new
        {
            Id = id,
            Rating = rating
        });
    }

}
