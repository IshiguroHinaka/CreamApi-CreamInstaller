using System.IO;
using System.Text;
using System.Threading.Tasks;
using CreamInstaller.Forms;
using CreamInstaller.Utility;

namespace CreamInstaller.Resources;

internal static class SmokeAPI
{
    internal static void GetSmokeApiComponents(this string directory, out string api32, out string api32_o, out string api64, out string api64_o,
        out string old_config, out string config, out string old_log, out string log, out string cache)
    {
        api32 = directory + @"\steam_api.dll";
        api32_o = directory + @"\steam_api_o.dll";
        api64 = directory + @"\steam_api64.dll";
        api64_o = directory + @"\steam_api64_o.dll";
        old_config = directory + @"\SmokeAPI.json";
        config = directory + @"\SmokeAPI.config.json";
        old_log = directory + @"\SmokeAPI.log";
        log = directory + @"\SmokeAPI.log.log";
        cache = directory + @"\SmokeAPI.cache.json";
    }

    internal static void CheckConfig(string directory, ProgramSelection selection, InstallForm installForm = null)
    {
        _ = selection;
        directory.GetSmokeApiComponents(out _, out _, out _, out _, out string old_config, out string config, out _, out _, out _);
        if (File.Exists(old_config))
        {
            File.Delete(old_config);
            installForm?.UpdateUser($"Deleted old configuration: {Path.GetFileName(old_config)}", LogTextBox.Action, false);
        }

        File.Create(config).Close();
        using StreamWriter writer = new(config, false, Encoding.UTF8);
        WriteConfig(writer);
        installForm?.UpdateUser($"Wrote configuration: {Path.GetFileName(config)}", LogTextBox.Action, false);
    }

    private static void WriteConfig(StreamWriter writer)
    {
        writer.WriteLine("{");
        writer.WriteLine("  \"$version\": 2,");
        writer.WriteLine("  \"logging\": false,");
        writer.WriteLine("  \"unlock_family_sharing\": true,");
        writer.WriteLine("  \"default_app_status\": \"unlocked\",");
        writer.WriteLine("  \"override_app_status\": {},");
        writer.WriteLine("  \"override_dlc_status\": {},");
        writer.WriteLine("  \"auto_inject_inventory\": true,");
        writer.WriteLine("  \"extra_inventory_items\": [],");
        writer.WriteLine("  \"extra_dlcs\": {},");
        writer.WriteLine("  \"store_config\": null");
        writer.WriteLine("}");
    }


    internal static async Task Uninstall(string directory, InstallForm installForm = null, bool deleteOthers = true)
        => await Task.Run(() =>
        {
            directory.GetCreamApiComponents(out _, out _, out _, out _, out string oldConfig);
            if (File.Exists(oldConfig))
            {
                File.Delete(oldConfig);
                installForm?.UpdateUser($"Deleted old CreamAPI configuration: {Path.GetFileName(oldConfig)}", LogTextBox.Action, false);
            }
            directory.GetSmokeApiComponents(out string api32, out string api32_o, out string api64, out string api64_o, out string old_config,
                out string config, out string old_log, out string log, out string cache);
            if (File.Exists(api32_o))
            {
                if (File.Exists(api32))
                {
                    File.Delete(api32);
                    installForm?.UpdateUser($"Deleted SmokeAPI: {Path.GetFileName(api32)}", LogTextBox.Action, false);
                }
                File.Move(api32_o, api32!);
                installForm?.UpdateUser($"Restored Steamworks: {Path.GetFileName(api32_o)} -> {Path.GetFileName(api32)}", LogTextBox.Action, false);
            }
            if (File.Exists(api64_o))
            {
                if (File.Exists(api64))
                {
                    File.Delete(api64);
                    installForm?.UpdateUser($"Deleted SmokeAPI: {Path.GetFileName(api64)}", LogTextBox.Action, false);
                }
                File.Move(api64_o, api64!);
                installForm?.UpdateUser($"Restored Steamworks: {Path.GetFileName(api64_o)} -> {Path.GetFileName(api64)}", LogTextBox.Action, false);
            }
            if (!deleteOthers)
                return;
            if (File.Exists(old_config))
            {
                File.Delete(old_config);
                installForm?.UpdateUser($"Deleted configuration: {Path.GetFileName(old_config)}", LogTextBox.Action, false);
            }
            if (File.Exists(config))
            {
                File.Delete(config);
                installForm?.UpdateUser($"Deleted configuration: {Path.GetFileName(config)}", LogTextBox.Action, false);
            }
            if (File.Exists(cache))
            {
                File.Delete(cache);
                installForm?.UpdateUser($"Deleted cache: {Path.GetFileName(cache)}", LogTextBox.Action, false);
            }
            if (File.Exists(old_log))
            {
                File.Delete(old_log);
                installForm?.UpdateUser($"Deleted log: {Path.GetFileName(old_log)}", LogTextBox.Action, false);
            }
            if (File.Exists(log))
            {
                File.Delete(log);
                installForm?.UpdateUser($"Deleted log: {Path.GetFileName(log)}", LogTextBox.Action, false);
            }
        });

    internal static async Task Install(string directory, ProgramSelection selection, InstallForm installForm = null, bool generateConfig = true)
        => await Task.Run(() =>
        {
            directory.GetCreamApiComponents(out _, out _, out _, out _, out string oldConfig);
            if (File.Exists(oldConfig))
            {
                File.Delete(oldConfig);
                installForm?.UpdateUser($"Deleted old CreamAPI configuration: {Path.GetFileName(oldConfig)}", LogTextBox.Action, false);
            }
            directory.GetSmokeApiComponents(out string api32, out string api32_o, out string api64, out string api64_o, out _, out _, out _, out _, out _);
            if (File.Exists(api32) && !File.Exists(api32_o))
            {
                File.Move(api32, api32_o!);
                installForm?.UpdateUser($"Renamed Steamworks: {Path.GetFileName(api32)} -> {Path.GetFileName(api32_o)}", LogTextBox.Action, false);
            }
            if (File.Exists(api32_o))
            {
                "SmokeAPI.steam_api.dll".Write(api32);
                installForm?.UpdateUser($"Wrote SmokeAPI: {Path.GetFileName(api32)}", LogTextBox.Action, false);
            }
            if (File.Exists(api64) && !File.Exists(api64_o))
            {
                File.Move(api64, api64_o!);
                installForm?.UpdateUser($"Renamed Steamworks: {Path.GetFileName(api64)} -> {Path.GetFileName(api64_o)}", LogTextBox.Action, false);
            }
            if (File.Exists(api64_o))
            {
                "SmokeAPI.steam_api64.dll".Write(api64);
                installForm?.UpdateUser($"Wrote SmokeAPI: {Path.GetFileName(api64)}", LogTextBox.Action, false);
            }
            if (generateConfig)
                CheckConfig(directory, selection, installForm);
        });
}
