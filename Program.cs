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

Game game = new();
game.Setup();
game.Play();

Console.ReadLine();

public class Karta
{
    public string Suit { get; set; }
    public string Value { get; set; }
    public int Power { get; set; }

    public Karta(string suit, string value, int power)
    {
        Suit = suit;
        Value = value;
        Power = power;
    }

    public override string ToString() => $"{Value} {Suit}";
}

public class Player
{
    public string Name { get; set; }
    public Queue<Karta> Cards { get; private set; } = new();

    public Player(string name) => Name = name;

    public bool HasCards => Cards.Count > 0;

    public Karta PlayCard() => Cards.Dequeue();

    public void AddCards(IEnumerable<Karta> wonCards)
    {
        foreach (var card in wonCards)
            Cards.Enqueue(card);
    }

    public void ShowCards() =>
        Console.WriteLine($"{Name} имеет {Cards.Count} карт(ы).");
}

public class Game
{
    private List<Karta> deck = new();
    private List<Player> players = new();

    private string[] suits = { "Черви", "Бубны", "Трефы", "Пики" };
    private (string value, int power)[] values =
    {
            ("6", 6), ("7", 7), ("8", 8), ("9", 9), ("10", 10),
            ("Валет", 11), ("Дама", 12), ("Король", 13), ("Туз", 14)
        };

    public void Setup()
    {
        foreach (var suit in suits)
            foreach (var (value, power) in values)
                deck.Add(new Karta(suit, value, power));

        deck = deck.OrderBy(_ => Guid.NewGuid()).ToList();

        players.Add(new Player("Игрок 1"));
        players.Add(new Player("Игрок 2"));

        for (int i = 0; i < deck.Count; i++)
            players[i % 2].Cards.Enqueue(deck[i]);

        Console.WriteLine("Карты розданы.\n");
    }

    public void Play()
    {
        int round = 1;
        while (players.All(p => p.HasCards))
        {
            Console.WriteLine($"Раунд {round++}:");

            var card1 = players[0].PlayCard();
            var card2 = players[1].PlayCard();

            Console.WriteLine($"{players[0].Name} кладёт: {card1}");
            Console.WriteLine($"{players[1].Name} кладёт: {card2}");

            var pile = new List<Karta> { card1, card2 };
            int winner = DetermineWinner(card1, card2);

            Console.WriteLine($"➤ Побеждает: {players[winner].Name}\n");

            players[winner].AddCards(pile);

            foreach (var p in players) p.ShowCards();

            Console.WriteLine("-----------------------------");
            System.Threading.Thread.Sleep(500);
        }

        var finalWinner = players.First(p => p.HasCards);
        Console.WriteLine($"\nПобедитель: {finalWinner.Name}!");
    }

    private int DetermineWinner(Karta c1, Karta c2)
    {
        if (c1.Value == "6" && c2.Value == "Туз") return 1;
        if (c2.Value == "6" && c1.Value == "Туз") return 0;

        if (c1.Power == c2.Power) return 0;
        return c1.Power > c2.Power ? 0 : 1;
    }
}