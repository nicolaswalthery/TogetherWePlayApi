-- Insertion du monstre Angulotl Needler avec ses actions et traits
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
    '550e8400-e29b-41d4-a716-446655440020'::uuid,
    'Angulotl Needler',         -- name
    'Any',                      -- alignment
    '4',                        -- challenge_rating
    1100,                       -- xp
    4,                          -- initiative_bonus
    'Artillery',                -- role
    'Small',                    -- creature_size
    14,                         -- armor_class
    NULL,                       -- minion_armor_class
    54,                         -- hit_points
    '12d6+12',                  -- hit_dice
    '20 ft.',                   -- speed
    '20 ft.',                   -- climb
    '30 ft.',                   -- swim
    NULL,                       -- fly
    7,                          -- strength
    18,                         -- dexterity
    12,                         -- constitution
    11,                         -- intelligence
    16,                         -- wisdom
    8,                          -- charisma
    '{"Acrobatics": 6, "Perception": 5, "Stealth": 8, "Survival": 5}'::jsonb,  -- skills
    'poison',                   -- damage_immunities
    'darkvision 60 ft., passive Perception 15',  -- senses
    'Angulotl',                 -- languages
    NULL,                       -- con_saving_throw
    6,                          -- dex_saving_throw
    NULL,                       -- str_saving_throw
    NULL,                       -- wis_saving_throw
    NULL,                       -- cha_saving_throw
    NULL,                       -- int_saving_throw
    2,                          -- proficiency_bonus
    '["Blowgun", "Darts"]'::jsonb,  -- equipments
    'Swamp, Coastal',           -- habitats
    'Humanoid',                 -- creature_type
    'Angulotl',                 -- creature_sub_type
    'Angulotl',                 -- monster_group
    'Stealthy sniper that strikes from hiding',  -- manner
    '{"description": "Angulotl Needlers are expert marksmen who use poisoned darts", "ecology": "Found in freshwater swamps, rivers, and rainforests", "society": "They like humans and adults tend to freak them out", "abilities": "Master snipers with deadly poison attacks"}'::jsonb,  -- lore
    29,                         -- page_source
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
      '550e8400-e29b-41d4-a716-446655440021',
      'Dart',
      'action',
      'Melee',
      'Melee Weapon Attack: +6 to hit, reach 5 ft., one target. Hit: 7 (1d6 + 4) piercing damage plus 7 (2d6) poison damage.',
      '5',
      NULL::VARCHAR(255),
      6::INTEGER,
      4::INTEGER,
      'd6',
      1::INTEGER,
      'Piercing',
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440022',
      'Blowgun',
      'action',
      'Ranged',
      'Ranged Weapon Attack: +6 to hit, range 25/100 ft., one target. Hit: 1 piercing damage, and the target must succeed on a DC 13 Constitution saving throw or be poisoned for 1 minute (save ends at end of turn). While poisoned in this way, the target takes 14 (4d6) poison damage at the start of each of their turns.',
      '25',
      '100',
      6::INTEGER,
      0::INTEGER,
      NULL::VARCHAR(20),
      NULL::INTEGER,
      'Piercing',
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440023',
      'Hop and Hide',
      'bonus',
      'NotSpecified',
      'The needler jumps a number of feet up to their walking speed, then takes the Hide action.',
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
      NULL::VARCHAR(1000),
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
    '550e8400-e29b-41d4-a716-446655440024',
    'Amphibious',
    'The needler can breathe air and water.',
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
    '550e8400-e29b-41d4-a716-446655440025',
    'Sniper',
    'If the needler misses with a ranged weapon attack while they are hidden, they remain hidden. Additionally, if the needler makes a ranged weapon attack with advantage and hits, the attack deals an extra 3 (1d6) damage.',
    NULL::INTEGER,
    3::INTEGER,
    'd6',
    1::INTEGER,
    NULL::VARCHAR(50),
    false,
    'When making ranged attack with advantage',
    'Ranged weapon attacks when hidden',
    NULL::VARCHAR(500)
  ),
  (
    '550e8400-e29b-41d4-a716-446655440026',
    'Toxiferous',
    'When a creature hits the needler with a melee attack while within 5 feet of them or touches the needler, that creature takes 2 (1d4) poison damage.',
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