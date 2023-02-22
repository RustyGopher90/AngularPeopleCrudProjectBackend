using AngularLearningProjectBackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;


namespace AngularLearningProjectBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ILogger<PeopleController> _logger;

        private MyConfig _myconifg;
        public PeopleController(MyConfig myconfig, ILogger<PeopleController> logger)
        {
            _myconifg = myconfig;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            IAsyncEnumerable<People> peeps = AllPeople();
            return Ok(peeps);
        }

        private async IAsyncEnumerable<People> AllPeople()
        {
            await using var connection = new MySqlConnection(_myconifg.ConnectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT FirstName, LastName, DOB, Email, ID FROM people;", connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                yield return new People
                {
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    DOB = reader.GetDateTime("DOB"),
                    Email =  CheckForDBNull(reader),
                    ID = reader.GetInt32("ID"),
                };
        }

        private string CheckForDBNull(MySqlDataReader value)
        {
            if (value.IsDBNull(3)) { return ""; }
            return value.GetString(3);

        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<int>> DeletePerson(int id, [FromBody] People person)
        {
            int rows;
            if (id != person.ID)
            {
                return BadRequest("Bad Request");
            }

            var connection = new MySqlConnection(_myconifg.ConnectionString);
            await connection.OpenAsync();
            MySqlTransaction transaction = await connection.BeginTransactionAsync();

            using var command = new MySqlCommand("delete from people where id= @id;", connection);
            command.Transaction = transaction;
            command.Parameters.AddWithValue("@id", person.ID);
            try
            {
                rows = await command.ExecuteNonQueryAsync();
                if (rows != 1)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Bad Request");
                }
                await transaction.CommitAsync();
                return await Task.FromResult(rows);
            }
            catch (Exception)
            {
                try
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Bad Request");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                    return BadRequest("Bad Request");
                }
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<int>> UpdatePerson(int id, [FromBody]People person)
        {
            int rows;
            if ( id != person.ID)
            {
                return BadRequest("Bad Request");
            }
            var connection = new MySqlConnection(_myconifg.ConnectionString);
            await connection.OpenAsync();
            MySqlTransaction transaction = await connection.BeginTransactionAsync();            

            using var command = new MySqlCommand("update people set FirstName = @firstname, LastName = @lastname, DOB = @dob, Email = @email where id= @id;", connection);
            command.Transaction= transaction;
            command.Parameters.AddWithValue("@firstname", person.FirstName);
            command.Parameters.AddWithValue("@lastname", person.LastName);
            command.Parameters.AddWithValue("@dob", person.DOB);
            command.Parameters.AddWithValue("@email", person.Email);
            command.Parameters.AddWithValue("@id", person.ID);
            try
            {
                rows = await command.ExecuteNonQueryAsync();
                if (rows != 1)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Bad Request");
                }
                await transaction.CommitAsync();
                return await Task.FromResult(rows);
            }
            catch (Exception)
            {
                try
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Bad Request");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                    return BadRequest("Bad Request");
                }
            }                        
        }

        private async IAsyncEnumerable<People> SinglePerson(int id)
        {
            await using var connection = new MySqlConnection(_myconifg.ConnectionString);

            await connection.OpenAsync();


            using var command = new MySqlCommand("SELECT FirstName, LastName, DOB, Email, ID FROM people where id= @id;", connection);
            command.Parameters.AddWithValue("@id", id);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                yield return new People
                {
                    FirstName = reader.GetString(0),
                    LastName = reader.GetString(1),
                    DOB = reader.GetDateTime(2),
                    Email = reader.GetString(3)
                };
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddPerson(People person)
        {
            int rows;
            var connection = new MySqlConnection(_myconifg.ConnectionString);
            await connection.OpenAsync();
            MySqlTransaction transaction = await connection.BeginTransactionAsync();

            using var command = new MySqlCommand("insert into people (FirstName, LastName, DOB, Email) values (@firstname, @lastname, @dob, @email)", connection);
            command.Transaction = transaction;
            command.Parameters.AddWithValue("@firstname", person.FirstName);
            command.Parameters.AddWithValue("@lastname", person.LastName);
            command.Parameters.AddWithValue("@dob", person.DOB);
            command.Parameters.AddWithValue("@email", person.Email);
            try
            {
                rows = await command.ExecuteNonQueryAsync();
                if (rows != 1)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Bad Request");
                }
                await transaction.CommitAsync();
                return await Task.FromResult(rows);
            }
            catch (Exception)
            {
                try
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Bad Request");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                    return BadRequest("Bad Request");
                }
            }
        }


    }
}
