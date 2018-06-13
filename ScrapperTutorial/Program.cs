using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace ScrapperTutorial
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            //Url completa http://www.rio.rj.gov.br/web/arquivogeral/principal

            var client = new RestClient("http://www.rio.rj.gov.br"); // Cliente que executara nossos requests;  

            var request = new RestRequest // Objeto de request;
            {
                Resource = "web/arquivogeral/principal" // recurso do site que iremos utilizar
            };

            var result = await client.ExecuteTaskAsync(request); // executar o request;


            var document = new HtmlDocument(); // nosso pagina;
            document.LoadHtml(result.Content); //carregando o html na nossa pagina;

            var postNodes = document.DocumentNode.SelectNodes("//div[contains (@class,'aui-w33')]"); // selecionando todos os nós de postagens via XPath


            var list = new List<ArchiveItem>(); //Criar lista de arquivos
            foreach (var post in postNodes)
            {
                var archiveItem = new ArchiveItem(); // Criar item de arquivo
                archiveItem.Image = "http://www.rio.rj.gov.br" + post.SelectSingleNode(post.XPath + "//img").Attributes["src"].Value; // Concatenando site mas path da imagem
                var titleNode = post.SelectSingleNode(post.XPath + "//p[@class='titulo3col3']"); //pegar nó do titulo
                archiveItem.Name = HttpUtility.HtmlDecode(titleNode.InnerText); // setar nome tirando encode de html
                archiveItem.Url = "http://www.rio.rj.gov.br" + titleNode.SelectSingleNode(titleNode.XPath + "//a").Attributes["href"].Value; // setar url
                archiveItem.Detail = HttpUtility.HtmlDecode(post.SelectSingleNode(post.XPath + "//div[@class='chamada3col3']").InnerText);// setar detalhe tirando encode de html

                list.Add(archiveItem); //Adcionando item a lista de arquivos;
            }

            Console.Read();
        }
    }
}
