using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

class Program
{
    static void Main()
    {
        string inputString;
        string inputKey;

        List<BaseCharacter> allies = new List<BaseCharacter>(){};
        List<BaseCharacter> bandits = new List<BaseCharacter>(){};
        Team alliesTM;
        Team banditsTM;

        inputKey = Console.ReadLine();
        BaseCharacter character;
        while (true)
        {
            switch (inputKey)
            {
                case "hero":
                {
                    while (inputKey == "hero")
                    {
                        inputString = Console.ReadLine();
                        if (inputString == "enemy" || inputString == "end")
                        {
                            inputKey = inputString;
                            goto changedKey;
                        }

                        character = BaseCharacter.CreateChar(inputString);
                        allies.Add(character);  
                    }
                    break;
                }
                
                
                case "enemy":
                    while (inputKey == "enemy")
                    {
                        inputString = Console.ReadLine();
                        if (inputString == "hero" || inputString == "end")
                        {
                            inputKey = inputString;
                            goto changedKey;
                        }
                    
                        character = BaseCharacter.CreateChar(inputString);
                        bandits.Add(character);
                    }
                    break;


                case "end":
                    if (allies.Count == 0 || bandits.Count == 0)
                    {
                        throw new Exception("Invalid numbers of characters");
                    }
                    alliesTM = new Team(allies, TypeOfTeams.heroes);
                    banditsTM = new Team(bandits, TypeOfTeams.bandits);
                    goto Battle;
                    
                
                default:
                    throw new Exception("Invalid team key");
            }
            changedKey: ;
        }
        
        Battle:
        Intro.PrintIntro(alliesTM, banditsTM);
        
        //Okeeeeeey let's goooo
        int moveCounter = 1;
        while (alliesTM.AmountOfAlives != 0 && banditsTM.AmountOfAlives != 0)
        {
            if (moveCounter % 2 != 0)
            {
                alliesTM.AttackTeam(banditsTM);
                // Console.WriteLine("---------------");
            }
            else
            {
                banditsTM.AttackTeam(alliesTM);
                // Console.WriteLine("---------------");
            }
        
            moveCounter += 1;
        }
    }
    
    

    class Team
    {
        public List<BaseCharacter> Members  {get; private init; }
        private bool isSmbAlive;
        private int amountOfAlives;
        public Enum TypeOfTeam { get; init; }

        public int AmountOfAlives
        {
            get => amountOfAlives;
            set
            {
                amountOfAlives = value;

                if (TypeOfTeam.Equals(TypeOfTeams.heroes) && AmountOfAlives == 0)
                {
                    switch (Members.Count)
                    {
                        case 1:
                            Console.WriteLine("Unfortunately our hero was brave, yet not enough skilled, or just lack of luck.");
                            break;
                        default:
                            Console.WriteLine("Unfortunately our heroes were brave, yet not enough skilled, or just lack of luck.");
                            break;
                    }
                }
                else if (TypeOfTeam.Equals(TypeOfTeams.bandits) && AmountOfAlives == 0)
                {
                    Console.WriteLine("Congratulations!");
                }
                
                
            }
        }
     
        
        public Team( List<BaseCharacter> members, Enum typeOfTeam)
        {
            TypeOfTeam = typeOfTeam;
            // IsSmbAlive = true;
            Members = members;
            foreach (var member in Members)
            {
                member.AllTeam = this;
            }
            amountOfAlives = Members.Count;
        }

