using Microsoft.AspNetCore.Mvc;
using SimpleMarketplace.Api.Services;

namespace SimpleMarketplace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest("El mensaje no puede estar vacío.");

            var response = await _chatService.GetAiResponseAsync(request.Message);
            return Ok(new { response });
        }
    }

    public class ChatRequestDto
    {
        public string Message { get; set; } = string.Empty;
    }
}
