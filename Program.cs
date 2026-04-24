var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>

{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");



var expensesList = new List<Expense>
{
    new Expense { Id = 1, Description = "Groceries", Amount = 150.75m, Date = DateTime.Now.AddDays(-2), Category = ExpenseCategory.Food },
    new Expense { Id = 2, Description = "Gas", Amount = 50.00m, Date = DateTime.Now.AddDays(-1), Category = ExpenseCategory.Transportation },
    new Expense { Id = 3, Description = "Movie Tickets", Amount = 30.00m, Date = DateTime.Now.AddDays(-3), Category = ExpenseCategory.Entertainment },
    new Expense { Id = 4, Description = "Electricity Bill", Amount = 120.00m, Date = DateTime.Now.AddDays(-10), Category = ExpenseCategory.Utilities },
    new Expense { Id = 5, Description = "Doctor Visit", Amount = 200.00m, Date = DateTime.Now.AddDays(-5), Category = ExpenseCategory.Healthcare }
};

app.MapGet("/expenses", () =>
{
    return expensesList;
})
.WithName("GetExpenses");

app.MapPost("/expenses/add", (Expense expense) =>
{
expensesList.Add(expense);
}).WithName("AddExpense");

app.MapPut("/expenses/update/{id}", (int id, Expense updatedExpense) =>
{
    var expenseToUpdate = expensesList.FirstOrDefault(e => e.Id == id);

    if (expenseToUpdate == null)
    {
        return Results.NotFound();
    }

    expenseToUpdate.Description = updatedExpense.Description;
    expenseToUpdate.Amount = updatedExpense.Amount;
    expenseToUpdate.Date = updatedExpense.Date;
    expenseToUpdate.Category = updatedExpense.Category;

    return Results.Ok(expenseToUpdate);
}).WithName("UpdateExpense");

app.MapDelete("/expenses/delete/{id}", (int id) =>
{
    var expenseToRemove = expensesList.FirstOrDefault(e => e.Id == id);
    if (expenseToRemove != null)
    {
        expensesList.Remove(expenseToRemove);
    }
}).WithName("DeleteExpense");

app.MapPost("/auth/login", (LoginDto loginDto) =>
{
    return (loginDto.username == "admin" && loginDto.password == "123456") ? Results.Ok("Login successful") : Results.Unauthorized();
}).WithName("login");

app.Run();

public class Expense
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public ExpenseCategory Category { get; set; }
}

public enum ExpenseCategory
{
    Food,
    Transportation,
    Entertainment,
    Utilities,
    Healthcare,
    Other
}

public class LoginDto
{
    public string username { get; set; }
    public string password { get; set; }
}