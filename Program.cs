using PizzaStore.DB;
using Microsoft.OpenApi.Models;
using PizzaStore;

var builder = WebApplication.CreateBuilder(args);
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "v1" });
});
    
var app = builder.Build();
    
if (app.Environment.IsDevelopment())
{
     app.UseDeveloperExceptionPage();
}
    
app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});
    
app.MapGet("/", () => "Hello World!");
    
app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
app.MapPost("/pizzas", (Pizza pizza) => PizzaDB.CreatePizza(pizza));
app.MapPut("/pizzas", (Pizza pizza) => PizzaDB.UpdatePizza(pizza));
app.MapDelete("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));
app.MapPost("/upload", Uploader.Upload);
app.MapPost("/download", Uploader.Download);
app.MapPost("/getLatestPlaybackName", Uploader.GetLatestPlayback);
app.MapPost("/GetUnSyncMessage", Uploader.GetUnSyncMessage);


app.Run();