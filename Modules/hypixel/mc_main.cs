using Hypixel.NET;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Chino_bot.Models;
using Discord;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Globalization;
using System.Threading;

namespace Chino_bot.Modules.hypixel
{
    public class mc_main : ModuleBase<SocketCommandContext>
    {
        Emote bangbang = Emote.Parse("<:bangbang:719351955457179730>");
        Emote cross = Emote.Parse("<:x:722772913564024933>");
        Emote questionMark = Emote.Parse("<:question:719528107874451476>");
        static string Key = "55b0c166-cec2-4b79-86cb-37fe62bb50c5";
        HypixelApi API = new HypixelApi(Key, 60);
        EmbedBuilder embed = new EmbedBuilder();                                                               
        

        [Command("auctions")]
        [Alias("a")]
        [Summary("Gets a player's auctions")]
        public async Task Auctions(string username = null)
        {
            if (username != null)
            {
                try
                {
                    //variables
                    var Stats = await API.GetAuctionsByPlayerNameAsync(username);
                    string auctions = "";


                    if (Stats.WasSuccessful)
                    {
                        if (Stats.Auctions.Count > 0)
                        {
                            embed.WithColor(0, 130, 0);

                            for (int i = 0; i < Stats.Auctions.Count(); i++)
                            {
                                TimeSpan ts = Stats.Auctions[i].End.Subtract(DateTime.Now);
                                if (Stats.Auctions[i].End < DateTime.Now)
                                {
                                    auctions += $"**{Stats.Auctions[i].ItemName}:** {Stats.Auctions[i].HighestBidAmount:N0} - ended\n";
                                }
                                else
                                {
                                    auctions += $"**{Stats.Auctions[i].ItemName}:** {Stats.Auctions[i].HighestBidAmount:N0} - {ts:d'd'h'h'm'm's's'}\n";
                                }
                            }
                        }
                        else
                        {
                            embed.WithColor(255, 0, 0);
                            auctions = "This player has no active auctions";
                        }

                    }   

                    embed.WithDescription(auctions);
                    embed.WithTitle($"{username}'s auctions:");
                    await Context.Channel.SendMessageAsync("",false, embed.Build());
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine(ex.Message);
                    await Context.Channel.SendMessageAsync(cross + " This player is not found!");
                }
            }
            else
            {
                await ReplyAsync(cross + " You need to write a username after the command!"); 
            }
        }

        [Command("bazaar")]
        [Alias("bazaar","b")]
        [Summary("Get's the price and amount of an item on Bazaar")]
        public async Task Bazaar([Remainder]string item = null)
        {
            //variables
            string[] parts;
            int amount = 1;

            // item - amount split
            if (item.Contains(','))
            {
                parts = item.Split(',');
                if (int.TryParse(parts[1], out amount))
                {
                    amount = int.Parse(parts[1]);
                }
            }
            item = BazaarItemCheck(item);
            var itemPrice = await API.GetBazaarProductsAsync();
            
            if (item == "")
            {
                await ReplyAsync(questionMark + " This item cannnot be found!");
            }
            else
            {

                EmbedFieldBuilder bazaarPrices = new EmbedFieldBuilder();
                EmbedFieldBuilder flipping = new EmbedFieldBuilder();

                //general embed
                embed.WithDescription("");
                embed.WithTitle($"{item}'s bazaar stats:");
                embed.WithFields(bazaarPrices, flipping);
                embed.WithColor(0,145,230);

                //embed fields
                bazaarPrices.WithName("Prices");
                bazaarPrices.WithValue($"Selling price: {(itemPrice.Products[item].SellSummary.FirstOrDefault().PricePerUnit * amount):f2} coins" +
                    $"\nBuying price: {(itemPrice.Products[item].BuySummary.FirstOrDefault().PricePerUnit * amount):f2} coins");
                bazaarPrices.WithIsInline(false);

                flipping.WithName("Flipping");
                flipping.WithValue($"Profit: {(itemPrice.Products[item].BuySummary.FirstOrDefault().PricePerUnit * amount) - (itemPrice.Products[item].SellSummary.FirstOrDefault().PricePerUnit * amount):f2} coins");


                await ReplyAsync("", false, embed.Build());
            }
        }


