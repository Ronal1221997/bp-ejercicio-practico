using AutoMapper;
using Banking.Application.DTOs;
using Banking.Application.DTOs.ReportDtos;
using Banking.Application.DTOs.TransactionBankDtos;
using Banking.Application.Interfaces;
using Banking.Application.Wrappers;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using System.Globalization;

namespace Banking.Application.Services
{


    public class TransactionBankService : ITransactionBankService
    {
        private readonly ITransactionBankRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository; // <--- Nueva dependencia
        private readonly IMapper _mapper;
        private readonly IPdfService _pdfService;
        private readonly ICustomerRepository _customerRepository; // Para obtener datos del cliente
        private readonly IAccountRepository _accountRepository1;

        public TransactionBankService(
            ITransactionBankRepository transactionRepository,
            IAccountRepository accountRepository,
            IMapper mapper,
            IPdfService pdfService,
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository1)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _pdfService = pdfService;
            _customerRepository = customerRepository;
            _accountRepository1 = accountRepository1;
        }

        public async Task<TransactionBankResponseDto> CreateTransactionAsync(TransactionBankRequestDto request)
        {
            // 1. Obtener la cuenta para validar fondos y estado
            var account = await _accountRepository.GetByIdAsync(request.AccountNumber);

            if (account == null)
                throw new KeyNotFoundException($"La cuenta {request.AccountNumber} no existe.");

            if (!account.Status)
                throw new InvalidOperationException("La cuenta no está activa.");

            // 2. Calcular el nuevo saldo basado en el saldo ACTUAL de la cuenta
            // Nota: Usamos 'InitialBalance' como el campo de saldo actual por diseño de la BD dada.
            decimal currentBalance = account.InitialBalance;
            decimal newBalance = currentBalance;

            // Normalizar input
            string type = request.TransactionType.ToLower().Trim();

            if (type == "deposito")
            {
                newBalance += request.Amount;
            }
            else if (type == "retiro")
            {
                // Validación crítica de negocio
                if (currentBalance < request.Amount)
                {
                    throw new InvalidOperationException("Saldo no disponible.");
                }
                newBalance -= request.Amount;
            }
            else
            {
                throw new ArgumentException("Tipo de transacción inválido. Use 'Deposito' o 'Retiro'.");
            }

            // 3. Crear la Entidad de Transacción
            var transaction = _mapper.Map<TransactionBank>(request);
            transaction.Date = DateTime.Now;
            transaction.Balance = newBalance; // Guardamos el saldo resultante en el historial

            // 4. ACTUALIZAR LA CUENTA (Paso Crítico)
            account.InitialBalance = newBalance;

            // 5. Persistencia (Guardamos ambos cambios)
            // En un entorno productivo, esto debería ir dentro de una "Transacción de BD" (UnitOfWork)
            // para asegurar que si falla uno, fallen ambos. Por ahora, lo hacemos secuencial.

            await _transactionRepository.AddAsync(transaction); // Guardar el registro
            await _accountRepository.UpdateAsync(account);      // Actualizar el dinero en la cuenta

            // 6. Retornar respuesta
            return _mapper.Map<TransactionBankResponseDto>(transaction);
        }

        public async Task<PagedResponse<TransactionBankResponseDto>> GetPagedTransactionsAsync(int accountNumber, PaginationFilter filter)
        {
            // 1. Llamar al repositorio obteniendo datos y conteo
            var (entities, totalCount) = await _transactionRepository.GetByAccountNumberPagedAsync(
                accountNumber,
                filter.PageNumber,
                filter.PageSize);

            // 2. Mapear las entidades a DTOs
            var dtos = _mapper.Map<IEnumerable<TransactionBankResponseDto>>(entities);

            // 3. Devolver la respuesta paginada con metadatos
            return new PagedResponse<TransactionBankResponseDto>(
                dtos,
                filter.PageNumber,
                filter.PageSize,
                totalCount);
        }


