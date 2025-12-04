using Owin;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using ProtectedCommon;

namespace ProtectedServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Signature.Configure(@"<RSAKeyValue>
    <Modulus>tY8cCx5hU3yKtajFtqfLPcM5gdXKTLai6YSDmEqNSLJbEKL6qEg952Q1qo+tb1mO+uqsIiWUK8q0WS1BoH1BeC9mULWAl68SJcNcmyG2YMn1glqIvJ4C1b9M67e2PC4ZbSG5/jJtb1s0Kr9gzpshqeCPtNjw29lpJ4tadLNfAWU=</Modulus>
    <Exponent>AQAB</Exponent>
    <P>3AaEsuJC9BQeTWylqIC49y9tVhvuLtVwYZKoo0nryV+uoJy5/JieM0QnlVzstIhWw/bcVrrBmmgI19LddCdkuQ==</P>
    <Q>0z6F6OFkjRVeQ0cVg+0NNxxnQrQVn7ZrNGtAvg3rXaKRCSraZt9G36aekCVRy1Bh3evRJ7riMgEcE8Fu4CwEDQ==</Q>
    <DP>tVpiEhfQ2+GhQGvm90ZyLrvWwPzwi4W9xY7elQie4jKNezDzU7Jv4w2wGrqnF/6wlYFqB8qTPTO25j2V7uFxcQ==</DP>
    <DQ>CN0wxUrf60OgRvZuorCJw2w/sP7ZgXAoI3T0rITtAWrW5ymTLInl8XCOasIGIp/m22cPybj/0NVXFkUhn+p46Q==</DQ>
    <InverseQ>r70boMxtNmROzrtOc2Jp/PPUC6y65NhJ1PXRi1PH1ymghpTJyoGN1tafI5+/h4AnFId8i4OPD8s6wIYxLycJOA==</InverseQ>
    <D>Ef4adpyeYw5s1adh3qHe0aJa/Nuxxmv0FaXI/8rlmP556VppcktkfR2wdtRxyN7sfT/L6r9kIXnfaRYD0rKPDf4RLhgHYhr+m2P2mek+Nno2NM7FrmT7ArwcxRvHk4YCHUKtjqBQs5FJ2rMn/AqELko6jgfAvdOF/HyWf/I5XTk=</D>
</RSAKeyValue>");
            Console.WriteLine(Signature.CreateClientConfig());


            string baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Program>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                HttpClient client = new HttpClient();

                var response = client.GetAsync(baseAddress + "api/configurations/License").Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.ReadLine();
            }
        }

        public void Configuration(IAppBuilder appBuilder) {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    }
}
