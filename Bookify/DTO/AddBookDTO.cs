using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookify.DTO
{
    public class AddBookDTO
    {
        /// <summary>
        /// The name of the book
        /// </summary>
        /// <example>The art of not giving a fuck</example>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The description of the book
        /// </summary>
        /// <example>Becoming your best self</example>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The number of the books avaible
        /// </summary>
        /// <example>10</example>
        [JsonProperty("bookCount")]
        public long BookCount { get; set; }
    }
}
