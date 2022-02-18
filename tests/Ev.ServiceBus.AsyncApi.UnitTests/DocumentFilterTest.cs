using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Ev.ServiceBus.AsyncApi.UnitTests
{
    public class DocumentFilterTest
    {
        [Fact]
        public async Task CheckStateOfSenderApp()
        {
            var factory = new SenderAppFactory();
            var client = factory.CreateClient();
            var response = await client.GetAsync("/asyncapi/asyncapi.json");

            response.StatusCode.Should().Be(StatusCodes.Status200OK);

            var json = await response.Content.ReadAsStringAsync();

            json.Should().Be("{\"asyncapi\":\"2.1.0\",\"info\":{\"title\":\"Receiver API\",\"version\":\"1.0.0\",\"description\":\"Sample sender project\"},\"servers\":{\"sb://yourconnection.servicebus.windows.net/\":{\"url\":\"sb://yourconnection.servicebus.windows.net/\",\"protocol\":\"amqp\"}},\"defaultContentType\":\"application/json\",\"channels\":{\"myqueue\":{\"publish\":{\"operationId\":\"pub/myqueue\",\"summary\":\"myqueue\",\"description\":\"Dispatch of messages through the myqueue Queue\",\"tags\":[{\"name\":\"Queue\",\"description\":\"The type of client\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}},{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"message\":{\"oneOf\":[{\"$ref\":\"#/components/messages/WeatherForecast[]\"}]}},\"bindings\":{\"amqp\":{\"is\":\"queue\",\"queue\":{\"name\":\"myqueue\",\"durable\":true,\"exclusive\":false,\"autoDelete\":false}}}},\"mytopic\":{\"publish\":{\"operationId\":\"pub/mytopic\",\"summary\":\"mytopic\",\"description\":\"Dispatch of messages through the mytopic Topic\",\"tags\":[{\"name\":\"Topic\",\"description\":\"The type of client\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}},{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"message\":{\"oneOf\":[{\"$ref\":\"#/components/messages/WeatherForecast\"}]}},\"bindings\":{\"amqp\":{\"is\":\"queue\",\"queue\":{\"name\":\"mytopic\",\"durable\":true,\"exclusive\":false,\"autoDelete\":false}}}}},\"components\":{\"schemas\":{\"weatherForecastof\":{\"id\":\"weatherForecastof\",\"title\":\"WeatherForecast[]\",\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/weatherForecastof/definitions/weatherForecast\"},\"definitions\":{\"weatherForecast\":{\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"date\":{\"type\":\"string\",\"format\":\"date-time\"},\"temperatureC\":{\"type\":\"integer\",\"format\":\"int32\"},\"temperatureF\":{\"type\":\"integer\",\"format\":\"int32\"},\"summary\":{\"type\":[\"null\",\"string\"]}}}}},\"weatherForecast\":{\"id\":\"weatherForecast\",\"title\":\"WeatherForecast\",\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"date\":{\"type\":\"string\",\"format\":\"date-time\"},\"temperatureC\":{\"type\":\"integer\",\"format\":\"int32\"},\"temperatureF\":{\"type\":\"integer\",\"format\":\"int32\"},\"summary\":{\"type\":[\"null\",\"string\"]}}}},\"messages\":{\"WeatherForecast[]\":{\"payload\":{\"$ref\":\"#/components/schemas/weatherForecastof\"},\"contentType\":\"application/json\",\"name\":\"WeatherForecast[]\",\"title\":\"WeatherForecast[]\",\"tags\":[{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"bindings\":{\"amqp\":{\"contentEncoding\":\"UTF-8\",\"messageType\":\"WeatherForecast[]\"}}},\"WeatherForecast\":{\"payload\":{\"$ref\":\"#/components/schemas/weatherForecast\"},\"contentType\":\"application/json\",\"name\":\"WeatherForecast\",\"title\":\"WeatherForecast\",\"tags\":[{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"bindings\":{\"amqp\":{\"contentEncoding\":\"UTF-8\",\"messageType\":\"WeatherForecast\"}}}},\"correlationIds\":{\"default\":{\"description\":\"Default Correlation ID\",\"location\":\"$message.header#/correlationId\"}}}}");
        }

        [Fact]
        public async Task CheckStateOfReceiverApp()
        {
            var factory = new ReceiverAppFactory();
            var client = factory.CreateClient();
            var response = await client.GetAsync("/asyncapi/asyncapi.json");
            response.StatusCode.Should().Be(StatusCodes.Status200OK);

            var json = await response.Content.ReadAsStringAsync();

            json.Should().Be("{\"asyncapi\":\"2.1.0\",\"info\":{\"title\":\"Receiver API\",\"version\":\"1.0.0\",\"description\":\"Sample receiver project\"},\"servers\":{\"sb://yourconnection.servicebus.windows.net/\":{\"url\":\"sb://yourconnection.servicebus.windows.net/\",\"protocol\":\"amqp\"}},\"defaultContentType\":\"application/json\",\"channels\":{\"myqueue\":{\"subscribe\":{\"operationId\":\"sub/myqueue\",\"summary\":\"myqueue\",\"description\":\"Reception of messages through the myqueue Queue\",\"tags\":[{\"name\":\"Queue\",\"description\":\"The type of client\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}},{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"message\":{\"oneOf\":[{\"$ref\":\"#/components/messages/WeatherForecast[]\"}]}},\"bindings\":{\"amqp\":{\"is\":\"queue\",\"queue\":{\"name\":\"myqueue\",\"durable\":true,\"exclusive\":false,\"autoDelete\":false}}}},\"mytopic/Subscriptions/mysubscription\":{\"subscribe\":{\"operationId\":\"sub/mytopic/Subscriptions/mysubscription\",\"summary\":\"mytopic/Subscriptions/mysubscription\",\"description\":\"Reception of messages through the mytopic/Subscriptions/mysubscription Subscription\",\"tags\":[{\"name\":\"Subscription\",\"description\":\"The type of client\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}},{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"message\":{\"oneOf\":[{\"$ref\":\"#/components/messages/WeatherForecast\"},{\"$ref\":\"#/components/messages/UserCreated\"},{\"$ref\":\"#/components/messages/UserPreferencesUpdated\"}]}},\"bindings\":{\"amqp\":{\"is\":\"queue\",\"queue\":{\"name\":\"mytopic/Subscriptions/mysubscription\",\"durable\":true,\"exclusive\":false,\"autoDelete\":false}}}},\"mytopic/Subscriptions/mysecondsubscription\":{\"subscribe\":{\"operationId\":\"sub/mytopic/Subscriptions/mysecondsubscription\",\"summary\":\"mytopic/Subscriptions/mysecondsubscription\",\"description\":\"Reception of messages through the mytopic/Subscriptions/mysecondsubscription Subscription\",\"tags\":[{\"name\":\"Subscription\",\"description\":\"The type of client\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}},{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"message\":{\"oneOf\":[{\"$ref\":\"#/components/messages/WeatherForecast\"}]}},\"bindings\":{\"amqp\":{\"is\":\"queue\",\"queue\":{\"name\":\"mytopic/Subscriptions/mysecondsubscription\",\"durable\":true,\"exclusive\":false,\"autoDelete\":false}}}}},\"components\":{\"schemas\":{\"weatherForecastof\":{\"id\":\"weatherForecastof\",\"title\":\"WeatherForecast[]\",\"type\":\"array\",\"items\":{\"$ref\":\"#/components/schemas/weatherForecastof/definitions/weatherForecast\"},\"definitions\":{\"weatherForecast\":{\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"date\":{\"type\":\"string\",\"format\":\"date-time\"},\"temperatureC\":{\"type\":\"integer\",\"format\":\"int32\"},\"temperatureF\":{\"type\":\"integer\",\"format\":\"int32\"},\"summary\":{\"type\":[\"null\",\"string\"]}}}}},\"weatherForecast\":{\"id\":\"weatherForecast\",\"title\":\"WeatherForecast\",\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"date\":{\"type\":\"string\",\"format\":\"date-time\"},\"temperatureC\":{\"type\":\"integer\",\"format\":\"int32\"},\"temperatureF\":{\"type\":\"integer\",\"format\":\"int32\"},\"summary\":{\"type\":[\"null\",\"string\"]}}},\"userCreated\":{\"id\":\"userCreated\",\"title\":\"UserCreated\",\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"userId\":{\"type\":[\"null\",\"string\"]}}},\"userPreferencesUpdated\":{\"id\":\"userPreferencesUpdated\",\"title\":\"UserPreferencesUpdated\",\"type\":\"object\",\"additionalProperties\":false,\"properties\":{\"sendNotifications\":{\"type\":\"boolean\"}}}},\"messages\":{\"WeatherForecast[]\":{\"payload\":{\"$ref\":\"#/components/schemas/weatherForecastof\"},\"contentType\":\"application/json\",\"name\":\"WeatherForecast[]\",\"title\":\"WeatherForecast[]\",\"tags\":[{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"bindings\":{\"amqp\":{\"contentEncoding\":\"UTF-8\",\"messageType\":\"WeatherForecast[]\"}}},\"WeatherForecast\":{\"payload\":{\"$ref\":\"#/components/schemas/weatherForecast\"},\"contentType\":\"application/json\",\"name\":\"WeatherForecast\",\"title\":\"WeatherForecast\",\"tags\":[{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"bindings\":{\"amqp\":{\"contentEncoding\":\"UTF-8\",\"messageType\":\"WeatherForecast\"}}},\"UserCreated\":{\"payload\":{\"$ref\":\"#/components/schemas/userCreated\"},\"contentType\":\"application/json\",\"name\":\"UserCreated\",\"title\":\"UserCreated\",\"tags\":[{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"bindings\":{\"amqp\":{\"contentEncoding\":\"UTF-8\",\"messageType\":\"UserCreated\"}}},\"UserPreferencesUpdated\":{\"payload\":{\"$ref\":\"#/components/schemas/userPreferencesUpdated\"},\"contentType\":\"application/json\",\"name\":\"UserPreferencesUpdated\",\"title\":\"UserPreferencesUpdated\",\"tags\":[{\"name\":\"Ev.ServiceBus\",\"description\":\"Generated by Ev.ServiceBus\",\"externalDocs\":{\"description\":\"Ev.ServiceBus repository\",\"url\":\"https://github.com/EcovadisCode/Ev.ServiceBus\"}}],\"bindings\":{\"amqp\":{\"contentEncoding\":\"UTF-8\",\"messageType\":\"UserPreferencesUpdated\"}}}},\"correlationIds\":{\"default\":{\"description\":\"Default Correlation ID\",\"location\":\"$message.header#/correlationId\"}}}}");
        }
    }
}
