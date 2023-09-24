using M3Tech.OCR.API.PredictionDocument.Data.Models;
using M3Tech.OCR.API.PredictionDocument.Repositories;
using M3Tech.OCR.API.PredictionDocument.Services;
using Microsoft.EntityFrameworkCore;

namespace M3Tech.OCR.API.PredictionDocument
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();

            var Configuration = builder.Configuration;

            builder.Services.AddDbContextFactory<DocgenieContext>(options => options.UseMySQL(Configuration.GetConnectionString("DocgenieDBContext")));

            builder.Services.AddScoped<IDocumentExtractionService, DocumentExtractionService>();
            builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
            builder.Services.AddScoped<IDocumentPredictionService, DocumentPredictionService>();

            var azureStorageConfig = Configuration.GetSection("AzureStorageConfig");
            builder.Services.AddScoped<IAzureStorageService>( x=> new AzureStorageService(azureStorageConfig["AzureStorageConnectionString"], azureStorageConfig["DocgenieContainer"]));


            builder.Services.AddTransient<ICallRepository, CallRepository>();
            builder.Services.AddTransient<ILabelRepository, LabelRepository>();
            builder.Services.AddTransient<IDocumentRepository, DocumentRepository>();
            builder.Services.AddTransient<ICallTypeRepository, CallTypeRepository>();
            builder.Services.AddTransient<IConsumerRepository, ConsumerRepository>();
            builder.Services.AddTransient<IDocumentCategoryRepository, DocumentCategoryRepository>();
            builder.Services.AddTransient<IDocumentCategorizationLogRepository, DocumentCategorizationLogRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}