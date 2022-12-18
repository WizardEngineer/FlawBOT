﻿using FlawBOT.Common;
using FlawBOT.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.Net;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class MiscService : HttpHandler
    {
        private static readonly ImmutableArray<string> Answers = new[]
        {
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Yes definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful"
        }.ToImmutableArray();

        public static string GetRandomAnswer()
        {
            var random = new Random();
            return Answers[random.Next(Answers.Length)];
        }

        public static async Task<string> GetCatFactAsync()
        {
            var result = await Http.GetStringAsync(Resources.URL_CatFacts).ConfigureAwait(false);
            return JObject.Parse(result)["fact"]?.ToString();
        }

        public static async Task<string> GetCatPhotoAsync()
        {
            var results = await Http.GetStringAsync(Resources.URL_CatPhoto).ConfigureAwait(false);
            return JObject.Parse(results)["file"]?.ToString();
        }

        public static async Task<DogData> GetDogPhotoAsync()
        {
            var results = await Http.GetStringAsync(Resources.URL_DogPhoto).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<DogData>(results);
        }

        public static async Task<IpStack> GetIpLocationAsync(IPAddress query)
        {
            var result = await Http.GetStringAsync(string.Format(Resources.URL_IPStack, query))
                .ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IpStack>(result);
        }
    }
}