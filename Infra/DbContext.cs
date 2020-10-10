using Microsoft.Azure.Cosmos;

namespace PequenaCozinheira.ReceitasParaFazer.Infra {
    public class DbContext {
        private CosmosClient client;

        public DbContext(string dbEndoint, string dbKey){
            this.client = new CosmosClient(dbEndoint, dbKey);  
        }
    }
}