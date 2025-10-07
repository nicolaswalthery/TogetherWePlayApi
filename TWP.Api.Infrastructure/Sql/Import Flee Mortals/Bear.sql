-- ============================================
-- Insertion du monstre Bear dans PostgreSQL
-- ============================================

-- Insertion du monstre principal avec récupération de l'ID
WITH inserted_monster AS (
  INSERT INTO monsters (
    id,
    name, 
    alignment, 
    challenge_rating, 
    xp, 
    initiative_bonus,
    role, 
    creature_size, 
    armor_class, 
    minion_armor_class, 
    hit_points,
    hit_dice, 
    speed, 
    climb, 
    swim, 
    fly, 
    strength, 
    dexterity,
    constitution, 
    intelligence, 
    wisdom, 
    charisma, 
    skills,
    damage_immunities, 
    damage_resistances,
    senses, 
    languages, 
    con_saving_throw,
    dex_saving_throw, 
    str_saving_throw, 
    wis_saving_throw,
    cha_saving_throw, 
    int_saving_throw, 
    proficiency_bonus,
    equipments, 
    habitats, 
    creature_type, 
    creature_sub_type,
    monster_group, 
    manner, 
    lore, 
    page_source, 
    source
  )
  VALUES (
    'b3e4f5d8-9a2c-4e7f-8d3a-1c5e9f2b7a4d'::uuid,
    'Bear',                                     -- name
    'Unaligned',                               -- alignment
    '2',                                        -- challenge_rating
    450,                                        -- xp
    0,                                          -- initiative_bonus
    'Controller',                               -- role
    'Large',                                    -- creature_size
    11,                                         -- armor_class
    NULL,                                       -- minion_armor_class
    34,                                         -- hit_points
    '5d10+15',                                  -- hit_dice
    '40 ft.',                                   -- speed
    NULL,                                       -- climb
    '30 ft.',                                   -- swim
    NULL,                                       -- fly
    19,                                         -- strength
    10,                                         -- dexterity
    17,                                         -- constitution
    8,                                          -- intelligence
    13,                                         -- wisdom
    7,                                          -- charisma
    '{"Perception": 3, "Survival": 3}'::jsonb, -- skills
    NULL,                                       -- damage_immunities
    NULL,                                       -- damage_resistances
    'passive Perception 13',                   -- senses
    '—',                                        -- languages
    NULL,                                       -- con_saving_throw
    NULL,                                       -- dex_saving_throw
    NULL,                                       -- str_saving_throw
    NULL,                                       -- wis_saving_throw
    NULL,                                       -- cha_saving_throw
    NULL,                                       -- int_saving_throw
    2,                                          -- proficiency_bonus
    '["None"]'::jsonb,                          -- equipments
    'Forest, Mountain',                         -- habitats
    'Beast',                                    -- creature_type
    NULL,                                       -- creature_sub_type
    'Bears',                                    -- monster_group
    'Bears are recorded and feared as embodiments of nature''s primal anger. Formidable in stature and status as an apex predator.', -- manner
    '{
      "description": "Bears are recorded and feared as embodiments of nature''s primal anger. In their own world, bears vary in size and diet.",
      "ecology": "The brown bear includes the grizzly bear and black bear, the largest land predators in the world despite their bulk.",
      "society": "Bears can sprint remarkably fast, climb and swim. Giant pandas prefer bamboo, polar bears are the largest, and spectacled bears have distinctive markings.",
      "ursineVariation": "In Ursine Variation, bears take embodiments in nature''s variation with different diets and features."
    }'::jsonb,                                 -- lore
    1,                                          -- page_source
    'Custom Bestiary'                          -- source
  )
  RETURNING id
)

