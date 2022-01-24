using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.UI;

namespace TTTReborn.Settings
{
    public partial class Settings
    {
        public Categories.General General { get; set; } = new();
    }

    namespace Categories
    {
        using Globalization;

        public partial class General
        {
            [DropdownSetting]
            public string Language { get; set; } = Globalization.TTTLanguage.FALLBACK_LANGUAGE;

            [SwitchSetting]
            public bool ReturnMissingKeys { get; set; } = false;

            [JsonIgnore]
            [DropdownOptions("Language")]
            public Dictionary<string, object> LanguageOptions
            {
                get
                {
                    Dictionary<string, object> dict = new();

                    foreach (Language language in TTTLanguage.Languages.Values)
                    {
                        dict.Add(language.Data.Name, language.Data.Code);
                    }

                    return dict;
                }
            }
        }
    }
}

namespace TTTReborn.Globalization
{
    public static class TTTLanguage
    {
        public static readonly Dictionary<string, Language> Languages = new();

        public static readonly List<ITranslatable> Translatables = new();

        public const string FALLBACK_LANGUAGE = "en-US";

        public static Language ActiveLanguage
        {
            get => _activeLanguage;
            set
            {
                _activeLanguage = value ?? GetLanguageByCode(FALLBACK_LANGUAGE);
            }
        }
        private static Language _activeLanguage;

        public static void Load()
        {
            foreach (string file in FileSystem.Mounted.FindFile("/lang/packs/", "*.json", false))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string json = FileSystem.Mounted.ReadAllText($"/lang/packs/{file}");

                Language language = new(name, json);

                Languages.Add(language.Data.Code ?? name, language);

                Log.Info($"Added language pack: '{name}'.");
            }

            ActiveLanguage = GetLanguageByCode(FALLBACK_LANGUAGE);
        }

        public static Language GetLanguageByCode(string name)
        {
            Language lang = null;

            if (Languages != null && !String.IsNullOrEmpty(name))
            {
                if (!Languages.TryGetValue(name, out lang))
                {
                    Log.Warning($"Tried to get a language that does not exist: '{name}'.");
                }
            }

            return lang ?? GetLanguageByCode(FALLBACK_LANGUAGE);
        }

        public static void UpdateLanguage(Language language)
        {
            if (language.Data.Code == ActiveLanguage.Data.Code)
            {
                return;
            }

            Language oldLanguage = ActiveLanguage;

            ActiveLanguage = language;

            Translatables.ForEach((translatable) => translatable.UpdateLanguage(ActiveLanguage));
        }

        [Event(TTTEvent.Settings.Change)]
        public static void OnChangeLanguageSettings()
        {
            UpdateLanguage(GetLanguageByCode(Settings.SettingsManager.Instance.General.Language));
        }
    }
}

namespace TTTReborn.Player
{
    using Globalization;

    public partial class TTTPlayer
    {
        [ClientCmd("ttt_language")]
        public static void ChangeLanguage(string name = null)
        {
            if (name == null)
            {
                Log.Info($"Your current language is set to '{TTTLanguage.ActiveLanguage.Data.Name}' ('{TTTLanguage.ActiveLanguage.Data.Code}').");

                return;
            }

            Language language = TTTLanguage.GetLanguageByCode(name);

            if (language == null)
            {
                Log.Warning($"Language '{name}' does not exist. Please enter an ISO (tag) code (http://www.lingoes.net/en/translator/langcode.htm).");

                return;
            }

            Log.Warning($"You set your language to '{language.Data.Name}'.");

            Settings.SettingsManager.Instance.General.Language = language.Data.Code;

            Event.Run(TTTEvent.Settings.Change);
        }
    }
}
