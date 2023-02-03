using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TextEditor
{
   


    [Serializable]
    public class Animal
    {
        public string Name { get; set; }
        public int height { get; set; }
        public int age { get; set; }
        public Animal(string name, int height, int age)
        {
            Name = name;
            this.height = height;
            this.age = age;
        }

        public Animal() { }
    }

    class Serializer// Работа с файлами
    {

        private string Serialize(Animal animal)
        {
            string str_out = "";
            str_out += animal.Name + Environment.NewLine;
            str_out += animal.height + Environment.NewLine;
            str_out += animal.age + Environment.NewLine;
            return str_out;
        }

        private string Serialize(List<Animal> animals)
        {
            string str_out = "";
            for (int i = 0; i < animals.Count; i++)
            {
                str_out += Serialize(animals[i]);
            }
            return str_out;
        }


        private List<Animal> Deserialize(string path)
        {
            int count=0;
            StreamReader streamReader = new StreamReader(path);
            List<Animal> result = new List<Animal>();

            
            while (streamReader.ReadLine()!=null)
            {
                count++;
            }

            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            count = count / 3;
            for (int i = 0; i < count; i++)
            {
                string name = streamReader.ReadLine();
                string height = streamReader.ReadLine();
                string age = streamReader.ReadLine();
                Animal animal = new Animal(name, Int32.Parse(height), Int32.Parse(age));
                result.Add(animal);
            }
            return result;
        }
        private string SerializeJson(List<Animal> animals)
        {
            return JsonSerializer.Serialize<List<Animal>>(animals);
        }
        private List<Animal> DeserializeJson(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string str = streamReader.ReadToEnd();
            return JsonSerializer.Deserialize<List<Animal>>(str);
        }

        private void serializeXML(List<Animal> animals, string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Animal>));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, animals);
            }
        }

        private List<Animal> DeserializeXml(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Animal>));
            List<Animal> animals;
            
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                animals = (List<Animal>)xmlSerializer.Deserialize(fs);
            }

            return animals;
        }
        public void WriteTxt(List<Animal> animals, string path)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(Serialize(animals));
            streamWriter.Close();
        }

        public List<Animal> ReadTxt(string path)
        {
            return Deserialize(path);
        }

        public void WriteJson(List<Animal> animals, string path)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(SerializeJson(animals));
            streamWriter.Close();
        }
        public List<Animal> ReadJson(string path)
        {
            return DeserializeJson(path);
        }
        public void WriteXML(List<Animal> animals, string path)
        {
            serializeXML(animals, path);
        }
        public List<Animal> ReadXML(string path)
        {
            return DeserializeXml(path);
        }
    }

    class Menu // Меню
    {
        Serializer serializer = new Serializer();
        List<Animal> animals = new List<Animal>();
        
        private string getType(string path)
        {
            string out_str = "";

            int i = path.Length-1;
            while (path[i]!='.')
            {
                out_str+=path[i];
                i--;
                if (i==-1)
                {
                    Console.WriteLine("Введён неверный путь");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
            out_str += '.';
            char[] charArray = out_str.ToCharArray();
            Array.Reverse(charArray);

            out_str = new string(charArray);
            return out_str;
        }
        private void InitAnimals()
        {
            List<Animal> animals = new List<Animal>();
            string[] names = new string[3]
            {
                "Бобик",
                "Рекс",
                "Полкан"
            };
            for (int i = 0; i < 3; i++)
            {
                animals.Add(new Animal(names[i], (i + 5) * 10, (i + 6) * 10));
            }
        }
        private void PrintAnimals(List<Animal> animals)
        {
            for (int i = 0; i < animals.Count; i++)
            {
                Console.WriteLine(animals[i].Name);
                Console.WriteLine(animals[i].height);
                Console.WriteLine(animals[i].age);

            }
        }
        public void Work()
        {
            
                Console.WriteLine("Введите путь к файлу: ");
                string path = Console.ReadLine();
                string type = getType(path);
                switch (type)
                {
                    case ".txt":
                        animals = serializer.ReadTxt(path);
                        break;
                    case ".json":
                        animals = serializer.ReadJson(path);
                        break;
                    case ".xml":
                        animals = serializer.ReadXML(path);
                        break;
                }

            PrintAnimals(animals);
            Console.WriteLine("Прочитан файл в формате " + getType(path));
            while (true)
            {
                Console.WriteLine(" Нажмите F1 для сохранения, Escape - выход из программы");
            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
            Console.Clear();
            
                if (consoleKeyInfo.Key == ConsoleKey.F1)
                {
                    Console.WriteLine("Введите путь к файлу: ");

                    path = Console.ReadLine();
                    type = getType(path);
                    switch (type)
                    {
                        case ".txt":
                            serializer.WriteTxt(animals, path);
                            break;
                        case ".json":
                            serializer.WriteJson(animals, path);
                            break;
                        case ".xml":
                            serializer.WriteXML(animals, path);
                            break;
                    }
                    Console.WriteLine("Файл сохранён в формате " + getType(path));
                    Environment.Exit(0);
                }
                else
                    if (consoleKeyInfo.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
            }
        }


    }
    internal class Program
    {
        

        static void Main(string[] args)
        {

            Menu menu = new Menu();
            menu.Work();
            Console.ReadKey();
        }
    }
}
