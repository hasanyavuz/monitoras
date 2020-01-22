using System.Collections.Generic;
using Newtonsoft.Json;

namespace Monitoras.Web {
    public class MTReturn {
        [JsonProperty (NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty (NullValueHandling = NullValueHandling.Ignore)]
        public string InternalMessage { get; set; }
        public int Code { get; set; }

        [JsonProperty (NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
        public bool Success { get; set; }

        [JsonProperty (NullValueHandling = NullValueHandling.Ignore)]
        public List<MTReturnError> Errors { get; set; }
    }

    public class MTReturnError {
        public string Message { get; set; }

        [JsonProperty (NullValueHandling = NullValueHandling.Ignore)]
        public string InternalMessage { get; set; }
        public string Name { get; set; }
    }
}