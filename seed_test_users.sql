-- Script para insertar datos de prueba en lukitas_db
-- Roles: student y coordinator
-- Contraseñas en texto plano (según AuthService.cs línea 105)

-- 1. Insertar roles
INSERT INTO roles (id, name, description, permissions, active) VALUES
(1, 'student', 'Estudiante del sistema educativo', 'view_campaigns,participate_missions,view_products', 1),
(2, 'coordinator', 'Coordinador de campañas', 'create_campaigns,manage_users,view_reports,manage_products', 1)
ON DUPLICATE KEY UPDATE name=VALUES(name);

-- 2. Insertar usuarios de prueba
INSERT INTO users (id, first_name, last_name, email, password, student_code, role_id, active, company, university) VALUES
(1, 'Juan', 'Pérez', 'student@test.com', 'password123', 'STU001', 1, 1, 'TechCorp', 'Universidad Nacional'),
(2, 'María', 'González', 'coordinator@test.com', 'admin123', NULL, 2, 1, 'EduManage', 'Universidad Nacional'),
(3, 'Carlos', 'Rodríguez', 'student2@test.com', 'test456', 'STU002', 1, 1, 'StartupXYZ', 'Universidad Tecnológica'),
(4, 'Ana', 'Martínez', 'coordinator2@test.com', 'coord456', NULL, 2, 1, 'CampusHub', 'Universidad Tecnológica')
ON DUPLICATE KEY UPDATE email=VALUES(email);

-- Verificar datos insertados
SELECT u.id, u.first_name, u.last_name, u.email, r.name as role, u.active, u.company, u.university
FROM users u
LEFT JOIN roles r ON u.role_id = r.id
WHERE u.email IN ('student@test.com', 'coordinator@test.com', 'student2@test.com', 'coordinator2@test.com');
