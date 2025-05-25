Race race = new Race();

race.AddCar(new SportCar("Ferrari"));
race.AddCar(new PassengerCar("Toyota"));
race.AddCar(new Truck("Volvo Truck"));
race.AddCar(new Bus("School Bus"));

race.StartRace();

Console.WriteLine("Гонка завершена.");
Console.ReadLine();

public abstract class Car
{
    public string Name { get; set; }
    public double Position { get; protected set; }
    public double MinSpeed { get; protected set; }
    public double MaxSpeed { get; protected set; }

    public event Action<Car>? OnFinish;

    public Car(string name, double minSpeed, double maxSpeed)
    {
        Name = name;
        MinSpeed = minSpeed;
        MaxSpeed = maxSpeed;
    }

    public virtual void Start() => Console.WriteLine($"{Name} вышел на старт.");

    public virtual void Move()
    {
        double speed = new Random().NextDouble() * (MaxSpeed - MinSpeed) + MinSpeed;
        Position += speed;
        Console.WriteLine($"{Name} едет со скоростью {speed:F2}, текущая позиция: {Position:F2}");

        if (Position >= 100)
        {
            OnFinish?.Invoke(this);
        }
    }
}

public class SportCar : Car
{
    public SportCar(string name) : base(name, 5, 10) { }
}

public class PassengerCar : Car
{
    public PassengerCar(string name) : base(name, 3, 7) { }
}

public class Truck : Car
{
    public Truck(string name) : base(name, 2, 5) { }
}

public class Bus : Car
{
    public Bus(string name) : base(name, 2, 6) { }
}

public delegate void CarAction();

public class Race
{
    private List<Car> cars = new();
    private bool raceFinished = false;

    public void AddCar(Car car)
    {
        car.OnFinish += FinishRace;
        cars.Add(car);
    }

    public void StartRace()
    {
        CarAction startAll = null!;
        foreach (var car in cars)
            startAll += car.Start;

        startAll();

        while (!raceFinished)
        {
            foreach (var car in cars)
            {
                car.Move();
                if (raceFinished) break;
            }
        }
    }

    private void FinishRace(Car winner)
    {
        raceFinished = true;
        Console.WriteLine($"\nПобедил {winner.Name}!\n");
    }
}
