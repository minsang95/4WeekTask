using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace _4weekTask
{
    internal class Program
    {
        interface Status
        {
            string Name { get; }
            int Level { get; set; }
            string Class { get; }
            int Atk { get; set; }
            int Def { get; set; }
            int Health { get; set; }
            int Gold { get; set; }
            int Exp { get; set; }
            bool IsDead => Health <= 0;
            Weapon Weapon { get; set; }
            Armor Armor { get; set; }
            Dungeon Dungeon { get; set; }
        }
        class Warrior : Status
        {
            public string Name { get; }
            public int Level { get; set; }
            public string Class { get; }
            public int Atk { get; set; }
            public int Def { get; set; }
            public int Health { get; set; }
            public int Gold { get; set; }
            public int Exp { get; set; }
            public bool IsDead => Health <= 0;
            public Weapon Weapon { get; set; }
            public Armor Armor { get; set; }
            public Dungeon Dungeon { get; set; }
            public Warrior(string name)
            {
                Name = name;
                Level = 1;
                Class = "전사";
                Atk = 10;
                Def = 5;
                Health = 100;
                Gold = 1500;
                Exp = 0;
                Weapon = new Weapon("맨손", 0, 0, 0, "아무것도 들고있지 않습니다.");
                Armor = new Armor("맨손", 0, 0, 0, "아무것도 들고있지 않습니다.");
            }
            public void weaponEquip()
            {
                Atk += Weapon.Atk;
                Def += Weapon.Def;
            }
            public void armorEquip()
            {
                Atk += Armor.Atk;
                Def += Armor.Def;
            }
            public void weaponUnequip()
            {
                Atk -= Weapon.Atk;
                Def -= Weapon.Def;
                Weapon = new Weapon("맨손", 0, 0, 0, "아무것도 들고있지 않습니다.");
            }
            public void armorUnequip()
            {
                Atk -= Armor.Atk;
                Def -= Armor.Def;
                Armor = new Armor("맨손", 0, 0, 0, "아무것도 들고있지 않습니다.");
            }
            public void clearDungeon()
            {
                int before = Gold;
                Health -= (int)(Dungeon.Damage - (Def - Dungeon.RecommendDef));
                Random gold = new Random();
                Gold += gold.Next((int)(Dungeon.Gold + (Dungeon.Gold * Atk * 0.01f)), (int)(Dungeon.Gold + (Dungeon.Gold * Atk * 0.02f)));
                Exp += Dungeon.Exp;
                Console.WriteLine($"몬스터를 소탕하고, 골드 상자를 발견했습니다!.\n현재 체력 : {Health}\n획득 골드 : {Gold - before}\n보유 골드 : {Gold}");
                expCheck();
                Console.ReadLine();
            }
            public void escapeDmg()
            {
                Health = (int)(Health * 0.5f);
                Console.WriteLine($"몬스터를 소탕하지 못하고, 던전을 탈출했습니다.\n현재 체력 : {Health}");
                Console.ReadLine();
            }
            public void expCheck()
            {
                Console.WriteLine($"{Dungeon.Exp} 의 경험치를 획득했습니다.");
                if (Exp == Level)
                {
                    ++Level;
                    Exp = 0;
                    Def += 1;
                    if (Level % 2 == 0)
                    {
                        Atk += 1;
                    }
                    Console.WriteLine($"레벨업!\n현재 레벨 : {Level}");
                }
            }
        }
        interface Item
        {
            public string Name { get; set; }
            public int Atk { get; }
            public int Def { get; }
            public int Price { get; }
            public string Info { get; }
            public bool Equipped { get; set; }
            public bool SoldOut { get; set; }

            public void equipped()
            {
                Equipped = true;
                Name = Name.Insert(0, "[E]");
            }
            public void unEquip()
            {
                Equipped = false;
                Name = Name.Replace("[E]", null);
            }
        }
        struct Weapon : Item
        {
            public string Name { get; set; }
            public int Atk { get; }
            public int Def { get; }
            public int Price { get; }
            public string Info { get; }
            public bool Equipped { get; set; }
            public bool SoldOut { get; set; }
            public Weapon(string name, int atk, int def, int price, string info)
            {
                Name = name;
                Atk = atk;
                Def = def;
                Price = price;
                Info = info;
                Equipped = false;
                SoldOut = false;
            }
        }
        struct Armor : Item
        {
            public string Name { get; set; }
            public int Atk { get; }
            public int Def { get; }
            public int Price { get; }
            public string Info { get; }
            public bool Equipped { get; set; }
            public bool SoldOut { get; set; }
            public Armor(string name, int atk, int def, int price, string info)
            {
                Name = $"{name}";
                Atk = atk;
                Def = def;
                Price = price;
                Info = info;
                Equipped = false;
                SoldOut = false;
            }
        }

        struct Dungeon
        {
            public int RecommendDef { get; set; }
            public int Damage { get; set; }
            public int Gold { get; set; }
            public int Exp { get; set; }

            public Dungeon(int dmg, int def, int gold, int exp)
            {
                Random damage = new Random();
                RecommendDef = def;
                Damage = damage.Next(dmg - 15, dmg + 1);
                Gold = gold;
                Exp = exp;
            }
        }

        public class Stage
        {
            public int choice0_startScene() // 0. 시작 화면
            {
                Console.Clear();
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
                Console.WriteLine("1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전입장\n5. 휴식하기\n6. 저장, 불러오기\n7. 종료\n\n원하시는 행동의 번호를 입력해주세요.");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice <= 0 || choice > 7)
                {
                    Console.WriteLine("잘못된 입력입니다.\n1. 상태 보기 2. 인벤토리 3. 상점 4. 던전입장 5. 휴식하기 6. 저장, 불러오기 7. 종료");
                    input = Console.ReadLine();
                }
                return choice;
            }

            public void choice1_status() // 1.상태 보기
            {
                Console.Clear();
                Console.WriteLine($"캐릭터의 정보가 표시됩니다.\n\n" +
                                  $"Lv. {player.Level}\nClass {player.Class}\n공격력 : {player.Atk}\n" +
                                  $"방어력 : {player.Def}\n체 력 : {player.Health}\nGold : {player.Gold} G\n\n" +
                                  $"장착 무기 : {player.Weapon.Name} | 공격력 +{player.Weapon.Atk} 방어력 +{player.Weapon.Def} | {player.Weapon.Info}\n" +
                                  $"장착 방어구 : {player.Armor.Name} | 공격력 +{player.Armor.Atk} 방어력 +{player.Armor.Def} | {player.Armor.Info}\n" +
                                  $"\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || !(choice == 0))
                {
                    Console.WriteLine("0. 나가기");
                    input = Console.ReadLine();
                }
            }

            public void choice2_inventory() // 2.인벤토리
            {
                Console.Clear();
                Console.WriteLine($"보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]\n");
                foreach (Item items in inventory)
                {
                    Console.WriteLine($"- {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info}");
                }
                Console.WriteLine("\n1. 장착 관리\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < 0 || choice > 1)
                {
                    Console.WriteLine("\n1. 장착 관리\n0. 나가기");
                    input = Console.ReadLine();
                }
                if (choice == 1)
                {
                    choice2_1_itemEquip();
                }
            }

            public void choice2_1_itemEquip() // 2-1.아이템 장착관리
            {
                Console.Clear();
                Console.WriteLine($"보유 중인 아이템을 관리할 수 있습니다.\n장착 또는 장착 해제하실 아이템 번호를 입력해주세요.\n\n[아이템 목록]\n");
                int number = 1;
                foreach (Item items in inventory)
                {
                    Console.WriteLine($"- {number} {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info}");
                    number++;
                }
                Console.WriteLine("\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice > inventory.Count || choice < 0)
                {
                    Console.WriteLine("잘못된 입력입니다.\n장착 또는 장착 해제하실 아이템 번호를 입력해주세요.\n0. 나가기");
                    input = Console.ReadLine();
                }
                if (!(choice == 0))
                {
                    if (!(inventory[choice - 1].Equipped))
                    {
                        if (inventory[choice - 1] is Weapon)
                        {
                            for (int i = 0; i < inventory.Count; i++)
                            {
                                if (inventory[i] is Weapon)
                                    inventory[i].unEquip();
                            }
                            player.weaponUnequip();
                            player.Weapon = (Weapon)inventory[choice - 1];
                            player.weaponEquip();
                            inventory[choice - 1].equipped();
                        }
                        else
                        {
                            for (int i = 0; i < inventory.Count; i++)
                            {
                                if (inventory[i] is Armor)
                                    inventory[i].unEquip();
                            }
                            player.armorUnequip();
                            player.Armor = (Armor)inventory[choice - 1];
                            player.armorEquip();
                            inventory[choice - 1].equipped();
                        }
                    }
                    else
                    {
                        if (inventory[choice - 1] is Weapon)
                        {
                            inventory[choice - 1].unEquip();
                            player.weaponUnequip();
                            player.Weapon = emptyWeapon;
                        }
                        else
                        {
                            inventory[choice - 1].unEquip();
                            player.armorUnequip();
                            player.Armor = emptyArmor;
                        }
                    }
                    choice2_1_itemEquip();
                }
            }

            public void choice3_itemshop() // 3.상점
            {
                Console.Clear();
                Console.WriteLine($"필요한 아이템을 얻을 수 있는 상점입니다.\n\n[보유골드]\n{player.Gold} G\n\n[아이템 목록]\n");
                foreach (Item items in itemshop)
                {
                    if (!(items.SoldOut))
                    {
                        Console.WriteLine($"- {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info} | {items.Price} G");
                    }
                    else
                    {
                        Console.WriteLine($"- {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info} | 구매완료");
                    }
                }
                Console.WriteLine("\n1. 아이템 구매\n2. 아이템 판매\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < 0 || choice > 2)
                {
                    Console.WriteLine("\n1. 아이템 구매\n2. 아이템 판매\n0. 나가기");
                    input = Console.ReadLine();
                }
                if (choice == 1)
                {
                    choice3_1_itembuy();
                }
                if (choice == 2)
                {
                    choice3_2_itemsell();
                }
            }

            public void choice3_1_itembuy() // 3-1.아이템 구매
            {
                Console.Clear();
                Console.WriteLine($"필요한 아이템을 얻을 수 있는 상점입니다.\n구매하실 아이템 번호를 입력해 주세요.\n\n[보유골드]\n{player.Gold} G\n\n[아이템 목록]\n");
                int number = 1;
                foreach (Item items in itemshop)
                {
                    if (!(items.SoldOut))
                    {
                        Console.WriteLine($"- {number} {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info} | {items.Price} G");
                    }
                    else
                    {
                        Console.WriteLine($"- {number} {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info} | 구매완료");
                    }
                    number++;
                }
                Console.WriteLine("\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice > itemshop.Count || choice < 0)
                {
                    Console.WriteLine("잘못된 입력입니다.\n구매하실 아이템 번호를 입력해 주세요.\n0. 나가기");
                    input = Console.ReadLine();
                }
                if (!(choice == 0))
                {
                    if (!(itemshop[choice - 1].SoldOut))
                    {
                        if (player.Gold >= itemshop[choice - 1].Price)
                        {
                            Console.WriteLine("구매를 완료했습니다.");
                            player.Gold -= itemshop[choice - 1].Price;
                            itemshop[choice - 1].SoldOut = true;
                            inventory.Add(itemshop[choice - 1]);
                            Console.ReadLine();
                            choice3_1_itembuy();
                        }
                        else
                        {
                            Console.WriteLine("Gold 가 부족합니다.");
                            Console.ReadLine();
                            choice3_1_itembuy();
                        }
                    }
                    else
                    {
                        Console.WriteLine("이미 구매한 아이템입니다.");
                        Console.ReadLine();
                        choice3_1_itembuy();
                    }
                }
            }
            public void choice3_2_itemsell() // 3-2.아이템 판매
            {
                Console.Clear();
                Console.WriteLine($"판매를 원하시는 아이템의 번호를 입력해주세요.\n\n[보유 골드]\n{player.Gold} G\n\n[아이템 목록]\n");
                int number = 1;
                foreach (Item items in inventory)
                {
                    Console.WriteLine($"- {number} {items.Name} | 공격력 +{items.Atk} 방어력 +{items.Def} | {items.Info} | {items.Price * 0.85f} G");
                    number++;
                }
                Console.WriteLine("0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice > inventory.Count || choice < 0)
                {
                    Console.WriteLine("잘못된 입력입니다.\n판매하실 아이템 번호를 입력해 주세요.\n0. 나가기");
                    input = Console.ReadLine();
                }
                if (!(choice == 0))
                {
                    if (!(inventory[choice - 1].Equipped))
                    {
                        Console.WriteLine("판매를 완료했습니다.");
                        player.Gold += (int)(inventory[choice - 1].Price * 0.85f);
                        for (int i = 0; i < itemshop.Count; i++)
                        {
                            if (inventory[choice - 1].Name == itemshop[i].Name)
                            {
                                itemshop[i].SoldOut = false;
                            }
                        }
                        inventory.RemoveAt(choice - 1);
                        Console.ReadLine();
                        choice3_2_itemsell();
                    }
                    else
                    {
                        Console.WriteLine("장착중인 아이템은 판매할수 없습니다.");
                        Console.ReadLine();
                        choice3_2_itemsell();
                    }
                }
            }
            public void enterDungeon() // 4. 던전입장
            {
                Console.Clear();
                Console.WriteLine("권장 능력치를 확인 후, 입장하실 던전의 번호를 입력해 주세요.\n\n" +
                    "1. 쉬운 던전 | 방어력 5 이상 권장\n2. 일반 던전 | 방어력 11 이상 권장\n3. 어려운 던전 | 방어력 17 이상 권장\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < 0 || choice > 3)
                {
                    Console.WriteLine("올바른 번호를 입력해 주세요.");
                    input = Console.ReadLine();
                }
                if (choice == 1) // 1번 던전
                {
                    Dungeon easy = new Dungeon(35, 5, 1000, 1); // 던전초기화 및 생성
                    player.Dungeon = easy;
                    if (player.Def < easy.RecommendDef)
                    {
                        Random diceEyes = new Random();
                        int dice = diceEyes.Next(0, 5);
                        if (dice >= 3)
                        {
                            player.clearDungeon();
                        }
                        else
                        {
                            player.escapeDmg();
                        }
                    }
                    else
                    {
                        player.clearDungeon();
                    }
                }
                if (choice == 2) // 2번 던전
                {
                    Dungeon nomal = new Dungeon(35, 11, 1700, 1);
                    player.Dungeon = nomal;
                    if (player.Def < nomal.RecommendDef)
                    {
                        Random diceEyes = new Random();
                        int dice = diceEyes.Next(0, 5);
                        if (dice >= 3)
                        {
                            player.clearDungeon();
                        }
                        else
                        {
                            player.escapeDmg();
                        }
                    }
                    else
                    {
                        player.clearDungeon();
                    }
                }
                if (choice == 3) // 3번 던전
                {
                    Dungeon hard = new Dungeon(35, 11, 1700, 1);
                    player.Dungeon = hard;
                    if (player.Def < hard.RecommendDef)
                    {
                        Random diceEyes = new Random();
                        int dice = diceEyes.Next(0, 5);
                        if (dice >= 3)
                        {
                            player.clearDungeon();
                        }
                        else
                        {
                            player.escapeDmg();
                        }
                    }
                    else
                    {
                        player.clearDungeon();
                    }
                }
            }

            public void rest() // 5.휴식하기
            {
                Console.Clear();
                Console.WriteLine($"500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.Gold} G)\n\n1. 휴식하기\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < 0 || choice > 1)
                {
                    Console.WriteLine("1. 휴식하기 0. 나가기");
                    input = Console.ReadLine();
                }
                if (choice == 1)
                {
                    if (player.Gold >= 500)
                    {
                        player.Gold -= 500;
                        player.Health = 100;
                        Console.WriteLine($"휴식을 완료했습니다. (보유 골드 : {player.Gold} G)");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Gold 가 부족합니다.");
                        Console.ReadLine();
                    }
                }
            }

            public void SaveOrLoad() // 6.저장 , 불러오기
            {
                Console.Clear();
                Console.WriteLine("게임을 저장하거나, 불러올수 있습니다.\n\n저장은 하나의 데이터로 덮어씌워집니다.\n\n1. 저장하기\n2. 불러오기\n0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < 0 || choice > 2)
                {
                    Console.WriteLine("1. 저장하기 2. 불러오기 0. 나가기");
                    input = Console.ReadLine();
                }
                if (choice == 1)
                {
                    save();
                    Console.WriteLine("저장이 완료되었습니다.");
                    Console.ReadLine();
                }
                if (choice == 2)
                {
                    load();
                    Console.WriteLine("불러오기가 완료되었습니다.");
                    Console.ReadLine();
                }
            }
            string filePath = "C:\\VisualStudioProject\\4weekTask\\4weekTask"; // 저장,불러오기 파일 경로
            public void save()
            {
                using (StreamWriter sw = new StreamWriter(filePath + "\\PlayerData.json"))
                {
                    sw.Write(JsonConvert.SerializeObject(player));
                }
                using (StreamWriter sw = new StreamWriter(filePath + "\\InventoryData.json"))
                {
                    sw.Write(JsonConvert.SerializeObject(inventory));
                }
                using (StreamWriter sw = new StreamWriter(filePath + "\\ShopData.json"))
                {
                    sw.Write(JsonConvert.SerializeObject(itemshop));
                }
            }
            public void load()
            {
                using (StreamReader sr = new StreamReader(filePath + "\\PlayerData.json"))
                {
                    player = JsonConvert.DeserializeObject<Warrior>(sr.ReadToEnd());
                }
                using (StreamReader sr = new StreamReader(filePath + "\\InventoryData.json"))
                {
                    inventory = JsonConvert.DeserializeObject<List<Item>>(sr.ReadToEnd());
                }
                using (StreamReader sr = new StreamReader(filePath + "\\ShopData.json"))
                {
                    itemshop = JsonConvert.DeserializeObject<List<Item>>(sr.ReadToEnd());
                }
            }

            public void end()
            {
                Console.Clear();
                Console.WriteLine("게임을 포기하시겠습니까?\n\n1. 종료 0. 나가기");
                string input = Console.ReadLine();
                int choice;
                while (!(int.TryParse(input, out choice)) || choice < 0 || choice > 1)
                {
                    Console.WriteLine("1. 종료 0. 나가기");
                    input = Console.ReadLine();
                }
                if (choice == 1)
                {
                    player.Health = 0;
                }
            }

            // 플레이어 , 아이템 생성
            Warrior player = new Warrior("Jack");
            List<Item> inventory = new List<Item>();
            List<Item> itemshop = new List<Item>();
            Weapon weapon1 = new Weapon("낡은 검", 2, 0, 600, "쉽게 볼 수 있는 낡은 검 입니다.");
            Weapon weapon2 = new Weapon("청동 도끼", 5, 0, 1500, "어디선가 사용됐던거 같은 도끼입니다.");
            Weapon weapon3 = new Weapon("스파르타의 창", 7, 0, 2100, "스파르타의 전사들이 사용했다는 전설의 창입니다.");
            Armor armor1 = new Armor("수련자 갑옷", 0, 5, 1000, "수련에 도움을 주는 갑옷입니다.");
            Armor armor2 = new Armor("무쇠갑옷", 0, 9, 2500, "무쇠로 만들어져 튼튼한 갑옷입니다.");
            Armor armor3 = new Armor("스파르타의 갑옷", 0, 15, 3500, "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.");
            Weapon emptyWeapon = new Weapon("맨손", 0, 0, 0, "아무것도 들고있지 않습니다.");
            Armor emptyArmor = new Armor("맨손", 0, 0, 0, "아무것도 들고있지 않습니다.");

            public void Playgame() // 게임시작
            {
                itemshop.Add(weapon1);
                itemshop.Add(weapon2);
                itemshop.Add(weapon3);
                itemshop.Add(armor1);
                itemshop.Add(armor2);
                itemshop.Add(armor3);
                while (!(player.IsDead))
                {
                    int choice = choice0_startScene();

                    if (choice == 1) // 1.상태 보기
                    {
                        choice1_status();
                    }
                    if (choice == 2) // 2.인벤토리
                    {
                        choice2_inventory();
                    }
                    if (choice == 3) // 3.상점
                    {
                        choice3_itemshop();
                    }
                    if (choice == 4) // 4.던전입장
                    {
                        enterDungeon();
                    }
                    if (choice == 5) // 5.휴식하기
                    {
                        rest();
                    }
                    if (choice == 6) // 6.저장,불러오기
                    {
                        SaveOrLoad();
                    }
                    if (choice == 7) // 7.게임종료
                    {
                        end();
                    }
                }
                Console.WriteLine("당신은 죽었습니다. 게임을 종료합니다.");
            }

            static void Main(string[] args)
            {
                Stage stage = new Stage();
                stage.Playgame();

            }

        }
    }
}
