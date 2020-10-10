using Newtonsoft.Json;

namespace PequenaCozinheira.ReceitasParaFazer.Models {
    public class Post {
        [JsonProperty("titulo")]
        public string Titulo { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}