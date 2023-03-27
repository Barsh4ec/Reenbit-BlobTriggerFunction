using System;
using System.IO;
using System.Net.Mail;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System.Net;

namespace BlobTriggerFunc
{
    public class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([BlobTrigger("testtask/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=testreenbitstorage;AccountKey=yrM2jzrLoFWdxCR2NeAl4rPcSxNFe9OXi/WptXjVim4xenAF+qxJyopqBUNRlmRWIomtzVAoTJrm+AStNfefLQ==;EndpointSuffix=core.windows.net");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("testtask");
            var blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.FetchAttributesAsync();
            log.LogInformation($"C# Blob trigger function processed blob\nName: {name} \nEmail: {blockBlob.Metadata["email"]}");



            string fromMail = "reenbitdotnettesttask@gmail.com";
            string fromPassword = "kfoizjrpnumznlrd";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "File uploaded successfully";
            message.To.Add(new MailAddress(blockBlob.Metadata["email"]));
            message.Body = $"<html><body> The file {name} has been uploaded successfully. </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
    }
}
