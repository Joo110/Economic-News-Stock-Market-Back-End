using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EconomicNews_BLL;
using EconomicNews_BLL.DTOS;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;

    public EmailController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.ToEmail) ||
            string.IsNullOrWhiteSpace(request.Subject) ||
            string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest(new
            {
                status = 400,
                message = "ToEmail, Subject, and Body are all required."
            });
        }

        await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Body);

        return Ok(new
        {
            status = 200,
            message = "Email sent successfully."
        });
    }
}
