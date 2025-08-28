-- Insertion du monstre Clawfish Companion avec ses actions et traits
-- Note: Les colonnes nullable doivent être explicitement castées pour PostgreSQL
-- IMPORTANT: shortRange et longRange sont de type VARCHAR(255), pas INTEGER!
-- Note: Ce monstre est un Companion dont les stats évoluent avec le niveau du caregiver

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
    '550e8400-e29b-41d4-a716-446655440050'::uuid,
    'Clawfish Companion',       -- name
    'Unaligned',                -- alignment
    '0',                        -- challenge_rating (companion scaling)
    10,                         -- xp
    1,                          -- initiative_bonus
    'Elite',                    -- role (companion)
    'Small',                    -- creature_size
    13,                         -- armor_class (plus PB in actual play)
    NULL,                       -- minion_armor_class
    7,                          -- hit_points (base, multiplied by caregiver level)
    '1d8',                      -- hit_dice (scales with caregiver)
    '30 ft.',                   -- speed
    '30 ft.',                   -- climb
    '40 ft.',                   -- swim
    NULL,                       -- fly
    16,                         -- strength
    13,                         -- dexterity
    12,                         -- constitution
    4,                          -- intelligence
    10,                         -- wisdom
    5,                          -- charisma
    '{"Perception": 0, "Stealth": 1}'::jsonb,  -- skills (plus PB in play)
    NULL,                       -- damage_immunities
    'passive Perception 10',    -- senses (plus PB in play)
    '—',                        -- languages
    NULL,                       -- con_saving_throw
    1,                          -- dex_saving_throw (plus PB in play)
    3,                          -- str_saving_throw (plus PB in play)
    NULL,                       -- wis_saving_throw
    NULL,                       -- cha_saving_throw
    NULL,                       -- int_saving_throw
    NULL,                       -- proficiency_bonus (equals caregiver's)
    '["None"]'::jsonb,          -- equipments
    'Coastal, Underwater, River',  -- habitats
    'Beast',                    -- creature_type
    NULL,                       -- creature_sub_type
    'Companion',                -- monster_group
    'Loyal beast companion with electrical abilities',  -- manner
    '{"description": "Clawfish are aquatic beasts that form bonds with caregivers", "ecology": "Found in rivers and coastal waters", "abilities": "Can generate electrical attacks and breathe underwater", "companion_info": "Scales with caregiver''s level, HP = 7 × caregiver level"}'::jsonb,  -- lore
    33,                         -- page_source
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
      '550e8400-e29b-41d4-a716-446655440051',
      'Signature Attack (Bite)',
      'action',
      'Melee',
      'Melee Weapon Attack: +3 plus PB to hit, reach 5 ft., one target. Hit: 1d6 plus PB piercing damage.',
      '5',
      NULL::VARCHAR(255),
      3::INTEGER,
      NULL::INTEGER,
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
      '550e8400-e29b-41d4-a716-446655440052',
      'Lightning Retaliation',
      'reaction',
      'Special',
      'Lightning Retaliation (Recharges after a Short or Long Rest). When the clawfish or their caregiver is attacked by a creature the clawfish can see within 5 feet of them, the clawfish shocks the attacker. The attacker must make a DC 10 plus PB Dexterity saving throw, taking PBd6 lightning damage on a failed save, or half as much damage on a successful one.',
      '5',
      NULL::VARCHAR(255),
      NULL::INTEGER,
      NULL::INTEGER,
      'd6',
      NULL::INTEGER,
      'Lightning',
      NULL::INTEGER,
      false,
      'When clawfish or caregiver is attacked within 5 feet',
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
    '550e8400-e29b-41d4-a716-446655440053',
    'Hold Breath',
    'The clawfish can hold their breath for 15 minutes.',
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
    '550e8400-e29b-41d4-a716-446655440054',
    '1st Level: Overwhelming Attack (2 Ferocity)',
    'The clawfish makes a signature attack. On a hit, the attack deals an extra PB lightning damage, and the target can''t take reactions until the start of the clawfish''s next turn.',
    NULL::INTEGER,
    NULL::INTEGER,
    NULL::VARCHAR(20),
    NULL::INTEGER,
    'Lightning',
    true,
    'Companion feature level 1',
    NULL::VARCHAR(500),
    NULL::VARCHAR(500)
  ),
  (
    '550e8400-e29b-41d4-a716-446655440055',
    '3rd Level: Coiling Claws (2 Ferocity)',
    'The clawfish makes a signature attack against an attacker or smaller creature. On a hit, the target is grappled (escape DC 10 plus PB). Until this grapple ends, the target is restrained and the clawfish can''t make a signature attack against another target.',
    NULL::INTEGER,
    NULL::INTEGER,
    NULL::VARCHAR(20),
    NULL::INTEGER,
    NULL::VARCHAR(50),
    true,
    'Companion feature level 3',
    NULL::VARCHAR(500),
    NULL::VARCHAR(500)
  ),
  (
    '550e8400-e29b-41d4-a716-446655440056',
    '5th Level: Lightning Bomb (8 Ferocity)',
    'Each creature within 10 feet of the clawfish must make a DC 10 plus PB Dexterity saving throw, taking PBd8 lightning damage on a failed save, or half as much damage on a successful one.',
    NULL::INTEGER,
    NULL::INTEGER,
    'd8',
    NULL::INTEGER,
    'Lightning',
    true,
    'Companion feature level 5',
    NULL::VARCHAR(500),
    NULL::VARCHAR(500)
  ),
  (
    '550e8400-e29b-41d4-a716-446655440057',
    'Mystic Connection: Riverkith',
    'You gain a swimming speed equal to your walking speed, and you can hold your breath for up to 15 minutes. Additionally, as a bonus action, you can breathe lightning in a 30-foot cone. Each creature in the cone must make a Dexterity saving throw against your exploit save DC, taking 6d6 lightning damage on a failed save, or half as much damage on a successful one. Once you use this bonus action, you can''t do so again until you finish a long rest.',
    NULL::INTEGER,
    NULL::INTEGER,
    'd6',
    6::INTEGER,
    'Lightning',
    true,
    'Beastheart Mystic Connection feature',
    NULL::VARCHAR(500),
    NULL::VARCHAR(500)
  )
) AS trait_data(
  id, title, description, attack_bonus,
  damage_bonus, damage_dice, number_damage_dice,
  damage_type, is_optional, trait_trigger,
  advantage_condition, disadvantage_condition
);