# 🚀 Guía de Deployment en Render

## 📋 Requisitos Previos

1. **Cuenta en Render**: [https://render.com](https://render.com) (plan gratuito disponible)
2. **Repositorio en GitHub**: Tu código debe estar en GitHub
3. **Archivos preparados**:
   - ✅ Dockerfile
   - ✅ .dockerignore
   - ✅ render.yaml
   - ✅ appsettings.Production.json

---

## 🎯 Opción 1: Deployment Automático con render.yaml (RECOMENDADO)

### Paso 1: Preparar el Repositorio

```bash
# Asegúrate de tener todos los cambios commiteados
git add .
git commit -m "feat: Add Render deployment configuration"
git push origin main
```

### Paso 2: Conectar Render con GitHub

1. Ve a [https://dashboard.render.com](https://dashboard.render.com)
2. Click en **"New +"** → **"Blueprint"**
3. Conecta tu cuenta de GitHub si aún no lo has hecho
4. Selecciona el repositorio `Luka-Final-Project`
5. Render detectará automáticamente el archivo `render.yaml`

### Paso 3: Configurar Variables de Entorno

Render creará automáticamente:
- ✅ Web Service: `lukitas-api`
- ✅ MySQL Database: `lukitas-mysql`

**Variables que se configuran automáticamente:**
- `DATABASE_URL` → Generada por Render (connection string de MySQL)
- `JWT_KEY` → Generada automáticamente (valor aleatorio seguro)
- `JWT_ISSUER` → FinalProjectAPI
- `JWT_AUDIENCE` → FinalProjectClient
- `ASPNETCORE_ENVIRONMENT` → Production

### Paso 4: (Opcional) Configurar Frontend URL

Si tienes un frontend desplegado, agrega esta variable:

1. Ve a tu servicio en Render Dashboard
2. Click en **"Environment"**
3. Agregar variable:
   - **Key**: `FrontendUrl`
   - **Value**: `https://tu-frontend.onrender.com`

### Paso 5: Deploy

1. Click en **"Apply"** o **"Deploy Blueprint"**
2. Render comenzará a:
   - ✅ Crear la base de datos MySQL
   - ✅ Construir la imagen Docker
   - ✅ Desplegar el servicio
   - ✅ Ejecutar migraciones automáticamente

⏱️ **Tiempo estimado**: 5-10 minutos

---

## 🎯 Opción 2: Deployment Manual

### Paso 1: Crear Base de Datos MySQL

1. En Render Dashboard, click **"New +"** → **"MySQL"**
2. Configurar:
   - **Name**: `lukitas-mysql`
   - **Database Name**: `lukitas_db`
   - **User**: `lukitas_user` (generado automáticamente)
   - **Region**: Elige el más cercano
   - **Plan**: Free
3. Click **"Create Database"**
4. **Importante**: Copia el **Internal Connection String**

### Paso 2: Crear Web Service

1. En Render Dashboard, click **"New +"** → **"Web Service"**
2. Conectar repositorio GitHub
3. Configurar:
   - **Name**: `lukitas-api`
   - **Region**: Misma que la base de datos
   - **Branch**: `main`
   - **Runtime**: **Docker**
   - **Dockerfile Path**: `./Dockerfile`
   - **Plan**: Free

### Paso 3: Configurar Variables de Entorno

En la sección **"Environment Variables"**, agregar:

```
DATABASE_URL = [pegar Internal Connection String de MySQL]
JWT_KEY = [generar una clave segura de al menos 32 caracteres]
JWT_ISSUER = FinalProjectAPI
JWT_AUDIENCE = FinalProjectClient
ASPNETCORE_ENVIRONMENT = Production
FrontendUrl = https://tu-frontend.onrender.com (opcional)
```

**Generar JWT_KEY segura**:
```bash
# En PowerShell
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 32 | % {[char]$_})

# En Bash/Linux
openssl rand -base64 32
```

### Paso 4: Deploy

1. Click **"Create Web Service"**
2. Render comenzará el build automáticamente

---

## 📊 Verificar el Deployment

### 1. Verificar que el servicio esté corriendo

```bash
# Tu URL será algo como:
https://lukitas-api.onrender.com

# Test endpoint:
curl https://lukitas-api.onrender.com/
# Respuesta esperada: "API is running"
```

### 2. Verificar Swagger (solo en desarrollo local)

Nota: Swagger está deshabilitado en producción por seguridad.

### 3. Test de Login

```bash
POST https://lukitas-api.onrender.com/api/auth/login
Content-Type: application/json

{
  "email": "admin@test.com",
  "password": "password123"
}
```

### 4. Ver Logs

En Render Dashboard:
1. Click en tu servicio `lukitas-api`
2. Tab **"Logs"**
3. Verás logs en tiempo real

---

## 🔧 Configuración de Hangfire en Producción

Hangfire funcionará automáticamente, pero el dashboard estará deshabilitado en producción por seguridad.

**Para habilitar Hangfire Dashboard en producción** (NO RECOMENDADO sin autenticación):

Edita `Program.cs`:

```csharp
// Cambiar:
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", options);
}

// Por:
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Lukitas - Hangfire Dashboard",
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
```

URL: `https://lukitas-api.onrender.com/hangfire`

---

## 🌍 URLs del Proyecto

Después del deployment, tendrás:

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **API Base** | `https://lukitas-api.onrender.com` | API REST principal |
| **Health Check** | `https://lukitas-api.onrender.com/` | Retorna "API is running" |
| **Auth Login** | `https://lukitas-api.onrender.com/api/auth/login` | Endpoint de autenticación |
| **Base de Datos** | (Interna) | MySQL en Render |

---

## 🚨 Troubleshooting

### Error: "Application failed to start"

**Solución**: Verificar logs en Render Dashboard

Causas comunes:
1. ❌ `DATABASE_URL` no configurada
2. ❌ `JWT_KEY` no configurada
3. ❌ Error en connection string de MySQL

### Error: "Can't connect to MySQL server"

**Solución**:
1. Verificar que la base de datos esté **Running**
2. Usar el **Internal Connection String** (no el External)
3. Verificar que ambos servicios estén en la **misma región**

### Error: CORS

Si tu frontend no puede conectar:

1. Asegúrate de haber configurado `FrontendUrl`
2. Verifica en los logs que se esté agregando el origin correcto
3. Tu frontend debe usar: `https://lukitas-api.onrender.com` (con HTTPS)

### Aplicación muy lenta (Plan Free)

El plan gratuito de Render:
- ⚠️ Se duerme después de 15 minutos de inactividad
- ⚠️ Tarda ~30 segundos en despertar
- ⚠️ 750 horas/mes gratuitas

**Solución**: Upgrade a plan Starter ($7/mes) para instancias siempre activas

---

## 📈 Escalar la Aplicación

### Opción 1: Vertical Scaling (más recursos)

1. En Render Dashboard → Tu servicio
2. **"Settings"** → **"Instance Type"**
3. Cambiar a plan pagado con más CPU/RAM

### Opción 2: Horizontal Scaling (más instancias)

1. En Render Dashboard → Tu servicio
2. **"Settings"** → **"Number of Instances"**
3. Aumentar a 2+ instancias (requiere plan Starter+)

---

## 🔐 Seguridad en Producción

### ✅ Implementado:
- Passwords hasheadas con BCrypt
- JWT para autenticación
- Variables de entorno para secretos
- CORS configurado
- Swagger deshabilitado en producción
- Hangfire Dashboard protegido

### 🔒 Recomendaciones adicionales:

1. **Usar HTTPS**: Render lo proporciona automáticamente
2. **Rate Limiting**: Considerar agregar middleware
3. **API Keys**: Para servicios externos
4. **Logging**: Integrar con servicio de logs externo
5. **Monitoring**: Usar New Relic o similar

---

## 📝 Mantenimiento

### Actualizar la Aplicación

```bash
# 1. Hacer cambios localmente
git add .
git commit -m "feat: nueva funcionalidad"
git push origin main

# 2. Render detectará el push automáticamente y redespleará
```

### Ver Métricas

En Render Dashboard:
- **Metrics**: CPU, RAM, Network usage
- **Logs**: Logs en tiempo real
- **Events**: Historial de deployments

### Backup de Base de Datos

Render no hace backups automáticos en plan gratuito.

**Hacer backup manual**:
1. Conectar con External Connection String
2. Usar `mysqldump`:

```bash
mysqldump -h [host] -u [user] -p lukitas_db > backup.sql
```

---

## 💰 Costos Estimados

### Plan Gratuito (Actual):
- Web Service: **Gratis** (750 horas/mes)
- MySQL Database: **Gratis** (90 días, luego $7/mes)
- **Total**: $0/mes (primeros 90 días)

### Plan Starter (Recomendado para producción):
- Web Service: **$7/mes** (instancia siempre activa)
- MySQL Database: **$7/mes** (backups automáticos)
- **Total**: $14/mes

---

## 🎉 Siguiente Paso

Una vez desplegado, prueba los endpoints principales:

1. **Health Check**: `GET https://lukitas-api.onrender.com/`
2. **Login**: `POST https://lukitas-api.onrender.com/api/auth/login`
3. **Crear campaña**: `POST https://lukitas-api.onrender.com/api/campaign`

**¡Tu API está lista para producción!** 🚀

---

## 📞 Soporte

- **Documentación Render**: https://render.com/docs
- **Render Status**: https://status.render.com
- **Community Forum**: https://community.render.com

---

**Fecha**: 2024-12-02
**Versión API**: 1.0
**Stack**: .NET 9 + MySQL + Hangfire
