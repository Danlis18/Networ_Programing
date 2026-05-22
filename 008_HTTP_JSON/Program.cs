
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

/*string keyApi = "0844a69f62c606c23b3d7fe28770fcf9";
string city = "Lviv";

string uri = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={keyApi}&mode=xml&units=metric";

HttpClient client = new HttpClient();

string data = await client.GetStringAsync(uri);
Console.WriteLine(data);

JObject weatherData = JObject.Parse(data);
Console.WriteLine((string)weatherData["main"][0]["temp"]);*/
string baseCurrency = "USD";
string apiKey = "fxr_live_9f3bedbca8ed6670f3a13671178762ccd448";
string uri = $"https://api.fxratesapi.com/latest?apikey={apiKey}&base={baseCurrency}";

HttpClient client = new HttpClient();

string data = await client.GetStringAsync(uri);
Console.WriteLine(data);

JObject exchangeData = JObject.Parse(data);
/*Console.WriteLine((string)exchangeData["base"]);
Console.WriteLine((string)exchangeData["date"]);
Console.WriteLine((string)exchangeData["rates"]["EUR"]);*/

JObject rates = (JObject)exchangeData["rates"]!;

string baseCode = (string)exchangeData["base"]!;
string dateRaw = (string)exchangeData["date"]!;

Console.WriteLine("КУРСИ ВАЛЮТ");
Console.WriteLine($"Базова валюта: {baseCode}");

foreach (var rate in rates)
{
    string currency = rate.Key;
    decimal value = (decimal)rate.Value!;
    Console.WriteLine($"1 {baseCode} = {value:0.####} {currency}");
}


//Навчання користування пакетом Newtonsoft.Json для роботи з JSON даними:
/*JObject json = JObject.Parse(@"{
    'CPU' : 'Intel',
    'RAM' : '16GB',
    'Storage' : '1TB',
    'Core' : 8,
    'Drives' : [
        'DVD read/writer',
        '500 gigabyte hard drive',
        '200 gigabyte hard drive'
    ],
    'input' : {
        'mouse' : 'Logitech',
        'keyboard' : 'Microsoft'
    }
}");*/
/*string ram = (string)json["RAM"];
Console.WriteLine(ram);

string Drives = (string)json["Drives"][0];
Console.WriteLine(Drives);

string mouse = (string)json["input"]["mouse"];
Console.WriteLine(mouse);

//вивід всього:
json["Drives"].Select(drive => (string)drive.ToString());
foreach (var drive in json["Drives"])
{
    Console.WriteLine(drive);
}*/