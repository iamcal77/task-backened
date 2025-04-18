using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using MyWebAPI.Services;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class QueryController : ControllerBase
    {
        private readonly QueryService _queryService;
        private readonly HistoryService _historyService;

        public QueryController(QueryService queryService, HistoryService historyService)
        {
            _queryService = queryService;
            _historyService = historyService;
        }

        [HttpPost]
        public async Task<IActionResult> PostQuery([FromBody] QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("Question cannot be empty.");

            var answer = await _queryService.GetLLMResponseAsync(request.Question);

            var response = new QueryResponse
            {
                Question = request.Question,
                Answer = answer
            };

            await _historyService.SaveQueryAsync(response);

            return Ok(response);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var queries = await _historyService.GetAllQueriesAsync();
            return Ok(queries);
        }
        [HttpPut("history/{id}")]
        public async Task<IActionResult> UpdateQuery(int id, [FromBody] QueryResponse updatedResponse)
        {
            var existing = await _historyService.GetQueryByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.Question = updatedResponse.Question;
            existing.Answer = updatedResponse.Answer;

            await _historyService.UpdateQueryAsync(existing);
            return Ok(existing);
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteQuery(int id)
        {
            var success = await _historyService.DeleteQueryAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
        //Get by iD
        [HttpGet("history/{id:int}")]
        public async Task<IActionResult> GetQueryById(int id)
        {
            var query = await _historyService.GetQueryByIdAsync(id);
            if (query == null)
                return NotFound($"Query with ID {id} not found.");

            return Ok(query);
        }


    }
}
