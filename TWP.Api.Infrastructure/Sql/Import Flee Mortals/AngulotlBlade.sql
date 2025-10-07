-- Insertion du monstre Angulotl Blade avec ses actions et traits
-- Note: Les colonnes nullable doivent être explicitement castées pour PostgreSQL
-- IMPORTANT: shortRange et longRange sont de type VARCHAR(255), pas INTEGER!

WITH inserted_monster AS (
  INSERT INTO monsters (
    id, name, alignment, challenge_rating, xp, initiative_bonus,
    role, creature_size, armor_class, minion_armor_class, hit_points,
    hit_dice, speed, climb, swim, fly, strength, dexterity,
    constitution, intelligence, wisdom, charisma, skills,
    damage_immunities, senses, languages, con_saving_throw,
    dex_saving_throw, str_saving_throw, wis_saving_throw,
    cha_saving_throw, int_saving_throw, proficiency_bonus,
    equipments, habitats, creature_type, creature_sub_type,
    monster_group, manner, lore, page_source, source
  )
  VALUES (
    '550e8400-e29b-41d4-a716-446655440010'::uuid,
    'Angulotl Blade',           -- name
    'Any',                      -- alignment
    '1/4',                      -- challenge_rating
    50,                         -- xp
    2,                          -- initiative_bonus
    'Skirmisher',               -- role
    'Small',                    -- creature_size
    13,                         -- armor_class
    NULL,                       -- minion_armor_class
    14,                         -- hit_points
    '4d6',                      -- hit_dice
    '20 ft.',                   -- speed
    '20 ft.',                   -- climb
    '30 ft.',                   -- swim
    NULL,                       -- fly
    7,                          -- strength
    15,                         -- dexterity
    11,                         -- constitution
    10,                         -- intelligence
    14,                         -- wisdom
    8,                          -- charisma
    '{"Perception": 4, "Stealth": 4}'::jsonb,  -- skills
    'poison',                   -- damage_immunities
    'darkvision 60 ft., passive Perception 14',  -- senses
    'Angulotl',                 -- languages
    NULL,                       -- con_saving_throw
    NULL,                       -- dex_saving_throw
    NULL,                       -- str_saving_throw
    NULL,                       -- wis_saving_throw
    NULL,                       -- cha_saving_throw
    NULL,                       -- int_saving_throw
    2,                          -- proficiency_bonus
    '["Machete"]'::jsonb,       -- equipments
    'Swamp, Coastal',           -- habitats
    'Humanoid',                 -- creature_type
    'Angulotl',                 -- creature_sub_type
    'Angulotl',                 -- monster_group
    'Aggressive and opportunistic ambush predator',  -- manner
    '{"description": "Angulotl Blades are poisonous creatures skilled in ambush tactics", "ecology": "Found in freshwater swamps, rivers, and rainforests", "society": "Prefer to avoid combat unless they outnumber their targets", "abilities": "Deadly toxins and slippery nature make them difficult to engage"}'::jsonb,  -- lore
    28,                         -- page_source
    'Flee, Mortals!'            -- source
  )
  RETURNING id
)
-- Insertion des actions
, inserted_actions AS (
  INSERT INTO actions (
    id, monster_id, name, type, attack_type, description,
    short_range, long_range, attack_bonus, damage_bonus,
    damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion,
    action_trigger, advantage_condition, disadvantage_condition
  )
  SELECT 
    action_data.id::uuid,
    (SELECT id FROM inserted_monster),
    action_data.name,
    action_data.type,
    action_data.attack_type,
    action_data.description,
    action_data.short_range,
    action_data.long_range,
    action_data.attack_bonus,
    action_data.damage_bonus,
    action_data.damage_dice,
    action_data.number_damage_dice,
    action_data.damage_type,
    action_data.limit_per_day,
    action_data.is_prohibited_for_minion,
    action_data.action_trigger,
    action_data.advantage_condition,
    action_data.disadvantage_condition
  FROM (VALUES
    (
      '550e8400-e29b-41d4-a716-446655440011',
      'Machete',
      'action',
      'Melee',
      'Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 5 (1d6 + 2) slashing damage.',
      '5',
      NULL::VARCHAR(255),
      4::INTEGER,
      2::INTEGER,
      'd6',
      1::INTEGER,
      'Slashing',
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440012',
      'Dart',
      'action',
      'Ranged',
      'Ranged Weapon Attack: +4 to hit, range 20/60 ft., one target. Hit: 4 (1d4 + 2) piercing damage.',
      '20',
      '60',
      4::INTEGER,
      2::INTEGER,
      'd4',
      1::INTEGER,
      'Piercing',
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440013',
      'Wild Hop',
      'bonus',
      'NotSpecified',
      'The blade jumps a number of feet up to their walking speed. If the blade uses this bonus action to jump at least 10 feet straight toward a creature, the next Machete attack the blade makes against that creature on the same turn is made with advantage.',
      NULL::VARCHAR(255),
      NULL::VARCHAR(255),
      NULL::INTEGER,
      NULL::INTEGER,
      NULL::VARCHAR(20),
      NULL::INTEGER,
      NULL::VARCHAR(50),
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      'Next Machete attack if jumping at least 10 feet straight toward target',
      NULL::VARCHAR(1000)
    )
  ) AS action_data(
    id, name, type, attack_type, description,
    short_range, long_range, attack_bonus, damage_bonus,
    damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion,
    action_trigger, advantage_condition, disadvantage_condition
  )
)
-- Insertion des traits
INSERT INTO traits (
  id, monster_id, title, description, attack_bonus,
  damage_bonus, damage_dice, number_damage_dice,
  damage_type, is_optional, trait_trigger,
  advantage_condition, disadvantage_condition
)
SELECT
  trait_data.id::uuid,
  (SELECT id FROM inserted_monster),
  trait_data.title,
  trait_data.description,
  trait_data.attack_bonus,
  trait_data.damage_bonus,
  trait_data.damage_dice,
  trait_data.number_damage_dice,
  trait_data.damage_type,
  trait_data.is_optional,
  trait_data.trait_trigger,
  trait_data.advantage_condition,
  trait_data.disadvantage_condition
