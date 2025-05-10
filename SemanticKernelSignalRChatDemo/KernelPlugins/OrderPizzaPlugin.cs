using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SemanticKernelSignalRChatDemo.KernelPlugins
{
    public class OrderPizzaPlugin()
    {
        [KernelFunction("get_pizza_menu")]
        [Description("Returns the pizza menu with available sizes, toppings, and crusts.")]
        [return: Description("Returns the pizza menu with available sizes, toppings, and crusts.")]
        public Menu GetPizzaMenuAsync()
        {
            return new Menu(
                Sizes: Enum.GetValues(typeof(PizzaSize)).Cast<PizzaSize>().ToList(),
                Toppings: Enum.GetValues(typeof(PizzaToppings)).Cast<PizzaToppings>().ToList(),
                Crusts: Enum.GetValues(typeof(PizzaCrust)).Cast<PizzaCrust>().ToList()
            );
        }

        [KernelFunction("add_pizza_to_cart")]
        [Description("Add a pizza to the user's cart; returns the new item and updated cart")]
        [return: Description("Returns the updated cart with the new item added.")]
        public CartDelta AddPizzaToCart(
            PizzaSize size,
            List<PizzaToppings> toppings,
            int quantity = 1,
            string specialInstructions = ""
        )
        {
            Cart.CurrentCart.Pizzas.Add(new Pizza(
                Id: Cart.CurrentCart.Pizzas.Count + 1,
                Size: size,
                Toppings: toppings,
                SpecialInstructions: specialInstructions,
                Price: CalculatePrice(size, toppings, quantity)
            ));

            Cart.CurrentCart.TotalPrice += CalculatePrice(size, toppings, quantity);

            return new CartDelta(
                AddedPizzas: Cart.CurrentCart.Pizzas,
                RemovedPizzas: new List<Pizza>(),
                TotalPrice: Cart.CurrentCart.TotalPrice
            );

            decimal CalculatePrice(PizzaSize size, List<PizzaToppings> toppings, int quantity)
            {
                decimal basePrice = size switch
                {
                    PizzaSize.Small => 8.99m,
                    PizzaSize.Medium => 10.99m,
                    PizzaSize.Large => 12.99m,
                    _ => throw new ArgumentOutOfRangeException(nameof(size), "Invalid pizza size")
                };
                decimal toppingsPrice = toppings.Count * 1.50m;
                return (basePrice + toppingsPrice) * quantity;
            }
        }

        [KernelFunction("remove_pizza_from_cart")]
        [Description("Removes a pizza from the user's cart; returns the updated cart and the removed item.")]
        [return: Description("Returns the updated cart with the item removed.")]
        public RemovePizzaResponse RemovePizzaFromCart(int pizzaId)
        {
            Cart.CurrentCart.Pizzas.RemoveAll(p => p.Id == pizzaId);
            Cart.CurrentCart.TotalPrice -= Cart.CurrentCart.Pizzas.FirstOrDefault(p => p.Id == pizzaId)?.Price ?? 0;

            return new RemovePizzaResponse(
                Success: true,
                Message: $"Pizza with ID {pizzaId} removed from cart."
            );
        }

        [KernelFunction("get_pizza_from_cart")]
        [Description("Returns the specific details of a pizza in the user's cart; use this instead of relying on previous messages since the cart may have changed since then.")]
        [return: Description("Returns the pizza details from the cart.")]
        public Pizza GetPizzaFromCart(int pizzaId)
        {
           return Cart.CurrentCart.Pizzas.FirstOrDefault(p => p.Id == pizzaId) ?? throw new ArgumentException($"Pizza with ID {pizzaId} not found in cart.");
        }

        [KernelFunction("get_cart")]
        [Description("Returns the user's current cart, including the total price and items in the cart.")]
        [return: Description("Returns the current cart with items and total price.")]
        public Cart GetCart()
        {
            return Cart.CurrentCart;
        }

        [KernelFunction("checkout")]
        [Description("Checkouts the user's cart; this function will retrieve the payment from the user and complete the order.")]
        [return: Description("Returns the checkout response with success status and message.")]
        public CheckoutResponse Checkout()
        {
           var price = Cart.CurrentCart.TotalPrice;
            if (price <= 0)
            {
                return new CheckoutResponse(
                    Success: false,
                    Message: "Your cart is empty. Please add items to your cart before checking out."
                ) ;
            }
            // Simulate payment processing
            // In a real-world application, you would integrate with a payment gateway here.
            // Clear the cart after checkout
            Cart.CurrentCart = new Cart { Pizzas = new List<Pizza>(), TotalPrice = 0 };
            return new CheckoutResponse(
                Success: true,
                Message: "Checkout successful! Your order has been placed."
            );
        }
    }


    public record Menu(
        List<PizzaSize> Sizes,
        List<PizzaToppings> Toppings,
        List<PizzaCrust> Crusts
    );

    public record CartDelta(
        List<Pizza> AddedPizzas,
        List<Pizza> RemovedPizzas,
        decimal TotalPrice
    );

    public record RemovePizzaResponse(
        bool Success,
        string Message
    );

    public record Pizza(
        int Id,
        PizzaSize Size,
        List<PizzaToppings> Toppings,
        string SpecialInstructions,
        decimal Price
    );

    public class Cart
    {
        public static Cart CurrentCart { get; set; } = new Cart { Pizzas = new List<Pizza>(), TotalPrice = 0 };
        public List<Pizza> Pizzas { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public record CheckoutResponse(
        bool Success,
        string Message
    );

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PizzaSize
    {
        Small,
        Medium,
        Large
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PizzaToppings
    {
        Pepperoni,
        Mushrooms,
        Onions,
        Sausage,
        Bacon,
        ExtraCheese,
        BlackOlives,
        GreenPeppers,
        Pineapple,
        Spinach
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PizzaCrust
    {
        Thin,
        Thick,
        Stuffed
    }
}
