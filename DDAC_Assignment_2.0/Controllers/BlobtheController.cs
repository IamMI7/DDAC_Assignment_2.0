using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Controllers
{
    public class BlobtheController : Controller
    {
        //Retrieve Blob Information
        private CloudBlobContainer getBlobStorageInformation(string blobname)
        {
            //step 1: read json
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();
            //to get key access
            //once link, time to read the content to get the connectionstring
            CloudStorageAccount objectaccount = CloudStorageAccount.Parse(configure["ConnectionStrings:BlobConnection"]);
            CloudBlobClient blobclient = objectaccount.CreateCloudBlobClient();
            //step 2: how to create a new container in the blob storage account.
            CloudBlobContainer container = blobclient.GetContainerReference(blobname);
            return container;
        }

        //Create blob if does not exist
        public bool CreateBlobContainer(string blobname)
        {
            CloudBlobContainer container = getBlobStorageInformation(blobname);
            container.CreateIfNotExistsAsync();
            return true;
        }

        //Upload File to Blob
        public String UploadBlob(IFormFile images, string filename, string blobname)
        {
            CloudBlobContainer container = getBlobStorageInformation(blobname);

            CloudBlockBlob blob = container.GetBlockBlobReference(filename);

            using (var fileStream = images.OpenReadStream())
            {
                blob.UploadFromStreamAsync(fileStream).Wait();
            }

            return "Success!";
        }

        //Retrieve Materials Image
        public List<String> GetMaterialsImage(List<String> model, string blobname)
        {
            CloudBlobContainer container = getBlobStorageInformation(blobname);

            List<string> blobs = new List<string>();

            BlobResultSegment result = container.ListBlobsSegmentedAsync(null).Result;

            foreach (IListBlobItem item in result.Results)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    //Query Images
                    foreach (var x in model)
                    {
                        if (blob.Name == x)
                        {
                            blobs.Add(blob.Name + "#" + blob.Uri.ToString());
                        }
                    }

                }
                else
                {
                    return model;
                }
            }
            return blobs;
        }

        //Retrieve user uploaded Ideas Images
        public List<String> getBlobFileLink(List<String> model, string blobname)
        {
            CloudBlobContainer container = getBlobStorageInformation(blobname);

            List<string> blobs = new List<string>();

            BlobResultSegment result = container.ListBlobsSegmentedAsync(null).Result;

            foreach (IListBlobItem item in result.Results)
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    //Query Images
                    foreach (var x in model)
                    {
                        if (blob.Name == x)
                        {
                            blobs.Add(blob.Uri.ToString());
                        }
                    }

                }
                else
                {
                    return model;
                }
            }
            return blobs;
        }
    }
    
}
