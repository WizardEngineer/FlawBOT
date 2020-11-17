﻿using System.Net;
using System.Threading.Tasks;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class DictionaryService : HttpHandler
    {
        public static async Task<DictionaryData> GetDictionaryDefinitionAsync(string query)
        {
            var results = await Http
                .GetStringAsync(string.Format(Resources.API_Dictionary, WebUtility.UrlEncode(query.Trim())))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<DictionaryData>(results);
        }
    }
}