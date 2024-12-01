using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServiceReferenceWSExemplo;

namespace ConsoleConsomeWS
{
    public class ProcessamentoXMLPorWSTiss
    {
        private readonly IConfiguration _configuration;

        public ProcessamentoXMLPorWSTiss(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> EnviarXMLAsync(string xml)
        {
            // Recuperar o endpoint do appsettings.json
            var endpointAddress = _configuration["WSTiss:Endpoint"];

            if (string.IsNullOrEmpty(endpointAddress))
                throw new Exception("Endpoint do WebService TISS não configurado.");

            // Configuração do binding
            var binding = new BasicHttpBinding
            {
                Security = new BasicHttpSecurity
                {
                    Mode = BasicHttpSecurityMode.Transport // HTTPS
                }
            };

            var endpoint = new EndpointAddress(endpointAddress);

            using var client = new WebService1SoapClient(binding, endpoint);

            try
            {
                // Chamar o método do WebService
                var response = await client.BemVindoAsync("JEfferson Rain");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao enviar XML para o WebService: {ex.Message}", ex);
            }
        }
    }
}
