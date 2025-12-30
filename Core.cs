using SantaHatItem.Utils;
using MelonLoader;

[assembly: MelonInfo(typeof(SantaHatItem.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace SantaHatItem
{
    public class Core : MelonMod
    {
        private bool _itemsInitialized = false;
        private S1API.Items.ClothingItemDefinition? _santaHat = null;

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
            _santaHat = S1API.Items.ClothingItemCreator.CloneFrom("cap")
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

            // Defer icon generation until player spawns to use their actual appearance
            S1API.Entities.Player.LocalPlayerSpawned += OnLocalPlayerSpawned;

            // Add to shops
            int shopsAdded = S1API.Shops.ShopManager.AddToCompatibleShops(_santaHat);
            MelonLogger.Msg($"Created clothing item: {_santaHat.Name}");
            MelonLogger.Msg($"Added to {shopsAdded} shop(s)");
        }

        private void OnLocalPlayerSpawned(S1API.Entities.Player player)
        {
            // Unsubscribe after first call
            S1API.Entities.Player.LocalPlayerSpawned -= OnLocalPlayerSpawned;

            if (_santaHat == null)
            {
                MelonLogger.Warning("Santa Hat item not initialized, cannot generate icon.");
                return;
            }

            // Generate icon using the player's actual avatar appearance
            S1API.Rendering.IconFactory.GenerateAccessoryIconSprite(
                S1API.Entities.Appearances.AccessoryFields.Head.SantaHat,
                generatedSprite =>
                {
                    if (generatedSprite != null)
                    {
                        _santaHat.Icon = generatedSprite;
                        MelonLogger.Msg("Generated custom icon for Santa Hat using player's appearance.");
                        
                        // Refresh the icon in all shop listings because we are setting the icon after the item is already added to the shop
                        int updated = S1API.Shops.ShopManager.RefreshItemIcon(_santaHat);
                        if (updated > 0)
                        {
                            MelonLogger.Msg($"Updated icon in {updated} shop listing(s)");
                        }
                    }
                    else
                    {
                        MelonLogger.Warning("Failed to generate icon for Santa Hat.");
                    }
                }
            );
        }
    }
}