using System;

namespace ReadingClab.ReadingClabs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            E_ReadingClab e_ReadingClab = new E_ReadingClab(88);
            e_ReadingClab.Work();
        }
    }
    class E_ReadingClab
    {
        private int _money = 0;
        private List<ElectronicBook> _ebooks = new List<ElectronicBook>();
        private Queue<Client> _clients = new Queue<Client>();

        public E_ReadingClab(int ebooksCount)
        {
            Random random = new Random();

            for (int i = 0; i < ebooksCount; i++)
            {
                _ebooks.Add(new ElectronicBook(random.Next(1, 5)));
            }

            CreateNewClients(28, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(120, 280), random));
            }
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс читательного клуба {_money} руб. Ждём с огромным нетерпением нового клиента");
                Console.WriteLine($"У вас новый клиент, и он хочет купить {newClient.DesiredMinutes} минуту");
                ShowAllElectronicBooksState();

                Console.Write("\nВы предлагаете ему электронную книгу под номером: ");
                string userInput = Console.ReadLine();
                //int ebookNumber = Convert.ToInt32(Console.ReadLine()) - 1;
                if (int.TryParse(userInput, out int ebookNumber))
                {
                    ebookNumber -= 1;

                    if (ebookNumber >= 0 && ebookNumber < _ebooks.Count)
                    {
                        if (_ebooks(ebookNumber).IsTaken)
                        {
                            Console.WriteLine("Вы пытаетесь посадить за электронную книгу, которая уже занята. Клиен разозлился и ушёл.");
                        }
                        else
                        {
                            if (newClient.CheckSolvency(_ebooks[ebookNumber]))
                            {
                                Console.WriteLine("Клиент пересчитал деньги, оплатил и взял книгу " + ebookNumber + 1);
                                _money += newClient.Pay();
                                _ebooks[ebookNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента не хватило денег и он ушёл.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы  сами не знаете какую электронную книгу предложить клиенту. Он разозлился и ушёл.");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Неверный ввод! Повторите снова.");
                }

                Console.WriteLine("Чтобы перейти к следующему клиенту, нажмите любую клавишу.");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        private void ShowAllElectronicBooksState()
        {
            Console.WriteLine("\nСписок всех электронных книг:");
            for (int i = 0; i < _ebooks.Count; i++)
            {
                Console.WriteLine(i + 1 + " - ");
                _ebooks[i].ShowState();
            }
        }

        private void SpendOneMinute()
        {
            foreach (var electronicBook in _ebooks)
            {
                electronicBook.SpendOneMinute();
            }
        }
    }

    class ElectronicBook
    {
        private Client _client;
        private int _minutesRemaining;
        public bool IsTaken
        {
            get
            {
                return _minutesRemaining > 0;
            }
        }

        public int PricePerMinute { get; private set; }

        public ElectronicBook(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }

        public void BecomeEmpty()
        {
            _client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }

        public void ShowState()
        {
            if (IsTaken)
                Console.WriteLine($"Книга занята, осталосьминут: {_minutesRemaining}");
            else
                Console.WriteLine($"Книга свободна, цена за минуту: {PricePerMinute}");
        }

    }

    class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(15, 35);
        }

        public bool CheckSolvency(ElectronicBook electronicBook)
        {
            _moneyToPay = DesiredMinutes * electronicBook.PricePerMinute;
            if (_money >= _moneyToPay)
            {
                return true;
            }
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}
