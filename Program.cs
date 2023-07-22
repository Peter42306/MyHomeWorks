using MyHomeWorks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

// ДЗ от 20 мая
// 

// Створіть програму для роботи з інформацією про музичний альбом, яка зберігатиме таку інформацію: 
// 1.Назва альбому.
// 2.Назва виконавця.
// 3.Рік випуску.
// 4.Тривалість.
// 5.Студія звукозапису.
//
// Програма має бути з такою функціональністю: 
// 1.Введення інформації про альбом. 
// 2. Виведення інформації про альбом. 
// 3. Серіалізація альбому. 
// 4. Збереження серіалізованого альбому у файл. 
// 5. Завантаження серіалізованого альбому з файлу. 
// 6. Збережіть дані про альбом у xml файлу
// 7. Додайте логування, використовуйте NLog

// + удаление сериализованного альбома из файла
// + загрузка данных из xml файла
//=====================================================================

namespace MyHomeWorks
{
    
    
    //=================================================================

    internal class Program
    {
        // Статический список, который будет содержать данные об альбомах
        static List<Album> albums = new List<Album>();

        // Имя файла, в котором будут храниться сериализованные альбомы .bin
        const string filePath = "MusicAlbums.bin";

        // Метод для загрузки данных об альбомах из файла
        static void LoadAlbumsFromFile()
        {
            // Проверяем, существует ли файл с именем, указанным в filePath
            if (File.Exists(filePath))
            {
                // Если файл существует, открываем его для чтения (FileMode.Open)
                // Создаем объект BinaryFormatter для десериализации данных
                // Десериализуем данные об альбомах из файла и сохраняем их в статический список albums
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    albums = (List<Album>)binaryFormatter.Deserialize(fileStream);
                }
            }
        }

        // Выводит информацию об альбомах, которая хранится в файле с именем, указанным в переменной filePath
        static void ShowAllAlbums()
        {
            Console.WriteLine($"All albums in the '{filePath}' file");
            Console.WriteLine("{0,-5}{1,-40}{2,-40}{3,-8}{4,-15}{5,-20}", "No.", "Title", "Artist", "Year", "Duration", "Studio");
            Console.WriteLine("=======================================================================================================================");

            // Проверка существования файла с указанным именем
            if (File.Exists(filePath))
            {
                // Создание объекта FileStream для чтения данных из файла
                using (FileStream fileStream=new FileStream(filePath,FileMode.Open))
                {
                    // Создание объекта BinaryFormatter для десериализации данных из файла
                    BinaryFormatter binaryFormatter=new BinaryFormatter();
                    
                    // Десериализация данных из файла в список albumsFromFile
                    List<Album>albumsFromFile=(List<Album>)binaryFormatter.Deserialize(fileStream);
                    
                    // Нумерация альбомов на выводе
                    int count = 0;

                    // Перебираем все альюомы в списке и вывод на консоль
                    foreach (var album in albumsFromFile)
                    {
                        count++;
                        Console.WriteLine($"{count,-5}{album.Title,-40}{album.Artist,-40}{album.Year,-8}{album.Duration,-15}{album.RecordingStudio,-20}");
                    }
                }
            }            
            else
            {
                Console.WriteLine($"File '{filePath}' was not found");
            }
            Console.WriteLine();
        }

        //static void CreateFileAlbumsToFile()
        //{
        //    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        BinaryFormatter binaryFormatter = new BinaryFormatter();
        //        binaryFormatter.Serialize(fileStream, albums);
        //    }
        //}

        // Метод сохраняет данные об альбомах из списка albums в файл с именем, указанным в переменной filePath
        static void SaveAlbumsToFile()
        {
            // Создание объекта FileStream для записи данных в файл
            using (FileStream fileStream=new FileStream(filePath,FileMode.OpenOrCreate))
            {
                // Создание объекта BinaryFormatter для сериализации данных в файл
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                // Вызов метода Serialize() объекта BinaryFormatter, который сериализует список albums и записывает его в файл, указанный в fileStream
                binaryFormatter.Serialize(fileStream, albums);
            }
        }

