using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using KarachiEstateHub.ViewModels;

namespace KarachiEstateHub.Services
{
    public class GroqPropertyAssistantService
    {
        private const string GroqChatCompletionsUrl = "https://api.groq.com/openai/v1/chat/completions";
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public bool TryExtractIntent(string prompt, out PropertySearchIntent intent, out string errorMessage)
        {
            intent = null;
            errorMessage = null;

            var apiKey = GetApiKey();
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                errorMessage = "Groq API key missing.";
                return false;
            }

            try
            {
                var requestBody = new
                {
                    model = GetModelName(),
                    temperature = 0,
                    max_tokens = 220,
                    messages = new object[]
                    {
                        new
                        {
                            role = "system",
                            content = "You convert Karachi real estate search text into JSON only. Return exactly these keys: purpose, propertyType, location, bedrooms, minPrice, maxPrice. Use null when unknown. Purpose must be Sale or Rent. Prices are PKR numbers. Interpret crore as 10000000 and lakh as 100000. Do not invent properties."
                        },
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    }
                };

                var json = PostJson(apiKey, serializer.Serialize(requestBody));
                GroqChatResponse response;
                try
                {
                    response = serializer.Deserialize<GroqChatResponse>(json);
                }
                catch
                {
                    errorMessage = "Groq response parsing failed.";
                    return false;
                }

                var content = response != null
                    && response.choices != null
                    && response.choices.Count > 0
                    && response.choices[0].message != null
                        ? response.choices[0].message.content
                        : null;

                if (String.IsNullOrWhiteSpace(content))
                {
                    errorMessage = "Groq response parsing failed.";
                    return false;
                }

                try
                {
                    intent = ParseIntentJson(content);
                }
                catch
                {
                    errorMessage = "Groq response parsing failed.";
                    return false;
                }

                if (intent == null)
                {
                    errorMessage = "Groq response parsing failed.";
                    return false;
                }

                return true;
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                errorMessage = response != null
                    ? "Groq failed: " + ((int)response.StatusCode)
                    : "Groq failed: 500";
                return false;
            }
            catch
            {
                errorMessage = "Groq failed: 500";
                return false;
            }
        }

        private static string GetApiKey()
        {
            return ConfigurationManager.AppSettings["GroqApiKey"];
        }

        private static string GetModelName()
        {
            var configuredModel = ConfigurationManager.AppSettings["GroqModel"];
            return String.IsNullOrWhiteSpace(configuredModel) ? "llama-3.1-8b-instant" : configuredModel;
        }

        private static string PostJson(string apiKey, string body)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(GroqChatCompletionsUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers[HttpRequestHeader.Authorization] = "Bearer " + apiKey;
            request.Timeout = 10000;

            var bytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        private PropertySearchIntent ParseIntentJson(string content)
        {
            var json = ExtractJsonObject(content);
            var raw = serializer.Deserialize<Dictionary<string, object>>(json);
            if (raw == null)
            {
                return null;
            }

            return new PropertySearchIntent
            {
                Purpose = ReadString(raw, "purpose"),
                PropertyType = ReadString(raw, "propertyType"),
                Location = ReadString(raw, "location"),
                Bedrooms = ReadNullableInt(raw, "bedrooms"),
                MinPrice = ReadNullableDecimal(raw, "minPrice"),
                MaxPrice = ReadNullableDecimal(raw, "maxPrice")
            };
        }

        private static string ExtractJsonObject(string content)
        {
            var start = content.IndexOf('{');
            var end = content.LastIndexOf('}');
            if (start < 0 || end <= start)
            {
                return content;
            }

            return content.Substring(start, end - start + 1);
        }

        private static string ReadString(IDictionary<string, object> raw, string key)
        {
            object value;
            return raw.TryGetValue(key, out value) && value != null ? value.ToString() : null;
        }

        private static int? ReadNullableInt(IDictionary<string, object> raw, string key)
        {
            object value;
            if (!raw.TryGetValue(key, out value) || value == null)
            {
                return null;
            }

            int parsed;
            return Int32.TryParse(value.ToString(), out parsed) ? parsed : (int?)null;
        }

        private static decimal? ReadNullableDecimal(IDictionary<string, object> raw, string key)
        {
            object value;
            if (!raw.TryGetValue(key, out value) || value == null)
            {
                return null;
            }

            decimal parsed;
            return Decimal.TryParse(value.ToString(), out parsed) ? parsed : (decimal?)null;
        }

        private class GroqChatResponse
        {
            public List<GroqChoice> choices { get; set; }
        }

        private class GroqChoice
        {
            public GroqMessage message { get; set; }
        }

        private class GroqMessage
        {
            public string content { get; set; }
        }
    }
}
