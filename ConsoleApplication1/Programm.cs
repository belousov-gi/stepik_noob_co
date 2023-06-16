using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

class Program
{
    static void Main()
    {
        string inputString;

        List<BaseCharacter> allies = new List<BaseCharacter>(){};
        List<BaseCharacter> bandits = new List<BaseCharacter>(){};
        
        inputString = Console.ReadLine();

        if (inputString == "hero")
        {
            while (true)
            {
                inputString = Console.ReadLine();
                if (inputString == "enemy")
                {
                    break;
                }
                var character = BaseCharacter.CreateChar(inputString);
                allies.Add(character); 
            }

            if (inputString == "enemy")
            {
                while (true)
                {
                    inputString = Console.ReadLine();
                    if (inputString == "end")
                    {
                        break;
                    }
                    var character = BaseCharacter.CreateChar(inputString);
                    bandits.Add(character);
                }
            }
            
            else
            {
                Console.WriteLine("Enter evil characters");
            }
        }
        
        else 
        {
            Console.WriteLine("Enter kind characters");
        }
        
        Team alliesTM = new Team(allies, TypeOfTeams.heroes);
        Team banditsTM = new Team(bandits, TypeOfTeams.bandits);

        Intro.PrintIntro(alliesTM, banditsTM);

        //Okeeeeeey let's goooo
        int moveCounter = 1;
        while (alliesTM.IsSmbAlive && banditsTM.IsSmbAlive)
        {
            if (moveCounter % 2 != 0)
            {
                alliesTM.AttackTeam(banditsTM);
            }
            else
            {
                banditsTM.AttackTeam(alliesTM); 
            }
            moveCounter += 1;
        }
    }
    
    

    class Team
    {
        public List<BaseCharacter> Members  {get; private init; }
        private bool isSmbAlive;
        public Enum TypeOfTeam { get; init; }

        public bool IsSmbAlive
        {
            get => isSmbAlive;
            set
            {
                isSmbAlive = value;
                if (value == false)
                {
                    if (TypeOfTeam.Equals(TypeOfTeams.heroes))
                    {
                        if (Members.Count > 1)
                        {
                            Console.WriteLine("Unfortunately our heroes were brave, yet not enough skilled, or just lack of luck.");
                        }
                        else
                        {
                            Console.WriteLine("Unfortunately our hero was brave, yet not enough skilled, or just lack of luck.");
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("Congratulations!");
                    }
                }
            }
        }
        
        public Team( List<BaseCharacter> members, Enum typeOfTeam)
        {
            TypeOfTeam = typeOfTeam;
            IsSmbAlive = true;
            Members = members;
            foreach (var member in Members)
            {
                member.AllTeam = this;
            }
        }

        public void AttackTeam(Team enemyTeam)
        {
            foreach (var member in Members)
            {
                if (member.IsAlive)
                {
                    var sortedEnemyTeam = enemyTeam.Members.OrderBy(member => member.HealthPoints).ToList();
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
                    if (!enemyTeam.Members.Exists(member => member.IsAlive))
                    {
                        enemyTeam.IsSmbAlive = false;
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
        
        public string Type { get; init; }

        public Team AllTeam { get; set; }
        public Weapon Weapon { get; init; }
        public MagicSkill MagicSkill { get; init; }
        
        public string Name { get; init; }
        public int Strenght { get; init; }
        public int Agility{ get; init; }

        public int Vitality { get; init; }
        public int Intelligence { get; init; }

        public int HealthPoints
        {
            get => healthPoints;
            set
            {
                healthPoints = value;
                if (healthPoints <= 0)
                {
                    healthPoints = 0;
                    IsAlive = false;
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
            set => armor = value;
        }
        
        public int MagicArmor
        {
            get => magicArmor;
            set => magicArmor = value;
        }

        private protected BaseCharacter(string name, int strength, int agility, int vitality, int intelligence, string type)
        {
            Name = name;
            Strenght = strength;
            Agility = agility;
            Vitality = vitality;
            Intelligence = intelligence;    
            HealthPoints = Vitality * 4;
            ManaPoints = Intelligence * 4;
            // Armor = (int)Math.Round(Agility / 2.0, MidpointRounding.AwayFromZero);
            // MagicArmor = (int)Math.Round(Intelligence / 2.0, MidpointRounding.AwayFromZero);
            Armor = (int)Math.Round(Agility / 2.0);
            MagicArmor = (int)Math.Round(Intelligence / 2.0);
            Type = type;
            IsAlive = true;
        }
        
        public static BaseCharacter CreateChar(string inputString)
        {
            string patternInfoCharacter = @"(\w+) (\d+) (\d+) (\d+) (\d+) (\D+)";
            var charaterinfo = Regex.Match(inputString, patternInfoCharacter).Groups;
            var type = charaterinfo[1].Value;
                
            
            BaseCharacter character =
                type == "knight" ? new Knight(charaterinfo[6].Value, int.Parse(charaterinfo[2].Value), int.Parse(charaterinfo[3].Value),
                                int.Parse(charaterinfo[4].Value), int.Parse(charaterinfo[5].Value)) :
                type == "thief" ? new Thief(charaterinfo[6].Value, int.Parse(charaterinfo[2].Value), int.Parse(charaterinfo[3].Value),
                    int.Parse(charaterinfo[4].Value), int.Parse(charaterinfo[5].Value)) :
                type == "mage" ? new Mage(charaterinfo[6].Value, int.Parse(charaterinfo[2].Value), int.Parse(charaterinfo[3].Value),
                    int.Parse(charaterinfo[4].Value), int.Parse(charaterinfo[5].Value)) : null;
            return character;
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
                    Console.WriteLine($"{Type} {Name} attacking {enemyMemeber.Type} {enemyMemeber.Name} with {MagicSkill.Name}.");
                    enemyMemeber.GetDamage(damage, typeOfAttack); 
                }
                return;
            }
            
            damage = Weapon.GetTotalDamage();
            typeOfAttack = TypeOfAttack.phisical;
            Console.WriteLine($"{Type} {Name} attacking {enemy.Type} {enemy.Name} with {Weapon.Name}.");
            enemy.GetDamage(damage, typeOfAttack);
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
            
            HealthPoints -= resultingDamage;
            Console.WriteLine($"{Type} {Name} get hit for {resultingDamage} hp and have {HealthPoints} hp left!");
            if (HealthPoints == 0)
            {
                Console.WriteLine($"{Type} {Name} is defeated!");
            }
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
            BonusDamage = Owner.Agility;
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

    

 
    


