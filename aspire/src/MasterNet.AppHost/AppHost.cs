using Projects;

var builder = DistributedApplication.CreateBuilder(args);


var password = builder.AddParameter("password", secret: true);

var myParameter = builder.AddParameter("myParameter");

var myConnectionString = builder.AddConnectionString("myConnectionString");


var server = builder.AddSqlServer("server", password, 1433)
    .WithLifetime(ContainerLifetime.Persistent);

var db = server.AddDatabase("MasterNetDB");

var cache = builder.AddRedis("cache")
                .WithRedisCommander()
                .WithLifetime(ContainerLifetime.Persistent);


var ratingService = builder.AddProject<MasterNet_RatingService>("ratingservice")
                        .WithReference(cache)
                        .WaitFor(cache);



var api = builder.AddProject<Projects.MasterNet_WebApi>("api")
           .WithHttpEndpoint(port: 5001)
           .WithReference(db)
           .WithReference(ratingService)
           .WaitFor(db)
           .WaitFor(ratingService);
          
builder.AddProject<Projects.MasterNet_Client>("client")
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

//builder.AddProject<MasterNet_MigrationService>("migration")
//    .WithReference(db)
//    .WaitFor(db)
//    .WithParentRelationship(server);

builder.AddProject<MasterNet_MigrationSQLService>("migrationSQL")
    .WithReference(db)
    .WaitFor(db)
    .WithParentRelationship(server);




builder.Build().Run();
