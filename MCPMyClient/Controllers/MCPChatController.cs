using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace YourAppNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class McpChatController : ControllerBase
    {
        private readonly IChatClient _chatClient;
        private readonly IMcpClient _mcpClient;

        public McpChatController(IChatClient chatClient, IMcpClient mcpClient)
        {
            _chatClient = chatClient;
            _mcpClient = mcpClient;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return BadRequest("Input cannot be empty.");

            var tools = await _mcpClient.ListToolsAsync();

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are a helpful assistant."),
                new(ChatRole.User, userInput)
            };

            try
            {
                var response = await _chatClient.GetResponseAsync(
                    messages,
                    new ChatOptions { Tools = [.. tools] });

                var assistantMessage = response.Messages
                    .LastOrDefault(m => m.Role == ChatRole.Assistant);

                var output = assistantMessage != null
                    ? string.Join(" ", assistantMessage.Contents.Select(c => c.ToString()))
                    : "(no assistant message received)";

                return Ok(new { answer = output });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("tools")]
        public async Task<IActionResult> ListTools()
        {
            var tools = await _mcpClient.ListToolsAsync();
            return Ok(tools);
        }
    }
}
