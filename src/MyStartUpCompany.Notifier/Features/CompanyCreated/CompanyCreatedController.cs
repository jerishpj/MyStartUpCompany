using Microsoft.AspNetCore.Mvc;

namespace MyStartUpCompany.Notifier.Features.CompanyCreated
{
    /// <summary>
    /// Company management Notification endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CompanyCreatedController : ControllerBase
    {
        // Post method to receive company created notifications
        [HttpPost]
        public IActionResult Post([FromBody] Models.CompanyCreatedNotification notification)
        {
            // Handle the notification (e.g., log it, send an email, etc.)
            return Ok();
        }
    }
}
