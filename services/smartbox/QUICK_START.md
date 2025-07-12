# ?? SMARTBOX USER SYNC - QUICK START GUIDE

## ?? **Prerequisites**

- **Docker & Docker Compose**: ?? ch?y PostgreSQL, RabbitMQ
- **.NET 9 SDK**: ?? build và run services
- **Identity Service**: Running t?i https://localhost:7004/
- **Auth Server**: Running t?i https://localhost:7600/

## ? **Quick Setup (5 minutes)**

### **1. Clone & Build**
```bash
cd services/smartbox
dotnet build
```

### **2. Start Infrastructure**
```bash
# Start PostgreSQL và RabbitMQ
docker-compose up -d postgres rabbitmq

# Wait for services to be ready
docker-compose logs -f postgres rabbitmq
```

### **3. Update Configuration**
```bash
# Update appsettings.json if needed
services/smartbox/host/Ecosystem.SmartBox.HttpApi.Host/appsettings.json
```

### **4. Run Database Migration**
```bash
cd shared/Ecosystem.DbMigrator
dotnet run
```

### **5. Start SmartBox Service**
```bash
cd services/smartbox/host/Ecosystem.SmartBox.HttpApi.Host
dotnet run
```

## ?? **Service URLs**

- **SmartBox API**: https://localhost:7003/
- **Swagger UI**: https://localhost:7003/swagger
- **Health Check**: https://localhost:7003/health

## ?? **Test the Integration**

### **1. Create User with Sync**
```bash
curl -X POST "https://localhost:7003/api/smartbox/users" \
-H "Authorization: Bearer YOUR_JWT_TOKEN" \
-H "Content-Type: application/json" \
-d '{
  "userName": "testuser",
  "email": "test@example.com",
  "password": "Password123!",
  "fullName": "Test User",
  "phoneNumber": "+84123456789",
  "isActive": true,
  "requirePasswordChange": true
}'
```

### **2. Check Health Status**
```bash
curl https://localhost:7003/health
```

### **3. Monitor Logs**
```bash
# Check SmartBox logs
docker-compose logs -f smartbox-api

# Check RabbitMQ management UI
http://localhost:15672/ (guest/guest)
```

## ?? **Common Issues & Solutions**

### **Issue: Identity Service Connection Failed**
```bash
# Check Identity Service is running
curl https://localhost:7004/health

# Update RemoteServices:IdentityService:BaseUrl in appsettings.json
```

### **Issue: RabbitMQ Connection Failed**
```bash
# Check RabbitMQ is running
docker-compose ps

# Restart RabbitMQ if needed
docker-compose restart rabbitmq
```

### **Issue: Database Connection Failed**
```bash
# Check PostgreSQL is running
docker-compose ps

# Run migration again
cd shared/Ecosystem.DbMigrator && dotnet run
```

## ?? **Monitoring Dashboard**

### **RabbitMQ Management**
- URL: http://localhost:15672/
- User: guest / guest
- Check queues: `SmartBox` queue should exist

### **Application Logs**
```bash
# Real-time logs
docker-compose logs -f smartbox-api

# Search for sync events
docker-compose logs smartbox-api | grep "sync"
```

### **Health Checks**
```json
GET /health
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "rabbitmq": "Healthy"
  }
}
```

## ?? **Testing Scenarios**

### **1. User Creation Flow**
```
1. Create user via API ? Should create in both SmartBox & Identity
2. Check user exists in Identity Service
3. Verify UserCreatedIntegrationEvent published
```

### **2. User Update Flow**
```
1. Update user via API ? Should sync to Identity Service
2. Verify changes reflected in Identity Service
3. Verify UserUpdatedIntegrationEvent published
```

### **3. Event Handling Flow**
```
1. Publish IdentityUserSyncedIntegrationEvent manually
2. Verify user created/updated in SmartBox
3. Check retry mechanism if sync fails
```

## ?? **API Examples**

### **Get Current User**
```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
https://localhost:7003/api/smartbox/users/current
```

### **Update User Status**
```bash
curl -X PUT -H "Authorization: Bearer YOUR_JWT_TOKEN" \
"https://localhost:7003/api/smartbox/users/USER_ID/set-active?isActive=false"
```

### **Check if Username Exists**
```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
"https://localhost:7003/api/smartbox/users/check-username?userName=testuser"
```

## ?? **Troubleshooting**

### **Enable Debug Logging**
```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Ecosystem.SmartBox": "Debug",
      "Volo.Abp.EventBus": "Debug"
    }
  }
}
```

### **Check Event Bus**
```bash
# Verify RabbitMQ queues
docker exec -it rabbitmq rabbitmqctl list_queues

# Check exchange bindings
docker exec -it rabbitmq rabbitmqctl list_bindings
```

### **Database Queries**
```sql
-- Check users in SmartBox
SELECT * FROM "SmartBoxUsers" ORDER BY "CreationTime" DESC;

-- Check integration events
SELECT * FROM "AbpEventBus" ORDER BY "CreationTime" DESC;
```

## ?? **Support**

### **Documentation**
- [Full Implementation Guide](./FINAL_IMPLEMENTATION_GUIDE.md)
- [ABP Framework Docs](https://docs.abp.io/)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)

### **Common Commands**
```bash
# Restart all services
docker-compose restart

# View all logs
docker-compose logs

# Clean rebuild
dotnet clean && dotnet build

# Reset database
docker-compose down -v && docker-compose up -d postgres
```

---

**?? You're ready to go! The SmartBox User Sync system is fully operational.**