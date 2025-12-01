using AutoMapper;
using Banking.Application.DTOs;
using Banking.Application.DTOs.ReportDtos;
using Banking.Application.DTOs.TransactionBankDtos;
using Banking.Application.Interfaces;
using Banking.Application.Services;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Moq;
namespace Banking.UnitTests.Services
{
    public class TransactionBankServiceTests
    {
        // Mocks de todas las dependencias
        private readonly Mock<ITransactionBankRepository> _mockTransactionRepo;
        private readonly Mock<IAccountRepository> _mockAccountRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPdfService> _mockPdfService;
        private readonly Mock<ICustomerRepository> _mockCustomerRepo;

        // El servicio a probar
        private readonly TransactionBankService _service;

        public TransactionBankServiceTests()
        {
            _mockTransactionRepo = new Mock<ITransactionBankRepository>();
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPdfService = new Mock<IPdfService>();
            _mockCustomerRepo = new Mock<ICustomerRepository>();

            // Instanciamos el servicio inyectando los mocks
            _service = new TransactionBankService(
                _mockTransactionRepo.Object,
                _mockAccountRepo.Object,
                _mockMapper.Object,
                _mockPdfService.Object,
                _mockCustomerRepo.Object,
                _mockAccountRepo.Object // Pasamos el mismo mock para el parámetro duplicado
            );
        }

        #region CreateTransactionAsync Tests

        [Fact]
        public async Task CreateTransaction_Deposit_ShouldIncreaseBalance()
        {
            // ARRANGE
            int accountId = 1;
            var account = new Account { AccountNumber = accountId, InitialBalance = 100m, Status = true };
            var request = new TransactionBankRequestDto { AccountNumber = accountId, Amount = 50m, TransactionType = "Deposito" };

            // Configuramos que la cuenta existe
            _mockAccountRepo.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

            // Configuramos el mapper
            _mockMapper.Setup(m => m.Map<TransactionBank>(request)).Returns(new TransactionBank());
            _mockMapper.Setup(m => m.Map<TransactionBankResponseDto>(It.IsAny<TransactionBank>()))
                       .Returns(new TransactionBankResponseDto());

            // ACT
            await _service.CreateTransactionAsync(request);

            // ASSERT
            // 1. Validar que el saldo subió (100 + 50 = 150)
            Assert.Equal(150m, account.InitialBalance);
            // 2. Validar que se llamó al Update de la cuenta
            _mockAccountRepo.Verify(r => r.UpdateAsync(account), Times.Once);
            // 3. Validar que se guardó la transacción
            _mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<TransactionBank>()), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_Withdrawal_InsufficientFunds_ShouldThrowException()
        {
            // ARRANGE
            var account = new Account { InitialBalance = 50m, Status = true }; // Solo tiene $50
            var request = new TransactionBankRequestDto { AccountNumber = 1, Amount = 100m, TransactionType = "Retiro" }; // Pide $100

            _mockAccountRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(account);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateTransactionAsync(request));
            Assert.Equal("Saldo no disponible.", ex.Message);