        // Удаление файла, который содержит сериализованные данные об альбомах
        static void DeleteFile()
        {
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                Console.WriteLine($"File {filePath} was not found");
            }
        }

        // Метод предназначен для удаления альбомов из списка albums на основе определенного критерия ("Title", "Artist", "Year", "Studio")
        static void DeleteAlbumByCriteria(string criteria, string value)
        {
            // С помощью метода FindAll() из списка albums создается новый список albumsToDelete, который содержит альбомы, соответствующие указанным критерию и значению
            // Проверку на соответствие выбранному критерию производим с помощью лямбда выражения
            List<Album> albumsToDelete = albums.FindAll(album =>
            {
                switch (criteria)
                {
                    case "Title":
                        return album.Title.Equals(value);
                    case "Artist":
                        return album.Artist.Equals(value);
                    case "Year":
                        int year=Int32.Parse(value);
                        return album.Year.Equals(year);
                    case "Stidio":
                        return album.RecordingStudio.Equals(value);
                    default: return false;
                }
            });
            // Если в списке albumsToDelete есть альбомы, соответствующие критерию, они удаляются из списка albums
            if (albumsToDelete.Count > 0 )
            {
                foreach (var album in albumsToDelete)
                {
                    albums.Remove(album);
                }
                // Сохраняем в файл обновлённый список албюомов
                SaveAlbumsToFile();
                Console.WriteLine($"Albums with the createria {criteria} / {value} were deleted");
            }
            else
            {
                Console.WriteLine("The criteria to delete was not found in the list of albums");
            }
        }

        //======================================================================================

        // Имя файла, в котором будут храниться сериализованные альбомы .xml
        const string xmlFilePath = "xmlFile.xml";

        // Метод для загрузки данных об альбомах из файла XML
        static void LoadAlbumsFromXmlFile()
        {
            if (File.Exists(xmlFilePath))
            {
                // Создаем объект XmlSerializer, указав тип данных, который мы хотим десериализовать (в данном случае - List<Album>)
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Album>));

                // Используем конструкцию using для открытия потока чтения из файла с помощью FileStream.
                // FileMode.OpenOrCreate указывает, что файл должен быть открыт для чтения, если он существует, и создан, если его нет.
                using (FileStream fileStream=new FileStream(xmlFilePath,FileMode.OpenOrCreate) )
                {
                    // Выполняем десериализацию данных из файла (XML) и преобразуем их обратно в список альбомов.
                    // Результат десериализации приводим к типу List<Album> и сохраняем в переменную albums.
                    albums = (List<Album>)xmlSerializer.Deserialize(fileStream);
                }
            }
            else
            {
                // Если файл с именем xmlFilePath не найден, выводим сообщение об ошибке
                Console.WriteLine($"File '{xmlFilePath}' was not found");
            }
        }

        // Метод сохраняет данные об альбомах из списка albums в файл с именем, указанным в переменной xmlFilePath
        static void SaveAlbumsToXmlFile()
        {
            // Создаем объект XmlSerializer, указав тип данных, который хотим сериализовать (в данном случае - List<Album>)
            XmlSerializer xmlSerializer =new XmlSerializer(typeof(List<Album>));

            // Используем конструкцию using для открытия потока записи в файл с помощью FileStream.
            // FileMode.OpenOrCreate указывает, что файл должен быть открыт для записи, если он существует, и создан, если его нет.
            using (FileStream fileStream=new FileStream(xmlFilePath,FileMode.OpenOrCreate) )
            {
                // Выполняем сериализацию списка альбомов (albums) в формат XML и записываем в файл (XML).
                // Сериализация происходит с помощью метода Serialize объекта XmlSerializer.
                xmlSerializer.Serialize(fileStream, albums);
            }
        }
        

        static void ShowAlbumsFromXmlFile()
        {
            // Проверяем, существует ли файл с именем, указанным в переменной xmlFilePath
            if (File.Exists(xmlFilePath))
            {
                // Создаем объект XmlSerializer, указав тип данных, который хотим десериализовать (в данном случае - List<Album>)
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Album>));

                // Используем конструкцию using для открытия потока чтения из файла с помощью FileStream.
                // FileMode.Open указывает, что файл должен быть открыт только для чтения.
                using (FileStream fileStream = new FileStream(xmlFilePath, FileMode.Open))
                {
                    // Выполняем десериализацию данных из файла (XML) и преобразование их обратно в список альбомов (List<Album>).
                    // Результат десериализации приводим к типу List<Album> и сохраняем в переменную albums.
                    albums = (List<Album>)xmlSerializer.Deserialize(fileStream);
                }

                // Нумерация альбомов на выводе
                int count = 0;

                Console.WriteLine($"All albums in the '{xmlFilePath}' file");
                Console.WriteLine("{0,-5}{1,-40}{2,-40}{3,-8}{4,-15}{5,-20}", "No.", "Title", "Artist", "Year", "Duration", "Studio");
                Console.WriteLine("=======================================================================================================================");

                // Перебираем все альбомы в списке и выводим на консоль
                foreach (var album in albums)
                {
                    count++;
                    Console.WriteLine($"{count,-5}{album.Title,-40}{album.Artist,-40}{album.Year,-8}{album.Duration,-15}{album.RecordingStudio,-20}");
                }
            }
            else
            {
                // Если файл не найден, выводится сообщение об ошибке с указанием имени файла, который не удалось найти.
                Console.WriteLine($"XML file '{xmlFilePath}' was not found.");
            }
            Console.WriteLine();
        }

        //static void createXMLFile()
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //}

        //=============================================================

        static void Main(string[] args)
        {
            // Комментарий: Этот вызов метода удаляет файл "MusicAlbums.bin", если он существует
            //DeleteFile();

            // Комментарий: Этот метод создает файл "MusicAlbums.bin" и сохраняет в него список альбомов. В текущем коде этот метод не используется, потому что список альбомов уже заполняется вручную ниже
            //CreateFileAlbumsToFile();

            // Этот метод загружает данные об альбомах из файла "MusicAlbums.bin" и сохраняет их в список albums
            LoadAlbumsFromFile();
            LoadAlbumsFromXmlFile();

            // Ниже добавляются новые альбомы в список albums с помощью метода Add класса List<Album>.
            // После каждого добавления новых альбомов, список сохраняется в файл с помощью метода SaveAlbumsToFile().

            albums.Add(new Album("A Love Supreme", "John Coltrane", 1965, 32, "Impulse! Records"));
            albums.Add(new Album("Time Out", "Dave Brubeck Quartet", 1959, 39, "Columbia Records"));
            albums.Add(new Album("Giant Steps", "John Coltrane", 1960, 47, "Atlantic Records"));
            albums.Add(new Album("Blue Train", "John Coltrane", 1957, 43, "Blue Note Records"));
            albums.Add(new Album("Ella and Louis", "Ella Fitzgerald and Louis Armstrong", 1956, 59, "Verve Records"));
            albums.Add(new Album("My Favorite Things", "John Coltrane", 1961, 41, "Atlantic Records"));
            albums.Add(new Album("The Black Saint and the Sinner Lady", "Charles Mingus", 1963, 39, "Impulse! Records"));
            albums.Add(new Album("Mingus Ah Um", "Charles Mingus", 1959, 49, "Columbia Records"));
            albums.Add(new Album("Somethin' Else", "Cannonball Adderley", 1958, 43, "Blue Note Records"));
            
            

            // В этом месте список альбомов, содержащий добавленные альбомы, сериализуется и сохраняется в файл "MusicAlbums.bin"
            SaveAlbumsToFile();
            SaveAlbumsToXmlFile();

            albums.Add(new Album("Take Five", "The Dave Brubeck Quartet", 1959, 38, "Columbia Records"));
            albums.Add(new Album("Maiden Voyage", "Herbie Hancock", 1965, 42, "Blue Note Records"));
            albums.Add(new Album("Saxophone Colossus", "Sonny Rollins", 1956, 39, "Prestige Records"));
            albums.Add(new Album("Bird", "Charlie Parker", 1952, 34, "Savoy Records"));
            albums.Add(new Album("The Shape of Jazz to Come", "Ornette Coleman", 1959, 36, "Atlantic Records"));
            albums.Add(new Album("Ah Um", "Charles Mingus", 1959, 44, "Columbia Records"));

            SaveAlbumsToFile();
            SaveAlbumsToXmlFile();

            albums.Add(new Album("Bitches Brew", "Miles Davis", 1970, 94, "Columbia Records"));
            albums.Add(new Album("Brilliant Corners", "Thelonious Monk", 1957, 47, "Riverside Records"));
            albums.Add(new Album("Head Hunters", "Herbie Hancock", 1973, 42, "Columbia Records"));
            albums.Add(new Album("Ellington at Newport", "Duke Ellington", 1956, 67, "Columbia Records"));
            albums.Add(new Album("Getz/Gilberto", "Stan Getz and João Gilberto", 1964, 35, "Verve Records"));

            SaveAlbumsToFile();
            SaveAlbumsToXmlFile();           

            // Выводится список всех альбомов в файле "MusicAlbums.bin" с помощью метода ShowAllAlbums().
            ShowAllAlbums();
            ShowAlbumsFromXmlFile();

            // Вызываются методы DeleteAlbumByCriteria, которые удаляют альбомы из списка albums по заданным критериям(по названию и году).
            // После каждого удаления, список альбомов сохраняется в файл.
            DeleteAlbumByCriteria("Title", "Bird");
            DeleteAlbumByCriteria("Year", "1959");

            ShowAllAlbums();
            ShowAlbumsFromXmlFile();
        }
    }
}

