var builder = DistributedApplication.CreateBuilder(args);

// Thêm CSDL PostgreSQL
//var postgres = builder.AddPostgreSQL("postgres")
//    .WithPgAdmin();

//var writedb = postgres.AddDatabase("writedb");

//// Thêm Redis
//var redis = builder.AddRedis("redis");

// Thêm project WebApi và kết nối nó với các dịch vụ trên
builder.AddProject<Projects.Ecosystem_WebApi>("webapi");
    //.WithReference(writedb)
    //.WithReference(redis);

builder.Build().Run();
