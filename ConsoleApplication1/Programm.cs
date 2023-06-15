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
        Mage qq = new Mage("sss", 1, 2, 3, 40);
        Knight kn = new Knight("sss", 1, 2, 300, 4);

        List<BaseCharacter> allies = new List<BaseCharacter>(){qq};
        List<BaseCharacter> bandits = new List<BaseCharacter>(){kn};

        Team alliesTM = new Team(allies, TypeOfTeams.heroes);
        Team banditsTM = new Team(bandits, TypeOfTeams.bandits);

        Team TeamToAttack = alliesTM;
        Team TeamToDefend = banditsTM;

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
                    if (this.TypeOfTeam.Equals(TypeOfTeams.heroes))
                    {
                        Console.WriteLine("Unfortunately our hero was brave, yet not enough skilled, or just lack of luck.");
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
                var sortedEnemyTeam = enemyTeam.Members.OrderBy(member => member.HealthPoints).ToList();
                BaseCharacter aimForAttack = null;
                
                //find alive enemy
                for (int i = 0; i < sortedEnemyTeam.Count; i++)
                {
                    if (sortedEnemyTeam[i].IsAlive)
                    {
                        aimForAttack = sortedEnemyTeam[i];
                        break;
                    }
                }

                if (aimForAttack != null)
                {
                    member.Attack(aimForAttack);
                    if (!enemyTeam.Members.Exists(member => member.IsAlive))
                    {
                        enemyTeam.isSmbAlive = false;
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
                if (value > HealthPoints)
                {
                    healthPoints = 0;
                    IsAlive = false;
                }
                else
                {
                    healthPoints = value ;  
                }
            }
        }
        
        public bool IsAlive { get; private set; }
        
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
            init => magicArmor = value;
        }

        private protected BaseCharacter(string name, int strenght, int agility, int vitality, int intelligence)
        {
            Name = name;
            Strenght = strenght;
            Agility = agility;
            Vitality = vitality;
            Intelligence = intelligence;    
            HealthPoints = Vitality * 4;
            ManaPoints = Intelligence * 4;
            Armor = (int)Math.Round(Agility / 2.0, MidpointRounding.AwayFromZero);
            MagicArmor = (int)Math.Round(Intelligence / 2.0, MidpointRounding.AwayFromZero);
        }

        public void Attack(BaseCharacter enemy)
        {
            int damage;
            Enum typeOfAttack;
            if (MagicSkill != null && ManaPoints > MagicSkill.ManaCost)
            {
                damage = MagicSkill.UseSkill();
                typeOfAttack = TypeOfAttack.magical;
                Console.WriteLine(typeOfAttack);
                ManaPoints -= MagicSkill.ManaCost;
                
                foreach (var memeber in enemy.AllTeam.Members)
                {
                    memeber.GetDamage(damage, typeOfAttack); 
                }
                  
            }
            
            damage = Weapon.GetTotalDamage();
            typeOfAttack = TypeOfAttack.phisical;
            Console.WriteLine(typeOfAttack);
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
            healthPoints -= resultingDamage;
            return resultingDamage;
        }
    }

    class  Knight : BaseCharacter
    {
        public Knight(string name, int strenght, int agility, int vitality, int intelligence) :
               base(name, strenght, agility, vitality, intelligence)
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
            base(name, strenght, agility, vitality, intelligence)
        {
            Agility += 3;
            Weapon = new Dagger(this);
        }
    }
    
    class  Mage : BaseCharacter
    {
        
        public Mage(string name, int strenght, int agility, int vitality, int intelligence) :
            base(name, strenght, agility, vitality, intelligence)
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
        public int BaseDamage { get; init; }
        public int BonusDamage { get; init; }
        public int BonusSkillDamage { get; init; }
        public BaseCharacter Owner { get; set; }

        private protected Weapon(BaseCharacter owner)
        {
            Owner = owner;
        }

        public int GetTotalDamage()
        {
            return BaseDamage + BonusDamage;
        }
    }

    class Sword:Weapon
    {
        public Sword(BaseCharacter owner):base(owner)
        {
            BaseDamage = 5;
            BonusDamage = Owner.Strenght;
        }
    }
    
    class Dagger:Weapon
    {
        public Dagger(BaseCharacter owner):base(owner)
        {
            BaseDamage = 4;
            BonusDamage = Owner.Agility;
        }
    }
    class Staff:Weapon
    {
        public Staff(BaseCharacter owner):base(owner)
        {
            BaseDamage = 15;
            BonusDamage = Owner.Agility;
            BonusSkillDamage = 10;
        }
    }

    class MagicSkill
    {
        public int ManaCost { get; private set; }
        public int MagicDamage { get; private protected set; }
        public BaseCharacter Owner { get; set; }
        private protected MagicSkill(int manaCost)
        {
            ManaCost = manaCost;
        }

        public int UseSkill()
        {
            return MagicDamage;
        }
    }

    class ChainLightning:MagicSkill
    {
        public ChainLightning(BaseCharacter skillUser) : base(40)
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
}

    

 
    


