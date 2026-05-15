using _008_HTTP;

//HttpClient httpClient = new HttpClient();
//string uri = "https://www.google.com/";

/*using HttpRequestMessage request =
    new HttpRequestMessage(HttpMethod.Get, uri);    // Get, Post, Put, Delete, Patch

using HttpResponseMessage response = await httpClient.SendAsync(request);

Console.WriteLine(response.StatusCode);

Console.WriteLine("Headers!");
foreach (var header in response.Headers)
{
    Console.Write($"{header.Key}: ");

    foreach (var headerValue in header.Value)
    {
        Console.WriteLine(headerValue);
    }
}

Console.WriteLine("Content");

string content = await response.Content.ReadAsStringAsync();

Console.WriteLine(content);*/


//Дастаємо з сайту код сторінки - варіант 1
/*using HttpResponseMessage response = await httpClient.GetAsync(uri);
string content = await response.Content.ReadAsStringAsync();
Console.WriteLine(content);*/


//Дастаємо з сайту код сторінки - варіант 2
/*string content = await httpClient.GetStringAsync(uri);
Console.WriteLine(content);*/


//Завантаження файлна з брузара
/*HttpClient httpClient = new HttpClient();
//string uri = "https://www.gutenberg.org/cache/epub/2265/pg2265.txt";
string uri = "https://cdn.pixabay.com/photo/2022/04/15/07/58/sunset-7133867_1280.jpg";

byte[] data = await httpClient.GetByteArrayAsync(uri);
string path = @"C:\Users\danil\OneDrive\Робочий стіл\TestFile\book.txt";
File.WriteAllBytes(path, data);*/



//Підтягуємо та розпаршуємо інформацію HTML 
HttpClient httpClient = new HttpClient();
string uri = "http://resources.finance.ua/ua/public/metal-cash.xml";

string conect = await httpClient.GetStringAsync(uri);
//Console.WriteLine(conect);
Source source = Serializer.Deserialize<Source>(conect);

//List<Organization> organizations = source.Organizations.Organization;

//foreach (Organization org in organizations)
//{
//    Console.WriteLine(org.Title);
//}


/*string bank = "Індустріалбанк";

var res = source.Organizations.Organization
    .Where((o) => o.Title == bank)
    //.Select(b => new
    //{
    //    Metals = b.Metals.Metal
    //})
    .FirstOrDefault();


List<Metal> metals = res.Metals.Metal;

foreach (var m in *//*res.Metals*//*metals)
{
    Console.WriteLine(m.Id);

    foreach (var item in m.N)
    {
        Console.WriteLine($"Id: {item.Id} | Br: {item.Br} | Ar: {item.Ar}");
    }
}*/
