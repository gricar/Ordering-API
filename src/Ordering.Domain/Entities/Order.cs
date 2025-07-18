using Ordering.Domain.Abstractions;
using Ordering.Domain.Enums;
using Ordering.Domain.Events;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Entities
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public CustomerId CustomerId { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public decimal TotalPrice
        {
            get => OrderItems.Sum(x => x.Price * x.Quantity);
            private set { }
        }

        public static Order Create(OrderId id, CustomerId customerId)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
                Status = OrderStatus.Pending
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));

            return order;
        }

        public void Update(OrderStatus status)
        {
            Status = status;

            AddDomainEvent(new OrderUpdatedEvent(this));
        }

        public void Add(ProductId productId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Cannot add items to an order that is not in pending status.");
            }

            var orderItem = OrderItem.Create(Id, productId, quantity, price);
            _orderItems.Add(orderItem);
        }

        public void Remove(OrderItemId orderItemId)
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Cannot remove items from an order that is not in pending status.");
            }

            var orderItem = _orderItems.FirstOrDefault(x => x.Id == orderItemId);

            if (orderItem is null)
            {
                throw new InvalidOperationException($"Order item with ID {orderItemId} not found in this order.");
            }

            _orderItems.Remove(orderItem);
        }

        public void Cancel(string justification)
        {
            if (Status == OrderStatus.Accepted || Status == OrderStatus.Preparing || Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException($"Order {Id.Value} cannot be cancelled because its status is '{Status}'. Preparation may have already begun.");
            }

            if (Status == OrderStatus.Canceled)
            {
                throw new InvalidOperationException($"Order {Id.Value} is already cancelled.");
            }

            if (string.IsNullOrWhiteSpace(justification))
            {
                throw new ArgumentException("Cancellation justification cannot be empty.", nameof(justification));
            }

            Update(OrderStatus.Canceled);
            //CancellationJustification = justification;
            //CancelledAt = DateTime.UtcNow;
        }
    }
}
