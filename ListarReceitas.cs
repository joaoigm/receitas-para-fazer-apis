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
    public static class ListarReceitas
    {
        [FunctionName("ListarReceitas")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var cosmosClient = new CosmosClient(Environment.GetEnvironmentVariable("CosmosClientConnectionString"));
            var database = cosmosClient.GetDatabase("pequena-cozinheira");
            var container = database.GetContainer("receitas-para-fazer");

            var items = container.GetItemLinqQueryable<Receita>(allowSynchronousQueryExecution: true);

            return new OkObjectResult(items.AsQueryable().ToList());
        }
    }
}
