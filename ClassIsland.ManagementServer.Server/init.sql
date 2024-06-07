use classisland_management_dev;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `client_groups` (
    `group_id` int NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`group_id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `policies` (
    `id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `is_enabled` tinyint(1) NULL,
    `content` json NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile_groups` (
    `id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `settings` (
    `id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `settings` json NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `clients` (
    `cuid` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `id` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `register_time` datetime NULL,
    `group_id` int NULL,
    `policy_version` int NULL DEFAULT '0',
    `timeLayout_version` int NULL DEFAULT '0',
    `subjects_version` int NULL DEFAULT '0',
    `defaultSettings_version` int NULL DEFAULT '0',
    `classplan_version` int NULL DEFAULT '0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`cuid`),
    CONSTRAINT `fk_clients_client_groups_1` FOREIGN KEY (`group_id`) REFERENCES `client_groups` (`group_id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile_subjects` (
    `id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `group_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `initials` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `is_out_door` tinyint(1) NULL DEFAULT '0',
    `attached_objects` json NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_profile_subjects_profile_groups_1` FOREIGN KEY (`group_id`) REFERENCES `profile_groups` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile_timelayouts` (
    `id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `group_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `attached_objects` json NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_profile_timelayouts_profile_groups_1` FOREIGN KEY (`group_id`) REFERENCES `profile_groups` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `object_updates` (
    `id` int NOT NULL AUTO_INCREMENT,
    `object_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `object_type` int NULL,
    `target_cuid` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `update_time` datetime NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_object_updates_clients_1` FOREIGN KEY (`target_cuid`) REFERENCES `clients` (`cuid`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `objects_assignees` (
    `id` int NOT NULL AUTO_INCREMENT,
    `object_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `object_type` int NULL,
    `target_client_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `target_client_cuid` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `target_group_id` int NULL,
    `assignee_type` int NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_objects_assignees_client_groups_1` FOREIGN KEY (`target_group_id`) REFERENCES `client_groups` (`group_id`),
    CONSTRAINT `fk_objects_assignees_clients_1` FOREIGN KEY (`target_client_cuid`) REFERENCES `clients` (`cuid`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile_classplans` (
    `id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `group_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `week_day` int NULL DEFAULT '0',
    `week_div` int NULL DEFAULT '0',
    `time_layout_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `is_enabled` tinyint(1) NULL,
    `attached_objects` json NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_profile_classplans_profile_groups_1` FOREIGN KEY (`group_id`) REFERENCES `profile_groups` (`id`),
    CONSTRAINT `fk_profile_classplans_profile_timelayouts_1` FOREIGN KEY (`time_layout_id`) REFERENCES `profile_timelayouts` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile_timelayout_timepoint` (
    `internal_id` int NOT NULL AUTO_INCREMENT,
    `parent_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `index` int NULL,
    `start` time NULL,
    `end` time NULL,
    `time_type` int NULL,
    `default_subject_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `attached_objects` json NULL,
    `is_hide_default` tinyint(1) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`internal_id`),
    CONSTRAINT `fk_profile_timelayout_timepoint_profile_timelayouts_1` FOREIGN KEY (`parent_id`) REFERENCES `profile_timelayouts` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `profile_classplan_classes` (
    `internal_id` int NOT NULL AUTO_INCREMENT,
    `index` int NULL,
    `parent_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `subject_id` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
    `attached_objects` json NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`internal_id`),
    CONSTRAINT `fk_profile_classplan_classes_profile_classplans_1` FOREIGN KEY (`parent_id`) REFERENCES `profile_classplans` (`id`),
    CONSTRAINT `fk_profile_classplan_classes_profile_subjects_1` FOREIGN KEY (`subject_id`) REFERENCES `profile_subjects` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE INDEX `fk_clients_client_groups_1` ON `clients` (`group_id`);

CREATE INDEX `fk_object_updates_clients_1` ON `object_updates` (`target_cuid`);

CREATE INDEX `fk_object_updates_settings_1` ON `object_updates` (`object_id`);

CREATE INDEX `fk_objects_assignees_client_groups_1` ON `objects_assignees` (`target_group_id`);

CREATE INDEX `fk_objects_assignees_clients_1` ON `objects_assignees` (`target_client_cuid`);

CREATE INDEX `fk_objects_assignees_profile_groups_1` ON `objects_assignees` (`object_id`);

CREATE INDEX `fk_profile_classplan_classes_profile_classplans_1` ON `profile_classplan_classes` (`parent_id`);

CREATE INDEX `fk_profile_classplan_classes_profile_subjects_1` ON `profile_classplan_classes` (`subject_id`);

CREATE INDEX `fk_profile_classplans_profile_groups_1` ON `profile_classplans` (`group_id`);

CREATE INDEX `fk_profile_classplans_profile_timelayouts_1` ON `profile_classplans` (`time_layout_id`);

CREATE INDEX `fk_profile_subjects_profile_groups_1` ON `profile_subjects` (`group_id`);

CREATE INDEX `fk_profile_timelayout_timepoint_profile_timelayouts_1` ON `profile_timelayout_timepoint` (`parent_id`);

CREATE INDEX `fk_profile_timelayouts_profile_groups_1` ON `profile_timelayouts` (`group_id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240601134828_Initial', '8.0.2');

COMMIT;