//File 'xmlFile.xml' was not found
//All albums in the 'MusicAlbums.bin' file
//No.  Title                                   Artist                                  Year    Duration       Studio
//=======================================================================================================================
//1    A Love Supreme                          John Coltrane                           1965    32             Impulse! Records
//2    Time Out                                Dave Brubeck Quartet                    1959    39             Columbia Records
//3    Giant Steps                             John Coltrane                           1960    47             Atlantic Records
//4    Blue Train                              John Coltrane                           1957    43             Blue Note Records
//5    Ella and Louis                          Ella Fitzgerald and Louis Armstrong     1956    59             Verve Records
//6    My Favorite Things                      John Coltrane                           1961    41             Atlantic Records
//7    The Black Saint and the Sinner Lady     Charles Mingus                          1963    39             Impulse! Records
//8    Mingus Ah Um                            Charles Mingus                          1959    49             Columbia Records
//9    Somethin' Else                          Cannonball Adderley                     1958    43             Blue Note Records
//10   Take Five                               The Dave Brubeck Quartet                1959    38             Columbia Records
//11   Maiden Voyage                           Herbie Hancock                          1965    42             Blue Note Records
//12   Saxophone Colossus                      Sonny Rollins                           1956    39             Prestige Records
//13   Bird                                    Charlie Parker                          1952    34             Savoy Records
//14   The Shape of Jazz to Come               Ornette Coleman                         1959    36             Atlantic Records
//15   Ah Um                                   Charles Mingus                          1959    44             Columbia Records
//16   Bitches Brew                            Miles Davis                             1970    94             Columbia Records
//17   Brilliant Corners                       Thelonious Monk                         1957    47             Riverside Records
//18   Head Hunters                            Herbie Hancock                          1973    42             Columbia Records
//19   Ellington at Newport                    Duke Ellington                          1956    67             Columbia Records
//20   Getz/Gilberto                           Stan Getz and Joao Gilberto             1964    35             Verve Records

