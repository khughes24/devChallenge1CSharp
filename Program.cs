using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using Newtonsoft.Json;


namespace devProject1
{
    class Program
    {
        //create browser object to use for nuget webscraper
        static ScrapingBrowser _browser = new ScrapingBrowser();
        static void Main(string[] args)
        {
            var results = new List<SiteItem>();
            //can access page using System methods
            String url = "http://devtools.truecommerce.net:8080/challenge001/";
            String returnOutput = "";
            using (System.Net.WebClient client = new System.Net.WebClient()){
                returnOutput = client.DownloadString(url);
            }
            Console.WriteLine(returnOutput);


            //But lets use ScrapySharp instead for sanities sake as i dont want to build whats already in this nuget package
            var stringLinks = new List<string>();
            
            //Get the list of strings
            stringLinks = GetMainPageLinks(url);

            //loop through all the strings to output them
            // and do stuff with them
            foreach (var link in stringLinks){
                Console.WriteLine(link);
                results.Add(GetItemDetails(link));
                
            }
            string json = JsonConvert.SerializeObject(results, Formatting.Indented);

            Console.WriteLine(json);

        }

        static HtmlNode GetHtml(string url){
            WebPage webPage = _browser.NavigateToPage(new Uri(url));
            return webPage.Html;
        }

        static SiteItem GetItemDetails(string url){
            SiteItem siteItem = new SiteItem();
            String baseUrl = "http://devtools.truecommerce.net:8080";
            String combiUrl = baseUrl + url;
            String htmlRaw = "";
            using (System.Net.WebClient client = new System.Net.WebClient()){
                htmlRaw = client.DownloadString(combiUrl);
            }

            var html = GetHtml(combiUrl);
            var ptext = html.CssSelect("p");
            foreach (var item in ptext){
                foreach (var subItem in item.Attributes){
                    Console.WriteLine(subItem);
                    if(subItem.Value.Contains("productDescription1")){
                        siteItem.ItemName = subItem.OwnerNode.InnerText.Trim();
                    }
                }
            }
            ptext = html.CssSelect("span");
            foreach (var item in ptext){
                foreach (var subItem in item.Attributes){
                    Console.WriteLine(subItem);
                    if(subItem.Value.Contains("productItemCode")){
                        siteItem.ItemCode = subItem.OwnerNode.InnerText;
                    }
                }
            }
            ptext = html.CssSelect("span");
            foreach (var item in ptext){
                foreach (var subItem in item.Attributes){
                    Console.WriteLine(subItem);
                    if(subItem.Value.Contains("productUnitPrice")){
                        siteItem.ItemCost = subItem.OwnerNode.InnerText;
                    }
                }
            }
            foreach (var item in ptext){
                foreach (var subItem in item.Attributes){
                    Console.WriteLine(subItem);
                    if(subItem.Value.Contains("productDescription2")){
                        siteItem.ItemDesc = subItem.OwnerNode.InnerText;
                    }
                }
            }
            foreach (var item in ptext){
                foreach (var subItem in item.Attributes){
                    Console.WriteLine(subItem);
                    if(subItem.Value.Contains("productKcalPer100Grams")){
                        siteItem.ItemKal = subItem.OwnerNode.InnerText;
                    }
                }
            }

            
            return siteItem;
        }

        static List<string> GetMainPageLinks(string url){
            
            var homePageLinks = new List<string>();
            var html = GetHtml(url);
            var links = html.CssSelect("a");

            foreach (var item in links){
                foreach (var subItem in item.Attributes){
                    if(subItem.Name == "href"){
                        if(homePageLinks.Contains(subItem.Value) == false){ //check to see if we already have this value in the list
                            homePageLinks.Add(subItem.Value); //we dont so let add it
                        }
                        
                    }
                }
            }
            return homePageLinks;
        }




    }
}
