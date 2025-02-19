var builder = WebApplication.CreateBuilder(args);

// Настройка сервисов
builder.Services.AddControllers();
// Регистрация сервиса для пользователей
builder.Services.AddSingleton<UserService>();
builder.Services.AddEndpointsApiExplorer();
// Настройка Swagger для документации API
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Использование Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

// Настройка HTTPS
app.UseHttpsRedirection();
// Настройка маршрутов для контроллеров
app.MapControllers();

// Запуск приложения
app.Run();