//All albums in the 'xmlFile.xml' file
//No.  Title                                   Artist                                  Year    Duration       Studio
//=======================================================================================================================
//1    A Love Supreme                          John Coltrane                           1965    32             Impulse! Records
//2    Time Out                                Dave Brubeck Quartet                    1959    39             Columbia Records
//3    Giant Steps                             John Coltrane                           1960    47             Atlantic Records
//4    Blue Train                              John Coltrane                           1957    43             Blue Note Records
//5    Ella and Louis                          Ella Fitzgerald and Louis Armstrong     1956    59             Verve Records
//6    My Favorite Things                      John Coltrane                           1961    41             Atlantic Records
//7    The Black Saint and the Sinner Lady     Charles Mingus                          1963    39             Impulse! Records
//8    Mingus Ah Um                            Charles Mingus                          1959    49             Columbia Records
//9    Somethin' Else                          Cannonball Adderley                     1958    43             Blue Note Records
//10   Take Five                               The Dave Brubeck Quartet                1959    38             Columbia Records
//11   Maiden Voyage                           Herbie Hancock                          1965    42             Blue Note Records
//12   Saxophone Colossus                      Sonny Rollins                           1956    39             Prestige Records
//13   Bird                                    Charlie Parker                          1952    34             Savoy Records
//14   The Shape of Jazz to Come               Ornette Coleman                         1959    36             Atlantic Records
//15   Ah Um                                   Charles Mingus                          1959    44             Columbia Records
//16   Bitches Brew                            Miles Davis                             1970    94             Columbia Records
//17   Brilliant Corners                       Thelonious Monk                         1957    47             Riverside Records
//18   Head Hunters                            Herbie Hancock                          1973    42             Columbia Records
//19   Ellington at Newport                    Duke Ellington                          1956    67             Columbia Records
//20   Getz/Gilberto                           Stan Getz and Joao Gilberto             1964    35             Verve Records

