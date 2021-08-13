using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace ERPSystem.Helpers
{
    public static class OrderBotConfig
    {
        public static bool isActive = false;

        public static int orderDelay = 300000; // in ms 
        public static int maxOrderNumber = 20;

        public static int minItems = 1; // Minimum amount of items per order
        public static int maxItems = 5; // Maximuim amount of items per order

        public static int maxTargetQuantity = 5;

        public static int maxSequence = 150;

        public static bool Refresh()
        {
            try
            {
                //                string json = string.Empty;

                //                string path = "C:\\inetpub\\wwwroot\\ERPSystem\\wwwroot\\bot\\orderBotConfig.json";
                //#if DEBUG
                //                path = "wwwroot/bot/orderBotConfig.json";
                //#endif

                //                using (StreamReader reader = new StreamReader(path))
                //                {
                //                    json = reader.ReadToEnd();
                //                }

                //                dynamic obj = JObject.Parse(json);

                //                OrderBotConfig.isActive = obj.isActive;
                //                OrderBotConfig.orderDelay = obj.orderDelay;
                //                OrderBotConfig.maxOrderNumber = obj.maxOrderNumber;
                //                OrderBotConfig.minItems = obj.minItems;
                //                OrderBotConfig.maxItems = obj.maxItems;
                //                OrderBotConfig.maxTargetQuantity = obj.maxTargetQuantity;
                //                OrderBotConfig.maxSequence = obj.maxSequence;

                OrderBotConfig.isActive = true;
#if DEBUG
                OrderBotConfig.isActive = false;
#endif
                OrderBotConfig.orderDelay = 300000; 
                OrderBotConfig.maxOrderNumber = 500000;
                OrderBotConfig.minItems = 1;
                OrderBotConfig.maxItems = 3;
                OrderBotConfig.maxTargetQuantity = 5;
                OrderBotConfig.maxSequence = 150;

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
