using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using MySql.Data.MySqlClient;

namespace ConqerAndDevelopIIServer
{

    
    class Tile
    {
        public Dictionary<string, int> Distances = new Dictionary<string, int>();
        public int PosX;
        public int PosY;
        public string Name;
        public int OwnerID = 0;
        public int SoldierHP = 0;
        public int SoldierMaxHP = 0;
        public int SoldierMoves = 0;
        public int SoldierMaxMoves = 0;
        public int SoldierOwnerID = 0;
        public int OccupiedBy = 0;
        public int Capital = 0;
        public int Development = 1;
        public int Fort = 0;
        public List<Population> Population = new List<Population>();

        public int GetPopFoodUsage()
        {
            int foodusage = 0;
            foreach(Population p in Population)
            {
                if (p.Level > 0)
                    foodusage += 2;
                else
                    foodusage++;
            }
            return foodusage;
        }


        public int GrowthProgress = 0;

        public int BaseFoodOutput = 4;

        public float FoodFromOtherEntities = 0;

        public float ProductionOutput
        {
            get
            {
                if(Population.Count != 0)
                {

                }


                return 0;
            }
        }

        public float FoodOutput
        {
            get
            {
                if (Population.Count != 0)
                {
                    float FlatIncrese = 0;
                    float Modifiers = 1;
                    if (Population.Count > 10)
                        Modifiers -= 0.2f;
                    if (Population.Count > 15)
                        Modifiers -= 0.2f;
                    if (Population.Count > 20)
                        Modifiers -= 0.1f;

                    foreach(Population p in Population)
                    {
                        if(p.Level == 1)
                        {
                            FlatIncrese += 1;
                            if(p.Happiness >= 0)
                            {
                                FlatIncrese += p.Happiness * 0.05f;
                            }
                            else
                            {
                                FlatIncrese += p.Happiness * 0.1f;
                            }
                        }
                        else if(p.Level == 2)
                        {
                            FlatIncrese += 0.25f;
                            if (p.Happiness >= 0)
                            {
                                FlatIncrese += p.Happiness * 0.01f;
                            }
                            else
                            {
                                FlatIncrese += p.Happiness * 0.02f;
                            }
                        }
                    }

                    foreach (BuildingSlot bs in BuildingSlots)
                    {
                        switch (bs.Type)
                        {
                            default:
                                break;
                            case "2": //farm
                                //+++Food(Flat: 0 / 2 / 4 / 8 / 12)
                                switch (bs.Level)
                                {
                                    case 1:
                                        FlatIncrese += 2;
                                        foreach (Population p in Population)
                                        {
                                            if (p.Level == 1)
                                            {
                                                FlatIncrese += 1.5f;
                                                if (p.Happiness >= 0)
                                                    FlatIncrese += 0.07f * p.Happiness;
                                                else
                                                    FlatIncrese += 0.15f * p.Happiness;
                                            }

                                        }
                                        break;
                                    case 2:
                                        FlatIncrese += 2;
                                        foreach (Population p in Population)
                                        {
                                            if (p.Level == 1)
                                            {
                                                FlatIncrese += 2.5f;
                                                if (p.Happiness >= 0)
                                                    FlatIncrese += 0.12f * p.Happiness;
                                                else
                                                    FlatIncrese += 0.25f * p.Happiness;
                                            }
                                        }
                                        break;
                                    case 3:
                                        FlatIncrese += 4;
                                        foreach (Population p in Population)
                                        {
                                            if (p.Level == 1)
                                            {
                                                FlatIncrese += 3f;
                                                if (p.Happiness >= 0)
                                                    FlatIncrese += 0.15f * p.Happiness;
                                                else
                                                    FlatIncrese += 0.3f * p.Happiness;
                                            }
                                        }
                                        break;
                                    case 4:
                                        FlatIncrese += 8;
                                        foreach (Population p in Population)
                                        {
                                            if (p.Level == 1)
                                            {
                                                FlatIncrese += 3.5f;
                                                if (p.Happiness >= 0)
                                                    FlatIncrese += 0.17f * p.Happiness;
                                                else
                                                    FlatIncrese += 0.35f * p.Happiness;
                                            }
                                        }
                                        break;
                                    case 5:
                                        FlatIncrese += 12;
                                        foreach (Population p in Population)
                                        {
                                            if (p.Level == 1)
                                            {
                                                FlatIncrese += 4f;
                                                if (p.Happiness >= 0)
                                                    FlatIncrese += 0.2f * p.Happiness;
                                                else
                                                    FlatIncrese += 0.4f * p.Happiness;
                                            }
                                        }
                                        break;
                                }
                                break;
                            case "6": //orchard
                                //++Food(Flat: 2 / 4 / 6 / 8 / 10)
                                FlatIncrese += bs.Level * 2;
                                foreach (Population p in Population)
                                {
                                    if (p.Level == 1)
                                        FlatIncrese += 0.2f * bs.Level;

                                    if (p.Level == 1)
                                    {
                                        FlatIncrese += 0.2f * bs.Level;
                                        if (p.Happiness >= 0)
                                            FlatIncrese += 0.1f * p.Happiness * bs.Level;
                                        else
                                            FlatIncrese += 0.2f * p.Happiness * bs.Level;
                                    }
                                }
                                break;
                            case "3": //forest
                                // +Food(Flat: 8 / 6 / 4 / 2 / 0 | Percentage: 25 / 20 / 10 / 5 / 0)
                                FlatIncrese += 10 - bs.Level * 2;
                                switch (bs.Level)
                                {
                                    case 1:
                                        Modifiers += 0.25f;
                                        break;
                                    case 2:
                                        Modifiers += 0.20f;
                                        break;
                                    case 3:
                                        Modifiers += 0.10f;
                                        break;
                                    case 4:
                                        Modifiers += 0.5f;
                                        break;
                                }
                                break;
                            case "8": //river
                                //+Food(Flat: 1 / 2 / 3 / 4 / 6 | Percentage: 5 / 5 / 10 / 10 / 15)
                                FlatIncrese += bs.Level;
                                if (bs.Level == 5)
                                    FlatIncrese++;
                                switch (bs.Level)
                                {
                                    case 1:
                                        Modifiers += 0.05f;
                                        break;
                                    case 2:
                                        Modifiers += 0.05f;
                                        break;
                                    case 3:
                                        Modifiers += 0.10f;
                                        break;
                                    case 4:
                                        Modifiers += 0.10f;
                                        break;
                                    case 5:
                                        Modifiers += 0.15f;
                                        break;
                                }

                                break;
                            case "market":
                                //???
                                break;
                        }
                    }
                    return Program.NormaliseNumber(BaseFoodOutput * Modifiers, 2) + FlatIncrese;
                }
                else
                {
                    return 0;
                }

            }
        }


