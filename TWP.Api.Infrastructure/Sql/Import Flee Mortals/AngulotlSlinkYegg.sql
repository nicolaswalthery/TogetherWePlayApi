-- ==========================================
-- INSERTION ANGULOTL SLINK (CR 2)
-- ==========================================
WITH inserted_slink AS (
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
    '550e8400-e29b-41d4-a716-446655440030'::uuid,
    'Angulotl Slink',
    'Any',
    '2',
    450,
    3,
    'Skirmisher',
    'Small',
    13,
    NULL,
    45,
    '10d6+10',
    '30 ft.',
    '30 ft.',
    '30 ft.',
    NULL,
    7,
    17,
    12,
    10,
    14,
    8,
    '{"Perception": 4, "Stealth": 7, "Survival": 4}'::jsonb,
    'poison',
    'darkvision 60 ft., passive Perception 14',
    'Angulotl',
    NULL,
    5,
    NULL,
    NULL,
    NULL,
    NULL,
    2,
    '["Poison Dagger"]'::jsonb,
    'Swamp, Coastal',
    'Humanoid',
    'Angulotl',
    'Angulotl',
    'Stealthy ambusher that uses spider climbing abilities',
    '{"description": "Angulotl Slinks are highly mobile ambushers that can climb any surface", "ecology": "Found in swamps, rivers, and rainforests", "abilities": "Combines spider climbing with poison attacks", "tactics": "Prefers to battle foes in partially aquatic environments with lots of vegetation"}'::jsonb,
    31,
    'Flee, Mortals!'
  )
  RETURNING id
)
, inserted_slink_actions AS (
  INSERT INTO actions (
    id, monster_id, name, type, attack_type, description,
    short_range, long_range, attack_bonus, damage_bonus,
    damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion,
    action_trigger, advantage_condition, disadvantage_condition
  )
  SELECT 
    action_data.id::uuid,
    (SELECT id FROM inserted_slink),
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
    ('550e8400-e29b-41d4-a716-446655440031', 'Multiattack', 'action', 'Special',
     'The slink makes one Poison Dagger attack and one Tongue Nab attack.',
     NULL::VARCHAR(255), NULL::VARCHAR(255), NULL::INTEGER, NULL::INTEGER,
     NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50), NULL::INTEGER, false,
     NULL::VARCHAR(1000), NULL::VARCHAR(1000), NULL::VARCHAR(1000)),
    ('550e8400-e29b-41d4-a716-446655440032', 'Poison Dagger', 'action', 'MeleeOrRanged',
     'Melee or Ranged Weapon Attack: +5 to hit, reach 5 ft. or range 20/60 ft., one target. Hit: 5 (1d4 + 3) piercing damage plus 10 (3d6) poison damage, and the target can''t take reactions until the start of their next turn.',
     '20', '60', 5::INTEGER, 3::INTEGER, 'd4', 1::INTEGER, 'Piercing', NULL::INTEGER, false,
     NULL::VARCHAR(1000), NULL::VARCHAR(1000), NULL::VARCHAR(1000)),
    ('550e8400-e29b-41d4-a716-446655440033', 'Tongue Nab', 'action', 'Melee',
     'Melee Weapon Attack: +5 to hit, reach 10 ft., one Medium or smaller creature. Hit: The slink can see that target. Small or Tiny object the slink can see that target is holding or carrying. The slink can catch the object if it has a free hand, otherwise it lands at the slink''s feet.',
     '10', NULL::VARCHAR(255), 5::INTEGER, NULL::INTEGER,
     NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50), NULL::INTEGER, false,
     NULL::VARCHAR(1000), NULL::VARCHAR(1000), NULL::VARCHAR(1000)),
    ('550e8400-e29b-41d4-a716-446655440034', 'Hop and Hide', 'bonus', 'NotSpecified',
     'The slink jumps a number of feet up to their walking speed, then takes the Hide action.',
     NULL::VARCHAR(255), NULL::VARCHAR(255), NULL::INTEGER, NULL::INTEGER,
     NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50), NULL::INTEGER, false,
     NULL::VARCHAR(1000), NULL::VARCHAR(1000), NULL::VARCHAR(1000))
  ) AS action_data(
    id, name, type, attack_type, description,
    short_range, long_range, attack_bonus, damage_bonus,
    damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion,
    action_trigger, advantage_condition, disadvantage_condition
  )
)
, inserted_slink_traits AS (
  INSERT INTO traits (
    id, monster_id, title, description, attack_bonus,
    damage_bonus, damage_dice, number_damage_dice,
    damage_type, is_optional, trait_trigger,
    advantage_condition, disadvantage_condition
  )
  SELECT
    trait_data.id::uuid,
    (SELECT id FROM inserted_slink),
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
    ('550e8400-e29b-41d4-a716-446655440035', 'Amphibious',
     'The slink can breathe air and water.',
     NULL::INTEGER, NULL::INTEGER, NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50),
     false, NULL::VARCHAR(500), NULL::VARCHAR(500), NULL::VARCHAR(500)),
    ('550e8400-e29b-41d4-a716-446655440036', 'Spider Climb',
     'The slink can climb difficult surfaces, including upside down on ceilings, without needing to make an ability check.',
     NULL::INTEGER, NULL::INTEGER, NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50),
     false, NULL::VARCHAR(500), NULL::VARCHAR(500), NULL::VARCHAR(500)),
    ('550e8400-e29b-41d4-a716-446655440037', 'Toxiferous',
     'When a creature hits the slink with a melee attack while within 5 feet of them or touches the slink, that creature takes 2 (1d4) poison damage.',
     NULL::INTEGER, NULL::INTEGER, 'd4', 1::INTEGER, 'Poison',
     false, 'When hit by melee attack within 5 feet or touched', NULL::VARCHAR(500), NULL::VARCHAR(500))
  ) AS trait_data(
    id, title, description, attack_bonus,
    damage_bonus, damage_dice, number_damage_dice,
    damage_type, is_optional, trait_trigger,
    advantage_condition, disadvantage_condition
  )
)

