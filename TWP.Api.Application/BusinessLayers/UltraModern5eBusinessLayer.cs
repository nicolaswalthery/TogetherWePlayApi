using Common.Extensions;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class UltraModern5eBusinessLayer : IUltraModern5eBusinessLayer
    {
        private string _shootAndLootTextResult = "Loot And Shoot";
        private IUltraModern5eJsonRepository _ultraModern5EJsonRepository;

        public UltraModern5eBusinessLayer(IUltraModern5eJsonRepository ultraModern5EJsonRepository)
        {
            _ultraModern5EJsonRepository = ultraModern5EJsonRepository;
        }

        public async Task<Result<string>> GenerateTreasureHoard(int challengeRating)
            => Safe.Execute(() =>
            {
                string treasureHoard = String.Empty;
                if (challengeRating >= 0 && challengeRating <= 4)
                    treasureHoard = GetTreasureHoardChallenge0_4();
                else if (challengeRating >= 5 && challengeRating <= 10)
                    treasureHoard = GetTreasureHoardChallenge5_10();
                else if (challengeRating >= 11 && challengeRating <= 16)
                    treasureHoard = GetTreasureHoardChallenge11_16();
                else if (challengeRating >= 17)
                    treasureHoard = GetTreasureHoardChallenge17Plus();
                return Result<string>.Success(treasureHoard);
            });

        public async Task<Result<ShootAndLootDto>> ShootAndLootGeneration()
            => await Safe.ExecuteAsync(async () =>
            {
                var shootAndLootDto = new ShootAndLootDto();
                GetAndMapWeaponTypeDataDto(shootAndLootDto);
                var numberOfLinesAndModels = GetAndMapWeaponRarityDataToDto(shootAndLootDto, ComputeRarityModifier(1, 2));
                GetAndMapWeaponCompanyDataToDto(shootAndLootDto);
                for (int i = 0; i < numberOfLinesAndModels.numberModels; i++)
                {
                    GetAndMapWeaponModelDataToDto(shootAndLootDto);
                }
                for (int i = 0; i < numberOfLinesAndModels.numberLines; i++)
                {
                    GetAndMapWeaponLineDataToDto(shootAndLootDto);
                }
                shootAndLootDto.Cost = (shootAndLootDto.BaseCost.ToInt() * shootAndLootDto.CostMultiplier.ToInt()).ToString();
                return Result<ShootAndLootDto>.Success(shootAndLootDto);
            });
        

        private string GetTreasureHoardChallenge0_4()
        {
            switch (RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d100))
            {
                case int n when (n >= 1 && n <= 6):
                    return "No treasure.";
                case int n when (n >= 7 && n <= 16):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*10} credits";
                case int n when (n >= 17 && n <= 26):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits";
                case int n when (n >= 27 && n <= 36):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*50} credits";
                case int n when (n >= 37 && n <= 44):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*10} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)))}";
                case int n when (n >= 45 && n <= 52):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)))}";               
                case int n when (n >= 53 && n <= 60):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*50} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)))}";
                case int n when (n >= 61 && n <= 65):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*10} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 66 && n <= 70):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 71 && n <= 75):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*50} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 76 && n <= 78):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*25} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 79 && n <= 80):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 81 && n <= 85):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*50} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 86 && n <= 92):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 93 && n <= 97):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*50} credits. {ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)))}";
                case int n when (n >= 98 && n <= 99):
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. {_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyASingleEntry().ResultText}";
                case 100:
                    return$"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d6)*50} credits. {_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyASingleEntry().ResultText}";
                default:
                    throw new Exception("Invalide Roll");

            }
        }

        private string GetTreasureHoardChallenge5_10()
        {
            switch (RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d100))
            {
                case int n when (n >= 1 && n <= 4):
                    return "No treasure.";
                case int n when (n >= 5 && n <= 10):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits.";
                case int n when (n >= 11 && n <= 16):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*50} credits.";
                case int n when (n >= 17 && n <= 22):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits.";
                case int n when (n >= 23 && n <= 28):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits.";
                case int n when (n >= 29 && n <= 32):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 33 && n <= 36):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*50} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 37 && n <= 40):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 41 && n <= 44):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 45 && n <= 49):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 50 && n <= 54):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*50} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 55 && n <= 59):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 60 && n <= 64):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 65 && n <= 66):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 67 && n <= 69):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*50} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 70 && n <= 72):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 73 && n <= 74):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 75):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n == 76):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*50} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n == 77):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n == 78):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n >= 79 && n <= 80):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n >= 81 && n <= 84):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*25} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 85 && n <= 88):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*50} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 89 && n <= 92):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 93 && n <= 96):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 97 && n <= 98):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n == 99):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6)*100} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n == 100):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4)*250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(1));
                default:
                    throw new Exception("Invalid Roll");
            }
        }

        private string GetTreasureHoardChallenge11_16()
        {
            switch (RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d100))
            {
                case int n when (n >= 1 && n <= 3):
                    return "No treasure.";
                case int n when (n >= 4 && n <= 6):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 250} credits.";
                case int n when (n >= 7 && n <= 9):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 750} credits.";
                case int n when (n >= 10 && n <= 12):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits.";
                case int n when (n >= 13 && n <= 15):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits.";
                case int n when (n >= 16 && n <= 19):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4))) + ", " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 20 && n <= 23):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 750} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4))) + ", " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 24 && n <= 26):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4))) + ", " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 27 && n <= 29):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4))) + ", " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_B_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 30 && n <= 35):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 36 && n <= 40):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 750} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 41 && n <= 45):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 46 && n <= 50):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 51 && n <= 54):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 55 && n <= 58):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 750} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 59 && n <= 62):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 63 && n <= 66):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 67 && n <= 68):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n >= 69 && n <= 70):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 750} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n >= 71 && n <= 72):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n >= 73 && n <= 74):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n >= 75 && n <= 80):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 250} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_F_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4))) + ", " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 81 && n <= 86):
                    return $"{RandomSelectionHelpers.RollDie(2, DiceTypeEnum.d4) * 750} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 87 && n <= 92):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 93 && n <= 96):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 97 && n <= 99):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(1));
                case int n when (n == 100):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(1));
                default:
                    throw new Exception("Invalid Roll");
            }
        }

        private string GetTreasureHoardChallenge17_16()
        {
            switch (RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d100))
            {
                case int n when (n >= 1 && n <= 2):
                    return "No treasure.";
                case int n when (n >= 3 && n <= 5):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 6 && n <= 8):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 9 && n <= 11):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 12 && n <= 14):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 15 && n <= 22):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 23 && n <= 30):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 31 && n <= 38):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 39 && n <= 46):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 47 && n <= 52):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 53 && n <= 58):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 59 && n <= 63):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 64 && n <= 68):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n == 69):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 70):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 71):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 72):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 73 && n <= 74):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 75 && n <= 76):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 77 && n <= 78):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 79 && n <= 80):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 81 && n <= 85):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 86 && n <= 90):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 91 && n <= 95):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 96 && n <= 100):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                default:
                    throw new Exception("Invalid Roll");
            }
        }

        private string GetTreasureHoardChallenge17Plus()
        {
            switch (RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d100))
            {
                case int n when (n >= 1 && n <= 2):
                    return "No treasure.";
                case int n when (n >= 3 && n <= 5):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 6 && n <= 8):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 9 && n <= 11):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 12 && n <= 14):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_C_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8)));
                case int n when (n >= 15 && n <= 22):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 23 && n <= 30):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 31 && n <= 38):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 39 && n <= 46):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_D_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 47 && n <= 52):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 53 && n <= 58):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 59 && n <= 63):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n >= 64 && n <= 68):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_E_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d6)));
                case int n when (n == 69):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 70):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 71):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n == 72):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_G_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 73 && n <= 74):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 75 && n <= 76):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 77 && n <= 78):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 79 && n <= 80):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_H_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 81 && n <= 85):
                    return $"{RandomSelectionHelpers.RollDie(3, DiceTypeEnum.d6) * 1000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 86 && n <= 90):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d10) * 2500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 91 && n <= 95):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d2) * 7500} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                case int n when (n >= 96 && n <= 100):
                    return $"{RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d8) * 5000} credits. " + ListToString(_ultraModern5EJsonRepository.GetTechItemTable_I_RandomTable().GetRandomlyManyEntries(RandomSelectionHelpers.RollDie(1, DiceTypeEnum.d4)));
                default:
                    throw new Exception("Invalid Roll");
            }
        }

        private string ListToString(List<RollTableEntryDto> rollTableEntryDtos) 
            => string.Join(", ", rollTableEntryDtos.Select(e => e.ResultText));

        private async Task<List<ShootAndLootDto>> ShootAndLootGenerations(int numberOfShootAndLoots)
        {
            var shootAndLootDtos = new List<ShootAndLootDto>();
            for (int i = 0; i < numberOfShootAndLoots; i++)
            {
                var result = await ShootAndLootGeneration();
                shootAndLootDtos.Add(result.Data);
            }
            return shootAndLootDtos;
        }

        private static int ComputeRarityModifier(int averageCharacterLevel, int monsterChallengeRating)
        {
            // Calculate the difference between the monster challenge rating and the average character level
            int difference = monsterChallengeRating - averageCharacterLevel;

            // Determine the rarity modifier based on the average character level
            int levelModifier = averageCharacterLevel switch
            {
                <= 5 => 0,  // Levels 1-5: No modifier
                <= 10 => 1, // Levels 6-10: +1
                <= 15 => 3, // Levels 11-15: +3
                _ => 6      // Levels 16-20: +6
            };

            // Add the difference to the rarity roll modifier
            return difference + levelModifier;
        }

        private void GetAndMapWeaponLineDataToDto(ShootAndLootDto shootAndLootDto)
        {
            var weaponLine = _ultraModern5EJsonRepository.GetShootAndLootLineData();
            var weaponLineAddProperties = _ultraModern5EJsonRepository.GetShootAndLootAdditionalProperty();

            var dieResult = RandomSelectionHelpers.RollDie(weaponLine.NumberOfDiceType, weaponLine.DiceType);

            shootAndLootDto.CompanyLineData = new()
            {
                AdditionalProperty = GetByDieResult(weaponLineAddProperties, dieResult).ResultText,
                lineName = GetByDieResult(weaponLine, dieResult).ResultText
            };
        }

        private void GetAndMapWeaponModelDataToDto(ShootAndLootDto shootAndLootDto)
        {
            var weaponModelNames = _ultraModern5EJsonRepository.GetShootAndLootModelName();
            var weaponModelBenefits = _ultraModern5EJsonRepository.GetShootAndLootModelBenefit();

            var dieResult = RandomSelectionHelpers.RollDie(weaponModelNames.NumberOfDiceType, weaponModelNames.DiceType);

            shootAndLootDto.CompanyModelData = new()
            {
                Benefit = GetByDieResult(weaponModelBenefits, dieResult).ResultText,
                modelName = GetByDieResult(weaponModelNames, dieResult).ResultText
            };
        }

        private void GetAndMapWeaponCompanyDataToDto(ShootAndLootDto shootAndLootDto)
        {
            var weaponCompanyNames = _ultraModern5EJsonRepository.GetShootAndLootCompanyName();
            var weaponDamageTypes = _ultraModern5EJsonRepository.GetShootAndLootDamageType();
            var weaponMagazine = _ultraModern5EJsonRepository.GetShootAndLootMagazine();
            var weaponTechLevel = _ultraModern5EJsonRepository.GetShootAndLootTechLevel();

            var dieResult = RandomSelectionHelpers.RollDie(weaponCompanyNames.NumberOfDiceType, weaponCompanyNames.DiceType);

            shootAndLootDto.CompanyName = GetByDieResult(weaponCompanyNames, dieResult).ResultText;
            shootAndLootDto.TechLevel = GetByDieResult(weaponTechLevel, dieResult).ResultText;
            shootAndLootDto.Magazines = GetByDieResult(weaponMagazine, dieResult).ResultText;
            shootAndLootDto.DamageType = GetByDieResult(weaponDamageTypes, dieResult).ResultText;
            shootAndLootDto.CompanyName = GetByDieResult(weaponCompanyNames, dieResult).ResultText;
        }

        private (int numberLines, int numberModels) GetAndMapWeaponRarityDataToDto(ShootAndLootDto shootAndLootDto, int rarityModifier)
        {
            var weaponRarities = _ultraModern5EJsonRepository.GetShootAndLootWeaponRarities();
            var weaponCostMultipliers = _ultraModern5EJsonRepository.GetShootAndLootWeaponCostMultipliers();
            var weaponNumberOfLines = _ultraModern5EJsonRepository.GetShootAndLootWeaponNumberOfLines();
            var weaponNumberOfModels = _ultraModern5EJsonRepository.GetShootAndLootWeaponNumberOfModels();
            var weaponBenefits = _ultraModern5EJsonRepository.GetShootAndLootWeaponBenefits();

            var dieResult = RandomSelectionHelpers.RollDie(weaponRarities.NumberOfDiceType, weaponRarities.DiceType) + rarityModifier;

            shootAndLootDto.Rarity = GetByDieResult(weaponRarities, dieResult).ResultText;
            shootAndLootDto.CostMultiplier = GetByDieResult(weaponCostMultipliers, dieResult).ResultText;
            shootAndLootDto.Benefit = GetByDieResult(weaponBenefits, dieResult).ResultText;

            return (GetByDieResult(weaponNumberOfLines, dieResult).ResultText.ToInt(), GetByDieResult(weaponNumberOfModels, dieResult).ResultText.ToInt());
        }

        private void GetAndMapWeaponTypeDataDto(ShootAndLootDto shootAndLootDto)
        {
            var weaponType = _ultraModern5EJsonRepository.GetShootAndLootWeaponType();
            var weaponWeight = _ultraModern5EJsonRepository.GetShootAndLootWeaponWeight();
            var weaponProperties = _ultraModern5EJsonRepository.GetShootAndLootWeaponProperties();
            var weaponBaseCost = _ultraModern5EJsonRepository.GetShootAndLootWeaponBaseCost();
            var weaponDamage = _ultraModern5EJsonRepository.GetShootAndLootWeaponDamage();
            var weaponRange = _ultraModern5EJsonRepository.GetShootAndLootWeaponRange();

            var dieResult = RandomSelectionHelpers.RollDie(weaponType.NumberOfDiceType, weaponType.DiceType);

            shootAndLootDto.WeaponType = GetByDieResult(weaponType, dieResult).ResultText;
            shootAndLootDto.BaseCost = GetByDieResult(weaponBaseCost, dieResult).ResultText;
            shootAndLootDto.Properties = GetByDieResult(weaponProperties, dieResult).ResultText;
            shootAndLootDto.Weight = GetByDieResult(weaponWeight, dieResult).ResultText;
            shootAndLootDto.Damage = GetByDieResult(weaponDamage, dieResult).ResultText;
            shootAndLootDto.Range = GetByDieResult(weaponRange, dieResult).ResultText;
        }

        private static RollTableEntryDto GetByDieResult(RollTableDto rollTableDto, int rollResult)
            => rollTableDto.Entries.First(e => e.MinRoll <= rollResult && e.MaxRoll >= rollResult);
    }
}
