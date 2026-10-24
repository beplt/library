using library.Models;
using Npgsql;

namespace library.Data;

// Every SQL statement about Members lives in this one class.
public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    // ASP.NET Core hands in the data source that Program.cs registered.
    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // Copies the current row of the reader into a Member.
    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            MemberType = reader.GetString(3)
        };
    }

    // READ: every Member, sorted by name.
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql = "SELECT Member_id, full_name, email, member_type FROM lending.Member ORDER BY full_name;";

        var Members = new List<Member>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            Members.Add(ReadMember(reader));
        }

        return Members;
    }
    
}