using StatBlocks;
using System.IO;
using System.Net;

DirectoryInfo inputs = new(@"C:\Users\theri\Documents\GitHub\StatBlocks\inputs\");
DirectoryInfo outputs = new($@"C:\Users\theri\Documents\GitHub\StatBlocks\result\");

foreach (FileInfo file in inputs.GetFiles(".txt"))
{
    string[] fileAsString = File.ReadAllLines(file.FullName);
    List<string> fileAsList = new List<string>();
    foreach (string i in fileAsString)
    {
        fileAsList.Add(i);
    }
    Creature creature = new(fileAsList);


    // Task task = creature.WriteToFile($@"C:\Users\theri\Documents\GitHub\StatBlocks\result\{creature.Name}.md");
    // Console.WriteLine("Written " + creature.Name);
}