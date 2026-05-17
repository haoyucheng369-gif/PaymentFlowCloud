using PaymentFlowCloud.Application.Contracts;
using Serilog;
using Serilog.Context;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    // ProviderMock 也写入 Seq，方便按 CorrelationId 查看 API -> Worker -> Provider -> Webhook。
    loggerConfiguration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341");
});

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapPost("/provider/payments", async (
    FakeProviderPaymentRequest request,
    IHttpClientFactory httpClientFactory,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) =>
{
    using var correlationScope = LogContext.PushProperty("CorrelationId", request.CorrelationId);
    using var paymentScope = LogContext.PushProperty("PaymentId", request.PaymentId);

    var logger = loggerFactory.CreateLogger("PaymentFlowCloud.ProviderMock");
    var providerPaymentId = $"fp_{Guid.NewGuid():N}";

    logger.LogInformation(
        "Fake provider accepted payment {PaymentId} with provider payment {ProviderPaymentId}",
        request.PaymentId,
        providerPaymentId);

    _ = Task.Run(async () =>
    {
        try
        {
            using var callbackCorrelationScope = LogContext.PushProperty("CorrelationId", request.CorrelationId);
            using var callbackPaymentScope = LogContext.PushProperty("PaymentId", request.PaymentId);

            // 模拟第三方支付平台异步处理后主动回调商户 webhook。
            await Task.Delay(TimeSpan.FromSeconds(3), CancellationToken.None);

            var webhook = new FakeProviderWebhookRequest
            {
                PaymentId = request.PaymentId,
                ProviderPaymentId = providerPaymentId,
                Status = "Succeeded",
                CorrelationId = request.CorrelationId
            };

            var client = httpClientFactory.CreateClient();
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, request.WebhookUrl)
            {
                Content = JsonContent.Create(webhook)
            };
            httpRequest.Headers.TryAddWithoutValidation("X-Correlation-Id", request.CorrelationId);

            using var response = await client.SendAsync(httpRequest, CancellationToken.None);
            response.EnsureSuccessStatusCode();

            logger.LogInformation(
                "Fake provider sent succeeded webhook for payment {PaymentId}",
                request.PaymentId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Fake provider failed to send webhook for payment {PaymentId}",
                request.PaymentId);
        }
    }, CancellationToken.None);

    return Results.Ok(new FakeProviderPaymentResponse
    {
        ProviderPaymentId = providerPaymentId,
        Status = "Accepted"
    });
});

app.Run();
