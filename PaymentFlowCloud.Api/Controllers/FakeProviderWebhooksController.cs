using Microsoft.AspNetCore.Mvc;
using PaymentFlowCloud.Application.Contracts;
using PaymentFlowCloud.Application.Payments;

namespace PaymentFlowCloud.Api.Controllers;

[ApiController]
[Route("webhooks/fake-provider")]
public class FakeProviderWebhooksController(CompletePaymentService completePaymentService) : ControllerBase
{
    [HttpPost("payment-succeeded")]
    public async Task<IActionResult> PaymentSucceeded(
        [FromBody] FakeProviderWebhookRequest request,
        CancellationToken cancellationToken)
    {
        // 第一版 webhook 只模拟支付成功，重复成功回调由应用层和领域状态保持幂等。
        await completePaymentService.CompleteAsync(request, cancellationToken);

        return Ok(new { status = "received" });
    }
}
