using System;

namespace Lab10Charp
{
    
    public delegate void MissingEventHandler(object sender, MissingEventArgs e);

    public class City
    {
        string name;
        int buildings;
        int days;
        Police police;
        Volunteers volunteers;
        public event MissingEventHandler Missing;

        string[] results;
        private Random rnd = new Random();
        double missingChance = 0.01;

        public City(string name, int buildings, int days)
        {
            this.name = name;
            this.buildings = buildings;
            this.days = days;

            police = new Police(this);
            volunteers = new Volunteers(this);

            police.On();
            volunteers.On();
        }

        protected virtual void OnMissing(MissingEventArgs e)
        {
            Console.WriteLine("ALERT: A person has gone missing in city " + name +
                              "! Building " + e.Building +
                              ", Day " + e.Day);

            if (Missing != null)
            {
                Delegate[] handlers = Missing.GetInvocationList();
                results = new string[handlers.Length];
                int i = 0;

                foreach (MissingEventHandler handler in handlers)
                {
                    handler(this, e);
                    results[i] = e.Result;
                    i++;
                }
            }
        }

        public void Simulate()
        {
            bool incidentOccurred = false;

            for (int day = 1; day <= days; day++)
            {
                for (int building = 1; building <= buildings; building++)
                {
                    if (rnd.NextDouble() < missingChance)
                    {
                        MissingEventArgs e = new MissingEventArgs(building, day);
                        OnMissing(e);
                        incidentOccurred = true;

                        for (int i = 0; i < results.Length; i++)
                        {
                            Console.WriteLine(results[i]);
                        }
                    }
                }
            }

            if (!incidentOccurred)
            {
                Console.WriteLine("INFO: Everything was calm in city " + name + ".");
            }
        }
    }

    public abstract class Service
    {
        protected City city;
        protected Random rnd = new Random();

        public Service(City city)
        {
            this.city = city;
        }

        public void On()
        {
            city.Missing += this.HandleMissing;
        }

        public void Off()
        {
            city.Missing -= this.HandleMissing;
        }

        public abstract void HandleMissing(object sender, MissingEventArgs e);
    }

    public class Police : Service
    {
        public Police(City city) : base(city) { }

        public override void HandleMissing(object sender, MissingEventArgs e)
        {
            int chance = rnd.Next(0, 10);

            if (chance > 5)
            {
                e.Result = "Police report: Person found successfully.";
            }
            else
            {
                e.Result = "Police report: Search is ongoing.";
            }
        }
    }

    public class Volunteers : Service
    {
        public Volunteers(City city) : base(city) { }

        public override void HandleMissing(object sender, MissingEventArgs e)
        {
            int chance = rnd.Next(0, 10);

            if (chance > 6)
            {
                e.Result = "Volunteers report: Person located and safe.";
            }
            else
            {
                e.Result = "Volunteers report: Continuing the search.";
            }
        }
    }

    public class MissingEventArgs : EventArgs
    {
        private int building;
        private int day;
        private string result;

        public int Building
        {
            get { return building; }
        }

        public int Day
        {
            get { return day; }
        }

        public string Result
        {
            get { return result; }
            set { result = value; }
        }

        public MissingEventArgs(int building, int day)
        {
            this.building = building;
            this.day = day;
        }
    }

    class Program
    {
        static void Main()
        {
            City city = new City("Lviv", 10, 30);
            city.Simulate();
        }
    }
}