        public async Task<ReporteFinalDto> GetReportePorFechasAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin)
        {
            // 1. LLAMADA OPTIMIZADA AL REPOSITORIO
            // Ya no buscamos cuentas primero. Buscamos transacciones directas.
            // El repositorio se encarga de filtrar por fecha y decidir si filtra por cliente o trae todo.
            var transactions = await _transactionRepository.GetReportDataAsync(clienteId, fechaInicio, fechaFin);

            // Validación opcional: Si no hay datos, puedes retornar vacío o lanzar error
            if (!transactions.Any())
            {
                // Retornar reporte vacío o lanzar excepción según tu regla de negocio
                // throw new KeyNotFoundException("No se encontraron movimientos en este rango.");
            }

            var listaDatos = new List<ReporteEstadoCuentaDto>();

            // 2. Procesar los datos (Mapeo y Cálculos)
            foreach (var t in transactions)
            {
                // Lógica matemática para obtener Saldo Inicial
                decimal valorNumerico = t.TransactionType.ToLower() == "retiro" ? -t.Amount : t.Amount;
                decimal saldoInicial = t.Balance - valorNumerico;

                // Usamos CultureInfo.InvariantCulture para asegurar que siempre use PUNTOS, sin importar el idioma del servidor
                string movimientoConSigno = (valorNumerico > 0 ? "+" : "") +
                                            valorNumerico.ToString("0.00", CultureInfo.InvariantCulture);

                listaDatos.Add(new ReporteEstadoCuentaDto
                {
                    Fecha = t.Date.ToString("dd/MM/yyyy"),
                    // Obtenemos los nombres gracias a los .Include() del repositorio
                    Cliente = t.Account.Customer.Person.Name,
                    NumeroCuenta = t.Account.AccountNumber.ToString(),
                    Tipo = t.Account.AccountType,
                    SaldoInicial = saldoInicial,
                    Estado = t.Account.Status,
                    Movimiento = movimientoConSigno,
                    SaldoDisponible = t.Balance
                });
            }

            // 3. Ordenar: Primero por Cliente, luego por Fecha
            var listaOrdenada = listaDatos
                .OrderBy(x => x.Cliente)
                .ThenByDescending(x => DateTime.ParseExact(x.Fecha, "dd/MM/yyyy", null))
                .ToList();

            // 4. Generar PDF
            var pdfBytes = _pdfService.GenerateStatementPdf(listaOrdenada);
            var base64Pdf = Convert.ToBase64String(pdfBytes);

            return new ReporteFinalDto
            {
                ReporteJson = listaOrdenada,
                ArchivoPdfBase64 = base64Pdf
            };
        }

        public async Task<PagedResponse<TransactionBankResponseDto>> GetAllTransactionsPagedAsync(PaginationFilter filter)
        {
            var (entities, totalCount) = await _transactionRepository.GetAllPagedAsync(filter.PageNumber, filter.PageSize);

            // Mapeamos a DTO
            var dtos = _mapper.Map<IEnumerable<TransactionBankResponseDto>>(entities);

            // Retornamos la respuesta paginada estándar
            return new PagedResponse<TransactionBankResponseDto>(
                dtos,
                filter.PageNumber,
                filter.PageSize,
                totalCount);
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null) throw new KeyNotFoundException($"Transacción {id} no encontrada.");

            // CORRECCIÓN: Usar "deposito" en español
            if (transaction.TransactionType.ToLower().Trim() == "deposito")
            {
                transaction.Account.InitialBalance -= transaction.Amount;
            }
            else // Asumimos retiro
            {
                transaction.Account.InitialBalance += transaction.Amount;
            }

            await _accountRepository.UpdateAsync(transaction.Account);
            await _transactionRepository.DeleteAsync(transaction);
        }

        public async Task UpdateTransactionAsync(int id, TransactionBankUpdateDto request)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null) throw new KeyNotFoundException($"Transacción {id} no encontrada.");
            var account = transaction.Account;

            // 1. REVERTIR (Usar español)
            if (transaction.TransactionType.ToLower().Trim() == "deposito")
            {
                account.InitialBalance -= transaction.Amount;
            }
            else
            {
                account.InitialBalance += transaction.Amount;
            }

            // 2. APLICAR NUEVO (Usar español)
            string newType = request.TransactionType.ToLower().Trim();

            if (newType == "retiro") // CORRECCIÓN: Usar "retiro"
            {
                if (account.InitialBalance < request.Amount)
                {
                    throw new InvalidOperationException("Saldo no disponible para la actualización.");
                }
                account.InitialBalance -= request.Amount;
            }
            else if (newType == "deposito") // CORRECCIÓN: Usar "deposito"
            {
                account.InitialBalance += request.Amount;
            }
            else
            {
                // Buena práctica: Validar que no llegue basura
                throw new ArgumentException("Tipo de transacción inválido");
            }

            // ... mapeo y guardado ...
            transaction.Date = request.Date;
            transaction.TransactionType = request.TransactionType; // Guardamos como vino, o normalizado
            transaction.Amount = request.Amount;
            transaction.Balance = account.InitialBalance;

            await _accountRepository.UpdateAsync(account);
            await _transactionRepository.UpdateAsync(transaction);
        }


    }
}
