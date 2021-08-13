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
                string json = string.Empty;

                using (StreamReader reader = new StreamReader("wwwroot/bot/orderBotConfig.json"))
                {
                    json = reader.ReadToEnd();
                }

                dynamic obj = JObject.Parse(json);

                OrderBotConfig.isActive = obj.isActive;
                OrderBotConfig.orderDelay = obj.orderDelay;
                OrderBotConfig.maxOrderNumber = obj.maxOrderNumber;
                OrderBotConfig.minItems = obj.minItems;
                OrderBotConfig.maxItems = obj.maxItems;
                OrderBotConfig.maxTargetQuantity = obj.maxTargetQuantity;
                OrderBotConfig.maxSequence = obj.maxSequence;

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
