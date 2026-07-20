using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.BLL.ViewModels;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeXErpSystem.BLL.ViewModels.Payments;

namespace CodeXErpSystem.BLL.Services.Classes
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentViewModel>> GetAllAsync()
        {
            var entities = await _unitOfWork.GetRepository<Payment>().FindAsync(includeProperties: "Customer,Supplier");
            return _mapper.Map<IEnumerable<PaymentViewModel>>(entities);
        }

        public async Task CreateAsync(PaymentViewModel model)
        {
            var entity = _mapper.Map<Payment>(model);
            
            // Prevent AutoMapper from creating empty navigation properties
            entity.Customer = null;
            entity.Supplier = null;
            
            // Auto-generate Numeric Receipt Number
            var existingPayments = await _unitOfWork.GetRepository<Payment>().GetAll(false);
            var maxNumber = existingPayments.Any() ? existingPayments.Max(p => int.TryParse(p.ReceiptNumber, out int n) ? n : 0) : 1000;
            entity.ReceiptNumber = (maxNumber + 1).ToString();

            // Adjust Balance
            if (entity.CustomerId.HasValue)
            {
                var customer = await _unitOfWork.GetRepository<Customer>().GetById(entity.CustomerId.Value);
                if (customer != null)
                {
                    customer.Balance = (customer.Balance ?? 0) - entity.Amount;
                    _unitOfWork.GetRepository<Customer>().Update(customer);
                }
            }
            else if (entity.SupplierId.HasValue)
            {
                var supplier = await _unitOfWork.GetRepository<Supplier>().GetById(entity.SupplierId.Value);
                if (supplier != null)
                {
                    supplier.Balance = (supplier.Balance ?? 0) + entity.Amount;
                    _unitOfWork.GetRepository<Supplier>().Update(supplier);
                }
            }

            _unitOfWork.GetRepository<Payment>().Add(entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(PaymentViewModel model)
        {
            var existingPayment = await _unitOfWork.GetRepository<Payment>().GetById(model.Id);
            if (existingPayment == null) return;

            // Revert old amount
            if (existingPayment.CustomerId.HasValue)
            {
                var customer = await _unitOfWork.GetRepository<Customer>().GetById(existingPayment.CustomerId.Value);
                if (customer != null)
                {
                    customer.Balance = (customer.Balance ?? 0) + existingPayment.Amount;
                    
                    // Apply new amount
                    if (model.CustomerId == existingPayment.CustomerId)
                    {
                        customer.Balance -= model.Amount;
                    }
                    _unitOfWork.GetRepository<Customer>().Update(customer);
                }
            }
            else if (existingPayment.SupplierId.HasValue)
            {
                var supplier = await _unitOfWork.GetRepository<Supplier>().GetById(existingPayment.SupplierId.Value);
                if (supplier != null)
                {
                    supplier.Balance = (supplier.Balance ?? 0) - existingPayment.Amount; // Revert by subtracting the old amount
                    
                    // Apply new amount
                    if (model.SupplierId == existingPayment.SupplierId)
                    {
                        supplier.Balance += model.Amount; // Apply by adding the new amount
                    }
                    _unitOfWork.GetRepository<Supplier>().Update(supplier);
                }
            }

            // If the user changed the customer or supplier entirely (rare but possible)
            if (model.CustomerId != existingPayment.CustomerId && model.CustomerId.HasValue)
            {
                 var newCustomer = await _unitOfWork.GetRepository<Customer>().GetById(model.CustomerId.Value);
                 if(newCustomer != null)
                 {
                     newCustomer.Balance = (newCustomer.Balance ?? 0) - model.Amount;
                     _unitOfWork.GetRepository<Customer>().Update(newCustomer);
                 }
            }
            else if (model.SupplierId != existingPayment.SupplierId && model.SupplierId.HasValue)
            {
                 var newSupplier = await _unitOfWork.GetRepository<Supplier>().GetById(model.SupplierId.Value);
                 if(newSupplier != null)
                 {
                     newSupplier.Balance = (newSupplier.Balance ?? 0) + model.Amount; // Apply by adding the new amount
                     _unitOfWork.GetRepository<Supplier>().Update(newSupplier);
                 }
            }

            // Keep the same receipt number
            model.ReceiptNumber = existingPayment.ReceiptNumber;

            // Update entity properties
            _mapper.Map(model, existingPayment);
            
            // Prevent AutoMapper from creating empty navigation properties
            existingPayment.Customer = null;
            existingPayment.Supplier = null;
            
            _unitOfWork.GetRepository<Payment>().Update(existingPayment);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existingPayment = await _unitOfWork.GetRepository<Payment>().GetById(id);
            if (existingPayment != null)
            {
                if (existingPayment.CustomerId.HasValue)
                {
                    var customer = await _unitOfWork.GetRepository<Customer>().GetById(existingPayment.CustomerId.Value);
                    if (customer != null)
                    {
                        customer.Balance = (customer.Balance ?? 0) + existingPayment.Amount;
                        _unitOfWork.GetRepository<Customer>().Update(customer);
                    }
                }
                else if (existingPayment.SupplierId.HasValue)
                {
                    var supplier = await _unitOfWork.GetRepository<Supplier>().GetById(existingPayment.SupplierId.Value);
                    if (supplier != null)
                    {
                        supplier.Balance = (supplier.Balance ?? 0) - existingPayment.Amount; // Revert by subtracting the old amount
                        _unitOfWork.GetRepository<Supplier>().Update(supplier);
                    }
                }
                _unitOfWork.GetRepository<Payment>().Delete(id);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