//Albums with the createria Title / Bird were deleted
//Albums with the createria Year / 1959 were deleted
//All albums in the 'MusicAlbums.bin' file
//No.  Title                                   Artist                                  Year    Duration       Studio
//=======================================================================================================================
//1    A Love Supreme                          John Coltrane                           1965    32             Impulse! Records
//2    Giant Steps                             John Coltrane                           1960    47             Atlantic Records
//3    Blue Train                              John Coltrane                           1957    43             Blue Note Records
//4    Ella and Louis                          Ella Fitzgerald and Louis Armstrong     1956    59             Verve Records
//5    My Favorite Things                      John Coltrane                           1961    41             Atlantic Records
//6    The Black Saint and the Sinner Lady     Charles Mingus                          1963    39             Impulse! Records
//7    Somethin' Else                          Cannonball Adderley                     1958    43             Blue Note Records
//8    Maiden Voyage                           Herbie Hancock                          1965    42             Blue Note Records
//9    Saxophone Colossus                      Sonny Rollins                           1956    39             Prestige Records
//10   Bitches Brew                            Miles Davis                             1970    94             Columbia Records
//11   Brilliant Corners                       Thelonious Monk                         1957    47             Riverside Records
//12   Head Hunters                            Herbie Hancock                          1973    42             Columbia Records
//13   Ellington at Newport                    Duke Ellington                          1956    67             Columbia Records
//14   Getz/Gilberto                           Stan Getz and Joao Gilberto             1964    35             Verve Records

//All albums in the 'xmlFile.xml' file
//No.  Title                                   Artist                                  Year    Duration       Studio
//=======================================================================================================================
//1    A Love Supreme                          John Coltrane                           1965    32             Impulse! Records
//2    Time Out                                Dave Brubeck Quartet                    1959    39             Columbia Records
//3    Giant Steps                             John Coltrane                           1960    47             Atlantic Records
//4    Blue Train                              John Coltrane                           1957    43             Blue Note Records
//5    Ella and Louis                          Ella Fitzgerald and Louis Armstrong     1956    59             Verve Records
//6    My Favorite Things                      John Coltrane                           1961    41             Atlantic Records
//7    The Black Saint and the Sinner Lady     Charles Mingus                          1963    39             Impulse! Records
//8    Mingus Ah Um                            Charles Mingus                          1959    49             Columbia Records
//9    Somethin' Else                          Cannonball Adderley                     1958    43             Blue Note Records
//10   Take Five                               The Dave Brubeck Quartet                1959    38             Columbia Records
//11   Maiden Voyage                           Herbie Hancock                          1965    42             Blue Note Records
//12   Saxophone Colossus                      Sonny Rollins                           1956    39             Prestige Records
//13   Bird                                    Charlie Parker                          1952    34             Savoy Records
//14   The Shape of Jazz to Come               Ornette Coleman                         1959    36             Atlantic Records
//15   Ah Um                                   Charles Mingus                          1959    44             Columbia Records
//16   Bitches Brew                            Miles Davis                             1970    94             Columbia Records
//17   Brilliant Corners                       Thelonious Monk                         1957    47             Riverside Records
//18   Head Hunters                            Herbie Hancock                          1973    42             Columbia Records
//19   Ellington at Newport                    Duke Ellington                          1956    67             Columbia Records
//20   Getz/Gilberto                           Stan Getz and Joao Gilberto             1964    35             Verve Records

//Press any key to continue . . .