-- Insertion des actions
, inserted_actions AS (
  INSERT INTO actions (
    id,
    monster_id,
    name, 
    type, 
    attack_type, 
    description,
    short_range, 
    long_range, 
    attack_bonus, 
    damage_bonus,
    damage_dice, 
    number_damage_dice, 
    damage_type,
    limit_per_day, 
    is_prohibited_for_minion,
    action_trigger, 
    advantage_condition, 
    disadvantage_condition
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
    ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Multiattack', 'action', 'Special', 
     'The bear makes one Bite attack and one Claw attack.', 
     NULL::varchar, NULL::varchar, NULL::integer, NULL::integer, NULL::varchar, NULL::integer, NULL::varchar, NULL::integer, false, NULL::varchar, NULL::varchar, NULL::varchar),
    
    ('a2b3c4d5-e6f7-8901-bcde-f23456789012', 'Bite', 'action', 'Melee',
     'Melee Weapon Attack: +6 to hit, reach 5 ft., one target. Hit: 8 (1d8 + 4) piercing damage. If the target is holding a smaller, they are grappled (escape DC 14). Until this grapple ends, the bear can''t use their Bite attack on another target.',
     '5', NULL::varchar, 6, 4, 'd8', 1, 'Piercing', NULL::integer, false, NULL::varchar, NULL::varchar, NULL::varchar),
    
    ('a3b4c5d6-e7f8-9012-cdef-345678901234', 'Claw', 'action', 'Melee',
     'Melee Weapon Attack: +6 to hit, reach 5 ft., one target. Hit: 11 (2d6 + 4) slashing damage. If the target is Medium or smaller, they are knocked prone.',
     '5', NULL::varchar, 6, 4, 'd6', 2, 'Slashing', NULL::integer, false, NULL::varchar, NULL::varchar, NULL::varchar),
    
    ('a4b5c6d7-e8f9-0123-def0-456789012345', 'Powerful Jaws', 'reaction', 'Special',
     'Recharge 6: Melee Weapon Attack: +6 to hit, reach 5 ft., one target. Hit: 10 (1d12 + 4) bludgeoning damage. If the target is Medium or smaller, they must pass a DC 14 Strength saving throw or be knocked up to 10 feet away and knocked prone. If the target has a shield, the bear can choose to shatter non-becoming damage.',
     '5', NULL::varchar, 6, 4, 'd12', 1, 'Bludgeoning', NULL::integer, true, 'Recharge 6', NULL::varchar, NULL::varchar)
     
  ) AS action_data(
    id, name, type, attack_type, description, short_range, long_range,
    attack_bonus, damage_bonus, damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion, action_trigger, 
    advantage_condition, disadvantage_condition
  )
)

-- Insertion des traits
INSERT INTO traits (
  id,
  monster_id,
  title, 
  description, 
  attack_bonus,
  damage_bonus, 
  damage_dice, 
  number_damage_dice,
  damage_type, 
  is_optional, 
  trait_trigger,
  advantage_condition, 
  disadvantage_condition
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
  ('c1a2b3c4-d5e6-f789-0abc-def123456789', 'Exceptional Smell',
   'The bear has a +10 bonus to Wisdom (Perception) checks that rely on smell, and they can smell creatures even through dirt, stone, or water.',
   NULL::integer, NULL::integer, NULL::varchar, NULL::integer, NULL::varchar, false, NULL::varchar, 
   'On Wisdom (Perception) checks that rely on smell', NULL::varchar),
  
  ('c2b3c4d5-e6f7-890a-bcde-f234567890ab', 'Natural Camouflage',
   'The bear has advantage on Dexterity (Stealth) checks made to hide in forested terrain.',
   NULL::integer, NULL::integer, NULL::varchar, NULL::integer, NULL::varchar, false, NULL::varchar,
   'On Dexterity (Stealth) checks made to hide in forested terrain', NULL::varchar),
  
  ('c3c4d5e6-f7a8-901b-cdef-345678901bcd', 'Adept Climber',
   'The bear has a climbing speed of 25 feet.',
   NULL::integer, NULL::integer, NULL::varchar, NULL::integer, NULL::varchar, true, NULL::varchar, NULL::varchar, NULL::varchar),
  
  ('c4d5e6f7-a8b9-012c-def0-456789012cde', 'Arctic Traveler',
   'The bear has advantage on saving throws to avoid gaining exhaustion, can tolerate temperatures as cold as -50° Fahrenheit, and ignores difficult terrain created by ice or snow.',
   NULL::integer, NULL::integer, NULL::varchar, NULL::integer, NULL::varchar, true, NULL::varchar,
   'On saving throws to avoid gaining exhaustion', NULL::varchar),
  
  ('c5e6f7a8-b9c0-123d-ef01-567890123def', 'Bonus Action: Fearsome Growl',
   'The bear lets out a mighty growl. Each creature within 15 feet of the bear that can hear them must succeed on a DC 13 Wisdom saving throw or be Frightened of the bear until the end of the bear''s next turn. Creatures who succeed on their saving throw is immune to the Fearsome Growl of all bears for 24 hours.',
   NULL::integer, NULL::integer, NULL::varchar, NULL::integer, NULL::varchar, true, 'Bonus action', NULL::varchar, NULL::varchar)
   
) AS trait_data(
  id, title, description, attack_bonus, damage_bonus, 
  damage_dice, number_damage_dice, damage_type, is_optional, 
  trait_trigger, advantage_condition, disadvantage_condition
);

-- Note: Pas de données Symbaroum pour ce monstre