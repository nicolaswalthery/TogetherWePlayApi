namespace TWP.Api.Application.Prompts
{
    public static class UM5ePrompts
    {
        /// <summary>
        /// Prompt that analyze narrative to determine appropriate CR and suggested roles
        /// </summary>
        /// <param name="narrativeDescription">Darrative description given by the user to be used by the LLM to adapte the lore of a creature to build its UM5e conterpart.</param>
        /// <returns></returns>
        public static string GetAnalyzeNarrativeToDetermineAppropriateCrAndSuggestedRolePrompt(this string narrativeDescription)
            => $@"Analyze this sci-fi adversary description and provide a JSON response with the following structure:
                    {{
                        ""suggestedCR"": <number between 0.125 and 30>,
                        ""threatLevel"": ""<low/medium/high/extreme>"",
                        ""suggestedRoles"": [""<role1>"", ""<role2>""],
                        ""keyTraits"": [""<trait1>"", ""<trait2>""],
                        ""weaponType"": ""<energy/projectile/melee/mixed>"",
                        ""techLevel"": <0, 1, 2, 3, 4, or 5>
                    }}

                    Tech Level Guidelines:
                        - TL0: This level covers the entirety of civilized history until 
                                the early industrial era, stopping before the harnessing of 
                                electric power, everything from the discovery of the wheel 
                                to its use in manufacturing. 
                                Vehicles. Gliders or basic aeroforms. Both ground and 
                                aircraft are limited to archaic steam power. 
                                Weapons. All weapons rely on steam or chemical 
                                propellants with simple loading mechanisms. The blunderbuss 
                                and musket are examples. 
                                Medical. Natural healing. TL0 benefi ts more from 
                                discovered human knowledge about biology than the tools 
                                that were developed consequently. Surgery can cure most 
                                wounds, but recovery can last a while.
                                 Similarity. Up to the mid-18th century.
                        - TL1: 
                             At this level, machines come into their own. Internal 
                            combustion and steam power have been perfected. Electric 
                            power and road vehicles are changing the way cities are built. 
                            Vehicles. Ground vehicles are run off steam or internal 
                            combustion. Electrical power is in its infancy. The fact they 
                            are mass-produced is the real achievement. Aircraft are 
                            flown by manual controls and receive propulsion from 
                            propellers.  Weapons. Bolt action rifl es and revolvers. Cartridge
                            fed fi rearms are becoming more common. 
                            Medical. The implementation of the scientifi c method 
                            and laboratory research has resulted in vaccines. Drugs are 
                            becoming commonplace.
                             Similarity. 19th to early 20th Century.

                        - TL2: 
                            At this level, almost every form of technology has integrated 
                            electronics and advanced computer control. Electrifi cation is 
                            now commonplace, though computers have yet to dominate 
                            civilization.
                             Vehicles. Ground vehicles now have electronics; some 
                            offer climate control. Aircraft now possess fl y-by-wire, 
                            vectored thrust, and vertical-take-off capacity. 
                            Weapons. Computer tracking and targeting. Infrared 
                            and thermal imaging is available, but not standard. Firearms 
                            haven’t changed but have grown more complicated with 
                            advanced reloading and higher fi ring rates. Advances in 
                            construction make them lighter with larger calibers. 
                            Medical. Computer diagnostic beds, MRIs, and X-Rays.
                             Similarity. Mid-late 20th century.
                        - TL3: 
                            Refinements in the manipulation of magnetic fi elds and 
                            energy levels characterize this stage. Computers now 
                            control most of civilization and link citizens together.
                            Vehicles. Vertical take-off fan craft and wingless jets 
                            keep aircraft aloft, are much more stable, and can fl y rings 
                            around more primitive craft. Aircraft designs are no longer 
                            dominated by their massive aeroforms. Ground vehicles still 
                            use wheels, but now mass transit magnetic vehicles appear 
                            as an alternative. 
                            Weapons. There will always be bullets, but the rise 
                            of both railcannons and self-propelled projectiles offer 
                            alternatives. Laser weaponry in its infancy. Advanced 
                            magnetics. Prototype exo-armor appears. 
                            Medical. Most known diseases are curable. Healing 
                            time cut to one-third with medical attention. Nanotech 
                            healing isolated in the laboratory.
                             Similarity. Early-mid 21st century.

                        - TL4: 
                            At this level, alternate energy and advanced in nuclear 
                            power has created an energy surplus. Nanotechnology is 
                            ubiquitous. Consumer space travel is now frequent.
                             Vehicles. Robots appear beyond the role of “dumb 
                            tool.” Exo-armor is mass-produced. Wheeled traffi c 
                            virtually nonexistent or, if it exists, can traverse any terrain. 
                            Ramjets shrink and provide massive thrust in small packages, 
                            revolutionizing transportation outside of magnetic-traffi c. 
                            Weapons. Laser weapons “tunable.” Plasma weaponry. 
                            Bolt weapons are outdated.
                            Medical. Nanotechnology can heal any wounds and 
                            even regenerate limbs.
                        - TL5: 
                            Any sufficiently advanced technology would be indistinguishable from magic.
                             Vehicles. Common antigravity replaces all previous 
                            transportation. 
                            Weapons. Disruptors, vapor rifl es, disintegrator 
                            weaponry. 
                            Medical. Complete body reconstruction.

                            Application of tech levels
                                 The tech level can affect the diffi culty and cost of crafting, 
                                repairing, and modifying technology. It can also change its 
                                rarity.
                                 TL 0 and TL 1. Common. All items with no listed TL 
                                are TL0.
                                TL 2. Uncommon
                                 TL 3. Rare
                                 TL 4. Very Rare
                                 TL 5. Legendary
                                 If setting a game at a higher TL, you can shift the rarity 
                                down to make items more common. Certain items (like 
                                exo-armor) may be rarer than their listed tech level. They 
                                may also count as multiple items. Tech levels can also apply 
                                in other ways depending on the device in question. See the 
                                item descriptions for details. 

                    Description: {narrativeDescription}

                    Base your CR suggestion on:
                    - Low (CR 0.125-4): Basic troops, drones, minor threats
                    - Medium (CR 5-10): Elite units, specialists, significant threats
                    - High (CR 11-16): Commanders, heavy units, major threats
                    - Extreme (CR 17+): Legendary units, boss-level threats

                    Base CR on both threat description and tech level.
                    Valid roles: Brute, Soldier, Controller, Skirmisher, Ambusher, Artillery, Minion, Solo, Support, Leader";
    }
}
