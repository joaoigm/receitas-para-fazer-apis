using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PequenaCozinheira.ReceitasParaFazer.Models;
using Microsoft.Azure.Cosmos;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;

namespace PequenaCozinheira.ReceitasParaFazer
{
    public static class NovaReceita
    {
        [FunctionName("NovaReceita")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
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

                if ((await items.Where(i => i.Nome == receita.Nome).CountAsync()) > 0) {
                    log.LogDebug($"Receita de ${receita.Nome} já cadastrada");
                    return new OkObjectResult(new {
                        message = "Receita já cadastrada"
                    });
                }

                receita.setId();

                await container.CreateItemAsync<Receita>(receita);
                var receitaCriada = items.Where(i => i.Id == receita.Id).ToArray()[0];
                return new OkObjectResult(receitaCriada);
            }
        }
    }
}
