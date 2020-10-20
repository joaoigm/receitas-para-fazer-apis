using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using PequenaCozinheira.ReceitasParaFazer.Models;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;

namespace PequenaCozinheira.ReceitasParaFazer
{
    public static class DeletarReceita
    {
        [FunctionName("DeletarReceita")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosClientConnectionString"));
            var database = cosmosClient.GetDatabase("pequena-cozinheira");
            var container = database.GetContainer("receitas-para-fazer");

            var items = container.GetItemLinqQueryable<Receita>(allowSynchronousQueryExecution: true);

            using(var reader = new StreamReader(req.Body)) {
                var receita = JsonConvert.DeserializeObject<Receita>(await reader.ReadToEndAsync(), new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore
                });

               if (receita.Id == null && string.IsNullOrEmpty(receita.Id.ToString())) {
                    return new BadRequestObjectResult(new {
                        message = "Id não pode ser nulo"
                    });
                }
                await container.DeleteItemAsync<Receita>(receita.Id.ToString(), PartitionKey.None);
                return new NoContentResult();
            }
        }
    }
}
