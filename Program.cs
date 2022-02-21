using StatBlocks;
using System.IO;

DirectoryInfo dir = new(@"C:\Users\theri\github\Deaoner\inputs");

foreach (FileInfo file in dir.GetFiles())
{
    if (file.Extension == ".txt")
    {
        Creature creature = new(System.IO.File.ReadAllLines(file.FullName));
        Task task = creature.WriteToFile($@"C:\Users\theri\Documents\Website\pathfinder\_creatures\{creature.Name}.md");
        Console.WriteLine("Written " + creature.Name);
    }
}