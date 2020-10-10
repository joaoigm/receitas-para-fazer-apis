using System;
using Newtonsoft.Json;

namespace PequenaCozinheira.ReceitasParaFazer.Models {

    public class Receita {

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonProperty("tipo")]
        public string Tipo { get; set; }
        [JsonProperty("fit")]
        public bool Fit { get; set; }
        [JsonProperty("vegan")]
        public bool Vegan { get; set; }
        [JsonProperty("testada")]
        public string Testada { get; set; }
        [JsonProperty("fonte")]
        public string Fonte { get; set; }
        [JsonProperty("post")]
        public Post Post { get; set; }


        public void setId() {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}