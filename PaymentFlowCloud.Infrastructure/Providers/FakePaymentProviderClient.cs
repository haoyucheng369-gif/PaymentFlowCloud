using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentFlowCloud.Application.Abstractions;
using PaymentFlowCloud.Application.Contracts;
using PaymentFlowCloud.Domain.Entities;

namespace PaymentFlowCloud.Infrastructure.Providers;

public class FakePaymentProviderClient(
    HttpClient httpClient,
    IOptions<FakePaymentProviderOptions> options,
    ILogger<FakePaymentProviderClient> logger) : IPaymentProviderClient
{
    private readonly FakePaymentProviderOptions _options = options.Value;

    public async Task SubmitPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        // 这里模拟真实 PSP 调用：提交支付后只拿到 Accepted，最终结果等待 webhook。
        var request = new FakeProviderPaymentRequest
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            MerchantOrderId = payment.MerchantOrderId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CorrelationId = payment.CorrelationId,
            WebhookUrl = _options.WebhookUrl
        };

        using var response = await httpClient.PostAsJsonAsync(
            "/provider/payments",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var providerResponse = await response.Content.ReadFromJsonAsync<FakeProviderPaymentResponse>(
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Fake provider accepted payment {PaymentId} with provider payment {ProviderPaymentId}",
            payment.Id,
            providerResponse?.ProviderPaymentId);
    }
}