        public void AttackTeam(Team enemyTeam)
        {
            foreach (var member in Members)
            {
                if (member.IsAlive)
                {
                    var sortedEnemyTeam = enemyTeam.Members.OrderBy(member => member.TargetPrioritet).ToList();
                    BaseCharacter aimForAttack = null;
                
                    //find alive enemy
                    for (int i = 0, amount = sortedEnemyTeam.Count; i < amount; i++)
                    {
                        if (sortedEnemyTeam[i].IsAlive)
                        {
                            aimForAttack = sortedEnemyTeam[i];
                            break;
                        }
                    }
                    
                    member.Attack(aimForAttack);
                    if (enemyTeam.AmountOfAlives == 0)
                    {
                        break;
                    }

                }
            }
        }
    }
    class BaseCharacter
    {
        //TODO: дописть параметры живости и логики когда хп =< 0, то мертв значит. + Проверить что при недостатке маны будет бить рукой маг.
        
        private int healthPoints;
        private int manaPoints;
        private int armor;
        private int magicArmor;
        private int vitality;
        private int intelligence;
        private int agility;

        public int TargetPrioritet { get; private set; }

        public string Type { get; init; }

        public Team AllTeam { get; set; }
        public Weapon Weapon { get; init; }
        public MagicSkill MagicSkill { get; init; }
        
        public string Name { get; init; }
        public int Strenght { get; set; }
        public int Agility
        {
            get => agility;
            set
            {
                agility = value;
                Armor = (int)Math.Round(Agility / 2.0);
            }
        }

        public int Vitality 
        {
            get => vitality;
            set
            {
                vitality = value;
                HealthPoints = Vitality * 4;
            }
        }

        public int Intelligence
        {
            get => intelligence;
            set
            {
                intelligence = value;
                MagicArmor = (int)Math.Round(Intelligence / 2.0);
                ManaPoints = Intelligence * 4;
            }
        }
        
        public int HealthPoints
        {
            get => healthPoints;
            set
            {
                healthPoints = value;
                ChangeTargetPrioritet();
                if (healthPoints <= 0)
                {
                    healthPoints = 0;
                    Console.WriteLine($"{Type} {Name} is defeated!");
                    IsAlive = false;
                    AllTeam.AmountOfAlives -= 1;
                }
            }
        }
        
        public bool IsAlive { get; private set;}
       
        
        public int ManaPoints
        {
            get => manaPoints;
            set => manaPoints = value;
        }
        public int Armor
        {
            get => armor;
            set
            {
                armor = value;
                ChangeTargetPrioritet();
            } 
        }
        
        public int MagicArmor
        {
            get => magicArmor;
            set => magicArmor = value;
        }

        private protected BaseCharacter(string name, int strength, int agility, int vitality, int intelligence, string type)
        {
            IsAlive = true;
            Name = name;
            Strenght = strength;
            Agility = agility;
            Vitality = vitality;
            Intelligence = intelligence;
            Type = type;
            // MagicArmor = (int)Math.Round(Intelligence / 2.0);
            // Armor = (int)Math.Round(Agility / 2.0);
            
        }

        private void ChangeTargetPrioritet()
        {
            TargetPrioritet = Armor + HealthPoints;
        }
        public static BaseCharacter CreateChar(string inputString)
        {
            string patternInfoCharacter = @"(\w+) ([-]*\d+)\s+([-]*\d+)\s+([-]*\d+)\s+([-]*\d+)\s+([a-zA-Z 0-9]+)";
            var charaterinfo = Regex.Match(inputString, patternInfoCharacter).Groups;
            if (charaterinfo.Count < 6)
            {
                throw new Exception("Invalid amount of input paramets");
            }
            string charType = charaterinfo[1].Value;
            int strenght = int.Parse(charaterinfo[2].Value);
            int agility = int.Parse(charaterinfo[3].Value);
            int vitality = int.Parse(charaterinfo[4].Value);
            int intelligence = int.Parse(charaterinfo[5].Value);
            string charName = charaterinfo[6].Value;

            int[] characterCharacteristics = new[] {strenght, agility, vitality, intelligence };

            foreach (var characteristic in characterCharacteristics)
            {
                if (characteristic < 0)
                {
                    throw new ArgumentException("Value must be greater than zero");
                }
            }

            BaseCharacter character;
            switch (charType)
            {
                case "knight":
                {
                    character = new Knight(charName, strenght, agility, vitality, intelligence);
                    break; 
                }

                case "thief":
                {
                    character = new Thief(charName, strenght, agility, vitality, intelligence);
                    break; 
                }

                case "mage":
                {
                    character = new Mage(charName, strenght, agility, vitality, intelligence);
                    break;
                }
                
                default:
                    throw new ArgumentException("Invalid class of character"); 
            }
            
            return character;
        }

