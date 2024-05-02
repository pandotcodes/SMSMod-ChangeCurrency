using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MyBox;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ChangeCurrency
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource StaticLogger;

        public static ConfigEntry<string> CurrencyPrefix;
        public static ConfigEntry<string> CurrencySuffix;
        public static ConfigEntry<string> CurrencyDecimalSeperator;

        public static ConfigEntry<float> CoinValue1ct;
        public static ConfigEntry<float> CoinValue5ct;
        public static ConfigEntry<float> CoinValue10ct;
        public static ConfigEntry<float> CoinValue25ct;
        public static ConfigEntry<float> CoinValue50ct;

        public static ConfigEntry<float> BillValue1d;
        public static ConfigEntry<float> BillValue5d;
        public static ConfigEntry<float> BillValue10d;
        public static ConfigEntry<float> BillValue20d;
        public static ConfigEntry<float> BillValue50d;

        public static ConfigEntry<float> LowestBill;
        public static ConfigEntry<string> TerminalSymbol;

        public static ConfigEntry<string> ValueText1ct;
        public static ConfigEntry<string> ValueText5ct;
        public static ConfigEntry<string> ValueText10ct;
        public static ConfigEntry<string> ValueText25ct;
        public static ConfigEntry<string> ValueText50ct;

        public static ConfigEntry<string> ValueText1d;
        public static ConfigEntry<string> ValueText5d;
        public static ConfigEntry<string> ValueText10d;
        public static ConfigEntry<string> ValueText20d;
        public static ConfigEntry<string> ValueText50d;

        public static ConfigEntry<TextureType> Texture1ct;
        public static ConfigEntry<TextureType> Texture5ct;
        public static ConfigEntry<TextureType> Texture10ct;
        public static ConfigEntry<TextureType> Texture25ct;
        public static ConfigEntry<TextureType> Texture50ct;

        public static ConfigEntry<TextureType> Texture1d;
        public static ConfigEntry<TextureType> Texture5d;
        public static ConfigEntry<TextureType> Texture10d;
        public static ConfigEntry<TextureType> Texture20d;
        public static ConfigEntry<TextureType> Texture50d;
        private void Awake()
        {
            StaticLogger = Logger;

            InitConfig();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded! Applying patch...");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
        public enum MoneyType
        {
            CENT_1 = 0b0001, CENT_5 = 0b0010, CENT_10 = 0b0011, CENT_25 = 0b0100, CENT_50 = 0b0101,
            DOLLAR_1 = 0b1001, DOLLAR_5 = 0b1010, DOLLAR_10 = 0b1011, DOLLAR_20 = 0b1100, DOLLAR_50 = 0b1101
        }
        public enum TextureType
        {
            USD, EUR, GBP, SGD, CAD
        }
        public static void ApplyMoneyTexture(MoneyType type, CheckoutDrawer instance, int instanceIndex, int prefabIndex)
        {
            var tex = GetMoneyTexture(type);
            if (tex == null) return;
            instance.gameObject.transform.GetChild(instanceIndex).GetChild(0).GetChild(0).GetComponentsInChildren<MeshRenderer>().ForEach(x => x.sharedMaterials.ForEach(y => y.mainTexture = tex));
            Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[prefabIndex].GetComponent<MeshRenderer>().sharedMaterials.ForEach(x => x.mainTexture = tex);
        }
        private static Texture2D GetMoneyTexture(MoneyType type)
        {
            bool isBill = (((int)type) & 0b1000) == 0b1000;
            Texture2D tex = new Texture2D(isBill ? 2048 : 1024, 1024);
            var bytes = GetMoneyTextureBytes(type);
            if (bytes == null) return null;
            tex.LoadImage(bytes);
            tex.Apply();
            return tex;
        }
        private static byte[] GetMoneyTextureBytes(MoneyType type)
        {
            switch (type)
            {
                default:
                case MoneyType.CENT_1:
                    switch (Texture1ct.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur_1;
                        case TextureType.GBP:
                            return Properties.Resources.gbp_1;
                        case TextureType.SGD:
                            return Properties.Resources.sgd_1;
                        case TextureType.CAD:
                            return Properties.Resources.cad_1;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.CENT_5:
                    switch (Texture5ct.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur_5;
                        case TextureType.GBP:
                            return Properties.Resources.gbp_5;
                        case TextureType.SGD:
                            return Properties.Resources.sgd_5;
                        case TextureType.CAD:
                            return Properties.Resources.cad_5;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.CENT_10:
                    switch (Texture10ct.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur_10;
                        case TextureType.GBP:
                            return Properties.Resources.gbp_10;
                        case TextureType.SGD:
                            return Properties.Resources.sgd_20;
                        case TextureType.CAD:
                            return Properties.Resources.cad_10;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.CENT_25:
                    switch (Texture25ct.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur_20;
                        case TextureType.GBP:
                            return Properties.Resources.gbp_20;
                        case TextureType.SGD:
                            return Properties.Resources.sgd_50;
                        case TextureType.CAD:
                            return Properties.Resources.cad_25;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.CENT_50:
                    switch (Texture50ct.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur_50;
                        case TextureType.GBP:
                            return Properties.Resources.gbp_50;
                        case TextureType.SGD:
                            return Properties.Resources.sgd1;
                        case TextureType.CAD:
                            return Properties.Resources.cad_50;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.DOLLAR_1:
                    switch (Texture1d.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur1;
                        case TextureType.GBP:
                            return Properties.Resources.gbp1;
                        case TextureType.SGD:
                            return Properties.Resources.sgd2;
                        case TextureType.CAD:
                            return Properties.Resources.cad1;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.DOLLAR_5:
                    switch (Texture5d.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur5;
                        case TextureType.GBP:
                            return Properties.Resources.gbp5;
                        case TextureType.SGD:
                            return Properties.Resources.sgd5;
                        case TextureType.CAD:
                            return Properties.Resources.cad5;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.DOLLAR_10:
                    switch (Texture10d.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur10;
                        case TextureType.GBP:
                            return Properties.Resources.gbp10;
                        case TextureType.SGD:
                            return Properties.Resources.sgd10;
                        case TextureType.CAD:
                            return Properties.Resources.cad10;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.DOLLAR_20:
                    switch (Texture20d.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur20;
                        case TextureType.GBP:
                            return Properties.Resources.gbp20;
                        case TextureType.SGD:
                            return Properties.Resources.sgd20;
                        case TextureType.CAD:
                            return Properties.Resources.cad20;
                        case TextureType.USD:
                        default:
                            return null;
                    }
                case MoneyType.DOLLAR_50:
                    switch (Texture50d.Value)
                    {
                        case TextureType.EUR:
                            return Properties.Resources.eur50;
                        case TextureType.GBP:
                            return Properties.Resources.gbp50;
                        case TextureType.SGD:
                            return Properties.Resources.sgd50;
                        case TextureType.CAD:
                            return Properties.Resources.cad50;
                        case TextureType.USD:
                        default:
                            return null;
                    }
            }
        }
        private void InitConfig()
        {
            CurrencyPrefix = Config.Bind("Money Text", "Prefix", "$", "The currency symbol (or arbitrary string) to use in front of the value, where the dollar sign would normally be.");
            CurrencySuffix = Config.Bind("Money Text", "Suffix", "", "The currency symbol (or arbitrary string) to use after the value, where a euro sign might for example be.");
            CurrencyDecimalSeperator = Config.Bind("Money Text", "Decimal Seperator", ".", "What symbol to use to seperate the whole number part from the fractional part.");

            CoinValue1ct = Config.Bind("Cash Values", "1 Cent Coin", 0.01f, "What cash value the 1 cent coin should represent.");
            CoinValue5ct = Config.Bind("Cash Values", "5 Cent Coin", 0.05f, "What cash value the 5 cent coin should represent.");
            CoinValue10ct = Config.Bind("Cash Values", "10 Cent Coin", 0.1f, "What cash value the 10 cent coin should represent.");
            CoinValue25ct = Config.Bind("Cash Values", "25 Cent Coin", 0.25f, "What cash value the 25 cent coin should represent.");
            CoinValue50ct = Config.Bind("Cash Values", "50 Cent Coin", 0.5f, "What cash value the 50 cent coin should represent.");

            BillValue1d = Config.Bind("Cash Values", "1 Dollar Bill", 1f, "What cash value the 1 dollar bill should represent.");
            BillValue5d = Config.Bind("Cash Values", "5 Dollar Bill", 5f, "What cash value the 5 dollar bill should represent.");
            BillValue10d = Config.Bind("Cash Values", "10 Dollar Bill", 10f, "What cash value the 10 dollar bill should represent.");
            BillValue20d = Config.Bind("Cash Values", "20 Dollar Bill", 20f, "What cash value the 20 dollar bill should represent.");
            BillValue50d = Config.Bind("Cash Values", "50 Dollar Bill", 50f, "What cash value the 50 dollar bill should represent.");

            ValueText1ct = Config.Bind("Value Texts", "1 Cent Coin", "1¢", "What value text to display below the 1 cent coin.");
            ValueText5ct = Config.Bind("Value Texts", "5 Cent Coin", "5¢", "What value text to display below the 5 cent coin.");
            ValueText10ct = Config.Bind("Value Texts", "10 Cent Coin", "10¢", "What value text to display below the 10 cent coin.");
            ValueText25ct = Config.Bind("Value Texts", "25 Cent Coin", "25¢", "What value text to display below the 25 cent coin.");
            ValueText50ct = Config.Bind("Value Texts", "50 Cent Coin", "50¢", "What value text to display below the 50 cent coin.");

            ValueText1d = Config.Bind("Value Texts", "1 Dollar Bill", "$1", "What value text to display above the 1 dollar bill.");
            ValueText5d = Config.Bind("Value Texts", "5 Dollar Bill", "$5", "What value text to display above the 5 dollar bill.");
            ValueText10d = Config.Bind("Value Texts", "10 Dollar Bill", "$10", "What value text to display above the 10 dollar bill.");
            ValueText20d = Config.Bind("Value Texts", "20 Dollar Bill", "$20", "What value text to display above the 20 dollar bill.");
            ValueText50d = Config.Bind("Value Texts", "50 Dollar Bill", "$50", "What value text to display above the 50 dollar bill.");

            Texture1ct = Config.Bind("Texture Types", "1 Cent Coin", TextureType.USD, "What visual style this coin should have.");
            Texture5ct = Config.Bind("Texture Types", "5 Cent Coin", TextureType.USD, "What visual style this coin should have.");
            Texture10ct = Config.Bind("Texture Types", "10 Cent Coin", TextureType.USD, "What visual style this coin should have.");
            Texture25ct = Config.Bind("Texture Types", "25 Cent Coin", TextureType.USD, "What visual style this coin should have.");
            Texture50ct = Config.Bind("Texture Types", "50 Cent Coin", TextureType.USD, "What visual style this coin should have.");

            Texture1d = Config.Bind("Texture Types", "1 Dollar Bill", TextureType.USD, "What visual style this bill should have.");
            Texture5d = Config.Bind("Texture Types", "5 Dollar Bill", TextureType.USD, "What visual style this bill should have.");
            Texture10d = Config.Bind("Texture Types", "10 Dollar Bill", TextureType.USD, "What visual style this bill should have.");
            Texture20d = Config.Bind("Texture Types", "20 Dollar Bill", TextureType.USD, "What visual style this bill should have.");
            Texture50d = Config.Bind("Texture Types", "50 Dollar Bill", TextureType.USD, "What visual style this bill should have.");

            LowestBill = Config.Bind("Miscellaneous", "Lowest Bill", 1f, "The lowest value of money that should be considered a bill by the game.");
            TerminalSymbol = Config.Bind("Miscellaneous", "Terminal Symbol", "$", "What symbol to display on the credit card terminal.");
        }
    }
    public static class CheckoutDrawerPatch
    {
        [HarmonyPatch(typeof(CheckoutChangeManager), "SpawnMoney")]
        public static class CheckoutChangeManager_SpawnMoney_Patch
        {
            public static void Prefix(bool isCoin) => Plugin.StaticLogger.LogWarning(isCoin);
        }
        [HarmonyPatch(typeof(CheckoutChangeManager), "AddOrRemoveMoney")]
        public static class CheckoutChangeManager_AddOrRemoveMoney_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    // Check if the instruction loads the constant 1f onto the stack
                    if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 1f)
                    {
                        // Replace the constant 1f with your desired value (e.g., 2f)
                        yield return new CodeInstruction(OpCodes.Ldc_R4, Plugin.LowestBill.Value);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(PosTerminal), "Start")]
        public static class PosTerminal_Start_Patch
        {
            public static void Postfix(PosTerminal __instance)
            {
                string p = Plugin.TerminalSymbol.Value;
                __instance.gameObject.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = p;
            }
        }
        [HarmonyPatch(typeof(CheckoutDrawer), "Awake")]
        public static class CheckoutDrawer_Awake_Patch
        {
            public static void Postfix(CheckoutDrawer __instance)
            {
                void UpdateText(TextMeshProUGUI text, string newText)
                {
                    text.text = newText;
                    text.enableWordWrapping = false;
                    text.gameObject.SetActive(false);
                    text.gameObject.SetActive(true);
                }

                __instance.gameObject.transform.GetChild(6).GetComponent<MoneyPack>().Value = Plugin.CoinValue1ct.Value; // $0.01
                __instance.gameObject.transform.GetChild(7).GetComponent<MoneyPack>().Value = Plugin.CoinValue5ct.Value; // $0.05
                __instance.gameObject.transform.GetChild(8).GetComponent<MoneyPack>().Value = Plugin.CoinValue10ct.Value; // $0.10
                __instance.gameObject.transform.GetChild(9).GetComponent<MoneyPack>().Value = Plugin.CoinValue25ct.Value; // $0.25
                __instance.gameObject.transform.GetChild(10).GetComponent<MoneyPack>().Value = Plugin.CoinValue50ct.Value; // $0.50

                __instance.gameObject.transform.GetChild(1).GetComponent<MoneyPack>().Value = Plugin.BillValue1d.Value; // $1.00
                __instance.gameObject.transform.GetChild(2).GetComponent<MoneyPack>().Value = Plugin.BillValue5d.Value; // $5.00
                __instance.gameObject.transform.GetChild(3).GetComponent<MoneyPack>().Value = Plugin.BillValue10d.Value; // $10.00
                __instance.gameObject.transform.GetChild(4).GetComponent<MoneyPack>().Value = Plugin.BillValue20d.Value; // $20.00
                __instance.gameObject.transform.GetChild(5).GetComponent<MoneyPack>().Value = Plugin.BillValue50d.Value; // $50.00

                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(5).GetComponent<TextMeshProUGUI>(), Plugin.ValueText1ct.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(6).GetComponent<TextMeshProUGUI>(), Plugin.ValueText5ct.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(7).GetComponent<TextMeshProUGUI>(), Plugin.ValueText10ct.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(8).GetComponent<TextMeshProUGUI>(), Plugin.ValueText25ct.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(9).GetComponent<TextMeshProUGUI>(), Plugin.ValueText50ct.Value);

                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(0).GetComponent<TextMeshProUGUI>(), Plugin.ValueText1d.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(1).GetComponent<TextMeshProUGUI>(), Plugin.ValueText5d.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(2).GetComponent<TextMeshProUGUI>(), Plugin.ValueText10d.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(3).GetComponent<TextMeshProUGUI>(), Plugin.ValueText20d.Value);
                UpdateText(__instance.gameObject.transform.GetChild(11).GetChild(4).GetComponent<TextMeshProUGUI>(), Plugin.ValueText50d.Value);

                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[0].Value = Plugin.CoinValue1ct.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[1].Value = Plugin.CoinValue5ct.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[2].Value = Plugin.CoinValue10ct.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[3].Value = Plugin.CoinValue25ct.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[4].Value = Plugin.CoinValue50ct.Value;

                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[5].Value = Plugin.BillValue1d.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[6].Value = Plugin.BillValue5d.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[7].Value = Plugin.BillValue10d.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[8].Value = Plugin.BillValue20d.Value;
                Singleton<MoneyGenerator>.Instance.m_MoneyPrefabs[9].Value = Plugin.BillValue50d.Value;

                Plugin.ApplyMoneyTexture(Plugin.MoneyType.DOLLAR_1, __instance, 1, 5);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.DOLLAR_5, __instance, 2, 6);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.DOLLAR_10, __instance, 3, 7);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.DOLLAR_20, __instance, 4, 8);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.DOLLAR_50, __instance, 5, 9);

                Plugin.ApplyMoneyTexture(Plugin.MoneyType.CENT_1, __instance, 6, 0);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.CENT_5, __instance, 7, 1);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.CENT_10, __instance, 8, 2);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.CENT_25, __instance, 9, 3);
                Plugin.ApplyMoneyTexture(Plugin.MoneyType.CENT_50, __instance, 10, 4);
            }
        }
    }
    public static class MoneySymbolPatch
    {

        [HarmonyPatch(typeof(Extensions), "ToMoneyText")]
        public static class Extensions_ToMoneyText_Patch
        {
            public static void Postfix(ref string __result, ref float money, ref float fontSize)
            {
                string p = Plugin.CurrencyPrefix.Value;
                string s = Plugin.CurrencySuffix.Value;
                string d = Plugin.CurrencyDecimalSeperator.Value;
                string text;
                if (money < 0f)
                {
                    text = "-$" + Math.Abs((float)Math.Round((double)money, 2)).ToString("0.00");
                }
                else
                {
                    text = "$" + ((float)Math.Round((double)money, 2)).ToString("0.00");
                }
                text = text.Replace(',', '.');
                text = text.Replace(".", d);
                text = text.Replace("$", p);
                text = text + s;
                int num = text.IndexOf(d);
                if (num != -1)
                {
                    string text2 = "<size=" + (fontSize * 20f / 25f).ToString() + ">";
                    text2 = text2.Replace(',', '.');
                    text = text.Insert(num + 1, text2);
                }
                __result = text;
            }
        }
    }
}
