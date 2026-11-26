using mayorista_bebidas.Models;
using Npgsql;

namespace mayorista_bebidas.Services;

public class ClienteService
{
    private readonly string _connectionString;

    public ClienteService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SupabaseConnection")
            ?? throw new InvalidOperationException("No se encontró ConnectionStrings:SupabaseConnection en la configuración.");
    }

    public async Task<List<Cliente>> ObtenerTodosAsync(bool soloActivos = false)
    {
        var clientes = new List<Cliente>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var sql = soloActivos
            ? "SELECT id, nombre, telefono, email, activo, creado_en FROM public.clientes WHERE activo = @activo ORDER BY id DESC;"
            : "SELECT id, nombre, telefono, email, activo, creado_en FROM public.clientes ORDER BY id DESC;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        if (soloActivos)
        {
            cmd.Parameters.AddWithValue("@activo", true);
        }

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            clientes.Add(new Cliente
            {
                Id = reader.GetInt32(0),
                Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                Telefono = reader.IsDBNull(2) ? null : reader.GetString(2),
                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                Activo = !reader.IsDBNull(4) && reader.GetBoolean(4),
                CreadoEn = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5)
            });
        }

        return clientes;
    }
}
