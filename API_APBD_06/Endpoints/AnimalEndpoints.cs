using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using API_APBD_06.DTO;
using API_APBD_06.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API_APBD_06.Endpoints;

public static class AnimalEndpoints
{
    public static void RegisterAnimalEndpoints(this WebApplication app)
    {
        app.MapGet("/animals", (IConfiguration configuration, string orderBy = "") =>
        {

            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "name";
            }
            var allowedColums = new List<string>{"name", "description", "category", "area"};
            if (!allowedColums.Contains(orderBy))
            {
                return Results.BadRequest("Invalid orderBy parametr");
            }
            var animals = new List<GetAllAnimalsResponse>();
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand($"SELECT * FROM Animal ORDER BY {orderBy}", sqlConnection);
                sqlCommand.Connection.Open();
                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    animals.Add(new GetAllAnimalsResponse(reader.GetInt32(0),
                        reader.GetString(1),
                        reader.IsDBNull(2) ? null : reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)));
                }
            }

            return Results.Ok(animals);
        });
        
        
        app.MapPost("animals", (IConfiguration configuration, CreateAnimalsRequest request,  IValidator<CreateAnimalsRequest> validator)=>
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
            
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand($"INSERT INTO Animal (Name, Descripton, Category, Area) VALUES (@name, @ds, @cg, @ar) ", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@name", request.Name);
                sqlCommand.Parameters.AddWithValue("@ds", (object)request.Descripton ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@cg", request.Category);
                sqlCommand.Parameters.AddWithValue("@ar", request.Area);
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
            }

            
            return Results.Created("", null);
        });

        app.MapPut("/animals{id:int}", (IConfiguration configuration, CreateAnimalsRequest request, IValidator<CreateAnimalsRequest> validator, int id) =>
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
            
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand($"UPDATE ANIMAL SET Name = @name, Descripton = @ds, Category = @cg, Area = @ar WHERE IdAnimal = @id", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@name", request.Name);
                sqlCommand.Parameters.AddWithValue("@ds", (object)request.Descripton ?? DBNull.Value);
                sqlCommand.Parameters.AddWithValue("@cg", request.Category);
                sqlCommand.Parameters.AddWithValue("@ar", request.Area);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
            }

            
            return Results.NoContent();
        });


        app.MapDelete("animals", (IConfiguration configuration, int id) =>
        {
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand($"DELETE FROM Animal WHERE IdAnimal = @id", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Connection.Open();
                var sqlDataReader = sqlCommand.ExecuteReader();
                if (!sqlDataReader.Read()) return Results.NotFound();
                sqlDataReader.Close();
                sqlCommand.ExecuteNonQuery();
            }

            return Results.NoContent();
        });
    }
}