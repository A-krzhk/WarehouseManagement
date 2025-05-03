using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Core.Services;

 public class InventoryOperationService : IInventoryOperationService
    {
        private readonly IGenericRepository<InventoryOperation> _operationRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly ApplicationDbContext _dbContext;

        public InventoryOperationService(
            IGenericRepository<InventoryOperation> operationRepository,
            IGenericRepository<Product> productRepository,
            ApplicationDbContext dbContext)
        {
            _operationRepository = operationRepository;
            _productRepository = productRepository;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<InventoryOperationDto>> GetAllOperationsAsync()
        {
            var operations = await _dbContext.InventoryOperations
                .Include(o => o.Product)
                .Include(o => o.User)
                .AsNoTracking()
                .ToListAsync();

            return operations.Select(o => new InventoryOperationDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                ProductName = o.Product?.Name ?? string.Empty,
                Quantity = o.Quantity,
                Type = o.Type,
                UserId = o.UserId,
                UserName = $"{o.User?.FirstName} {o.User?.LastName}",
                Timestamp = o.Timestamp,
                Notes = o.Notes
            });
        }

        public async Task<InventoryOperationDto?> GetOperationByIdAsync(int id)
        {
            var operation = await _dbContext.InventoryOperations
                .Include(o => o.Product)
                .Include(o => o.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (operation == null)
                return null;

            return new InventoryOperationDto
            {
                Id = operation.Id,
                ProductId = operation.ProductId,
                ProductName = operation.Product?.Name ?? string.Empty,
                Quantity = operation.Quantity,
                Type = operation.Type,
                UserId = operation.UserId,
                UserName = $"{operation.User?.FirstName} {operation.User?.LastName}",
                Timestamp = operation.Timestamp,
                Notes = operation.Notes
            };
        }

        public async Task<InventoryOperationDto> CreateOperationAsync(CreateInventoryOperationDto operationDto, string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var product = await _dbContext.Products.FindAsync(operationDto.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {operationDto.ProductId} not found.");
                
                if (operationDto.Type == OperationType.Incoming)
                {
                    product.Quantity += operationDto.Quantity;
                }
                else 
                {
                    if (product.Quantity < operationDto.Quantity)
                        throw new InvalidOperationException($"Not enough stock for product {product.Name}. Current stock: {product.Quantity}");
                    
                    product.Quantity -= operationDto.Quantity;
                }

                await _productRepository.UpdateAsync(product);
                
                var operation = new InventoryOperation
                {
                    ProductId = operationDto.ProductId,
                    Quantity = operationDto.Quantity,
                    Type = operationDto.Type,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    Notes = operationDto.Notes
                };

                await _operationRepository.AddAsync(operation);

                await transaction.CommitAsync();

                var user = await _dbContext.Users.FindAsync(userId);

                return new InventoryOperationDto
                {
                    Id = operation.Id,
                    ProductId = operation.ProductId,
                    ProductName = product.Name,
                    Quantity = operation.Quantity,
                    Type = operation.Type,
                    UserId = operation.UserId,
                    UserName = $"{user?.FirstName} {user?.LastName}",
                    Timestamp = operation.Timestamp,
                    Notes = operation.Notes
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<InventoryOperationDto>> GetOperationsByProductIdAsync(int productId)
        {
            var operations = await _dbContext.InventoryOperations
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();

            return operations.Select(o => new InventoryOperationDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                ProductName = o.Product?.Name ?? string.Empty,
                Quantity = o.Quantity,
                Type = o.Type,
                UserId = o.UserId,
                UserName = $"{o.User?.FirstName} {o.User?.LastName}",
                Timestamp = o.Timestamp,
                Notes = o.Notes
            });
        }

        public async Task<IEnumerable<InventoryOperationDto>> GetOperationsByUserIdAsync(string userId)
        {
            var operations = await _dbContext.InventoryOperations
                .Include(o => o.Product)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return operations.Select(o => new InventoryOperationDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                ProductName = o.Product?.Name ?? string.Empty,
                Quantity = o.Quantity,
                Type = o.Type,
                UserId = o.UserId,
                UserName = $"{o.User?.FirstName} {o.User?.LastName}",
                Timestamp = o.Timestamp,
                Notes = o.Notes
            });
        }
    }