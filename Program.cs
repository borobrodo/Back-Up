using System;
using System.IO;
//using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BackUp 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var settings = config.GetRequiredSection("Settings");
            //string testPath = config.GetRequiredSection("Settings").GetValue<String>("SourcePath");

            string sourcePath = settings.GetValue<String>("SourcePath");
            DirectoryInfo source = new DirectoryInfo(sourcePath);

            string destinationPath = settings.GetValue<String>("DestinationPath");
            DateTime dateNow = DateTime.Now;
            string timeStampFolderName = source.Name + "_" + dateNow.ToString("yyyy-MM-ddTHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            string timeStampFolderPath = Path.Combine(destinationPath, timeStampFolderName);
            DirectoryInfo timeStampFolder = new DirectoryInfo(timeStampFolderPath);
            
            Copy(source, timeStampFolder);

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        public static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public static void Copy(DirectoryInfo source, DirectoryInfo destination, bool recurse = true)
        {
            if (source.FullName.ToLower() == destination.FullName.ToLower()) 
            {
                Console.WriteLine($"Destination path [{destination.FullName}] is the same as the source path [{source.FullName}]. Exiting");
                return;
            }

            if (!(source.Exists)) 
            { 
                Console.WriteLine($"Path [{source.FullName}] doesn't exist. Nothing to copy");
                return;
            }

            if (!(destination.Exists)) 
            {
                Directory.CreateDirectory(destination.FullName);
                Console.WriteLine($"Folder [{destination.FullName}] has been created");
            }
            else
            {
                if (!IsDirectoryEmpty(destination.FullName))
                {
                    Console.WriteLine($"Folder [{destination.FullName}] already exists and is not empty. Exiting");
                    // add code to ask user if he wants to erase destination folder's content
                    return;
                }
            }

            foreach (FileInfo file in source.GetFiles()) 
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name));
                Console.WriteLine($"Copied [{file.Name}] to [{destination.FullName}]");
            }

            foreach (DirectoryInfo sourceSubFolder in source.GetDirectories())
            {
                string destinationSubFolderPath = Path.Combine(destination.FullName, sourceSubFolder.Name);
                DirectoryInfo destinationSubFolder = new DirectoryInfo(destinationSubFolderPath);
                Copy(sourceSubFolder, destinationSubFolder);
            }
        }
    }
}
