using Azure.Data.Tables;
using AzureCosmosApiForTable;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddAzureKeyVault(
//    $"https://{builder.Configuration["KeyVault:Vault"]}.vault.azure.net/", 
//    builder.Configuration["KeyVault:ClientId"], 
//    builder.Configuration["KeyVault:ClientSecret"]);

var app = builder.Build();

app.UseHttpsRedirection();

// New instance of the TableClient class
TableServiceClient tableServiceClient = new TableServiceClient(builder.Configuration.GetConnectionString("AzureCosmosDB"));
//TableServiceClient tableServiceClient = new TableServiceClient(builder.Configuration["azurecosmosdb"]);

app.MapPost("/create", async () =>
{
    // New instance of TableClient class referencing the server-side table
    TableClient tableClient = tableServiceClient.GetTableClient(
        tableName: "adventureworks"
    );

    await tableClient.CreateIfNotExistsAsync();

    // Create new item using composite key constructor
    var prod1 = new Product()
    {
        RowKey = "68719518389",
        PartitionKey = "gear-surf-surfboards",
        Name = "Ocean Surfboard II",
        Quantity = 8,
        Sale = true
    };

    // Add new item to server-side table
    await tableClient.AddEntityAsync<Product>(prod1);
});

app.MapGet("/read-one", async () =>
{
    // New instance of TableClient class referencing the server-side table
    TableClient tableClient = tableServiceClient.GetTableClient(
        tableName: "adventureworks"
    );

    // Read a single item from container
    //var product = await tableClient.GetEntityAsync<Product>(
    //    rowKey: "68719518388",
    //    partitionKey: "gear-surf-surfboards"
    //);

    // Query by using LINQ
    var product = tableClient.QueryAsync<Product>(x => x.RowKey == "68719518388" 
                                                && x.PartitionKey == "gear-surf-surfboards");

    return product;
});

app.MapGet("/read-all", async () =>
{
    // New instance of TableClient class referencing the server-side table
    TableClient tableClient = tableServiceClient.GetTableClient(
        tableName: "adventureworks"
    );

    // Read a single item from container
    var products = tableClient.QueryAsync<Product>(x => x.PartitionKey == "gear-surf-surfboards");
    return products;
});


app.Run();
