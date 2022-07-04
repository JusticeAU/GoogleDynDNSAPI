using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net.Http;
using System.Net.Http.Headers;


namespace GoogleDynDNSAPI
{
    internal class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            string IPAddress = "";
            string credentials = "";
            string domain = "";

            // If no configuration file, it must be generated.
            if(!File.Exists("config.cfg"))
            {
                // Create file
                Console.WriteLine("Configuration file missing. Please enter details:");

                Console.Write("Username: ");
                credentials += Console.ReadLine();
                credentials += ":";

                Console.Write("Password: ");
                credentials += Console.ReadLine();

                Console.Write("Host Name: ");
                domain = Console.ReadLine();

                Console.WriteLine("Building config.cfg");
                using (StreamWriter sw = new StreamWriter("config.cfg"))
                {
                    // Store credentials as Base64 encoded string.
                    sw.WriteLine(System.Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));
                    sw.WriteLine(domain);
                }
            }

            // Get current IP
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://domains.google.com/checkip");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                IPAddress = responseBody;
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

            // Check if its changed
            string lastIP = "";
            if(File.Exists("lastIP.cfg"))
            {
                using (StreamReader sr = new StreamReader("lastIP.cfg"))
                    lastIP = sr.ReadLine();
            }

            if (lastIP != IPAddress)
            {
                Console.WriteLine("IP Address has changed. Updating Google DNS.");

                // load data from file
                using (StreamReader sr = new StreamReader("config.cfg"))
                {
                    credentials = sr.ReadLine();
                    domain = sr.ReadLine();
                }

                // Update DNS
                string URL = "https://domains.google.com/nic/update?hostname=" + domain + "&myip=" + IPAddress;

                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    HttpResponseMessage response = await client.GetAsync(URL);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }

                // Update last IP
                using (StreamWriter sw = new StreamWriter("lastIP.cfg"))
                    sw.WriteLine(IPAddress);
            }
            else
                Console.WriteLine("IP Address has not changed.");
        }
    }
}
