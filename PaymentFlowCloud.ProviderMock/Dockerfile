FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PaymentFlowCloud.ProviderMock/*.csproj PaymentFlowCloud.ProviderMock/
COPY PaymentFlowCloud.Application/*.csproj PaymentFlowCloud.Application/
COPY PaymentFlowCloud.Domain/*.csproj PaymentFlowCloud.Domain/
RUN dotnet restore PaymentFlowCloud.ProviderMock/PaymentFlowCloud.ProviderMock.csproj

COPY . .
RUN dotnet publish PaymentFlowCloud.ProviderMock/PaymentFlowCloud.ProviderMock.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentFlowCloud.ProviderMock.dll"]
