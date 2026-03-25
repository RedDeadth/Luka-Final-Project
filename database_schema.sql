-- =====================================================
-- LUKITAS EDUCATIONAL SYSTEM - DATABASE SCHEMA
-- Relational Server Implementation
-- =====================================================

-- Create Database
CREATE DATABASE IF NOT EXISTS lukitas_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE lukitas_db;

-- =====================================================
-- CORE TABLES
-- =====================================================

-- Roles Table
CREATE TABLE roles (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(200),
    permissions VARCHAR(500),
    active BOOLEAN DEFAULT TRUE
) ENGINE=InnoDB COMMENT='System roles: student and coordinator';

-- Users Table
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    student_code VARCHAR(20) UNIQUE,
    role_id INT,
    active BOOLEAN DEFAULT TRUE,
    company VARCHAR(100) NOT NULL,
    university VARCHAR(100) NOT NULL,
    CONSTRAINT fk_users_roles FOREIGN KEY (role_id) REFERENCES roles(id)
) ENGINE=InnoDB COMMENT='Educational system users';

-- Campaigns Table
CREATE TABLE campaigns (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    name VARCHAR(150) NOT NULL,
    description TEXT,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    schedule VARCHAR(100),
    location VARCHAR(200),
    contact_number VARCHAR(10),
    budget DECIMAL(15,2),
    active BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_campaigns_users FOREIGN KEY (user_id) REFERENCES users(id)
) ENGINE=InnoDB COMMENT='Campaigns promoted by coordinators';

-- Accounts Table
CREATE TABLE accounts (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    account_number VARCHAR(20) NOT NULL UNIQUE,
    balance DECIMAL(15,2) DEFAULT 0.00,
    status VARCHAR(10) DEFAULT 'active',
    campaign_id INT,
    CONSTRAINT fk_accounts_users FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT fk_accounts_campaigns FOREIGN KEY (campaign_id) REFERENCES campaigns(id)
) ENGINE=InnoDB;

-- Transfers Table
CREATE TABLE transfers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    source_account_id INT,
    destination_account_id INT,
    transfer_date DATE NOT NULL,
    amount DECIMAL(15,2) NOT NULL,
    status VARCHAR(15) DEFAULT 'pending',
    CONSTRAINT fk_transfers_source FOREIGN KEY (source_account_id) REFERENCES accounts(id),
    CONSTRAINT fk_transfers_destination FOREIGN KEY (destination_account_id) REFERENCES accounts(id)
) ENGINE=InnoDB;

-- =====================================================
-- SUPPLIER & PRODUCT TABLES
-- =====================================================

-- Supplier Types Table
CREATE TABLE supplier_types (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    active BOOLEAN DEFAULT TRUE
) ENGINE=InnoDB;

-- Suppliers Table
CREATE TABLE suppliers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_type_id INT,
    name VARCHAR(150) NOT NULL,
    email VARCHAR(150),
    phone VARCHAR(20),
    status VARCHAR(10) DEFAULT 'active',
    CONSTRAINT fk_suppliers_types FOREIGN KEY (supplier_type_id) REFERENCES supplier_types(id)
) ENGINE=InnoDB;

-- Product Types Table
CREATE TABLE product_types (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    active BOOLEAN DEFAULT TRUE
) ENGINE=InnoDB;

-- Products Table
CREATE TABLE products (
    id INT AUTO_INCREMENT PRIMARY KEY,
    supplier_id INT,
    product_type_id INT,
    code VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    price DECIMAL(12,2) NOT NULL,
    stock INT DEFAULT 0,
    status VARCHAR(10) DEFAULT 'active',
    CONSTRAINT fk_products_suppliers FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
    CONSTRAINT fk_products_types FOREIGN KEY (product_type_id) REFERENCES product_types(id)
) ENGINE=InnoDB;

-- Coupons Table
CREATE TABLE coupons (
    id INT AUTO_INCREMENT PRIMARY KEY,
    campaign_id INT,
    supplier_id INT,
    code VARCHAR(50) NOT NULL UNIQUE,
    discount_type ENUM('percentage', 'fixed_amount'),
    discount_value DECIMAL(10,2) NOT NULL,
    expiration_date DATE NOT NULL,
    active BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_coupons_campaigns FOREIGN KEY (campaign_id) REFERENCES campaigns(id),
    CONSTRAINT fk_coupons_suppliers FOREIGN KEY (supplier_id) REFERENCES suppliers(id)
) ENGINE=InnoDB;

