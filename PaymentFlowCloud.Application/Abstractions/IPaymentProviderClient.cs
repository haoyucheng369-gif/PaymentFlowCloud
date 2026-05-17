using PaymentFlowCloud.Domain.Entities;

namespace PaymentFlowCloud.Application.Abstractions;

// 应用层只关心“提交支付到外部提供方”，具体 HTTP 调用放在 Infrastructure。
public interface IPaymentProviderClient
{
    Task SubmitPaymentAsync(Payment payment, CancellationToken cancellationToken = default);
}
