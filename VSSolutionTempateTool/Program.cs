using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            string templateToken = "TworkWebApplicationStarter";


            List<string> extensionsToProcess = new List<string>
            {
                ".config"
                ,".xml"
                ,".js"
                ,".css"
                ,".txt"
                ,".cs"
                ,".csproj"
                ,".asax"
                ,".user"
                ,".cshtml"
                ,".sln"
            };

            List<string> foldersToSkip = new List<string>
            {
                "bin"
                ,"obj"
            };

            Console.Title = "Twork Technology Solution Template Tool";

            Console.Write("Enter template folder path >");

            string templateFolderPath = Console.ReadLine();

            bool pathExists = System.IO.Directory.Exists(templateFolderPath);

            if (!pathExists)
            {

                Console.WriteLine("the path does not exist");

                Console.WriteLine("Press enter to exit >");

                Console.ReadLine();

                return;
            }

            Console.WriteLine("Enter output folder path >");

            string outputFolderInput = Console.ReadLine();

            pathExists = System.IO.Directory.Exists(outputFolderInput);

            if (!pathExists)
            {
               
                Console.WriteLine("the path does not exist");
                Console.WriteLine("Would you like to create it? (y/n)>");
                if (Console.ReadLine().ToLower() == "y")
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(outputFolderInput);
                    }
                    catch (Exception)
                    {

                        Console.WriteLine("Could not create {0}");
                        Console.WriteLine("Press enter to exit >");

                        Console.ReadLine();
                        return;
                    }
                    
                }
                else
                {
                    Console.WriteLine("Press enter to exit >");

                    Console.ReadLine();

                    return;
                }
            }

            string outputFolderPath = string.IsNullOrWhiteSpace(outputFolderInput) ? System.IO.Path.Combine(templateFolderPath, "OutputFolder") : outputFolderInput;

            Console.WriteLine("Enter your solution name replacement token>");
            
            Console.WriteLine("Press <Enter> to accept the default ({0})>",templateToken);

            string inputToken = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(inputToken))
            {
            
                templateToken = inputToken;
            
            }

            Console.WriteLine("Looking for solution file . . .");

            IEnumerable<string> files = System.IO.Directory.EnumerateFiles(templateFolderPath, "*.sln");

            if (files.Count() == 1)
            {

                string fileName = System.IO.Path.GetFileName(files.Single());

                Console.WriteLine("Solution file {0} was found in the folder {1}.", fileName, templateFolderPath);

                Console.Write("Enter new solution name >");

                string newSolutionName = Console.ReadLine();

                Console.WriteLine("Processing the {0} template . . . ", fileName);

                Console.WriteLine("Creating the {0} solution from the {1} template . . .", newSolutionName, fileName);

                string[] directoryNames = System.IO.Directory.GetDirectories(templateFolderPath, "*", System.IO.SearchOption.AllDirectories);

                List<string> directories = directoryNames.OrderBy(s => s).ToList();
            
                ProcessDirectory(templateFolderPath, templateFolderPath, outputFolderPath, newSolutionName, templateToken, extensionsToProcess, foldersToSkip);


                foreach (var directory in directories)
                {

                    ProcessDirectory(directory, templateFolderPath, outputFolderPath, newSolutionName, templateToken, extensionsToProcess, foldersToSkip);

                }

            }
            else
            {

                if (files.Count() == 0)
                {

                    Console.WriteLine("The folder {0} does not contain a solution file.", templateFolderPath);

                }
                else
                {

                    Console.WriteLine("The folder {0} contains more than 1 solution file.\n {1} files found.\nThis is not permitted.", templateFolderPath, files.Count());

                }

                Console.WriteLine("Press enter to exit >");

                Console.ReadLine();

                return;

            }

            Console.WriteLine("Press enter to exit >");

            Console.ReadLine();

        }

        private static void ProcessDirectory(string directory, string templatePath, string outputFolderPath, string newName, string templateToken, List<string> extensionsToProcess, List<string> foldersToSkip)
        {
            string newDirectoryName = directory.Replace(templatePath, outputFolderPath);

            if (foldersToSkip.Any(folderName => directory.Contains(folderName)))
            {

                Console.WriteLine("Skipping {0} \n\n\n", directory);

                return;

            }

            Console.WriteLine("Processing {0} . . . ", directory);

            if (directory.Contains(templateToken))
            {

                newDirectoryName = newDirectoryName.Replace(templateToken, newName);

                Console.WriteLine("Renaming. ");

            }

            Console.WriteLine("Creating the folder {0}", newDirectoryName);

            System.IO.Directory.CreateDirectory(newDirectoryName);

            Console.WriteLine("Processing files . . . ");

            List<string> fileNames = System.IO.Directory.GetFiles(directory).OrderBy(d => d).ToList();

            foreach (var processingFileName in fileNames)
            {

                string extension = System.IO.Path.GetExtension(processingFileName);

                string fileNameToCreate = System.IO.Path.GetFileName(processingFileName).Replace(templateToken, newName);

                string fileToCreate = System.IO.Path.Combine(newDirectoryName, fileNameToCreate);

                if (extensionsToProcess.Contains(extension))
                {

                    int replacements = 0;

                    Console.WriteLine("\tProcessing {0} . . . ", fileNameToCreate);

                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.File.Create(fileToCreate)))
                    {

                        string line;

                        using (System.IO.StreamReader reader = new System.IO.StreamReader(processingFileName))
                        {

                            while ((line = reader.ReadLine()) != null)
                            {

                                if (line.Contains(templateToken))
                                {
                                    Console.WriteLine(line);

                                    line = line.Replace(templateToken, newName);

                                    replacements++;

                                    Console.WriteLine(line);
                                }

                                writer.WriteLine(line);

                            }

                        }

                    }

                    Console.WriteLine("\t{0} lines with token altered.", replacements);

                }
                else
                {

                    Console.WriteLine("Copying {0}  ", fileNameToCreate);

                    System.IO.File.Copy(processingFileName, fileToCreate, true);

                }



            }

            Console.WriteLine("\n\n");
        }
    }
}