        public void PrintResultDamage(int healthPoints, int resultingDamage)
        {
            int remainingHealthPoints = healthPoints - resultingDamage;
            remainingHealthPoints = remainingHealthPoints > 0 ? remainingHealthPoints : 0;
            Console.WriteLine($"{Type} {Name} get hit for {resultingDamage} hp and have {remainingHealthPoints} hp left!"); 
        }
        public void Attack(BaseCharacter enemy)
        {
            int damage;
            Enum typeOfAttack;
            if (MagicSkill != null && ManaPoints > MagicSkill.ManaCost)
            {
                damage = MagicSkill.UseSkill();
                typeOfAttack = TypeOfAttack.magical;
                ManaPoints -= MagicSkill.ManaCost;
                
                foreach (var enemyMemeber in enemy.AllTeam.Members)
                {
                    if (enemyMemeber.IsAlive)
                    {
                        Console.WriteLine($"{Type} {Name} attacking {enemyMemeber.Type} {enemyMemeber.Name} with {MagicSkill.Name}.");
                        enemyMemeber.GetDamage(damage, typeOfAttack); 
                    }
                }
            }
            else
            {
                damage = Weapon.GetTotalDamage();
                typeOfAttack = TypeOfAttack.phisical;
                Console.WriteLine($"{Type} {Name} attacking {enemy.Type} {enemy.Name} with {Weapon.Name}.");
                enemy.GetDamage(damage, typeOfAttack);  
            }

        }

        //TODO:can I change access modifier?
        public int GetDamage(int incomingDamage, Enum typeOfDamage)
        {
            int resultingDamage;
            if (typeOfDamage.Equals(TypeOfAttack.phisical))
            {
                resultingDamage = incomingDamage - Armor - Agility;
            }
            else 
            {
                resultingDamage = incomingDamage - MagicArmor - Intelligence;
            }

            resultingDamage = resultingDamage < 0 ? 0 : resultingDamage;

            PrintResultDamage(HealthPoints, resultingDamage);
            HealthPoints -= resultingDamage;
            return resultingDamage;
        }
    }

    class  Knight : BaseCharacter
    {
        public Knight(string name, int strenght, int agility, int vitality, int intelligence) :
               base(name, strenght, agility, vitality, intelligence, "Knight")
        {
            HealthPoints += 15;
            Strenght += 2;
            Armor += 2;
            Weapon = new Sword(this);
        }
    }
    
    class  Thief : BaseCharacter
    {
        public Thief(string name, int strenght, int agility, int vitality, int intelligence) :
            base(name, strenght, agility, vitality, intelligence, "Thief")
        {
            Agility += 3;
            Weapon = new Dagger(this);
        }
    }
    
    class  Mage : BaseCharacter
    {
        
        public Mage(string name, int strenght, int agility, int vitality, int intelligence) :
            base(name, strenght, agility, vitality, intelligence, "Mage")
        {
            Intelligence += 5;
            ManaPoints += 25;
            MagicArmor += 2;
            Weapon = new Staff(this);
            MagicSkill = new ChainLightning(this);
        }
    }

    class Weapon
    {
        public string Name { get; private init; }
        public int BaseDamage { get; init; }
        public int BonusDamage { get; init; }
        public int BonusSkillDamage { get; init; }
        public BaseCharacter Owner { get; set; }

        private protected Weapon(string weaponName, BaseCharacter owner)
        {
            Name = weaponName;
            Owner = owner;
        }

