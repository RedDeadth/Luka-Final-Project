-- ============================================
-- FIX: Actualizar Autenticación MySQL 9.x
-- ============================================

-- 1. Verificar el plugin de autenticación actual
SELECT user, host, plugin FROM mysql.user WHERE user = 'root';

-- 2. Cambiar a mysql_native_password (compatible con Pomelo)
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'root123';

-- 3. Aplicar cambios
FLUSH PRIVILEGES;

-- 4. Verificar que el cambio se aplicó
SELECT user, host, plugin FROM mysql.user WHERE user = 'root';

-- ============================================
-- RESULTADO ESPERADO:
-- plugin debería cambiar de 'caching_sha2_password' 
-- a 'mysql_native_password'
-- ============================================
