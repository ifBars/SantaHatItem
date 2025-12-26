using SantaHatItem.Utils;
using MelonLoader;

[assembly: MelonInfo(typeof(SantaHatItem.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace SantaHatItem
{
    public class Core : MelonMod
    {
        private bool _itemsInitialized = false;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Main" && !_itemsInitialized)
            {
                InitializeCustomClothing();
                _itemsInitialized = true;
            }
        }

        private void InitializeCustomClothing()
        {
            // Create Santa Hat clothing item using existing accessory
            var santaHat = S1API.Items.ClothingItemCreator.CloneFrom("cap")
                .WithBasicInfo(
                    id: "santa_hat",
                    name: "Santa Hat",
                    description: "A festive hat for the holidays.")
                .WithClothingAsset(S1API.Entities.Appearances.AccessoryFields.Head.SantaHat)
                .WithColorable(false)
                .WithPricing(100f, 0.5f)
                .WithKeywords("hat", "cap", "santa", "christmas", "holiday")
                .WithLabelColor(UnityEngine.Color.red)
                .Build();

            // Add to shops
            int shopsAdded = S1API.Shops.ShopManager.AddToCompatibleShops(santaHat);
            MelonLogger.Msg($"Created clothing item: {santaHat.Name}");
            MelonLogger.Msg($"Added to {shopsAdded} shop(s)");
        }
    }
}