        //Modifiers
        /*
          

            1 population eats 2 food
            Also 1 popultion can give 1/2/3/4/5 money depending on the development
            Also the further the province is from capital, the less tax you get.

            You need 100 growth points for a new population
            0.1 food gives 1 growth points

            Each unit costs 10 gold each turn
            It can go down or up depending on the strategy (mass conscription vs. profesional army)

            Research projects:
                
                -Farming technology
                   
                    1:
                        Unlocks Farm and Orchard level 1,2
                        Gives flat 4 food income in capital
                        Gives 5% food base income increase        
                    2:
                        Unlocks Farm and Orchard level 3
                        Gives flat 2 food income in capital
                    3:
                        Gives flat 1 food in every province
                    4:
                        Unlocks Farm and Orchard level 4
                    5:
                        Unlocks Farm and Orchard level 5
                        Gives flat 2 food income in capital
                    
                -Production technology
                    
                    1:
                        Unlocks Factory level 1
                        Gives 1 production in capital
                    2:
                        Unlocks Factory level 2,3
                        -25% building cost in capital
                    3:
                        Gives 2 production in capital
                        +5% production globally
                    4:
                        +5% production globally
                        Unlocks Factory level 4
                    5:
                        +5% production in capital
                        Unlocks Factory level 5


                -Politics technology
                    
                    1:
                        Unlocks Palace
                        Unlocks Food Distribution
                        Unlocks Religion (it will be update)
                    2:
                        Unlocks Money
                        Unlocks Conscription Strategies
                        Unlocks Temple
                    3:
                        Unlocks ?Government Building?
                        Unlocks More Governments (?idk?)
                        Much better colonisation
                    4:
                        Better taxes
                        Unlocks Church
                    5:
                        ???


                -Military technology
                    
                    1:
                        Better units
                        Unlocks Fort level 1
                    2:
                        Better units
                        Unlocks more units (some bowmen or smth)
                    3:
                        Unlocks Fort level 2
                        Better units
                    4:
                        Better units
                    5:
                        Better units
                        Unlocks Fort level 3
                        Units ignore Fort level 1

            2 population makes 1 production point


            + = 5%
            ++ = 10%
            +++ = 20%


        1.Factory:
            +++Production (Flat: 2/3/5/6/8 | Percentage: 5/10/20/30/50)
            -Growth (Percentage: 0/0/5/7.5/10)
            ++Development (???)

          
        0.Free: 
            +Growth (Percentage: 10)

        2.Farm:
            +++Food (Flat: 4/8/12/18/24)
            -Development (???)
            ++Growth (Percentage: 5/10/10/15/15)
            
        3.Forest:
            +Food (Flat: 8/6/4/2/0 | Percentage: 25/20/10/5/0)
            +Production (Flat: 1/1/2/2/4 | Percentage: 0/5/5/10/20)
            +Defence (???)

        4.Fort:
            +++Defence (???)
        
        5.Mountains:
            +Production (???)
            ++Defence (???)

        6.Orchard:
            ++Food (Flat: 2/4/6/8/10)
            +Money (???)
            +Development (???)

        7.Research Lab:
            +++Research (???)
            -Money (???)
            ++Development (???)
        
        8.River:
            +Food (Flat: 1/2/3/4/6 | Percentage: 5/5/10/10/15)
            +++Growth (Percentage: 20/20/25/25/30)
            +Defence (???)

        9.Market:
            +Money
            +Food(Only if more than 10 population)
                 
        10.Administration Buidling?
            ++Growth
            *Better admin*
            --Money
        
        11.Palace
            +Growth
            *Better admin*
            -Money
        
        12.Lake:
            +Growth
            +Food

        



        ?DLC?

        Temple/Church:
        ???



                  
         */


        public List<BuildingSlot> BuildingSlots = new List<BuildingSlot>();
        

