using Azure.Messaging.ServiceBus;
using Lampros.Services.EmailAPI.Message;
using Lampros.Services.EmailAPI.Models.Dto;
using Lampros.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Lampros.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedEmailSubscription;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _emailOrderPlacedProcessor;
        private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
            orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedEmailSubscription = configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedEmailSubscription);
            
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailOrderPlacedProcessor.StartProcessingAsync();
        }

        
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _emailOrderPlacedProcessor.StopProcessingAsync();
            await _emailOrderPlacedProcessor.DisposeAsync();
        }
        private async Task OnEmailCartReceived(ProcessMessageEventArgs args)
        {
            //This is where you receive the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto cartMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                //TODO: try to log message
                await _emailService.EmailCartAndLog(cartMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            //This is where you receive the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //TODO: try to log message
                await _emailService.LogOrderPlaced(rewardsMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            //This is where you receive the message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //TODO: try to log message
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


    }
}