-- ==========================================
-- INSERTION ANGULOTL YEGG (CR 1/8, RETAINER)
-- ==========================================
, inserted_yegg AS (
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
    '550e8400-e29b-41d4-a716-446655440040'::uuid,
    'Angulotl Yegg',
    'Any',
    '1/8',
    25,
    2,
    'Minion',
    'Small',
    13,
    NULL,
    7,
    '2d6',
    '25 ft.',
    '20 ft.',
    '30 ft.',
    NULL,
    11,
    14,
    10,
    10,
    14,
    10,
    '{"Perception": 2, "Athletics": 0, "Stealth": 3, "Survival": 2}'::jsonb,
    'poison',
    'darkvision 60 ft., passive Perception 12',
    'Common, Angulotl',
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    '["Light Armor"]'::jsonb,
    'Swamp, Coastal',
    'Humanoid',
    'Angulotl',
    'Angulotl',
    'Young angulotl with magical potential serving as a retainer',
    '{"description": "Angulotl Yeggs are young angulotl that serve as retainers", "ecology": "Found in swamps and coastal areas", "abilities": "Angulotl value magic items that enhance their low Strength", "retainer_info": "Special retainer class with scalable abilities based on mentor''s level"}'::jsonb,
    32,
    'Flee, Mortals!'
  )
  RETURNING id
)
, inserted_yegg_actions AS (
  INSERT INTO actions (
    id, monster_id, name, type, attack_type, description,
    short_range, long_range, attack_bonus, damage_bonus,
    damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion,
    action_trigger, advantage_condition, disadvantage_condition
  )
  SELECT 
    action_data.id::uuid,
    (SELECT id FROM inserted_yegg),
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
    ('550e8400-e29b-41d4-a716-446655440041', 'Claw', 'action', 'Melee',
     'Melee Weapon Attack: +4 to hit, reach 5 ft., one target. Hit: 4 (1d4 + 2) slashing damage, and if the target is a Small or larger creature, the clawfish can pull themself into the target''s space and attach to the target. While attached, the clawfish moves with the target without provoking opportunity attacks, and the clawfish can''t make Claw attacks. A creature who can reach the clawfish can use an action to make a DC 12 Strength (Athletics) or Dexterity (Acrobatics) check. On a success, the clawfish is detached and shunted into an unoccupied space of the clawfish''s choice within 5 feet of the target. The clawfish also detaches if they become grappled, incapacitated, prone, or restrained, or if they use 5 feet of movement to detach.',
     '5', NULL::VARCHAR(255), 4::INTEGER, 2::INTEGER, 'd4', 1::INTEGER, 'Slashing', 
     NULL::INTEGER, false, NULL::VARCHAR(1000), NULL::VARCHAR(1000), NULL::VARCHAR(1000)),
    ('550e8400-e29b-41d4-a716-446655440042', 'Electroshock (Recharge 6)', 'action', 'Special',
     'Each creature touching the clawfish takes 7 (2d6) lightning damage. If the clawfish is immersed in water, each other creature within 10 feet of the clawfish who is touching the same body of water must succeed on a DC 11 Dexterity saving throw or take the same damage.',
     NULL::VARCHAR(255), NULL::VARCHAR(255), NULL::INTEGER, NULL::INTEGER, 'd6', 2::INTEGER, 'Lightning',
     NULL::INTEGER, false, 'Recharge 6', NULL::VARCHAR(1000), NULL::VARCHAR(1000))
  ) AS action_data(
    id, name, type, attack_type, description,
    short_range, long_range, attack_bonus, damage_bonus,
    damage_dice, number_damage_dice, damage_type,
    limit_per_day, is_prohibited_for_minion,
    action_trigger, advantage_condition, disadvantage_condition
  )
)
INSERT INTO traits (
  id, monster_id, title, description, attack_bonus,
  damage_bonus, damage_dice, number_damage_dice,
  damage_type, is_optional, trait_trigger,
  advantage_condition, disadvantage_condition
)
SELECT
  trait_data.id::uuid,
  (SELECT id FROM inserted_yegg),
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
  ('550e8400-e29b-41d4-a716-446655440043', 'Amphibious',
   'The angulotl yegg can breathe air and water.',
   NULL::INTEGER, NULL::INTEGER, NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50),
   false, NULL::VARCHAR(500), NULL::VARCHAR(500), NULL::VARCHAR(500)),
  ('550e8400-e29b-41d4-a716-446655440044', 'Hold Breath',
   'The clawfish can hold their breath for 15 minutes.',
   NULL::INTEGER, NULL::INTEGER, NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50),
   false, NULL::VARCHAR(500), NULL::VARCHAR(500), NULL::VARCHAR(500)),
  ('550e8400-e29b-41d4-a716-446655440045', '3rd Level: Death from Above! (3/Day)',
   'As a bonus action, the yegg jumps up to their walking speed without provoking opportunity attacks. If they leap over an enemy during this jump, the yegg can make a signature attack with advantage against that enemy. On a hit, this attack deals an extra PB piercing damage and sticks.',
   NULL::INTEGER, NULL::INTEGER, NULL::VARCHAR(20), NULL::INTEGER, 'Piercing',
   true, '3rd level retainer ability', 'When leaping over an enemy', NULL::VARCHAR(500)),
  ('550e8400-e29b-41d4-a716-446655440046', '5th Level: Lay Low (3/Day)',
   'When the yegg hits a creature with a signature attack while hidden, the yegg can use their reaction to remain hidden.',
   NULL::INTEGER, NULL::INTEGER, NULL::VARCHAR(20), NULL::INTEGER, NULL::VARCHAR(50),
   true, '5th level retainer ability', NULL::VARCHAR(500), NULL::VARCHAR(500)),
  ('550e8400-e29b-41d4-a716-446655440047', '7th Level: Excoriating Acid (3/Day)',
   'As an action, the yegg throws a flask of acid at a point they can see within 20 feet of them. The flask shatters, and each creature within 10 feet of that point must make a DC 10 plus PB Dexterity saving throw, taking PBd10 acid damage on a failed save, or half as much damage on a successful save.',
   NULL::INTEGER, NULL::INTEGER, 'd10', NULL::INTEGER, 'Acid',
   true, '7th level retainer ability', NULL::VARCHAR(500), NULL::VARCHAR(500))
) AS trait_data(
  id, title, description, attack_bonus,
  damage_bonus, damage_dice, number_damage_dice,
  damage_type, is_optional, trait_trigger,
  advantage_condition, disadvantage_condition
);