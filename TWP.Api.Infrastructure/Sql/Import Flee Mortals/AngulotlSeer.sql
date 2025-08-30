-- Insertion du monstre Angulotl Seer avec ses actions et traits
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
    '550e8400-e29b-41d4-a716-446655440001'::uuid,
    'Angulotl Seer',           -- name
    'Any',                      -- alignment
    '6',                        -- challenge_rating
    2300,                       -- xp
    3,                          -- initiative_bonus
    'Controller',               -- role
    'Small',                    -- creature_size
    13,                         -- armor_class
    NULL,                       -- minion_armor_class
    81,                         -- hit_points
    '18d6+18',                  -- hit_dice
    '30 ft.',                   -- speed
    '20 ft.',                   -- climb
    '30 ft.',                   -- swim
    NULL,                       -- fly
    7,                          -- strength
    17,                         -- dexterity
    12,                         -- constitution
    12,                         -- intelligence
    18,                         -- wisdom
    10,                         -- charisma
    '{"Arcana": 4, "Perception": 10, "Stealth": 6, "Survival": 7}'::jsonb,  -- skills
    'poison',                   -- damage_immunities
    'darkvision 120 ft., truesight 60 ft., passive Perception 20',  -- senses
    'Angulotl',                 -- languages
    NULL,                       -- con_saving_throw
    6,                          -- dex_saving_throw
    NULL,                       -- str_saving_throw
    7,                          -- wis_saving_throw
    NULL,                       -- cha_saving_throw
    NULL,                       -- int_saving_throw
    3,                          -- proficiency_bonus
    '["None"]'::jsonb,          -- equipments
    'Swamp, Coastal',           -- habitats
    'Humanoid',                 -- creature_type
    'Angulotl',                 -- creature_sub_type
    'Angulotl',                 -- monster_group
    'Mystical seers who use divination and toxic abilities',  -- manner
    '{"description": "Angulotl Seers are mystical amphibious humanoids with powerful divination abilities", "ecology": "Lives in swampy or coastal areas", "abilities": "Combines toxic defense with divination magic and psychedelic powers"}'::jsonb,  -- lore
    30,                         -- page_source
    'Custom Monster Manual'     -- source
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
      '550e8400-e29b-41d4-a716-446655440002',
      'Acid Grasp',
      'action',
      'Melee',
      'Melee Spell Attack: +7 to hit, reach 5 ft., one target. Hit: 14 (4d6) acid damage. If the target is a creature and they attack the seer before the start of the seer''s next turn, the target takes 14 (4d6) acid damage.',
      '5',
      NULL::VARCHAR(255),
      7::INTEGER,
      NULL::INTEGER,
      'd6',
      4::INTEGER,
      'Acid',
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440003',
      'Refulgent Beam',
      'action',
      'Ranged',
      'The seer fires two magical beams of radiant energy, each beam targeting a creature the seer can see within 120 feet of them. A beam forces a creature to make a DC 15 Dexterity saving throw or take 13 (3d8) radiant damage and shed dim light in a 10-foot radius for the target''s next turn (at end of turn). While shedding light in this way, attack rolls against the target have advantage, the target can''t benefit from being invisible, and they can''t take the Hide action.',
      '120',
      NULL::VARCHAR(255),
      NULL::INTEGER,
      NULL::INTEGER,
      'd8',
      3::INTEGER,
      'Radiant',
      NULL::INTEGER,
      false,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440004',
      'Navigate Bubble (1/Day)',
      'action',
      'Special',
      'The seer creates a 5-foot-radius sphere filled with toxic gas in an unoccupied space they can see within 60 feet of them. If undisturbed, this bubble lasts for 1 minute and dissipates harmlessly. If a creature attacks the bubble, touches it, enters its space, or otherwise disturbs it, the bubble bursts. Creatures within 15 feet of it must make a DC 15 Constitution saving throw, taking 27 (6d8) poison damage on a failed save, or half as much damage on a successful one.',
      '60',
      NULL::VARCHAR(255),
      NULL::INTEGER,
      NULL::INTEGER,
      'd8',
      6::INTEGER,
      'Poison',
      1::INTEGER,
      true,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440005',
      'Psychedelic Drone (1/Day)',
      'action',
      'Special',
      'Each enemy within 60 feet of the seer who can hear them must succeed on a DC 15 Wisdom saving throw or take 10 (3d6) psychic damage and be dazed for 1 minute (save ends at end of turn).',
      '60',
      NULL::VARCHAR(255),
      NULL::INTEGER,
      NULL::INTEGER,
      'd6',
      3::INTEGER,
      'Psychic',
      1::INTEGER,
      true,
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000),
      NULL::VARCHAR(1000)
    ),
    (
      '550e8400-e29b-41d4-a716-446655440006',
      'Hop',
      'bonus',
      'NotSpecified',
      'The seer jumps a number of feet up to their walking speed.',
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
    '550e8400-e29b-41d4-a716-446655440007',
    'Amphibious',
    'The seer can breathe air and water.',
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
    '550e8400-e29b-41d4-a716-446655440008',
    'Third Eye',
    'Creatures within 30 feet of the seer can''t hide from the seer. Creatures who can''t be targeted by divination magic are immune to this trait.',
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
    '550e8400-e29b-41d4-a716-446655440009',
    'Toxiferous',
    'When a creature hits the seer with a melee attack while within 5 feet of them or touches the seer, that creature takes 2 (1d4) poison damage.',
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