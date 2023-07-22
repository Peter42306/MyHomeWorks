using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHomeWorks
{
    // объекты этого класса могут быть сериализованы и десериализованы
    [Serializable]

    // Класс представляющий данные о музыкальном альбоме
    public class Album
    {
        // Свойство, представляющее название альбома
        public string Title { get; set; }

        // Свойство, представляющее имя исполнителя
        public string Artist { get; set; }

        // Свойство, представляющее год выпуска альбома
        public int Year { get; set; }

        // Свойство, представляющее продолжительность альбома в формате TimeSpan
        public int Duration { get; set; }

        // Свойство, представляющее студию записи альбома
        public string RecordingStudio { get; set; }

        // Конструктор с параметрами
        public Album(string title, string artist, int year, int duration, string recordingStudio)
        {
            this.Title = title;
            this.Artist = artist;
            this.Year = year;
            this.Duration = duration;
            this.RecordingStudio = recordingStudio;
        }

        // Пустой конструктор для 
        public Album()
        {

        }
    }
}