        public int GetTotalDamage()
        {
            return BaseDamage + BonusDamage;
        }
    }

    class Sword:Weapon
    {
        public Sword(BaseCharacter owner):base("sword", owner)
        {
            BaseDamage = 5;
            BonusDamage = Owner.Strenght;
        }
    }
    
    class Dagger:Weapon
    {
        public Dagger(BaseCharacter owner):base("dagger", owner)
        {
            BaseDamage = 4;
            BonusDamage = Owner.Agility;
        }
    }
    class Staff:Weapon
    {
        public Staff(BaseCharacter owner):base("staff", owner)
        {
            BaseDamage = 15;
            BonusDamage = Owner.Strenght;
            BonusSkillDamage = 10;
        }
    }

    class MagicSkill
    {
        public string Name { get; private init; }
        public int ManaCost { get; private set; }
        public int MagicDamage { get; private protected set; }
        public BaseCharacter Owner { get; set; }
        private protected MagicSkill(string skillName, int manaCost)
        {
            Name = skillName;
            ManaCost = manaCost;
        }

        public int UseSkill()
        {
            return MagicDamage;
        }
    }

    class ChainLightning:MagicSkill
    {
        public ChainLightning(BaseCharacter skillUser) : base("chain lightning", 40)
        {
            MagicDamage = skillUser.Weapon.BonusSkillDamage + skillUser.Intelligence;
        }
    }
    enum TypeOfAttack
    {
        phisical,
        magical,
    }
    
    enum TypeOfTeams
    {
        heroes,
        bandits,
    }

    class Intro
    {
        static public void PrintIntro(Team alliesTM, Team enemyTM)
        {
            Console.WriteLine("Stay a while and listen, and I will tell you a story. A story of Dungeons and Dragons, of " +
                              "Orcs and Goblins, of Ghouls and Ghosts, of Kings and Quests, but most importantly -- of Heroes " +
                              "and NoobCo -- Well... A story of Heroes.");

            string aboutHeroesAndEnimies;
            string[] namesOfHeroes = new string[alliesTM.Members.Count];
            string[] namesOfEnimies = new string[enemyTM.Members.Count];
            
            if (alliesTM.Members.Count == 1)
            {
                aboutHeroesAndEnimies = $"So here starts the journey of our hero {alliesTM.Members[0].Type} {alliesTM.Members[0].Name} got order to eliminate the local ";
            }
            else
            {
                int membersAmount = alliesTM.Members.Count;
                for (int i = 0; i < membersAmount; i++)
                {
                    namesOfHeroes[i] = $"{alliesTM.Members[i].Type} {alliesTM.Members[i].Name}";
                   
                }
                
                aboutHeroesAndEnimies = $"So here starts the journey of our heroes: {String.Join(", ", namesOfHeroes)} got order to eliminate the local ";
            }
            
            if (enemyTM.Members.Count == 1)
            {
                namesOfEnimies[0] = $"{enemyTM.Members[0].Type} {enemyTM.Members[0].Name}";
                aboutHeroesAndEnimies += $"bandit known as {enemyTM.Members[0].Type} {enemyTM.Members[0].Name}.";
            }
            else
            {
                int membersAmount = enemyTM.Members.Count;
                for (int i = 0; i < membersAmount; i++)
                {
                    namesOfEnimies[i] = $"{enemyTM.Members[i].Type} {enemyTM.Members[i].Name}";
                }

                aboutHeroesAndEnimies += $"gang consists of well known bandits: {String.Join(", ", namesOfEnimies)}.";

            }
            Console.WriteLine(aboutHeroesAndEnimies);

            if (namesOfHeroes.Length > 1)
            {
                Console.WriteLine($"{String.Join(", ", namesOfHeroes)} engaged the {String.Join(", ", namesOfEnimies)}.");  
            }
            
        }
    }
    
}

    

 
    


