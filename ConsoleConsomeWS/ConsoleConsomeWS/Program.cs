using System;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;

// Definindo o contrato do serviço
[ServiceContract]
public interface IWebService1
{
    [OperationContract(Action = "http://tempuri.org/HelloWorld", ReplyAction = "*")]
    Task<string> HelloWorldAsync();

    [OperationContract(Action = "http://tempuri.org/SomarNumeros", ReplyAction = "*")]
    Task<int> SomarNumerosAsync(int num1, int num2);

    [OperationContract(Action = "http://tempuri.org/BemVindo", ReplyAction = "*")]
    Task<string> BemVindoAsync(string nome);
}

class Program
{
    static async Task Main(string[] args)
    {
        // Configuração do builder
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Construindo a configuração
        IConfiguration configuration = builder.Build();

        // Lendo o endpoint do webservice
        var webServiceEndpoint = configuration["WebService:Endpoint"];
        Console.WriteLine($"Endpoint do WebService: {webServiceEndpoint}");

        // Aqui você pode fazer a chamada ao webservice usando o endpoint
        // Definindo o endpoint do serviço
        //var endpointAddress = new EndpointAddress("https://localhost:44331/WebService1.asmx");
        var endpointAddress = new EndpointAddress(webServiceEndpoint);
        
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport); // Configurando para usar segurança
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None; // Ajuste conforme necessário
        // Criando o cliente
        var client = new ChannelFactory<IWebService1>(binding, endpointAddress);

        // Abrindo o canal
        var serviceClient = client.CreateChannel();

        try
        {
            // Chamada ao método HelloWorld
            var helloWorldResponse = await serviceClient.HelloWorldAsync();
            Console.WriteLine($"Resposta HelloWorld: {helloWorldResponse}");

            // Chamada ao método SomarNumeros
            int num1 = 5;
            int num2 = 10;
            var somarResponse = await serviceClient.SomarNumerosAsync(num1, num2);
            Console.WriteLine($"Resultado da soma: {somarResponse}");

            // Chamada ao método BemVindo
            string nome = "João";
            var bemVindoResponse = await serviceClient.BemVindoAsync(nome);
            Console.WriteLine($"Mensagem de boas-vindas: {bemVindoResponse}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            // Fechando o canal
            if (serviceClient is IClientChannel channel)
            {
                try
                {
                    if (channel.State == CommunicationState.Faulted)
                    {
                        channel.Abort();
                    }
                    else
                    {
                        channel.Close();
                    }
                }
                catch
                {
                    channel.Abort();
                }
            }
        }
    }
}