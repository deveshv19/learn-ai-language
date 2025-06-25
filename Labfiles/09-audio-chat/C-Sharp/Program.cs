using System;
using Azure;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

 // Add references
 using Azure.Identity;
 using Azure.AI.Projects;
 using Azure.AI.Inference;


namespace chat_app
{
    class Program
    {
        static void Main(string[] args)
        {
            // Clear the console
            Console.Clear();
            
            try
            {
                // Get configuration settings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string project_connection = configuration["PROJECT_ENDPOINT"];
                string model_deployment = configuration["MODEL_DEPLOYMENT"];



                 // Initialize the project client
                DefaultAzureCredentialOptions options = new() { 
                    ExcludeEnvironmentCredential = true,
                    ExcludeManagedIdentityCredential = true
                };
                var projectClient = new AIProjectClient(
                    new Uri(project_connection),
                    new DefaultAzureCredential(options));



                 // Get a chat client
                ChatCompletionsClient chat = projectClient.GetChatCompletionsClient();




                // Initialize prompts
                string system_message = "You are an AI assistant for a produce supplier company.";
                string prompt = "";

                // Loop until the user types 'quit'
                while (prompt.ToLower() != "quit")
                {
                    // Get user input
                    Console.WriteLine("\nAsk a question about the audio\n(or type 'quit' to exit)\n");
                    prompt = Console.ReadLine().ToLower();
                    if (prompt == "quit")
                    {
                        break;
                    }
                    else if (prompt.Length < 1)
                    {
                        Console.WriteLine("Please enter a question.\n");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Getting a response ...\n");


                         // Get a response to audio input
                        string audioUrl = "https://github.com/MicrosoftLearning/mslearn-ai-language/raw/refs/heads/main/Labfiles/09-audio-chat/data/fresas.mp3";
                        var requestOptions = new ChatCompletionsOptions()
                        {
                            Model = model_deployment,
                            Messages =
                            {
                                new ChatRequestSystemMessage(system_message),
                                new ChatRequestUserMessage(
                                    new ChatMessageTextContentItem(prompt),
                                    new ChatMessageAudioContentItem(new Uri(audioUrl))),
                            }
                        };
                        var response = chat.Complete(requestOptions);
                        Console.WriteLine(response.Value.Content);
                        

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
