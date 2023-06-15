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
        while (Console.ReadLine() != "end")
        {
            Console.WriteLine(qq.Attack(kn));  
        };
        

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

        public int Attack(BaseCharacter enemy)
        {
            int damage;
            Enum typeOfAttack;
            if (MagicSkill != null && ManaPoints > MagicSkill.ManaCost)
            {
                damage = MagicSkill.UseSkill();
                typeOfAttack = TypeOfAttack.magical;
                Console.WriteLine(typeOfAttack);
                ManaPoints -= MagicSkill.ManaCost;
                return enemy.GetDamage(damage, typeOfAttack);  
            }
            
            damage = Weapon.GetTotalDamage();
            typeOfAttack = TypeOfAttack.phisical;
            Console.WriteLine(typeOfAttack);
            return enemy.GetDamage(damage, typeOfAttack);
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

        // public override int Attack(BaseCharacter enemy)
        // {
            // if (this.ManaPoints > this.MagicSkill.ManaCost)
            // {
            //     if (enemy.IsAlive)
            //     {
            //         int damage = this.MagicSkill.UseSkill();
            //         Enum typeOfAttack = TypeOfAttack.magical;
            //         Console.WriteLine(typeOfAttack);
            //         this.ManaPoints -= this.MagicSkill.ManaCost;
            //         return enemy.GetDamage(damage, typeOfAttack);  
            //     }
            //     else
            //     {
            //         Console.WriteLine("Enemy is already dead");
            //     }
            //     
            // }
            // else
            // {
            //     return base.Attack(enemy);
            // }
        // }
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
}

    

 
    


