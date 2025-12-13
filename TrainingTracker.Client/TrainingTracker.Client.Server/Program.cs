using Microsoft.EntityFrameworkCore;
using TrainingTracker.Client.Server.Data;
using MediatR;
using System.Reflection; // Wymagane dla MediatR
using FluentValidation;
using FluentValidation.AspNetCore;

namespace TrainingTracker.Client.Server // TUTAJ U¯YWAMY POPRAWNEJ NAZWY PRZESTRZENI NAZW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 1. Dodaj politykê CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5173", "https://localhost:5173") // ZEZWÓL na domenê klienta
                              .AllowAnyHeader() // Zezwól na wszystkie nag³ówki (Content-Type, itp.)
                              .AllowAnyMethod(); // Zezwól na wszystkie metody (GET, POST, PUT, DELETE)
                    });
            });

            //koniec CORS
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());


            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}