            // Validar que NO se guardó nada
            _mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<TransactionBank>()), Times.Never);
        }

        #endregion

        #region GetReportePorFechasAsync Tests

        [Fact]
        public async Task GetReporte_ShouldCalculateInitialBalanceCorrectly()
        {
            // ARRANGE
            // Usamos fechas fijas para evitar problemas de milisegundos en el Mock
            DateTime start = new DateTime(2025, 1, 1);
            DateTime end = new DateTime(2025, 1, 31);

            var transaction = new TransactionBank
            {
                Amount = 200m,
                TransactionType = "Retiro", // Esto funcionará porque el servicio hace ToLower()
                Balance = 800m,
                Date = new DateTime(2025, 1, 15),
                Account = new Account
                {
                    AccountNumber = 1,
                    AccountType = "Ahorros",
                    Status = true,
                    Customer = new Customer { Person = new Person { Name = "Juan" } }
                }
            };

            // Usamos It.IsAny<DateTime> para que el mock responda sin importar la hora exacta
            _mockTransactionRepo.Setup(r => r.GetReportDataAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                                .ReturnsAsync(new List<TransactionBank> { transaction });

            _mockPdfService.Setup(p => p.GenerateStatementPdf(It.IsAny<IEnumerable<ReporteEstadoCuentaDto>>()))
                           .Returns(new byte[0]);

            // ACT
            var result = await _service.GetReportePorFechasAsync(1, start, end);
            var itemReporte = result.ReporteJson.First();

            // ASSERT
            Assert.Equal(1000m, itemReporte.SaldoInicial);
            // Nota: El servicio pone el signo. "-200.00" es correcto si tu servicio usa "-" para retiros.
            Assert.Equal("-200.00", itemReporte.Movimiento);
        }
        #endregion

        #region DeleteTransactionAsync Tests

        [Fact]
        public async Task Delete_Deposit_ShouldDecreaseAccountBalance()
        {
            // ARRANGE
            var account = new Account { InitialBalance = 500m };
            var transaction = new TransactionBank
            {
                TransactionId = 1,
                Amount = 100m,
                TransactionType = "Deposito", // Coincide con el servicio corregido
                Account = account
            };

            _mockTransactionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(transaction);

            // ACT
            await _service.DeleteTransactionAsync(1);

            // ASSERT
            // 500 - 100 = 400. Ahora sí funcionará.
            Assert.Equal(400m, account.InitialBalance);
            _mockTransactionRepo.Verify(r => r.DeleteAsync(transaction), Times.Once);
        }
        #endregion

        #region UpdateTransactionAsync Tests

        [Fact]
        public async Task Update_ChangeDepositAmount_ShouldRevertAndApplyNew()
        {
            // ARRANGE
            // Escenario: Un depósito de $100 (Original) se corrige a $150 (Nuevo).
            // Saldo inicial cuenta: $500.

            var account = new Account { InitialBalance = 500m };
            var oldTransaction = new TransactionBank
            {
                TransactionId = 1,
                Amount = 100m,
                TransactionType = "Deposito",
                Account = account
            };

            var updateRequest = new TransactionBankUpdateDto
            {
                Amount = 150m,
                TransactionType = "Deposito",
                Date = DateTime.Now
            };

            _mockTransactionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(oldTransaction);

            // ACT
            await _service.UpdateTransactionAsync(1, updateRequest);

            // ASSERT
            // Lógica esperada:
            // 1. Revertir viejo ($500 - $100 = $400)
            // 2. Aplicar nuevo ($400 + $150 = $550)
            Assert.Equal(550m, account.InitialBalance);

            // Verificar que se guardaron los cambios
            _mockAccountRepo.Verify(r => r.UpdateAsync(account), Times.Once);
            Assert.Equal(150m, oldTransaction.Amount); // El objeto en memoria se actualizó
        }

        [Fact]
        public async Task Update_Withdrawal_InsufficientFunds_ShouldThrow()
        {
            // ARRANGE
            // Escenario: Querer actualizar un retiro a un monto mayor al disponible.
            var account = new Account { InitialBalance = 100m }; // Saldo actual bajo
            var oldTransaction = new TransactionBank
            {
                TransactionId = 1,
                Amount = 50m,
                TransactionType = "Retiro", // Fue un retiro previo
                Account = account
            };

            // Queremos actualizar el retiro a $200 (Imposible, saldo revertido será $150)
            var updateRequest = new TransactionBankUpdateDto { Amount = 200m, TransactionType = "Retiro" };

            _mockTransactionRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(oldTransaction);

            // ACT & ASSERT
            // Revertir: 100 + 50 = 150 (Saldo temporal)
            // Nuevo: 150 < 200 => Error
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateTransactionAsync(1, updateRequest));
        }

        #endregion
    }
}
