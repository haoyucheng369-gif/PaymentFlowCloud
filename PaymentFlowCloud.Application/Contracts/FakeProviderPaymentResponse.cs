namespace PaymentFlowCloud.Application.Contracts;

// fake provider 立即返回的受理结果，不代表最终支付成功。
public class FakeProviderPaymentResponse
{
    // 模拟第三方支付平台自己的支付流水号。
    public string ProviderPaymentId { get; set; } = default!;

    // 第一版只返回 Accepted，最终结果通过 webhook 异步通知。
    public string Status { get; set; } = "Accepted";
}
