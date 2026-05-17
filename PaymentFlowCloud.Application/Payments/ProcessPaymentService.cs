using Microsoft.Extensions.Logging;
using PaymentFlowCloud.Application.Abstractions;

namespace PaymentFlowCloud.Application.Payments;

public class ProcessPaymentService(
    IPaymentRepository paymentRepository,
    ILogger<ProcessPaymentService> logger)
{
    public async Task<bool> MarkProcessingAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        // Worker 只传 PaymentId，真正的状态流转规则由领域对象负责。
        var payment = await paymentRepository.FindByIdAsync(paymentId, cancellationToken);

        if (payment is null)
        {
            logger.LogWarning("Payment {PaymentId} was not found during worker processing", paymentId);
            return false;
        }

        payment.MarkProcessing();

        await paymentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Payment {PaymentId} marked processing after fake provider accepted it",
            payment.Id);

        return true;
    }
}
