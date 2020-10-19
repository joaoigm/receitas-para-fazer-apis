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
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Linq;

namespace PequenaCozinheira.ReceitasParaFazer
{
    public static class NovasReceitas
    {
        [FunctionName("NovasReceitas")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosClientConnectionString"));
            var database = cosmosClient.GetDatabase("pequena-cozinheira");
            var container = database.GetContainer("receitas-para-fazer");

            var items = container.GetItemLinqQueryable<Receita>(allowSynchronousQueryExecution: true);

            using (var reader = new StreamReader(req.Body))
            {
                var receitas = JsonConvert.DeserializeObject<List<Receita>>(await reader.ReadToEndAsync(), new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                if (receitas.Any())
                {
                    var receitasCriadas = new List<Receita>(receitas.Count);
                    receitas.ForEach( (r) => {
                        if (items.Where(i => i.Nome == r.Nome).CountAsync().Result > 0) {
                            log.LogDebug($"Receita de ${r.Nome} já cadastrada");
                        }
                        r.setId();
                        receitasCriadas.Add(container.CreateItemAsync<Receita>(r).Result.Resource);
                    });
                    return new OkObjectResult(receitasCriadas);
                }

                return new NoContentResult();
            }
        }
    }
}
