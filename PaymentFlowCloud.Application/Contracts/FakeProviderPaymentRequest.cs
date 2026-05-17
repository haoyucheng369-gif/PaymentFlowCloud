namespace PaymentFlowCloud.Application.Contracts;

// Worker 调用 fake provider 时发送的支付请求。
public class FakeProviderPaymentRequest
{
    // 本系统内部支付技术 ID，用来让 webhook 回调时定位 Payment。
    public Guid PaymentId { get; set; }

    // 订单技术 ID，用来串起 Order -> Payment -> Webhook。
    public Guid? OrderId { get; set; }

    // 商户订单号，模拟真实业务系统里的外部订单号。
    public string MerchantOrderId { get; set; } = default!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "EUR";

    // 贯穿 API、Worker、Provider、Webhook 的链路 ID。
    public string CorrelationId { get; set; } = default!;

    // fake provider 异步处理完成后要回调的 API webhook 地址。
    public string WebhookUrl { get; set; } = default!;
}