-- =====================================================
-- SALES TABLES
-- =====================================================

-- Sales Table
CREATE TABLE sales (
    id INT AUTO_INCREMENT PRIMARY KEY,
    account_id INT,
    sale_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    total DECIMAL(15,2) NOT NULL,
    status VARCHAR(15) DEFAULT 'pending',
    CONSTRAINT fk_sales_accounts FOREIGN KEY (account_id) REFERENCES accounts(id)
) ENGINE=InnoDB COMMENT='sale_id serves as voucher code';

-- Sale Details Table
CREATE TABLE sale_details (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sale_id INT,
    product_id INT,
    quantity INT NOT NULL,
    unit_price DECIMAL(12,2) NOT NULL,
    subtotal DECIMAL(15,2) NOT NULL,
    CONSTRAINT fk_sale_details_sales FOREIGN KEY (sale_id) REFERENCES sales(id),
    CONSTRAINT fk_sale_details_products FOREIGN KEY (product_id) REFERENCES products(id)
) ENGINE=InnoDB;

-- =====================================================
-- GAMIFICATION TABLES
-- =====================================================

-- Mission Templates (Available mission types catalog)
CREATE TABLE mission_templates (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(255),
    reward_points INT DEFAULT 0,
    active BOOLEAN DEFAULT TRUE
) ENGINE=InnoDB COMMENT='Master catalog of system missions';

-- User Missions Assignment
CREATE TABLE user_missions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    mission_id INT,
    completed BOOLEAN DEFAULT FALSE,
    assignment_date DATE DEFAULT (CURRENT_DATE),
    completion_date DATE NULL,
    sale_id INT NULL,
    CONSTRAINT fk_user_missions_users FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT fk_user_missions_templates FOREIGN KEY (mission_id) REFERENCES mission_templates(id),
    CONSTRAINT fk_user_missions_sales FOREIGN KEY (sale_id) REFERENCES sales(id)
) ENGINE=InnoDB COMMENT='Missions assigned to users with their status';

-- Achievements Table
CREATE TABLE achievements (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_mission_id INT,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(255),
    achievement_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    active BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_achievements_user_missions FOREIGN KEY (user_mission_id) REFERENCES user_missions(id)
) ENGINE=InnoDB COMMENT='Achievements obtained by completing missions';

-- =====================================================
-- INDEXES FOR PERFORMANCE
-- =====================================================

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_student_code ON users(student_code);
CREATE INDEX idx_users_role ON users(role_id);
CREATE INDEX idx_accounts_user ON accounts(user_id);
CREATE INDEX idx_accounts_campaign ON accounts(campaign_id);
CREATE INDEX idx_accounts_number ON accounts(account_number);
CREATE INDEX idx_campaigns_user ON campaigns(user_id);
CREATE INDEX idx_campaigns_dates ON campaigns(start_date, end_date);
CREATE INDEX idx_products_supplier ON products(supplier_id);
CREATE INDEX idx_products_type ON products(product_type_id);
CREATE INDEX idx_products_code ON products(code);
CREATE INDEX idx_sales_account ON sales(account_id);
CREATE INDEX idx_sales_date ON sales(sale_date);
CREATE INDEX idx_user_missions_user ON user_missions(user_id);
CREATE INDEX idx_user_missions_template ON user_missions(mission_id);
CREATE INDEX idx_user_missions_completed ON user_missions(completed);

-- =====================================================
-- SAMPLE DATA
-- =====================================================

INSERT INTO roles (name, description, permissions) VALUES
('student', 'Student role with basic permissions', 'view_campaigns,participate,earn_lukitas'),
('coordinator', 'Coordinator role with management permissions', 'create_campaigns,manage_users,view_reports');

INSERT INTO mission_templates (name, description, reward_points, active) VALUES
('First Purchase', 'Make your first purchase in the system', 50, TRUE),
('Campaign Participation', 'Participate in a social campaign', 100, TRUE),
('Friend Referral', 'Refer a friend to join LUKA', 75, TRUE),
('Weekly Active', 'Be active for 7 consecutive days', 150, TRUE),
('Eco Warrior', 'Participate in 3 ecological campaigns', 200, TRUE);
