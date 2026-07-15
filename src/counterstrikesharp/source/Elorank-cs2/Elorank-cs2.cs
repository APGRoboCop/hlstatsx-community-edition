using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using MySqlConnector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Elorank_cs2;

public class ElorankConfig : BasePluginConfig
{
    [JsonPropertyName("DatabaseHost")] public string DatabaseHost { get; set; } = "127.0.0.1";
    [JsonPropertyName("DatabasePort")] public int DatabasePort { get; set; } = 3306;
    [JsonPropertyName("DatabaseUser")] public string DatabaseUser { get; set; } = "hlxuser";
    [JsonPropertyName("DatabasePassword")] public string DatabasePassword { get; set; } = "password";
    [JsonPropertyName("DatabaseName")] public string DatabaseName { get; set; } = "hlstatsx";
    [JsonPropertyName("CooldownSeconds")] public int CooldownSeconds { get; set; } = 30;
}

public class Elorank_cs2 : BasePlugin, IPluginConfig<ElorankConfig>
{
    public override string ModuleName => "[CS2] Elorank for HLstatsX";
    public override string ModuleVersion => "1.3";
    public override string ModuleAuthor => "lovasatt";
    public override string ModuleDescription => "Allows players to set their Competitive rank in HLstatsX manually.";

    public ElorankConfig Config { get; set; } = new();
    private ConcurrentDictionary<ulong, DateTime> _lastCommandUsage = new ConcurrentDictionary<ulong, DateTime>();

    public void OnConfigParsed(ElorankConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            if (@event.Userid != null) _lastCommandUsage.TryRemove(@event.Userid.SteamID, out _);
            return HookResult.Continue;
        });

        Console.WriteLine("[Elorank-cs2] Plugin loaded successfully.");
    }

    [ConsoleCommand("css_mm", "Opens the MM Rank selection menu")]
    [ConsoleCommand("mm", "Opens the MM Rank selection menu")]
    public void OnCommandMM(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.IsBot) return;

        if (_lastCommandUsage.TryGetValue(player.SteamID, out var lastUsed))
        {
            var diff = DateTime.Now - lastUsed;
            if (diff.TotalSeconds < Config.CooldownSeconds)
            {
                int remaining = Config.CooldownSeconds - (int)diff.TotalSeconds;
                player.PrintToChat(Localizer["error.cooldown", remaining]);
                return;
            }
            _lastCommandUsage.TryRemove(player.SteamID, out _);
        }

        var menu = new ChatMenu(Localizer["menu.title"]);

        menu.AddMenuOption("No Rank", (p, opt) => SelectRank(p, 0, "No Rank"));
        menu.AddMenuOption("Silver I", (p, opt) => SelectRank(p, 1, "Silver I"));
        menu.AddMenuOption("Silver II", (p, opt) => SelectRank(p, 2, "Silver II"));
        menu.AddMenuOption("Silver III", (p, opt) => SelectRank(p, 3, "Silver III"));
        menu.AddMenuOption("Silver IV", (p, opt) => SelectRank(p, 4, "Silver IV"));
        menu.AddMenuOption("Silver Elite", (p, opt) => SelectRank(p, 5, "Silver Elite"));
        menu.AddMenuOption("Silver Elite Master", (p, opt) => SelectRank(p, 6, "Silver Elite Master"));
        menu.AddMenuOption("Gold Nova I", (p, opt) => SelectRank(p, 7, "Gold Nova I"));
        menu.AddMenuOption("Gold Nova II", (p, opt) => SelectRank(p, 8, "Gold Nova II"));
        menu.AddMenuOption("Gold Nova III", (p, opt) => SelectRank(p, 9, "Gold Nova III"));
        menu.AddMenuOption("Gold Nova Master", (p, opt) => SelectRank(p, 10, "Gold Nova Master"));
        menu.AddMenuOption("Master Guardian I", (p, opt) => SelectRank(p, 11, "Master Guardian I"));
        menu.AddMenuOption("Master Guardian II", (p, opt) => SelectRank(p, 12, "Master Guardian II"));
        menu.AddMenuOption("Master Guardian Elite", (p, opt) => SelectRank(p, 13, "Master Guardian Elite"));
        menu.AddMenuOption("Distinguished Master Guardian", (p, opt) => SelectRank(p, 14, "DMG"));
        menu.AddMenuOption("Legendary Eagle", (p, opt) => SelectRank(p, 15, "Legendary Eagle"));
        menu.AddMenuOption("Legendary Eagle Master", (p, opt) => SelectRank(p, 16, "LEM"));
        menu.AddMenuOption("Supreme Master First Class", (p, opt) => SelectRank(p, 17, "Supreme"));
        menu.AddMenuOption("The Global Elite", (p, opt) => SelectRank(p, 18, "Global Elite"));

        MenuManager.OpenChatMenu(player, menu);
    }
    
    private void SelectRank(CCSPlayerController player, int rankId, string rankName)
    {
        if (player == null || !player.IsValid)
            return;
        MenuManager.CloseActiveMenu(player);
        _lastCommandUsage[player.SteamID] = DateTime.Now;
        SetRank(player.Slot, player.SteamID, rankId, rankName);
    }

    private void SetRank(int slot, ulong steamId, int rankId, string rankName)
    {
        string uniqueId = GetSteam2ID(steamId);

        // Async database operation
        Task.Run(async () =>
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = Config.DatabaseHost,
                    Port = (uint)Config.DatabasePort,
                    UserID = Config.DatabaseUser,
                    Password = Config.DatabasePassword,
                    Database = Config.DatabaseName
                };

                using var connection = new MySqlConnection(builder.ConnectionString);
                await connection.OpenAsync();

                string query = @"
                    UPDATE hlstats_Players p
                    JOIN hlstats_PlayerUniqueIds u ON p.playerId = u.playerId
                    SET p.mmrank = @Rank
                    WHERE u.uniqueId = @UniqueId AND u.game = 'cs2';
                ";

                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Rank", rankId);
                cmd.Parameters.AddWithValue("@UniqueId", uniqueId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                // Return to the main thread
                Server.NextFrame(() =>
                {
                    var targetPlayer = Utilities.GetPlayerFromSlot(slot);
                    if (targetPlayer != null && targetPlayer.IsValid && targetPlayer.SteamID == steamId)
                    {
                        if (rowsAffected > 0)
                        {
                            targetPlayer.PrintToChat(Localizer["rank.set.success", rankName]);
                        }
                        else
                        {
                            targetPlayer.PrintToChat(Localizer["error.profile.not.found"]);
                            targetPlayer.PrintToChat(Localizer["error.wait.for.stats"]);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Elorank-cs2] Database Error: {ex.Message}");
            }
        });
    }

    private string GetSteam2ID(ulong steamId64)
    {
        if (steamId64 < 76561197960265728) return "";
        long steamId32 = (long)steamId64 - 76561197960265728;
        long y = steamId32 % 2;
        long z = (steamId32 - y) / 2;
        return $"{y}:{z}";
    }
}