        public Tile(int px, int py)
        {
            PosX = px;
            PosY = py;
            BuildingSlots.Add(new BuildingSlot());
            BuildingSlots.Add(new BuildingSlot());
            BuildingSlots.Add(new BuildingSlot());
            Name = Program.GenerateName(Program.random.Next(4,13));
            foreach (BuildingSlot bs in BuildingSlots)
            {
                bs.Level = 0;
                bs.Type = "0";
            }
        }
        public void MakeSoldier(int HP, int MaxHP, int Moves, int MaxMoves, int OwnerID)
        {
            SoldierHP = HP;
            SoldierMaxHP = MaxHP;
            SoldierMoves = Moves;
            SoldierMaxMoves = MaxMoves;
            SoldierOwnerID = OwnerID;
        }
    }
    class BuildingSlot
    {
        public int Level;
        public string Type;
    }


    class TileToUpdate
    {
        public Tile t;
        public string username;
        public bool Read = false;
        public TileToUpdate(Tile tile, string usr)
        {
            t = tile;
            username = usr;
        }
    }
    class Culture
    {
        public string Name = "";
        public Dictionary<Culture, int> Relations = new Dictionary<Culture, int>();
        public Culture(string name)
        {
            Name = name;
        }
    }
    class Population
    {
        public Culture Culture;
        public string Religion = "";
        public int GrowthPoints = 0;
        public int Level = 1;
        public float Happiness = 0;
        public Population(int level, Culture culture)
        {
            Level = level;
            Culture = culture;
        }
    }
    class Government
    {

    }
    class Game
    {
        public List<Culture> Cultures = new List<Culture>();
        public Dictionary<int, Culture> MainCultures = new Dictionary<int, Culture>();
        public List<string> users = new List<string>();
        public List<TileToUpdate> TilesToUpdate = new List<TileToUpdate>();
        public List<Tile> Tiles = new List<Tile>();
        public int Slots;
        public string Host;
        public string Name;
        public string Password;
        public Dictionary<int, float> Gold = new Dictionary<int, float> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 } };
        public Dictionary<int, float> Food = new Dictionary<int, float>();
        public Dictionary<int, float> FoodGain = new Dictionary<int, float> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 } };
        public float AbsoluteFoodGain(int id)
        {
            float FG = 0;
            foreach(Tile t in Tiles)
            {
                if (t.OwnerID == id)
                    FG += t.FoodOutput;
            }
            return FG;
        }
        public Dictionary<int, int> ResearchPoints = new Dictionary<int, int>();
        public Dictionary<int, float> WorkPoints = new Dictionary<int, float>();
        public bool Ongoing = false;
        public Dictionary<int, string> IDUsername = new Dictionary<int, string>();
        public Dictionary<string, int> UsernameID = new Dictionary<string, int>();
        public Dictionary<string, bool> ConnectedToGame = new Dictionary<string, bool>();
        public Dictionary<int, Dictionary<string, int>> Technology = new Dictionary<int, Dictionary<string, int>> {
            { 1,new Dictionary<string, int>
                {
                    {
                        "farming", 0
                    },
                    {
                        "production", 0
                    },
                    {
                        "politics", 0
                    },
                    {
                        "military", 0
                    }
                }   
            },
            { 2,new Dictionary<string, int>
                {
                    {
                        "farming", 0
                    },
                    {
                        "production", 0
                    },
                    {
                        "politics", 0
                    },
                    {
                        "military", 0
                    }
                }
            },
            { 3,new Dictionary<string, int>
                {
                    {
                        "farming", 0
                    },
                    {
                        "production", 0
                    },
                    {
                        "politics", 0
                    },
                    {
                        "military", 0
                    }
                }
            },
            { 4,new Dictionary<string, int>
                {
                    {
                        "farming", 0
                    },
                    {
                        "production", 0
                    },
                    {
                        "politics", 0
                    },
                    {
                        "military", 0
                    }
                }
            }

        };


        public void AddTileToUpdate(Tile t)
        {
            bool seen = false;
            foreach (TileToUpdate tu in TilesToUpdate)
            {
                if (tu.t == t)
                {
                    seen = true;
                    break;
                }
            }
            if (!seen)
            {
                foreach (string usr in users)
                    TilesToUpdate.Add(new TileToUpdate(t, usr));
            }
        }

        public Game(int slots, string host, string name, string password = "")
        {
           
            Slots = slots;
            Host = host;
            Name = name;
            Password = password;
            users.Add(host);
            IDUsername.Add(1, host);
            UsernameID.Add(host, 1);
            ConnectedToGame.Add(host, false);
            Program.Games.Add(this);
            Thread t = new Thread(x => Program.HandleGames(this));            
            t.Start();
            
           
        }
        public void Delete()
        {
            Program.Games.Remove(this);
        }
        public int GetTileCount(int PlayerID)
        {
            int ret = 0;
            foreach(Tile t in Tiles)
            {
                if(t.OwnerID == PlayerID && t.OccupiedBy == 0)
                {
                    ret++;
                }
            }
            return ret;
        }
        public string GetUsers()
        {
            string output = Host + " ";
            foreach (string s in users)
            {
                if (s != Host)
                {
                    output += s + " ";
                }
            }
            output.TrimEnd();
            return output;
        }
        
        public void MakeMap()
        {
            Console.WriteLine("Here it goes!");
            Random rand = new Random();
            for (int a = 0; a < 11; a++)
            { 
                for (int b = 0; b < 12; b++)
                {
                    if (rand.Next(0, 7) > 0)
                    {                  
                        Tile t = new Tile(b, a);
                        Tiles.Add(t);
                        t.OwnerID = 0;
                        
                        if (rand.Next(0,20) == 0)
                        {
                            foreach(BuildingSlot bs in t.BuildingSlots)
                            {
                                if (bs.Type == "nothing")
                                {
                                    bs.Type = "river";
                                    break;
                                }
                            }
                        }
                        if(rand.Next(0,15) == 0)
                        {
                            foreach (BuildingSlot bs in t.BuildingSlots)
                            {
                                if (bs.Type == "nothing")
                                {
                                    bs.Type = "forest";
                                    break;
                                }
                            }
                        }

                                                                 
                    }
                }              
            }
            for(int a = 1; a < users.Count() + 1; a++)
            {

                string c = (Program.Alphabet[rand.Next(0, Program.Alphabet.Count())] + Program.Alphabet[rand.Next(0, Program.Alphabet.Count())] + Program.Alphabet[rand.Next(0, Program.Alphabet.Count())] + Program.Alphabet[rand.Next(0, Program.Alphabet.Count())] + Program.Alphabet[rand.Next(0, Program.Alphabet.Count())]).ToString();
                Cultures.Add(new Culture(c));
                bool done = false;
                do
                {
                    int random = rand.Next(0, Tiles.Count());
                    if (Tiles[random].OwnerID == 0)
                    {
                        Tiles[random].OwnerID = a;
                        Tiles[random].MakeSoldier(5, 5, 2, 2, a);
                        Tiles[random].SoldierOwnerID = a;
                        Tiles[random].Fort = 1;
                        Tiles[random].Capital = 1;
                        done = true;
                        Tiles[random].Development = 3;
                        Tiles[random].Population.Add(new Population(1, Cultures.Where(x => x.Name == c).First()));
                        MainCultures[a] = Cultures.Where(x => x.Name == c).First();
                    }
                }
                while (!done);
                    
            }
            foreach(Tile t in Tiles)
            {
                if(t.Capital == 1)
                {
                    foreach(Tile sometile in Program.Bordering(t,1,Tiles).Keys.Where(x => x.OwnerID == 0))
                    {
                        sometile.OwnerID = t.OwnerID;
                    }
                }
            }
            foreach(Tile t in Tiles)
            {
                AddTileToUpdate(t);
                Dictionary<Tile, int> Borl = Program.Bordering(t, 15, Tiles);
                foreach (Tile bor in Borl.Keys)
                {
                    t.Distances.Add($"{bor.PosX}.{bor.PosY}", Borl[bor]);
                }
            }
        }
    }
    class Program
    {
        public static Random random = new Random();
        public static string Alphabet = "abcdefghijklmnopqrstuwvxyz";
        public static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
        public static Dictionary<Tile, int> Bordering(Tile T, int Distance, List<Tile> Tiles)
        {
            Dictionary<Tile, int> Bordering = new Dictionary<Tile, int>();
            Bordering.Add(T, 0);
            int count = 1;
            for (int a = 0; a < Distance; a++)
            {
                Dictionary<Tile, int> AddToBoredering = new Dictionary<Tile, int>();
                foreach (Tile tile in Bordering.Keys)
                {
                    foreach (Tile t in Tiles)
                    {
                        if (t.PosX % 2 == 1 && !AddToBoredering.Keys.Contains(t) && !Bordering.Keys.Contains(t))
                        {
                            if (t.PosX + 1 == tile.PosX && t.PosY == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX == tile.PosX && t.PosY + 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX - 1 == tile.PosX && t.PosY == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX - 1 == tile.PosX && t.PosY - 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX + 1 == tile.PosX && t.PosY - 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX == tile.PosX && t.PosY - 1 == tile.PosY)
                                AddToBoredering.Add(t, a);
                        }
                        else if (!Bordering.Keys.Contains(t) && !AddToBoredering.Keys.Contains(t) && t.PosX % 2 == 0)
                        {
                            if (t.PosX == tile.PosX && t.PosY + 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX - 1 == tile.PosX && t.PosY == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX + 1 == tile.PosX && t.PosY == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX + 1 == tile.PosX && t.PosY + 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX - 1 == tile.PosX && t.PosY + 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                            if (t.PosX == tile.PosX && t.PosY - 1 == tile.PosY)
                                AddToBoredering.Add(t, a);

                        }

                    }

                }

                foreach (Tile t in AddToBoredering.Keys)
                {
                    if (!Bordering.Keys.Contains(t))
                    {
                        Bordering.Add(t, count);
                    }
                }
                count++;
            }
            return Bordering;
        }
        static void Main(string[] args)
        {            
           
            Console.WriteLine("Server up and ready!");
            Thread t = new Thread(Start);
            t.Start();
            try
            {
                while (true)
                {
                    string command = Console.ReadLine();

                    
                    if (command.Contains("help"))
                    {
                        if (command.Split().Count() > 1)
                        {
                            switch (command.Split()[1])
                            {
                                default:
                                    Console.WriteLine($"No such command as {command.Split()[1]} was found. Type help to learn about the existing commands");
                                    break;
                                case "exit":
                                    Console.WriteLine("exit: Closes the server app");
                                    break;
                                case "gamels":
                                    Console.WriteLine("gamels: Shows info about all running games");
                                    break;
                                case "getgamemaps":
                                    Console.WriteLine("getgamemaps: Shows maps of all games");
                                    break;
                                case "userls":
                                    Console.WriteLine("userls: Shows names of all loged players");
                                    break;
                                case "delgame":
                                    Console.WriteLine("delgame <name of the game>: Deletes a game of given name");
                                    break;
                                case "food":
                                    Console.WriteLine("food <name of the game>: Gives information about current amount of food each player has in given game");
                                    break;

                            }
                        }
                        else
                        {
                            Console.WriteLine("List of possible commands: \nexit gamels getgamemaps userls delgame food");
                        }
                    }
                    else if (command.Contains("exit"))
                    {
                        t.Abort();
                        throw new Exception("Server closed manually");
                        Environment.Exit(0);
                    }
                    else if (command.Contains("gamels"))
                    {
                        Console.Clear();
                        Console.WriteLine("Games running:");
                        foreach (Game g in Games)
                        {
                            Console.WriteLine($"     -{g.Name}: Host: {g.Host} | Password: {g.Password} | Connected users: {g.GetUsers()}");
                        }
                    }
                    else if (command.Contains("getgamemaps"))
                    {
                        Console.Clear();
                        Console.WriteLine("Game maps: ");
                       
                        foreach (Game g in Games)
                        {
                            Console.WriteLine("\n  " + g.Name);
                            string s = "";
                            foreach(Tile ti in g.Tiles)
                            {
                                s += " " + ti.PosX + "." + ti.PosY + "." + ti.OwnerID;
                            }
                            Console.WriteLine(s);
                        }
                    }
                    else if (command.Contains("userls"))
                    {
                        Console.Clear();
                        Console.WriteLine("Connected useres:");
                        foreach (string u in Users)
                        {

                            Console.WriteLine($"     -{u}");
                        }
                    }
                    else if (command.Contains("delgame"))
                    {
                        Console.WriteLine(command);
                        string[] commandsplit = command.Split(' ');
                        Game ga = null;
                        foreach (Game g in Games)
                        {
                            if (g.Name == commandsplit[1])
                                ga = g;
                        }
                        if (ga != null)
                            ga.Delete();
                        else
                            Console.WriteLine($"No such game as {commandsplit[1]} exists!");
                    }
                    else if (command.Contains("food"))
                    {
                        string[] attrib = command.Split(' ');
                        try
                        {
                            int a = 1;
                            foreach(int f in Games.Where(x => x.Name == attrib[1]).First().Food.Values)
                            {
                                Console.WriteLine($"-{a}: {f}");
                                a++;
                            }
                            
                        }
                        catch
                        {
                            Console.WriteLine("Wrong arguments! food <gamename> is the correct construction of this command");
                        }
                    }               
                    else if (command.Contains("chggamvar"))
                    {
                        try
                        {
                            string[] attrib = command.Split(' ');
                            Game ChgGame = null;
                            Tile ChgTile = null;
                            foreach (Game g in Games)
                            {
                                if (g.Name == attrib[1])
                                    ChgGame = g;
                            }
                            switch (attrib[2])
                            {
                                case "province":

                                    if (attrib[3].Contains("capital_"))
                                    {
                                        attrib[3].Remove("capital_".Count());
                                        int id = int.Parse(attrib[3]);
                                        ChgTile = ChgGame.Tiles.First(x => x.Capital == 1 && x.OwnerID == id);
                                    }

                                    break;
                            }
                            switch (attrib[4])
                            {
                                case "pop":
                                   
                                    ChgGame.AddTileToUpdate(ChgTile);
                                    break;
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Error!");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Server closed with error: " + e.Message);
                
            }

            
        }
        public static void Start()
        {
            listener.Start();
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }
        public static List<Game> Games = new List<Game>();
        public static List<string> Users = new List<string>();
        static TcpListener listener = new TcpListener(2055);
        public static List<Thread> Threads = new List<Thread>();
        public static float NormaliseNumber(float number, int digits)
        {
            float d = number * 100;
            d = (float)Math.Floor(d);
            d = d * 0.01f;
            return d;

        }
        public static void HandleGames(Game game)
        {
            Console.WriteLine($"Game {game.Name} has been started by user {game.Host}");
            game.Food.Add(1, 10);
            game.Food.Add(2, 10);
            game.Food.Add(3, 10);
            game.Food.Add(4, 10);
            try
            {
                Random r = new Random();
                while (true)
                {
                    Thread.Sleep(500);
                    if (game.Ongoing && game.ConnectedToGame.All(x => x.Value == true))
                    {
                        while (true)
                        {
                            Thread.Sleep(15000);
                            List<TileToUpdate> Remove = new List<TileToUpdate>();
                            foreach(TileToUpdate t in game.TilesToUpdate)
                            {
                                if (t.Read)
                                    Remove.Add(t);
                            }
                            game.TilesToUpdate.RemoveAll(x => Remove.Contains(x));
                           
                            

 
                            foreach (Tile t in game.Tiles)
                            {
                                t.FoodFromOtherEntities = 0;
                                if (t.SoldierHP > 0)
                                {
                                    if (t.SoldierHP < t.SoldierMaxHP && t.SoldierMoves == t.SoldierMaxMoves && t.OccupiedBy == 0)
                                        t.SoldierHP++;
                                    if (t.SoldierMoves > 0 && t.OccupiedBy != 0)
                                    {
                                        if (Bordering(t, 1, game.Tiles).Any(x => x.Key.Fort == 0 && x.Key.OccupiedBy == 0))
                                        {
                                            if (new Random().Next(1, 4) == 1)
                                            {
                                                t.OwnerID = t.OccupiedBy;
                                                t.OccupiedBy = 0;
                                                game.AddTileToUpdate(t);
                                                
                                            }
                                        }
                                        else
                                        {
                                            if (new Random().Next(1, 21) == 1)
                                            {
                                                t.OwnerID = t.OccupiedBy;
                                                t.OccupiedBy = 0;
                                                game.AddTileToUpdate(t);
                                            }
                                        }
                                    }
                                    if (t.SoldierMoves != t.SoldierMaxMoves)
                                    {
                                        t.SoldierMoves = t.SoldierMaxMoves;
                                        if(!game.TilesToUpdate.Any(x => x.t == t))
                                        {
                                            game.AddTileToUpdate(t);
                                        }
                                    }
                                    
                                }
                            }
                            List<Tile> SortedTiles = game.Tiles.ToArray().ToList();
                            SortedTiles.OrderBy(x => x.Population);
                            SortedTiles.Reverse();
                            
                            foreach(Tile t in SortedTiles)
                            {
                                float foodo = t.FoodOutput;
                                
                                
                                if(foodo - t.GetPopFoodUsage()  > 0.1 && t.OwnerID != 0)
                                {
                                    foodo -= t.GetPopFoodUsage();
                                    float maxfoodo = foodo;
                                    t.GrowthProgress += (int)Math.Floor(foodo * 2.5);
                                    foodo *= 0.75f;
                                    foodo -= maxfoodo * 0.1f;
                                    List<Tile> surr = new List<Tile>();
                                    foreach(Tile SurrTile in Bordering(t, 1, game.Tiles).Keys.Where(x => x.OwnerID == t.OwnerID))
                                    {
                                        surr.Add(SurrTile);
                                    }
                                    float fd = maxfoodo * 0.1f;
                                    foodo -= fd;
                                    foreach (Tile st in surr)
                                    {
                                        st.FoodFromOtherEntities += fd / surr.Count();
                                    }

                                    List<Tile> MostPopulated = new List<Tile>();
                                    foreach(Tile mostpop in SortedTiles)
                                    {
                                        if(mostpop.OwnerID == t.OwnerID)
                                        {
                                            MostPopulated.Add(mostpop);
                                        }
                                    }
                                    foodo -= maxfoodo * 0.25f;
                                    if(MostPopulated.Count > 4)
                                    {
                                        for(int a = 0; a < 5; a++)
                                        {
                                            MostPopulated[a].FoodFromOtherEntities += maxfoodo * 0.05f;
                                        }
                                    }
                                    else
                                    {
                                        foreach(Tile mos in MostPopulated)
                                        {
                                            mos.FoodFromOtherEntities += maxfoodo * 0.25f / MostPopulated.Count();
                                        }
                                    }
                                    game.Food[t.OwnerID] += NormaliseNumber(foodo, 2);
                                    
                                    
                                }
                                
                            }
                            foreach(Tile t in SortedTiles)
                            {
                                float foodomax = t.FoodOutput + t.FoodFromOtherEntities;
                    
                                if(foodomax >= t.GetPopFoodUsage())
                                {
                                    foodomax -= t.GetPopFoodUsage();
                                    t.GrowthProgress += (int)Math.Floor(foodomax * 10f);
                                    if(t.GrowthProgress >= 100)
                                    {
                                        t.GrowthProgress -= 100;
                                        t.Population.Add(new Population(1, t.Population[new Random().Next(0,t.Population.Count)].Culture));
                                        game.AddTileToUpdate(t);
                                    }
                                }
                                else
                                {
                                    float miss = (t.FoodOutput + t.FoodFromOtherEntities) - t.GetPopFoodUsage();
                                    t.GrowthProgress -= (int)Math.Floor(miss * 10f);
                                    if(t.GrowthProgress < 0)
                                    {
                                        t.Population.Remove(t.Population[new Random().Next(0, t.Population.Count)]);
                                        t.GrowthProgress = 75;
                                        game.AddTileToUpdate(t);
                                    }

                                }
                            }



                            //Make options like 10% food given automaticaly etc.

                            //So 25% goes to the province that produces food x
                            //10% goes to the surounding provinces x
                            //25% goes to the top 5 most poulous provinces x
                            //rest, 40% goes to the player or is distributed 

                            for(int a = 1; a < 5; a++)
                            {
                                game.Food[a] *= 0.9f;
                            }

                            Console.WriteLine("Turn made: Player 1 has: " + game.Food[1] + " food. Capital of player 1 has: " + game.Tiles.Where(x => x.Capital == 1 && x.OwnerID == 1).First().GrowthProgress + " growth progress");
                            if (!Games.Contains(game))
                            {
                                throw new Exception("No users");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Game {game.Name} was terminated because of the following error: {e.Message}");
                game.Delete();
                
            }
        }
        
        public static void HandleClient(object client)
        {
            string username = "";
            TcpClient tcpclient = (TcpClient)client;
            Stream s = tcpclient.GetStream();
            StreamWriter Write = new StreamWriter(s);
            StreamReader Read = new StreamReader(s);
            Write.AutoFlush = true;
            tcpclient.ReceiveTimeout = 180000;
            try
            {
                string msg = Read.ReadLine();
                if (msg == "connect")
                {
                    username = Read.ReadLine();
                    Console.WriteLine($"User { username } connected!");
                    Users.Add(username);
                }
                else
                    throw new Exception();
                while (true)
                {
                    
                    string message = Read.ReadLine();

                    if (message.StartsWith("create game"))
                    {
                        string[] split = message.Split(' ');
                        Game g = new Game(int.Parse(split[2]), username, split[3], split[4]);

                    }
                    else if (message.StartsWith("get lobby"))
                    {
                        Game ga = null;
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {
                                ga = g;

                            }
                        }

                        if (ga == null)
                        {
                            Write.WriteLine("null");
                            Console.WriteLine("null");
                        }
                        else if (ga.Ongoing == false)
                        {
                            string somestring = ga.GetUsers();
                            string me = $"{ga.Name} {ga.Slots.ToString()} {ga.Host} {somestring}";
                            Write.WriteLine(me);
                            
                        }
                        else if (ga.Ongoing == true)
                        {
                            Write.WriteLine("start");

                        }
                    }
                    else if (message.StartsWith("mov"))
                    {
                        string[] split = message.Split(' ');
                        Tile Home = null;
                        Tile Dest = null;
                        Console.WriteLine(message);

                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {
                                foreach (Tile t in g.Tiles)
                                {
                                    if (t.PosX == int.Parse(split[1].Split('.')[0]) && t.PosY == int.Parse(split[1].Split('.')[1]))
                                        Home = t;
                                    else if (t.PosX == int.Parse(split[2].Split('.')[0]) && t.PosY == int.Parse(split[2].Split('.')[1]))
                                        Dest = t;
                                }


                                if (Home.SoldierOwnerID == g.UsernameID[username])
                                {
                                    if (Dest.OwnerID == 0)
                                    {
                                        Dest.MakeSoldier(Home.SoldierHP, Home.SoldierMaxHP, 0, Home.SoldierMaxMoves, g.UsernameID[username]);
                                        Home.MakeSoldier(0, 0, 0, 0, 0);

                                        g.AddTileToUpdate(Dest);
                                        g.AddTileToUpdate(Home);

                                    }
                                    else if (Dest.OwnerID == g.UsernameID[username] || Dest.OccupiedBy == g.UsernameID[username])
                                    {
                                        if (Dest.SoldierHP == 0)
                                        {
                                            Dest.MakeSoldier(Home.SoldierHP, Home.SoldierMaxHP, Home.SoldierMoves - Home.Distances[$"{Dest.PosX}.{Dest.PosY}"], Home.SoldierMaxMoves, g.UsernameID[username]);
                                            Home.MakeSoldier(0, 0, 0, 0, 0);
                                            foreach (string u in g.users)
                                            {
                                                g.AddTileToUpdate(Dest);
                                                g.AddTileToUpdate(Home);
                                            }
                                        }
                                    }
                                    else if (Dest.OwnerID != g.UsernameID[username] && Dest.OccupiedBy != g.UsernameID[username])
                                    {
                                        if (Dest.SoldierHP > 0)
                                        {
                                            if (Dest.Fort > 0)
                                            {
                                                Home.SoldierHP -= new Random().Next(2, 4);
                                            }
                                            else
                                            {
                                                Home.SoldierHP -= new Random().Next(1, 4);
                                            }
                                            if (Home.SoldierHP > 0)
                                                Dest.SoldierHP -= new Random().Next(1, 3);
                                            else
                                                Dest.SoldierHP -= 1;
                                            Dest.SoldierMoves -= 1;
                                            Home.SoldierMoves = 0;
                                            if (Dest.SoldierHP <= 0)
                                            {
                                                Dest.MakeSoldier(Home.SoldierHP, Home.SoldierMaxHP, 0, Home.SoldierMaxMoves, g.UsernameID[username]);
                                                Home.MakeSoldier(0, 0, 0, 0, 0);
                                                Dest.OccupiedBy = g.UsernameID[username];
                                            }
                                            foreach (string u in g.users)
                                            {
                                                g.AddTileToUpdate(Dest);
                                                g.AddTileToUpdate(Home);
                                            }

                                        }
                                        else
                                        {
                                            
                                            Dest.MakeSoldier(Home.SoldierHP, Home.SoldierMaxHP, 0, Home.SoldierMaxMoves, g.UsernameID[username]);
                                            Home.MakeSoldier(0, 0, 0, 0, 0);
                                            foreach (string u in g.users)
                                            {
                                                g.AddTileToUpdate(Dest);
                                                g.AddTileToUpdate(Home);
                                            }
                                        }
                                    }

                                }
                            }
                        }


                    }
                    else if (message.StartsWith("getmap"))
                    {
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {
                                string somestring = "";
                                foreach (Tile t in g.Tiles)
                                {
                                    somestring += t.PosX + "." + t.PosY + "." +t.Name + " ";
                                }
                                somestring = somestring.Trim();
                                Write.WriteLine(somestring);
                                Console.WriteLine(somestring);
                                g.ConnectedToGame[username] = true;
                            }
                        }
                    }
                    else if (message.StartsWith("getgameinfo"))
                    {
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {

                                string somestring = $"{NormaliseNumber(g.Gold[g.UsernameID[username]], 2)} {NormaliseNumber(g.Food[g.UsernameID[username]], 2)} {g.FoodGain[g.UsernameID[username]]} {g.AbsoluteFoodGain(g.UsernameID[username])}|";

                                foreach (TileToUpdate t in g.TilesToUpdate)
                                {
                                    if (t.username == username)
                                    {
                                        int serfs = 0;
                                        int burg = 0;
                                        int nobi = 0;

                                        foreach (Population p in t.t.Population)
                                        {
                                            if (p.Level == 1)
                                                serfs++;
                                            else if (p.Level == 2)
                                                burg++;
                                            else if (p.Level == 3)
                                                nobi++;            
                                        }

                                        if (t.t.SoldierHP > 0)
                                            somestring += $"{t.t.PosX}.{t.t.PosY}.{t.t.OwnerID}.{t.t.Capital}.{t.t.Development}.{t.t.OccupiedBy}.{serfs}.{burg}.{nobi}.{t.t.BuildingSlots[0].Type}.{t.t.BuildingSlots[0].Level}.{t.t.BuildingSlots[1].Type}.{t.t.BuildingSlots[1].Level}.{t.t.BuildingSlots[2].Type}.{t.t.BuildingSlots[2].Level}.{t.t.SoldierHP}.{t.t.SoldierMaxHP}.{t.t.SoldierMoves}.{t.t.SoldierMaxMoves}.{t.t.SoldierOwnerID} ";
                                        else
                                            somestring += $"{t.t.PosX}.{t.t.PosY}.{t.t.OwnerID}.{t.t.Capital}.{t.t.Development}.{t.t.OccupiedBy}.{serfs}.{burg}.{nobi}.{t.t.BuildingSlots[0].Type}.{t.t.BuildingSlots[0].Level}.{t.t.BuildingSlots[1].Type}.{t.t.BuildingSlots[1].Level}.{t.t.BuildingSlots[2].Type}.{t.t.BuildingSlots[2].Level} ";
                                       
                                        t.Read = true;
                                    }
                                }
                                Write.WriteLine(somestring);
                            }
                        }
                    }
                    else if (message.StartsWith("getgropro "))
                    {
                        string[] split = message.Split(' ');
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {

                                foreach (Tile t in g.Tiles)
                                {
                                    if (t.PosX == int.Parse(split[1].Split('.')[0]) && t.PosY == int.Parse(split[1].Split('.')[1]))
                                    {
                                        Write.WriteLine(t.GrowthProgress);
                                    }
                                }
                            }
                        }
                    }
                    else if (message.StartsWith("invfod "))
                    {
                        string[] split = message.Split(' ');
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {
                                foreach (Tile t in g.Tiles)
                                {
                                    if (t.PosX == int.Parse(split[1].Split('.')[0]) && t.PosY == int.Parse(split[1].Split('.')[1]))
                                    {
                                        t.GrowthProgress += (int)Math.Floor(float.Parse(split[2]) * 10f);
                                        g.Food[g.UsernameID[username]] -= float.Parse(split[2]);
                                        if (t.GrowthProgress >= 100)
                                        {
                                            t.GrowthProgress = t.GrowthProgress - 100;
                                            if (t.OwnerID != 0)
                                            {                                               
                                                t.Population.Add(new Population(1, t.Population[new Random().Next(0, t.Population.Count)].Culture));
                                            }
                                            else
                                            {
                                                t.Population.Add(new Population(1, g.MainCultures[g.UsernameID[username]]));
                                            }
                                        }
                                        g.AddTileToUpdate(t);
                                    }
                                }
                            }
                        }
                    }
                    else if (message.StartsWith("get games"))
                    {
                        string tosend = "";
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {
                                tosend = "fuckyou";
                                break;
                            }
                            if (g.Ongoing == false && g.users.Count() < g.Slots)
                                tosend += $"#{g.Name}|{g.Slots}|{g.GetUsers()}";
                        }
                        Write.WriteLine(tosend);
                    }
                    else if (message.StartsWith("join lobby"))
                    {
                        bool joined = false;
                        foreach (Game g in Games)
                        {
                            if (!g.Ongoing && g.Name == message.Split(' ')[2])
                            {
                                if (g.Password == message.Split(' ')[3])
                                {
                                    g.users.Add(username);
                                    if (!g.IDUsername.Values.Contains(username))
                                    {
                                        g.IDUsername.Add(g.IDUsername.Count() + 1, username);
                                        Write.WriteLine(g.IDUsername.Count());
                                        g.UsernameID.Add(username, g.IDUsername.Count());
                                        g.ConnectedToGame.Add(username, false);
                                    }
                                    joined = true;
                                    break;

                                }
                                

                            }
                            
                        }
                        if (joined)
                        {
                            Write.WriteLine("joined");

                        }
                        else
                        {
                            Write.WriteLine("notjoined");
                        }
                    }
                    else if (message.StartsWith("disconnect"))
                    {
                        throw new Exception("User " + username + " disconnected manually");
                    }
                    else if (message.StartsWith("start game"))
                    {

                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                            {
                                g.Ongoing = true;
                                g.MakeMap();
                                Console.WriteLine($"Game { g.Name} startes!");
                            }
                        }

                    }
                    else if (message.StartsWith("exit lobby"))
                    {
                        Game ga = null;
                        foreach (Game g in Games)
                        {
                            if (g.users.Contains(username))
                                ga = g;
                        }
                        ga.users.Remove(username);
                        if (ga.users.Count() == 0 || ga.Host == username)
                            ga.Delete();
                    }
                    else
                    {
                        throw new Exception("Wrong command");
                    }
                            
                    
                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Game ga = null;
                foreach (Game g in Games)
                {
                    if (g.users.Contains(username))
                    {
                        g.users.Remove(username);
                        if (g.users.Count() == 0 || g.Host == username)
                            ga = g;
                     
                    }
                }
                if (ga != null)
                    ga.Delete();
                        tcpclient.Close();
            }
            finally
            {
                Console.WriteLine($"User {username} has disconnected!");
                Users.Remove(username);
            }                       
        }
    }
}
