using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;

namespace Parser
{
    class Goods
    {
        public uint CategoryID;
        public Dictionary<string, uint> goods = new Dictionary<string, uint>();

        public Goods(uint categoryID)
        {
            CategoryID = categoryID;
        }
    }

    class Cart
    {
        public uint UserIP;
        public List<CartGoods> CartItems = new List<CartGoods>();
        public bool Paid;
    }

    class CartGoods
    {
        public uint CartGoodsID;
        public uint GoodsID;
        public uint Amount;

        public CartGoods(uint cartGoodsID, uint goodsID, uint amount)
        {
            CartGoodsID = cartGoodsID;
            GoodsID = goodsID;
            Amount = amount;
        }
    }

    enum Actions { Root, Category, Product, Cart, Pay, Success };

    class Action
    {
        public uint UserIP;
        public Actions ActionType;
        public string ActionReference;
        public string DateTime;
    }

    static class Constants
    {
        public const int ActionCapacity = 30000;
        public const int partDate = 2;
        public const int partTime = 3;
        public const int partIP = 6;
        public const int partURL = 7;
        public const int CategoryName = 1;
        public const int ProductName = 2;
        public const int cartGoodsID = 1;
        public const int cartAmount = 2;
        public const int cartCartID = 3;
        public const int payUserID = 1;
        public const int payCartID = 2;
        public const int successCartID = 1;
    }

    class Program
    {
        static public uint IpToUint(string ip)
        {
            byte[] bytes = IPAddress.Parse(ip).GetAddressBytes();
            Array.Reverse(bytes); // flip big-endian(network order) to little-endian
            return BitConverter.ToUInt32(bytes, 0);
        }

        static void Main(string[] args)
        {
            // string - Category Name
            Dictionary<string, Goods> categoriesAndGoods = new Dictionary<string, Goods>();
            // int - CartID
            Dictionary<int, Cart> cartAndItems = new Dictionary<int, Cart>();
            // string - UserIP, uint - UserID
            Dictionary<UInt32, long> users = new Dictionary<UInt32, long>();
            List<Action> actions = new List<Action>(Constants.ActionCapacity);

            const string patternCategoryAndGoods = "/([a-z_]+)/([a-z_]*)";
            const string patternCart = @"cart\?goods_id=(\d+)&amount=(\d+)&cart_id=(\d+)";
            const string patternPay = @"pay\?user_id=(\d+)&cart_id=(\d+)";
            const string patternSuccessPay = @"success_pay_(\d+)";

            using (StreamReader sr = new StreamReader(args[0]))
            {
                string line;

                int actionCount = 0;
                uint cartItemID = 1;
                uint categoryID = 1;
                // temporary ID
                uint userID = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                    uint IPnumber = IpToUint(parts[Constants.partIP]);
                    if (!users.ContainsKey(IPnumber))
                    {
                        users.Add(IPnumber, userID);
                        userID++;
                    }
                    actions.Add(new Action()
                    {
                        DateTime = parts[Constants.partDate] + ' ' + parts[Constants.partTime],
                        UserIP = IPnumber
                    });

                    // Action -> Category or Product
                    Match m = Regex.Match(parts[Constants.partURL], patternCategoryAndGoods);
                    if (m.Success)
                    {
                        string category = m.Groups[Constants.CategoryName].Value;
                        if (!categoriesAndGoods.ContainsKey(category))
                        {
                            categoriesAndGoods.Add(category, new Goods(categoryID++));
                        }
                        string product = m.Groups[Constants.ProductName].Value;
                        // if the product page is selected, then action type is product, else category
                        if (product != string.Empty)
                        {
                            if(!categoriesAndGoods[category].goods.ContainsKey(product))
                            {
                                categoriesAndGoods[category].goods.Add(product, 0);
                            }

                            actions[actionCount].ActionType = Actions.Product;
                            actions[actionCount].ActionReference = category+' '+product;
                        }
                        else
                        {
                            actions[actionCount].ActionType = Actions.Category;
                            actions[actionCount].ActionReference = categoriesAndGoods[category].CategoryID.ToString();
                        }
                    }
                    // Action -> Cart
                    else if ((m = Regex.Match(parts[Constants.partURL], patternCart)).Success)
                    {
                        int cartID = Int32.Parse(m.Groups[Constants.cartCartID].Value);
                        if (!cartAndItems.ContainsKey(cartID))
                            cartAndItems.Add(cartID, new Cart());
                        cartAndItems[cartID].CartItems.Add(new CartGoods(cartItemID,
                                                                    UInt32.Parse(m.Groups[Constants.cartGoodsID].Value),
                                                                    UInt32.Parse(m.Groups[Constants.cartAmount].Value))
                            );
                        cartAndItems[cartID].UserIP = IPnumber;

                        // finding selected product and set it's id
                        for (int i = actionCount - 1; ; i--)
                            if (actions[i].UserIP == IPnumber)
                            {
                                string[] temp = actions[i].ActionReference.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                                categoriesAndGoods[temp[0]].goods[temp[1]] = UInt32.Parse(m.Groups[Constants.cartGoodsID].Value);
                                break;
                            }

                        actions[actionCount].ActionType = Actions.Cart;
                        actions[actionCount].ActionReference = cartItemID.ToString();
                        cartItemID++;
                    }
                    // Action -> Pay
                    else if ((m = Regex.Match(parts[Constants.partURL], patternPay)).Success)
                    {
                        users[IPnumber] = Int64.Parse(m.Groups[Constants.payUserID].Value);

                        actions[actionCount].ActionType = Actions.Pay;
                        actions[actionCount].ActionReference = m.Groups[Constants.payCartID].Value;
                    }
                    // Action -> Success Pay
                    else if ((m = Regex.Match(parts[Constants.partURL], patternSuccessPay)).Success)
                    {
                        string successCartID = m.Groups[Constants.successCartID].Value;
                        cartAndItems[Int32.Parse(successCartID)].Paid = true;

                        actions[actionCount].ActionType = Actions.Success;
                        actions[actionCount].ActionReference = successCartID;
                    }
                    // Action -> Root
                    else
                    {
                        actions[actionCount].ActionType = Actions.Root;
                        actions[actionCount].ActionReference = "\\N";
                    }

                    actionCount++;
                }
            }

