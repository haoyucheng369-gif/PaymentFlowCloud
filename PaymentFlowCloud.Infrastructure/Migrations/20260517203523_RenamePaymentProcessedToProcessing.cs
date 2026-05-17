using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentFlowCloud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePaymentProcessedToProcessing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 状态值存成字符串，重命名 enum 后需要同步历史数据。
            migrationBuilder.Sql(
                "UPDATE Payments SET Status = 'Processing' WHERE Status = 'Processed'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 回滚时恢复旧状态名，保证 migration 可逆。
            migrationBuilder.Sql(
                "UPDATE Payments SET Status = 'Processed' WHERE Status = 'Processing'");
        }
    }
}
