CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `mission_templates` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `reward_points` int NULL DEFAULT '0',
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Master catalog of system missions';

CREATE TABLE `product_types` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `roles` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `permissions` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='System roles: student and coordinator';

CREATE TABLE `supplier_types` (
    `id` int NOT NULL AUTO_INCREMENT,
    `name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `users` (
    `id` int NOT NULL AUTO_INCREMENT,
    `first_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `last_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `student_code` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `role_id` int NULL,
    `active` tinyint(1) NULL DEFAULT '1',
    `company` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `university` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_users_roles` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Educational system users';

CREATE TABLE `suppliers` (
    `id` int NOT NULL AUTO_INCREMENT,
    `supplier_type_id` int NULL,
    `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT 'active',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_suppliers_types` FOREIGN KEY (`supplier_type_id`) REFERENCES `supplier_types` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `campaigns` (
    `id` int NOT NULL AUTO_INCREMENT,
    `user_id` int NULL,
    `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `start_date` date NOT NULL,
    `end_date` date NOT NULL,
    `schedule` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `location` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `contact_number` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `budget` decimal(15,2) NULL,
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_campaigns_users` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Campaigns promoted by coordinators';

CREATE TABLE `products` (
    `id` int NOT NULL AUTO_INCREMENT,
    `supplier_id` int NULL,
    `product_type_id` int NULL,
    `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `price` decimal(12,2) NOT NULL,
    `stock` int NULL DEFAULT '0',
    `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT 'active',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_products_suppliers` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`),
    CONSTRAINT `fk_products_types` FOREIGN KEY (`product_type_id`) REFERENCES `product_types` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `accounts` (
    `id` int NOT NULL AUTO_INCREMENT,
    `user_id` int NULL,
    `account_number` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `balance` decimal(15,2) NULL DEFAULT '0.00',
    `status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT 'active',
    `campaign_id` int NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_accounts_campaigns` FOREIGN KEY (`campaign_id`) REFERENCES `campaigns` (`id`),
    CONSTRAINT `fk_accounts_users` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `coupons` (
    `id` int NOT NULL AUTO_INCREMENT,
    `campaign_id` int NULL,
    `supplier_id` int NULL,
    `code` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `discount_type` enum('percentage','fixed_amount') CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `discount_value` decimal(10,2) NOT NULL,
    `expiration_date` date NOT NULL,
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_coupons_campaigns` FOREIGN KEY (`campaign_id`) REFERENCES `campaigns` (`id`),
    CONSTRAINT `fk_coupons_suppliers` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `sales` (
    `id` int NOT NULL AUTO_INCREMENT,
    `account_id` int NULL,
    `sale_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
    `total` decimal(15,2) NOT NULL,
    `status` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT 'pending',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_sales_accounts` FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='sale_id serves as voucher code';

CREATE TABLE `transfers` (
    `id` int NOT NULL AUTO_INCREMENT,
    `source_account_id` int NULL,
    `destination_account_id` int NULL,
    `transfer_date` date NOT NULL,
    `amount` decimal(15,2) NOT NULL,
    `status` varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL DEFAULT 'pending',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_transfers_destination` FOREIGN KEY (`destination_account_id`) REFERENCES `accounts` (`id`),
    CONSTRAINT `fk_transfers_source` FOREIGN KEY (`source_account_id`) REFERENCES `accounts` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `sale_details` (
    `id` int NOT NULL AUTO_INCREMENT,
    `sale_id` int NULL,
    `product_id` int NULL,
    `quantity` int NOT NULL,
    `unit_price` decimal(12,2) NOT NULL,
    `subtotal` decimal(15,2) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_sale_details_products` FOREIGN KEY (`product_id`) REFERENCES `products` (`id`),
    CONSTRAINT `fk_sale_details_sales` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `user_missions` (
    `id` int NOT NULL AUTO_INCREMENT,
    `user_id` int NULL,
    `mission_id` int NULL,
    `completed` tinyint(1) NULL DEFAULT '0',
    `assignment_date` date NULL DEFAULT curdate(),
    `completion_date` date NULL,
    `sale_id` int NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_user_missions_sales` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`id`),
    CONSTRAINT `fk_user_missions_templates` FOREIGN KEY (`mission_id`) REFERENCES `mission_templates` (`id`),
    CONSTRAINT `fk_user_missions_users` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Missions assigned to users with their status';

CREATE TABLE `achievements` (
    `id` int NOT NULL AUTO_INCREMENT,
    `user_mission_id` int NULL,
    `name` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
    `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `achievement_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
    `active` tinyint(1) NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`id`),
    CONSTRAINT `fk_achievements_user_missions` FOREIGN KEY (`user_mission_id`) REFERENCES `user_missions` (`id`)
) CHARACTER SET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Achievements obtained by completing missions';

CREATE UNIQUE INDEX `account_number` ON `accounts` (`account_number`);

CREATE INDEX `idx_accounts_campaign` ON `accounts` (`campaign_id`);

CREATE INDEX `idx_accounts_user` ON `accounts` (`user_id`);

CREATE INDEX `fk_achievements_user_missions` ON `achievements` (`user_mission_id`);

CREATE INDEX `idx_campaigns_dates` ON `campaigns` (`start_date`, `end_date`);

CREATE INDEX `idx_campaigns_user` ON `campaigns` (`user_id`);

CREATE UNIQUE INDEX `code` ON `coupons` (`code`);

CREATE INDEX `fk_coupons_campaigns` ON `coupons` (`campaign_id`);

CREATE INDEX `fk_coupons_suppliers` ON `coupons` (`supplier_id`);

CREATE UNIQUE INDEX `code1` ON `products` (`code`);

CREATE INDEX `idx_products_supplier` ON `products` (`supplier_id`);

CREATE INDEX `idx_products_type` ON `products` (`product_type_id`);

CREATE INDEX `fk_sale_details_products` ON `sale_details` (`product_id`);

CREATE INDEX `fk_sale_details_sales` ON `sale_details` (`sale_id`);

CREATE INDEX `idx_sales_account` ON `sales` (`account_id`);

CREATE INDEX `idx_sales_date` ON `sales` (`sale_date`);

CREATE INDEX `fk_suppliers_types` ON `suppliers` (`supplier_type_id`);

CREATE INDEX `fk_transfers_destination` ON `transfers` (`destination_account_id`);

CREATE INDEX `fk_transfers_source` ON `transfers` (`source_account_id`);

CREATE INDEX `fk_user_missions_sales` ON `user_missions` (`sale_id`);

CREATE INDEX `idx_user_missions_completed` ON `user_missions` (`completed`);

CREATE INDEX `idx_user_missions_template` ON `user_missions` (`mission_id`);

CREATE INDEX `idx_user_missions_user` ON `user_missions` (`user_id`);

CREATE UNIQUE INDEX `email` ON `users` (`email`);

CREATE INDEX `idx_users_role` ON `users` (`role_id`);

CREATE UNIQUE INDEX `idx_users_student_code` ON `users` (`student_code`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251202161724_InitialCreate', '9.0.10');

COMMIT;