            string curDir = Directory.GetCurrentDirectory() + @"\logs\";
            if(!Directory.Exists(curDir))
            {
                Directory.CreateDirectory(curDir);
            }
            Directory.SetCurrentDirectory(curDir);

            // write UserID, User IP
            using (StreamWriter swUser = new StreamWriter("user.txt", false))
                foreach (KeyValuePair<uint, long> user in users)
                {
                    swUser.WriteLine("{0},{1}", user.Value, user.Key);
                }
            // write UserID, ActionType, ActionReference, DateTime
            using (StreamWriter swActions = new StreamWriter("actions.txt", false))
                foreach (Action action in actions)
                {
                    if (action.ActionType == Actions.Product)
                    {
                        string[] temp = action.ActionReference.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        action.ActionReference = categoriesAndGoods[temp[0]].goods[temp[1]].ToString();
                    }
                    swActions.WriteLine("{0},{1},{2},{3}", users[action.UserIP], (int)action.ActionType, action.ActionReference, action.DateTime);
                }
            // write CategoryID, Category Name
            using (StreamWriter swCategory = new StreamWriter("category.txt", false))
            using (StreamWriter swGoods = new StreamWriter("goods.txt", false))
                foreach (KeyValuePair<string, Goods> category in categoriesAndGoods)
                {
                    swCategory.WriteLine("{0},{1}", category.Value.CategoryID, category.Key);
                    foreach (KeyValuePair<string, uint> product in category.Value.goods)
                        swGoods.WriteLine("{0},{1},{2}", product.Value, category.Value.CategoryID, product.Key);
                }
            // write CartID, UserID, Paid into cart
            // write CartGoodsID, CartID, GoodsID, Amount into cartGoods
            using (StreamWriter swCart = new StreamWriter("cart.txt", false))
            using (StreamWriter swItems = new StreamWriter("cartGoods.txt", false))
                foreach (KeyValuePair<int, Cart> cart in cartAndItems)
                {
                    swCart.WriteLine("{0},{1},{2}", cart.Key, users[cart.Value.UserIP], cart.Value.Paid ? 1 : 0);
                    foreach (CartGoods item in cart.Value.CartItems)
                    {
                        swItems.WriteLine("{0},{1},{2},{3}", item.CartGoodsID, cart.Key, item.GoodsID, item.Amount);
                    }
                }

            string text = File.ReadAllText(args[1] + "load_db_sample.sql");
            curDir = Regex.Replace(curDir, @"\\", @"\\");
            text = text.Replace("$$PATH$$", "'" + curDir);
            File.WriteAllText(args[1]+"load_db.sql", text);
        }
    }
}
