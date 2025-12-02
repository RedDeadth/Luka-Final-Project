-- ============================================
-- SCRIPT COMPLETO: Crear Base de Datos y Tablas
-- Base de datos: lukitas_db
-- ============================================

-- 1. CREAR BASE DE DATOS (si no existe)
CREATE DATABASE IF NOT EXISTS lukitas_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE lukitas_db;

-- 2. CREAR TABLA DE ROLES
CREATE TABLE IF NOT EXISTS roles (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    description VARCHAR(200),
    permissions VARCHAR(500),
    active TINYINT(1) DEFAULT 1,
    INDEX idx_roles_active (active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='System roles: student and coordinator';

-- 3. CREAR TABLA DE USUARIOS
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    student_code VARCHAR(20) UNIQUE,
    role_id INT,
    active TINYINT(1) DEFAULT 1,
    company VARCHAR(100) NOT NULL,
    university VARCHAR(100) NOT NULL,
    INDEX idx_users_email (email),
    INDEX idx_users_role (role_id),
    INDEX idx_users_student_code (student_code),
    CONSTRAINT fk_users_roles FOREIGN KEY (role_id) REFERENCES roles(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Educational system users';

-- 4. INSERTAR ROLES
INSERT INTO roles (id, name, description, permissions, active) VALUES
(1, 'student', 'Estudiante del sistema educativo', 'view_campaigns,participate_missions,view_products', 1),
(2, 'coordinator', 'Coordinador de campañas', 'create_campaigns,manage_users,view_reports,manage_products', 1)
ON DUPLICATE KEY UPDATE name=VALUES(name);

-- 5. INSERTAR USUARIOS DE PRUEBA
INSERT INTO users (id, first_name, last_name, email, password, student_code, role_id, active, company, university) VALUES
(1, 'Juan', 'Pérez', 'student@test.com', 'password123', 'STU001', 1, 1, 'TechCorp', 'Universidad Nacional'),
(2, 'María', 'González', 'coordinator@test.com', 'admin123', NULL, 2, 1, 'EduManage', 'Universidad Nacional'),
(3, 'Carlos', 'Rodríguez', 'student2@test.com', 'test456', 'STU002', 1, 1, 'StartupXYZ', 'Universidad Tecnológica'),
(4, 'Ana', 'Martínez', 'coordinator2@test.com', 'coord456', NULL, 2, 1, 'CampusHub', 'Universidad Tecnológica')
ON DUPLICATE KEY UPDATE email=VALUES(email);

-- 6. VERIFICAR DATOS INSERTADOS
SELECT 
    u.id, 
    u.first_name, 
    u.last_name, 
    u.email, 
    r.name as role, 
    u.active, 
    u.company, 
    u.university
FROM users u
LEFT JOIN roles r ON u.role_id = r.id
ORDER BY u.id;

-- ============================================
-- RESULTADO ESPERADO:
-- 4 usuarios creados (2 students, 2 coordinators)
-- ============================================