FROM (VALUES
  (
    '550e8400-e29b-41d4-a716-446655440014',
    'Amphibious',
    'The blade can breathe air and water.',
    NULL::INTEGER,
    NULL::INTEGER,
    NULL::VARCHAR(20),
    NULL::INTEGER,
    NULL::VARCHAR(50),
    false,
    NULL::VARCHAR(500),
    NULL::VARCHAR(500),
    NULL::VARCHAR(500)
  ),
  (
    '550e8400-e29b-41d4-a716-446655440015',
    'Slippery',
    'Opportunity attacks against the blade are made with disadvantage.',
    NULL::INTEGER,
    NULL::INTEGER,
    NULL::VARCHAR(20),
    NULL::INTEGER,
    NULL::VARCHAR(50),
    false,
    NULL::VARCHAR(500),
    NULL::VARCHAR(500),
    'Opportunity attacks against the blade'
  ),
  (
    '550e8400-e29b-41d4-a716-446655440016',
    'Toxiferous',
    'When a creature hits the blade with a melee attack while within 5 feet of them or touches the blade, that creature takes 2 (1d4) poison damage.',
    NULL::INTEGER,
    NULL::INTEGER,
    'd4',
    1::INTEGER,
    'Poison',
    false,
    'When hit by melee attack within 5 feet or touched',
    NULL::VARCHAR(500),
    NULL::VARCHAR(500)
  )
) AS trait_data(
  id, title, description, attack_bonus,
  damage_bonus, damage_dice, number_damage_dice,
  damage_type, is_optional, trait_trigger,
  advantage_condition, disadvantage_condition
);