        [Command("profiles")]
        [Alias("ps")]
        [Summary("Gets skyblock profiles")]
        public async Task SkyblockProfile([Remainder]string username = null)
        {
            
            string desc = "";
            var ProfileLists = API.GetSkyblockProfilesByName(username);
            int index = -1;


            List<string> uuids = new List<string>();
            List<string> usernames = new List<string>();
            for (int i = 0; i < ProfileLists.Count(); i++)
            {
                uuids = ProfileLists[i].Profile.Members.Keys.ToList();
            }
            for (int i = 0; i < uuids.Count(); i++)
            {
                usernames.Add($"{API.GetUserByUuid(uuids[i]).Player.DisplayName}");
                if (username.ToLower() == API.GetUserByUuid(uuids[i]).Player.DisplayName.ToLower())
                {
                    username = API.GetUserByUuid(uuids[i]).Player.DisplayName;
                    index = i;
                }
            }


            embed.WithDescription(desc);
            embed.WithThumbnailUrl($"https://minotar.net/avatar/{username}");
            embed.WithColor(0, 170, 0);
            embed.WithAuthor($"{username}'s stats");


            await Context.Channel.SendMessageAsync("", false, embed.Build());

        }


        public string BazaarItemCheck(string item = null)
        {
            if (item.Contains(" "))
            {
                if (item.Contains(","))
                {
                    string[] ItemWithoutCount = item.Split(',');
                    item = ItemWithoutCount[0];
                }
                item = item.Replace(" ", "_");
            }
            string itemID = "";
            List<string> Items = new List<string>()
            {
                "ENCHANTED_RAW_CHICKEN",
                "INK_SACK:3;COCOA_BEANS;COCOA",
                "BROWN_MUSHROOM",
                "ENCHANTED_WATER_LILY",
                "INK_SACK:4;LAPIS_LAZULI;LAPIS",
                "TARANTULA_WEB",
                "CARROT_ITEM",
                "ENCHANTED_POTATO",
                "LOG:1;SPRUCE_LOG;SPRUCE",
                "ENCHANTED_SLIME_BALL",
                "ENCHANTED_GOLDEN_CARROT",
                "LOG:3;JUNGLE_LOG;JUNGLE",
                "LOG:2;BIRCH_LOG;BIRCH",
                "ENCHANTED_RABBIT_HIDE",
                "ENCHANTED_GLOWSTONE_DUST",
                "ENCHANTED_INK_SACK",
                "ENCHANTED_CACTUS",
                "ENCHANTED_SUGAR_CANE",
                "ENCHANTED_BIRCH_LOG",
                "ENCHANTED_GUNPOWDER",
                "ENCHANTED_MELON",
                "ENCHANTED_COOKED_SALMON",
                "ENCHANTED_SUGAR",
                "LOG;OAK_LOG;OAK",
                "CACTUS",
                "ENCHANTED_BLAZE_ROD",
                "GHAST_TEAR",
                "ENCHANTED_CAKE",
                "PUMPKIN",
                "ENCHANTED_ENDER_PEARL",
                "PURPLE_CANDY",
                "WHEAT",
                "ENCHANTED_FERMENTED_SPIDER_EYE",
                "ENCHANTED_GOLD_BLOCK",
                "ENCHANTED_RAW_SALMON",
                "ENCHANTED_JUNGLE_LOG",
                "ENCHANTED_FLINT",
                "ENCHANTED_GLISTERING_MELON",
                "IRON_INGOT",
                "PRISMARINE_SHARD",
                "ENCHANTED_EMERALD",
                "ENCHANTED_SPIDER_EYE",
                "ENCHANTED_EMERALD_BLOCK",
                "RED_MUSHROOM",
                "MUTTON",
                "ENCHANTED_MELON_BLOCK",
                "ENCHANTED_CLAY_BALL",
                "DIAMOND",
                "COBBLESTONE",
                "SPIDER_EYE",
                "RAW_FISH",
                "ENCHANTED_PUFFERFISH",
                "GLOWSTONE_DUST",
                "GOLD_INGOT",
                "REVENANT_VISCERA",
                "TARANTULA_SILK",
                "POTATO_ITEM",
                "ENCHANTED_MUTTON",
                "ENCHANTED_HUGE_MUSHROOM_1;ENCHANTED_BROWN_MUSHROOM_BLOCK",
                "SUPER_COMPACTOR_3000",
                "ENCHANTED_IRON",
                "STOCK_OF_STONKS",
                "ENCHANTED_COBBLESTONE",
                "ENCHANTED_BONE",
                "ENCHANTED_PAPER",
                "ENCHANTED_HUGE_MUSHROOM_2;ENCHANTED_RED_MUSHROOM_BLOCK",
                "PORK",
                "ENCHANTED_DIAMOND_BLOCK",
                "EMERALD",
                "ENCHANTED_RABBIT_FOOT",
                "PRISMARINE_CRYSTALS",
                "HOT_POTATO_BOOK",
                "ENCHANTED_ICE",
                "ICE",
                "CLAY_BALL",
                "HUGE_MUSHROOM_1;BROWN_MUSHROOM_BLOCK",
                "HUGE_MUSHROOM_2;RED_MUSHROOM_BLOCK",
                "LOG_2:1;DARK_OAK_LOG;DARK_OAK;DARKOAK",
                "GREEN_GIFT",
                "GOLDEN_TOOTH",
                "STRING",
                "PACKED_ICE",
                "WATER_LILY",
                "RABBIT_FOOT",
                "LOG_2;ACACIA_LOG;ACACIA",
                "REDSTONE",
                "ENCHANTED_OBSIDIAN",
                "ENCHANTED_COAL",
                "COAL",
                "ENCHANTED_QUARTZ",
                "ENDER_PEARL",
                "ENCHANTED_COAL_BLOCK",
                "ENCHANTED_CACTUS_GREEN",
                "ENCHANTED_PRISMARINE_CRYSTALS",
                "ENCHANTED_CARROT_ON_A_STICK",
                "ENCHANTED_ENDSTONE",
                "ENCHANTED_LAPIS_LAZULI_BLOCK",
                "ENCHANTED_COOKIE",
                "ENCHANTED_STRING",
                "SLIME_BALL",
                "ENDER_STONE",
                "ENCHANTED_RAW_FISH",
                "ENCHANTED_ACACIA_LOG",
                "ENCHANTED_EGG",
                "QUARTZ",
                "ENCHANTED_EYE_OF_ENDER",
                "SAND",
                "RAW_CHICKEN",
                "MAGMA_CREAM",
                "SUGAR_CANE",
                "ENCHANTED_LAPIS_LAZULI",
                "ENCHANTED_GHAST_TEAR",
                "ENCHANTED_COCOA",
                "RED_GIFT",
                "ENCHANTED_RAW_BEEF",
                "SEEDS",
                "ENCHANTED_LEATHER",
                "ENCHANTED_SPONGE",
                "ENCHANTED_FEATHER",
                "ENCHANTED_SLIME_BLOCK",
                "ENCHANTED_OAK_LOG",
                "RABBIT_HIDE",
                "WHITE_GIFT",
                "INK_SACK",
                "FLINT",
                "ENCHANTED_SPRUCE_LOG",
                "WOLF_TOOTH",
                "ENCHANTED_ROTTEN_FLESH",
                "ENCHANTED_GRILLED_PORK",
                "SULPHUR",
                "NETHER_STALK",
                "RABBIT",
                "ENCHANTED_NETHER_STALK",
                "ENCHANTED_REDSTONE_BLOCK",
                "ENCHANTED_QUARTZ_BLOCK",
                "ENCHANTED_CARROT",
                "ENCHANTED_PUMPKIN",
                "GREEN_CANDY",
                "ENCHANTED_REDSTONE",
                "ROTTEN_FLESH",
                "ENCHANTED_COOKED_FISH",
                "OBSIDIAN",
                "ENCHANTED_MAGMA_CREAM",
                "GRAVEL",
                "MELON",
                "RAW_FISH:3;PUFFERFISH",
                "ENCHANTED_PRISMARINE_SHARD",
                "ENCHANTED_IRON_BLOCK",
                "LEATHER",
                "ENCHANTED_COOKED_MUTTON",
                "BONE",
                "RAW_FISH:1;RAW_SALMON",
                "REVENANT_FLESH",
                "ENCHANTED_PORK",
                "ENCHANTED_GLOWSTONE",
                "ENCHANTED_BREAD",
                "FEATHER",
                "ENCHANTED_CHARCOAL",
                "ENCHANTED_BLAZE_POWDER",
                "NETHERRACK",
                "SUMMONING_EYE",
                "SPONGE",
                "BLAZE_ROD",
                "ENCHANTED_DARK_OAK_LOG",
                "ENCHANTED_BAKED_POTATO",
                "COMPACTOR",
                "ENCHANTED_DIAMOND",
                "ENCHANTED_GOLD"
            };

            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Contains(';'))
                {
                  string[] line = Items[i].Split(';');
                    for (int j = 0; j < line.Length; j++)
                    {
                        if (item.ToLower() == line[j].ToLower())
                        {
                            itemID = line[0];
                        }
                    }
                }
                else
                {
                    if (item.ToLower() == Items[i].ToLower())
                    {
                        itemID = Items[i];
                    }
                }
            }

            return itemID;
        }
    }
}
