﻿using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models.InlineButtons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using tg.Models;


namespace tg.UsersCache
{
    public class CacheCommand
    {
        [ReplyMenuHandler("Show Countdown")]
        public static async Task CheckCache(ITelegramBotClient botClient, Update update)
        {
            var cache = update.GetCacheData<UserCache>();

            string message;
            if (cache.scheduleDict.Count > 0)
            {
                message = "Users in the cache:";
                foreach (var user in cache.scheduleDict)
                {
                    int daysUntilBirthday = GetDaysUntilBirthday(user.Value);
                    message += $"\n- Name: {user.Key}, Date: {user.Value.ToString("dd.MM.yyyy")}, Days until birthday: {daysUntilBirthday}";
                }
            }
            else
            {
                message = "No users in the cache.";
            }

            Message _ = await PRTelegramBot.Helpers.Message.Send(botClient, update, message);
        }

        [ReplyMenuHandler("clearcache")]
        [InlineCallbackHandler<EditCountdownTHeader>(EditCountdownTHeader.AllDel)]
        public static async Task ClearCache(ITelegramBotClient botClient, Update update)
        {
            string message = "Cache cleared!";
            update.GetCacheData<UserCache>().ClearData();
            Message _ = await PRTelegramBot.Helpers.Message.Send(botClient, update, message);
        }

        public static async Task UpdateCache(Update update, string name, DateTime date)
        {
            var cache = update.GetCacheData<UserCache>();

            if (!cache.scheduleDict.ContainsKey(name))
            {
                cache.scheduleDict.Add(name, date);
            }
            else
            {
                cache.scheduleDict[name] = date;
            }
        }

        private static int GetDaysUntilBirthday(DateTime birthday)
        {
            DateTime currentDate = DateTime.Today;
            DateTime birthdayThisYear = new DateTime(currentDate.Year, birthday.Month, birthday.Day);

            if (birthdayThisYear < currentDate)
            {
                birthdayThisYear = birthdayThisYear.AddYears(1);
            }

            TimeSpan difference = birthdayThisYear - currentDate;
            int daysUntilBirthday = (int)difference.TotalDays;

            return daysUntilBirthday;
        }
    }

}